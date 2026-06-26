// ****************************************************************************
// Файл: CardButton.cs
// Описание: Расширенная кнопка-карточка. Самостоятельно управляет аппаратной
//           анимацией масштабирования (GPU) при наведении с помощью хелпера Fluidity.
// ****************************************************************************

#nullable enable

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

/// <summary>
/// Расширенная кнопка с поддержкой кастомных цветов для состояний наведения
/// и встроенной автоматической анимацией увеличения.
/// </summary>
public class CardButton : Button
{
    private Visual? _cardVisual;

    private static readonly Vector3 HoverScale = new(1.04f, 1.04f, 1.0f);
    private static readonly Vector3 NormalScale = Vector3.One;

    #region [Dependency Properties]

    public static readonly DependencyProperty PointerOverBackgroundCustomProperty =
        DependencyProperty.Register(nameof(PointerOverBackgroundCustom), typeof(Brush), typeof(CardButton), new PropertyMetadata(null));

    public Brush PointerOverBackgroundCustom
    {
        get => (Brush)GetValue(PointerOverBackgroundCustomProperty);
        set => SetValue(PointerOverBackgroundCustomProperty, value);
    }

    public static readonly DependencyProperty PointerOverBorderBrushCustomProperty =
        DependencyProperty.Register(nameof(PointerOverBorderBrushCustom), typeof(Brush), typeof(CardButton), new PropertyMetadata(null));

    public Brush PointerOverBorderBrushCustom
    {
        get => (Brush)GetValue(PointerOverBorderBrushCustomProperty);
        set => SetValue(PointerOverBorderBrushCustomProperty, value);
    }

    public static readonly DependencyProperty PointerOverForegroundCustomProperty =
        DependencyProperty.Register(nameof(PointerOverForegroundCustom), typeof(Brush), typeof(CardButton), new PropertyMetadata(null));

    public Brush PointerOverForegroundCustom
    {
        get => (Brush)GetValue(PointerOverForegroundCustomProperty);
        set => SetValue(PointerOverForegroundCustomProperty, value);
    }

    #endregion

    public CardButton()
    {
        this.Loaded += OnCardButtonLoaded;
        this.Unloaded += OnCardButtonUnloaded;
        this.PointerEntered += OnCardButtonPointerEntered;
        this.PointerExited += OnCardButtonPointerExited;
    }

    private void OnCardButtonLoaded(object sender, RoutedEventArgs e)
    {
        _cardVisual = ElementCompositionPreview.GetElementVisual(this);
        Compositor compositor = _cardVisual.Compositor;

        _cardVisual.BindCenterPoint();

        var easeOut = compositor.CreatePennerEquation(PennerType.CubicEaseOut);

        var implicitAnimations = _cardVisual.ImplicitAnimations ?? compositor.CreateImplicitAnimationCollection();
        implicitAnimations.CreateImplicitAnimation("Scale", TimeSpan.FromMilliseconds(200), easeOut);

        _cardVisual.ImplicitAnimations = implicitAnimations;

        AnimateParentLayoutTransition(compositor);
    }

    private void OnCardButtonUnloaded(object sender, RoutedEventArgs e)
    {
        // Очищаем только ссылку на визуал для сборщика мусора
        _cardVisual = null;
    }

    private void AnimateParentLayoutTransition(Compositor compositor)
    {
        if (this.Parent is UIElement parentElement)
        {
            var parentVisual = ElementCompositionPreview.GetElementVisual(parentElement);

            // Используем переименованный EaseInOut
            var layoutEase = compositor.CreatePennerEquation(PennerType.EaseInOut);

            var animations = parentVisual.ImplicitAnimations ?? compositor.CreateImplicitAnimationCollection();
            animations.CreateImplicitAnimation("Offset", TimeSpan.FromMilliseconds(320), layoutEase);

            parentVisual.ImplicitAnimations = animations;
        }
    }

    private void OnCardButtonPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (_cardVisual is null) return;
        _cardVisual.Scale = HoverScale;
    }

    private void OnCardButtonPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (_cardVisual is null) return;
        _cardVisual.Scale = NormalScale;
    }
}