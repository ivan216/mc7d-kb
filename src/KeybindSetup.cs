using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class KeybindSetup : Form
    {
        Keybindings keybinds;
        string curKeybindsName;
        Keybindings.KeybindSet curKeybinds;
        MenuStrip _menuStrip;

        /// <summary>Per-TextBox capture state.</summary>
        class CaptureState
        {
            public bool IsActive;
            public bool HasPrimaryKey;
            /// <summary>True when a modifier key was pressed during this capture
            /// session. Guards against stale KeyUp events from modifiers
            /// held before entering the textbox.</summary>
            public bool EverModifier;
            public string OriginalKey;
            public Keybindings.IAction OriginalAction;
        }

        Dictionary<TextBox, CaptureState> _captureStates = new Dictionary<TextBox, CaptureState>();

        public KeybindSetup(Keybindings keybinds, MenuStrip menuStrip)
        {
            InitializeComponent();
            this.keybinds = keybinds;
            _menuStrip = menuStrip;
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

            var capState = new CaptureState
            {
                OriginalKey = key,
                OriginalAction = action,
                IsActive = false,  // starts idle
            };
            _captureStates[textBox] = capState;

            textBox.Enter += (s, e) =>
            {
                var state = _captureStates[(TextBox)s];
                state.IsActive = true;
                state.HasPrimaryKey = false;
                state.EverModifier = false;
            };

            textBox.KeyDown += Capture_KeyDown;
            textBox.KeyUp += Capture_KeyUp;
            // Suppress character insertion — chord text is set programmatically
            textBox.KeyPress += (s, e) => e.Handled = true;

            textBox.Leave += (s, e) =>
            {
                var tb = (TextBox)s;
                var state = _captureStates[tb];
                if (!state.IsActive)
                    return;
                state.IsActive = false;
                // If capture was not completed, restore original key
                if (string.IsNullOrEmpty(tb.Text) || !curKeybinds.binds.ContainsKey(tb.Text))
                {
                    tb.Text = state.OriginalKey;
                }
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

        // ---- Chord capture ----

        private void Capture_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var state = _captureStates[tb];
            e.SuppressKeyPress = true;

            if (!state.IsActive)
            {
                // Previous capture completed or rejected — starting fresh.
                state.IsActive = true;
                state.HasPrimaryKey = false;
                state.EverModifier = false;
                // Fall through to process this key
            }

            Keys keyCode = e.KeyCode;
            KeyCombo combo = KeyCombo.FromKeyPress(keyCode);

            // Modifier key pressed — preview current composite state
            if (ChordUtils.IsModifierKey(keyCode))
            {
                state.EverModifier = true;
                UpdateCapturePreview(tb, combo);
                return;
            }

            // Primary (non-modifier) key pressed — finalise the chord
            string chord = combo.ToChordString();
            FinaliseCapture(tb, state, chord);
        }

        private void Capture_KeyUp(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var state = _captureStates[tb];

            if (!state.IsActive || state.HasPrimaryKey)
                return;

            Keys keyCode = e.KeyCode;
            if (!ChordUtils.IsModifierKey(keyCode))
                return;

            // Check if ANY modifier is still physically held
            Keys mods = Control.ModifierKeys;
            if ((mods & (Keys.Control | Keys.Shift | Keys.Alt)) != 0)
            {
                // Still holding modifiers — update the preview
                KeyCombo combo = KeyCombo.FromKeyPress(keyCode);
                UpdateCapturePreview(tb, combo);
                return;
            }

            // All modifiers released with no primary key pressed.
            // Only finalise if a modifier was actually pressed during capture
            // (not a stale KeyUp from a modifier held before entering).
            if (state.EverModifier)
                FinaliseCapture(tb, state, keyCode.ToString());
            else
                UpdateCapturePreview(tb, KeyCombo.FromKeyPress(keyCode));
        }

        /// <summary>
        /// Show the current modifier state in the TextBox while capturing.
        /// </summary>
        private void UpdateCapturePreview(TextBox tb, KeyCombo combo)
        {
            string prefix = ChordUtils.BuildChord(combo.Ctrl, combo.Shift, combo.Alt, "");
            if (!string.IsNullOrEmpty(prefix))
                tb.Text = prefix + "+…";
            else
                tb.Text = "(press a key)";
        }

        /// <summary>
        /// Validate and commit a captured chord.
        /// </summary>
        private void FinaliseCapture(TextBox tb, CaptureState state, string chord)
        {
            // Normalise
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

            // Commit
            state.IsActive = false;
            state.HasPrimaryKey = true;
            tb.Text = chord;
            curKeybinds.binds.Remove(state.OriginalKey);
            curKeybinds.binds.Add(chord, state.OriginalAction);
            state.OriginalKey = chord;
        }

        private bool IsMenuShortcut(string chord)
        {
            if (_menuStrip == null) return false;
            return ChordUtils.IsMenuShortcutChord(chord, _menuStrip);
        }
    }
}
