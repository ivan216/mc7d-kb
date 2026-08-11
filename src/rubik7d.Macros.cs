using System;
using System.Windows.Forms;
using Microsoft.DirectX;

namespace _3dedit
{
    public partial class Form1
    {
        int RecordingMacroStatus = REC_MACRO_NONE;
        int OldRecMacroStatus = REC_MACRO_NONE;
        const int REC_MACRO_NONE = 0;
        const int REC_MACRO_STICKERS = 1;
        const int REC_MACRO_CODE = 2;
        const int REC_MACRO_APPLY = 3;

        int[] MacroStickers = new int[100];
        int LMacroStickers = 0;
        int[] MStickers;
        int LMStickers;
        int MacroStart = 0;
        bool MacroReverse = false;
        CMacro CurMacro;
        CMacroFile Macros;

        double[] m_macroVec;
        int[] m_macroOrient;
        string m_macroName;

        private void mi_StartRecordig_Click(object sender,EventArgs e) {
            if(StructuredRecorder.IsRecording || StructuredRecordCandidate != null) {
                MessageBox.Show("Finish or cancel structured macro recording before recording an ordinary macro.");
                return;
            }
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
                RedrawClickStatus();
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
    }
}
