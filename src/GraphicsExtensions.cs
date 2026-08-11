using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace _3dedit
{
    // Extension method for rounded rectangle drawing
    static class GraphicsExtensions
    {
        public static void FillRoundedRect(this Graphics g, Brush brush, RectangleF rect, float r)
        {
            using (var path = GetRoundedRect(rect, r))
                g.FillPath(brush, path);
        }

        public static void DrawRoundedRect(this Graphics g, Pen pen, RectangleF rect, float r)
        {
            using (var path = GetRoundedRect(rect, r))
                g.DrawPath(pen, path);
        }

        static GraphicsPath GetRoundedRect(RectangleF rect, float r)
        {
            var path = new GraphicsPath();
            r = Math.Min(r, Math.Min(rect.Width / 2f, rect.Height / 2f));
            path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
            path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
            path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
