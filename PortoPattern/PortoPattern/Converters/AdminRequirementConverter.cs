// Файл: AdminRequirementConverter.cs
// Расположение: UI Проект -> Папка Converters (Глобальная регистрация в App.xaml)
// Описание: Конвертер для WinUI 3 XAML. Отвечает за динамическое управление свойством IsEnabled 
// элементов интерфейса в зависимости от типа папки (системная/пользовательская) и прав администратора текущего процесса.

#nullable enable
using System;
using Microsoft.UI.Xaml.Data;
using PortoPattern.Core.IgnorNew;

namespace PortoPattern.Converters;

public class AdminRequirementConverter : IValueConverter
{
    // ==========================================
    // Блок 1: Логика конвертации (Model -> UI)
    // ==========================================

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isSystemFolder)
        {
            // Чекбокс активен, если папка НЕ является системной
            // ИЛИ если папка системная, но у текущего пользователя есть права администратора.
            return !isSystemFolder || IgnoreItem.IsUserAdministrator();
        }

        // В случае непредвиденных типов данных блокируем элемент в целях безопасности
        return false;
    }

    // ==========================================
    // Блок 2: Обратная конвертация (UI -> Model)
    // ==========================================

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // Изменение состояния IsEnabled в UI не должно менять исходные данные модели.
        throw new NotImplementedException();
    }
}