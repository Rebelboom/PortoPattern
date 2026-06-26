// ****************************************************************************
// File: BlackListChangedMessage.cs
// Description: Сообщение о том, что список профилей был изменен в ядре.
// ****************************************************************************

#nullable enable
using System;

namespace PortoPattern.Messages;

/// <summary>
/// Сообщение, уведомляющее о том, что данные в IgnorManager изменились.
/// </summary>
public record BlackListChangedMessage();