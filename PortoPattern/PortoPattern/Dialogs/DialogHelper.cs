// File: DialogHelper.cs
// Description: Потокобезопасный помощник для корректной инициализации, стилизации и отображения окон ContentDialog в WinUI 3.
// NOTE: Метод ShowAsync использует ContentDialogPlacement.Popup для предотвращения визуальных багов при вызове окна.
// TODO: Если в будущем потребуется открывать диалоги поверх конкретных контейнеров, добавить перегрузку с параметром ContentDialogPlacement.InPlace.

#nullable enable
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortoPattern.Dialogs;

public static class DialogHelper
{
    static bool isOpening = false;
    static SemaphoreSlim semaSlim = new SemaphoreSlim(1, 1);

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public static async Task<ContentDialogResult> ShowAsync(ContentDialog dialog, FrameworkElement element)
    {
        ContentDialogResult dialogResult = ContentDialogResult.None;
        if (element is null) { return dialogResult; }
        try
        {
            await semaSlim.WaitAsync();
            if (!isOpening && dialog is not null && element is not null)
            {
                isOpening = true;
                if (dialog.XamlRoot is null)
                    dialog.XamlRoot = element.XamlRoot;

                dialog.RequestedTheme = element.ActualTheme;
                element.ActualThemeChanged += (sender, args) => { dialog.RequestedTheme = element.ActualTheme; };

                dialogResult = await dialog.ShowAsync(ContentDialogPlacement.Popup);
                isOpening = false;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ERROR] DialogHelper.ShowAsync: {ex.Message}");
#endif
        }
        finally { semaSlim.Release(); }
        return dialogResult;
    }

    public static ContentDialogResult ShowAsTask(ContentDialog dialog, FrameworkElement element)
    {
        ContentDialogResult dialogResult = ContentDialogResult.None;
        if (!isOpening && dialog is not null && element is not null)
        {
            isOpening = true;
            bool enqueued = element.DispatcherQueue.TryEnqueue(() =>
            {
                if (dialog.XamlRoot is null)
                    dialog.XamlRoot = element.XamlRoot;
                dialog.RequestedTheme = element.ActualTheme;
                element.ActualThemeChanged += (sender, args) => { dialog.RequestedTheme = element.ActualTheme; };
                dialog.ShowAsync().AsTask().ContinueWith(t =>
                {
#if DEBUG
                    if (t.Exception != null)
                        System.Diagnostics.Debug.WriteLine($"[ERROR] ShowAsTask: {t.Exception.Message}");
                    else
#endif
                        dialogResult = t.Result;

                    isOpening = false;
                });
            });

            if (!enqueued)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[ERROR] DispatcherQueue.TryEnqueue failed. Unable to show ContentDialog.");
#endif
                isOpening = false;
            }
        }
        return dialogResult;
    }
}