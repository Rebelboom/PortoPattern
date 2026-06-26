// File: NavigationService.cs
// Description: Implementation of INavigationService. Manages the navigation stack, 
// transitions, and synchronization of the view model lifecycle (INavigable).
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.Navigation;

internal readonly record struct NavigationEntry(Type VmType, object? Param);

public sealed class NavigationService : INavigationService, IDisposable
{
    private readonly IViewFactory _factory;
    private readonly Stack<NavigationEntry> _history = new();

    private Frame? _frame;
    private object? _currentVm;
    private object? _currentParam;

    private readonly SemaphoreSlim _gate = new(1, 1);
    private CancellationTokenSource? _cts;
    private bool _disposed;

    public bool CanGoBack => _history.Count > 0;

    public NavigationService(IViewFactory factory)
        => _factory = factory;

    public void Initialize(Frame frame)
    {
        if (_frame != null)
        {
            var error = "NavigationService already initialized";
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] NavigationService: {error}");
#endif
            throw new InvalidOperationException(error);
        }

        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
    }

    public Task NavigateToAsync<TViewModel>(object? parameter = null, CancellationToken ct = default)
        where TViewModel : class
        => ExecuteAsync(typeof(TViewModel), parameter, false, ct);

    public Task GoBackAsync(CancellationToken ct = default)
    {
        if (!CanGoBack)
            return Task.CompletedTask;

        var entry = _history.Peek();
        return ExecuteAsync(entry.VmType, entry.Param, true, ct);
    }

    private async Task ExecuteAsync(Type targetType, object? param, bool isBack, CancellationToken ext)
    {
        await _gate.WaitAsync(ext);

        var oldCts = _cts;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ext);
        oldCts?.Cancel();
        oldCts?.Dispose();

        var token = _cts.Token;

        try
        {
            if (_frame == null)
                throw new InvalidOperationException("Frame not initialized");

            if (isBack)
            {
                if (_history.Count == 0) return;
            }
            else
            {
                if (_currentVm?.GetType() == targetType) return;
            }

            var page = _factory.Create(targetType);
            var vm = page.DataContext
                ?? throw new InvalidOperationException($"DataContext is null on page {page.GetType().Name}");

            if (_currentVm is INavigable oldNav)
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, timeout.Token);

                try
                {
                    await oldNav.OnNavigatedFromAsync(linked.Token);
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine($"[DEBUG ERROR] Navigation suspend error (OnNavigatedFromAsync): {e.Message}");
#endif
                }
            }

            if (isBack)
                _history.Pop();
            else if (_currentVm != null)
                _history.Push(new NavigationEntry(_currentVm.GetType(), _currentParam));

            var oldPage = _frame.Content as Page;
            var oldVm = _currentVm;

            _currentVm = vm;
            _currentParam = param;

            _frame.ContentTransitions = new TransitionCollection
            {
                new NavigationThemeTransition
                {
                    DefaultNavigationTransitionInfo = isBack
                        ? new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromLeft }
                        : new EntranceNavigationTransitionInfo()
                }
            };

            _frame.Content = page;

            // Очистка старой страницы для предотвращения утечек памяти
            if (oldPage != null)
            {
                oldPage.DataContext = null;
            }

            if (oldVm is IDisposable d)
            {
                d.Dispose();
            }

            if (vm is INavigable nav)
            {
                try
                {
                    await nav.OnNavigatedToAsync(param, token);
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine($"[DEBUG ERROR] Navigation resume error (OnNavigatedToAsync): {e.Message}");
#endif
                }
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _gate.Dispose();
        _cts?.Dispose();
        _disposed = true;
    }

    // TODO: Добавить проверку на наличие циклической навигации (Navigation Cache).
    // TODO: Реализовать блокировку навигации, если текущая ViewModel возвращает 'false' в методе CanNavigateFrom (защита от потери несохраненных данных).
}