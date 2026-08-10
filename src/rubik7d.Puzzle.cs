using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class Form1
    {
        bool m_runUndo=false;
        bool qSolved=true;

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
    }
}
