#nullable enable

using System;
using System.Diagnostics;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;

namespace PortoPattern.Helpers;

/// <summary>
/// Обеспечивает применение эффекта свечения (Bloom/DropShadow) для UI-элементов 
/// через низкоуровневый слой Microsoft.UI.Composition.
/// </summary>
public static class BloomHelper
{
    private static readonly Windows.UI.Color _defaultColor = Windows.UI.Color.FromArgb(190, 255, 255, 255);

    /// <summary>
    /// Добавляет эффект свечения для указанного UI-элемента.
    /// </summary>
    public static void AddBloom(UIElement element, UIElement parent, Windows.UI.Color color, Vector3 offset, float blurRadius = 10)
    {
        if (element == null || parent == null)
        {
#if DEBUG
            Console.WriteLine("[WARNING] AddBloom: One (or more) UIElement is null, cannot continue.");
#endif
            return;
        }

        try
        {
            if (color == Microsoft.UI.Colors.Transparent)
                color = _defaultColor;

            var visual = ElementCompositionPreview.GetElementVisual(element);

            // Скрываем исходный визуальный элемент, так как вместо него будет отображаться RedirectVisual
            visual.Opacity = 0;
            var compositor = visual.Compositor;

            var sizeBind = compositor.CreateExpressionAnimation("visual.Size");
            sizeBind.SetReferenceParameter("visual", visual);

            var offsetBind = compositor.CreateExpressionAnimation("visual.Offset");
            offsetBind.SetReferenceParameter("visual", visual);

            var rVisual = compositor.CreateRedirectVisual(visual);
            rVisual.StartAnimation("Size", sizeBind);

            var lVisual = compositor.CreateLayerVisual();
            lVisual.StartAnimation("Size", sizeBind);
            lVisual.StartAnimation("Offset", offsetBind);

            lVisual.Children.InsertAtTop(rVisual);

            var shadow = compositor.CreateDropShadow();
            shadow.BlurRadius = blurRadius;
            shadow.Color = color;
            shadow.Offset = offset;
            shadow.SourcePolicy = CompositionDropShadowSourcePolicy.InheritFromVisualContent;

            lVisual.Shadow = shadow;
            lVisual.Opacity = 1.0f;

            var parentContainerVisual = ElementCompositionPreview.GetElementChildVisual(parent) as ContainerVisual;

            if (parentContainerVisual == null)
            {
                parentContainerVisual = compositor.CreateContainerVisual();
                parentContainerVisual.RelativeSizeAdjustment = Vector2.One;
                ElementCompositionPreview.SetElementChildVisual(parent, parentContainerVisual);
            }

            parentContainerVisual.Children.InsertAtTop(lVisual);
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] AddBloom execution failed: {ex.Message}");
#endif
        }
    }

    public static void AddBloom(UIElement element, UIElement parent, float blurRadius = 10)
        => AddBloom(element, parent, _defaultColor, Vector3.Zero, blurRadius);

    public static void AddBloom(UIElement element, UIElement parent, Windows.UI.Color color, float blurRadius = 10)
        => AddBloom(element, parent, color, Vector3.Zero, blurRadius);

    /// <summary>
    /// Удаляет эффект свечения, возвращая исходную прозрачность элемента.
    /// </summary>
    public static void RemoveBloom(UIElement element, UIElement parent, LayerVisual? layerVisual)
    {
        if (element == null || parent == null)
            return;

        try
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            visual.Opacity = 1.0f; // Восстанавливаем видимость оригинального элемента

            var parentContainerVisual = ElementCompositionPreview.GetElementChildVisual(parent) as ContainerVisual;
            if (parentContainerVisual != null)
            {
                if (layerVisual is not null)
                {
                    parentContainerVisual.Children.Remove(layerVisual);
                }
                else
                {
                    parentContainerVisual.Children.RemoveAll();
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] RemoveBloom execution failed: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Удаляет все дочерние композиционные слои из переданного элемента.
    /// </summary>
    public static void RemoveAllChildVisuals(UIElement element)
    {
        if (element == null) return;

        try
        {
            var visual = ElementCompositionPreview.GetElementChildVisual(element) as ContainerVisual;
            visual?.Children.RemoveAll();
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] RemoveAllChildVisuals failed: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Находит ближайший родительский контейнер типа Panel и очищает его эффекты.
    /// </summary>
    public static void RemoveAllParentVisuals(UIElement childElement)
    {
        if (childElement == null) return;

        try
        {
            var parentPanel = FindParentPanel(childElement);
            if (parentPanel != null)
            {
                var visual = ElementCompositionPreview.GetElementChildVisual(parentPanel) as ContainerVisual;
                visual?.Children.RemoveAll();
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERROR] RemoveAllParentVisuals failed: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Выполняет обход дерева визуальных элементов вверх для поиска контейнера макета.
    /// </summary>
    public static Panel? FindParentPanel(UIElement element)
    {
        if (element == null)
            return null;

        DependencyObject? parent = element;
        while (parent != null)
        {
            parent = VisualTreeHelper.GetParent(parent);

            if (parent is StackPanel panel) { return panel; }
            if (parent is Grid grid) { return grid; }
            if (parent is Canvas cnvs) { return cnvs; }
            if (parent is ItemsStackPanel ispnl) { return ispnl; }
            if (parent is ItemsWrapGrid iwgrd) { return iwgrd; }
            if (parent is RelativePanel rpanel) { return rpanel; }
            if (parent is SwapChainPanel scpnl) { return scpnl; }
            if (parent is SwapChainBackgroundPanel scbpnl) { return scbpnl; }
            if (parent is VariableSizedWrapGrid vswgrd) { return vswgrd; }
            if (parent is VirtualizingPanel vpnl) { return vpnl; }
            if (parent is VirtualizingStackPanel vspnl) { return vspnl; }
            if (parent is WrapGrid wgrd) { return wgrd; }
        }
        return null;
    }
}