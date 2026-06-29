/******************************************************************************
 * Файл: SystemProfileFactory.cs
 * Описание: Фабрика для генерации и внедрения системных профилей в общий список.
 * Гарантирует, что защищенные профили всегда существуют и настроены корректно.
 ******************************************************************************/

#nullable enable
using System.Collections.Generic;
using System.Linq;
using PortoPattern.Core.IgnorSpace;

namespace PortoPattern.Core.IgnorNew;

public static class SystemProfileFactory
{
    // Проверяет наличие системных профилей в списке и добавляет их при отсутствии.
    public static void EnsureSystemProfilesExist(List<BlackListProfile> profiles)
    {
        // Инициализируем генератор правил
        var generator = new IgnorRuleGenerator();

        // Проверяем и добавляем защищенный (администраторский) профиль
        if (!profiles.Any(p => p.ProfileName == IgnorProfileConstants.AdminProfileName))
        {
            // [ПРАВКА]: Добавлен аргумент isGlobal: false. 
            // Администраторские папки (ProtectedFolders) должны обрабатываться как точные пути (ExactPath).
            profiles.Insert(0, CreateProfile(
                IgnorProfileConstants.AdminProfileName,
                IgnorProfileConstants.ProtectedFolders,
                generator,
                requiresAdmin: true,
                defaultChecked: true,
                isGlobal: false));
        }

        // Проверяем и добавляем стандартный системный профиль
        if (!profiles.Any(p => p.ProfileName == IgnorProfileConstants.UserProfileName))
        {
            // [ПРАВКА]: Добавлен аргумент isGlobal: true. 
            // Стандартные системные папки (UnprotectedFolders) обрабатываются глобально (GlobalName).
            profiles.Insert(1, CreateProfile(
                IgnorProfileConstants.UserProfileName,
                IgnorProfileConstants.UnprotectedFolders,
                generator,
                requiresAdmin: false,
                defaultChecked: true,
                isGlobal: true));
        }
    }

    // [ПРАВКА]: Добавлен параметр bool isGlobal в сигнатуру метода.
    private static BlackListProfile CreateProfile(string name, IReadOnlyList<string> folders, IgnorRuleGenerator generator, bool requiresAdmin, bool defaultChecked, bool isGlobal)
    {
        // Создаем каркас профиля
        var profile = new BlackListProfile
        {
            ProfileName = name,
            IsActive = true,
            // [ПРАВКА]: Устанавливаем режим профиля в соответствии с переданным параметром
            IsGlobalMode = isGlobal,
            IsSystem = true,
            RequiresAdmin = requiresAdmin
        };

        // Определение префикса на основе уровня привилегий профиля
        string prefix = requiresAdmin ? "admin" : "user";

        foreach (var folder in folders)
        {
            // [ПРАВКА]: Передаем динамический параметр isGlobal вместо хардкода
            var rule = generator.CreateRule(folder, isGlobal: isGlobal, prefix: prefix);

            if (rule != null)
            {
                rule.IsSystem = true;

                // Пробрасываем флаг RequiresAdmin в само правило
                rule.RequiresAdmin = requiresAdmin;

                rule.IsChecked = defaultChecked;
                profile.Rules.Add(rule);
            }
        }

        return profile;
    }
}