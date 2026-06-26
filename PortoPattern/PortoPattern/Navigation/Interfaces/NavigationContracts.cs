// File: NavigationContracts.cs
// Description: Defines core contracts for navigation, including the lifecycle of view models (INavigable) 
// and the main navigation service interface for WinUI 3.

#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace PortoPattern.Navigation.Interfaces;

public interface INavigable
{
    Task OnNavigatedToAsync(object? parameter, CancellationToken ct);
    Task OnNavigatedFromAsync(CancellationToken ct);
}

public interface INavigationService
{
    bool CanGoBack { get; }
    void Initialize(Frame frame);

    Task NavigateToAsync<TViewModel>(object? parameter = null, CancellationToken ct = default)
        where TViewModel : class;

    Task GoBackAsync(CancellationToken ct = default);
}

public interface IViewFactory
{
    Page Create(Type viewModelType);
}

public interface IViewRegistry
{
    void Register<TViewModel, TView>()
        where TViewModel : class
        where TView : Page;
}

public interface IViewFor<out TViewModel> where TViewModel : class
{
    TViewModel ViewModel { get; }
}