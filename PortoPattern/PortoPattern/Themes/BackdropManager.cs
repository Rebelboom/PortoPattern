// ****************************************************************************
// File: BackdropManager.cs
// Description: Управляет эффектом DesktopAcrylic для окна приложения WinUI 3.
// Правки: Добавлен метод RefreshColors() для принудительного обновления кистей при смене темы.
// ****************************************************************************

#nullable enable

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WinRT;

namespace PortoPattern.Themes;

public sealed class BackdropManager : IDisposable
{
    private readonly Window _window;

    private DesktopAcrylicController? _acrylicController;
    private readonly SystemBackdropConfiguration _configurationSource = new();
    private ICompositionSupportsSystemBackdrop? _backdropTarget;

    private bool _isDisposed;

    /// <summary>
    /// Инициализирует менеджер акрила для указанного окна.
    /// </summary>
    public BackdropManager(Window window)
    {
        _window = window;
    }

    /// <summary>
    /// Инициализирует контроллер Acrylic эффекта и подписывает окно на системные события.
    /// </summary>
    public void Initialize()
    {
        if (_isDisposed) return;

        try
        {
            // Проверка наличия очереди диспетчера (критическое требование WinUI 3 для эффектов Backdrop)
            if (Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread() == null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[DEBUG ERROR] BackdropManager: DispatcherQueue is missing in current thread.");
#endif
                return;
            }

            // Подписка на жизненный цикл окна (активность и закрытие)
            _window.Activated += OnWindowActivated;
            _window.Closed += OnWindowClosed;

            _configurationSource.IsInputActive = true;

            // Инициализация Acrylic эффекта, если операционная система его поддерживает
            if (DesktopAcrylicController.IsSupported())
            {
                _acrylicController = new DesktopAcrylicController();
                _backdropTarget = _window.As<ICompositionSupportsSystemBackdrop>();

                _acrylicController.AddSystemBackdropTarget(_backdropTarget);
                _acrylicController.SetSystemBackdropConfiguration(_configurationSource);
            }

            // Применяем цвета из ColorManager сразу при создании окна
            ApplyColors();
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DEBUG ERROR] BackdropManager.Initialize: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Принудительно обновляет цвета акрила (вызывается при смене темы приложения).
    /// </summary>
    public void RefreshColors()
    {
        ApplyColors();
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isDisposed) return;
        // Переключаем визуальное состояние эффекта (активное/затемненное) при изменении фокуса окна
        _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args) => Dispose();

    /// <summary>
    /// Извлекает статический цвет из ColorManager и передает его в контроллер Acrylic.
    /// </summary>
    private void ApplyColors()
    {
        if (_isDisposed) return;

        try
        {
            // Получаем актуальный цвет поверхности и значение прозрачности
            var color = ColorManager.GetSurfaceColor();
            var opacity = ColorManager.BackdropTintOpacity;

            if (_acrylicController != null)
            {
                // Назначаем один цвет для TintColor (фокус) и FallbackColor (потеря фокуса / старые ОС)
                _acrylicController.TintColor = color;
                _acrylicController.FallbackColor = color;
                _acrylicController.TintOpacity = opacity;
            }
            else if (_window.Content is FrameworkElement element)
            {
                // Резервный вариант: если Acrylic не поддерживается, красим фон контента обычной кистью
                var brush = new SolidColorBrush(color);
                if (element is Control control) control.Background = brush;
                else if (element is Panel panel) panel.Background = brush;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DEBUG ERROR] BackdropManager.ApplyColors: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Освобождает системные ресурсы контроллера и отписывается от событий окна.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            // Отписываемся от событий окна, чтобы избежать утечек памяти
            _window.Activated -= OnWindowActivated;
            _window.Closed -= OnWindowClosed;

            if (_acrylicController != null)
            {
                // Вызов освобождения контроллера акрила без ручного удаления таргета во избежание гонки потоков DWM
                _acrylicController.Dispose();
                _acrylicController = null;
            }
            _backdropTarget = null;
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DEBUG ERROR] BackdropManager.Dispose: {ex.Message}");
#endif
        }
    }
}