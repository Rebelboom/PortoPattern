using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace PortoPattern;

public sealed partial class ToolTipFade : ToolTip
{
    #region [Props]
    private Storyboard? _fadeInStoryboard;
    private Storyboard? _fadeOutStoryboard;

    /// <summary>
    /// The time it takes for the tooltip to fade in and out, in milliseconds.
    /// </summary>
    public static readonly DependencyProperty FadeTimeProperty = DependencyProperty.Register(
         nameof(FadeTime),
         typeof(double),
         typeof(ToolTipFade),
         new PropertyMetadata(300d, OnFadeTimeChanged));

    public double FadeTime
    {
        get => (double)GetValue(FadeTimeProperty);
        set => SetValue(FadeTimeProperty, value);
    }

    private static void OnFadeTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ToolTipFade ctrl)
        {
            ctrl.ChangeFadeTime((double)e.NewValue);
        }
    }

    private void ChangeFadeTime(double value)
    {
#if DEBUG
        Debug.WriteLine($"[INFO] FadeTime is now {value} ms");
#endif
        InitializeAnimations();
    }
    #endregion

    public ToolTipFade()
    {
        DefaultStyleKey = typeof(ToolTip);

        InitializeAnimations();

        Opened += OnOpened;
        Closed += OnClosed;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Opacity = 0.0;
        Placement = Microsoft.UI.Xaml.Controls.Primitives.PlacementMode.Mouse;
        Padding = new Thickness(0);
        BorderThickness = new Thickness(0);
        Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new ToolTipFadeAutomationPeer(this);

    private void InitializeAnimations()
    {
        #region [Fade In]
        _fadeInStoryboard = new Storyboard();
        var fadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(FadeTime),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeInAnimation, this);
        Storyboard.SetTargetProperty(fadeInAnimation, "Opacity");

        _fadeInStoryboard.Children.Add(fadeInAnimation);
        #endregion

        #region [Fade Out]
        _fadeOutStoryboard = new Storyboard();
        var fadeOutAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(FadeTime / 2),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeOutAnimation, this);
        Storyboard.SetTargetProperty(fadeOutAnimation, "Opacity");

        _fadeOutStoryboard.Children.Add(fadeOutAnimation);
        _fadeOutStoryboard.Completed -= FadeOutStoryboardOnCompleted;
        _fadeOutStoryboard.Completed += FadeOutStoryboardOnCompleted;
        #endregion
    }

    private void FadeOutStoryboardOnCompleted(object? sender, object e)
    {
#if DEBUG
        Debug.WriteLine("[INFO] FadeOutStoryboard was completed.");
#endif
        Visibility = Visibility.Collapsed;
    }

    private void OnOpened(object sender, object e)
    {
        Visibility = Visibility.Visible;
        _fadeInStoryboard?.Begin();
    }

    private void OnClosed(object sender, object e)
    {
#if DEBUG
        Debug.WriteLine("[INFO] FadeOutStoryboard was started.");
#endif
        _fadeOutStoryboard?.Begin();
    }
}

/// <summary>
/// Support for UI automation.
/// </summary>
public class ToolTipFadeAutomationPeer(ToolTipFade control) : FrameworkElementAutomationPeer(control)
{
    protected override string GetClassNameCore() => nameof(ToolTipFade);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;

    protected override string GetNameCore() =>
        (Owner as ToolTipFade)?.Name is { Length: > 0 } name ? name : base.GetNameCore();
}