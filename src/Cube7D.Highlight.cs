using System.Collections.Generic;

namespace _3dedit {

    public partial class Cube7D {
        // Apply grip highlight for a single axis: un-highlight cells not matching the layer
        // axisIndex: zero-based axis (0..D-1)
        // layerMask: normalised layer mask (bit per layer, after orientation/inversion processing)
        // excludeMode: if true, un-highlight cells IN the layer instead of OUTSIDE
        public void ApplyGripHighlightToAxis(int axisIndex, int layerMask, bool excludeMode) {
            int m1 = 1 << (N - 1);
            int m = (layerMask & 1) + (layerMask << 1) + ((layerMask & m1) << 2);
            int c0 = Pow(N2, axisIndex);
            for (int i = 0; i < NC; i++) {
                int k = (i / c0) % N2;
                bool inLayer = (m & (1 << k)) != 0;
                if (excludeMode == inLayer) HighLighted[i] = false;
            }
        }

        public void HighLightGrip()
        {
            int f0, m0;
            NormGrip(out f0, out m0);
            if (f0 == -1) return;
            ApplyGripHighlightToAxis(f0 - 1, m0, false);
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

        // Highlight all stickers belonging to pieces in the same orbit as the clicked sticker
        internal void FindStickersByOrbit(int stk) {
            HighLighted.SetAll(false);
            int target=StkMap[stk];
            ushort targetSig=OrbitSig[target];
            byte targetKind=OrbitKind[target];
            for(int i=0;i<NC;i++)
                if(Cube[i]!=0 && OrbitSig[i]==targetSig && OrbitKind[i]==targetKind)
                    HighLighted[i]=true;
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

        internal void FindStickersByMask(int[] hmask,bool cAll,int[] ncolMask) {
            FindStickersByMask(hmask, cAll, ncolMask, null);
        }

        internal void FindStickersByMask(int[] hmask,bool cAll,int[] ncolMask, Dictionary<int,int> orbitMask) {  // array indexed by 1..14, hmask=-1,0,1; ncolMask indexed by 1..7, values: -1=exclude (dark), 0=neutral/gray, 1=include (highlight)
            HighLighted.SetAll(false);

            // Check if any color is selected (black check)
            bool hasColorBlackCheck = HasBlackCheck(hmask, 1, 14);

            int []cmask=new int[16];
            if(cAll) {
                // Precompute stride per dimension: stride[j] = N2^j
                int[] stride=new int[D];
                stride[0]=1;
                for(int j=1;j<D;j++) stride[j]=stride[j-1]*N2;

                for(int i=0;i<NC;i++) {
                    // ── Skip surface cells (any coord is 0 or N2-1) ──
                    int t=i;
                    bool internalCell=true;
                    for(int j=0;j<D;j++) {
                        int w=t%N2;
                        if(w==0 || w==N2-1) { internalCell=false; break; }
                        t/=N2;
                    }
                    if(!internalCell) continue;

                    // ── Build color presence mask for this non-surface cell ──
                    for(int j=1;j<=D;j++) cmask[j]=cmask[j+7]=-1;
                    for(int j=0;j<D;j++) {
                        int s=stride[j];
                        cmask[Cube[i-s]]=1;
                        cmask[Cube[i+s]]=1;
                    }

                    // ── Validate all dimensions against color filters ──
                    bool match=true;
                    for(int j=1;j<=D;j++) {
                        if(hmask[j]*cmask[j]<0 || hmask[j+7]*cmask[j+7]<0) { match=false; break; }
                    }

                    // ── Light up stickers on both sides of each dimension ──
                    if(match) {
                        for(int j=0;j<D;j++) {
                            int s=stride[j];
                            if(Cube[i-s]!=0) HighLighted[i-s]=true;
                            if(Cube[i+s]!=0) HighLighted[i+s]=true;
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
                    int ncol = OrbitSig[i]&7;

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

            // Apply orbitMask filtering: only exclude, never override inclusions
            if(orbitMask != null && orbitMask.Count > 0) {
                for(int i=0;i<NC;i++) {
                    if(Cube[i]==0) continue;
                    int val;
                    if(orbitMask.TryGetValue(GetOrbitKey(i),out val)) {
                        if(val < 0) HighLighted[i]=false;
                    }
                }
            }
        }

        internal void HighlightAll() {
            HighlightAll(null);
        }

        internal void HighlightAll(int[] ncolMask) {
            HighlightAll(ncolMask, null);
        }

        internal void HighlightAll(int[] ncolMask, Dictionary<int,int> orbitMask) {
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

                    int ncol = OrbitSig[i]&7;
                    HighLighted[i] = hasNColBlackCheck ? (ncolMask[ncol] > 0) : (ncolMask[ncol] >= 0);
                }
            }

            // Apply orbitMask filtering: only exclude, never override inclusions
            if(orbitMask != null && orbitMask.Count > 0) {
                for(int i=0;i<NC;i++) {
                    if(Cube[i]==0) continue;
                    int val;
                    if(orbitMask.TryGetValue(GetOrbitKey(i),out val)) {
                        if(val < 0) HighLighted[i]=false;
                    }
                }
            }
        }
    }
}
