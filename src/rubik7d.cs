using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace _3dedit
{

    public partial class Form1:System.Windows.Forms.Form {
        public string VERSION = "v0.8.4";
        static string GetBuildDate() {
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return new DateTime(2000, 1, 1).AddDays(v.Build).ToString("yyyy.MM.dd");
        }
		public Form1() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// 
			// dxControl2
			// 
			this.dxControl2 = new _3dedit.DXControl();


			this.dxControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dxControl2.Location = new System.Drawing.Point(0, 28);
            this.dxControl2.Name = "dxControl2";
            this.dxControl2.Size = new System.Drawing.Size(793, 544);
            this.dxControl2.TabIndex = 0;
			this.dxControl2.MouseUp += new MouseEventHandler(MouseUpEvt);
			this.dxControl2.MouseDown += new MouseEventHandler(MouseDownEvt);
			this.dxControl2.MouseMove += new MouseEventHandler(MouseEvt);
            this.dxControl2.KeyDown += new KeyEventHandler(KeyDownEvt);
            this.dxControl2.KeyUp += new KeyEventHandler(KeyUpEvt);
            this.dxControl2.Leave += new EventHandler(dxControl2_Leave);

			this.panel2.Controls.Add(this.dxControl2);
            this.panel2.Controls.SetChildIndex(this.dxControl2, 0);

            NColMask=new int[8];
            FaceMask=new int[15];
            for(int i=0;i<8;i++) NColMask[i]=0;  // 0 = Indeterminate (normal/neutral)
            for(int i=0;i<15;i++) FaceMask[i]=0;
            for(int i=1;i<=7;i++) { GripAxisMask[i]=0; GripLayerNum[i]=1; }

            m_orbChipMap=new Dictionary<int,CheckBox>();
            m_orbitMaskCache=new Dictionary<int,int>();
            m_pendingOrbitChipStates=new Dictionary<int,CheckState>();

            LoadSettings("MC7D_settings.txt");
            Macros=new CMacroFile(GetDim(),GetSize());
            m_Timer=new System.Threading.Timer(this.UpdateTime,null,0,117);

            // UpdateTime complains about access from another thread if we try this before creating m_timer???
            LoadKeybinds("MC7D_keybinds.txt");
            Keybinds.ActiveLayoutChanged += this.CheckKeybindSet;
            Keybinds.KeybindLayoutsChanged += this.UpdateKeybindMenu;
            this.UpdateKeybindMenu(null, EventArgs.Empty);
            Keybindings.loaded = Keybinds;

            // Add "Keybinds Reference" menu item after "Edit Keybinds"
            _refMenuItem = new ToolStripMenuItem("Keybinds Reference");
            _refMenuItem.Click += (s, me) => ToggleKeybindsReference();
            viewToolStripMenuItem.DropDownItems.Add(_refMenuItem);

            // Wire up macro hotkey execution
            Keybindings.ExecuteMacroById = ExecuteMacroByIdCmd;

            // Block wheel on TrackBar/NumericUpDown; redirect to parent panel for scrolling
            Application.AddMessageFilter(new WheelGuard(this));
            // Catch ALL key up/down messages before any control filters them
            Application.AddMessageFilter(new KeybindsRefreshFilter(this));
            // Ensure the DirectX control has focus at startup so keybinds work immediately
            this.Shown += (s, me) => dxControl2.Focus();
            // Click sidebar background → move focus away from sidebar controls
            panel1.MouseDown += (s, me) => dxControl2.Focus();

            // Release grip/key state when menu is opened (dxControl2 loses focus
            // but keyboard events may not fire cleanly during menu navigation).
            menuStrip1.MenuActivate += (s, me) => ReleaseAllKeyboardState();
            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.DropDownOpened += (s, me) => ReleaseAllKeyboardState();
            }

            // Undo frame skip control — placed below speed slider
            nudUndoFrameSkip = new NumericUpDown();
            nudUndoFrameSkip.Location = new System.Drawing.Point(97, 268);
            nudUndoFrameSkip.Size = new System.Drawing.Size(76, 20);
            nudUndoFrameSkip.Minimum = 1;
            nudUndoFrameSkip.Maximum = 100000;
            nudUndoFrameSkip.Value = 1;
            nudUndoFrameSkip.Name = "nudUndoFrameSkip";
            panel1.Controls.Add(nudUndoFrameSkip);

            lblUndoFrameSkip = new Label();
            lblUndoFrameSkip.Location = new System.Drawing.Point(9, 270);
            lblUndoFrameSkip.Size = new System.Drawing.Size(75, 13);
            lblUndoFrameSkip.Text = "Frame Skip";
            panel1.Controls.Add(lblUndoFrameSkip);

            // Shift existing controls below the TrackBar down to make room
            foreach (Control c in panel1.Controls) {
                if (c.Top >= 277 && c != nudUndoFrameSkip && c != lblUndoFrameSkip) {
                    c.Top += 23;
                }
            }
        }

        int GetDim() {
            if(mi_Puzzle4D.Checked) return 4;
            if(mi_Puzzle5D.Checked) return 5;
            if(mi_Puzzle6D.Checked) return 6;
            if(mi_Puzzle7D.Checked) return 7;
            return 4;
        }
        void SetDim(int n) {
            mi_Puzzle4D.Checked=(n==4);
            mi_Puzzle5D.Checked=(n==5);
            mi_Puzzle6D.Checked=(n==6);
            mi_Puzzle7D.Checked=(n==7);
        }
        int GetSize() {
            if(mi_PuzzleSize2.Checked) return 2;
            if(mi_PuzzleSize3.Checked) return 3;
            if(mi_PuzzleSize4.Checked) return 4;
            if(mi_PuzzleSize5.Checked) return 5;
            if(mi_PuzzleSize6.Checked) return 6;
            if(mi_PuzzleSize7.Checked) return 7;
            if(mi_PuzzleSize8.Checked) return 8;
            if(mi_PuzzleSize9.Checked) return 9;
            return 3;
        }
        void SetSize(int n)
        {
            mi_PuzzleSize2.Checked=(n==2);
            mi_PuzzleSize3.Checked=(n==3);
            mi_PuzzleSize4.Checked=(n==4);
            mi_PuzzleSize5.Checked=(n==5);
            mi_PuzzleSize6.Checked = (n == 6);
            mi_PuzzleSize7.Checked = (n == 7);
            mi_PuzzleSize8.Checked = (n == 8);
            mi_PuzzleSize9.Checked = (n == 9);
        }

        Cube7D Cube;
        CubeObj CubeView;
        int TRate=500;
        NumericUpDown nudUndoFrameSkip;
        Label lblUndoFrameSkip;

        const int CLICK_MODE_2=0;
        const int CLICK_MODE_2_OPP=1;
        const int CLICK_MODE_3=2;

        int ClickMode=CLICK_MODE_2;
        int NClicks=0;
        int FaceClick=0;
        int FaceFrom=0;
        bool ClickQual=true;
        
        int TwistMask;     // pressed 1-5
        bool RotateCube;   // pressed Ctrl

        Keybindings Keybinds = new Keybindings();
        Form KeybindsSetup;
        KeybindsReference KeybindsRef;
        ToolStripMenuItem _refMenuItem;

        /// <summary>Tracks actions activated by currently held keys.
        /// Used for consumed-modifier calculation and KeyUp dispatch.</summary>
        Dictionary<Keys, Keybindings.IAction> _activeKeyActions = new Dictionary<Keys, Keybindings.IAction>();

        bool AltHighlight=false;
        int[] NColMask;  // -1: only unhighlight, 0: normal (Indeterminate), 1: only highlight
        bool MaskStickers = false;  // true: exclude unchecked stickers from mesh (unclickable), false: only dim them
        CheckState HighlightByColorsState = CheckState.Unchecked;
        int[] FaceMask;
        Dictionary<int,CheckBox> m_orbChipMap; // orbit key → chip checkbox
        int[] GripAxisMask = new int[8];  // index 1..7, -1=exclude, 0=neutral, 1=include
        Dictionary<int,int> m_orbitMaskCache; // reused by GetOrbitFilterMask
        Dictionary<int,CheckState> m_pendingOrbitChipStates; // persisted across chip rebuilds
        int[] GripLayerNum = new int[8];  // index 1..7, layer numbers 1..N

        int DiffLight=150;
        int SpecLight=100;
        int Transparency=64;

        int RecordingMacroStatus=REC_MACRO_NONE;
        int OldRecMacroStatus=REC_MACRO_NONE;
        const int REC_MACRO_NONE=0;
        const int REC_MACRO_STICKERS=1;
        const int REC_MACRO_CODE=2;
        const int REC_MACRO_APPLY=3;

        string SettingsFileName;

        int[] MacroStickers=new int[100];
        int LMacroStickers=0;
        int[] MStickers;
        int LMStickers;
        int MacroStart=0;
        bool MacroReverse=false;
        CMacro CurMacro;
        CMacroFile Macros;

        int[,] RevStack=new int[100,2];
        int LRevStack=0;
        bool qSolved=true;

        long m_TStart=0;
        bool m_TRun=false;

        System.Threading.Timer m_Timer;

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

            // Build a self-excluding KeyCombo from the OS modifier state
            KeyCombo combo = KeyCombo.FromKeyPress(keyCode);

            // Resolve with consumed-modifier awareness
            Keys consumedMods = GetConsumedModifiers();
            var action = Keybinds.GetActionWithFallback(
                keyCode.ToString(), combo.Ctrl, combo.Shift, combo.Alt, consumedMods);

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
        }

        // Helper method: check if any mask has non-gray value (black check or uncheck)
        private bool HasSelection(int[] mask, int startIndex, int endIndex) {
            for (int i = startIndex; i <= endIndex; i++) {
                if (mask[i] != 0) {
                    return true;
                }
            }
            return false;
        }

        private void ProcessHighLights()
        {
            // Check if any show cubies is not gray (black check or uncheck)
            bool hasNColSelection = (cb_HighlightByColors.CheckState != CheckState.Unchecked)
                                    && HasSelection(NColMask, 1, 7);

            // Check if any color is not gray (black check or uncheck)
            bool hasColorSelection = HasSelection(FaceMask, 1, 14);

            // Only apply NColMask filtering when Enable highlighting is checked or indeterminate
            int[] effectiveNColMask = (cb_HighlightByColors.CheckState != CheckState.Unchecked) ? NColMask : null;

            // Compute orbit chip mask — only when highlighting is on
            Dictionary<int,int> rawOrbitMask = (cb_HighlightByColors.CheckState != CheckState.Unchecked)
                ? GetOrbitFilterMask() : null;

            // Build effective orbit mask (Cube7D only handles exclusion: val < 0)
            //
            // Orbit chip semantics:
            //   Checked   (+1): show ONLY this orbit within its C-value group.
            //                    Also activates the C-value via NColMask if neutral.
            //   Unchecked (-1): exclude this orbit from display.
            //   Indeterminate:   no opinion — leave it to other filters.
            //
            // Implementation: since the orbit mask only supports exclusion (-1),
            // a "checked" chip is expanded into exclusions of all OTHER same-C-value
            // orbits that lack a checked chip of their own.
            //
            Dictionary<int,int> effectiveOrbitMask = null;
            if(rawOrbitMask != null && effectiveNColMask != null) {
                bool ncolCloned = false;
                int[] allOrbitKeys = null;

                foreach(KeyValuePair<int,int> kv in rawOrbitMask) {
                    int orbitKey = kv.Key;
                    int chipVal = kv.Value;
                    int c = Cube7D.GetStkNColsFromOrbitKey(orbitKey);

                    // C-values hidden by Show Cubies (-1) ignore all chip states
                    if(effectiveNColMask[c] == -1) continue;

                    if(chipVal == -1) {
                        // ── Unchecked chip → exclude this orbit ──
                        if(effectiveOrbitMask == null)
                            effectiveOrbitMask = new Dictionary<int,int>();
                        if(!effectiveOrbitMask.ContainsKey(orbitKey))
                            effectiveOrbitMask[orbitKey] = -1;
                    } else {
                        // ── Checked chip → activate C-value if neutral ──
                        if(effectiveNColMask[c] == 0) {
                            if(!ncolCloned) {
                                effectiveNColMask = (int[])effectiveNColMask.Clone();
                                ncolCloned = true;
                            }
                            effectiveNColMask[c] = 1;
                        }

                        // ── Exclude all other orbits of this C-value ──
                        if(allOrbitKeys == null)
                            allOrbitKeys = Cube.GetAllOrbitKeys();
                        foreach(int other in allOrbitKeys) {
                            if(Cube7D.GetStkNColsFromOrbitKey(other) != c) continue;
                            if(other == orbitKey) continue; // the checked one survives

                            // If this other orbit lacks a checked chip → exclude it
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
                // Black check: show whole cubies
                if (hasColorSelection || hasNColSelection)
                    Cube.FindStickersByMask(FaceMask, true, effectiveNColMask, effectiveOrbitMask);
                else
                    Cube.HighlightAll(effectiveNColMask, effectiveOrbitMask);
            } else if (cb_HighlightByColors.CheckState == CheckState.Indeterminate) {
                // Gray check: show only matching stickers
                if (hasColorSelection || hasNColSelection)
                    Cube.FindStickersByMask(FaceMask, false, effectiveNColMask, effectiveOrbitMask);
                else {
                    // All gray: all dark, unless grip axis filter is active
                    if (HasSelection(GripAxisMask, 1, 7)) {
                        // Let grip axis filter determine what is visible (like show cubies)
                        Cube.HighlightAll(null, effectiveOrbitMask);
                    } else {
                        Cube.HighLighted.SetAll(false);
                    }
                    Cube.HighLightGrip();
                    ApplyGripAxisFilters();
                    return;
                }
            } else {
                // Unchecked: show all cubies normally (ignore color/cubies filters)
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

				// Apply same transformation chain as keyboard grip:
				// 1. NormGripMask: negative value → reverse bits (same as Grip.OnKeyDown + NormGripMask)
				// 2. Axis inverted flag (X=2, V=5) → flip mask bits
				// 3. Orientation → if oriented axis is positive, reverse mask
				// 4. Use oriented absolute axis for stride
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
	
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		void RefreshView(){
			dxControl2.ParkCamera(false);
			dxControl2.SetSceneChanged();
		}

     	void mkPickObject(EAction act,ref Vector3 pt,ref Vector3 vec,double ang){
            m_runUndo=false;
            int stk=CubeView.FindSticker(ref pt,ref vec);
            if(stk<0) {
                NClicks=0;
                ClickQual=true;
                // Clear Twist3c state when clicking empty area
                Cube.partialTwist3c.Reset();
                switch(RecordingMacroStatus) {
                    case REC_MACRO_STICKERS: RecordingMacroStatus=OldRecMacroStatus=REC_MACRO_NONE; break;
                    case REC_MACRO_APPLY: RecordingMacroStatus=OldRecMacroStatus; break;
                }
                RedrawClickStatus();
                if(AltHighlight){
                    AltHighlight=false;
                    ProcessHighLights();
                    Redraw();
                }
                return;
            }
            int ncl=NClicks;
            int fclick=FaceClick;
            int ffrom=FaceFrom;

            if(ncl==0) {   // first click
                switch(act) {
                    case EAction.ActionCtrlClick: {
                            bool rr=Cube.RotateCubeBySticker(stk);
                            if(rr) {
                                Redraw();
                                return;
                            }
                            // Clear Twist3c state when mouse clicking starts (before validity check)
                            Cube.partialTwist3c.Reset();
                            FaceClick=Cube.GetFirstSticker(stk,ClickMode,out FaceFrom);
                            if(FaceClick!=0) {
                                NClicks=ClickMode==CLICK_MODE_3 ? 1 : 2;
                                ClickQual=true;
                                RotateCube=true;
                            } else {
                                NClicks=0;
                                ClickQual=false;
                            }
                            RedrawClickStatus();
                            Redraw();  // Ensure UI updates for invalid clicks
                            return;
                        }
                    case EAction.ActionCtrlRightClick: {
                            bool rr=Cube.RotateCubeByStickerInverse(stk);
                            if(rr) {
                                Redraw();
                                return;
                            }
                            return;
                        }
                    case EAction.ActionShiftClick: {
                            Cube.FindOtherStickers(stk);
                            AltHighlight=true;
                            Redraw();
                            return;
                        }
                    case EAction.ActionCtrlShiftClick: {
                            Cube.FindAdjStickers(stk);
                            AltHighlight=true;
                            Redraw();
                            return;
                        }
                    default: {
                            ClickQual=true;
                            if(RecordingMacroStatus==REC_MACRO_STICKERS || RecordingMacroStatus==REC_MACRO_APPLY) {
                                if(LMacroStickers==MacroStickers.Length) {
                                    int[] mm=new int[2*LMacroStickers];
                                    Buffer.BlockCopy(MacroStickers,0,mm,0,LMacroStickers*4);
                                    MacroStickers=mm;
                                }
                                MacroStickers[LMacroStickers++]=Cube.StkMap[stk];
                                if(RecordingMacroStatus==REC_MACRO_STICKERS) {
                                    bool qx=Cube.CheckStickerSet(MacroStickers,LMacroStickers);
                                    if(qx) {
                                        LMStickers=LMacroStickers;
                                        MStickers=new int[LMStickers];
                                        Buffer.BlockCopy(MacroStickers,0,MStickers,0,LMStickers*4);

                                        RecordingMacroStatus=REC_MACRO_CODE;
                                        MacroStart=Cube.LPtr;
                                    }
                                } else {
                                    if(LMacroStickers==CurMacro.NStickers) {
                                        int[] cmap=Cube.CmpStickerSet(CurMacro.Stickers,MacroStickers,LMacroStickers);
                                        if(cmap==null) {
                                            ClickQual=false;
                                            CurMacro=null;
                                        } else {
                                            Cube.ApplyMacro(cmap,CurMacro.Code,CurMacro.LMacro,MacroReverse);
                                            ProcessHighLights();
                                            TestBuild();
                                            Redraw();
                                        }
                                        RecordingMacroStatus=OldRecMacroStatus;
                                    }
                                }
                            } else {
                                TwistMask=0;
                                int n=GetSize();
                                for(int i=0;i<n;i++) {
                                    if((S3DirectX.GetAsyncKeyState(0x31+i) & 0x8000) != 0) TwistMask|=(1<<i);
                                }
                                if(TwistMask==0) TwistMask=1;

                                FaceClick=Cube.GetFirstSticker(stk,ClickMode,out FaceFrom);
                                if(FaceClick!=0) {
                                    NClicks=ClickMode==CLICK_MODE_3 ? 1 : 2;
                                    RotateCube=false;
                                    // Clear Twist3c state when mouse clicking starts
                                    Cube.partialTwist3c.Reset();
                                } else {
                                    NClicks=0;
                                    ClickQual=false;
                                    // Clear Twist3c state even for invalid clicks
                                    Cube.partialTwist3c.Reset();
                                }
                            }
                            RedrawClickStatus();
                            Redraw();  // Ensure UI updates for invalid clicks
                            return;
                        }
                }
            } else if(ncl==1){  // only 3-mode
                FaceFrom=Cube.GetSecondSticker(stk,fclick);
                if(FaceFrom==0) {
                    ClickQual=false;
                    NClicks=0;
                } else {
                    ClickQual=true;
                    NClicks=2;
                }
                RedrawClickStatus();
            }else{  // last click
                bool rr=false;
                int ff=Cube.GetSecondSticker(stk,fclick);
                if(ff!=0) {
                    if(RotateCube) {
                        rr=Cube.RotateCubeByStickers(fclick,ffrom,ff);
                    } else {
                        rr=Cube.Twist(fclick,ffrom,ff,TwistMask);
                        ProcessHighLights();
                        TestBuild();
                    }
                }
                if(!rr) {
                    ClickQual=false;
                    NClicks=0;
                    RedrawClickStatus();
                    return;
                }
                ClickQual=true;
                FaceFrom=ff;
                NClicks=0;
                RedrawClickStatus();
                Redraw();
            }
		}

        void RedrawClickStatus() {
            // Check if Twist3c is in progress
            int twist3cStep = Cube?.partialTwist3c?.step ?? 0;

            if (twist3cStep > 0) {
                // Twist3c is active, show its progress in blue
                pb_ClickStatus.ForeColor = Color.Blue;
                pb_ClickStatus.Value = twist3cStep * 33 + 10; // 0->10, 1->43, 2->76, 3->109 (but will reset before 3)
            } else {
                // Normal click status (green/red based on quality)
                pb_ClickStatus.ForeColor = ClickQual ? Color.Green : Color.Red;
                pb_ClickStatus.Value = NClicks * 40 + 10;
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
            if(Cube==null) return;
            long s=m_TRun ? (DateTime.Now.Ticks-m_TStart) : Cube.CTime;
            s/=10000; // ms
            int tms=(int)(s%1000); s/=1000;
            int ts=(int)(s%60); s/=60;
            int tm=(int)(s%60); s/=60;
            string m=String.Format("   Time: {0}:{1:D2}:{2:D2}.{3:D3}",s,tm,ts,tms);
            m_lblCTime.Text=m;
        }

        void Redraw() {
            ShowRevStack();
            CubeView.Dispose();  // colors changed
            dxControl2.SetSceneChanged();
        }

        void TestBuild() {
            if(qSolved) return;
            if(Cube.LShuffle == 0) return;
            if(!m_TRun) {
                m_TStart=DateTime.Now.Ticks-Cube.CTime;
                m_TRun=true;
            }
            if(Cube.CheckCube()) {
                m_TRun=false;
                Cube.CTime=DateTime.Now.Ticks-m_TStart;
                RedrawClickStatus();
                MessageBox.Show(string.Format("You have solved {0}^{1} cube scrambled by {2} twists.\r\nCongratulations!",Cube.N,Cube.D,Cube.LShuffle));
                qSolved=true;
            }
        }

        /*********** load/save scene ************/
        void NewScene(){ NewScene(true); }
		void NewScene(bool rebuildOrbitChips){
			dxControl2.ClearMeshes();
            CubeView=null;
            GC.Collect();
            m_pendingOrbitChipStates.Clear();
            Cube=new Cube7D();
            Cube.Init(GetSize(),GetDim());
            qSolved=true;

            if(rebuildOrbitChips) RebuildOrbitChips(false);

            if(Macros==null || !Macros.CheckSize(GetDim(),GetSize())) {
                Macros=new CMacroFile(GetDim(),GetSize());
                InitMacroList();
            }
            NClicks=0; ClickQual=true;
            LRevStack=0;
            RecordingMacroStatus=OldRecMacroStatus=REC_MACRO_NONE;

            m_TRun=false;
            ShowCube();
            dxControl2.ParkCamera(true);
        }
#if false
        void LoadScene(){
			OpenFileDialog sf=new OpenFileDialog();
			sf.DefaultExt=".scn";
			sf.Filter="Сцена (*.scn)|*.scn";
			sf.InitialDirectory=Application.StartupPath+"\\scenes";
			if(sf.ShowDialog()==DialogResult.OK){
				NewScene();
				LoadScene(sf.FileName);
				sceneName=System.IO.Path.GetFileNameWithoutExtension(sf.FileName);
				foreach(MeshObj M in m_objList){
					dxControl2.AddMesh(M);
				}
			}
			dxControl2.ParkCamera(true);
		}

		void SaveScene(){
			SaveFileDialog sf=new SaveFileDialog();
			sf.DefaultExt=".scn";
			sf.Filter="Сцена (*.scn)|*.scn";
			sf.InitialDirectory=Application.StartupPath+"\\scenes";
			if(sceneName!=null){
				sf.FileName=sceneName;
				sf.OverwritePrompt=false;
			}
			if(sf.ShowDialog()==DialogResult.OK){
				SaveScene(sf.FileName);
				sceneName=System.IO.Path.GetFileNameWithoutExtension(sf.FileName);
			}

		}
#endif
        
        void ShowCube() {
            if(CubeView==null) {
                CubeView=new CubeObj();
                dxControl2.AddMesh(CubeView);
            }

            byte[] col;
            float[][] coord;
            int[] map;
            BitArray hmask;

            int nstk=Cube.GetStickers(out col,out map,out coord,out hmask);
            CubeView.SetCoords(col,map,coord,hmask,NColMask,nstk);
            CubeView.MaskStickers = MaskStickers;
            UpdateGripAxisNUDs();
            ProcessHighLights();
            dxControl2.SetSceneChanged();
            ShowRevStack();
            RedrawClickStatus();
            //            dxControl2.Invalidate();
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

        private void mi_Puzzle4D_Click(object sender,EventArgs e) {
            SetDim(4);
            NewScene();
        }

        private void mi_Puzzle5D_Click(object sender,EventArgs e) {
            SetDim(5);
            NewScene();
        }

        private void mi_Puzzle6D_Click(object sender,EventArgs e) {
            SetDim(6);
            NewScene();
        }

        private void mi_Puzzle7D_Click(object sender,EventArgs e) {
            SetDim(7);
            NewScene();
        }

        private void mi_PuzzleSize2_Click(object sender, EventArgs e)
        {
            SetSize(2);
            NewScene();
        }
        private void mi_PuzzleSize3_Click(object sender,EventArgs e) {
            SetSize(3);
            NewScene();
        }

        private void mi_PuzzleSize4_Click(object sender,EventArgs e) {
            SetSize(4);
            NewScene();
        }

        private void mi_PuzzleSize5_Click(object sender,EventArgs e) {
            SetSize(5);
            NewScene();
        }

        private void mi_PuzzleSize6_Click(object sender, EventArgs e)
        {
            SetSize(6);
            NewScene();
        }

        private void mi_PuzzleSize7_Click(object sender, EventArgs e)
        {
            SetSize(7);
            NewScene();
        }

        private void mi_PuzzleSize8_Click(object sender, EventArgs e)
        {
            SetSize(8);
            NewScene();
        }

        private void mi_PuzzleSize9_Click(object sender, EventArgs e)
        {
            SetSize(9);
            NewScene();
        }
        
        

        private void mi_Reset_Click(object sender,EventArgs e) {
            NewScene(false);
        }

        private void mi_Undo_Click(object sender,EventArgs e) {
            bool r=Cube.Undo();
            if(r) {
                if(AltHighlight) Redraw();
                else { ProcessHighLights(); Redraw(); }
            }
        }

        private void mi_Redo_Click(object sender,EventArgs e) {
            bool r=Cube.Redo();
            if(r) {
                if(AltHighlight) Redraw();
                else { ProcessHighLights(); Redraw(); }
            }
        }

        private void Form1_KeyPress(object sender,KeyPressEventArgs e) {

        }

        private void mi_FullScramble_Click(object sender,EventArgs e) {
            Scramble(-1);
        }

        private void mi_Scramble1_Click(object sender,EventArgs e) {
            Scramble(1);
        }
        private void mi_Scramble2_Click(object sender,EventArgs e) {
            Scramble(2);
        }
        private void mi_Scramble3_Click(object sender,EventArgs e) {
            Scramble(3);
        }
        private void mi_Scramble4_Click(object sender,EventArgs e) {
            Scramble(4);
        }
        private void mi_Scramble5_Click(object sender,EventArgs e) {
            Scramble(5);
        }
        void Scramble(int N) {
            if(Cube!=null) Cube.partialTwist3c.Reset();
            dxControl2.ClearMeshes();
            CubeView=null;
            GC.Collect();
            Cube.Scramble(N);
            NClicks=0; ClickQual=true;
            RecordingMacroStatus=OldRecMacroStatus=REC_MACRO_NONE;
            qSolved=false;
            m_TRun=false;
            ShowCube();
        }

        bool m_runUndo=false;
        private void mi_FullUndo_Click(object sender,EventArgs e) {
            m_runUndo=true;
            int frameSkip=Math.Max(1,(int)nudUndoFrameSkip.Value);
            int frameCount=0;
            while(Cube.Undo()) {
                bool needHL = cb_HighlightByColors.CheckState != CheckState.Unchecked
                    && (HasSelection(FaceMask, 1, 14)
                        || HasSelection(NColMask, 1, 7)
                        || GetOrbitFilterMask() != null
                        || HasSelection(GripAxisMask, 1, 7)
                        || (Cube != null && Cube.Gripped[0] != -1));
                frameCount++;
                if(frameCount%frameSkip==0) {
                    if(needHL) ProcessHighLights();
                    Redraw();
                    dxControl2.Scene.Render3DEnvironment();
                    Thread.Sleep(TRate);
                }
                Application.DoEvents();
                if(!m_runUndo) break;
            }
            // Catch-up render if the last iteration was skipped
            if(frameCount%frameSkip!=0) {
                ShowRevStack();
                ProcessHighLights();
                CubeView.Dispose();
                dxControl2.SetSceneChanged();
                dxControl2.Scene.Render3DEnvironment();
            }
            ProcessHighLights();
        }

        private void mi_FullRedo_Click(object sender,EventArgs e) {
            m_runUndo=true;
            int frameSkip=Math.Max(1,(int)nudUndoFrameSkip.Value);
            int frameCount=0;
            while(Cube.Redo()) {
                bool needHL = cb_HighlightByColors.CheckState != CheckState.Unchecked
                    && (HasSelection(FaceMask, 1, 14)
                        || HasSelection(NColMask, 1, 7)
                        || GetOrbitFilterMask() != null
                        || HasSelection(GripAxisMask, 1, 7)
                        || (Cube != null && Cube.Gripped[0] != -1));
                frameCount++;
                if(frameCount%frameSkip==0) {
                    if(needHL) ProcessHighLights();
                    Redraw();
                    dxControl2.Scene.Render3DEnvironment();
                    Thread.Sleep(TRate);
                }
                Application.DoEvents();
                if(!m_runUndo) break;
            }
            // Catch-up render if the last iteration was skipped
            if(frameCount%frameSkip!=0) {
                ShowRevStack();
                ProcessHighLights();
                CubeView.Dispose();
                dxControl2.SetSceneChanged();
                dxControl2.Scene.Render3DEnvironment();
            }
            ProcessHighLights();
        }

        private void stopToolStripMenuItem_Click(object sender,EventArgs e) {
            m_runUndo=false;
        }

        string m_FileName=null;

        private void mi_Open_Click(object sender,EventArgs e) {
            OpenFileDialog sf=new OpenFileDialog();
            sf.DefaultExt=".log";
            sf.Filter="MC7D Log file (*.log)|*.log";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                if(Cube!=null) Cube.partialTwist3c.Reset();
                // Exit macro sticker selection when loading a different puzzle
                RecordingMacroStatus=OldRecMacroStatus=REC_MACRO_NONE;
                m_pendingOrbitChipStates.Clear();
                m_FileName=sf.FileName;
                Text=m_FileName+" - MC7D";
                Cube.Load(m_FileName);
                RebuildOrbitChips(false);
                ShowCube();
                dxControl2.ParkCamera(true);
                NClicks=0; ClickQual=true;
                LRevStack=0;
                qSolved=Cube.CheckCube();
                SetDim(Cube.D);
                SetSize(Cube.N);

                // Clear macros if they don't match the loaded puzzle
                if(Macros==null || !Macros.CheckSize(GetDim(),GetSize())) {
                    Macros=new CMacroFile(GetDim(),GetSize());
                    InitMacroList();
                }
            }
        }

        private void mi_Save_Click(object sender,EventArgs e) {
            if(m_FileName==null) mi_SaveAs_Click(sender,e);
            if(m_TRun) Cube.CTime=DateTime.Now.Ticks-m_TStart;
            Cube.Save(m_FileName);
        }

        private void mi_SaveAs_Click(object sender,EventArgs e) {
            SaveFileDialog sf=new SaveFileDialog();
            sf.RestoreDirectory=true;
            sf.DefaultExt=".log";
            sf.Filter="MC7D Log file (*.log)|*.log";
            if(sf.ShowDialog()==DialogResult.OK) {
                m_FileName=sf.FileName;
                Text=m_FileName+" - MC7D";
                if(m_TRun) Cube.CTime=DateTime.Now.Ticks-m_TStart;
                Cube.Save(m_FileName);
            }
        }

        private void mi_SaveStripMarkersAs_Click(object sender,EventArgs e) {
            SaveFileDialog sf=new SaveFileDialog();
            sf.RestoreDirectory=true;
            sf.DefaultExt=".log";
            sf.Filter="MC7D Log file (*.log)|*.log";
            if(sf.ShowDialog()==DialogResult.OK) {
                if(m_TRun) Cube.CTime=DateTime.Now.Ticks-m_TStart;
                Cube.SaveStripMarkers(sf.FileName);
            }
        }

        bool SettingsSaved=false;
        private void mi_Exit_Click(object sender,EventArgs e) {
            if(!SettingsSaved) {
                SaveSettings("MC7D_settings.txt");
                SaveKeybinds("MC7D_keybinds.txt");
                SettingsSaved =true;
            }
            Application.Exit();
        }
        private void Form1_FormClosing(object sender,FormClosingEventArgs e) {
            if(!SettingsSaved) {
                SaveSettings("MC7D_settings.txt");
                SaveKeybinds("MC7D_keybinds.txt");
                SettingsSaved =true;
            }
            Keybindings.ExecuteMacroById = null;
        }

        bool m_setgeom=false;
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

            // Restore grip axis filters
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

        void SaveSettings(string fn) {
            try {
                StreamWriter sw=new StreamWriter(fn);
                sw.NewLine="\r\n";
                sw.WriteLine("MC7D Settings");
                sw.WriteLine("Size {0} {1}",GetDim(),GetSize());
                sw.WriteLine("BlockSize {0:F2}",Cube7D.BSize);
                sw.WriteLine("StickerSize {0:F2}",Cube7D.SSize);
                sw.WriteLine("FaceSeparation {0:F2}",Cube7D.FSep);
                sw.WriteLine("Perspective {0:F2}",Cube7D.FExt);
                string ln="Colors";
                for(int i=1;i<=14;i++) ln+=" "+CubeObj.Colors[i];
                sw.WriteLine(ln);
                sw.WriteLine("ClickMode {0}",ClickMode+1);
                sw.WriteLine("HighlightMode {0}", (int)cb_HighlightByColors.CheckState);
                sw.WriteLine("MaskStickers {0}",MaskStickers ? "T" : "F");
                sw.WriteLine("DiffLight {0}",DiffLight);
                sw.WriteLine("SpecLight {0}",SpecLight);
                sw.WriteLine("Transparency {0}",CubeObj.Transparency);
                sw.WriteLine("QuickMacro {0}",m_cbQuickMacro.Checked ? "T" : "F");
                if(m_FileName!=null) sw.WriteLine("FileName "+m_FileName);
                sw.Close();
            } catch { }
        }
        void LoadSettings(string fn) {
            if(File.Exists(fn)) {
                try {
                    StreamReader sr=new StreamReader(fn);
                    string ln=sr.ReadLine();
                    if(ln!="MC7D Settings") return;
                    for(;;) {
                        ln=sr.ReadLine();
                        if(ln==null) break;
                        string[] pars=ln.Split(' ');
                        switch(pars[0]) {
                            case "Size":
                                SetDim(int.Parse(pars[1]));
                                SetSize(int.Parse(pars[2]));
                                break;
                            case "BlockSize":
                                Cube7D.BSize=double.Parse(pars[1]); break;
                            case "StickerSize":
                                Cube7D.SSize=double.Parse(pars[1]); break;
                            case "FaceSeparation":
                                Cube7D.FSep=double.Parse(pars[1]); break;
                            case "Perspective":
                                Cube7D.FExt=double.Parse(pars[1]); break;
                            case "Colors":
                                for(int i=1;i<=14 && i<pars.Length;i++) CubeObj.Colors[i]=uint.Parse(pars[i]);
                                break;
                            case "ClickMode":
                                ClickMode=int.Parse(pars[1])-1;
                                break;
                            case "HighlightMode":
                                int highlightMode;
                                if(int.TryParse(pars[1], out highlightMode)
                                    && highlightMode >= (int)CheckState.Unchecked
                                    && highlightMode <= (int)CheckState.Indeterminate)
                                    HighlightByColorsState = (CheckState)highlightMode;
                                break;
                            case "FileName":
                                m_FileName=ln.Substring(9);
                                break;
                            case "DiffLight":
                                DiffLight=int.Parse(pars[1]);
                                break;
                            case "SpecLight":
                                SpecLight=int.Parse(pars[1]);
                                break;
                            case "Transparency":
                                CubeObj.Transparency=int.Parse(pars[1]);
                                break;
                            case "MaskStickers":
                                MaskStickers = (pars[1][0]=='T');
                                break;
                            case "QuickMacro":
                                m_cbQuickMacro.Checked=(pars[1][0]=='T');
                                break;
                        }
                    }
                    sr.Close();
                } catch { }
            }

            InitGeomSettings();
            SetLight();
            if(m_FileName==null) NewScene();
            else {
                Text=m_FileName+" - MC7D";
                m_pendingOrbitChipStates.Clear();
                Cube=new Cube7D();
                Cube.Load(m_FileName);
                RebuildOrbitChips(false);
                ShowCube();
                SetDim(Cube.D); SetSize(Cube.N);
                dxControl2.ParkCamera(true);
                NClicks=0; ClickQual=true;
                LRevStack=0;
                qSolved=Cube.CheckCube();


            }
        }

        void SaveKeybinds(string fn)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fn);
                sw.NewLine = "\r\n";
                sw.WriteLine("MC7D Keybinds");
                sw.Write(Keybinds.Serialize());
                sw.WriteLine();
                sw.Close();
            }
            catch { }
        }

        void LoadKeybinds(string fn)
        {
            StreamReader sr = null;
            if (File.Exists(fn))
            {
                try
                {
                    sr = new StreamReader(fn);
                    string ln = sr.ReadLine();
                    if (ln != "MC7D Keybinds") return;

                    int idx = 1;

                    for (; ; )
                    {
                        ln = sr.ReadLine();
                        idx++;
                        if (ln == null) break;

                        Keybinds.LoadKeybindSet(ln, idx);
                    }
                }
                catch {
                }
            }
            if (sr != null)
            {
                sr.Close();
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
        }

        void KeybindMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Keybinds.switchKeybindSet(item.Text);
        }

        void SetLight() {
            foreach(CameraLight L in dxControl2.Scene.Lights) {
                L.Specular=SpecLight;
                L.Diffuse=DiffLight;
            }
            dxControl2.SetSceneChanged();
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
                // Need to rebuild the mesh to apply/remove sticker exclusion
                if (CubeView != null) {
                    CubeView.MaskStickers = MaskStickers;
                    CubeView.Dispose();  // clear meshes so they rebuild with new mask mode
                }
                Redraw();
            }
        }

        private void btn_ResetHighlightSelection_Click(object sender, EventArgs e) {
            // Reset all color filters to gray (indeterminate)
            for (int i = 1; i <= 14; i++) {
                FaceMask[i] = 0;
            }

            // Reset all show cubies filters to gray (indeterminate)
            for (int i = 1; i <= 7; i++) {
                NColMask[i] = 0;
            }

            // Reset grip axis filters to gray (indeterminate)
            for (int i = 1; i <= 7; i++) {
                GripAxisMask[i] = 0;
                GripLayerNum[i] = 1;
            }

            // Update UI checkboxes to indeterminate state
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

            // Reset orbit filters
            ResetOrbitChips();

            m_setgeom = false;

            // Refresh highlighting
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
            int panelW=m_pnlOrbitFilters.ClientSize.Width;
            System.Drawing.Font font7=new System.Drawing.Font("Microsoft Sans Serif",8f);
            m_setgeom = true;

            for(int c=1;c<=7;c++) {
                if(groups[c].Count==0) continue;

                // C-value label
                var lbl=new Label();
                lbl.Text="C"+c+":";
                lbl.Location=new System.Drawing.Point(3,y);
                lbl.Font=font7;
                lbl.AutoSize=true;
                lbl.TextAlign=ContentAlignment.MiddleLeft;
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
                    chip.AutoSize=true; // natural height adapts to DPI
                    chip.Location=new System.Drawing.Point(x,y);
                    m_pnlOrbitFilters.Controls.Add(chip);
                    m_orbChipMap[orbitKey]=chip;

                    if(rowH < chip.Height+gap) rowH=chip.Height+gap;

                    // Measure actual width for positioning
                    int chipW=Math.Max(chip.Width,30);
                    // If chip won't fit, wrap to next row
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

        // Build orbit filter mask from chip states:  -1=exclude, 0=neutral, 1=include
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

        // Reset all orbit chips to neutral
        void ResetOrbitChips() {
            m_pendingOrbitChipStates.Clear();
            if(m_orbChipMap==null) return;
            foreach(var chip in m_orbChipMap.Values) {
                chip.CheckState=CheckState.Indeterminate;
            }
        }

        double[] m_macroVec;
        int[] m_macroOrient;
        string m_macroName;

        private void mi_StartRecordig_Click(object sender,EventArgs e) {
            if(RecordingMacroStatus!=REC_MACRO_CODE) {
//                MessageBox.Show("Click Reference Stickers");
                RecordingMacroStatus=REC_MACRO_STICKERS;
                m_macroVec=GetMatrix();
                m_macroOrient=(int[])Cube.Orient.Clone();
                LMacroStickers=0;
            } else {
                if(Cube.LPtr<=MacroStart) {
                    MessageBox.Show("Negative Macro Length");
                } else {
                    TextDialog edt=new TextDialog("Enter Macro Name");
                    DialogResult res=edt.ShowDialog(this);
                    if(res==DialogResult.OK) {
                        CMacro m=new CMacro(edt.Value,LMStickers,MStickers,Cube.LPtr-MacroStart,Cube.Seq,MacroStart);
                        m.Vectors=m_macroVec;
                        m.Orient=m_macroOrient;
                        Macros.AddMacro(m);
                        if(m_lbMacros.FindStringExact(m.Name)==ListBox.NoMatches) {
                            m_lbMacros.Items.Add(m.Name);
                        }
                    }
                }
                RecordingMacroStatus=REC_MACRO_NONE;
            }
            RedrawClickStatus();
        }

        void InitMacroList(){
            string[] str=Macros.GetNamesList();
            m_lbMacros.Items.Clear();
            foreach(string s in str) m_lbMacros.Items.Add(s);
        }

        private void m_lbMacros_MouseDown(object sender,MouseEventArgs e) {
            int index=m_lbMacros.IndexFromPoint(e.Location);
            if(index!=ListBox.NoMatches) {
                m_lbMacros.SelectedIndex=index;
            }
            m_macroName=(string)m_lbMacros.SelectedItem;
            if(m_macroName==null) return;
            if(m_RunByClick.Checked) {
                if(e.Button==MouseButtons.Left) {
                    appMacro(false);
                } else if(e.Button==MouseButtons.Right) {
                    appMacro(true);
                }
                return;
            }

            ContextMenu c=new ContextMenu();
            c.MenuItems.Add(new MenuItem("Apply",ApplyMacro));
            c.MenuItems.Add(new MenuItem("Reverse",ReverseMacro));
            c.MenuItems.Add(new MenuItem("Delete",DeleteMacro));
            c.MenuItems.Add(new MenuItem("Rename",RenameMacro));
            c.Show(m_lbMacros,e.Location);
        }

        private void m_lbMacros_MouseDoubleClick(object sender,MouseEventArgs e) {
            // Note: This method is currently not effectively used due to MouseDown handling
            if(m_RunByClick.Checked) return;
            m_macroName=(string)m_lbMacros.SelectedItem;
            if(m_macroName==null) return;
            appMacro(false);
        }

        private void ApplyMacro(object sender,EventArgs e) {
            appMacro(false);
        }
        void appMacro(bool qrev){
            if(RecordingMacroStatus==REC_MACRO_STICKERS || RecordingMacroStatus==REC_MACRO_APPLY) return;
            CurMacro=Macros.GetMacro(m_macroName);
            if(CurMacro==null) {
                ClickQual=false;
                return;
            }
            if(m_cbQuickMacro.Checked && CurMacro.Vectors!=null) {
                int[] cmap=GetFastMacroRef(CurMacro.Vectors,CurMacro.Orient);
                Cube.ApplyMacro(cmap,CurMacro.Code,CurMacro.LMacro,qrev);
                ProcessHighLights();
                TestBuild();
                Redraw();
                return;
            }
            OldRecMacroStatus=RecordingMacroStatus;
            RecordingMacroStatus=REC_MACRO_APPLY;
            LMacroStickers=0;
            MacroReverse=qrev;
            RedrawClickStatus();
        }
        private void ReverseMacro(object sender,EventArgs e) {
            appMacro(true);
        }

        // Execute macro by its 1-based index in the listbox (alphabetical order)
        void ExecuteMacroByIdCmd(int id, bool reverse) {
            if(RecordingMacroStatus==REC_MACRO_STICKERS || RecordingMacroStatus==REC_MACRO_APPLY) return;
            int index = id - 1;
            if (index < 0 || index >= m_lbMacros.Items.Count) return;
            m_macroName = (string)m_lbMacros.Items[index];
            appMacro(reverse);
        }

        private void DeleteMacro(object sender,EventArgs e) {
            Macros.Delete(m_macroName);
            m_lbMacros.Items.Remove(m_macroName);
        }
        private void RenameMacro(object sender,EventArgs e) {
            TextDialog edt=new TextDialog("Enter New Macro Name");
            edt.Value=m_macroName;
            DialogResult res=edt.ShowDialog(this);
            if(res==DialogResult.OK) {
                Macros.Rename(m_macroName,edt.Value);
                InitMacroList();
            }
        }

        private void saveMacroFileToolStripMenuItem_Click(object sender,EventArgs e) {
            if(Macros.FileName==null) saveMacroFileAsToolStripMenuItem_Click(sender,e);
            Macros.Save();
        }

        private void loadMacroFileToolStripMenuItem_Click(object sender,EventArgs e) {
            OpenFileDialog sf=new OpenFileDialog();
            sf.DefaultExt=".dat";
            sf.Filter="MC7D Macro file (*.dat)|*.dat";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                CMacroFile mf=new CMacroFile(sf.FileName);
                if(!mf.CheckSize(GetDim(),GetSize())) {
                    MessageBox.Show("Wrong cube size");
                    return;
                }
                Macros=mf;
                InitMacroList();
            }
        }

        private void saveMacroFileAsToolStripMenuItem_Click(object sender,EventArgs e) {
            SaveFileDialog sf=new SaveFileDialog();
            sf.DefaultExt=".dat";
            sf.Filter="MC7D Macro file (*.dat)|*.dat";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                Macros.SaveAs(sf.FileName);
            }
        }

        double[] GetMatrix() {
            Camera cam=dxControl2.Scene.Camera;
            Matrix M=Microsoft.DirectX.Matrix.LookAtLH(
				cam.CameraPosition,
				cam.PointToLookAt,
				cam.UpVector);
            double[] res=new double[] { M.M11,M.M12,M.M13,M.M21,M.M22,M.M23,M.M31,M.M32,M.M33 };
            return res;
        }
        double VPr(double []v,int ia,double []w,int ib){
            return v[3*ia]*w[3*ib]+v[3*ia+1]*w[3*ib+1]+v[3*ia+2]*w[3*ib+2];
        }
        int []GetBestCorr(double []dest){
            int[] res=new int[6];
            double[] dat=GetMatrix();
            double dbest=0,dbest1=0;
            int D=Cube.D;
            for(int k=0;k<6;k++) {
                int a0=k/2,a1=(a0+k%2+1)%3,a2=3-a0-a1;
                double s0=VPr(dest,0,dat,a0);
                double s1=VPr(dest,1,dat,a1);
                double s2=VPr(dest,2,dat,a2);
                double s=s0*s0+s1*s1+s2*s2;
                if(s>dbest) {
                    dbest=s;
                    res[0]=(a0+1)*(s0<0 ? -1 : 1);
                    res[1]=(a1+1)*(s1<0 ? -1 : 1);
                    res[2]=(a2+1)*(s2<0 ? -1 : 1);
                }
                if(D>4) {
                    double ss=s0*s0;
                    if(D>5) ss+=s1*s1;
                    if(D>6) ss+=s2*s2;
                    if(D==7 || (a2==2 && (D==6 || a1==1))) {
                        if(ss>dbest1) {
                            dbest1=ss;
                            res[3]=(a0+4)*(s0<0 ? -1 : 1);
                            res[4]=(a1+4)*(s1<0 ? -1 : 1);
                            res[5]=(a2+4)*(s2<0 ? -1 : 1);
                        }
                    }
                }
            }
            return res;
        }
        int[] GetFastMacroRef(double[] mvec,int []morient) {
            int []C=GetBestCorr(mvec);
            int []res=new int[8];
            res[1]=Cube.Orient[0];
            for(int i=0;i<7;i++) {
                int p=i==0 ? Cube.Orient[0] : Cube.Orient[Math.Abs(C[i-1])]*(C[i-1]<0 ? -1 : 1);
                int q=morient[i];
                if(q<0) { q=-q; p=-p; }
                res[q]=p;
            }
            return res;
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

        private void btnTogglePanel_Click(object sender, EventArgs e) {
            SetPanelCollapsed(!m_panelCollapsed);
        }
        private void mi_ShowSidePanel_Click(object sender, EventArgs e) {
            SetPanelCollapsed(!m_panelCollapsed);
        }
        private void SetPanelCollapsed(bool collapsed) {
            m_panelCollapsed = collapsed;
            panel1.Visible = !collapsed;
            btnTogglePanel.Text = collapsed ? "‹" : "›";
            mi_ShowSidePanel.Checked = !collapsed;

            // Update button position
            UpdateToggleButtonPosition();

            // Force layout refresh to ensure 3D area resizes correctly
            panel2.PerformLayout();
            this.PerformLayout();
        }

        private void Form1_Load(object sender, EventArgs e) {
            // Initialize button position on form load
            UpdateToggleButtonPosition();

            // Rebuild orbit chips after form is fully initialized (fixes AutoSize layout on first load)
            if(m_orbChipMap != null && m_orbChipMap.Count > 0) RebuildOrbitChips();
        }

        private void Form1_Resize(object sender, EventArgs e) {
            // Update button position when window resizes
            UpdateToggleButtonPosition();
        }

        /// <summary>Focus the DirectX control whenever this form is activated,
        /// so keybindings work immediately after modal dialogs close or
        /// after the keyboard reference window is clicked.</summary>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
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

        // Intercept all WM_KEYDOWN / WM_KEYUP at the message-queue level so the
        // keyboard reference refreshes even when focus is on a child control
        // (Tab/arrows get consumed for focus navigation and never reach KeyUpEvt).
        class KeybindsRefreshFilter : IMessageFilter
        {
            Form1 _form;
            public KeybindsRefreshFilter(Form1 form) { _form = form; }
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == 0x100 || m.Msg == 0x101) // WM_KEYDOWN or WM_KEYUP
                {
                    var r = _form.KeybindsRef;
                    if (r != null && !r.IsDisposed)
                        r.ConsumedModifiers = _form.GetConsumedModifiers();
                }
                return false;
            }
        }
    }
}
