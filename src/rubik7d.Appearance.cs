using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;

namespace _3dedit
{
    public partial class Form1
    {
        int ClickMode=CLICK_MODE_2;
        int DiffLight=150;
        int SpecLight=100;
        bool m_setgeom=false;
        long m_TStart=0;
        bool m_TRun=false;
        System.Threading.Timer m_Timer;

        void RefreshView(){
            dxControl2.ParkCamera(false);
            dxControl2.SetSceneChanged();
        }

        void RedrawClickStatus() {
            int twist3cStep = Cube?.partialTwist3c?.step ?? 0;

            if (twist3cStep > 0) {
                pb_ClickStatus.ForeColor = Color.Blue;
                pb_ClickStatus.Value = twist3cStep == 1 ? 55 : 95;
            } else {
                pb_ClickStatus.ForeColor = ClickQual ? Color.Green : Color.Red;
                if (!ClickQual) pb_ClickStatus.Value = 95;
                else if (NClicks <= 0) pb_ClickStatus.Value = 12;
                else if (NClicks == 1) pb_ClickStatus.Value = 55;
                else pb_ClickStatus.Value = 95;
            }

            Color bg=Color.Black;
            if(RecordingMacroStatus==REC_MACRO_STICKERS || RecordingMacroStatus==REC_MACRO_APPLY) bg=Color.White;
            if(!ClickQual && twist3cStep == 0) bg=Color.DarkRed;
            dxControl2.Scene.BGColor=bg;

            switch(RecordingMacroStatus) {
                case REC_MACRO_NONE:
                    ms_MacroStatus.Text="  Ready"; break;
                case REC_MACRO_STICKERS:
                case REC_MACRO_APPLY:
                    ms_MacroStatus.Text="  Select Stickers: "+LMacroStickers; break;
                case REC_MACRO_CODE:
                    ms_MacroStatus.Text="  Enter macro: "+Cube.GetNTwists(MacroStart,Cube.LPtr); break;
            }
            UpdateTime(null);
        }

        void UpdateTime(object xxx) {
            if (InvokeRequired)
            {
                if (IsHandleCreated && !IsDisposed)
                    BeginInvoke((MethodInvoker)(() => UpdateTime(xxx)));
                return;
            }
            if(Cube==null) return;
            long s=m_TRun ? (DateTime.Now.Ticks-m_TStart) : Cube.CTime;
            s/=10000;
            int tms=(int)(s%1000); s/=1000;
            int ts=(int)(s%60); s/=60;
            int tm=(int)(s%60); s/=60;
            string m=String.Format("   Time: {0}:{1:D2}:{2:D2}.{3:D3}",s,tm,ts,tms);
            m_lblCTime.Text=m;
            RefreshStatusScrollHost();
        }

        private void Form1_KeyPress(object sender,KeyPressEventArgs e) {

        }

        void InitGeomSettings() {
            m_setgeom=true;
            trk_BlockSize.Value=Math.Max(1,(int)(Cube7D.BSize*trk_BlockSize.Maximum+0.5));
            trk_Perspective.Value=Math.Max(0,(int)(Cube7D.FExt/2*trk_Perspective.Maximum+0.5));
            trk_faceSep.Value=Math.Max(0,(int)(Cube7D.FSep*trk_faceSep.Maximum+0.5));
            trk_StickerSize.Value=Math.Max(1,(int)(Cube7D.SSize*trk_StickerSize.Maximum+0.5));
            m_bCol1.BackColor=Color.FromArgb((int)CubeObj.Colors[1]);
            m_bCol2.BackColor=Color.FromArgb((int)CubeObj.Colors[2]);
            m_bCol3.BackColor=Color.FromArgb((int)CubeObj.Colors[3]);
            m_bCol4.BackColor=Color.FromArgb((int)CubeObj.Colors[4]);
            m_bCol5.BackColor=Color.FromArgb((int)CubeObj.Colors[5]);
            m_bCol6.BackColor=Color.FromArgb((int)CubeObj.Colors[6]);
            m_bCol7.BackColor=Color.FromArgb((int)CubeObj.Colors[7]);
            m_bCol8.BackColor=Color.FromArgb((int)CubeObj.Colors[8]);
            m_bCol9.BackColor=Color.FromArgb((int)CubeObj.Colors[9]);
            m_bCol10.BackColor=Color.FromArgb((int)CubeObj.Colors[10]);
            m_bCol11.BackColor=Color.FromArgb((int)CubeObj.Colors[11]);
            m_bCol12.BackColor=Color.FromArgb((int)CubeObj.Colors[12]);
            m_bCol13.BackColor=Color.FromArgb((int)CubeObj.Colors[13]);
            m_bCol14.BackColor=Color.FromArgb((int)CubeObj.Colors[14]);
            switch(ClickMode) {
                case CLICK_MODE_2: m_rbClick2.Checked=true; break;
                case CLICK_MODE_2_OPP: m_rbClick2Inv.Checked=true; break;
                case CLICK_MODE_3: m_rbClick3.Checked=true; break;
            }
            CheckState[] st=new CheckState[] { CheckState.Unchecked,CheckState.Indeterminate,CheckState.Checked };
            cb_Col1.CheckState=st[FaceMask[1]+1];
            cb_Col2.CheckState=st[FaceMask[2]+1];
            cb_Col3.CheckState=st[FaceMask[3]+1];
            cb_Col4.CheckState=st[FaceMask[4]+1];
            cb_Col5.CheckState=st[FaceMask[5]+1];
            cb_Col6.CheckState=st[FaceMask[6]+1];
            cb_Col7.CheckState=st[FaceMask[7]+1];
            cb_Col8.CheckState=st[FaceMask[8]+1];
            cb_Col9.CheckState=st[FaceMask[9]+1];
            cb_Col10.CheckState=st[FaceMask[10]+1];
            cb_Col11.CheckState=st[FaceMask[11]+1];
            cb_Col12.CheckState=st[FaceMask[12]+1];
            cb_Col13.CheckState=st[FaceMask[13]+1];
            cb_Col14.CheckState=st[FaceMask[14]+1];

            cb_Show1C.CheckState = (NColMask[1] == 1) ? CheckState.Checked : (NColMask[1] == -1) ? CheckState.Unchecked : CheckState.Indeterminate;
            cb_Show2C.CheckState = (NColMask[2] == 1) ? CheckState.Checked : (NColMask[2] == -1) ? CheckState.Unchecked : CheckState.Indeterminate;
            cb_Show3C.CheckState = (NColMask[3] == 1) ? CheckState.Checked : (NColMask[3] == -1) ? CheckState.Unchecked : CheckState.Indeterminate;
            cb_Show4C.CheckState = (NColMask[4] == 1) ? CheckState.Checked : (NColMask[4] == -1) ? CheckState.Unchecked : CheckState.Indeterminate;
            cb_Show5C.CheckState = (NColMask[5] == 1) ? CheckState.Checked : (NColMask[5] == -1) ? CheckState.Unchecked : CheckState.Indeterminate;
            cb_Show6C.CheckState = (NColMask[6] == 1) ? CheckState.Checked : (NColMask[6] == -1) ? CheckState.Unchecked : CheckState.Indeterminate;
            cb_Show7C.CheckState = (NColMask[7] == 1) ? CheckState.Checked : (NColMask[7] == -1) ? CheckState.Unchecked : CheckState.Indeterminate;

            trk_LightDiff.Value=DiffLight;
            trk_LightSpec.Value=SpecLight;
            m_trkTransparency.Value=255-CubeObj.Transparency;
            cb_HighlightByColors.CheckState = HighlightByColorsState;
            cb_MaskStickers.Checked = MaskStickers;

            int gnudsMax = (Cube != null) ? ((1 << Cube.N) - 1) : ((1 << Cube7D.MaxN) - 1);
            SyncGripLayerNUDs(gnudsMax);
            CheckState[] st3 = new CheckState[] { CheckState.Unchecked, CheckState.Indeterminate, CheckState.Checked };
            cb_GripAxis1.CheckState = st3[GripAxisMask[1] + 1];
            cb_GripAxis2.CheckState = st3[GripAxisMask[2] + 1];
            cb_GripAxis3.CheckState = st3[GripAxisMask[3] + 1];
            cb_GripAxis4.CheckState = st3[GripAxisMask[4] + 1];
            cb_GripAxis5.CheckState = st3[GripAxisMask[5] + 1];
            cb_GripAxis6.CheckState = st3[GripAxisMask[6] + 1];
            cb_GripAxis7.CheckState = st3[GripAxisMask[7] + 1];

            m_setgeom=false;
        }

        private void trk_faceSep_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom) {
                Cube7D.BSize=(double)trk_BlockSize.Value/trk_BlockSize.Maximum;
                Cube7D.FExt=2.0*trk_Perspective.Value/trk_Perspective.Maximum;
                Cube7D.FSep=(double)trk_faceSep.Value/trk_faceSep.Maximum;
                Cube7D.SSize=(double)trk_StickerSize.Value/trk_StickerSize.Maximum;
                if(Cube!=null) {
                    Cube.SetCoords();
                    Redraw();
                }
            }
        }

        private void m_bCol1_Click(object sender,EventArgs e) { ChangeColor(m_bCol1,1); }
        private void m_bCol2_Click(object sender,EventArgs e) { ChangeColor(m_bCol2,2); }
        private void m_bCol3_Click(object sender,EventArgs e) { ChangeColor(m_bCol3,3); }
        private void m_bCol4_Click(object sender,EventArgs e) { ChangeColor(m_bCol4,4); }
        private void m_bCol5_Click(object sender,EventArgs e) { ChangeColor(m_bCol5,5); }
        private void m_bCol6_Click(object sender,EventArgs e) { ChangeColor(m_bCol6,6); }
        private void m_bCol7_Click(object sender,EventArgs e) { ChangeColor(m_bCol7,7); }
        private void m_bCol8_Click(object sender,EventArgs e) { ChangeColor(m_bCol8,8); }
        private void m_bCol9_Click(object sender,EventArgs e) { ChangeColor(m_bCol9,9); }
        private void m_bCol10_Click(object sender,EventArgs e) { ChangeColor(m_bCol10,10); }
        private void m_bCol11_Click(object sender,EventArgs e) { ChangeColor(m_bCol11,11); }
        private void m_bCol12_Click(object sender,EventArgs e) { ChangeColor(m_bCol12,12); }
        private void m_bCol13_Click(object sender,EventArgs e) { ChangeColor(m_bCol13,13); }
        private void m_bCol14_Click(object sender,EventArgs e) { ChangeColor(m_bCol14,14); }

        void ChangeColor(Button btn,int cid){
            ColorDialog c=new ColorDialog();
            c.Color=btn.BackColor;
            if(c.ShowDialog()==DialogResult.OK) {
                btn.BackColor=c.Color;
                CubeObj.Colors[cid]=(uint)(c.Color.ToArgb());
                Redraw();
            }
        }

        private void ClickMode_CheckedChanged(object sender,EventArgs e) {
            if(m_rbClick3.Checked) ClickMode=CLICK_MODE_3;
            else if(m_rbClick2Inv.Checked) ClickMode=CLICK_MODE_2_OPP;
            else ClickMode=CLICK_MODE_2;
        }

        void SetLight() {
            foreach(CameraLight L in dxControl2.Scene.Lights) {
                L.Specular=SpecLight;
                L.Diffuse=DiffLight;
            }
            dxControl2.SetSceneChanged();
        }

        private int ClampGripLayerNum(int value, int maxVal)
        {
            return Math.Max(Math.Min(value, maxVal), -maxVal);
        }

        private void SyncGripLayerNUDs(int maxVal)
        {
            NumericUpDown[] nuds = new NumericUpDown[] { nud_GripLayer1, nud_GripLayer2, nud_GripLayer3, nud_GripLayer4,
                                                         nud_GripLayer5, nud_GripLayer6, nud_GripLayer7 };
            int minVal = -maxVal;
            m_setgeom = true;
            for (int i = 0; i < 7; i++) {
                int clamped = ClampGripLayerNum(GripLayerNum[i + 1], maxVal);
                GripLayerNum[i + 1] = clamped;
                nuds[i].Maximum = maxVal;
                nuds[i].Minimum = minVal;
                if (nuds[i].Value != clamped) nuds[i].Value = clamped;
            }
            m_setgeom = false;
        }

        private void UpdateGripAxisNUDs()
        {
            if (Cube == null) return;
            int maxVal = (1 << Cube.N) - 1;
            // Reset grip state for axes beyond current dimension before syncing UI/model.
            for (int i = Cube.D + 1; i <= 7; i++) {
                GripAxisMask[i] = 0;
                GripLayerNum[i] = 1;
            }
            SyncGripLayerNUDs(maxVal);
        }

        private void trk_LightDiff_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom){
            DiffLight=trk_LightDiff.Value;
            SetLight();
            }
        }

        private void trk_LightSpec_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom){
            SpecLight=trk_LightSpec.Value;
            SetLight();
            }
        }
    }
}
