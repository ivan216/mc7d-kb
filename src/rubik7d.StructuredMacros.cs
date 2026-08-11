using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class Form1
    {
        List<CStructuredMacro> StructuredMacros = new List<CStructuredMacro>();
        string StructuredMacroFileName;
        bool StructuredMacrosDirty;
        int StructuredMacrosDim = -1;
        int StructuredMacrosSize = -1;
        StructuredMacroRecorder StructuredRecorder = new StructuredMacroRecorder();
        StructuredMacroWindow StructuredMacroWindow;
        CStructuredMacro StructuredRecordCandidate;
        CStructuredMacro CurStructuredMacro;
        Dictionary<int,int> CurStructuredOverrideMasks;
        bool StructuredMacroReverse;

        void BindCube(Cube7D cube) {
            if(cube!=null) cube.TwistExecuted += Cube_TwistExecuted;
        }

        void Cube_TwistExecuted(int gripAxis,int fromAxis,int toAxis,int mask) {
            if(Cube==null || StructuredRecorder==null) return;
            StructuredRecorder.RecordCubeTwistCode(gripAxis,fromAxis,toAxis,mask,Cube.N);
        }

        private void manageStructuredMacros_Click(object sender,EventArgs e) {
            ShowStructuredMacroWindow(null);
        }

        private void recordStructuredMacroRecording_Click(object sender,EventArgs e) {
            if(StructuredRecordCandidate != null) {
                CancelStructuredMacroReferenceSelection();
                RedrawClickStatus();
            } else if(StructuredRecorder.IsRecording) {
                stopStructuredMacroRecording_Click(sender,e);
            } else {
                startStructuredMacroRecording_Click(sender,e);
            }
        }

        private void startStructuredMacroRecording_Click(object sender,EventArgs e) {
            if(StructuredRecorder.IsRecording || StructuredRecordCandidate != null) {
                MessageBox.Show("Structured macro recording is already active.");
                return;
            }
            if(RecordingMacroStatus!=REC_MACRO_NONE) {
                MessageBox.Show("Finish the ordinary macro operation before recording a structured macro.");
                return;
            }

            CStructuredMacro macro = new CStructuredMacro("");
            macro.NStickers = 0;
            macro.Stickers = new int[0];
            macro.Vectors = GetMatrix();
            macro.Orient = (int[])Cube.Orient.Clone();
            StructuredRecordCandidate = macro;
            RecordingMacroStatus = REC_MACRO_STICKERS;
            LMacroStickers = 0;
            RedrawClickStatus();
        }

        private void stopStructuredMacroRecording_Click(object sender,EventArgs e) {
            if(StructuredRecordCandidate != null) {
                CancelStructuredMacroReferenceSelection();
                RedrawClickStatus();
                return;
            }
            CStructuredMacro macro;
            string error;
            if(!StructuredRecorder.TryFinish(out macro,out error)) {
                MessageBox.Show(error);
                return;
            }
            if(!PromptStructuredMacroName(macro, "Enter Structured Macro Name")) {
                RedrawClickStatus();
                return;
            }
            SaveStructuredMacro(macro);
            ShowStructuredMacroWindow(macro);
            RedrawClickStatus();
        }

        private void cancelStructuredMacroRecording_Click(object sender,EventArgs e) {
            if(!StructuredRecorder.IsRecording && StructuredRecordCandidate == null) return;
            CancelStructuredMacroReferenceSelection();
            StructuredRecorder.Cancel();
            RedrawClickStatus();
        }

        private void CancelStructuredMacroReferenceSelection() {
            if(StructuredRecordCandidate == null) return;
            StructuredRecordCandidate = null;
            LMacroStickers = 0;
            if(RecordingMacroStatus == REC_MACRO_STICKERS)
                RecordingMacroStatus = OldRecMacroStatus = REC_MACRO_NONE;
        }

        bool PromptStructuredMacroName(CStructuredMacro macro, string title) {
            while(true) {
                TextDialog edt = new TextDialog(title);
                edt.Value = macro.Name;
                if(edt.ShowDialog(this)!=DialogResult.OK) return false;

                string name = StructuredMacroNames.Normalize(edt.Value);
                if(name.Length == 0) {
                    MessageBox.Show("Structured macro name cannot be empty.");
                    continue;
                }
                int existing = StructuredMacroNames.FindIndex(StructuredMacros, name, macro);
                if(existing >= 0) {
                    DialogResult overwrite = MessageBox.Show(
                        "Structured macro '" + name + "' already exists. Overwrite it?",
                        "Structured Macro",
                        MessageBoxButtons.YesNo);
                    if(overwrite != DialogResult.Yes) continue;
                }

                macro.Name = name;
                return true;
            }
        }

        void SaveStructuredMacro(CStructuredMacro macro) {
            int index = StructuredMacroNames.FindIndex(StructuredMacros, macro.Name, macro);
            if(index >= 0) StructuredMacros[index] = macro;
            else if(!StructuredMacros.Contains(macro)) StructuredMacros.Add(macro);
            MarkStructuredMacrosDirty();
        }

        void ShowStructuredMacroWindow(CStructuredMacro selected) {
            if(StructuredMacroWindow == null || StructuredMacroWindow.IsDisposed) {
                StructuredMacroWindow = new StructuredMacroWindow(StructuredMacros,
                    new Func<int>(GetSize), new Func<int>(GetDim),
                    new ApplyStructuredMacroHandler(ApplyStructuredMacro),
                    new Action(MarkStructuredMacrosDirty));
                StructuredMacroWindow.FormClosed += delegate { StructuredMacroWindow = null; };
                StructuredMacroWindow.Show(this);
            } else {
                StructuredMacroWindow.RefreshMacros(selected);
                if(!StructuredMacroWindow.Visible) StructuredMacroWindow.Show(this);
            }
            if(StructuredMacroWindow.WindowState == FormWindowState.Minimized)
                StructuredMacroWindow.WindowState = FormWindowState.Normal;
            StructuredMacroWindow.BringToFront();
            StructuredMacroWindow.Focus();
            if(selected != null) StructuredMacroWindow.RefreshMacros(selected);
        }

        void ApplyStructuredMacro(CStructuredMacro macro, IDictionary<int, int> overrideMasks, bool reverse) {
            if(macro == null || Cube == null) return;
            if(RecordingMacroStatus==REC_MACRO_STICKERS || RecordingMacroStatus==REC_MACRO_APPLY) return;
            int[] cmap = null;
            if(m_cbQuickMacro.Checked && macro.Vectors != null && macro.Orient != null) {
                cmap = GetFastMacroRef(macro.Vectors, macro.Orient);
            } else if(macro.NStickers > 0) {
                BeginStructuredMacroApplySelection(macro, overrideMasks, reverse);
                return;
            }

            ExecuteStructuredMacroMapped(macro, overrideMasks, reverse, cmap);
        }

        void ExecuteStructuredMacroMapped(CStructuredMacro macro, IDictionary<int, int> overrideMasks,
            bool reverse, int[] axisMap) {
            bool applied;
            using(StructuredRecorder.Suppress()) {
                applied = StructuredMacroExecutor.Apply(Cube, macro, axisMap, overrideMasks, reverse);
            }
            if(!applied) {
                MessageBox.Show("Structured macro contains an invalid twist for the current puzzle.");
                return;
            }

            if(StructuredRecorder.IsRecording) {
                StructuredRecorder.RecordStructuredMacroInvocation(macro, overrideMasks, reverse, Cube.N, axisMap);
                RedrawClickStatus();
            }
            ProcessHighLights();
            TestBuild();
            Redraw();
        }

        void BeginStructuredMacroApplySelection(CStructuredMacro macro, IDictionary<int, int> overrideMasks, bool reverse) {
            CurStructuredMacro = macro;
            CurStructuredOverrideMasks = CloneOverrideMasks(overrideMasks);
            StructuredMacroReverse = reverse;
            OldRecMacroStatus = RecordingMacroStatus;
            RecordingMacroStatus = REC_MACRO_APPLY;
            LMacroStickers = 0;
            RedrawClickStatus();
        }

        static Dictionary<int,int> CloneOverrideMasks(IDictionary<int,int> overrideMasks) {
            Dictionary<int,int> result = new Dictionary<int,int>();
            if(overrideMasks != null) {
                foreach(KeyValuePair<int,int> kv in overrideMasks) result[kv.Key] = kv.Value;
            }
            return result;
        }

        void MarkStructuredMacrosDirty() {
            StructuredMacrosDirty = true;
            StructuredMacrosDim = GetDim();
            StructuredMacrosSize = GetSize();
        }

        void ResetStructuredMacrosForCurrentSize() {
            StructuredMacros.Clear();
            StructuredMacroFileName = null;
            StructuredMacrosDirty = false;
            StructuredMacrosDim = GetDim();
            StructuredMacrosSize = GetSize();
            if(StructuredMacroWindow != null && !StructuredMacroWindow.IsDisposed)
                StructuredMacroWindow.RefreshMacros(null);
        }

        void ClearPendingStructuredMacroState() {
            StructuredRecordCandidate = null;
            CurStructuredMacro = null;
            CurStructuredOverrideMasks = null;
        }

        void EnsureStructuredMacrosMatchCurrentSize() {
            if(StructuredMacrosDim == GetDim() && StructuredMacrosSize == GetSize()) return;
            ResetStructuredMacrosForCurrentSize();
        }

        private void loadStructuredMacros_Click(object sender,EventArgs e) {
            if(StructuredRecorder.IsRecording) {
                MessageBox.Show("Finish or cancel structured macro recording before loading a structured macro file.");
                return;
            }

            OpenFileDialog sf = new OpenFileDialog();
            sf.DefaultExt = ".smdat";
            sf.Filter = "MC7D Structured Macro file (*.smdat)|*.smdat";
            sf.RestoreDirectory = true;
            if(sf.ShowDialog() != DialogResult.OK) return;

            try {
                CStructuredMacroFile file = new CStructuredMacroFile(sf.FileName);
                if(!file.CheckSize(GetDim(), GetSize())) {
                    MessageBox.Show("Wrong cube size");
                    return;
                }
                StructuredMacros = file.Macros;
                StructuredMacroFileName = file.FileName;
                StructuredMacrosDirty = false;
                StructuredMacrosDim = GetDim();
                StructuredMacrosSize = GetSize();
                if(StructuredMacroWindow != null && !StructuredMacroWindow.IsDisposed) {
                    StructuredMacroWindow.Close();
                    StructuredMacroWindow = null;
                }
                ShowStructuredMacroWindow(null);
            } catch(Exception ex) {
                MessageBox.Show("Failed to load structured macros: " + ex.Message);
            }
        }

        private void saveStructuredMacros_Click(object sender,EventArgs e) {
            if(StructuredMacroFileName == null) {
                saveStructuredMacrosAs_Click(sender, e);
                return;
            }
            if(!StructuredMacrosDirty) return;
            SaveStructuredMacrosTo(StructuredMacroFileName);
        }

        private void saveStructuredMacrosAs_Click(object sender,EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = ".smdat";
            sf.Filter = "MC7D Structured Macro file (*.smdat)|*.smdat";
            sf.RestoreDirectory = true;
            if(sf.ShowDialog() == DialogResult.OK)
                SaveStructuredMacrosTo(sf.FileName);
        }

        void SaveStructuredMacrosTo(string fileName) {
            try {
                CStructuredMacroFile file = new CStructuredMacroFile(GetDim(), GetSize());
                file.Macros.AddRange(StructuredMacros);
                file.SaveAs(fileName);
                StructuredMacroFileName = fileName;
                StructuredMacrosDirty = false;
                StructuredMacrosDim = GetDim();
                StructuredMacrosSize = GetSize();
            } catch(Exception ex) {
                MessageBox.Show("Failed to save structured macros: " + ex.Message);
            }
        }
    }
}
