using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3dedit
{
    /// <summary>
    /// Visual keyboard overlay that shows which action each key is bound to,
    /// inspired by Hyperspeedcube's keybinds reference.
    /// </summary>
    public class KeybindsReference : Form
    {
        // ---- Grid layout (22.5 × 6.5 units) ----

        struct KeyDef
        {
            public Keys Code;
            public string Label;
            public float GX, GY, GW, GH; // grid position/size

            public KeyDef(Keys code, string label, float gx, float gy, float gw = 1f, float gh = 1f)
            {
                Code = code; Label = label;
                GX = gx; GY = gy; GW = gw; GH = gh;
            }
        }

        static KeyDef[] KeyLayout = BuildLayout();
        static readonly float GRID_W = 19.0f;
        static readonly float GRID_H = 6.0f;

        static KeyDef[] BuildLayout()
        {
            var list = new System.Collections.Generic.List<KeyDef>();
            float x, y;
            float fw = 15f / 16f; // ~0.9375 each — Esc F1-12 Ins PrtSc Del fill x=0..15
            float fh = 0.85f;     // shorter height for the function row

            // ---- Row 0: Equal-width F-keys + Ins PrtSc Del + Nav keys (y=0) ----
            x = 0; y = 0;
            Add(list, Keys.Escape, "Esc", x, y, fw, fh); x += fw;
            Add(list, Keys.F1,  "F1",  x, y, fw, fh); x += fw;
            Add(list, Keys.F2,  "F2",  x, y, fw, fh); x += fw;
            Add(list, Keys.F3,  "F3",  x, y, fw, fh); x += fw;
            Add(list, Keys.F4,  "F4",  x, y, fw, fh); x += fw;
            Add(list, Keys.F5,  "F5",  x, y, fw, fh); x += fw;
            Add(list, Keys.F6,  "F6",  x, y, fw, fh); x += fw;
            Add(list, Keys.F7,  "F7",  x, y, fw, fh); x += fw;
            Add(list, Keys.F8,  "F8",  x, y, fw, fh); x += fw;
            Add(list, Keys.F9,  "F9",  x, y, fw, fh); x += fw;
            Add(list, Keys.F10, "F10", x, y, fw, fh); x += fw;
            Add(list, Keys.F11, "F11", x, y, fw, fh); x += fw;
            Add(list, Keys.F12, "F12", x, y, fw, fh); x += fw;
            Add(list, Keys.Insert,     "Ins",   x, y, fw, fh); x += fw;
            Add(list, Keys.PrintScreen, "PrtSc", x, y, fw, fh); x += fw;
            Add(list, Keys.Delete,     "Del",   x, y, fw, fh); x += fw;

            // Nav keys at y=0, flush right against numpad columns
            x = 15f; y = 0;
            Add(list, Keys.Home,    "Home", x, y, 1f, fh); x += 1f;
            Add(list, Keys.End,     "End",  x, y, 1f, fh); x += 1f;
            Add(list, Keys.PageUp,  "PgUp", x, y, 1f, fh); x += 1f;
            Add(list, Keys.PageDown,"PgDn", x, y, 1f, fh); x += 1f;

            // ---- Row 1: Numbers + Numpad row 1 (y=1.2) ----
            x = 0; y = 0.85f;
            Add(list, Keys.Oemtilde,  "~", x, y); x += 1f;
            Add(list, Keys.D1,  "1",  x, y); x += 1f;
            Add(list, Keys.D2,  "2",  x, y); x += 1f;
            Add(list, Keys.D3,  "3",  x, y); x += 1f;
            Add(list, Keys.D4,  "4",  x, y); x += 1f;
            Add(list, Keys.D5,  "5",  x, y); x += 1f;
            Add(list, Keys.D6,  "6",  x, y); x += 1f;
            Add(list, Keys.D7,  "7",  x, y); x += 1f;
            Add(list, Keys.D8,  "8",  x, y); x += 1f;
            Add(list, Keys.D9,  "9",  x, y); x += 1f;
            Add(list, Keys.D0,  "0",  x, y); x += 1f;
            Add(list, Keys.OemMinus,  "-",  x, y); x += 1f;
            Add(list, Keys.Oemplus,   "=",  x, y); x += 1f;
            Add(list, Keys.Back, "Bksp", x, y, 2f); x += 2f;

            x = 15f; y = 0.85f;
            Add(list, Keys.NumLock,  "NumLk", x, y); x += 1f;
            Add(list, Keys.Divide,   "/",     x, y); x += 1f;
            Add(list, Keys.Multiply, "*",     x, y); x += 1f;
            Add(list, Keys.Subtract, "-",     x, y); x += 1f;

            // ---- Row 2: QWERTY + Numpad 7-9+ (y=2.2) ----
            x = 0; y = 1.85f;
            Add(list, Keys.Tab,  "Tab", x, y, 1.5f); x += 1.5f;
            Add(list, Keys.Q,  "Q",   x, y); x += 1f;
            Add(list, Keys.W,  "W",   x, y); x += 1f;
            Add(list, Keys.E,  "E",   x, y); x += 1f;
            Add(list, Keys.R,  "R",   x, y); x += 1f;
            Add(list, Keys.T,  "T",   x, y); x += 1f;
            Add(list, Keys.Y,  "Y",   x, y); x += 1f;
            Add(list, Keys.U,  "U",   x, y); x += 1f;
            Add(list, Keys.I,  "I",   x, y); x += 1f;
            Add(list, Keys.O,  "O",   x, y); x += 1f;
            Add(list, Keys.P,  "P",   x, y); x += 1f;
            Add(list, Keys.OemOpenBrackets,  "[",  x, y); x += 1f;
            Add(list, Keys.OemCloseBrackets, "]",  x, y); x += 1f;
            Add(list, Keys.OemPipe, "\\", x, y, 1.5f); x += 1.5f;

            x = 15f; y = 1.85f;
            Add(list, Keys.NumPad7, "7", x, y); x += 1f;
            Add(list, Keys.NumPad8, "8", x, y); x += 1f;
            Add(list, Keys.NumPad9, "9", x, y); x += 1f;
            Add(list, Keys.Add,     "+", x, y, 1f, 2f);

            // ---- Row 3: Caps + Numpad 4-6 (y=3.2) ----
            x = 0; y = 2.85f;
            Add(list, Keys.CapsLock, "Caps", x, y, 1.75f); x += 1.75f;
            Add(list, Keys.A,  "A",  x, y); x += 1f;
            Add(list, Keys.S,  "S",  x, y); x += 1f;
            Add(list, Keys.D,  "D",  x, y); x += 1f;
            Add(list, Keys.F,  "F",  x, y); x += 1f;
            Add(list, Keys.G,  "G",  x, y); x += 1f;
            Add(list, Keys.H,  "H",  x, y); x += 1f;
            Add(list, Keys.J,  "J",  x, y); x += 1f;
            Add(list, Keys.K,  "K",  x, y); x += 1f;
            Add(list, Keys.L,  "L",  x, y); x += 1f;
            Add(list, Keys.OemSemicolon, ";",  x, y); x += 1f;
            Add(list, Keys.OemQuotes, "'",  x, y); x += 1f;
            Add(list, Keys.Enter, "Enter", x, y, 2.25f); x += 2.25f;

            x = 15f; y = 2.85f;
            Add(list, Keys.NumPad4, "4", x, y); x += 1f;
            Add(list, Keys.NumPad5, "5", x, y); x += 1f;
            Add(list, Keys.NumPad6, "6", x, y); x += 1f;

            // ---- Row 4: Shift + Numpad 1-3 Ent (y=4.2) ----
            x = 0; y = 3.85f;
            Add(list, Keys.LShiftKey, "Shift", x, y, 2.25f); x += 2.25f;
            Add(list, Keys.Z,  "Z",  x, y); x += 1f;
            Add(list, Keys.X,  "X",  x, y); x += 1f;
            Add(list, Keys.C,  "C",  x, y); x += 1f;
            Add(list, Keys.V,  "V",  x, y); x += 1f;
            Add(list, Keys.B,  "B",  x, y); x += 1f;
            Add(list, Keys.N,  "N",  x, y); x += 1f;
            Add(list, Keys.M,  "M",  x, y); x += 1f;
            Add(list, Keys.Oemcomma,   ",",  x, y); x += 1f;
            Add(list, Keys.OemPeriod,  ".",  x, y); x += 1f;
            Add(list, Keys.OemQuestion,"/",  x, y); x += 1f;
            Add(list, Keys.Up,    "↑", x, y); x += 1f;
            Add(list, Keys.RShiftKey, "Shift", x, y, 1.75f); x += 1.75f;

            x = 15f; y = 3.85f;
            Add(list, Keys.NumPad1, "1",   x, y); x += 1f;
            Add(list, Keys.NumPad2, "2",   x, y); x += 1f;
            Add(list, Keys.NumPad3, "3",   x, y); x += 1f;
            Add(list, Keys.Enter,   "Ent", x, y, 1f, 2f);

            // ---- Row 5: Bottom (y=4.85) ----
            x = 0; y = 4.85f;
            Add(list, Keys.LControlKey, "Ctrl", x, y, 1.25f); x += 1.25f;
            list.Add(new KeyDef(Keys.None, "", x, y, 1f, 1f)); x += 1f; // spacer
            Add(list, Keys.LWin,   "Win",  x, y, 1.25f); x += 1.25f;
            Add(list, Keys.LMenu,  "Alt",  x, y, 1.25f); x += 1.25f;
            Add(list, Keys.Space,  "",     x, y, 4f);    x += 4f;
            Add(list, Keys.RMenu,  "Alt",  x, y, 1.25f); x += 1.25f;
            Add(list, Keys.RControlKey, "Ctrl", x, y, 1.25f); x += 1.25f;
            Add(list, Keys.Left,  "←", x, y); x += 1f;
            Add(list, Keys.Down,  "↓", x, y); x += 1f;
            Add(list, Keys.Right, "→", x, y);

            x = 15f; y = 4.85f;
            Add(list, Keys.NumPad0, "0", x, y, 2f); x += 2f;
            Add(list, Keys.Decimal, ".", x, y, 1f);

            return list.ToArray();
        }

        static void Add(System.Collections.Generic.List<KeyDef> list, Keys code, string label, float gx, float gy, float gw = 1f, float gh = 1f)
        {
            if (code != Keys.None)
                list.Add(new KeyDef(code, label, gx, gy, gw, gh));
        }

        // ---- Layout constants ----

        const int BAR_HEIGHT = 28;
        const int KB_X = 5;              // horizontal padding
        const int BOTTOM_PAD = 5;        // bottom padding
        // Total non-keyboard vertical pixels
        int NonKbV() { return BAR_HEIGHT + 3 + BOTTOM_PAD; }

        // Window client width → total client height (keyboard keeps GRID ratio)
        int WindowHFromW(int w)   { return (int)(NonKbV() + (w - KB_X * 2f) / GRID_W * GRID_H); }
        // Total client height → window client width
        int WindowWFromH(int h)   { return (int)(KB_X * 2f + (h - NonKbV()) * GRID_W / GRID_H); }

        // ---- Dragging and collapse ----

        const int COLLAPSED_W = 200;
        bool _collapsed = false;
        Size _lastExpandedSize;
        Point _dragStart = Point.Empty;

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 2;

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int left, top, right, bottom; }

        // ---- Instance members ----

        Keybindings _keybinds;
        ToolTip _tooltip;
        int _hoverIndex = -1;

        /// <summary>Forwards physical key presses when this window has focus.</summary>
        public Action<Keys> PhysicalKeyDown;
        /// <summary>Forwards physical key releases when this window has focus.</summary>
        public Action<Keys> PhysicalKeyUp;

        public KeybindsReference(Keybindings keybinds)
        {
            _keybinds = keybinds;
            _tooltip = new ToolTip();
            _tooltip.ShowAlways = true;

            this.Text = "";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 100);
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(240, 240, 240);

            _lastExpandedSize = new Size(950, WindowHFromW(950));
            ApplySize(_lastExpandedSize);

            this.Resize += (s, e) => Invalidate();
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.MouseLeave += (s, e) => { _hoverIndex = -1; Invalidate(); };

            // Forward physical key events to the main form when this window
            // (accidentally) gets focus, so shortcuts still execute.
            this.KeyPreview = true;
            this.KeyDown += (s, ke) => { if (PhysicalKeyDown != null) PhysicalKeyDown(ke.KeyCode); };
            this.KeyUp += (s, ke) => { if (PhysicalKeyUp != null) PhysicalKeyUp(ke.KeyCode); };
        }

        /// <summary>Set size and clamp position to keep window on-screen.</summary>
        void ApplySize(Size s)
        {
            this.Size = s;
            var screen = Screen.FromControl(this).WorkingArea;
            int x = this.Left, y = this.Top;
            if (x < screen.Left) x = screen.Left;
            if (y < screen.Top) y = screen.Top;
            if (x + this.Width > screen.Right) x = screen.Right - this.Width;
            if (y + this.Height > screen.Bottom) y = screen.Bottom - this.Height;
            if (x != this.Left || y != this.Top) this.Location = new Point(x, y);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEACTIVATE = 0x0021;
            const int MA_NOACTIVATE = 3;

            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTLEFT = 10, HTRIGHT = 11;
            const int HTTOP = 12, HTTOPLEFT = 13, HTTOPRIGHT = 14;
            const int HTBOTTOM = 15, HTBOTTOMLEFT = 16, HTBOTTOMRIGHT = 17;

            const int WM_SIZING = 0x0214;
            const int WMSZ_TOP = 3, WMSZ_BOTTOM = 6;

            const int EDGE = 5;

            if (m.Msg == WM_MOUSEACTIVATE)
            {
                // Prevent focus-stealing; mouse events still arrive normally
                m.Result = (IntPtr)MA_NOACTIVATE;
                return;
            }

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if (m.Result != (IntPtr)HTCLIENT) return;

                int x = m.LParam.ToInt32() & 0xFFFF;
                int y = (m.LParam.ToInt32() >> 16) & 0xFFFF;
                Point p = this.PointToClient(new Point(x, y));

                if (_collapsed) return;

                bool l = p.X < EDGE, r = p.X >= this.ClientSize.Width - EDGE;
                bool t = p.Y < EDGE, b = p.Y >= this.ClientSize.Height - EDGE;

                if      (l && t) m.Result = (IntPtr)HTTOPLEFT;
                else if (r && t) m.Result = (IntPtr)HTTOPRIGHT;
                else if (l && b) m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (r && b) m.Result = (IntPtr)HTBOTTOMRIGHT;
                else if (l)      m.Result = (IntPtr)HTLEFT;
                else if (r)      m.Result = (IntPtr)HTRIGHT;
                else if (t)      m.Result = (IntPtr)HTTOP;
                else if (b)      m.Result = (IntPtr)HTBOTTOM;
                return;
            }

            if (m.Msg == WM_SIZING && !_collapsed)
            {
                RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                int w = rc.right - rc.left;
                int h = rc.bottom - rc.top;
                if (w > 0 && h > 0)
                {
                    int edge = m.WParam.ToInt32();
                    if (edge == WMSZ_TOP || edge == WMSZ_BOTTOM)
                    {
                        // Height-driven: derive width
                        int newW = WindowWFromH(h);
                        if (newW >= COLLAPSED_W) rc.right = rc.left + newW;
                    }
                    else
                    {
                        // Width-driven (or corner): derive height
                        int newH = WindowHFromW(w);
                        if (newH >= NonKbV() + 20) rc.bottom = rc.top + newH;
                    }
                    Marshal.StructureToPtr(rc, m.LParam, false);
                }
            }

            base.WndProc(ref m);
        }

        // ---- Toggle collapse ----

        void ToggleCollapse()
        {
            _collapsed = !_collapsed;
            if (_collapsed)
            {
                _lastExpandedSize = this.Size;
                ApplySize(new Size(COLLAPSED_W, BAR_HEIGHT + 2));
            }
            else
                ApplySize(_lastExpandedSize);
            Invalidate();
        }

        // ---- Mouse handlers ----

        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Y <= BAR_HEIGHT)
                _dragStart = e.Location;
        }

        void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _dragStart != Point.Empty)
            {
                _dragStart = Point.Empty;
                ToggleCollapse();
            }
            _dragStart = Point.Empty;
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragStart != Point.Empty && e.Button == MouseButtons.Left)
            {
                int dx = e.X - _dragStart.X;
                int dy = e.Y - _dragStart.Y;
                if (Math.Abs(dx) > 3 || Math.Abs(dy) > 3)
                {
                    _dragStart = Point.Empty;
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                    return;
                }
            }

            _hoverIndex = -1;
            if (!_collapsed)
            {
                float scale = (this.ClientSize.Width - 10) / GRID_W;
                float ox = 5f;
                float oy = BAR_HEIGHT + 3f;

                for (int i = 0; i < KeyLayout.Length; i++)
                {
                    var k = KeyLayout[i];
                    float rx = ox + k.GX * scale;
                    float ry = oy + k.GY * scale;
                    float rw = k.GW * scale;
                    float rh = k.GH * scale;

                    if (e.X >= rx && e.X <= rx + rw && e.Y >= ry && e.Y <= ry + rh)
                    {
                        _hoverIndex = i;
                        string desc = ResolveDescription(k);
                        if (!string.IsNullOrEmpty(desc))
                            _tooltip.SetToolTip(this, $"{k.Label} ({k.Code}): {desc}");
                        else
                            _tooltip.SetToolTip(this, $"{k.Label} ({k.Code})");
                        break;
                    }
                }
            }
        }

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
            float scale = (this.ClientSize.Width - 10) / GRID_W;
            float ox = 5f;
            float oy = BAR_HEIGHT + 3f;

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
                using (var p = new Pen(Color.FromArgb(80, 80, 80), Math.Max(1f, scale * 0.06f)))
                {
                    float pad = scale * 0.08f;
                    var rect = new RectangleF(rx + pad, ry + pad, rw - pad * 2, rh - pad * 2);
                    g.FillRoundedRect(b, rect, scale * 0.15f);
                    g.DrawRoundedRect(p, rect, scale * 0.15f);
                }

                if (!string.IsNullOrEmpty(k.Label))
                {
                    float labelSize = scale * 0.22f;
                    using (var f = new Font("Segoe UI", labelSize, FontStyle.Regular, GraphicsUnit.Pixel))
                    using (var b = new SolidBrush(Color.FromArgb(60, 60, 60)))
                    {
                        var sz = g.MeasureString(k.Label, f);
                        g.DrawString(k.Label, f, b,
                            rx + (rw - sz.Width) / 2f,
                            ry + scale * 0.1f);
                    }
                }

                if (!string.IsNullOrEmpty(desc))
                {
                    float descSize = scale * 0.2f;
                    using (var f = new Font("Segoe UI", descSize, FontStyle.Regular, GraphicsUnit.Pixel))
                    using (var b = new SolidBrush(Color.FromArgb(30, 100, 180)))
                    {
                        var sz = g.MeasureString(desc, f);
                        g.DrawString(desc, f, b,
                            rx + (rw - sz.Width) / 2f,
                            ry + rh * 0.55f);
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

        string ResolveDescription(KeyDef k)
        {
            if (_keybinds == null || _keybinds.activeKeybinds == null)
                return "";

            string keyName = k.Code.ToString();
            var action = _keybinds.GetActionWithFallback(keyName, false, false, false, Keys.None);
            if (action == null)
                return "";

            return action.GetDescription();
        }

    }

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
