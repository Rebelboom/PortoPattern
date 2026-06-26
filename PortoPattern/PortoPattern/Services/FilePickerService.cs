#nullable enable
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;
using PortoPattern.Interfaces;
using System.Runtime.InteropServices;

namespace PortoPattern.Services;

/// <summary>
/// Сервис для выбора директорий.
/// Обернут в логику обхода ограничений WinUI 3 при запуске с правами администратора.
/// </summary>
public interface IFilePickerService
{
    Task<string?> PickFolderAsync();
}

public class FilePickerService : IFilePickerService
{
    private readonly IWindowProvider _windowProvider;

    public FilePickerService(IWindowProvider windowProvider)
    {
        _windowProvider = windowProvider;
    }

    /// <summary>
    /// Открывает диалоговое окно выбора папки. 
    /// Сначала пытается использовать стандартный WinUI Picker. В случае ошибки прав доступа (запуск от Администратора)
    /// переключается на использование Win32 API.
    /// </summary>
    public async Task<string?> PickFolderAsync()
    {
        try
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            IntPtr hwnd = _windowProvider.GetMainWindowHandle();

            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("Window handle is not initialized.");

            // Инициализация окна Picker для WinUI 3
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();

            // ИСПРАВЛЕНИЕ: Если пользователь нажал "Отмена", folder будет null.
            // Мы сразу возвращаем null и завершаем работу метода. Запасной Picker не вызывается.
            return folder?.Path;
        }
        catch (Exception ex)
        {
            // Логируем ошибку только в режиме отладки.
            // Исключение здесь означает, что стандартный Picker заблокирован ОС (например, из-за UAC).
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Стандартный Picker заблокирован или упал: {ex.Message}. Переход к Win32.");
#endif
        }

        // Запуск резервного варианта происходит только при исключении в блоке try
        return PickFolderWin32();
    }

    /// <summary>
    /// Резервный метод выбора папки через Win32 Shell API. 
    /// Игнорирует проблемы с уровнями целостности UAC.
    /// </summary>
    private string? PickFolderWin32()
    {
        var hwnd = _windowProvider.GetMainWindowHandle();
        var browseInfo = new BROWSEINFO();

        browseInfo.hwndOwner = hwnd;
        browseInfo.lpszTitle = "Параметры сканирования: Выберите директорию для анализа";

        // Настройка флагов окна. 
        // ИСПРАВЛЕНИЕ: Убран Callback с жестким ресайзом. Windows сама установит корректный размер
        // диалогового окна, исключая обрезку рамок и кнопок.
        browseInfo.ulFlags = (uint)(
            BrowseFlags.BIF_NEWDIALOGSTYLE |     // Современный вид, изменение размера пользователем, контекстное меню
            BrowseFlags.BIF_EDITBOX |            // Добавляет текстовое поле ввода пути вверху дерева
            BrowseFlags.BIF_RETURNONLYFSDIRS |   // Блокирует выбор виртуальных папок (Корзина, Панель управления)
            BrowseFlags.BIF_VALIDATE             // Проверяет корректность пути при ручном вводе
        );

        // Устанавливаем lpfn в IntPtr.Zero, так как мы отказались от ручного управления окном
        browseInfo.lpfn = IntPtr.Zero;

        // Вызов системного окна
        IntPtr pidl = SHBrowseForFolder(ref browseInfo);

        if (pidl != IntPtr.Zero)
        {
            IntPtr pathPtr = Marshal.AllocHGlobal(260 * 2); // Буфер под максимальную длину пути (MAX_PATH)
            try
            {
                if (SHGetPathFromIDList(pidl, pathPtr))
                {
                    return Marshal.PtrToStringUni(pathPtr);
                }
            }
            finally
            {
                // Обязательное освобождение неуправляемой памяти
                Marshal.FreeHGlobal(pathPtr);
                Marshal.FreeCoTaskMem(pidl);
            }
        }

        return null;
    }

    /// <summary>
    /// Полный набор флагов BROWSEINFO для настройки внешнего вида окна Win32.
    /// </summary>
    [Flags]
    private enum BrowseFlags : uint
    {
        BIF_RETURNONLYFSDIRS = 0x00000001,   // Только реальные папки файловой системы (блокирует кнопку ОК на виртуальных)
        BIF_DONTGOBELOWDOMAIN = 0x00000002,  // Не показывать сетевые папки за пределами локального домена
        BIF_STATUSTEXT = 0x00000004,         // Текстовая строка статуса (только для старого стиля окна)
        BIF_RETURNFSANCESTORS = 0x00000008,  // Возвращать только предков файловой системы
        BIF_EDITBOX = 0x00000010,            // Поле ввода пути текстом
        BIF_VALIDATE = 0x00000020,           // Валидация текста из поля EditBox
        BIF_NEWDIALOGSTYLE = 0x00000040,     // Современный стиль окна с возможностью изменения размеров
        BIF_USENEWUI = 0x00000040 | 0x00000010, // Комбинация Нового стиля и Поля ввода пути
        BIF_BROWSEINCLUDEFILES = 0x00004000, // Отображать внутри дерева не только папки, но и файлы
        BIF_SHAREABLE = 0x00008000,          // Отображать общие сетевые ресурсы
        BIF_NONEWFOLDERBUTTON = 0x00000200,  // Скрыть кнопку "Создать папку"
        BIF_NOTRANSLATETARGETS = 0x00000400  // Не преобразовывать ярлыки (.lnk) в целевые папки
    }

    /// <summary>
    /// Структура параметров для системного вызова SHBrowseForFolder.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct BROWSEINFO
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public string lpszDisplayName;
        public string lpszTitle;
        public uint ulFlags;
        public IntPtr lpfn; // Возвращен стандартный IntPtr для функции обратного вызова
        public IntPtr lParam;
        public int iImage;
    }

    // P/Invoke импорты системных библиотек Windows
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);
}