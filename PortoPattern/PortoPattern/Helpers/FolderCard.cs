using System;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Hosting;
using PortoPattern.Helpers;

namespace PortoPattern.Controls;

public class FolderCard : ContentControl
{
    private Visual? _borderVisual;

    public static readonly DependencyProperty PointerOverBackgroundCustomProperty =
        DependencyProperty.Register(nameof(PointerOverBackgroundCustom), typeof(Brush), typeof(FolderCard), new PropertyMetadata(null));

    public static readonly DependencyProperty PointerOverBorderBrushCustomProperty =
        DependencyProperty.Register(nameof(PointerOverBorderBrushCustom), typeof(Brush), typeof(FolderCard), new PropertyMetadata(null));

    public static readonly DependencyProperty PointerOverForegroundCustomProperty =
        DependencyProperty.Register(nameof(PointerOverForegroundCustom), typeof(Brush), typeof(FolderCard), new PropertyMetadata(null));

    public static readonly DependencyProperty SearchTextProperty =
        DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(FolderCard), new PropertyMetadata(string.Empty));

    public Brush PointerOverBackgroundCustom { get => (Brush)GetValue(PointerOverBackgroundCustomProperty); set => SetValue(PointerOverBackgroundCustomProperty, value); }
    public Brush PointerOverBorderBrushCustom { get => (Brush)GetValue(PointerOverBorderBrushCustomProperty); set => SetValue(PointerOverBorderBrushCustomProperty, value); }
    public Brush PointerOverForegroundCustom { get => (Brush)GetValue(PointerOverForegroundCustomProperty); set => SetValue(PointerOverForegroundCustomProperty, value); }
    public string SearchText { get => (string)GetValue(SearchTextProperty); set => SetValue(SearchTextProperty, value); }

    public FolderCard()
    {
        this.DefaultStyleKey = typeof(FolderCard);

        this.PointerEntered += OnFolderCardPointerEntered;
        this.PointerExited += OnFolderCardPointerExited;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("CardBorder") is FrameworkElement cardBorder)
        {
            _borderVisual = ElementCompositionPreview.GetElementVisual(cardBorder);
            Compositor compositor = _borderVisual.Compositor;

            _borderVisual.BindCenterPoint();

            var easeOut = compositor.CreatePennerEquation(PennerType.CubicEaseOut);

            var implicitAnimations = _borderVisual.ImplicitAnimations ?? compositor.CreateImplicitAnimationCollection();
            implicitAnimations.CreateImplicitAnimation("Scale", TimeSpan.FromMilliseconds(200), easeOut);

            _borderVisual.ImplicitAnimations = implicitAnimations;
        }

        if (this.Parent is UIElement parentElement)
        {
            var parentVisual = ElementCompositionPreview.GetElementVisual(parentElement);
            var layoutEase = parentVisual.Compositor.CreatePennerEquation(PennerType.EaseInOut);

            var animations = parentVisual.ImplicitAnimations ?? parentVisual.Compositor.CreateImplicitAnimationCollection();
            animations.CreateImplicitAnimation("Offset", TimeSpan.FromMilliseconds(450), layoutEase);

            parentVisual.ImplicitAnimations = animations;
        }
    }

    private void OnFolderCardPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "PointerOver", true);

        if (_borderVisual is null) return;
        _borderVisual.Scale = new Vector3(1.04f, 1.04f, 1.0f);
    }

    private void OnFolderCardPointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Normal", true);

        if (_borderVisual is null) return;
        _borderVisual.Scale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}