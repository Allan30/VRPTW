﻿using ScottPlot.Drawing;
using ScottPlot;
using System.Drawing;
using System.Linq;
using System;
using ScottPlot.Plottable;

namespace VRPTW.UI.Extensions;

public class CustomTooltip : IPlottable, IHasColor, IHittable
{

    private readonly Tooltip _tooltip;

    public CustomTooltip(Tooltip tooltip)
    {
        _tooltip = tooltip;
    }

    public bool IsVisible
    {
        get => _tooltip.IsVisible;
        set
        {
            _tooltip.IsVisible = value;
        }
    }
    public int XAxisIndex
    {
        get => _tooltip.XAxisIndex;
        set
        {
            _tooltip.XAxisIndex = value;
        }
    }
    public int YAxisIndex
    {
        get => _tooltip.YAxisIndex;
        set
        {
            _tooltip.YAxisIndex = value;
        }
    }

    public Color Color
    {
        get => _tooltip.Color;
        set
        {
            _tooltip.Color = value;
        }
    }

    public Cursor HitCursor
    {
        get => _tooltip.HitCursor;
        set
        {
            _tooltip.HitCursor = value;
        }
    }
    public bool HitTestEnabled
    {
        get => _tooltip.HitTestEnabled;
        set
        {
            _tooltip.HitTestEnabled = value;
        }
    }

    public AxisLimits GetAxisLimits() => _tooltip.GetAxisLimits();

    public LegendItem[] GetLegendItems() => _tooltip.GetLegendItems();

    public bool HitTest(Coordinate coord) => _tooltip.HitTest(coord);

    public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
    {
        if (!IsVisible)
            return;

        using (var gfx = GDI.Graphics(bmp, dims, lowQuality, clipToDataArea: true))
        using (var font = GDI.Font(_tooltip.Font))
        using (var fillBrush = GDI.Brush(_tooltip.FillColor))
        using (var fontBrush = GDI.Brush(_tooltip.Font.Color))
        using (var pen = GDI.Pen(_tooltip.BorderColor, _tooltip.BorderWidth))
        {
            SizeF labelSize = gfx.MeasureString(_tooltip.Label, font);

            bool labelIsOnRight = dims.DataWidth - dims.GetPixelX(_tooltip.X) - labelSize.Width > 0;
            int sign = labelIsOnRight ? 1 : -1;

            PointF arrowHeadLocation = new PointF(dims.GetPixelX(_tooltip.X), dims.GetPixelY(_tooltip.Y));

            float contentBoxInsideEdgeX = arrowHeadLocation.X + sign * _tooltip.ArrowSize;
            PointF upperArrowVertex = new PointF(contentBoxInsideEdgeX, arrowHeadLocation.Y - _tooltip.ArrowSize);
            PointF lowerArrowVertex = new PointF(contentBoxInsideEdgeX, arrowHeadLocation.Y + _tooltip.ArrowSize);

            float contentBoxTopEdge = upperArrowVertex.Y - _tooltip.LabelPadding;
            float contentBoxBottomEdge = Math.Max(contentBoxTopEdge + labelSize.Height, lowerArrowVertex.Y) + 2 * _tooltip.LabelPadding;

            PointF[] points =
            {
                    arrowHeadLocation,
                    upperArrowVertex,
                    new PointF(contentBoxInsideEdgeX, upperArrowVertex.Y - _tooltip.LabelPadding),
                    new PointF(contentBoxInsideEdgeX + sign * (labelSize.Width + _tooltip.LabelPadding), upperArrowVertex.Y - _tooltip.LabelPadding),
                    new PointF(contentBoxInsideEdgeX + sign * (labelSize.Width + _tooltip.LabelPadding), contentBoxBottomEdge),
                    new PointF(contentBoxInsideEdgeX, contentBoxBottomEdge),
                    lowerArrowVertex,
                    arrowHeadLocation,
                    // add one more point to prevent render artifacts where thick line ends meet
                    upperArrowVertex,
                };

            byte[] pathPointTypes = Enumerable.Range(0, points.Length).Select(_ => (byte)System.Drawing.Drawing2D.PathPointType.Line).ToArray();

            var path = new System.Drawing.Drawing2D.GraphicsPath(points, pathPointTypes);

            gfx.FillPath(fillBrush, path);
            gfx.DrawPath(pen, path);

            float labelOffsetX = labelIsOnRight ? 0 : -labelSize.Width;
            float labelX = contentBoxInsideEdgeX + labelOffsetX + sign * _tooltip.LabelPadding / 2;
            float labelY = upperArrowVertex.Y;
            gfx.DrawString(_tooltip.Label, font, fontBrush, labelX, labelY);

            // calculate where the tooltip is in coordinate units and save it for later hit detection
            Coordinate[] corners = points.Select(pt => dims.GetCoordinate(pt.X, pt.Y)).ToArray();
            var prop = typeof(Tooltip).GetField("LastRenderRect", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            prop.SetValue(_tooltip, CoordinateRect.BoundingBox(corners));
        }
    }
    
    public void ValidateData(bool deep = false) => _tooltip.ValidateData(deep);
}
