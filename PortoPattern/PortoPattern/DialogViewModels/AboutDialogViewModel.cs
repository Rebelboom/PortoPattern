#nullable enable

// NOTE: Подключаем корневое пространство имен, если AppConfig находится там.
using PortoPattern;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel for the About dialog.
/// Provides data binding properties for the view, abstracting away the static AppConfig.
/// </summary>
public partial class AboutDialogViewModel : DialogViewModel
{
    // NOTE: Свойства инициализируются из конфигурации приложения. 
    // Если в будущем потребуется динамическое обновление (например, запрос версии с сервера), 
    // эти свойства можно будет легко перевести в [ObservableProperty].

    /// <summary>
    /// Gets the application name.
    /// </summary>
    public string AppName { get; } = AppConfig.AppName;

    /// <summary>
    /// Gets the author information.
    /// </summary>
    public string Author { get; } = AppConfig.Author;

    /// <summary>
    /// Gets the application description.
    /// </summary>
    public string Description { get; } = AppConfig.Description;

    /// <summary>
    /// Gets environment-specific notes.
    /// </summary>
    public string EnvironmentNote { get; } = AppConfig.EnvironmentNote;

    /// <summary>
    /// Gets the contact information for support or architecture details.
    /// </summary>
    public string ContactInfo { get; } = AppConfig.ContactInfo;

    /// <summary>
    /// Gets the formatted application version.
    /// </summary>
    public string AppVersion { get; } = AppConfig.AppVersion;

    /// <summary>
    /// Gets the system architecture the app is running on.
    /// </summary>
    public string Architecture { get; } = AppConfig.Architecture;

    public AboutDialogViewModel()
    {
        // Конструктор пока пуст, так как данные загружаются синхронно при инициализации свойств.
    }
}