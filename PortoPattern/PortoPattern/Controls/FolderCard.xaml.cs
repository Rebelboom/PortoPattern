// ****************************************************************************
// Файл: FolderCard.xaml.cs
// Описание: Логика взаимодействия для визуальной карточки папки.
// Обеспечивает плавные переходы состояний (Hover/Normal) через Storyboards.
// ****************************************************************************

#nullable enable
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using PortoPattern.ViewModels;
using System;
using System.Diagnostics;

namespace PortoPattern.Controls;

public sealed partial class FolderCard : UserControl
{
    // =========================================================================
    // БЛОК 1: СВОЙСТВА (Properties)
    // =========================================================================

    /// <summary> 
    /// Свойство для доступа x:Bind к данным. 
    /// Приводится к типу FolderCardViewModel из DataContext.
    /// </summary>
    public FolderCardViewModel ViewModel => (FolderCardViewModel)DataContext;

    private Storyboard? _currentStoryboard;

    // =========================================================================
    // БЛОК 2: ИНИЦИАЛИЗАЦИЯ
    // =========================================================================

    public FolderCard()
    {
        try
        {
            this.InitializeComponent();

            // Подписка на изменение контекста данных для принудительного обновления x:Bind
            this.DataContextChanged += (s, e) =>
            {
                if (e.NewValue is FolderCardViewModel)
                {
                    Bindings.Update();
                }
            };
        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод ошибки инициализации в консоль отладки
            Debug.WriteLine($"[FolderCard Error]: {ex.Message}");
#endif
        }
    }

    // =========================================================================
    // БЛОК 3: ВЗАИМОДЕЙСТВИЕ (Анимации)
    // =========================================================================

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        // Запуск анимации появления кнопок при наведении
        AnimateOverlay(1.0, true);
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        // Запуск анимации скрытия кнопок при уходе курсора
        AnimateOverlay(0.0, false);
    }

    /// <summary>
    /// Управляет прозрачностью и доступностью оверлея через Storyboard.
    /// </summary>
    /// <param name="targetOpacity">Целевое значение прозрачности (0 или 1).</param>
    /// <param name="isHitTestVisible">Флаг возможности взаимодействия с кнопками.</param>
    private void AnimateOverlay(double targetOpacity, bool isHitTestVisible)
    {
        try
        {
            // Остановка текущей анимации перед запуском новой
            _currentStoryboard?.Stop();

            DoubleAnimation opacityAnimation = new()
            {
                To = targetOpacity,
                Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(opacityAnimation, Overlay);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            _currentStoryboard = new Storyboard();
            _currentStoryboard.Children.Add(opacityAnimation);
            _currentStoryboard.Begin();

            // Установка доступности кнопок для клика
            Overlay.IsHitTestVisible = isHitTestVisible;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"[FolderCard Animation Error]: {ex.Message}");
#endif
        }
    }
}