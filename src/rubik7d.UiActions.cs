using System;
using System.Drawing;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class Form1
    {
        int TRate=500;
        int[,] RevStack=new int[100,2];
        int LRevStack=0;

        static string GetBuildDate() {
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return new DateTime(2000, 1, 1).AddDays(v.Build).ToString("yyyy.MM.dd");
        }

        private void aboutToolStripMenuItem_Click(object sender,EventArgs e) {
            MessageBox.Show(string.Format(Properties.Resources.AboutText, VERSION, GetBuildDate()));
        }

        private void usageGuideToolStripMenuItem_Click(object sender,EventArgs e) {
            Form guideForm = new Form();
            guideForm.Text = "Usage Guide";
            guideForm.Size = new System.Drawing.Size(640, 480);
            guideForm.StartPosition = FormStartPosition.CenterParent;
            guideForm.MinimumSize = new System.Drawing.Size(480, 320);
            guideForm.MinimizeBox = false;
            guideForm.MaximizeBox = false;

            TextBox tb = new TextBox();
            tb.Multiline = true;
            tb.ReadOnly = true;
            tb.ScrollBars = ScrollBars.Vertical;
            tb.Dock = DockStyle.Fill;
            tb.WordWrap = true;
            tb.Font = new System.Drawing.Font("Segoe UI", 10f);
            tb.Padding = new Padding(10);
            tb.Text = Properties.Resources.UsageGuideText;
            tb.Select(0, 0);

            guideForm.Controls.Add(tb);
            guideForm.ShowDialog(this);
        }

        private void startExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>=RevStack.GetLength(0)) {
                int[,] stk=new int[2*LRevStack,2];
                Buffer.BlockCopy(RevStack,0,stk,0,8*LRevStack);
                RevStack=stk;
            }
            RevStack[LRevStack,0]=Cube.LPtr;
            RevStack[LRevStack,1]=-1;
            LRevStack++;
            ShowRevStack();
        }

        private void stopExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>0) {
                if(RevStack[LRevStack-1,1]>=0 || Cube.LPtr<=RevStack[LRevStack-1,0]) LRevStack--;
                else RevStack[LRevStack-1,1]=Cube.LPtr;
            }
            ShowRevStack();
        }

        private void undoExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>0) {
                int r=RevStack[LRevStack-1,1];
                if(r>=0) {
                    int p=Cube.LPtr;
                    if(p>=r) {
                        Cube.ApplySeqReverse(RevStack[LRevStack-1,0],r);
                        if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                            Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                    }
                }
                LRevStack--;
            }
            Redraw();
        }

        private void commutatorToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>0) {
                int r=RevStack[LRevStack-1,1];
                if(r>=0) {
                    int p=Cube.LPtr;
                    if(p>=r) {
                        Cube.ApplySeqReverse(RevStack[LRevStack-1,0],r);
                        Cube.ApplySeqReverse(r,p);
                        ProcessHighLights();
                    }
                }
                LRevStack--;
            }
            Redraw();
        }

        private void ShowRevStack() {
            int p=Cube.LShuffle;
            int q=Cube.LPtr;

            string l="";
            for(int i=0;i<LRevStack;i++) {
                int r=RevStack[i,0];
                if(r!=p) l+=Cube.GetNTwists(p,r).ToString()+"[";
                else l+="[";
                p=r;
                r=RevStack[i,1];
                if(r>=0) {
                    l+=Cube.GetNTwists(p,r).ToString()+"]";
                    p=r;
                }
            }
            if(p<q) l+=Cube.GetNTwists(p,q).ToString();

            ms_RevStack.Text = "RevStack: " + l;
            ms_Twists.Text="  Twists: "+Cube.NTwists;
            RefreshStatusScrollHost();
        }

        private void recalculateToolStripMenuItem_Click(object sender,EventArgs e) {
            Cube.Recalculate();
            Redraw();
        }

        private void m_trkUndoSpeed_ValueChanged(object sender,EventArgs e) {
            if(m_trkFullUndoSpeed.Value==m_trkFullUndoSpeed.Maximum) {
                TRate=0;
            } else {
                double p=(double)(m_trkFullUndoSpeed.Value)/m_trkFullUndoSpeed.Maximum;
                p=Math.Pow(10,3-2*p);
                TRate=(int)p;
            }
        }

        private void m_trkTransparency_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom) {
                CubeObj.Transparency=255-m_trkTransparency.Value;
                Redraw();
            }

        }

        private void editKeybinds_Click(object sender, EventArgs e)
        {
            if (KeybindsSetup == null || KeybindsSetup.IsDisposed)
            {
                KeybindsSetup = new KeybindSetup(this.Keybinds, menuStrip1);
            }
            KeybindsSetup.Show();
            KeybindsSetup.Focus();
            KeybindsSetup.WindowState = FormWindowState.Normal;
        }

        private void ToggleKeybindsReference()
        {
            if (KeybindsRef == null || KeybindsRef.IsDisposed)
            {
                KeybindsRef = new KeybindsReference(Keybinds, menuStrip1);
                KeybindsRef.PhysicalKeyDown = (key) => { var e = new KeyEventArgs(key); KeyDownEvt(null, e); };
                KeybindsRef.PhysicalKeyUp = (key) => { var e = new KeyEventArgs(key); KeyUpEvt(null, e); };
                KeybindsRef.ViewportFocusRequested = () => dxControl2.Focus();
                KeybindsRef.Show(this);
                KeybindsRef.FormClosed += (s, fce) => { _refMenuItem.Checked = false; };
                _refMenuItem.Checked = true;
            }
            else
            {
                KeybindsRef.Close();
                KeybindsRef = null;
                _refMenuItem.Checked = false;
            }
        }

        void UpdateKeybindMenu(object sender, EventArgs e)
        {
            activeKeybind.DropDownItems.Clear();
            foreach (var item in Keybinds.keybinds)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = item.Key;
                menuItem.Click += new System.EventHandler(this.KeybindMenuItem_Click);

                activeKeybind.DropDownItems.Add(menuItem);
            }
            this.CheckKeybindSet(sender, e);
            RefreshStatusScrollHost();
        }

        void KeybindMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Keybinds.switchKeybindSet(item.Text);
        }
    }
}
