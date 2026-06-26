#nullable enable
using Microsoft.UI; // Для Microsoft.UI.Colors
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Color = Windows.UI.Color;

namespace PortoPattern.Controls;

/// <summary>
/// Пользовательский элемент управления для отображения текста с возможностью подсветки искомых фрагментов.
/// </summary>
public sealed partial class HighlightTextBlock : UserControl
{
    /*
    ============================================================================
    DEPENDENCY PROPERTIES
    ============================================================================
    */

    /// <summary>
    /// Свойство зависимости для основного текста.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(HighlightTextBlock),
            new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// Свойство зависимости для текста, который необходимо подсветить.
    /// </summary>
    public static readonly DependencyProperty HighlightTextProperty =
        DependencyProperty.Register(
            nameof(HighlightText),
            typeof(string),
            typeof(HighlightTextBlock),
            new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// Свойство зависимости для управления переносом текста.
    /// </summary>
    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(
            nameof(TextWrapping),
            typeof(TextWrapping),
            typeof(HighlightTextBlock),
            new PropertyMetadata(TextWrapping.Wrap, OnTextWrappingChanged));

    /// <summary>
    /// Свойство зависимости для размера шрифта.
    /// </summary>
    public static new readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register(
            nameof(FontSize),
            typeof(double),
            typeof(HighlightTextBlock),
            new PropertyMetadata(14.0, OnFontSizeChanged));

    /// <summary>
    /// Свойство зависимости для кисти фона подсветки.
    /// Метаданные инициализируются null, чтобы избежать разделяемого мутабельного 
    /// состояния (shared mutable state) между всеми экземплярами контрола.
    /// </summary>
    public static readonly DependencyProperty HighlightBackgroundProperty =
        DependencyProperty.Register(
            nameof(HighlightBackground),
            typeof(Brush),
            typeof(HighlightTextBlock),
            new PropertyMetadata(null, OnHighlightBrushChanged));

    /// <summary>
    /// Свойство зависимости для кисти текста подсветки.
    /// </summary>
    public static readonly DependencyProperty HighlightForegroundProperty =
        DependencyProperty.Register(
            nameof(HighlightForeground),
            typeof(Brush),
            typeof(HighlightTextBlock),
            new PropertyMetadata(null, OnHighlightBrushChanged));

    /*
    ============================================================================
    CLR WRAPPERS
    ============================================================================
    */

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string HighlightText
    {
        get => (string)GetValue(HighlightTextProperty);
        set => SetValue(HighlightTextProperty, value);
    }

    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public new double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public Brush HighlightBackground
    {
        get => (Brush)GetValue(HighlightBackgroundProperty);
        set => SetValue(HighlightBackgroundProperty, value);
    }

    public Brush HighlightForeground
    {
        get => (Brush)GetValue(HighlightForegroundProperty);
        set => SetValue(HighlightForegroundProperty, value);
    }

    /*
    ============================================================================
    PRIVATE FIELDS
    ============================================================================
    */

    // Единственный инстанс TextHighlighter переиспользуется для всего контрола.
    private readonly TextHighlighter _highlighter = new();

    /*
    ============================================================================
    CTOR
    ============================================================================
    */

    public HighlightTextBlock()
    {
        InitializeComponent();

        // Инициализируем кисти для каждого экземпляра индивидуально,
        // предотвращая изменение состояния у других контролов.
        InitializeDefaultBrushes();
    }

    /*
    ============================================================================
    DP CALLBACKS
    ============================================================================
    */

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is HighlightTextBlock control)
        {
            control.UpdateHighlights();
        }
    }

    private static void OnTextWrappingChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is HighlightTextBlock control &&
            control.PART_TextBlock is not null)
        {
            control.PART_TextBlock.TextWrapping = (TextWrapping)e.NewValue;
        }
    }

    private static void OnFontSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is HighlightTextBlock control &&
            control.PART_TextBlock is not null)
        {
            control.PART_TextBlock.FontSize = (double)e.NewValue;
        }
    }

    private static void OnHighlightBrushChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is HighlightTextBlock control)
        {
            // Прямая передача кисти в _highlighter без промежуточных трансформаций.
            if (e.Property == HighlightBackgroundProperty)
            {
                control._highlighter.Background = e.NewValue as Brush;
            }
            else if (e.Property == HighlightForegroundProperty)
            {
                control._highlighter.Foreground = e.NewValue as Brush;
            }

            control.UpdateHighlights();
        }
    }

    /*
    ============================================================================
    BRUSH INITIALIZATION
    ============================================================================
    */

    private void InitializeDefaultBrushes()
    {
        try
        {
            // 1. Инициализация фона.
            // Создаем уникальный экземпляр Transparent-кисти для данного контрола,
            // чтобы избежать shared mutable state из статических метаданных DP.
            if (HighlightBackground is null)
            {
                HighlightBackground = new SolidColorBrush(Colors.Transparent);
            }

            // 2. Инициализация цвета текста.
            // Если цвет уже передан из XAML через биндинг или стиль - не трогаем.
            if (HighlightForeground is not null)
            {
                return;
            }

            var resources = Application.Current.Resources;
            Brush? fgBrush = null;

            // Пытаемся найти специфичный ресурс для приложения.
            if (resources.TryGetValue("Porto.Highlight.Foreground", out object foregroundObj)
                && foregroundObj is Brush foregroundBrush)
            {
                fgBrush = foregroundBrush;
            }
            // Фолбэк на системный акцентный цвет ОС.
            else if (resources.TryGetValue("SystemAccentColor", out object accentObj))
            {
                Color accentColor = Colors.Blue; // Safe default

                if (accentObj is Color c)
                    accentColor = c;
                else if (accentObj is SolidColorBrush b)
                    accentColor = b.Color;

                fgBrush = new SolidColorBrush(accentColor);
            }
            // Последняя линия защиты - дефолтный синий.
            else
            {
                fgBrush = new SolidColorBrush(Color.FromArgb(255, 0, 120, 215));
            }

            HighlightForeground = fgBrush;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"[HighlightTextBlock Brushes Init Error]: {ex.Message}");
#endif
            // Жесткий фолбэк при любой ошибке.
            HighlightForeground = new SolidColorBrush(Colors.Black);
        }
    }

    /*
    ============================================================================
    HIGHLIGHT UPDATE
    ============================================================================
    */

    private void UpdateHighlights()
    {
        try
        {
            var tb = PART_TextBlock;
            if (tb is null) return;

            string text = Text ?? string.Empty;
            string query = HighlightText ?? string.Empty;

            tb.Text = text;

            tb.TextHighlighters.Clear();
            _highlighter.Ranges.Clear();

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(query))
            {
                return;
            }

            tb.TextHighlighters.Add(_highlighter);

            // Использование ReadOnlySpan<char> исключает выделение памяти 
            // под новые строки при поиске подстроки (Zero-allocation).
            ReadOnlySpan<char> textSpan = text.AsSpan();
            ReadOnlySpan<char> querySpan = query.AsSpan();

            // Глобальный оффсет для правильного позиционирования найденных 
            // фрагментов относительно начала оригинальной строки.
            int globalOffset = 0;

            while (!textSpan.IsEmpty)
            {
                // Ищем вхождение с учетом игнорирования регистра
                int index = textSpan.IndexOf(querySpan, StringComparison.OrdinalIgnoreCase);

                if (index < 0)
                {
                    break;
                }

                _highlighter.Ranges.Add(new TextRange
                {
                    StartIndex = globalOffset + index,
                    Length = querySpan.Length
                });

                // Вычисляем шаг сдвига (индекс найденного + длина искомого слова)
                int advance = index + querySpan.Length;

                // Отрезаем обработанную часть (сдвигаем окно поиска)
                textSpan = textSpan.Slice(advance);

                // Увеличиваем оффсет на размер среза
                globalOffset += advance;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"[HighlightTextBlock Update Error]: {ex.Message}");
#endif
        }
    }
}