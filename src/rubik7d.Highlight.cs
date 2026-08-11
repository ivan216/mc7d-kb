using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class Form1
    {
        bool AltHighlight=false;
        int[] NColMask;  // -1: only unhighlight, 0: normal (Indeterminate), 1: only highlight
        bool MaskStickers = false;  // true: exclude unchecked stickers from mesh (unclickable), false: only dim them
        CheckState HighlightByColorsState = CheckState.Unchecked;
        int[] FaceMask;
        Dictionary<int,CheckBox> m_orbChipMap; // orbit key -> chip checkbox
        int[] GripAxisMask = new int[8];  // index 1..7, -1=exclude, 0=neutral, 1=include
        Dictionary<int,int> m_orbitMaskCache; // reused by GetOrbitFilterMask
        Dictionary<int,CheckState> m_pendingOrbitChipStates; // persisted across chip rebuilds
        int[] GripLayerNum = new int[8];  // index 1..7, layer numbers 1..N

        private bool HasSelection(int[] mask, int startIndex, int endIndex) {
            for (int i = startIndex; i <= endIndex; i++) {
                if (mask[i] != 0) return true;
            }
            return false;
        }

        private void ProcessHighLights()
        {
            bool hasNColSelection = (cb_HighlightByColors.CheckState != CheckState.Unchecked)
                                    && HasSelection(NColMask, 1, 7);
            bool hasColorSelection = HasSelection(FaceMask, 1, 14);
            int[] effectiveNColMask = (cb_HighlightByColors.CheckState != CheckState.Unchecked) ? NColMask : null;
            Dictionary<int,int> rawOrbitMask = (cb_HighlightByColors.CheckState != CheckState.Unchecked)
                ? GetOrbitFilterMask() : null;

            Dictionary<int,int> effectiveOrbitMask = null;
            if(rawOrbitMask != null && effectiveNColMask != null) {
                bool ncolCloned = false;
                int[] allOrbitKeys = null;

                foreach(KeyValuePair<int,int> kv in rawOrbitMask) {
                    int orbitKey = kv.Key;
                    int chipVal = kv.Value;
                    int c = Cube7D.GetStkNColsFromOrbitKey(orbitKey);
                    if(effectiveNColMask[c] == -1) continue;

                    if(chipVal == -1) {
                        if(effectiveOrbitMask == null)
                            effectiveOrbitMask = new Dictionary<int,int>();
                        if(!effectiveOrbitMask.ContainsKey(orbitKey))
                            effectiveOrbitMask[orbitKey] = -1;
                    } else {
                        if(effectiveNColMask[c] == 0) {
                            if(!ncolCloned) {
                                effectiveNColMask = (int[])effectiveNColMask.Clone();
                                ncolCloned = true;
                            }
                            effectiveNColMask[c] = 1;
                        }

                        if(allOrbitKeys == null)
                            allOrbitKeys = Cube.GetAllOrbitKeys();
                        foreach(int other in allOrbitKeys) {
                            if(Cube7D.GetStkNColsFromOrbitKey(other) != c) continue;
                            if(other == orbitKey) continue;

                            int otherVal;
                            bool hasOther = rawOrbitMask.TryGetValue(other, out otherVal);
                            if(!hasOther || otherVal <= 0) {
                                if(effectiveOrbitMask == null)
                                    effectiveOrbitMask = new Dictionary<int,int>();
                                if(!effectiveOrbitMask.ContainsKey(other))
                                    effectiveOrbitMask[other] = -1;
                            }
                        }
                    }
                }
            }

            if (cb_HighlightByColors.CheckState == CheckState.Checked) {
                if (hasColorSelection || hasNColSelection)
                    Cube.FindStickersByMask(FaceMask, true, effectiveNColMask, effectiveOrbitMask);
                else
                    Cube.HighlightAll(effectiveNColMask, effectiveOrbitMask);
            } else if (cb_HighlightByColors.CheckState == CheckState.Indeterminate) {
                if (hasColorSelection || hasNColSelection)
                    Cube.FindStickersByMask(FaceMask, false, effectiveNColMask, effectiveOrbitMask);
                else {
                    if (HasSelection(GripAxisMask, 1, 7)) {
                        Cube.HighlightAll(null, effectiveOrbitMask);
                    } else {
                        Cube.HighLighted.SetAll(false);
                    }
                    Cube.HighLightGrip();
                    ApplyGripAxisFilters();
                    return;
                }
            } else {
                Cube.HighlightAll(null, effectiveOrbitMask);
            }

            Cube.HighLightGrip();
            if (cb_HighlightByColors.CheckState != CheckState.Unchecked)
                ApplyGripAxisFilters();
        }

        private void ApplyGripAxisFilters()
        {
            if (Cube == null) return;
            if(!HasSelection(GripAxisMask, 1, 7)) return;

            for (int axis = 1; axis <= Cube.D; axis++)
            {
                int maskVal = GripAxisMask[axis];
                if (maskVal == 0) continue;

                int rawMask = GripLayerNum[axis];
                if (rawMask == 0) continue;

                int m0 = rawMask >= 0 ? rawMask : Cube.reverse(-rawMask);
                bool inverted = (axis == 2 || axis == 5);
                if (inverted) m0 = Cube.reverse(m0);

                int oriented = Cube.Orient[axis - 1];
                if (oriented > 0) m0 = Cube.reverse(m0);

                int f0 = Math.Abs(oriented) - 1;
                Cube.ApplyGripHighlightToAxis(f0, m0, maskVal == -1);
            }
        }

        public void ProcessClick(MouseEventArgs e){
            dxControl2.ProcessPick(e,ETarget.TargetObject,new OnAction(mkPickObject));
        }

        int chstate(CheckState v) {
            return v==CheckState.Checked ? 1 : v==CheckState.Unchecked ? -1 : 0;
        }

        private void cb_Col1_CheckStateChanged(object sender,EventArgs e) {
            if(!m_setgeom){
                FaceMask[1]=chstate(cb_Col1.CheckState);
                FaceMask[2]=chstate(cb_Col2.CheckState);
                FaceMask[3]=chstate(cb_Col3.CheckState);
                FaceMask[4]=chstate(cb_Col4.CheckState);
                FaceMask[5]=chstate(cb_Col5.CheckState);
                FaceMask[6]=chstate(cb_Col6.CheckState);
                FaceMask[7]=chstate(cb_Col7.CheckState);
                FaceMask[8]=chstate(cb_Col8.CheckState);
                FaceMask[9]=chstate(cb_Col9.CheckState);
                FaceMask[10]=chstate(cb_Col10.CheckState);
                FaceMask[11]=chstate(cb_Col11.CheckState);
                FaceMask[12]=chstate(cb_Col12.CheckState);
                FaceMask[13]=chstate(cb_Col13.CheckState);
                FaceMask[14]=chstate(cb_Col14.CheckState);
                if(cb_HighlightByColors.CheckState!=CheckState.Unchecked){
                    ProcessHighLights();
                    Redraw();
                }
            }
        }

        private void cb_HighlightByColors_CheckedChanged(object sender,EventArgs e) {
            HighlightByColorsState = cb_HighlightByColors.CheckState;
            if(m_setgeom || Cube == null) return;
            ProcessHighLights();
            Redraw();
        }

        private void cb_MaskStickers_CheckedChanged(object sender, EventArgs e) {
            if (!m_setgeom) {
                MaskStickers = cb_MaskStickers.Checked;
                if (CubeView != null) {
                    CubeView.MaskStickers = MaskStickers;
                    CubeView.Dispose();
                }
                Redraw();
            }
        }

        private void btn_ResetHighlightSelection_Click(object sender, EventArgs e) {
            for (int i = 1; i <= 14; i++) FaceMask[i] = 0;
            for (int i = 1; i <= 7; i++) NColMask[i] = 0;
            for (int i = 1; i <= 7; i++) {
                GripAxisMask[i] = 0;
                GripLayerNum[i] = 1;
            }

            m_setgeom = true;
            cb_Col1.CheckState = CheckState.Indeterminate;
            cb_Col2.CheckState = CheckState.Indeterminate;
            cb_Col3.CheckState = CheckState.Indeterminate;
            cb_Col4.CheckState = CheckState.Indeterminate;
            cb_Col5.CheckState = CheckState.Indeterminate;
            cb_Col6.CheckState = CheckState.Indeterminate;
            cb_Col7.CheckState = CheckState.Indeterminate;
            cb_Col8.CheckState = CheckState.Indeterminate;
            cb_Col9.CheckState = CheckState.Indeterminate;
            cb_Col10.CheckState = CheckState.Indeterminate;
            cb_Col11.CheckState = CheckState.Indeterminate;
            cb_Col12.CheckState = CheckState.Indeterminate;
            cb_Col13.CheckState = CheckState.Indeterminate;
            cb_Col14.CheckState = CheckState.Indeterminate;

            cb_Show1C.CheckState = CheckState.Indeterminate;
            cb_Show2C.CheckState = CheckState.Indeterminate;
            cb_Show3C.CheckState = CheckState.Indeterminate;
            cb_Show4C.CheckState = CheckState.Indeterminate;
            cb_Show5C.CheckState = CheckState.Indeterminate;
            cb_Show6C.CheckState = CheckState.Indeterminate;
            cb_Show7C.CheckState = CheckState.Indeterminate;

            cb_GripAxis1.CheckState = CheckState.Indeterminate;
            cb_GripAxis2.CheckState = CheckState.Indeterminate;
            cb_GripAxis3.CheckState = CheckState.Indeterminate;
            cb_GripAxis4.CheckState = CheckState.Indeterminate;
            cb_GripAxis5.CheckState = CheckState.Indeterminate;
            cb_GripAxis6.CheckState = CheckState.Indeterminate;
            cb_GripAxis7.CheckState = CheckState.Indeterminate;

            nud_GripLayer1.Value = 1;
            nud_GripLayer2.Value = 1;
            nud_GripLayer3.Value = 1;
            nud_GripLayer4.Value = 1;
            nud_GripLayer5.Value = 1;
            nud_GripLayer6.Value = 1;
            nud_GripLayer7.Value = 1;

            ResetOrbitChips();

            m_setgeom = false;

            ProcessHighLights();
            Redraw();
        }

        private void cb_GripAxis_CheckStateChanged(object sender, EventArgs e)
        {
            if (m_setgeom) return;
            CheckBox[] boxes = new CheckBox[] { cb_GripAxis1, cb_GripAxis2, cb_GripAxis3, cb_GripAxis4,
                                                cb_GripAxis5, cb_GripAxis6, cb_GripAxis7 };
            for (int i = 0; i < 7; i++)
                GripAxisMask[i + 1] = (boxes[i].CheckState == CheckState.Checked) ? 1
                                    : (boxes[i].CheckState == CheckState.Unchecked) ? -1 : 0;
            ProcessHighLights();
            Redraw();
        }

        private void nud_GripLayer_ValueChanged(object sender, EventArgs e)
        {
            if (m_setgeom) return;
            NumericUpDown[] nuds = new NumericUpDown[] { nud_GripLayer1, nud_GripLayer2, nud_GripLayer3, nud_GripLayer4,
                                                         nud_GripLayer5, nud_GripLayer6, nud_GripLayer7 };
            for (int i = 0; i < 7; i++)
                GripLayerNum[i + 1] = (int)nuds[i].Value;
            ProcessHighLights();
            Redraw();
        }

        private void cb_Show1C_CheckedChanged(object sender,EventArgs e) {
            if(!m_setgeom) {
                CheckState[] st = new CheckState[] { cb_Show1C.CheckState, cb_Show2C.CheckState, cb_Show3C.CheckState,
                                                      cb_Show4C.CheckState, cb_Show5C.CheckState, cb_Show6C.CheckState, cb_Show7C.CheckState };
                for(int i=1; i<=7; i++) {
                    NColMask[i] = (st[i-1] == CheckState.Checked) ? 1 : (st[i-1] == CheckState.Unchecked) ? -1 : 0;
                }
                ProcessHighLights();
                Redraw();
            }
        }

        private int CompareOrbitKeysForUi(int a, int b) {
            ushort sigA = Cube7D.GetTierSigFromOrbitKey(a);
            ushort sigB = Cube7D.GetTierSigFromOrbitKey(b);
            int t1a = (sigA >> 3) & 7, t1b = (sigB >> 3) & 7; if(t1a != t1b) return t1a.CompareTo(t1b);
            int t2a = (sigA >> 6) & 7, t2b = (sigB >> 6) & 7; if(t2a != t2b) return t2a.CompareTo(t2b);
            int t3a = (sigA >> 9) & 7, t3b = (sigB >> 9) & 7; if(t3a != t3b) return t3a.CompareTo(t3b);
            int t4a = (sigA >> 12) & 7, t4b = (sigB >> 12) & 7; if(t4a != t4b) return t4a.CompareTo(t4b);

            int kindA = Cube7D.GetOrbitKind(a);
            int kindB = Cube7D.GetOrbitKind(b);
            if(kindA == kindB) return 0;
            if(kindA == 0) return -1;
            if(kindB == 0) return 1;
            return kindA.CompareTo(kindB);
        }

        private void CaptureOrbitChipStates() {
            if(m_orbChipMap == null) return;
            if(m_orbChipMap.Count == 0) return;
            m_pendingOrbitChipStates.Clear();
            foreach(var kv in m_orbChipMap) {
                if(kv.Value.CheckState != CheckState.Indeterminate)
                    m_pendingOrbitChipStates[kv.Key] = kv.Value.CheckState;
            }
        }

        private void ApplyPendingOrbitChipStates() {
            if(m_orbChipMap == null || m_pendingOrbitChipStates == null) return;
            foreach(var kv in m_pendingOrbitChipStates) {
                CheckBox chip;
                if(m_orbChipMap.TryGetValue(kv.Key, out chip))
                    chip.CheckState = kv.Value;
            }
        }

        void RebuildOrbitChips() {
            RebuildOrbitChips(true);
        }

        void RebuildOrbitChips(bool preserveState) {
            if(preserveState) CaptureOrbitChipStates();
            else m_pendingOrbitChipStates.Clear();
            m_pnlOrbitFilters.Controls.Clear();
            m_orbChipMap.Clear();

            if(Cube==null) return;

            int[] allOrbitKeys = Cube.GetAllOrbitKeys();
            if(allOrbitKeys.Length==0) return;

            int maxTier=(Cube.N-1)/2;

            List<int>[] groups=new List<int>[8];
            for(int c=1;c<=7;c++) groups[c]=new List<int>();
            for(int i=0;i<allOrbitKeys.Length;i++) {
                int cVal=Cube7D.GetStkNColsFromOrbitKey(allOrbitKeys[i]);
                if(cVal>=1 && cVal<=7) groups[cVal].Add(allOrbitKeys[i]);
            }

            int y=3, gap=8;
            int orbitTabIndex = 0;
            int panelW=m_pnlOrbitFilters.ClientSize.Width;
            System.Drawing.Font font7=new System.Drawing.Font("Microsoft Sans Serif",8f);
            m_setgeom = true;

            for(int c=1;c<=7;c++) {
                if(groups[c].Count==0) continue;

                var lbl=new Label();
                lbl.Text="C"+c+":";
                lbl.Location=new System.Drawing.Point(3,y);
                lbl.Font=font7;
                lbl.AutoSize=true;
                lbl.TextAlign=ContentAlignment.MiddleLeft;
                lbl.TabStop = false;
                m_pnlOrbitFilters.Controls.Add(lbl);

                groups[c].Sort(CompareOrbitKeysForUi);

                int x=40;
                int rowH=0;
                foreach(int orbitKey in groups[c]) {
                    var chip=new CheckBox();
                    chip.Text=Cube7D.FormatOrbitKeyLabel(orbitKey,maxTier);
                    chip.Font=font7;
                    chip.ThreeState=true;
                    chip.CheckState=CheckState.Indeterminate;
                    chip.Tag=orbitKey;
                    chip.CheckStateChanged+=ChipOrbit_CheckStateChanged;
                    chip.AutoSize=true;
                    chip.TabIndex=orbitTabIndex++;
                    chip.Location=new System.Drawing.Point(x,y);
                    m_pnlOrbitFilters.Controls.Add(chip);
                    m_orbChipMap[orbitKey]=chip;

                    if(rowH < chip.Height+gap) rowH=chip.Height+gap;

                    int chipW=Math.Max(chip.Width,30);
                    if(x+chipW>panelW) { x=40; y+=rowH; chip.Location=new System.Drawing.Point(x,y); }
                    x+=chipW+gap;
                }
                y+=rowH+4;
            }

            if(preserveState) ApplyPendingOrbitChipStates();
            m_setgeom = false;
        }

        void ChipOrbit_CheckStateChanged(object sender,EventArgs e) {
            if(m_setgeom) return;
            CheckBox chip = sender as CheckBox;
            if(chip != null && chip.Tag is int) {
                int orbitKey = (int)chip.Tag;
                if(chip.CheckState == CheckState.Indeterminate) m_pendingOrbitChipStates.Remove(orbitKey);
                else m_pendingOrbitChipStates[orbitKey] = chip.CheckState;
            }
            ProcessHighLights();
            Redraw();
        }

        Dictionary<int,int> GetOrbitFilterMask() {
            if(m_orbChipMap==null || m_orbChipMap.Count==0) return null;

            m_orbitMaskCache.Clear();
            bool any=false;
            foreach(var kv in m_orbChipMap) {
                int val=0;
                switch(kv.Value.CheckState) {
                    case CheckState.Checked: val=1; any=true; break;
                    case CheckState.Unchecked: val=-1; any=true; break;
                }
                if(val!=0) m_orbitMaskCache[kv.Key]=val;
            }
            return any ? m_orbitMaskCache : null;
        }

        void ResetOrbitChips() {
            m_pendingOrbitChipStates.Clear();
            if(m_orbChipMap==null) return;
            foreach(var chip in m_orbChipMap.Values) {
                chip.CheckState=CheckState.Indeterminate;
            }
        }
    }
}
