using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class Form1
    {
        /// <summary>Tracks actions activated by currently held keys.
        /// Used for consumed-modifier calculation and KeyUp dispatch.</summary>
        Dictionary<Keys, Keybindings.IAction> _activeKeyActions = new Dictionary<Keys, Keybindings.IAction>();

        Keybindings Keybinds = new Keybindings();
        Form KeybindsSetup;
        KeybindsReference KeybindsRef;
        ToolStripMenuItem _refMenuItem;

        int ClickX,ClickY;
        double cpath=0;

        private bool WantClick(){
            return true;
        }

        private void MouseDownEvt(object sender, MouseEventArgs e) {
            ClickX=e.X; ClickY=e.Y; cpath=0;
            MouseEvt(sender,e);
        }

        void addPath(int x,int y) {
            cpath+=MyMath.pyth(ClickX-x,ClickY-y);
            ClickY=y; ClickX=x;
        }

        private void MouseUpEvt(object sender, MouseEventArgs e) {
            addPath(e.X,e.Y);
            if(WantClick()){
                if(e.Clicks==2 || cpath<=6){
                    ProcessClick(e);
                }
            }
            MouseEvt(sender,e);
        }

        private void MouseEvt(object sender, MouseEventArgs e) {
            addPath(e.X,e.Y);
            if(e.Button==MouseButtons.Left){
                dxControl2.ProcessMouse(e,ETarget.TargetCamera,null);
            }else{
                OnAction func=null;
                ETarget targ=ETarget.TargetCamera;
                dxControl2.ProcessMouse(e,targ,func);
            }
        }

        private void KeyDownEvt(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;

            var action = ResolveKeybindAction(keyCode);

            if (action == null)
            {
                RefreshKeybindsReferenceDisplay();
                return;
            }

            _activeKeyActions[keyCode] = action;

            bool redraw = false, didTwist = false;
            int prevStep = Cube?.partialTwist3c?.step ?? 0;
            action.OnKeyDown(ref Cube, ref redraw, ref didTwist);
            // Clear mouse click state when Twist3c grip is first set (step 0 to 1)
            if (Cube != null && prevStep == 0 && Cube.partialTwist3c.step == 1) {
                NClicks = 0;
                FaceClick = 0;
                FaceFrom = 0;
                ClickQual = true;
            }
            PostKeybindAction(redraw, didTwist);

            RefreshKeybindsReferenceDisplay();
        }

        private void KeyUpEvt(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;

            if (!_activeKeyActions.TryGetValue(keyCode, out var action))
            {
                RefreshKeybindsReferenceDisplay();
                return;
            }

            _activeKeyActions.Remove(keyCode);

            bool redraw = false, didTwist = false;
            action.OnKeyUp(ref Cube, ref redraw, ref didTwist);
            PostKeybindAction(redraw, didTwist);

            RefreshKeybindsReferenceDisplay();
        }

        /// <summary>
        /// Calculate the set of modifier flags that are "consumed" —
        /// modifier keys that are currently held with an active action.
        /// Consumed modifiers are excluded when resolving other key presses,
        /// so holding Shift (for a grip) doesn't prevent other keys from
        /// matching their bindings.
        /// </summary>
        private Keys GetConsumedModifiers()
        {
            Keys consumed = Keys.None;
            foreach (Keys key in _activeKeyActions.Keys)
            {
                consumed |= ChordUtils.GetModifierFlag(key);
            }
            return consumed;
        }

        /// <summary>Convenience: sync ConsumedModifiers to the keyboard ref and redraw.</summary>
        private void RefreshKeybindsReferenceDisplay()
        {
            if (KeybindsRef != null && !KeybindsRef.IsDisposed)
            {
                KeybindsRef.ConsumedModifiers = GetConsumedModifiers();
                KeybindsRef.RefreshDisplay();
            }
        }

        /// <summary>
        /// Release all active key actions and grip state.
        /// Called when the control loses focus or the menu is activated.
        /// </summary>
        private void ReleaseAllKeyboardState()
        {
            // Release all tracked key actions (layers, grips, etc.)
            foreach (var kvp in _activeKeyActions)
            {
                bool redraw = false, didTwist = false;
                kvp.Value.OnKeyUp(ref Cube, ref redraw, ref didTwist);
                if (redraw)
                {
                    ProcessHighLights();
                    Redraw();
                }
            }
            _activeKeyActions.Clear();

            // Release Cube grip if any
            if (Cube != null && Cube.Gripped[0] != -1)
            {
                Cube.Grip(-1, 1);
                ProcessHighLights();
                Redraw();
            }
        }

        private void PostKeybindAction(bool redraw, bool didTwist)
        {

            if (redraw)
            {
                ProcessHighLights();
                Redraw();
                RedrawClickStatus();  // Update status bar for Twist3c progress
            }

            if (didTwist)
            {
                TestBuild();
            }
        }

        private void dxControl2_Leave(object sender, EventArgs e) {
            ReleaseAllKeyboardState();
        }

        private void dxControl2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                e.IsInputKey = true;
        }

        private Keybindings.IAction ResolveKeybindAction(Keys keyCode)
        {
            KeyCombo combo = KeyCombo.FromKeyPress(keyCode);
            Keys consumedMods = GetConsumedModifiers();
            return Keybinds.GetActionWithFallback(
                keyCode.ToString(), combo.Ctrl, combo.Shift, combo.Alt, consumedMods);
        }

        private bool NonViewportUiHasFocus()
        {
            return panel1.ContainsFocus || menuStrip1.ContainsFocus || statusStrip1.ContainsFocus;
        }

        private bool IsDescendantOf(Control child, Control ancestor)
        {
            while (child != null)
            {
                if (child == ancestor) return true;
                child = child.Parent;
            }
            return false;
        }

        private bool ControlKeepsOwnKeyboardBehavior(Control control)
        {
            while (control != null && control != panel1)
            {
                if (control is TextBoxBase || control is ComboBox || control is UpDownBase || control is TrackBar)
                    return true;
                control = control.Parent;
            }
            return false;
        }

        private bool ShouldRoutePanelKeyDown(Control control, Keys keyCode)
        {
            return IsDescendantOf(control, panel1)
                && !ControlKeepsOwnKeyboardBehavior(control)
                && ResolveKeybindAction(keyCode) != null;
        }

        private bool ShouldRoutePanelKeyUp(Control control, Keys keyCode)
        {
            return IsDescendantOf(control, panel1)
                && !ControlKeepsOwnKeyboardBehavior(control)
                && _activeKeyActions.ContainsKey(keyCode);
        }

        private void CheckKeybindSet(object sender, EventArgs e)
        {
            // Clear twist3c state when switching keybind layouts
            if (Cube != null) Cube.partialTwist3c.Reset();
            RedrawClickStatus();

            string active = Keybinds.activeKeybindsName;
            activeKeybind.Text = $"Keybinds: {active}";
            foreach (ToolStripMenuItem item in activeKeybind.DropDownItems)
            {
                item.Checked = item.Name == active;
            }
            RefreshStatusScrollHost();
        }

        // Intercept mouse wheel on TrackBar/NumericUpDown when not focused
        // Redirect to parent panel for scrolling instead of changing the slider value
        class WheelGuard : IMessageFilter {
            Form1 _form;
            public WheelGuard(Form1 form) { _form = form; }
            public bool PreFilterMessage(ref Message m) {
                if (m.Msg != 0x020A) return false;
                Control c = Control.FromChildHandle(m.HWnd);
                // NumericUpDown and TrackBar are composite controls;
                // walk up to find the actual parent control
                while (c != null && !(c is TrackBar) && !(c is NumericUpDown))
                    c = c.Parent;
                if (c == null) return false;
                // Only apply to controls in the main form, not modal dialogs (e.g. KeybindSetup)
                if (c.FindForm() != _form) return false;
                if (!c.Focused && c.Parent is System.Windows.Forms.ScrollableControl sc) {
                    int delta = (short)((m.WParam.ToInt32() >> 16) & 0xFFFF);
                    int sy = -sc.AutoScrollPosition.Y;
                    sc.AutoScrollPosition = new System.Drawing.Point(0, sy - delta);
                    return true;
                }
                return false;
            }
        }

        // Intercept key messages so the keyboard reference repaints reliably,
        // and so sidebar controls can still drive main keybind actions.
        class KeybindsRefreshFilter : IMessageFilter
        {
            Form1 _form;
            public KeybindsRefreshFilter(Form1 form) { _form = form; }
            public bool PreFilterMessage(ref Message m)
            {
                const int WM_KEYDOWN = 0x100;
                const int WM_KEYUP = 0x101;
                const int WM_SYSKEYDOWN = 0x104;
                const int WM_SYSKEYUP = 0x105;

                if (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP || m.Msg == WM_SYSKEYDOWN || m.Msg == WM_SYSKEYUP)
                {
                    var r = _form.KeybindsRef;
                    if (r != null && !r.IsDisposed)
                    {
                        r.ConsumedModifiers = _form.GetConsumedModifiers();
                        r.RefreshDisplay();
                    }

                    Control control = Control.FromChildHandle(m.HWnd);
                    if (control == null || control.FindForm() != _form)
                        return false;

                    Keys keyCode = (Keys)(int)m.WParam;
                    bool handled =
                        (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
                            ? _form.ShouldRoutePanelKeyDown(control, keyCode)
                            : _form.ShouldRoutePanelKeyUp(control, keyCode);

                    if (!handled)
                        return false;

                    var e = new KeyEventArgs(keyCode);
                    if (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
                        _form.KeyDownEvt(control, e);
                    else
                        _form.KeyUpEvt(control, e);

                    return true;
                }
                return false;
            }
        }
    }
}
