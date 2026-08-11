using System;
using System.Collections.Generic;

namespace _3dedit {

    public partial class Cube7D {
        private byte BuildOrbitKind(int cellIndex,int nst) {
            int orbitKind;
            if(nst == 1 && TryGetChiralOrbitKind(cellIndex, out orbitKind))
                return (byte)orbitKind;
            return OrbitKindNormal;
        }

        private bool TryGetChiralOrbitKind(int cellIndex, out int orbitKind) {
            orbitKind = OrbitKindNormal;

            int[] coords = new int[D];
            int negCount = 0;
            int inversionCount = 0;
            int t = cellIndex;
            for(int axis = 0; axis < D; axis++) {
                int p = t % N2;
                t /= N2;

                // Centered lattice coordinate for the cubie position on this axis.
                int coord = 2 * p - (N + 1);
                if(coord == 0) return false;

                int absCoord = Math.Abs(coord);
                for(int prev = 0; prev < axis; prev++) {
                    int prevAbs = Math.Abs(coords[prev]);
                    if(prevAbs == absCoord) return false;
                    if(prevAbs > absCoord) inversionCount++;
                }

                if(coord < 0) negCount++;
                coords[axis] = coord;
            }

            orbitKind = ((negCount + inversionCount) & 1) == 0 ? OrbitKindPositive : OrbitKindNegative;
            return true;
        }

        // Return all unique orbit keys present in the current puzzle (for UI and filters)
        public int[] GetAllOrbitKeys() {
            if(!_orbitKeysDirty && _cachedOrbitKeys != null)
                return _cachedOrbitKeys;

            HashSet<int> set=new HashSet<int>();
            for(int i=0;i<NC;i++)
                if(Cube[i]!=0) set.Add(GetOrbitKey(i));
            _cachedOrbitKeys=new int[set.Count];
            set.CopyTo(_cachedOrbitKeys);
            _orbitKeysDirty=false;
            return _cachedOrbitKeys;
        }

        private int GetOrbitKey(int cellIndex) {
            return BuildOrbitKey(OrbitSig[cellIndex], OrbitKind[cellIndex]);
        }

        public static int BuildOrbitKey(ushort orbitSig,int orbitKind) {
            return (orbitSig & OrbitSigMask) | ((orbitKind & 0x3) << OrbitKindShift);
        }

        public static ushort GetTierSigFromOrbitKey(int orbitKey) {
            return (ushort)(orbitKey & OrbitSigMask);
        }

        public static int GetOrbitKind(int orbitKey) {
            return (orbitKey & OrbitKindMask) >> OrbitKindShift;
        }

        public static int GetStkNColsFromOrbitKey(int orbitKey) {
            return orbitKey&7;
        }

        public static string FormatOrbitKeyLabel(int orbitKey, int maxTier) {
            ushort sig = GetTierSigFromOrbitKey(orbitKey);
            int[] tiers = DecodeNonStickerTiers(sig, maxTier);
            string label = "[" + string.Join(",", Array.ConvertAll(tiers, t => t.ToString())) + "]";
            int kind = GetOrbitKind(orbitKey);
            if(kind == OrbitKindPositive) return "+" + label;
            if(kind == OrbitKindNegative) return "-" + label;
            return label;
        }

        // Decode non-sticker tier counts for display: [count_1, count_2, ...]
        public static int[] DecodeNonStickerTiers(ushort sig,int maxTier) {
            int[] r=new int[maxTier];
            for(int k=1;k<=maxTier;k++) r[k-1]=(sig>>(k*3))&7;
            return r;
        }
    }
}
