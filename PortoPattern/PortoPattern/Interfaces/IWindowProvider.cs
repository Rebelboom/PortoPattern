using System;

namespace PortoPattern.Interfaces;

public interface IWindowProvider
{
    IntPtr GetMainWindowHandle();

    // Добавлено для проверки состояния в зависимых сервисах
    bool IsInitialized { get; }
}