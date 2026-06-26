// File: ViewFactory.cs
// Description: Concrete implementation of IViewFactory and IViewRegistry. 
// Uses IServiceProvider to resolve Page types and maintains a mapping between ViewModels and Views.
#nullable enable
using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.Navigation.Services;

public sealed class ViewFactory : IViewRegistry, IViewFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, Type> _map = new();

    public ViewFactory(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public void Register<TViewModel, TView>()
        where TViewModel : class
        where TView : Page
    {
        if (!_map.TryAdd(typeof(TViewModel), typeof(TView)))
        {
            var error = $"ViewModel {typeof(TViewModel).Name} is already registered to a View.";
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] ViewFactory: {error}");
#endif
            throw new InvalidOperationException(error);
        }
    }

    public Page Create(Type viewModelType)
    {
        if (!_map.TryGetValue(viewModelType, out var viewType))
        {
            var error = $"No view registered for {viewModelType.Name}.";
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] ViewFactory: {error}");
#endif
            throw new InvalidOperationException(error);
        }

        try
        {
            var page = (Page)_serviceProvider.GetRequiredService(viewType);
            return page;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] ViewFactory resolution failed: {ex.Message}");
#endif
            throw;
        }
    }

    // TODO: Реализовать поддержку Scoped-контейнеров для страниц, чтобы ViewModel жила только пока открыта страница.
}