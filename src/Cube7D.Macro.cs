namespace _3dedit {

    public partial class Cube7D {
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

        public void ApplyMacro(int[] map,int[] macro,int lmacro,bool qrev) {
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
    }
}
