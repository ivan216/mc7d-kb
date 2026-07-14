using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

namespace _3dedit {
    class CMacro {
        internal string Name;
        internal int NStickers;
        internal int[] Stickers;
        internal int LMacro;
        internal int[] Code;
        internal double[] Vectors;
        internal int[] Orient;
        internal CMacro(string name,int nstk,int[] stk,int lcode,int[] code,int from) {
            Name=name.Replace(' ','_');
            NStickers=nstk;
            Stickers=new int[nstk];
            Buffer.BlockCopy(stk,0,Stickers,0,nstk*4);
            int lm=0;
            for(int i=0;i<lcode;i++) if(code[from+i]>=0) lm++;
            LMacro=lm;
            Code=new int[lm];
            lm=0;
            for(int i=0;i<lcode;i++) if(code[from+i]>=0) Code[lm++]=code[from+i];
        }

    }

    class CMacroFile {
        Hashtable m_htbl;
        int Dim,Size;
        internal string FileName;

        internal CMacroFile(int d,int s) {
            m_htbl=new Hashtable();
            FileName=null;
            Dim=d; Size=s;
        }

        internal bool CheckSize(int d,int s) {
            return d==Dim && s==Size;
        }


        internal void AddMacro(CMacro x) {
            if(m_htbl.ContainsKey(x.Name)) m_htbl[x.Name]=x;
            else m_htbl.Add(x.Name,x);
        }
        internal string[] GetNamesList() {
            return (string[])(new ArrayList(m_htbl.Keys).ToArray(typeof(string)));
        }
        internal CMacro GetMacro(string name) { return (CMacro)m_htbl[name]; }

        internal void Delete(string name) {
            m_htbl.Remove(name);
        }
        internal void Rename(string oldname,string newname) {
            if(m_htbl.ContainsKey(newname)) return;
            CMacro m=(CMacro)m_htbl[oldname];
            m.Name=newname;
            m_htbl.Remove(oldname);
            m_htbl.Add(newname,m);
        }
        internal void SaveAs(string fn) {
            FileName=fn;
            Save();
        }
        internal void Save() {
            try {
                StreamWriter sw=new StreamWriter(FileName);
                sw.NewLine="\r\n";
                sw.WriteLine("MC7D Macro File");
                sw.WriteLine("{0} {1} {2}",Dim,Size,m_htbl.Count);
                foreach(CMacro m in m_htbl.Values) {
                    sw.WriteLine("{0} {1} {2}",m.Name,m.NStickers,m.LMacro);
                    if(m.Vectors!=null) {
                        string sv="#Dir";
                        for(int i=0;i<9;i++) sv+=string.Format(" {0:F4}",m.Vectors[i]);
                        for(int i=0;i<Dim;i++) sv+=string.Format(" {0}",m.Orient[i]);
                        sw.WriteLine(sv);
                    }
                    for(int i=0;i<m.NStickers;i++) sw.Write("{0} ",m.Stickers[i]);
                    sw.WriteLine();
                    for(int j=0;j<m.LMacro;j++) {
                        sw.Write("{0} ",m.Code[j]);
                        if(j%16==15 || j==m.LMacro-1) sw.WriteLine();
                    }
                }
                sw.Close();
            } catch { }
        }
        internal CMacroFile(string fn){
            try {
                StreamReader sr=new StreamReader(fn);
                string ln=sr.ReadLine();
                if(ln!="MC7D Macro File") goto _1;
                FileName=fn;
                ln=sr.ReadLine();
                string[] ss=ln.Split(' ');
                Dim=int.Parse(ss[0]);
                Size=int.Parse(ss[1]);
                int nm=int.Parse(ss[2]);
                m_htbl=new Hashtable();
                for(int i=0;i<nm;i++) {
                    ln=sr.ReadLine();
                    ss=ln.Split(' ');
                    string name=ss[0];
                    int nstk=int.Parse(ss[1]);
                    int lm=int.Parse(ss[2]);
                    int[] stk=new int[nstk];
                    int[] code=new int[lm];
                    double[] V=null;
                    int[] Or=null;
                    ln=sr.ReadLine();
                    ss=ln.Split(' ');
                    if(ss[0]=="#Dir") {
                        V=new double[9];
                        for(int j=0;j<9;j++) V[j]=double.Parse(ss[j+1]);
                        Or=new int[7];
                        for(int j=0;j<Dim;j++) Or[j]=int.Parse(ss[j+10]);
                        ln=sr.ReadLine();
                        ss=ln.Split(' ');
                    }
                    for(int j=0;j<nstk;j++) stk[j]=int.Parse(ss[j]);
                    int p=0;
                    while(p<lm) {
                        ln=sr.ReadLine();
                        ss=ln.Split(' ');
                        for(int j=0;j<ss.Length;j++) {
                            if(p==lm) break;
                            if(ss[j]!="") code[p++]=int.Parse(ss[j]);
                        }
                    }
                    CMacro mm=new CMacro(name,nstk,stk,lm,code,0);
                    mm.Vectors=V;
                    mm.Orient=Or;
                    m_htbl.Add(name,mm);
                }
                sr.Close();
                return;
            } catch { }
            _1:
            m_htbl=new Hashtable();
            FileName=null;
            Dim=4; Size=3;

        }

    }
}
