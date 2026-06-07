using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Linq;


namespace _3dedit {
    
    public class Cube7D {
        public int N,D;
        int NC,N2;
        byte[] Cube,Cube2;
        public BitArray HighLighted;
        public byte[] StkNCols;

        int NStk;
        int NStk0,NStk1;
        float[,] Coord;
        public int[] StkMap; // sticker to Cube
        public static double FSep=0.5,FExt=1,BSize=0.8,SSize=0.6;

        public int[] Orient;  // +-side
        int[] DimStat; // 0 - center, 1 - main, 2 - secondary

        public int[] Gripped;
        public Keybindings.Twist partialTwist;
        public HashSet<Keybindings.Layer> LayerOverrides;

        public short[] Seq;
        public int LSeq,LPtr,LShuffle;
        public int NTwists;
        public long CTime;

        public Cube7D() { }

        static int Pow(int p,int n) {
            int b=1;
            while(--n>=0) b*=p;
            return b;
        }
        static int Digit(int v,int p,int n) {
            while(--n>=0) v/=p;
            return v%p;
        }

        public void Init(int n,int d) {
            D=d; N=n; N2=N+2;
            NC=Pow(N2,d);
            Cube=new byte[NC];
            Cube2=new byte[NC];

            NStk0=Pow(N,D-4)+2*(D-4)*Pow(N,D-5);
            NStk1=NStk0*N*N*N;
            NStk=NStk1*7;

            Coord=new float[NStk,28];
            StkMap=new int[NStk];
            HighLighted=new BitArray(NC);
            StkNCols=new byte[NC];

            Orient=new int[7];
            for(int i=0;i<D;i++) Orient[i]=i+1;
            DimStat=new int[7];
            for(int i=1;i<D;i++) DimStat[i]=i<4 ? 1 : 2;

            Gripped = new int[2] { -1, 1 };
            partialTwist = new Keybindings.Twist();
            LayerOverrides = new HashSet<Keybindings.Layer>();

            SetCoords();
            InitCube();
            InitStkMap();
            LSeq=LPtr=LShuffle=0;
            Seq=new short[10000];
            CTime=0;
        }

        public void SetCoords() {
            double b0,db,ds,ss,dS,sS;

            db=1.0/N;
            ds=db*BSize/(N2);
            ss=ds*SSize;
            b0=db*(1-BSize)/2+ds*(1-SSize)/2-0.5;
            sS=3*ss;
            dS=(db*BSize-sS)/2;

            int st0=Pow(N2,D-4),st1=st0*N*N*N,st2=st1*7;
            int u=0;
            for(int p=0;p<st2;p++) {
                int f=p/st1;
                int b=(p%st1)/st0;
                int s=p%st0;

                double x0,dx,y0,dy,z0,dz;
                x0=b0+(b%N)*db;
                y0=b0+((b/N)%N)*db;
                z0=b0+(b/N/N)*db;

                int nf=0;
                if(D<=4) {
                    x0+=dS; dx=sS;
                } else {
                    int n=s%N2;
                    x0+=n*ds; dx=ss;
                    if(n==0 || n==N+1) nf++;
                }
                if(D<=5) {
                    y0+=dS; dy=sS;
                } else {
                    int n=(s/N2)%N2;
                    y0+=n*ds; dy=ss;
                    if(n==0 || n==N+1) nf++;
                }
                if(D<=6) {
                    z0+=dS; dz=sS;
                } else {
                    int n=s/N2/N2;
                    z0+=n*ds; dz=ss;
                    if(n==0 || n==N+1) nf++;
                }

                if(nf>1) continue;
                if(nf==1) {
                    x0+=dx/8; dx*=0.75;
                    y0+=dy/8; dy*=0.75;
                    z0+=dz/8; dz*=0.75;
                } else if(D==4) {
                    x0-=dx/2; dx*=2;
                    y0-=dy/2; dy*=2;
                    z0-=dz/2; dz*=2;
                }

                Coord[u,0]=Coord[u,6]=Coord[u,12]=Coord[u,18]=(float)x0;
                Coord[u,3]=Coord[u,9]=Coord[u,15]=Coord[u,21]=(float)(x0+dx);
                Coord[u,1]=Coord[u,4]=Coord[u,13]=Coord[u,16]=(float)y0;
                Coord[u,7]=Coord[u,10]=Coord[u,19]=Coord[u,22]=(float)(y0+dy);
                Coord[u,2]=Coord[u,5]=Coord[u,8]=Coord[u,11]=(float)z0;
                Coord[u,14]=Coord[u,17]=Coord[u,20]=Coord[u,23]=(float)(z0+dz);

                if(f!=0) {
                    int k=(f-1)/2,dk=2*(f&1)-1;
                    for(int i=0;i<24;i+=3) {
                        float x=Coord[u,i+k];
                        double cf=1+(dk*x+0.5)*FExt;
                        Coord[u,i]*=(float)cf;
                        Coord[u,i+1]*=(float)cf;
                        Coord[u,i+2]*=(float)cf;
                        Coord[u,i+k]=(float)(x+dk*(1+FSep));
                    }
                }
                double xmin=Coord[u,0],xmax=Coord[u,0];
                double ymin=Coord[u,1],ymax=Coord[u,1];
                double zmin=Coord[u,2],zmax=Coord[u,2];
                for(int i=3;i<24;i+=3) {
                    xmin=Math.Min(xmin,Coord[u,i]);
                    xmax=Math.Max(xmax,Coord[u,i]);
                    ymin=Math.Min(ymin,Coord[u,i+1]);
                    ymax=Math.Max(ymax,Coord[u,i+1]);
                    zmin=Math.Min(zmin,Coord[u,i+2]);
                    zmax=Math.Max(zmax,Coord[u,i+2]);
                }
                Coord[u,24]=(float)((xmax+xmin)/2);
                Coord[u,25]=(float)((ymax+ymin)/2);
                Coord[u,26]=(float)((zmax+zmin)/2);
                Coord[u,27]=(float)(MyMath.pyth(zmax-zmin,ymax-ymin,xmax-xmin)/2);
                u++;
            }
        }


        void InitCube() {
            for(int i=0;i<NC;i++) {
                int a=i,b=0;
                int nst=0;
                for(int n=1;n<=D;n++) {
                    int p=a%N2; a/=N2;
                    if(p==0 || p==N2-1) {
                        if(b==0) b=p==0 ? n : n+7;
                        else b=-1;
                        nst++;
                    } else if(p==1 || p==N) nst++;

                }
                Cube[i]=(byte)(b<0 ? 0 : b);
                StkNCols[i]=(byte)nst;
            }
            HighLighted.SetAll(true);
        }

        void InitStkMap() {
            int st0=Pow(N2,D-4),st1=st0*N*N*N,st2=st1*7;
            int u=0;

            int[] buf=new int[D],disp=new int[D+1];

            for(int i=0;i<D;i++) {
                int p=Orient[i];
                if(p>0) {
                    disp[i]=Pow(N2,p-1);
                } else {
                    disp[i]=-Pow(N2,-1-p);
                    disp[D]-=disp[i]*(N+1);
                }
                DimStat[Math.Abs(p)-1]=(i+2)/3;
            }

            for(int p=0;p<st2;p++) {
                buf[0]=N+1;
                int a=p;
                if(D>4) { buf[4]=a%N2; a/=N2; }
                if(D>5) { buf[5]=a%N2; a/=N2; }
                if(D>6) { buf[6]=a%N2; a/=N2; }
                buf[1]=a%N+1; a/=N;
                buf[2]=a%N+1; a/=N;
                buf[3]=a%N+1; a/=N;

                int nf=0;
                for(int i=4;i<D;i++) {
                    if(buf[i]==0 || buf[i]==N+1) {
                        nf++; buf[0]=N;
                    }
                }
                if(nf>1) continue;

                if(a>0) {
                    int f=(a+1)/2;
                    if(a%2==0) {
                        int w=buf[f];
                        buf[f]=N+1-buf[0];
                        buf[0]=w;
                    } else {
                        int w=buf[f];
                        buf[f]=buf[0];
                        buf[0]=N+1-w;
                    }
                }
                int m=disp[D];
                for(int i=0;i<D;i++) m+=disp[i]*buf[i];
                StkMap[u++]=m;
            }
        }
        public int GetStickers(out byte[] col,out int[] map,out float[,] coord,out BitArray hlight,out byte []ncol) {
            col=Cube;
            map=StkMap;
            coord=Coord;
            ncol=StkNCols;
            hlight=HighLighted;
            return NStk;
        }

        public int reverse(int m) {
            int m1=0;
            for(int i=0;i<N;i++) {
                m1=(m1<<1)+(m&1);
                m>>=1;
            }
            return m1;
        }
        void NormTwist(ref int f0,ref int f1,ref int f2,ref int m) {
            if(f0<0) f0=-f0;
            else m=reverse(m);
            m&=((1<<N)-1);
            while(f1<0) {
                int c=-f1; f1=f2; f2=c;
            }
            if(f2<0) {
                int c=-f2; f2=f1; f1=c;
            }
        }

        public bool TwistGrip(int f1, int f2) {
            if (Gripped[0] < 0)
            {
                return false;
            }
            int f0 = Orient[Gripped[0] - 1];
            f1 = Orient[f1 - 1];
            f2 = Orient[f2 - 1];
            return Twist(f0, f1, f2, GetLayerMask());
        }

        public int NormGripMask(int m0)
        {
            return m0 < 0 ? reverse(-m0) : m0;
        }
        public void Grip(int f0, int m0) {
            m0 = NormGripMask(m0);

            if (Gripped[0] == f0 && Gripped[1] == m0)
            {
                Gripped[0] = -1;
            }

            Gripped[0] = f0;
            Gripped[1] = m0;

            if (Gripped[0] == -1)
            {
                partialTwist.toAxis = null;
                partialTwist.fromAxis = null;
            }
        }

        public bool Twist(int f0,int f1,int f2,int m0) {
            NormTwist(ref f0,ref f1,ref f2,ref m0);           
            if(f0==f1 || f0==f2 ||f1==f2) return false;
            f0--; f1--; f2--;
            MakeTwist(f0,f1,f2,m0);

            int code=(((f0*D+f1)*D+f2)<<N)+m0;
#if false            
            int rcode=(((f0*D+f2)*D+f1)<<N)+m0;

            if(LPtr>LShuffle && Seq[LPtr-1]==rcode) LPtr--;
            else {
                CheckSeqLen();
                Seq[LPtr++]=(short)code;
            }
#else
            CheckSeqLen();
            Seq[LPtr++]=(short)code;
#endif
            NTwists++;
            LSeq=LPtr;
            return true;
        }


        void CheckSeqLen() {
            if(Seq.Length==LPtr) {
                short[] p2=new short[2*LPtr];
                for(int i=0;i<LPtr;i++) p2[i]=Seq[i];
                Seq=p2;
            }
        }

        public void MakeTwist(int f0,int f1,int f2,int m0) {
            if(f0<0 || f1<0 || f2<0 || m0<=0 || f0==f1 || f0==f2 || f1==f2) {
                System.Windows.Forms.MessageBox.Show(string.Format("Error in twist: f0={0}, f1={1}, f2={2}, m={3}",f0,f1,f2,m0));
                return;
            }

            int m1=1<<(N-1);
            int m=(m0&1)+(m0<<1)+((m0&m1)<<2);
            int c0=Pow(N2,f0),c1=Pow(N2,f1),c2=Pow(N2,f2);
            for(int i=0;i<NC;i++) {
                int k=(i/c0)%N2;
                if((m&(1<<k))==0) Cube2[i]=Cube[i];
                else {
                    int d1=(i/c1)%N2,d2=(i/c2)%N2;
                    int i1=i+(N+1-d2-d1)*c1+(d1-d2)*c2;
                    Cube2[i1]=Cube[i];
                }
            }
            for(int i=0;i<NC;i++) Cube[i]=Cube2[i];
        }

        public int GetLayerMask()
        {
            if (LayerOverrides.Count == 0) return Gripped[1];

            int res = 0;
            foreach( var item in LayerOverrides)
            {
                res |= item.layerMask;
            }

            if ((Gripped[1]&reverse(1)) != 0)
            {
                res = reverse(res);
            }

            return res;
        }

        public int[] NormGrip()
        {
            int f0 = Gripped[0], m0 = GetLayerMask();
            if (f0 == -1) return Gripped;

            f0 = Orient[f0 - 1];
            if (f0 < 0) f0 = -f0;
            else m0 = reverse(m0);

            return new int[] { f0, m0 };
        }

        public void HighLightGrip()
        {
            int[] ng = NormGrip();
            int f0 = ng[0], m0 = ng[1];
            if (f0 == -1) return;

            f0 = f0 - 1;

            int m1 = 1 << (N - 1);
            int m = (m0 & 1) + (m0 << 1) + ((m0 & m1) << 2);
            int c0 = Pow(N2, f0);
            for (int i = 0; i < NC; i++)
            {
                int k = (i / c0) % N2;
                if ((m & (1 << k)) == 0) HighLighted[i] = false;
            }
        }

        internal int GetFirstSticker(int s,int ClickMode,out int F1) {  // returns FaceClick
            F1=0;
            if(s<0 || s>=StkMap.Length) { System.Windows.Forms.MessageBox.Show("Error in GetFirstSticker: s="+s); return 0; }
            int u=StkMap[s];
            int f0=0,f1=0,f2=0;

            int fmain=s/NStk1;
            if(fmain==0) fmain=Orient[0];
            else fmain=Orient[(fmain+1)/2]*(2*(fmain&1)-1);

            int nmain=0,nsec=0;
            for(int i=0;i<D;i++) {
                int n=u%N2; u/=N2;
                if(n==0 || n==N+1) {
                    f0=n==0 ? -(i+1) : i+1;
                } else if(n==1 || n==N) {
                    if(DimStat[i]==2) {
                        nsec++; f2=(n==1 ? -(i+1) : i+1);
                    } else {
                        nmain++; f1=(n==1 ? -(i+1) : i+1);
                    }
                }
            }
            if(ClickMode==2) return f0;
            if(DimStat[Math.Abs(f0)-1]==2) {  // click to secondary!
                if(nsec==1) F1=f2;
                else F1=fmain;
            } else {
                if(nmain==1) F1=f1;
                else if(nsec==1) F1=f2;
                else return 0;
            }
            if(ClickMode==1) {
                int cc=F1;
                F1=f0;
                f0=cc;
            }
            return f0;
        }
        internal int GetSecondSticker(int s,int fclick) {
            if(s<0 || s>=StkMap.Length) { System.Windows.Forms.MessageBox.Show("Error in GetSecondSticker: s="+s); return 0; }
            int u=StkMap[s];
            int fd=Math.Abs(fclick)-1;
            int f0=0,f1=0,f2=0;

            int nmain=0,nsec=0;
            for(int i=0;i<D;i++) {
                int n=u%N2; u/=N2;
                if(i==fd) continue; // forbidden direction
                if(n==0 || n==N+1) {
                    f0=n==0 ? -(i+1) : i+1;
                } else if(n==1 || n==N) {
                    if(DimStat[i]==2) {
                        nsec++; f2=(n==1 ? -(i+1) : i+1);
                    } else {
                        nmain++; f1=(n==1 ? -(i+1) : i+1);
                    }
                }
            }
            if(f0!=0) return f0;
            if(nmain==1) return f1;
            if(nsec==1) return f2;
            return 0;
        }

        
        public int GetStickerFace(int st) {  // absolute
            for(int i=0;i<D;i++) {
                int n=st%N2; st/=N2;
                if(n==0) return -(i+1);
                if(n==N+1) return i+1;
            }
            return 0;
        }
        public int GetFaceDir(int f) { // absolute to visible
            for(int i=0;i<D;i++) {
                if(Orient[i]==f) return i+1;
                if(Orient[i]==-f) return -(i+1);
            }
            return 0;
        }

        public bool RotateCubeByGrip()
        {
            int[] ng = NormGrip();
            int f0 = ng[0];
            if ((ng[1]&reverse(1)) == 0)
            {
                f0 = -f0;
            }
            return RotateCubeByFacet(f0);
        }

        public bool RotateCubeBySticker(int st) { // visible
            int f0=GetStickerFace(StkMap[st]);
            return RotateCubeByFacet(f0);
        }

        public bool RotateCubeByStickerInverse(int st) { // visible - inverse operation
            int f0=GetStickerFace(StkMap[st]);
            // Inverse operation: rotate by the opposite face
            int f0_inverse = -f0;
            return RotateCubeByFacet(f0_inverse);
        }

        public bool RotateCubeByFacet(int f0)
        {
            int d0 = GetFaceDir(f0);
            if (Math.Abs(d0) == 1) return false;

            int c = Orient[0];
            if (d0 > 0) Orient[d0 - 1] = -c;
            else Orient[-d0 - 1] = c;
            Orient[0] = f0;
            InitStkMap();
            return true;
        }

        public bool RotateCubeByStickers(int f0,int f1,int f2) { // absolute
            int d1=GetFaceDir(f1),d2=GetFaceDir(f2);
            int v1=Math.Abs(d1)-1,v2=Math.Abs(d2)-1;
            if(v1==v2) return false;
            int c=Orient[v1];
            if(d1*d2>0) {
                Orient[v1]=-Orient[v2];
                Orient[v2]=c;
            } else {
                Orient[v1]=Orient[v2];
                Orient[v2]=-c;
            }
            InitStkMap();
            return true;
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
                if(D==5) nt=100;
                else nt=2*D*(D-1)*N;
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
                Seq[LPtr++]=(short)((((f0*D+f1)*D+f2)<<N)+m);
            }
            LShuffle=LSeq=LPtr;
            NTwists=0;
            Recalculate();
        }

        internal void FindOtherStickers(int stk) {
            HighLighted.SetAll(false);
            stk=StkMap[stk];
            int v=1;
            for(int i=0;i<D;i++) {
                int s=(stk/v)%N2;
                if(s==0) stk+=v;
                else if(s==N+1) stk-=v;
                v*=N2;
            }
            v=1;
            for(int i=0;i<D;i++) {
                if(Cube[stk+v]!=0) HighLighted[stk+v]=true;
                if(Cube[stk-v]!=0) HighLighted[stk-v]=true;
                v*=N2;
            }
        }

        internal void FindAdjStickers(int stk) {
            FindOtherStickers(stk);
        }

        // Helper method: check if any mask has black check (value > 0)
        private bool HasBlackCheck(int[] mask, int startIndex, int endIndex) {
            for(int i = startIndex; i <= endIndex; i++) {
                if(mask[i] > 0) {
                    return true;
                }
            }
            return false;
        }

        internal void FindStickersByMask(int[] hmask,bool cAll) {  // array indexed by 1..14, hmask=-1,0,1
            FindStickersByMask(hmask, cAll, null);
        }

        internal void FindStickersByMask(int[] hmask,bool cAll,int[] ncolMask) {  // array indexed by 1..14, hmask=-1,0,1; ncolMask indexed by 1..7, values: -1=exclude (dark), 0=neutral/gray, 1=include (highlight)
            HighLighted.SetAll(false);

            // Check if any color is selected (black check)
            bool hasColorBlackCheck = HasBlackCheck(hmask, 1, 14);

            int []cmask=new int[16];
            if(cAll) {
                for(int i=0;i<NC;i++) {
                    bool qf=true;
                    int v=i;
                    for(int j=0;j<D;j++) {
                        int w=v%N2; v/=N2;
                        if(w==0 || w==N2-1) { qf=false; break; }
                    }
                    if(!qf) continue;
                    v=1;
                    for(int j=1;j<=D;j++) cmask[j]=cmask[j+7]=-1;
                    for(int j=1;j<=D;j++) {
                        cmask[Cube[i-v]]=1;
                        cmask[Cube[i+v]]=1;
                        v*=N2;
                    }
                    qf=true;
                    for(int j=1;j<=D;j++) {
                        if(hmask[j]*cmask[j]<0) { qf=false; break; }
                        if(hmask[j+7]*cmask[j+7]<0) { qf=false; break; }
                    }
                    if(qf) {
                        v=1;
                        for(int j=1;j<=D;j++) {
                            if(Cube[i-v]!=0) {
                                HighLighted[i-v]=true;
                            }
                            if(Cube[i+v]!=0) {
                                HighLighted[i+v]=true;
                            }
                            v*=N2;
                        }
                    }
                }
            } else {
                // Gray check mode: only show matching stickers
                if(hasColorBlackCheck) {
                    // Has black checks: only highlight those colors
                    for(int i=0;i<NC;i++) {
                        if(Cube[i]!=0 && hmask[Cube[i]]>0) {
                            HighLighted[i]=true;
                        }
                    }
                } else {
                    // No black checks: highlight all, then exclude unchecked colors
                    for(int i=0;i<NC;i++) {
                        if(Cube[i]!=0 && hmask[Cube[i]] >= 0) {  // Gray or black (not uncheck)
                            HighLighted[i]=true;
                        }
                    }
                }
            }

            // Apply ncolMask filtering on top of color filtering
            if(ncolMask != null) {
                bool hasNColBlackCheck = HasBlackCheck(ncolMask, 1, 7);

                for(int i=0;i<NC;i++) {
                    if(Cube[i]==0) continue;
                    int ncol = StkNCols[i];

                    if(hasNColBlackCheck) {
                        // If there are black checks, only keep those with black check
                        if(ncolMask[ncol] <= 0) {
                            HighLighted[i]=false;
                        }
                    } else {
                        // No black checks: uncheck means exclude
                        if(ncolMask[ncol] < 0) {
                            HighLighted[i]=false;
                        }
                    }
                }
            }
        }

        internal void HighlightAll() {
            HighlightAll(null);
        }

        internal void HighlightAll(int[] ncolMask) {
            if(ncolMask == null) {
                HighLighted.SetAll(true);
                for(int i=0;i<NC;i++) {
                    if(Cube[i]==0) HighLighted[i]=false;
                }
            } else {
                bool hasNColBlackCheck = HasBlackCheck(ncolMask, 1, 7);

                for(int i=0;i<NC;i++) {
                    if(Cube[i]==0) {
                        HighLighted[i] = false;
                        continue;
                    }

                    int ncol = StkNCols[i];
                    HighLighted[i] = hasNColBlackCheck ? (ncolMask[ncol] > 0) : (ncolMask[ncol] >= 0);
                }
            }
        }

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
                    if(Seq.Length<lseq) Seq=new short[2*lseq];
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
                                short c=0;
                                if(cc=="m[") c=-2;
                                else if(cc=="m]") c=-1;
                                else c=short.Parse(cc);
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

        int dnorm(int v) { return v==0 ? 1 : v==N+1 ? N : v; }
        public bool CheckStickerSet(int[] set,int lset) {
            int[,] M=new int[lset,D];
            for(int i=0;i<lset;i++) {
                int v=set[i];
                for(int j=0;j<D;j++) {
                    M[i,j]=(v%N2);
                    v/=N2;
                }
            }
            for(int i=0;i<D;i++) {
                for(int j=0;j<=i;j++) {
                    bool q1=false,q2=false;
                    for(int k=0;k<lset;k++) {
                        if(M[k,i]!=M[k,j]) q1=true;
                        if(M[k,i]+M[k,j]!=N+1) q2=true;
                    }
                    if(!q2 || (i!=j && !q1)) return false;
                }
            }
            return true;
        }

        public int[] CmpStickerSet(int[] setMacro,int[] setObj,int lset) {
            int[] res=new int[D+1];
            int[,] M1=new int[lset,D];
            int[,] M2=new int[lset,D];
            for(int i=0;i<lset;i++) {
                int v1=setMacro[i];
                int v2=setObj[i];
                for(int j=0;j<D;j++) {
                    M1[i,j]=(v1%N2); v1/=N2;
                    M2[i,j]=(v2%N2); v2/=N2;
                }
            }
            for(int i=0;i<D;i++) {
                bool qs=false;
                for(int j=0;j<D;j++) {
                    bool q1=true,q2=true;
                    for(int m=0;m<lset;m++) {
                        if(M1[m,i]!=M2[m,j]) q1=false;
                        if(M1[m,i]+M2[m,j]!=N+1) q2=false;
                    }
                    if(q1) { res[i+1]=j+1; qs=true; break; }
                    if(q2) { res[i+1]=-(j+1); qs=true; break; }
                }
                if(!qs) return null;
            }
            return res;
        }

        public void ApplyMacro(int[] map,short[] macro,int lmacro,bool qrev) {
            StartMacro();
            if(qrev) {
                for(int i=lmacro-1;i>=0;i--) {
                    int code=macro[i];
                    int m=code&((1<<N)-1);
                    code>>=N;
                    Twist(map[code/D/D+1],map[(code%D)+1],map[(code/D)%D+1],reverse(m));
                }
            } else {
                for(int i=0;i<lmacro;i++) {
                    int code=macro[i];
                    int m=code&((1<<N)-1);
                    code>>=N;
                    Twist(map[code/D/D+1],map[(code/D)%D+1],map[code%D+1],reverse(m));
                }
            }
            StopMacro();
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
        public bool CheckCube() {
            byte[] F=new byte[2*D];
            for(int i=0;i<NC;i++) {
                if(Cube[i]==0) continue;
                int v=i;
                for(int j=0;j<D;j++) {
                    int k=v%N2; v/=N2;
                    if(k==0) {
                        if(F[j]!=0 && F[j]!=Cube[i]) return false;
                        F[j]=Cube[i];
                        break;
                    }
                    if(k==N+1) {
                        if(F[j+D]!=0 && F[j+D]!=Cube[i]) return false;
                        F[j+D]=Cube[i];
                        break;
                    }
                }
            }
            return true;
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
