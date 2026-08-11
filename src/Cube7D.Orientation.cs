using System;

namespace _3dedit {

    public partial class Cube7D {
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
            int f0, m0;
            NormGrip(out f0, out m0);
            if ((m0 & reverse(1)) == 0)
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
    }
}
