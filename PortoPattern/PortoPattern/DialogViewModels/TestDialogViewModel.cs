#nullable enable

using PortoPattern.ViewModels;

namespace PortoPattern.ViewModels;

public partial class TestDialogViewModel : DialogViewModel
{
    public static new string Title => "Тестовый диалог";

    public string Message => "DialogService работает корректно.";
}