using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PortoPattern.Controls;

public sealed partial class TopToolBar : UserControl
{
    // TITLE

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(TopToolBar),
            new PropertyMetadata(string.Empty, OnTitleChanged));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TopToolBar control)
        {
            control.PageTitle.Text = e.NewValue?.ToString();
        }
    }

    // SUBTITLE

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(
            nameof(Subtitle),
            typeof(string),
            typeof(TopToolBar),
            new PropertyMetadata(string.Empty, OnSubtitleChanged));

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    private static void OnSubtitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TopToolBar control)
        {
            var text = e.NewValue?.ToString();

            control.PageSubtitle.Text = text;

            control.PageSubtitle.Visibility =
                string.IsNullOrWhiteSpace(text)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }
    }

    // SEARCH TEXT
    // Создаем независимое свойство для текста поиска, чтобы получать его из страницы
    public static readonly DependencyProperty SearchTextProperty =
        DependencyProperty.Register(
            nameof(SearchText),
            typeof(string),
            typeof(TopToolBar),
            new PropertyMetadata(string.Empty));

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public TopToolBar()
    {
        InitializeComponent();
    }
}