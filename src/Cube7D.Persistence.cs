using System;
using System.IO;

namespace _3dedit {

    public partial class Cube7D {
        public void Save(string fn) {
            CSum=0;
            AddCSum(D); AddCSum(N); AddCSum(LSeq); AddCSum(LShuffle); AddCSum(LPtr);
            AddCSum(CTime/10000);
            for(int i=0;i<LSeq;i++) AddCSum(Seq[i]);
            try {
                StreamWriter sw=new StreamWriter(fn);
                sw.NewLine="\r\n";
                sw.WriteLine("MC7D {0} {1} {2} {3} {4}",D,N,LSeq,LShuffle,LPtr);
                char[] line=new char[257];
                int p=0;
                for(int i=0;i<NC;i++) {
                    int b=Cube[i];
                    if(b==0) continue;
                    line[p++]=(char)(b<10 ? b+0x30 : b+0x37);
                    if(p==256){
                        sw.WriteLine(new string(line,0,p));
                        p=0;
                    }
                }
                if(p!=0) sw.WriteLine(new string(line,0,p));
                sw.WriteLine("#time {0}",CTime/10000);
                sw.WriteLine("#CRC {0}",RevBit(CSum));
                sw.WriteLine("*");
                for(int i=0;i<LSeq;i+=16) {
                    int l=Math.Min(16,LSeq-i);
                    for(int j=0;j<l;j++) {
                        if(i+j==LShuffle) sw.Write("m| ");
                        int d=Seq[i+j];
                        if(d==-2) sw.Write("m[ ");
                        else if(d==-1) sw.Write("m] ");
                        else sw.Write("{0} ",d);
                    }
                    sw.WriteLine();
                }
                sw.Close();
            } catch {
                System.Windows.Forms.MessageBox.Show("Cannot save file "+fn);
                return;
            }
        }

        public void SaveStripMarkers(string fn) {
            // Build flat index mapping: filter out macro markers (-1, -2)
            int flatLShuffle = 0, flatLPtr = 0, flatLSeq = 0;
            for(int i=0;i<LSeq;i++) {
                if(Seq[i] < 0) continue;
                if(i < LShuffle) flatLShuffle = flatLSeq + 1;
                if(i < LPtr) flatLPtr = flatLSeq + 1;
                flatLSeq++;
            }
            CSum=0;
            AddCSum(D); AddCSum(N); AddCSum(flatLSeq); AddCSum(flatLShuffle); AddCSum(flatLPtr);
            AddCSum(CTime/10000);
            for(int i=0;i<LSeq;i++) if(Seq[i]>=0) AddCSum(Seq[i]);
            try {
                StreamWriter sw=new StreamWriter(fn);
                sw.NewLine="\r\n";
                sw.WriteLine("MC7D {0} {1} {2} {3} {4}",D,N,flatLSeq,flatLShuffle,flatLPtr);
                char[] line=new char[257];
                int p=0;
                for(int i=0;i<NC;i++) {
                    int b=Cube[i];
                    if(b==0) continue;
                    line[p++]=(char)(b<10 ? b+0x30 : b+0x37);
                    if(p==256){
                        sw.WriteLine(new string(line,0,p));
                        p=0;
                    }
                }
                if(p!=0) sw.WriteLine(new string(line,0,p));
                sw.WriteLine("#time {0}",CTime/10000);
                sw.WriteLine("#CRC {0}",RevBit(CSum));
                sw.WriteLine("*");
                int idx=0;
                for(int i=0;i<LSeq;i++) {
                    if(Seq[i] < 0) continue;
                    if(idx==flatLShuffle) sw.Write("m| ");
                    sw.Write("{0} ",Seq[i]);
                    if(++idx%16==0) sw.WriteLine();
                }
                if(idx%16!=0) sw.WriteLine();
                sw.Close();
            } catch {
                System.Windows.Forms.MessageBox.Show("Cannot save file "+fn);
                return;
            }
        }

        ulong CSum;
        void AddCSum(long m) {
            CSum=CSum*0x12345675+(ulong)m;
        }
        ulong RevBit(ulong x) {
            ulong y=0;
            for(int i=0;i<64;i++) {
                y=(y<<1)+(x&1);
                x>>=1;
            }
            return y;
        }
        public void Load(string fn){
            try {
                StreamReader sw=new StreamReader(fn);
                ulong crc=0;
                bool ct=false;
                for(;;) {
                    string line=sw.ReadLine();
                    if(line==null) break;
                    string[] s=line.Split(' ');
                    if(s[0]=="MagicCube4D") {
                        sw.Close();
                        sw=new StreamReader(fn);
                        bool xx=ImportLogMC4D(sw);
                        if(!xx) break;
                        sw.Close();
                        return;
                    }
                    if(s[0]!="MC7D" || s.Length<6) break;
                    int d=int.Parse(s[1]);
                    int n=int.Parse(s[2]);
                    int lseq=int.Parse(s[3]);
                    int lshuf=int.Parse(s[4]);
                    int lptr=int.Parse(s[5]);
                    Init(n,d);

                    int lp=0,p=0;
                    for(int i=0;i<NC;i++) {
                        if(Cube[i]==0) continue;
                        while(p==lp) {
                            line=sw.ReadLine();
                            lp=line.Length;
                            p=0;
                            while(p<lp && line[p]=='0') p++;
                        }
                        char c=line[p++];
                        if(c>='0' && c<='9') Cube[i]=(byte)(c-'0');
                        else if(c>='A' && c<='E') Cube[i]=(byte)(c-'A'+10);
                        else goto _1;
                        while(p<lp && line[p]=='0') p++;
                    }
                    if(Seq.Length<lseq) Seq=new int[2*lseq];
                    for(;;) {
                        line=sw.ReadLine();
                        if(line==null) break;
                        if(line.StartsWith("#time ")) {
                            CTime=long.Parse(line.Split(' ')[1])*10000;
                            ct=true;
                        } else if(line.StartsWith("#CRC ")) crc=RevBit(ulong.Parse(line.Split(' ')[1]));
                        if(line[0]=='#' || line[0]=='*') continue;
                        break;
                    }

                    for(int i=0;i<lseq;) {
                        if(line==null) goto _1;
                        s=line.Split(' ');
                        foreach(string cc in s) {
                            if(cc=="m|") LShuffle=i;
                            else if(cc!=""){
                                if(i==lseq) goto _1;
                                int c=0;
                                if(cc=="m[") c=-2;
                                else if(cc=="m]") c=-1;
                                else c=int.Parse(cc);
                                Seq[i]=c;
                                i++;
                            }
                        }
                        line=sw.ReadLine();
                    }
                    LSeq=lseq;
                    LShuffle=lshuf;
                    LPtr=lptr;
                    NTwists=GetNTwists(lshuf,lptr);
                    sw.Close();

                    if(crc!=RevBit(1234567890123456789L) && ct) {
                        CSum=0;
                        AddCSum(D); AddCSum(N); AddCSum(LSeq); AddCSum(LShuffle); AddCSum(LPtr);
                        AddCSum(CTime/10000);
                        for(int i=0;i<LSeq;i++) AddCSum(Seq[i]);

                        if(crc!=CSum) {
                            System.Windows.Forms.MessageBox.Show("Checksum Error");
                            Init(3,7);
                        }
                    }
                    return;
                }
_1: ;
            }catch{}
            System.Windows.Forms.MessageBox.Show("Cannot load file "+fn);
            Init(3,7);
        }

        internal bool ImportLogMC4D(StreamReader sw) {
            ImportMC4D imp=new ImportMC4D();
            bool qok=imp.ReadLog(sw);
            if(!qok) {
                System.Windows.Forms.MessageBox.Show("Cannot import log");
                return false;
            }
            int n=imp.Size;
            int d=4;
            int lseq=imp.LSeq;
            int lshuf=imp.LShuffle;
            int lptr=lseq;
            Init(n,d);

            Seq=imp.Seq;
            LSeq=lseq;
            LShuffle=lshuf;
            LPtr=lptr;
            NTwists=GetNTwists(lshuf,lptr);
            Recalculate();
            return true;
        }
    }
}
