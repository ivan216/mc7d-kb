using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class KeybindsReference
    {
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

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
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
            if (ViewportFocusRequested != null)
                ViewportFocusRequested();

            if (e.Button == MouseButtons.Left && e.Y <= BAR_HEIGHT)
                _dragStart = e.Location;
        }

        void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _dragStart != Point.Empty)
                ToggleCollapse();
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

                    if (e.X >= rx && e.X <= rx + rw && e.Y >= ry && e.Y <= ry + rh)
                    {
                        _hoverIndex = i;
                        string tip = ResolveTooltip(k);
                        if (!string.IsNullOrEmpty(tip))
                            _tooltip.SetToolTip(this, $"{k.Label} ({k.Code}): {tip}");
                        else
                            _tooltip.SetToolTip(this, $"{k.Label} ({k.Code})");
                        break;
                    }
                }
            }
        }
    }
}
