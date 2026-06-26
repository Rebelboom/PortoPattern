// ****************************************************************************
// File: NavigationMessage.cs
// Description: Message for loosely coupled navigation requests.
// ****************************************************************************

#nullable enable
using System;

namespace PortoPattern.Messages;

/// <summary>
/// Сообщение, запрашивающее переход на определенную ViewModel.
/// </summary>
/// <param name="ViewModelType">Тип вью-модели, на которую нужно перейти.</param>
public record NavigationMessage(Type ViewModelType);