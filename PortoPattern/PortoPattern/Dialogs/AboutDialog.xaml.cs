#nullable enable

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PortoPattern.Helpers;
using System;

namespace PortoPattern.Dialogs;

public sealed partial class AboutDialog : ContentDialog
{
    public AboutDialog()
    {
        try
        {
            this.InitializeComponent();

            this.Loaded += AboutDialogOnLoaded;
            this.Unloaded += AboutDialogOnUnloaded;
            this.Opened += DialogOnOpened;
            this.GotFocus += OnGotFocus;
            this.LostFocus += OnLostFocus;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] AboutDialog constructor failed: {ex.Message}");
#endif
        }
    }

    private void OnGotFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            // Используются те же цвета свечения, что и в старом проекте
            BloomHelper.AddBloom((UIElement)imgLevel, (UIElement)cdGrid, Windows.UI.Color.FromArgb(230, 11, 203, 239), 12);
            BloomHelper.AddBloom((UIElement)tbTitle, (UIElement)cdStack, Windows.UI.Color.FromArgb(255, 255, 255, 255), 8);
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] AddBloom failed: {ex.Message}");
#endif
        }
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            BloomHelper.RemoveBloom((UIElement)imgLevel, (UIElement)cdGrid, null);
            BloomHelper.RemoveBloom((UIElement)tbTitle, (UIElement)cdStack, null);
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] RemoveBloom failed: {ex.Message}");
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

    private void DialogOnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        try
        {
            if (App.Current.Resources.TryGetValue("CardAcrylicBrush", out object brushObj) && brushObj is Microsoft.UI.Xaml.Media.Brush UIBrush)
            {
                this.Background = UIBrush;
            }
            else
            {
                // Резервный программный вариант полностью синхронизирован со значениями CardAcrylicBrush темы Coral Mint (#EA580C)
                this.Background = new AcrylicBrush
                {
                    TintOpacity = 0.25,
                    TintLuminosityOpacity = 0.15,
                    TintColor = Windows.UI.Color.FromArgb(255, 234, 88, 12),
                    FallbackColor = Windows.UI.Color.FromArgb(255, 234, 88, 12)
                };
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] DialogOnOpened background setup failed: {ex.Message}");
#endif
        }
    }
}