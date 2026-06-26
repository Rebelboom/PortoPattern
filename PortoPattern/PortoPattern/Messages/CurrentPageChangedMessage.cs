// ****************************************************************************
// File: CurrentPageChangedMessage.cs
// Description: Message sent by a ViewModel to notify that it has become active.
// ****************************************************************************

#nullable enable
using System;

namespace PortoPattern.Messages;

/// <summary>
/// Сообщение, уведомляющее о том, что текущая страница изменилась.
/// </summary>
/// <param name="ViewModelType">Тип вью-модели, которая стала активной.</param>
public record CurrentPageChangedMessage(Type ViewModelType);