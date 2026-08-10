using System;

namespace _3dedit {

    public partial class Cube7D {
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
    }
}
