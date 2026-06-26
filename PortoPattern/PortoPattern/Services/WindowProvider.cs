#nullable enable
using System;
using PortoPattern.Interfaces;

namespace PortoPattern.Services;

public class WindowProvider : IWindowProvider
{
    private IntPtr _hwnd = IntPtr.Zero;

    public bool IsInitialized => _hwnd != IntPtr.Zero;

    public void SetWindowHandle(IntPtr hwnd)
    {
        if (_hwnd != IntPtr.Zero && _hwnd != hwnd)
        {
#if DEBUG
            Console.WriteLine("[DEBUG WARNING] WindowProvider: Attempting to overwrite an existing HWND.");
#endif
            return;
        }
        _hwnd = hwnd;
    }

    public IntPtr GetMainWindowHandle()
    {
#if DEBUG
        if (_hwnd == IntPtr.Zero)
        {
            Console.WriteLine("[DEBUG ERROR] WindowProvider: MainWindowHandle accessed before initialization.");
        }
#endif
        return _hwnd;
    }
}