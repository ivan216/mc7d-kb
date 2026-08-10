using System;
using System.Drawing;
using System.Windows.Forms;

namespace _3dedit
{
    /// <summary>
    /// Visual keyboard overlay that shows which action each key is bound to,
    /// inspired by Hyperspeedcube's keybinds reference.
    /// </summary>
    public partial class KeybindsReference : Form
    {
        // ---- Instance members ----

        Keybindings _keybinds;
        MenuStrip _menu;
        ToolTip _tooltip;
        int _hoverIndex = -1;

        /// <summary>Modifier flags consumed by active key actions (set by Form1 before refresh).</summary>
        public Keys ConsumedModifiers = Keys.None;

        /// <summary>Forwards physical key presses when this window has focus.</summary>
        public Action<Keys> PhysicalKeyDown;
        /// <summary>Forwards physical key releases when this window has focus.</summary>
        public Action<Keys> PhysicalKeyUp;
        /// <summary>Requests that the main viewport regain keyboard focus when this window is clicked.</summary>
        public Action ViewportFocusRequested;
        public KeybindsReference(Keybindings keybinds, MenuStrip menu)
        {
            _keybinds = keybinds;
            _menu = menu;
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

            // Refresh when the user switches keybind layouts
            _keybinds.ActiveLayoutChanged += (s, e) => { if (!_collapsed) Invalidate(); };
        }
    }
}
