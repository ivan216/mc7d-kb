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
        readonly Func<int> m_getDim;
        readonly ApplyStructuredMacroHandler m_apply;
        readonly Action m_changed;

        FlowLayoutPanel m_macroList;
        CStructuredMacro m_selectedMacro;
        ScrollableTextView m_astPreview;
        Panel m_twistGridHost;
        DataGridView m_twistGrid;
        ScrollableTextView m_rktPreview;
        Button m_applyButton;
        Button m_reverseButton;
        Button m_renameButton;
        Button m_deleteButton;
        Button m_buildRktButton;
        bool m_refreshingDetails;

        internal StructuredMacroWindow(IList<CStructuredMacro> macros, Func<int> getSize,
            Func<int> getDim, ApplyStructuredMacroHandler apply, Action changed) {
            if(macros == null) throw new ArgumentNullException("macros");
            if(getSize == null) throw new ArgumentNullException("getSize");
            if(getDim == null) throw new ArgumentNullException("getDim");
            if(apply == null) throw new ArgumentNullException("apply");

            m_macros = macros;
            m_getSize = getSize;
            m_getDim = getDim;
            m_apply = apply;
            m_changed = changed;
            InitializeUi();
            RefreshMacros(null);
        }

        internal void RefreshMacros(CStructuredMacro selected) {
            CStructuredMacro old = selected != null ? selected : SelectedMacro;
            m_macroList.SuspendLayout();
            m_macroList.Controls.Clear();
            List<CStructuredMacro> sorted = new List<CStructuredMacro>(m_macros);
            sorted.Sort(delegate(CStructuredMacro a, CStructuredMacro b) {
                return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
            });
            for(int i=0;i<sorted.Count;i++) {
                Button item = new Button();
                item.Text = sorted[i].Name;
                item.Tag = sorted[i];
                item.Width = Math.Max(1, m_macroList.ClientSize.Width - 8);
                item.Height = 24;
                item.Margin = new Padding(2);
                item.TextAlign = ContentAlignment.MiddleLeft;
                item.Click += new EventHandler(macroListItem_Click);
                m_macroList.Controls.Add(item);
            }
            m_macroList.ResumeLayout();

            if(old != null) {
                for(int i=0;i<m_macroList.Controls.Count;i++) {
                    if(object.ReferenceEquals(m_macroList.Controls[i].Tag, old)) {
                        SelectMacro(old);
                        return;
                    }
                }
            }
            if(m_macroList.Controls.Count > 0) SelectMacro((CStructuredMacro)m_macroList.Controls[0].Tag);
            else {
                m_selectedMacro = null;
                RefreshDetails();
            }
        }

        CStructuredMacro SelectedMacro {
            get { return m_selectedMacro; }
        }

        void macroListItem_Click(object sender, EventArgs e) {
            Button item = sender as Button;
            if(item == null) return;
            SelectMacro(item.Tag as CStructuredMacro);
        }

        void SelectMacro(CStructuredMacro macro) {
            m_selectedMacro = macro;
            for(int i=0;i<m_macroList.Controls.Count;i++) {
                Control control = m_macroList.Controls[i];
                bool selected = object.ReferenceEquals(control.Tag, macro);
                control.BackColor = selected ? SystemColors.Highlight : SystemColors.Control;
                control.ForeColor = selected ? SystemColors.HighlightText : SystemColors.ControlText;
            }
            RefreshDetails();
        }

        void InitializeUi() {
            Text = "Structured Macros";
            Size = new Size(1153, 656);
            MinimumSize = Size.Empty;
            StartPosition = FormStartPosition.WindowsDefaultLocation;

            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.FixedPanel = FixedPanel.Panel1;
            split.Panel1MinSize = 0;
            split.Panel2MinSize = 0;
            Controls.Add(split);
            split.HandleCreated += delegate {
                if(split.Width > 360) {
                    split.SplitterDistance = 130;
                }
            };

            m_macroList = new FlowLayoutPanel();
            m_macroList.Dock = DockStyle.Fill;
            m_macroList.AutoScroll = true;
            m_macroList.FlowDirection = FlowDirection.TopDown;
            m_macroList.WrapContents = false;
            m_macroList.Resize += delegate {
                for(int i=0;i<m_macroList.Controls.Count;i++)
                    m_macroList.Controls[i].Width = Math.Max(1, m_macroList.ClientSize.Width - 8);
            };
            split.Panel1.Controls.Add(m_macroList);

            TableLayoutPanel right = new TableLayoutPanel();
            right.Dock = DockStyle.Fill;
            right.ColumnCount = 1;
            right.RowCount = 2;
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            split.Panel2.Controls.Add(right);

            SplitContainer detailSplit = new SplitContainer();
            detailSplit.Dock = DockStyle.Fill;
            detailSplit.Orientation = Orientation.Horizontal;
            detailSplit.Panel1MinSize = 0;
            detailSplit.Panel2MinSize = 0;
            detailSplit.SplitterWidth = 6;
            detailSplit.HandleCreated += delegate {
                if(detailSplit.Height > 260) detailSplit.SplitterDistance = 92;
            };
            right.Controls.Add(detailSplit, 0, 0);

            m_astPreview = new ScrollableTextView();
            m_astPreview.Dock = DockStyle.Fill;
            m_astPreview.Font = new Font("Consolas", 10.0f);
            detailSplit.Panel1.Controls.Add(m_astPreview);

            SplitContainer rktSplit = new SplitContainer();
            rktSplit.Dock = DockStyle.Fill;
            rktSplit.Orientation = Orientation.Horizontal;
            rktSplit.Panel1MinSize = 0;
            rktSplit.Panel2MinSize = 0;
            rktSplit.SplitterWidth = 6;
            rktSplit.HandleCreated += delegate {
                if(rktSplit.Height > 260) rktSplit.SplitterDistance = rktSplit.Height - 110;
            };
            detailSplit.Panel2.Controls.Add(rktSplit);

            m_twistGrid = new DataGridView();
            m_twistGrid.Dock = DockStyle.None;
            m_twistGrid.Location = new Point(0, 0);
            m_twistGrid.AllowUserToAddRows = false;
            m_twistGrid.AllowUserToDeleteRows = false;
            m_twistGrid.RowHeadersVisible = false;
            m_twistGrid.RowHeadersWidth = 4;
            m_twistGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_twistGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            m_twistGrid.ScrollBars = ScrollBars.None;
            m_twistGrid.BorderStyle = BorderStyle.FixedSingle;
            m_twistGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            m_twistGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            m_twistGrid.CellClick += new DataGridViewCellEventHandler(twistGrid_CellClick);
            m_twistGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(twistGrid_EditingControlShowing);
            m_twistGrid.CellEndEdit += delegate { RefreshEffectiveMasks(); RefreshRktPreview(); };
            m_twistGrid.ColumnWidthChanged += delegate { ResizeTwistGridToContent(); };
            AddTextColumn("Id", "Id", true);
            AddTextColumn("Twist", "Twist", true, 160);
            AddTextColumn("DefaultMask", "Default Mask", true);
            AddMaskColumn("OverrideMask", "Override Mask");
            AddTextColumn("EffectiveMask", "Effective Mask", true);
            AddCheckColumn("RktSelect", "RKT", false);
            AddSignColumn("TargetSign", "Target Sign");
            AddAxisColumn("TargetAxis", "Target Axis");
            AddSignColumn("AdjustSign", "Adjust Sign");
            AddAxisColumn("AdjustAxis", "Adjust Axis");

            m_twistGridHost = new StableAutoScrollPanel();
            m_twistGridHost.Dock = DockStyle.Fill;
            m_twistGridHost.AutoScroll = true;
            m_twistGridHost.BorderStyle = BorderStyle.FixedSingle;
            m_twistGridHost.Resize += delegate { ResizeTwistGridToContent(); };
            m_twistGridHost.Controls.Add(m_twistGrid);
            rktSplit.Panel1.Controls.Add(m_twistGridHost);

            GroupBox rktGroup = new GroupBox();
            rktGroup.Text = "RKT Preview";
            rktGroup.Dock = DockStyle.Fill;
            m_rktPreview = new ScrollableTextView();
            m_rktPreview.Dock = DockStyle.Fill;
            m_rktPreview.Font = new Font("Consolas", 9.0f);
            rktGroup.Controls.Add(m_rktPreview);
            rktSplit.Panel2.Controls.Add(rktGroup);

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Fill;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Padding = new Padding(0, 6, 0, 0);
            right.Controls.Add(buttons, 0, 1);

            m_applyButton = AddButton(buttons, "Apply", delegate { ApplySelected(false); });
            m_reverseButton = AddButton(buttons, "Reverse", delegate { ApplySelected(true); });
            m_buildRktButton = AddButton(buttons, "Build RKT Macro", delegate { BuildRktMacro(); });
            m_renameButton = AddButton(buttons, "Rename", delegate { RenameSelected(); });
            m_deleteButton = AddButton(buttons, "Delete", delegate { DeleteSelected(); });
            AddButton(buttons, "Refresh", delegate { RefreshMacros(SelectedMacro); });
        }

        const int TwistGridColumnWidth = 112;

        void AddTextColumn(string name, string header, bool readOnly) {
            AddTextColumn(name, header, readOnly, TwistGridColumnWidth);
        }

        void AddTextColumn(string name, string header, bool readOnly, int width) {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.ReadOnly = readOnly;
            column.Width = width;
            column.MinimumWidth = 18;
            column.Resizable = DataGridViewTriState.True;
            m_twistGrid.Columns.Add(column);
            ResizeTwistGridToContent();
        }

        void AddCheckColumn(string name, string header, bool readOnly) {
            DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.ReadOnly = readOnly;
            column.Width = TwistGridColumnWidth;
            column.MinimumWidth = 18;
            column.Resizable = DataGridViewTriState.True;
            m_twistGrid.Columns.Add(column);
            ResizeTwistGridToContent();
        }

        void AddMaskColumn(string name, string header) {
            DataGridViewNumericUpDownColumn column = new DataGridViewNumericUpDownColumn();
            column.Name = name;
            column.HeaderText = header;
            column.Width = TwistGridColumnWidth;
            column.MinimumWidth = 18;
            column.Resizable = DataGridViewTriState.True;
            SetMaskColumnRange(column);
            m_twistGrid.Columns.Add(column);
            ResizeTwistGridToContent();
        }

        void AddSignColumn(string name, string header) {
            DataGridViewPlainComboBoxColumn column = new DataGridViewPlainComboBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.Width = TwistGridColumnWidth;
            column.MinimumWidth = 18;
            column.Resizable = DataGridViewTriState.True;
            column.Items.Add("+");
            column.Items.Add("-");
            m_twistGrid.Columns.Add(column);
            ResizeTwistGridToContent();
        }

        void AddAxisColumn(string name, string header) {
            DataGridViewPlainComboBoxColumn column = new DataGridViewPlainComboBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.Width = TwistGridColumnWidth;
            column.MinimumWidth = 18;
            column.Resizable = DataGridViewTriState.True;
            PopulateAxisColumn(column);
            m_twistGrid.Columns.Add(column);
            ResizeTwistGridToContent();
        }

        void RefreshGridInputLimits() {
            DataGridViewNumericUpDownColumn maskColumn =
                m_twistGrid.Columns["OverrideMask"] as DataGridViewNumericUpDownColumn;
            if(maskColumn != null) SetMaskColumnRange(maskColumn);

            PopulateAxisColumn(m_twistGrid.Columns["TargetAxis"] as DataGridViewComboBoxColumn);
            PopulateAxisColumn(m_twistGrid.Columns["AdjustAxis"] as DataGridViewComboBoxColumn);
        }

        void SetMaskColumnRange(DataGridViewNumericUpDownColumn column) {
            int maxMask = MaxMask();
            column.Minimum = -maxMask;
            column.Maximum = maxMask;
        }

        void PopulateAxisColumn(DataGridViewComboBoxColumn column) {
            if(column == null) return;
            column.Items.Clear();
            int dim = m_getDim();
            if(dim < 1) dim = 1;
            if(dim > 7) dim = 7;
            for(int axis=1;axis<=dim;axis++)
                column.Items.Add(StructuredAxis.FormatName(axis));
        }

        int MaxMask() {
            return (1 << m_getSize()) - 1;
        }

        void ResizeTwistGridToContent() {
            if(m_twistGrid == null) return;
            Point scroll = GetTwistGridScroll();

            int width = 2;
            for(int i=0;i<m_twistGrid.Columns.Count;i++)
                if(m_twistGrid.Columns[i].Visible) width += m_twistGrid.Columns[i].Width;

            int height = m_twistGrid.ColumnHeadersVisible ? m_twistGrid.ColumnHeadersHeight : 0;
            for(int i=0;i<m_twistGrid.Rows.Count;i++)
                if(m_twistGrid.Rows[i].Visible) height += m_twistGrid.Rows[i].Height;
            height += 2;

            if(m_twistGridHost != null) {
                width = Math.Max(width, m_twistGridHost.ClientSize.Width);
                height = Math.Max(height, m_twistGridHost.ClientSize.Height);
            }

            m_twistGrid.Size = new Size(width, height);
            RestoreTwistGridScroll(scroll);
        }

        Point GetTwistGridScroll() {
            if(m_twistGridHost == null) return Point.Empty;
            Point pos = m_twistGridHost.AutoScrollPosition;
            return new Point(-pos.X, -pos.Y);
        }

        void RestoreTwistGridScroll(Point scroll) {
            if(m_twistGridHost == null) return;
            m_twistGridHost.AutoScrollPosition = scroll;
        }

        Button AddButton(FlowLayoutPanel panel, string text, EventHandler click) {
            Button button = new Button();
            button.Text = text;
            button.AutoSize = true;
            button.Click += click;
            panel.Controls.Add(button);
            return button;
        }

        void twistGrid_CellClick(object sender, DataGridViewCellEventArgs e) {
            if(e.RowIndex < 0 || e.ColumnIndex < 0) return;
            DataGridViewColumn column = m_twistGrid.Columns[e.ColumnIndex];
            if(column == null) return;

            if(column.Name == "RktSelect") {
                DataGridViewCell cell = m_twistGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                bool selected = cell.Value is bool && (bool)cell.Value;
                cell.Value = !selected;
                RefreshRktPreview();
                return;
            }

            if(column.Name != "OverrideMask" && column.Name != "TargetSign" && column.Name != "TargetAxis"
                && column.Name != "AdjustSign" && column.Name != "AdjustAxis")
                return;

            Point scroll = GetTwistGridScroll();
            try {
                m_twistGrid.CurrentCell = m_twistGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                m_twistGrid.BeginEdit(false);
                ComboBox combo = m_twistGrid.EditingControl as ComboBox;
                if(combo != null) combo.DroppedDown = true;
            } finally {
                RestoreTwistGridScroll(scroll);
                BeginInvoke(new MethodInvoker(delegate { RestoreTwistGridScroll(scroll); }));
            }
        }

        void twistGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            TextBox editor = e.Control as TextBox;
            if(editor == null || m_twistGrid.CurrentCell == null) return;
            string columnName = m_twistGrid.Columns[m_twistGrid.CurrentCell.ColumnIndex].Name;
            if(columnName != "OverrideMask") return;

            editor.SelectionStart = editor.TextLength;
            editor.SelectionLength = 0;
        }

        void RefreshDetails() {
            m_refreshingDetails = true;
            try {
                CStructuredMacro macro = SelectedMacro;
                bool hasMacro = macro != null;
                m_applyButton.Enabled = hasMacro;
                m_reverseButton.Enabled = hasMacro;
                m_renameButton.Enabled = hasMacro;
                m_deleteButton.Enabled = hasMacro;
                m_buildRktButton.Enabled = hasMacro;

                m_astPreview.Text = hasMacro ? macro.RootNode.ToExpression() : "";
                m_twistGrid.Rows.Clear();
                RefreshGridInputLimits();
                if(!hasMacro) {
                    m_rktPreview.Text = "";
                    return;
                }

                List<StructuredTwist> twists = macro.GetTwistsSorted();
                for(int i=0;i<twists.Count;i++) {
                    StructuredTwist twist = twists[i];
                    int row = m_twistGrid.Rows.Add();
                    DataGridViewRow gridRow = m_twistGrid.Rows[row];
                    gridRow.Tag = twist;
                    gridRow.Cells["Id"].Value = "T" + twist.Id;
                    gridRow.Cells["Twist"].Value = twist.ToTwistString();
                    gridRow.Cells["DefaultMask"].Value = twist.DefaultMask.ToString();
                    gridRow.Cells["OverrideMask"].Value = 0;
                    gridRow.Cells["EffectiveMask"].Value = EffectiveMaskText(twist, 0);
                    gridRow.Cells["RktSelect"].Value = false;
                    gridRow.Cells["TargetSign"].Value = "+";
                    gridRow.Cells["TargetAxis"].Value = null;
                    gridRow.Cells["AdjustSign"].Value = "+";
                    gridRow.Cells["AdjustAxis"].Value = null;
                }
            } finally {
                m_refreshingDetails = false;
            }
            ResizeTwistGridToContent();
            RefreshRktPreview();
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
            if(overrideMask == 0) return true;

            int maxMask = MaxMask();
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

        List<StructuredRktSelection> ReadRktSelections(bool requireAny) {
            List<StructuredRktSelection> selections = new List<StructuredRktSelection>();
            for(int i=0;i<m_twistGrid.Rows.Count;i++) {
                DataGridViewRow row = m_twistGrid.Rows[i];
                StructuredTwist twist = row.Tag as StructuredTwist;
                if(twist == null || !IsRktSelected(row)) continue;

                int targetCell = ParseAxisCell(row, "TargetSign", "TargetAxis", "target cell", twist.Id);
                int adjustCell = ParseAxisCell(row, "AdjustSign", "AdjustAxis", "adjust cell", twist.Id);
                selections.Add(new StructuredRktSelection(twist.Id, targetCell, adjustCell));
            }

            if(requireAny && selections.Count == 0)
                throw new ArgumentException("Select at least one twist for RKT.");
            return selections;
        }

        bool IsRktSelected(DataGridViewRow row) {
            object value = row.Cells["RktSelect"].Value;
            return value is bool && (bool)value;
        }

        int ParseAxisCell(DataGridViewRow row, string signColumnName, string axisColumnName, string label, int twistId) {
            object signValue = row.Cells[signColumnName].Value;
            object axisValue = row.Cells[axisColumnName].Value;
            string sign = signValue == null ? "" : signValue.ToString().Trim();
            string axis = axisValue == null ? "" : axisValue.ToString().Trim();
            if(sign.Length == 0 || axis.Length == 0)
                throw new ArgumentException("Missing RKT " + label + " for T" + twistId + ".");
            return StructuredAxis.Parse(sign + axis);
        }

        void RefreshRktPreview() {
            if(m_refreshingDetails) return;
            CStructuredMacro macro = SelectedMacro;
            if(macro == null) {
                m_rktPreview.Text = "";
                return;
            }

            try {
                List<StructuredRktSelection> selections = ReadRktSelections(false);
                if(selections.Count == 0) {
                    m_rktPreview.Text = "Select one or more twist rows and enter signed Target/Adjust cells, e.g. -X and +W.";
                    return;
                }

                CStructuredMacro preview = StructuredRktBuilder.Generate(macro, selections, macro.Name + "_RKT");
                m_rktPreview.Text = preview.ToDebugString();
            } catch(Exception ex) {
                m_rktPreview.Text = ex.Message;
            }
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

        void BuildRktMacro() {
            CStructuredMacro macro = SelectedMacro;
            if(macro == null) return;
            try {
                List<StructuredRktSelection> selections = ReadRktSelections(true);
                string name;
                if(!PromptRktName(macro, out name)) return;

                CStructuredMacro generated = StructuredRktBuilder.Generate(macro, selections, name);
                int existing = StructuredMacroNames.FindIndex(m_macros, name, null);
                if(existing >= 0) m_macros.RemoveAt(existing);
                m_macros.Add(generated);
                if(m_changed != null) m_changed();
                RefreshMacros(generated);
            } catch(Exception ex) {
                MessageBox.Show(this, ex.Message, "Structured Macro RKT");
            }
        }

        bool PromptRktName(CStructuredMacro source, out string name) {
            name = "";
            string current = source.Name + "_RKT";
            while(true) {
                TextDialog edt = new TextDialog("Enter RKT Macro Name");
                edt.Value = current;
                if(edt.ShowDialog(this) != DialogResult.OK) return false;

                current = StructuredMacroNames.Normalize(edt.Value);
                if(current.Length == 0) {
                    MessageBox.Show(this, "Structured macro name cannot be empty.", "Structured Macro RKT");
                    continue;
                }

                int existing = StructuredMacroNames.FindIndex(m_macros, current, null);
                if(existing >= 0) {
                    DialogResult overwrite = MessageBox.Show(this,
                        "Structured macro '" + current + "' already exists. Overwrite it?",
                        "Structured Macro RKT", MessageBoxButtons.YesNo);
                    if(overwrite != DialogResult.Yes) continue;
                }

                name = current;
                return true;
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

    public sealed class DataGridViewNumericUpDownColumn : DataGridViewColumn {
        int m_minimum = -127;
        int m_maximum = 127;

        public DataGridViewNumericUpDownColumn()
            : base(new DataGridViewNumericUpDownCell()) {
        }

        internal int Minimum {
            get { return m_minimum; }
            set {
                m_minimum = value;
                if(m_maximum < m_minimum) m_maximum = m_minimum;
            }
        }

        internal int Maximum {
            get { return m_maximum; }
            set {
                m_maximum = value;
                if(m_minimum > m_maximum) m_minimum = m_maximum;
            }
        }

        public override object Clone() {
            DataGridViewNumericUpDownColumn clone = (DataGridViewNumericUpDownColumn)base.Clone();
            clone.Minimum = Minimum;
            clone.Maximum = Maximum;
            return clone;
        }
    }

    public sealed class DataGridViewNumericUpDownCell : DataGridViewTextBoxCell {
        public override Type EditType {
            get { return typeof(DataGridViewNumericUpDownEditingControl); }
        }

        public override Type ValueType {
            get { return typeof(int); }
        }

        public override object DefaultNewRowValue {
            get { return 0; }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
            DataGridViewCellStyle dataGridViewCellStyle) {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            DataGridViewNumericUpDownEditingControl editor =
                DataGridView.EditingControl as DataGridViewNumericUpDownEditingControl;
            if(editor == null) return;

            DataGridViewNumericUpDownColumn column = OwningColumn as DataGridViewNumericUpDownColumn;
            if(column != null) {
                editor.Minimum = column.Minimum;
                editor.Maximum = column.Maximum;
            }

            int value = 0;
            object cellValue = Value;
            if(cellValue != null && cellValue != DBNull.Value)
                int.TryParse(cellValue.ToString(), out value);

            if(value < editor.Minimum) value = (int)editor.Minimum;
            if(value > editor.Maximum) value = (int)editor.Maximum;
            editor.Value = value;
        }

    }

    public sealed class DataGridViewNumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl {
        DataGridView m_dataGridView;
        bool m_valueChanged;
        int m_rowIndex;

        public DataGridViewNumericUpDownEditingControl() {
            DecimalPlaces = 0;
            ThousandsSeparator = false;
        }

        public DataGridView EditingControlDataGridView {
            get { return m_dataGridView; }
            set { m_dataGridView = value; }
        }

        public object EditingControlFormattedValue {
            get { return ((int)Value).ToString(); }
            set {
                int parsed;
                if(value != null && int.TryParse(value.ToString(), out parsed)) Value = parsed;
                else Value = 0;
            }
        }

        public int EditingControlRowIndex {
            get { return m_rowIndex; }
            set { m_rowIndex = value; }
        }

        public bool EditingControlValueChanged {
            get { return m_valueChanged; }
            set { m_valueChanged = value; }
        }

        public Cursor EditingPanelCursor {
            get { return Cursors.Default; }
        }

        public bool RepositionEditingControlOnValueChange {
            get { return false; }
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle) {
            Font = dataGridViewCellStyle.Font;
            ForeColor = dataGridViewCellStyle.ForeColor;
            BackColor = dataGridViewCellStyle.BackColor;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) {
            Keys key = keyData & Keys.KeyCode;
            return key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down
                || key == Keys.Home || key == Keys.End
                || !dataGridViewWantsInputKey;
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll) {
            Select(0, Text.Length);
        }

        protected override void OnValueChanged(EventArgs e) {
            m_valueChanged = true;
            if(m_dataGridView != null)
                m_dataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(e);
        }
    }

    public sealed class DataGridViewPlainComboBoxColumn : DataGridViewComboBoxColumn {
        public DataGridViewPlainComboBoxColumn() {
            CellTemplate = new DataGridViewPlainComboBoxCell();
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            FlatStyle = FlatStyle.Flat;
        }

        public override object Clone() {
            DataGridViewPlainComboBoxColumn clone = (DataGridViewPlainComboBoxColumn)base.Clone();
            clone.DisplayStyle = DisplayStyle;
            clone.FlatStyle = FlatStyle;
            return clone;
        }
    }

    public sealed class DataGridViewPlainComboBoxCell : DataGridViewComboBoxCell {
        public DataGridViewPlainComboBoxCell() {
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            FlatStyle = FlatStyle.Flat;
        }

        public override object Clone() {
            DataGridViewPlainComboBoxCell clone = (DataGridViewPlainComboBoxCell)base.Clone();
            clone.DisplayStyle = DisplayStyle;
            clone.FlatStyle = FlatStyle;
            return clone;
        }
    }

    internal sealed class StableAutoScrollPanel : Panel {
        protected override Point ScrollToControl(Control activeControl) {
            return DisplayRectangle.Location;
        }
    }

    internal sealed class ScrollableTextView : Panel {
        readonly Label m_label;

        internal ScrollableTextView() {
            AutoScroll = true;
            BorderStyle = BorderStyle.FixedSingle;

            m_label = new Label();
            m_label.AutoSize = true;
            m_label.Location = new Point(3, 3);
            m_label.Padding = new Padding(2);
            m_label.UseMnemonic = false;
            Controls.Add(m_label);

            Resize += delegate { UpdateLabelWidth(); };
        }

        public override Font Font {
            get { return base.Font; }
            set {
                base.Font = value;
                if(m_label != null) m_label.Font = value;
            }
        }

        public override string Text {
            get { return m_label.Text; }
            set {
                m_label.Text = value == null ? "" : value;
                UpdateLabelWidth();
            }
        }

        void UpdateLabelWidth() {
            if(m_label == null) return;
            int width = Math.Max(0, ClientSize.Width - 10);
            m_label.MaximumSize = new Size(width, 0);
        }
    }
}
