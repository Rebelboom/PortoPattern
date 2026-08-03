#nullable enable

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.ViewModels;
using System;

namespace PortoPattern.Dialogs;

public sealed partial class AboutDialog : ContentDialog
{
    // NOTE: Строго типизированное свойство для работы x:Bind в XAML
    public AboutDialogViewModel? ViewModel => DataContext as AboutDialogViewModel;

    public AboutDialog()
    {
        try
        {
            InitializeComponent();

            // NOTE: Обновляем привязки, когда DialogFactory устанавливает DataContext
            DataContextChanged += (s, e) => Bindings.Update();

            Loaded += AboutDialogOnLoaded;
            Unloaded += AboutDialogOnUnloaded;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] AboutDialog constructor failed: {ex.Message}");
#endif
        }
    }

    private void AboutDialogOnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            StoryboardSpin?.Begin();
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] Storyboard begin failed: {ex.Message}");
#endif
        }
    }

    private void AboutDialogOnUnloaded(object sender, RoutedEventArgs e)
    {
        try
        {
            StoryboardSpin?.Stop();
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] Storyboard stop failed: {ex.Message}");
#endif
        }
    }
}