namespace _3dedit {

    public partial class Cube7D {
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
            int codeF0=f0,codeF1=f1,codeF2=f2,codeMask=m0;
            f0--; f1--; f2--;
            MakeTwist(f0,f1,f2,m0);

            int code=(((f0*D+f1)*D+f2)<<N)+m0;
#if false
            int rcode=(((f0*D+f2)*D+f1)<<N)+m0;

            if(LPtr>LShuffle && Seq[LPtr-1]==rcode) LPtr--;
            else {
                CheckSeqLen();
                Seq[LPtr++]=code;
            }
#else
            CheckSeqLen();
            Seq[LPtr++]=code;
#endif
            NTwists++;
            LSeq=LPtr;
            if(TwistExecuted!=null) TwistExecuted(codeF0,codeF1,codeF2,codeMask);
            return true;
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
                // Normalize negative masks the same way as NormGripMask: -X -> reverse(X)
                int m = item.layerMask;
                if (m < 0) m = reverse(-m);
                res |= m;
            }

            if ((Gripped[1]&reverse(1)) != 0)
            {
                res = reverse(res);
            }

            return res;
        }

        // Get combined mask from LayerOverrides only (ignoring Gripped state)
        // Returns layer 1 mask if no overrides are active
        public int GetLayerOverridesMask()
        {
            if (LayerOverrides.Count == 0) return 1;

            int res = 0;
            foreach (var item in LayerOverrides)
            {
                res |= item.layerMask;
            }
            return res;
        }

        public void NormGrip(out int f0, out int m0)
        {
            f0 = Gripped[0];
            m0 = GetLayerMask();
            if (f0 == -1) return;

            f0 = Orient[f0 - 1];
            if (f0 < 0) f0 = -f0;
            else m0 = reverse(m0);
        }
    }

    // State tracker for Twist3c (3-click twist)
    public class PartialTwist3c {
        public Keybindings.Axis gripAxis;
        public int gripLayerMask;
        public Keybindings.Axis fromAxis;
        public Keybindings.Axis toAxis;
        public int step; // 0=empty, 1=grip set, 2=from set, 3=complete
        public int negativeCount; // Count of negative flags in step 2 and 3

        public PartialTwist3c() {
            Reset();
        }

        public void Reset() {
            gripAxis = null;
            gripLayerMask = 1;
            fromAxis = null;
            toAxis = null;
            step = 0;
            negativeCount = 0;
        }

        public bool IsValid() {
            return step == 3 && gripAxis != null && fromAxis != null && toAxis != null
                && gripAxis.idx != fromAxis.idx && fromAxis.idx != toAxis.idx && gripAxis.idx != toAxis.idx;
        }
    }
}
