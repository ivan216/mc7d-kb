using System.Collections.Generic;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class KeybindsReference
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
        static readonly float GRID_H = 5.85f;

        static KeyDef[] BuildLayout()
        {
            var list = new List<KeyDef>();
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

        // Keyboard drawing helpers
        float KbScale() { return (this.ClientSize.Width - 10) / GRID_W; }
        float KbOx()    { return 5f; }
        float KbOy()    { return BAR_HEIGHT + 3f; }
    }
}
