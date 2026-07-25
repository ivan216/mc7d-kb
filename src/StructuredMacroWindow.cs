using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _3dedit {
    internal delegate void ApplyStructuredMacroHandler(CStructuredMacro macro,
        IDictionary<int, int> overrideMasks, bool reverse);

    internal sealed class StructuredMacroWindow : Form {
        readonly IList<CStructuredMacro> m_macros;
        readonly Func<int> m_getSize;
        readonly ApplyStructuredMacroHandler m_apply;
        readonly Action m_changed;

        ListBox m_macroList;
        TextBox m_astPreview;
        DataGridView m_twistGrid;
        Button m_applyButton;
        Button m_reverseButton;
        Button m_renameButton;
        Button m_deleteButton;

        internal StructuredMacroWindow(IList<CStructuredMacro> macros, Func<int> getSize,
            ApplyStructuredMacroHandler apply, Action changed) {
            if(macros == null) throw new ArgumentNullException("macros");
            if(getSize == null) throw new ArgumentNullException("getSize");
            if(apply == null) throw new ArgumentNullException("apply");

            m_macros = macros;
            m_getSize = getSize;
            m_apply = apply;
            m_changed = changed;
            InitializeUi();
            RefreshMacros(null);
        }

        internal void RefreshMacros(CStructuredMacro selected) {
            CStructuredMacro old = selected != null ? selected : SelectedMacro;
            m_macroList.BeginUpdate();
            try {
                m_macroList.Items.Clear();
                List<CStructuredMacro> sorted = new List<CStructuredMacro>(m_macros);
                sorted.Sort(delegate(CStructuredMacro a, CStructuredMacro b) {
                    return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
                });
                for(int i=0;i<sorted.Count;i++) m_macroList.Items.Add(sorted[i]);
            } finally {
                m_macroList.EndUpdate();
            }

            if(old != null) {
                for(int i=0;i<m_macroList.Items.Count;i++) {
                    if(object.ReferenceEquals(m_macroList.Items[i], old)) {
                        m_macroList.SelectedIndex = i;
                        return;
                    }
                }
            }
            if(m_macroList.Items.Count > 0) m_macroList.SelectedIndex = 0;
            else RefreshDetails();
        }

        CStructuredMacro SelectedMacro {
            get { return m_macroList.SelectedItem as CStructuredMacro; }
        }

        void InitializeUi() {
            Text = "Structured Macros";
            Size = new Size(1153, 656);
            MinimumSize = new Size(860, 460);
            StartPosition = FormStartPosition.WindowsDefaultLocation;

            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.FixedPanel = FixedPanel.Panel1;
            Controls.Add(split);
            split.HandleCreated += delegate {
                if(split.Width > 360) {
                    split.SplitterDistance = 130;
                    split.Panel1MinSize = 110;
                    split.Panel2MinSize = 200;
                }
            };

            m_macroList = new ListBox();
            m_macroList.Dock = DockStyle.Fill;
            m_macroList.SelectedIndexChanged += delegate { RefreshDetails(); };
            split.Panel1.Controls.Add(m_macroList);

            TableLayoutPanel right = new TableLayoutPanel();
            right.Dock = DockStyle.Fill;
            right.ColumnCount = 1;
            right.RowCount = 3;
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            split.Panel2.Controls.Add(right);

            m_astPreview = new TextBox();
            m_astPreview.Dock = DockStyle.Fill;
            m_astPreview.Multiline = true;
            m_astPreview.ReadOnly = true;
            m_astPreview.ScrollBars = ScrollBars.Vertical;
            m_astPreview.Font = new Font("Consolas", 10.0f);
            right.Controls.Add(m_astPreview, 0, 0);

            m_twistGrid = new DataGridView();
            m_twistGrid.Dock = DockStyle.Fill;
            m_twistGrid.AllowUserToAddRows = false;
            m_twistGrid.AllowUserToDeleteRows = false;
            m_twistGrid.RowHeadersVisible = false;
            m_twistGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_twistGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            m_twistGrid.CellEndEdit += delegate { RefreshEffectiveMasks(); };
            AddTextColumn("Id", "Id", true);
            AddTextColumn("Twist", "Twist", true);
            AddTextColumn("DefaultMask", "Default Mask", true);
            AddTextColumn("OverrideMask", "Override Mask", false);
            AddTextColumn("EffectiveMask", "Effective Mask", true);
            right.Controls.Add(m_twistGrid, 0, 1);

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Fill;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Padding = new Padding(0, 6, 0, 0);
            right.Controls.Add(buttons, 0, 2);

            m_applyButton = AddButton(buttons, "Apply", delegate { ApplySelected(false); });
            m_reverseButton = AddButton(buttons, "Reverse", delegate { ApplySelected(true); });
            m_renameButton = AddButton(buttons, "Rename", delegate { RenameSelected(); });
            m_deleteButton = AddButton(buttons, "Delete", delegate { DeleteSelected(); });
            AddButton(buttons, "Refresh", delegate { RefreshMacros(SelectedMacro); });
        }

        void AddTextColumn(string name, string header, bool readOnly) {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.ReadOnly = readOnly;
            m_twistGrid.Columns.Add(column);
        }

        Button AddButton(FlowLayoutPanel panel, string text, EventHandler click) {
            Button button = new Button();
            button.Text = text;
            button.AutoSize = true;
            button.Click += click;
            panel.Controls.Add(button);
            return button;
        }

        void RefreshDetails() {
            CStructuredMacro macro = SelectedMacro;
            bool hasMacro = macro != null;
            m_applyButton.Enabled = hasMacro;
            m_reverseButton.Enabled = hasMacro;
            m_renameButton.Enabled = hasMacro;
            m_deleteButton.Enabled = hasMacro;

            m_astPreview.Text = hasMacro ? macro.RootNode.ToExpression() : "";
            m_twistGrid.Rows.Clear();
            if(!hasMacro) return;

            List<StructuredTwist> twists = macro.GetTwistsSorted();
            for(int i=0;i<twists.Count;i++) {
                StructuredTwist twist = twists[i];
                int row = m_twistGrid.Rows.Add();
                DataGridViewRow gridRow = m_twistGrid.Rows[row];
                gridRow.Tag = twist;
                gridRow.Cells["Id"].Value = "T" + twist.Id;
                gridRow.Cells["Twist"].Value = twist.ToTwistString();
                gridRow.Cells["DefaultMask"].Value = twist.DefaultMask.ToString();
                gridRow.Cells["OverrideMask"].Value = "";
                gridRow.Cells["EffectiveMask"].Value = EffectiveMaskText(twist, 0);
            }
        }

        void RefreshEffectiveMasks() {
            for(int i=0;i<m_twistGrid.Rows.Count;i++) {
                DataGridViewRow row = m_twistGrid.Rows[i];
                StructuredTwist twist = row.Tag as StructuredTwist;
                if(twist == null) continue;

                int overrideMask;
                string error;
                if(!TryParseOverrideCell(row, out overrideMask, out error)) {
                    row.Cells["EffectiveMask"].Value = error;
                } else {
                    row.Cells["EffectiveMask"].Value = EffectiveMaskText(twist, overrideMask);
                }
            }
        }

        string EffectiveMaskText(StructuredTwist twist, int overrideMask) {
            int logicalMask = overrideMask != 0 ? overrideMask : twist.DefaultMask;
            CompiledStructuredTwist compiled = new CompiledStructuredTwist(twist.Id,
                twist.SignedGripAxis, twist.FromAxis, twist.ToAxis, logicalMask);
            int gripAxis;
            int fromAxis;
            int toAxis;
            int cubeMask;
            StructuredTwistRuntime.ResolveForCubeTwist(compiled, m_getSize(), null,
                out gripAxis, out fromAxis, out toAxis, out cubeMask);
            return cubeMask.ToString();
        }

        bool TryParseOverrideCell(DataGridViewRow row, out int overrideMask, out string error) {
            overrideMask = 0;
            error = null;
            object value = row.Cells["OverrideMask"].Value;
            string text = value == null ? "" : value.ToString().Trim();
            if(text.Length == 0) return true;

            if(!int.TryParse(text, out overrideMask)) {
                error = "invalid";
                return false;
            }
            if(overrideMask == 0) {
                error = "zero";
                return false;
            }

            int maxMask = (1 << m_getSize()) - 1;
            if(Math.Abs(overrideMask) > maxMask) {
                error = ">" + maxMask;
                return false;
            }
            return true;
        }

        Dictionary<int, int> ReadOverrideMasks() {
            Dictionary<int, int> overrides = new Dictionary<int, int>();
            for(int i=0;i<m_twistGrid.Rows.Count;i++) {
                DataGridViewRow row = m_twistGrid.Rows[i];
                StructuredTwist twist = row.Tag as StructuredTwist;
                if(twist == null) continue;

                int overrideMask;
                string error;
                if(!TryParseOverrideCell(row, out overrideMask, out error))
                    throw new ArgumentException("Invalid override mask for T" + twist.Id + ": " + error);
                if(overrideMask != 0) overrides[twist.Id] = overrideMask;
            }
            return overrides;
        }

        void ApplySelected(bool reverse) {
            CStructuredMacro macro = SelectedMacro;
            if(macro == null) return;
            try {
                m_apply(macro, ReadOverrideMasks(), reverse);
            } catch(Exception ex) {
                MessageBox.Show(this, ex.Message, "Structured Macro");
            }
        }

        void RenameSelected() {
            CStructuredMacro macro = SelectedMacro;
            if(macro == null) return;
            TextDialog edt = new TextDialog("Enter New Structured Macro Name");
            edt.Value = macro.Name;
            if(edt.ShowDialog(this) != DialogResult.OK) return;
            string name = StructuredMacroNames.Normalize(edt.Value);
            if(name.Length == 0) {
                MessageBox.Show(this, "Structured macro name cannot be empty.", "Structured Macro");
                return;
            }

            int existing = StructuredMacroNames.FindIndex(m_macros, name, macro);
            if(existing >= 0) m_macros.RemoveAt(existing);
            macro.Name = name;
            if(m_changed != null) m_changed();
            RefreshMacros(macro);
        }

        void DeleteSelected() {
            CStructuredMacro macro = SelectedMacro;
            if(macro == null) return;
            if(MessageBox.Show(this, "Delete structured macro '" + macro.Name + "'?",
                "Structured Macro", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            m_macros.Remove(macro);
            if(m_changed != null) m_changed();
            RefreshMacros(null);
        }
    }
}
