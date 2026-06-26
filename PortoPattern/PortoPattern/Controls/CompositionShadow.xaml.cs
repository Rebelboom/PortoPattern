// ****************************************************************************
// Файл: CompositionShadow.xaml.cs
// Описание: Улучшенный контрол аппаратных теней. 
// Исправлены ошибки именования свойств и предупреждения о null-значениях.
// Адаптировано под .NET 10 / C# 14.
// ****************************************************************************

#nullable enable

using System;
using System.Numerics;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;
using System.Diagnostics;

namespace PortoPattern.Controls;

/// <summary>
/// Контрол для создания динамических теней через Composition API.
/// Поддерживает автоматическое извлечение AlphaMask для текста, фигур и картинок.
/// </summary>
[ContentProperty(Name = nameof(CastingElement))]
public sealed partial class CompositionShadow : UserControl
{
    // =========================================================================
    // ПРИВАТНЫЕ ПОЛЯ
    // =========================================================================

    // Используем null!, чтобы подавить CS8618, так как инициализация в конструкторе гарантирована
    private readonly DropShadow _dropShadow = null!;
    private readonly SpriteVisual _shadowVisual = null!;

    // =========================================================================
    // СВОЙСТВА ЗАВИСИМОСТИ (Dependency Properties)
    // =========================================================================

    public static readonly DependencyProperty CastingElementProperty = DependencyProperty.Register(
        nameof(CastingElement), typeof(FrameworkElement), typeof(CompositionShadow),
        new PropertyMetadata(null, (d, e) =>
            ((CompositionShadow)d).OnCastingElementChanged(e.OldValue as FrameworkElement, e.NewValue as FrameworkElement)));

    public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
        nameof(BlurRadius), typeof(double), typeof(CompositionShadow),
        new PropertyMetadata(16.0, (d, e) =>
            ((CompositionShadow)d)._dropShadow.BlurRadius = (float)(double)e.NewValue));

    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color), typeof(Color), typeof(CompositionShadow),
        new PropertyMetadata(Microsoft.UI.Colors.Black, (d, e) =>
            ((CompositionShadow)d)._dropShadow.Color = (Color)e.NewValue));

    public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(
        nameof(OffsetX), typeof(double), typeof(CompositionShadow),
        new PropertyMetadata(0.0, (d, e) => ((CompositionShadow)d).UpdateShadowOffset()));

    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(
        nameof(OffsetY), typeof(double), typeof(CompositionShadow),
        new PropertyMetadata(0.0, (d, e) => ((CompositionShadow)d).UpdateShadowOffset()));

    public static readonly DependencyProperty ShadowOpacityProperty = DependencyProperty.Register(
        nameof(ShadowOpacity), typeof(double), typeof(CompositionShadow),
        new PropertyMetadata(1.0, (d, e) =>
            ((CompositionShadow)d)._dropShadow.Opacity = (float)(double)e.NewValue));

    // =========================================================================
    // ИНИЦИАЛИЗАЦИЯ
    // =========================================================================

    public CompositionShadow()
    {
        try
        {
            this.InitializeComponent();

            // Создание визуальных объектов композиции
            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _shadowVisual = compositor.CreateSpriteVisual();
            _dropShadow = compositor.CreateDropShadow();
            _shadowVisual.Shadow = _dropShadow;

            // Привязка визуального слоя к элементу Border в XAML
            ElementCompositionPreview.SetElementChildVisual(ShadowElement, _shadowVisual);

            // Подписки на жизненный цикл
            this.SizeChanged += (_, _) => UpdateShadowSize();
            this.Loaded += (_, _) => ConfigureShadowVisual();
        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод критических ошибок инициализации в консоль отладки
            Debug.WriteLine($"[CompositionShadow Init Error]: {ex.Message}");
#endif
        }
    }

    // =========================================================================
    // АКСЕССОРЫ
    // =========================================================================

    public FrameworkElement? CastingElement
    {
        get => (FrameworkElement?)GetValue(CastingElementProperty);
        set => SetValue(CastingElementProperty, value);
    }

    public double BlurRadius { get => (double)GetValue(BlurRadiusProperty); set => SetValue(BlurRadiusProperty, value); }
    public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
    public double OffsetX { get => (double)GetValue(OffsetXProperty); set => SetValue(OffsetXProperty, value); }
    public double OffsetY { get => (double)GetValue(OffsetYProperty); set => SetValue(OffsetYProperty, value); }
    public double ShadowOpacity { get => (double)GetValue(ShadowOpacityProperty); set => SetValue(ShadowOpacityProperty, value); }

    // =========================================================================
    // ЛОГИКА КОМПОЗИЦИИ (Update Engine)
    // =========================================================================

    private void OnCastingElementChanged(FrameworkElement? oldElement, FrameworkElement? newElement)
    {
        // Управление событиями для предотвращения утечек памяти
        if (oldElement is not null) oldElement.SizeChanged -= OnCastingElementSizeChanged;
        if (newElement is not null) newElement.SizeChanged += OnCastingElementSizeChanged;

        ConfigureShadowVisual();
    }

    private void OnCastingElementSizeChanged(object sender, SizeChangedEventArgs e) => UpdateShadowSize();

    private void ConfigureShadowVisual()
    {
        UpdateShadowMask();
        UpdateShadowSize();
        UpdateShadowOffset();
    }

    /// <summary>
    /// Извлекает AlphaMask из элемента, чтобы тень повторяла его форму (текст, картинка).
    /// </summary>
    private void UpdateShadowMask()
    {
        if (CastingElement is null)
        {
            _dropShadow.Mask = null;
            return;
        }

        try
        {
            _dropShadow.Mask = CastingElement switch
            {
                Image img => img.GetAlphaMask(),
                Shape shp => shp.GetAlphaMask(),
                TextBlock txt => txt.GetAlphaMask(),
                _ => null
            };
        }
        catch
        {
            _dropShadow.Mask = null;
        }
    }

    private void UpdateShadowSize()
    {
        // Синхронизация размеров спрайта тени с CastingElement
        Vector2 newSize = CastingElement is not null
            ? new((float)CastingElement.ActualWidth, (float)CastingElement.ActualHeight)
            : new((float)ActualWidth, (float)ActualHeight);

        _shadowVisual.Size = newSize;
    }

    private void UpdateShadowOffset()
    {
        // Применяем смещение тени по осям X и Y
        _dropShadow.Offset = new Vector3((float)OffsetX, (float)OffsetY, 0f);
    }
}