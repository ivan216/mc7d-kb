using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class KeybindSetup : Form
    {
        Keybindings keybinds;
        string curKeybindsName;
        Keybindings.KeybindSet curKeybinds;
        MenuStrip _menuStrip;

        /// <summary>Per-TextBox capture state for the chord-capture state machine.</summary>
        class CaptureState
        {
            // Currently held modifier flags
            public bool CtrlHeld, ShiftHeld, AltHeld;
            // Modifier flags ever seen during this capture session
            public bool EverCtrl, EverShift, EverAlt;
            // Locked primary key info
            public bool HasPrimaryKey;
            public string PrimaryKeyName;
            // Whether capture has been finalised (chord locked or invalid)
            public bool IsDone;
            // Whether a chord was committed since the TextBox was last entered
            public bool WasCaptured;
            // The binding key before editing (to restore on cancel / reject)
            public string OriginalKey;
            public Keybindings.IAction OriginalAction;
        }

        Dictionary<TextBox, CaptureState> _captureStates = new Dictionary<TextBox, CaptureState>();

        public KeybindSetup(Keybindings keybinds, Form mainForm, MenuStrip menuStrip)
        {
            InitializeComponent();
            _menuStrip = menuStrip;
            this.keybinds = keybinds;
        }

        private void SetLayout(string name)
        {
            _captureStates.Clear();
            Control addButton = keybindsPanel.Controls[keybindsPanel.Controls.Count - 1];
            foreach (Button btn in keybindSetsPanel.Controls)
            {
                btn.Enabled = true;
                if (btn.Text == name) { btn.Enabled = false; }
            }

            if (!keybinds.keybinds.ContainsKey(name))
            {
                this.Text = "Keybinds Setup";
                curKeybindsName = "";
                curKeybinds = null;
                keybindsPanel.Controls.Clear();
                keybindsPanel.Controls.Add(addButton);

                return;
            }

            curKeybindsName = name;
            curKeybinds = keybinds.keybinds[name];
            this.Text = $"Keybinds Setup - {name}";

            keybindsPanel.Controls.Clear();
            foreach (var item in curKeybinds.binds)
            {
                FlowLayoutPanel panel = CreateKeybindPanel(item.Key, item.Value);
                keybindsPanel.Controls.Add(panel);
            }
            keybindsPanel.Controls.Add(addButton);
            keybindsPanel.Focus();
        }

        private FlowLayoutPanel CreateKeybindPanel(string key, Keybindings.IAction action)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Name = key,
                Height = 28,
                WrapContents = false,
                AutoSize = true,
            };

            TextBox textBox = new TextBox
            {
                Text = key,
                Name = "textBox" + key,
                Size = new Size(192, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };

            // Set up chord-capture state machine
            var capState = new CaptureState
            {
                OriginalKey = key,
                OriginalAction = action,
                IsDone = true, // idle until this TextBox gets focus
            };
            _captureStates[textBox] = capState;

            textBox.Enter += (s, e) =>
            {
                // Reset state when user starts editing this box
                var state = _captureStates[(TextBox)s];
                state.CtrlHeld = state.ShiftHeld = state.AltHeld = false;
                state.EverCtrl = state.EverShift = state.EverAlt = false;
                state.HasPrimaryKey = false;
                state.PrimaryKeyName = null;
                state.IsDone = false;
                state.WasCaptured = false;
            };

            textBox.KeyDown += Capture_KeyDown;
            textBox.KeyUp += Capture_KeyUp;
            // Suppress all character insertion — the TextBox content is
            // entirely controlled by the capture state machine via tb.Text.
            textBox.KeyPress += (s, e) => e.Handled = true;

            textBox.Leave += (s, e) =>
            {
                var tb = (TextBox)s;
                var state = _captureStates[tb];
                // If capture was not completed, restore original key
                if (!state.IsDone || string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = state.OriginalKey;
                }
                state.IsDone = true;
            };

            ComboBox comboBox = new ComboBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                DropDownStyle = ComboBoxStyle.DropDownList,
                ItemHeight = 24,
                Name = "comboBox" + key,
                Size = new Size(120, 30),
            };

            FlowLayoutPanel extra = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Margin = new Padding(0),
                WrapContents = false,
                AutoSize = true,
            };
            var extras = action.SetupControls();
            extra.Controls.AddRange(extras);

            var actions = Keybindings.ActionFactories.Keys.ToArray();
            comboBox.Items.AddRange(actions);
            comboBox.SelectedIndex = comboBox.Items.IndexOf(action.GetType().Name);
            comboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;
            comboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
            {
                capState.OriginalAction = Keybindings.ActionFactories[(string)comboBox.SelectedItem]();
                action = capState.OriginalAction;
                extra.Controls.Clear();
                extra.Controls.AddRange(action.SetupControls());

                if (textBox.Text != "")
                {
                    curKeybinds.binds[textBox.Text] = action;
                }
            };

            Button delete = new Button
            {
                Size = new Size { Height = 20, Width = 20 },
                Text = "×",
            };
            delete.Click += (object sender, EventArgs e) =>
            {
                var confirmResult = MessageBox.Show($"Are you sure you want to delete {comboBox.SelectedItem} keybind for \"{textBox.Text}\"?",
                                    "Confirm Delete",
                                    MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    string curKey = textBox.Text;
                    if (!string.IsNullOrEmpty(curKey) && curKeybinds.binds.ContainsKey(curKey))
                        curKeybinds.binds.Remove(curKey);
                    keybindsPanel.Controls.Remove(panel);
                }
            };

            panel.Controls.AddRange(new Control[] { 
                delete,
                textBox,
                comboBox,
                extra
            });

            return panel;
        }

        private void CreateLayoutButton(string name)
        {
            Button btn = new Button();
            btn.Text = name;
            btn.Name = name;
            btn.Click += new System.EventHandler(this.SwitchLayout_Click);
            btn.Width = addNewLayout.Width;

            this.keybindSetsPanel.Controls.Add(btn);
        }

        private void KeybindSetup_Load(object sender, EventArgs e)
        {
            foreach (var item in keybinds.keybinds) {
                CreateLayoutButton(item.Key);
            }

            SetLayout(keybinds.activeKeybindsName);
        }

        private void AddNewLayout_Click(object sender, EventArgs e)
        {
            TextDialog td = new TextDialog("Enter Layout Name");
            DialogResult res = td.ShowDialog(this);
            if (res.Equals(DialogResult.OK))
            {
                string name = td.Value.Replace(' ', '_').Replace(',', '_');
                if (keybinds.CreateKeybindSet(name))
                {
                    CreateLayoutButton(name);
                }
            }
        }

        private void DeleteLayout_Click(object sender, EventArgs e)
        {
            if (curKeybindsName == "")
            {
                MessageBox.Show("No layout selected");
                return;
            }

            var confirmResult = MessageBox.Show($"Are you sure you want to delete keybind layout \"{curKeybindsName}\"?",
                                    "Confirm Delete",
                                    MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                keybinds.DeleteKeybindSet(curKeybindsName);
                Control[] res = this.keybindSetsPanel.Controls.Find(curKeybindsName, false);
                foreach (var item in res)
                {
                    keybindSetsPanel.Controls.Remove(item);
                }
                SetLayout("");
            }
        }

        private void AddKeybind_Click(object sender, EventArgs e)
        {
            if (curKeybindsName == "")
            {
                MessageBox.Show("No layout selected");
                return;
            }

            Control addButton = keybindsPanel.Controls[keybindsPanel.Controls.Count - 1];

            var action = new Keybindings.Twist();
            Control panel = CreateKeybindPanel("", action);

            keybindsPanel.Controls.Remove(addButton);
            keybindsPanel.Controls.Add(panel);
            keybindsPanel.Controls.Add(addButton);
        }

        private void SwitchLayout_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            SetLayout(btn.Text);
        }

        private void Capture_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var state = _captureStates[tb];
            e.SuppressKeyPress = true;

            if (state.IsDone)
            {
                // Previous capture completed or rejected — starting fresh.
                // Sync modifier flags from the actual physical key state
                // (KeyUp events were suppressed by IsDone, so the tracked
                // state may be stale).
                state.IsDone = false;
                state.HasPrimaryKey = false;
                state.PrimaryKeyName = null;
                state.WasCaptured = false;
                Keys mods = Control.ModifierKeys;
                state.CtrlHeld = (mods & Keys.Control) != 0;
                state.ShiftHeld = (mods & Keys.Shift) != 0;
                state.AltHeld = (mods & Keys.Alt) != 0;
                state.EverCtrl = state.CtrlHeld;
                state.EverShift = state.ShiftHeld;
                state.EverAlt = state.AltHeld;
                // Fall through to process this key
            }

            Keys keyCode = e.KeyCode;

            // ---- Modifier key pressed ----
            if (ChordUtils.IsModifierKey(keyCode))
            {
                var flag = ChordUtils.GetModifierFlag(keyCode);
                if (flag == Keys.Control) { state.CtrlHeld = true; state.EverCtrl = true; }
                if (flag == Keys.Shift)  { state.ShiftHeld = true; state.EverShift = true; }
                if (flag == Keys.Alt)    { state.AltHeld = true; state.EverAlt = true; }

                // Don't finalise yet — a primary key may follow
                return;
            }

            // ---- Primary key pressed ----
            string keyName = keyCode.ToString();

            if (state.HasPrimaryKey)
            {
                // Second primary key → reject
                state.IsDone = true;
                tb.Text = state.OriginalKey;
                MessageBox.Show("Only one primary key is allowed", "Invalid chord",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // First primary key in this session → lock the chord
            state.HasPrimaryKey = true;
            state.PrimaryKeyName = keyName;

            string chord = ChordUtils.BuildChord(state.CtrlHeld, state.ShiftHeld, state.AltHeld, keyName);
            FinaliseCapture(tb, state, chord);
        }

        private void Capture_KeyUp(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var state = _captureStates[tb];

            if (state.IsDone) return;

            Keys keyCode = e.KeyCode;

            if (!ChordUtils.IsModifierKey(keyCode))
            {
                // Primary key released — nothing more to do here (already locked in KeyDown)
                return;
            }

            // Modifier released — update state
            var flag = ChordUtils.GetModifierFlag(keyCode);
            if (flag == Keys.Control) state.CtrlHeld = false;
            if (flag == Keys.Shift) state.ShiftHeld = false;
            if (flag == Keys.Alt) state.AltHeld = false;

            // Still holding some modifiers, or already have a primary key → keep waiting
            if (state.CtrlHeld || state.ShiftHeld || state.AltHeld || state.HasPrimaryKey)
                return;

            // ---- All modifiers released, no primary key appeared ----

            // If a chord was previously committed, these are stale modifier
            // releases from the old capture. Don't show an error — just
            // wait for the next key press to start a fresh capture.
            if (state.WasCaptured)
            {
                state.IsDone = true;
                return;
            }

            // After a rejection, Ever* flags were cleared by the re-capture
            // path in KeyDown. If a stale KeyUp sneaks in here, treat it as
            // idle rather than showing an error.
            if (!state.EverCtrl && !state.EverShift && !state.EverAlt
                && !state.CtrlHeld && !state.ShiftHeld && !state.AltHeld
                && !state.HasPrimaryKey)
            {
                return;
            }

            int modifierCount = (state.EverCtrl ? 1 : 0)
                              + (state.EverShift ? 1 : 0)
                              + (state.EverAlt ? 1 : 0);

            if (modifierCount == 1)
            {
                // Single modifier key → use as primary key
                string chord = ChordUtils.BuildChord(false, false, false, keyCode.ToString());
                FinaliseCapture(tb, state, chord);
            }
            else
            {
                // Multiple modifier keys, no primary key → reject
                state.IsDone = true;
                tb.Text = state.OriginalKey;
                MessageBox.Show("A primary key is required", "Invalid chord",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Called when a valid chord has been captured.
        /// Updates the TextBox, validates against duplicates and menu shortcuts,
        /// and commits the binding.
        /// </summary>
        private void FinaliseCapture(TextBox tb, CaptureState state, string chord)
        {
            state.IsDone = true;

            // Normalise the chord string
            chord = ChordUtils.Normalize(chord);
            if (chord == null)
            {
                tb.Text = state.OriginalKey;
                return;
            }

            // Reject if same as the current layout's existing key (and it changed)
            if (chord != state.OriginalKey && curKeybinds.binds.ContainsKey(chord))
            {
                tb.Text = state.OriginalKey;
                MessageBox.Show($"This chord is already used in the current layout",
                    "Duplicate chord", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Reject if reserved by a menu shortcut
            if (IsMenuShortcut(chord))
            {
                tb.Text = state.OriginalKey;
                MessageBox.Show($"This chord is reserved by a menu shortcut",
                    "Reserved chord", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Commit the binding update
            tb.Text = chord;
            curKeybinds.binds.Remove(state.OriginalKey);
            curKeybinds.binds.Add(chord, state.OriginalAction);
            state.OriginalKey = chord;
            state.WasCaptured = true;
        }

        /// <summary>
        /// Check whether a chord is reserved by a WinForms menu shortcut
        /// on the main form's MenuStrip.
        /// </summary>
        private bool IsMenuShortcut(string chord)
        {
            if (_menuStrip == null) return false;
            return ChordUtils.IsMenuShortcutChord(chord, _menuStrip);
        }
    }
}
