using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Linq;


namespace _3dedit {
    
    public partial class Cube7D {
        public int N,D;
        public const int MaxN = 9;
        int NC,N2;
        byte[] Cube,Cube2;
        public BitArray HighLighted;
        public ushort[] OrbitSig;
        public byte[] OrbitKind;
        int[] _cachedOrbitKeys; // cached by GetAllOrbitKeys, invalidated by InitCube
        bool _orbitKeysDirty = true;

        const int OrbitSigMask = 0x7FFF;
        const int OrbitKindShift = 15;
        const int OrbitKindMask = 0x3 << OrbitKindShift;
        const int OrbitKindNormal = 0;
        const int OrbitKindPositive = 1;
        const int OrbitKindNegative = 2;

        int NStk;
        int NStk0,NStk1;
        float[][] Coord;
        public int[] StkMap; // sticker to Cube
        public static double FSep=0.5,FExt=1,BSize=0.8,SSize=0.6;

        public int[] Orient;  // +-side
        int[] DimStat; // 0 - center, 1 - main, 2 - secondary

        public int[] Gripped;
        public Keybindings.Twist partialTwist;
        public PartialTwist3c partialTwist3c;
        public HashSet<Keybindings.Layer> LayerOverrides;

        public int[] Seq;
        public int LSeq,LPtr,LShuffle;
        public int NTwists;
        public long CTime;

        public Cube7D() { }

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

            Coord=new float[NStk][];
            for(int _i=0;_i<NStk;_i++) Coord[_i]=new float[28];
            StkMap=new int[NStk];
            HighLighted=new BitArray(NC);
            OrbitSig=new ushort[NC];
            OrbitKind=new byte[NC];

            Orient=new int[7];
            for(int i=0;i<D;i++) Orient[i]=i+1;
            DimStat=new int[7];
            for(int i=1;i<D;i++) DimStat[i]=i<4 ? 1 : 2;

            Gripped = new int[2] { -1, 1 };
            partialTwist = new Keybindings.Twist();
            partialTwist.fromAxis = null;
            partialTwist.toAxis = null;
            partialTwist3c = new PartialTwist3c();
            LayerOverrides = new HashSet<Keybindings.Layer>();

            SetCoords();
            InitCube();
            InitStkMap();
            LSeq=LPtr=LShuffle=0;
            Seq=new int[10000];
            CTime=0;
        }

        int dnorm(int v) { return v==0 ? 1 : v==N+1 ? N : v; }

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

    }
}
