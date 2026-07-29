#nullable enable
// File: BackdropManager.cs

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WinRT;

namespace PortoPattern.Themes;

public sealed class BackdropManager : IDisposable
{
    private readonly Window _window;

    private DesktopAcrylicController? _acrylicController;
    private readonly SystemBackdropConfiguration _configurationSource = new();
    private ICompositionSupportsSystemBackdrop? _backdropTarget;

    private bool _isDisposed;

    public BackdropManager(Window window)
    {
        _window = window;
    }

    public void Initialize()
    {
        if (_isDisposed) return;

        try
        {
            if (Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread() == null)
            {
                return;
            }

            _window.Activated += OnWindowActivated;
            _window.Closed += OnWindowClosed;

            _configurationSource.IsInputActive = true;

            if (DesktopAcrylicController.IsSupported())
            {
                _acrylicController = new DesktopAcrylicController();
                _backdropTarget = _window.As<ICompositionSupportsSystemBackdrop>();

                _acrylicController.AddSystemBackdropTarget(_backdropTarget);
                _acrylicController.SetSystemBackdropConfiguration(_configurationSource);
            }

            ApplyColors();
        }
        catch (Exception)
        {
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isDisposed) return;
        _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args) => Dispose();

    private void ApplyColors()
    {
        if (_isDisposed) return;

        try
        {
            var color = ColorManager.GetSurfaceColor();
            var opacity = ColorManager.BackdropTintOpacity;

            if (_acrylicController != null)
            {
                _acrylicController.TintColor = color;
                _acrylicController.FallbackColor = color;
                _acrylicController.TintOpacity = opacity;
            }
            else if (_window.Content is FrameworkElement element)
            {
                var brush = new SolidColorBrush(color);
                if (element is Control control) control.Background = brush;
                else if (element is Panel panel) panel.Background = brush;
            }
        }
        catch (Exception)
        {
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            _window.Activated -= OnWindowActivated;
            _window.Closed -= OnWindowClosed;

            if (_acrylicController != null)
            {
                _acrylicController.Dispose();
                _acrylicController = null;
            }
            _backdropTarget = null;
        }
        catch (Exception)
        {
        }
    }
}