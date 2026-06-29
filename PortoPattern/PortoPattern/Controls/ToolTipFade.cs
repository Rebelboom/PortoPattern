using System;
using System.Diagnostics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;

namespace PortoPattern;

public sealed partial class ToolTipFade : ToolTip
{
    private Visual? _visual;
    private Compositor? _compositor;
    private ScalarKeyFrameAnimation? _fadeInAnimation;
    private ScalarKeyFrameAnimation? _fadeOutAnimation;

    /// <summary>
    /// Время появления тултипа в миллисекундах.
    /// </summary>
    public static readonly DependencyProperty FadeTimeProperty = DependencyProperty.Register(
        nameof(FadeTime),
        typeof(double),
        typeof(ToolTipFade),
        new PropertyMetadata(250d, OnFadeTimeChanged));

    public double FadeTime
    {
        get => (double)GetValue(FadeTimeProperty);
        set => SetValue(FadeTimeProperty, value);
    }

    private static void OnFadeTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ToolTipFade ctrl)
        {
            ctrl.UpdateAnimations();
        }
    }

    public ToolTipFade()
    {
        // Оставляем дефолтный стиль, чтобы не ломать шаблон WinUI 3
        DefaultStyleKey = typeof(ToolTip);

        Opened += OnOpened;
        Closed += OnClosed;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Настройки по умолчанию
        Placement = Microsoft.UI.Xaml.Controls.Primitives.PlacementMode.Mouse;
        Opacity = 0.0;

        // Инициализируем Composition API на основе визуального дерева элемента
        _visual = ElementCompositionPreview.GetElementVisual(this);
        _compositor = _visual.Compositor;

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (_compositor is null) return;

        // Анимация появления (Fade In)
        _fadeInAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _fadeInAnimation.InsertKeyFrame(0f, 0f);
        _fadeInAnimation.InsertKeyFrame(1f, 1f);
        _fadeInAnimation.Duration = TimeSpan.FromMilliseconds(FadeTime);
        _fadeInAnimation.Target = "Opacity";

        // Анимация затухания (Fade Out) — делаем чуть быстрее для отзывчивости
        _fadeOutAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _fadeOutAnimation.InsertKeyFrame(0f, 1f);
        _fadeOutAnimation.InsertKeyFrame(1f, 0f);
        _fadeOutAnimation.Duration = TimeSpan.FromMilliseconds(FadeTime * 0.75);
        _fadeOutAnimation.Target = "Opacity";
    }

    private void OnOpened(object sender, RoutedEventArgs e)
    {
        if (_visual is null || _fadeInAnimation is null) return;

        _visual.StopAnimation("Opacity");
        _visual.StartAnimation("Opacity", _fadeInAnimation);
    }

    private void OnClosed(object sender, RoutedEventArgs e)
    {
        if (_visual is null || _fadeOutAnimation is null) return;

        _visual.StopAnimation("Opacity");

        // Используем Scoped Batch, чтобы красиво затушить тултип на Render-потоке
        var batch = _compositor?.CreateScopedBatch(CompositionBatchTypes.Animation);

        _visual.StartAnimation("Opacity", _fadeOutAnimation);

        if (batch is not null)
        {
            batch.Completed += (_, _) =>
            {
#if DEBUG
                Debug.WriteLine("[INFO] ToolTipFade out animation completed completely.");
#endif
            };
            batch.End();
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new ToolTipFadeAutomationPeer(this);
}

/// <summary>
/// Поддержка UI Automation для кастомного тултипа.
/// </summary>
public class ToolTipFadeAutomationPeer(ToolTipFade control) : FrameworkElementAutomationPeer(control)
{
    protected override string GetClassNameCore() => nameof(ToolTipFade);

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;

    protected override string GetNameCore() =>
        (Owner as ToolTipFade)?.Name is { Length: > 0 } name ? name : base.GetNameCore();
}