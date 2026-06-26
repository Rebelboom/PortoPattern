// Файл: Fluidity.cs
// Описание: Расширения для работы с Composition API и создания плавных аппаратных анимаций (UI Fluidity).

#nullable enable

using System;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;

namespace PortoPattern.Helpers;

/// <summary>
/// Доступные типы кривых Безье для анимаций.
/// </summary>
public enum PennerType
{
    CubicEaseOut,
    CubicEaseIn,
    BackEaseOut,
    EaseInOut // Переименовано из Default
}

/// <summary>
/// Обеспечивает плавность интерфейса через создание неявных и параметрических анимаций.
/// </summary>
public static class Fluidity
{
    /// <summary>
    /// Создает неявную (Implicit) анимацию для указанного свойства с поддержкой функций плавности.
    /// </summary>
    public static ImplicitAnimationCollection CreateImplicitAnimation(
        this ImplicitAnimationCollection source,
        string target,
        TimeSpan? duration = null,
        CompositionEasingFunction? easing = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(target);

        KeyFrameAnimation? animation = null;
        Compositor compositor = source.Compositor;

        switch (target)
        {
            case "Offset":
            case "Scale":
            case "Translation":
            case "CenterPoint":
                animation = compositor.CreateVector3KeyFrameAnimation();
                break;
            case "Size":
                animation = compositor.CreateVector2KeyFrameAnimation();
                break;
            case "Opacity":
            case "RotationAngle":
                animation = compositor.CreateScalarKeyFrameAnimation();
                break;
        }

        if (animation is null) return source;

        if (easing is not null)
        {
            animation.InsertExpressionKeyFrame(1f, "this.FinalValue", easing);
        }
        else
        {
            animation.InsertExpressionKeyFrame(1f, "this.FinalValue");
        }

        animation.Duration = duration ?? TimeSpan.FromMilliseconds(250);
        animation.Target = target;

        source[target] = animation;
        return source;
    }

    /// <summary>
    /// Генерирует функцию плавности на основе уравнений Безье (Penner Equations).
    /// </summary>
    public static CompositionEasingFunction CreatePennerEquation(this Compositor compositor, PennerType type = PennerType.CubicEaseOut)
    {
        return type switch
        {
            PennerType.CubicEaseOut => compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.61f), new Vector2(0.355f, 1.0f)),
            PennerType.CubicEaseIn => compositor.CreateCubicBezierEasingFunction(new Vector2(0.55f, 0.055f), new Vector2(0.675f, 0.19f)),
            PennerType.BackEaseOut => compositor.CreateCubicBezierEasingFunction(new Vector2(0.175f, 0.885f), new Vector2(0.32f, 1.275f)),
            PennerType.EaseInOut => compositor.CreateCubicBezierEasingFunction(new Vector2(0.445f, 0.05f), new Vector2(0.55f, 0.95f)),
            _ => compositor.CreateCubicBezierEasingFunction(new Vector2(0.445f, 0.05f), new Vector2(0.55f, 0.95f))
        };
    }

    /// <summary>
    /// Автоматически привязывает CenterPoint визуала к его геометрическому центру.
    /// </summary>
    public static void BindCenterPoint(this Visual target)
    {
        ArgumentNullException.ThrowIfNull(target); // Добавлена защита от null

        var exp = target.Compositor.CreateExpressionAnimation("Vector3(this.Target.Size.X / 2, this.Target.Size.Y / 2, 0f)");
        target.StartAnimation("CenterPoint", exp);
    }
}