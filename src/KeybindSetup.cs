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

        static Dictionary<string, Func<Keybindings.IAction>> actionList = new Dictionary<string, Func<Keybindings.IAction>>
        {
            { "Grip", () => new Keybindings.Grip() },
            { "Twist", () => new Keybindings.Twist() },
            { "GripTwist", () => new Keybindings.GripTwist() },
            { "Twist2c", () => new Keybindings.Twist2c() },
            { "Twist3c", () => new Keybindings.Twist3c() },
            { "Layer", () => new Keybindings.Layer() },
            { "Recenter", () => new Keybindings.Recenter() },
            { "ChangeLayout", () => new Keybindings.ChangeLayout() },
            { "Macro", () => new Keybindings.Macro() },
            { "MacroReverse", () => new Keybindings.MacroReverse() },
        };

        public KeybindSetup(Keybindings keybinds)
        {
            InitializeComponent();
            this.keybinds = keybinds;
        }

        private void SetLayout(string name)
        {
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
                Size = new Size(96, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };
            textBox.KeyUp += new KeyEventHandler(this.Hotkey_KeyUp);
            textBox.KeyUp += (object sender, KeyEventArgs e) =>
            {
                var tb = (TextBox)sender;
                if (curKeybinds.binds.ContainsKey(tb.Text))
                {
                    MessageBox.Show($"{tb.Text} is already bound to {curKeybinds.binds[tb.Text].Serialize()}");
                    tb.Text = key;
                }
                else
                {
                    curKeybinds.binds.Remove(key);
                    curKeybinds.binds.Add(tb.Text, action);
                    key = tb.Text;
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

            var actions = actionList.Keys.ToArray();
            comboBox.Items.AddRange(actions);
            comboBox.SelectedIndex = comboBox.Items.IndexOf(action.GetType().Name);
            comboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;
            comboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
            {
                action = actionList[(string)comboBox.SelectedItem]();
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
                var confirmResult = MessageBox.Show($"Are you sure you want to delete {comboBox.SelectedItem} keybind for \"{key}\"?",
                                    "Confirm Delete",
                                    MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    if (curKeybinds.binds.ContainsKey(key)) curKeybinds.binds.Remove(key);
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SwitchLayout_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            SetLayout(btn.Text);
        }

        private void Hotkey_KeyUp(object sender, KeyEventArgs e)
        {
            Control ctrl = (Control)sender;
            ctrl.Text = e.KeyCode.ToString();
        }
    }
}
