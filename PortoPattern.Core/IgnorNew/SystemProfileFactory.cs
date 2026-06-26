// Файл: SystemProfileFactory.cs
// Описание: Фабрика для генерации и внедрения системных профилей в общий список.
// Гарантирует, что защищенные профили всегда существуют в приложении и не могут быть удалены пользователем.

#nullable enable
using System.Collections.Generic;
using System.Linq;
using PortoPattern.Core.IgnorSpace;

namespace PortoPattern.Core.IgnorNew;

public static class SystemProfileFactory
{
    /// <summary>
    /// Проверяет наличие системных профилей в списке и добавляет их, если они отсутствуют.
    /// Выполняет инициализацию, если коллекция была пустой или повреждена.
    /// </summary>
    public static void EnsureSystemProfilesExist(List<BlackListProfile> profiles)
    {
        // Инициализируем генератор правил для создания корректных VirtualVariable на основе путей
        var generator = new IgnorRuleGenerator();

        // Проверяем и добавляем защищенный (администраторский) профиль, если он отсутствует
        if (!profiles.Any(p => p.ProfileName == IgnorProfileConstants.AdminProfileName))
        {
            // Вставляем в самое начало (индекс 0), чтобы они отображались первыми в UI
            profiles.Insert(0, CreateProfile(
                IgnorProfileConstants.AdminProfileName,
                IgnorProfileConstants.ProtectedFolders,
                generator,
                requiresAdmin: true,
                defaultChecked: true));
        }

        // Проверяем и добавляем стандартный системный профиль, если он отсутствует
        if (!profiles.Any(p => p.ProfileName == IgnorProfileConstants.UserProfileName))
        {
            // Вставляем после администраторского профиля
            profiles.Insert(1, CreateProfile(
                IgnorProfileConstants.UserProfileName,
                IgnorProfileConstants.UnprotectedFolders,
                generator,
                requiresAdmin: false,
                defaultChecked: true));
        }
    }

    /// <summary>
    /// Вспомогательный метод для формирования объекта BlackListProfile.
    /// Устанавливает базовые флаги IsSystem и RequiresAdmin.
    /// </summary>
    private static BlackListProfile CreateProfile(string name, IReadOnlyList<string> folders, IgnorRuleGenerator generator, bool requiresAdmin, bool defaultChecked)
    {
        var profile = new BlackListProfile
        {
            ProfileName = name,
            IsActive = true,
            IsGlobalMode = true,
            IsSystem = true, // Флаг, запрещающий удаление через UI
            RequiresAdmin = requiresAdmin
        };

        foreach (var folder in folders)
        {
            // Создаем правило (isGlobal = true, так как это системные имена папок)
            var rule = generator.CreateRule(folder, isGlobal: true);
            if (rule != null)
            {
                rule.IsSystem = true; // Правило также защищено от удаления
                rule.IsChecked = defaultChecked;
                profile.Rules.Add(rule);
            }
        }

        return profile;
    }
}