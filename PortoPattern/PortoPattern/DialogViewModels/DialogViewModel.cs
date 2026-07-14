// ****************************************************************************
// File: DialogViewModel.cs
// Description: Base ViewModel for all application dialogs.
// ****************************************************************************

#nullable enable

using System.Threading.Tasks;

namespace PortoPattern.ViewModels;

public abstract class DialogViewModel : MainViewModel
{
    /// <summary>
    /// Заголовок диалога.
    /// </summary>
    public virtual string Title => string.Empty;

    /// <summary>
    /// Определяет, может ли диалог быть закрыт.
    /// Используется, например, для отмены закрытия при невалидных данных.
    /// </summary>
    public virtual bool CanClose => true;

    /// <summary>
    /// Вызывается после открытия диалога.
    /// </summary>
    public virtual Task OnOpenedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Вызывается непосредственно перед закрытием диалога.
    /// </summary>
    public virtual Task OnClosingAsync()
    {
        return Task.CompletedTask;
    }

    // ------------------------------------------------------------------------
    // NOTE:
    // If future dialogs require asynchronous initialization (e.g. loading data,
    // restoring state, or preparing resources before the dialog is displayed),
    // consider introducing a virtual InitializeAsync() method here.
    //
    // The method is intentionally omitted until a real use case appears
    // (YAGNI principle).
    // ------------------------------------------------------------------------
}