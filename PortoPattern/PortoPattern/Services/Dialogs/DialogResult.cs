#nullable enable

namespace PortoPattern.Services.Dialogs;

/// <summary>
/// Defines the standard outcomes for any dialog.
/// </summary>
public enum DialogOutcome
{
    None,
    Ok,
    Cancel,
    Yes,
    No
}

/// <summary>
/// Base class representing the result of a dialog operation.
/// </summary>
public class DialogResult
{
    /// <summary>
    /// Gets the generic outcome of the dialog (Ok, Cancel, etc.).
    /// </summary>
    public DialogOutcome Outcome { get; init; }

    public DialogResult(DialogOutcome outcome)
    {
        Outcome = outcome;
    }
}

/// <summary>
/// Represents the result of a dialog operation with a typed payload.
/// </summary>
/// <typeparam name="T">The type of the custom data returned by the dialog.</typeparam>
public class DialogResult<T> : DialogResult
{
    /// <summary>
    /// Gets the payload data returned by the dialog.
    /// </summary>
    public T? Payload { get; init; }

    public DialogResult(DialogOutcome outcome, T? payload = default) : base(outcome)
    {
        Payload = payload;
    }
}