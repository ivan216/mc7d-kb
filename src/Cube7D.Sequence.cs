using System;

namespace _3dedit {

    public partial class Cube7D {
        void CheckSeqLen() {
            if(Seq.Length==LPtr) {
                int[] p2=new int[2*LPtr];
                for(int i=0;i<LPtr;i++) p2[i]=Seq[i];
                Seq=p2;
            }
        }

        public void StartMacro() {
            CheckSeqLen();
            Seq[LPtr++]=-2;
            LSeq=LPtr;
        }
        public void StopMacro() {
            CheckSeqLen();
            Seq[LPtr++]=-1;
            LSeq=LPtr;
        }

        public bool Undo() {
            bool res=false;
            int cmacro=0;
            for(;;) {
                if(LPtr==LShuffle) break;
                int code=Seq[--LPtr];
                if(code==-1) cmacro++;
                else if(code==-2) cmacro--;
                else {
                    int m=code&((1<<N)-1);
                    code>>=N;
                    MakeTwist(code/D/D,code%D,(code/D)%D,m);
                    NTwists--;
                    res=true;
                }
                if(cmacro<=0) break;
            }
            return res;
        }
        public bool Redo() {
            bool res=false;
            int cmacro=0;
            for(;;) {
                if(LPtr==LSeq) break;
                int code=Seq[LPtr++];
                if(code==-2) cmacro++;
                else if(code==-1) cmacro--;
                else {
                    int m=code&((1<<N)-1);
                    code>>=N;
                    MakeTwist(code/D/D,(code/D)%D,code%D,m);
                    NTwists++;
                    res=true;
                }
                if(cmacro<=0) break;
            }
            return res;
        }

        public int GetNTwists(int from,int to) {
            int res=0;
            for(int i=from;i<to && i<LPtr;i++) if(Seq[i]>=0) res++;
            return res;
        }

        internal void Scramble(int nt) {
            if(nt<0) {
                nt=2*D*(D-1)*N;
            }
            Init(N,D);
            LPtr=0;
            uint seed=(uint)(DateTime.Now.Ticks/10000000);
            int cv=D*(D-1)*(D-2)*N;
            for(int i=0;i<nt;i++) {
                seed=(seed*0x1010005+1);
                int tt=(int)(((long)seed*cv)>>32);
                int f0=tt%D; tt/=D;
                int f1=tt%(D-1); tt/=D-1;
                int f2=tt%(D-2); tt/=D-2;
                int m=1<<tt;
                f2=(f1+f2+1)%(D-1);
                f1=(f0+f1+1)%D;
                f2=(f0+f2+1)%D;
                Seq[LPtr++]=(((f0*D+f1)*D+f2)<<N)+m;
            }
            LShuffle=LSeq=LPtr;
            NTwists=0;
            Recalculate();
        }

        public void ApplySeqReverse(int from,int to) {
            for(int i=to-1;i>=from;i--) {
                int code=Seq[i];
                if(code==-1) StartMacro();
                else if(code==-2) StopMacro();
                else {
                    int m=code&((1<<N)-1);
                    code>>=N;
                    Twist(code/D/D+1,code%D+1,(code/D)%D+1,reverse(m));
                }
            }
        }

        internal void Recalculate() {
            InitCube();
            int []F=new int[D];
            int fm=1<<N,fm1=fm/2;
            for(int i=0;i<NC;i++) {
                if(Cube[i]==0){ Cube2[i]=0; continue; }
                int v=i;
                for(int j=0;j<D;j++) {
                    F[j]=v%N2; v/=N2;
                }
                for(int j=0;j<LPtr;j++) {
                    int c=Seq[j];
                    if(c<0) continue;
                    int m=c&(fm-1); m=(m&1)+(m<<1)+((m&fm1)<<2);
                    c>>=N;
                    int f0=c/D/D;
                    int f1=(c/D)%D;
                    int f2=c%D;
                    if(((1<<F[f0])&m)!=0) {
                        int a=N+1-F[f2];
                        F[f2]=F[f1];
                        F[f1]=a;
                    }
                }
                v=0;
                for(int j=D;--j>=0;) v=v*N2+F[j];
                Cube2[v]=Cube[i];
            }
            for(int i=0;i<NC;i++) Cube[i]=Cube2[i];
            NTwists=GetNTwists(LShuffle,LPtr);
        }
    }
}
