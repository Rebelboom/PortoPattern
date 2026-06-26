#nullable enable
using Microsoft.UI.Xaml.Controls;

namespace PortoPattern.Dialogs;

public sealed partial class MatchPolicyDialog : ContentDialog
{
    public MatchPolicyDialog(string folderName)
    {
        this.InitializeComponent();
        QuestionText.Text = $"Игнорировать только эту конкретную директорию или вообще любые папки с именем \"{folderName}\" при сканировании?";
    }
}
