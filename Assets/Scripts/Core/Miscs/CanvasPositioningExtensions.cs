using UnityEngine;

/// <summary>
/// Small helper class to convert viewport, screen or world positions to canvas space.
/// Only works with screen space canvases.
/// </summary>
/// <example>
/// <code>
/// objectOnCanvasRectTransform.anchoredPosition = specificCanvas.WorldToCanvasPoint(worldspaceTransform.position);
/// </code>
/// </example>
public static class CanvasPositioningExtensions
{
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null, bool useNormalizeViewPort = false)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var viewportPosition = camera.WorldToViewportPoint(worldPosition);
        viewportPosition.z = 0;

        if (useNormalizeViewPort)
        {
            Rect normalizedViewPort = camera.rect;
            viewportPosition.x = viewportPosition.x * normalizedViewPort.width + normalizedViewPort.x;
            viewportPosition.y = viewportPosition.y * normalizedViewPort.height + normalizedViewPort.y;
        }

        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                                           screenPosition.y / Screen.height,
                                           0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }
}