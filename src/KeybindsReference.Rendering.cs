using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class KeybindsReference
    {
        /// <summary>Call from the main form on key events to refresh the display.</summary>
        public void RefreshDisplay()
        {
            if (!_collapsed) Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background
            using (var bg = new SolidBrush(Color.FromArgb(220, 220, 220)))
                g.FillRectangle(bg, this.ClientRectangle);

            // Draw the bar
            DrawBar(g);

            if (_collapsed) return;

            // Draw keyboard below the bar
            float scale = KbScale();
            float ox = KbOx();
            float oy = KbOy();

            for (int i = 0; i < KeyLayout.Length; i++)
            {
                var k = KeyLayout[i];
                float rx = ox + k.GX * scale;
                float ry = oy + k.GY * scale;
                float rw = k.GW * scale;
                float rh = k.GH * scale;

                bool pressed = (GetAsyncKeyState((int)k.Code) & 0x8000) != 0;
                bool hovered = i == _hoverIndex;

                string desc = ResolveDescription(k);

                Color fill;
                if (k.Code == Keys.None)
                    fill = Color.FromArgb(210, 210, 215); // spacer
                else if (pressed)
                    fill = Color.FromArgb(80, 160, 80);
                else if (hovered)
                    fill = Color.FromArgb(200, 200, 220);
                else
                    fill = Color.White;

                using (var b = new SolidBrush(fill))
                using (var p = new Pen(Color.FromArgb(80, 80, 80), Math.Max(1f, scale * 0.04f)))
                {
                    float pad = scale * 0.03f;
                    var rect = new RectangleF(rx + pad, ry + pad, rw - pad * 2, rh - pad * 2);
                    g.FillRoundedRect(b, rect, scale * 0.15f);
                    g.DrawRoundedRect(p, rect, scale * 0.15f);
                }

                if (!string.IsNullOrEmpty(k.Label))
                {
                    float labelSize = scale * 0.24f;
                    using (var f = new Font("Segoe UI", labelSize, FontStyle.Regular, GraphicsUnit.Pixel))
                    using (var b = new SolidBrush(Color.FromArgb(60, 60, 60)))
                    {
                        var sz = g.MeasureString(k.Label, f);
                        g.DrawString(k.Label, f, b,
                            rx + (rw - sz.Width) / 2f,
                            ry + scale * 0.005f);
                    }
                }

                if (!string.IsNullOrEmpty(desc))
                {
                    string line1, line2;
                    int sp = desc.IndexOf('\n');
                    if (sp > 0) { line1 = desc.Substring(0, sp); line2 = desc.Substring(sp + 1); }
                    else        { line1 = desc; line2 = null; }

                    if (line2 != null)
                    {
                        // Two-line (GripTwist): line1 upper, line2 lower
                        float t1 = scale * 0.30f;
                        using (var f1 = new Font("Segoe UI", t1, FontStyle.Bold, GraphicsUnit.Pixel))
                        using (var b1 = new SolidBrush(Color.FromArgb(30, 100, 180)))
                        {
                            var sz = g.MeasureString(line1, f1);
                            g.DrawString(line1, f1, b1,
                                rx + (rw - sz.Width) / 2f,
                                ry + rh * 0.28f);
                        }
                        float t2 = scale * 0.28f;
                        using (var f2 = new Font("Segoe UI", t2, FontStyle.Bold, GraphicsUnit.Pixel))
                        using (var b2 = new SolidBrush(Color.FromArgb(60, 120, 190)))
                        {
                            var sz = g.MeasureString(line2, f2);
                            g.DrawString(line2, f2, b2,
                                rx + (rw - sz.Width) / 2f,
                                ry + rh * 0.58f);
                        }
                    }
                    else
                    {
                        // Single-line: big text centered in the key
                        float ts = scale * 0.34f;
                        using (var f = new Font("Segoe UI", ts, FontStyle.Bold, GraphicsUnit.Pixel))
                        using (var b = new SolidBrush(Color.FromArgb(30, 100, 180)))
                        {
                            var sz = g.MeasureString(line1, f);
                            g.DrawString(line1, f, b,
                                rx + (rw - sz.Width) / 2f,
                                ry + (rh - sz.Height) / 2f + scale * 0.02f);
                        }
                    }
                }
            }
        }

        void DrawBar(Graphics g)
        {
            var barRect = new Rectangle(0, 0, this.ClientSize.Width, BAR_HEIGHT);
            using (var bg = new SolidBrush(Color.FromArgb(55, 55, 65)))
                g.FillRectangle(bg, barRect);

            string text = _collapsed ? "▶  Keybinds" : "▼  Keybinds";
            using (var f = new Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var b = new SolidBrush(Color.FromArgb(220, 220, 220)))
            {
                var sz = g.MeasureString(text, f);
                g.DrawString(text, f, b,
                    (this.ClientSize.Width - sz.Width) / 2f,
                    (BAR_HEIGHT - sz.Height) / 2f);
            }

            // Thin highlight line at bottom of bar
            using (var p = new Pen(Color.FromArgb(90, 90, 100)))
                g.DrawLine(p, 0, BAR_HEIGHT - 1, this.ClientSize.Width, BAR_HEIGHT - 1);
        }
    }
}
