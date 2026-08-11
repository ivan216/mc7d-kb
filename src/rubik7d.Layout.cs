using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class Form1
    {
        Panel _statusScrollHost;
        ToolStripMenuItem _menuOverflowItem;
        List<ToolStripMenuItem> _topMenuItems;
        bool _menuOverflowLayoutBusy;
        const int SidePanelResizeGripWidth = 6;
        const int SidePanelMinWidth = 160;
        int _sidePanelMaxWidth;
        bool _resizingSidePanel;
        int _sidePanelResizeStartMouseX;
        int _sidePanelResizeStartWidth;
        int _sidePanelResizePreviewWidth;
        int _sidePanelResizePreviewScreenX;
        bool _sidePanelResizePreviewVisible;
        int _sidePanelExpandedWidth;

        private void btnTogglePanel_Click(object sender, EventArgs e) {
            SetPanelCollapsed(!m_panelCollapsed);
        }

        private void mi_ShowSidePanel_Click(object sender, EventArgs e) {
            SetPanelCollapsed(!m_panelCollapsed);
        }

        private void SetPanelCollapsed(bool collapsed) {
            m_panelCollapsed = collapsed;
            if (collapsed)
                _sidePanelExpandedWidth = panel1.Width;
            else if (_sidePanelExpandedWidth > 0)
                panel1.Width = ClampSidePanelWidth(_sidePanelExpandedWidth);
            panel1.Visible = !collapsed;
            btnTogglePanel.Text = collapsed ? "‹" : "›";
            mi_ShowSidePanel.Checked = !collapsed;

            UpdateResponsiveLayout();
        }

        private void InitializeSidePanelResize()
        {
            _sidePanelExpandedWidth = panel1.Width;
            panel1.MinimumSize = new Size(SidePanelMinWidth, 0);
            panel1.MouseMove += panel1_ResizeMouseMove;
            panel1.MouseDown += panel1_ResizeMouseDown;
            panel1.MouseUp += panel1_ResizeMouseUp;
            panel1.MouseLeave += panel1_ResizeMouseLeave;
            panel1.MouseCaptureChanged += panel1_ResizeMouseCaptureChanged;
            UpdateSidePanelScrollBounds();
        }

        private void CaptureInitialSidePanelWidth()
        {
            if (_sidePanelMaxWidth > 0) return;

            _sidePanelMaxWidth = panel1.Width;
            _sidePanelExpandedWidth = panel1.Width;
            panel1.MaximumSize = new Size(_sidePanelMaxWidth, 0);
        }

        private bool IsSidePanelResizeHit(Point point)
        {
            return panel1.Visible && point.X >= 0 && point.X <= SidePanelResizeGripWidth;
        }

        private int ClampSidePanelWidth(int width)
        {
            int maxWidth = _sidePanelMaxWidth > 0 ? _sidePanelMaxWidth : Math.Max(panel1.Width, width);
            return Math.Max(SidePanelMinWidth, Math.Min(maxWidth, width));
        }

        private void panel1_ResizeMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !IsSidePanelResizeHit(e.Location)) return;

            _resizingSidePanel = true;
            _sidePanelResizeStartMouseX = Cursor.Position.X;
            _sidePanelResizeStartWidth = panel1.Width;
            _sidePanelResizePreviewWidth = panel1.Width;
            ShowSidePanelResizePreview(_sidePanelResizePreviewWidth);
            panel1.Capture = true;
        }

        private void panel1_ResizeMouseMove(object sender, MouseEventArgs e)
        {
            if (_resizingSidePanel)
            {
                UpdateSidePanelResizePreview(GetSidePanelResizeWidthFromMouse());
                return;
            }

            panel1.Cursor = IsSidePanelResizeHit(e.Location) ? Cursors.SizeWE : Cursors.Default;
        }

        private void panel1_ResizeMouseUp(object sender, MouseEventArgs e)
        {
            StopSidePanelResize();
        }

        private void panel1_ResizeMouseLeave(object sender, EventArgs e)
        {
            if (!_resizingSidePanel)
                panel1.Cursor = Cursors.Default;
        }

        private void panel1_ResizeMouseCaptureChanged(object sender, EventArgs e)
        {
            if (_resizingSidePanel && !panel1.Capture)
                StopSidePanelResize();
        }

        private void StopSidePanelResize()
        {
            if (!_resizingSidePanel) return;

            int finalWidth = _sidePanelResizePreviewWidth > 0
                ? _sidePanelResizePreviewWidth
                : GetSidePanelResizeWidthFromMouse();

            HideSidePanelResizePreview();
            _resizingSidePanel = false;
            panel1.Capture = false;
            panel1.Cursor = Cursors.Default;

            finalWidth = ClampSidePanelWidth(finalWidth);
            if (panel1.Width != finalWidth)
            {
                panel1.Width = finalWidth;
                _sidePanelExpandedWidth = panel1.Width;
                UpdateResponsiveLayout();
            }
        }

        private int GetSidePanelResizeWidthFromMouse()
        {
            int delta = _sidePanelResizeStartMouseX - Cursor.Position.X;
            return ClampSidePanelWidth(_sidePanelResizeStartWidth + delta);
        }

        private void UpdateSidePanelResizePreview(int width)
        {
            width = ClampSidePanelWidth(width);
            if (_sidePanelResizePreviewVisible && _sidePanelResizePreviewWidth == width) return;

            HideSidePanelResizePreview();
            _sidePanelResizePreviewWidth = width;
            ShowSidePanelResizePreview(width);
        }

        private void ShowSidePanelResizePreview(int width)
        {
            if (_sidePanelResizePreviewVisible) return;

            _sidePanelResizePreviewScreenX = GetSidePanelResizePreviewScreenX(width);
            DrawSidePanelResizePreview(_sidePanelResizePreviewScreenX);
            _sidePanelResizePreviewVisible = true;
        }

        private void HideSidePanelResizePreview()
        {
            if (!_sidePanelResizePreviewVisible) return;

            DrawSidePanelResizePreview(_sidePanelResizePreviewScreenX);
            _sidePanelResizePreviewVisible = false;
        }

        private int GetSidePanelResizePreviewScreenX(int width)
        {
            int clientX = this.ClientSize.Width - ClampSidePanelWidth(width);
            return this.PointToScreen(new Point(clientX, 0)).X;
        }

        private void DrawSidePanelResizePreview(int screenX)
        {
            int screenTop = this.PointToScreen(Point.Empty).Y;
            int screenBottom = this.PointToScreen(new Point(0, this.ClientSize.Height)).Y;
            ControlPaint.DrawReversibleLine(
                new Point(screenX, screenTop),
                new Point(screenX, screenBottom),
                Color.Gray);
        }

        private void UpdateResponsiveLayout()
        {
            UpdateSidePanelScrollBounds();
            UpdateToggleButtonPosition();
            panel2.PerformLayout();
            this.PerformLayout();
            UpdateMenuOverflowLayout();
            RefreshStatusScrollHost();
            if (dxControl2 != null)
                dxControl2.Invalidate();
        }

        private void UpdateSidePanelScrollBounds()
        {
            int maxRight = 0;
            int maxBottom = 0;
            foreach (Control control in panel1.Controls)
            {
                maxRight = Math.Max(maxRight, control.Right + control.Margin.Right);
                maxBottom = Math.Max(maxBottom, control.Bottom + control.Margin.Bottom);
            }
            panel1.AutoScrollMinSize = new Size(maxRight + 8, maxBottom + 8);
        }

        private void Form1_Load(object sender, EventArgs e) {
            // Initialize button position on form load
            UpdateResponsiveLayout();

            // Rebuild orbit chips after form is fully initialized (fixes AutoSize layout on first load)
            if(m_orbChipMap != null && m_orbChipMap.Count > 0) RebuildOrbitChips();
        }

        private void Form1_Resize(object sender, EventArgs e) {
            // Update button position when window resizes
            UpdateResponsiveLayout();
        }

        private void InitializeStatusScrollHost()
        {
            if (_statusScrollHost != null) return;

            _statusScrollHost = new Panel();
            _statusScrollHost.Name = "statusScrollHost";
            _statusScrollHost.Dock = DockStyle.Bottom;
            _statusScrollHost.AutoScroll = true;
            _statusScrollHost.AutoSize = false;
            _statusScrollHost.Margin = Padding.Empty;
            _statusScrollHost.Padding = Padding.Empty;
            _statusScrollHost.Height = statusStrip1.Height + SystemInformation.HorizontalScrollBarHeight;
            _statusScrollHost.BackColor = statusStrip1.BackColor;

            panel2.Controls.Remove(statusStrip1);
            panel2.Controls.Add(_statusScrollHost);
            _statusScrollHost.Controls.Add(statusStrip1);

            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(0, 0);
            statusStrip1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            statusStrip1.AutoSize = false;
            statusStrip1.CanOverflow = false;
            statusStrip1.GripStyle = ToolStripGripStyle.Hidden;
            statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip1.Margin = Padding.Empty;
            statusStrip1.Padding = Padding.Empty;
            statusStrip1.Stretch = false;
            statusStrip1.SizingGrip = false;
            pb_ClickStatus.Size = new Size(52, pb_ClickStatus.Height);
            pb_ClickStatus.Maximum = 100;
            pb_ClickStatus.Minimum = 0;
            activeKeybind.DropDownDirection = ToolStripDropDownDirection.AboveRight;
            foreach (ToolStripItem item in statusStrip1.Items)
                item.Overflow = ToolStripItemOverflow.Never;
        }

        private void RefreshStatusScrollHost()
        {
            if (_statusScrollHost == null || statusStrip1 == null) return;

            statusStrip1.PerformLayout();
            int contentWidth = statusStrip1.Padding.Horizontal;
            int contentHeight = statusStrip1.Padding.Vertical;

            foreach (ToolStripItem item in statusStrip1.Items)
            {
                if (!item.Visible) continue;
                Size preferred = item.GetPreferredSize(Size.Empty);
                int itemWidth = preferred.Width;
                int itemHeight = preferred.Height;
                if (item is ToolStripLabel || item is ToolStripStatusLabel)
                {
                    Size textSize = TextRenderer.MeasureText(
                        item.Text ?? string.Empty,
                        item.Font,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                    itemWidth = Math.Max(itemWidth, textSize.Width + item.Padding.Horizontal);
                    itemHeight = Math.Max(itemHeight, textSize.Height + item.Padding.Vertical);
                }

                contentWidth += itemWidth + item.Margin.Horizontal;
                contentHeight = Math.Max(contentHeight, itemHeight + item.Margin.Vertical);
            }

            contentWidth = Math.Max(contentWidth, _statusScrollHost.ClientSize.Width);
            contentHeight = Math.Max(contentHeight, statusStrip1.Height);

            bool needsHorizontalScroll = contentWidth > _statusScrollHost.ClientSize.Width;
            _statusScrollHost.Height = contentHeight + (needsHorizontalScroll ? SystemInformation.HorizontalScrollBarHeight : 0);
            statusStrip1.Size = new Size(contentWidth, contentHeight);
            _statusScrollHost.AutoScrollMinSize = new Size(statusStrip1.Size.Width, 0);
            _statusScrollHost.PerformLayout();
            panel2.PerformLayout();
        }

        private ToolStripItem CloneMenuItem(ToolStripItem source)
        {
            ToolStripMenuItem menu = source as ToolStripMenuItem;
            if (menu != null)
            {
                if (menu.DropDownItems.Count > 0)
                {
                    ToolStripMenuItem copy = new ToolStripMenuItem(menu.Text);
                    copy.Enabled = menu.Enabled;
                    copy.Checked = menu.Checked;
                    copy.CheckOnClick = menu.CheckOnClick;
                    copy.ShortcutKeys = menu.ShortcutKeys;
                    copy.ShortcutKeyDisplayString = menu.ShortcutKeyDisplayString;
                    copy.Tag = menu;
                    foreach (ToolStripItem child in menu.DropDownItems)
                        copy.DropDownItems.Add(CloneMenuItem(child));
                    HookMenuReleaseHandlers(copy);
                    return copy;
                }

                ToolStripMenuItem leaf = new ToolStripMenuItem(menu.Text);
                leaf.Enabled = menu.Enabled;
                leaf.Checked = menu.Checked;
                leaf.CheckOnClick = menu.CheckOnClick;
                leaf.ShortcutKeys = menu.ShortcutKeys;
                leaf.ShortcutKeyDisplayString = menu.ShortcutKeyDisplayString;
                leaf.Click += (s, e) => menu.PerformClick();
                leaf.Tag = menu;
                HookMenuReleaseHandlers(leaf);
                return leaf;
            }

            ToolStripSeparator separator = source as ToolStripSeparator;
            if (separator != null)
                return new ToolStripSeparator();

            ToolStripItem copyItem = new ToolStripMenuItem(source.Text);
            copyItem.Enabled = source.Enabled;
            copyItem.Tag = source;
            return copyItem;
        }

        private void InitializeMenuOverflow()
        {
            if (_menuOverflowItem != null) return;

            _topMenuItems = new List<ToolStripMenuItem>
            {
                fileToolStripMenuItem,
                editToolStripMenuItem,
                viewToolStripMenuItem,
                puzzleToolStripMenuItem,
                macroToolStripMenuItem,
                helpToolStripMenuItem
            };

            _menuOverflowItem = new ToolStripMenuItem("...");
            _menuOverflowItem.Name = "menuOverflowItem";
            _menuOverflowItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _menuOverflowItem.AutoSize = true;
            _menuOverflowItem.Alignment = ToolStripItemAlignment.Left;
            _menuOverflowItem.Overflow = ToolStripItemOverflow.Never;

            HookMenuReleaseHandlers(_menuOverflowItem);
            foreach (ToolStripMenuItem item in _topMenuItems)
            {
                item.Alignment = ToolStripItemAlignment.Left;
                item.Overflow = ToolStripItemOverflow.Never;
                HookMenuReleaseHandlers(item);
            }

            if (!menuStrip1.Items.Contains(_menuOverflowItem))
                menuStrip1.Items.Add(_menuOverflowItem);
        }

        private void HookMenuReleaseHandlers(ToolStripMenuItem item)
        {
            if (item == null) return;
            item.DropDownOpened -= MenuDropDownOpenedReleaseState;
            item.DropDownOpened += MenuDropDownOpenedReleaseState;
            foreach (ToolStripItem child in item.DropDownItems)
            {
                ToolStripMenuItem menuChild = child as ToolStripMenuItem;
                if (menuChild != null)
                    HookMenuReleaseHandlers(menuChild);
            }
        }

        private void MenuDropDownOpenedReleaseState(object sender, EventArgs e)
        {
            ReleaseAllKeyboardState();
        }

        private void UpdateMenuOverflowLayout()
        {
            if (_menuOverflowLayoutBusy || menuStrip1 == null || _menuOverflowItem == null || _topMenuItems == null)
                return;

            if (menuStrip1.ClientSize.Width <= 0)
                return;

            _menuOverflowLayoutBusy = true;
            try
            {
                menuStrip1.SuspendLayout();

                if (!menuStrip1.Items.Contains(_menuOverflowItem))
                    menuStrip1.Items.Add(_menuOverflowItem);

                int available = Math.Max(0, menuStrip1.ClientSize.Width - menuStrip1.Margin.Horizontal - menuStrip1.Padding.Horizontal);
                int overflowWidth = _menuOverflowItem.GetPreferredSize(Size.Empty).Width;

                List<ToolStripMenuItem> overflow = new List<ToolStripMenuItem>();

                foreach (ToolStripMenuItem item in _topMenuItems)
                    item.Visible = true;

                int used = 0;
                bool needOverflow = false;
                foreach (ToolStripMenuItem item in _topMenuItems)
                {
                    int width = item.GetPreferredSize(Size.Empty).Width;
                    if (!needOverflow && used + width <= available)
                    {
                        item.Visible = true;
                        used += width;
                    }
                    else
                    {
                        needOverflow = true;
                        item.Visible = false;
                        overflow.Add(item);
                    }
                }

                bool showOverflow = overflow.Count > 0;
                if (showOverflow)
                {
                    int budget = Math.Max(0, available - overflowWidth);
                    overflow.Clear();
                    used = 0;
                    needOverflow = false;

                    foreach (ToolStripMenuItem item in _topMenuItems)
                    {
                        int width = item.GetPreferredSize(Size.Empty).Width;
                        if (!needOverflow && used + width <= budget)
                        {
                            item.Visible = true;
                            used += width;
                        }
                        else
                        {
                            needOverflow = true;
                            item.Visible = false;
                            overflow.Add(item);
                        }
                    }
                }

                _menuOverflowItem.DropDownItems.Clear();
                if (showOverflow)
                {
                    foreach (ToolStripMenuItem item in overflow)
                        _menuOverflowItem.DropDownItems.Add(CloneMenuItem(item));
                    _menuOverflowItem.Visible = true;
                }
                else
                {
                    _menuOverflowItem.Visible = false;
                }
            }
            finally
            {
                menuStrip1.ResumeLayout(true);
                menuStrip1.PerformLayout();
                menuStrip1.Invalidate();
                _menuOverflowLayoutBusy = false;
            }
        }

        /// <summary>Return focus to the DirectX control on activation unless the
        /// user is already interacting with sidebar/menu controls.</summary>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!NonViewportUiHasFocus())
                dxControl2.Focus();
        }

        private void UpdateToggleButtonPosition() {
            int buttonY = (this.ClientSize.Height - btnTogglePanel.Height) / 2;
            if (m_panelCollapsed) {
                // Place at the very right edge of client area
                btnTogglePanel.Location = new System.Drawing.Point(this.ClientSize.Width - btnTogglePanel.Width, buttonY);
            } else {
                // Place on top of panel1's left edge, shifted left by 1px for better alignment
                btnTogglePanel.Location = new System.Drawing.Point(panel1.Location.X - 1, buttonY);
            }
            btnTogglePanel.BringToFront();
        }
    }
}
