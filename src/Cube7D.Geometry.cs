using System;
using System.Collections;

namespace _3dedit {

    public partial class Cube7D {
        static int Pow(int p,int n) {
            int b=1;
            while(--n>=0) b*=p;
            return b;
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

                Coord[u][0]=Coord[u][6]=Coord[u][12]=Coord[u][18]=(float)x0;
                Coord[u][3]=Coord[u][9]=Coord[u][15]=Coord[u][21]=(float)(x0+dx);
                Coord[u][1]=Coord[u][4]=Coord[u][13]=Coord[u][16]=(float)y0;
                Coord[u][7]=Coord[u][10]=Coord[u][19]=Coord[u][22]=(float)(y0+dy);
                Coord[u][2]=Coord[u][5]=Coord[u][8]=Coord[u][11]=(float)z0;
                Coord[u][14]=Coord[u][17]=Coord[u][20]=Coord[u][23]=(float)(z0+dz);

                if(f!=0) {
                    int k=(f-1)/2,dk=2*(f&1)-1;
                    for(int i=0;i<24;i+=3) {
                        float x=Coord[u][i+k];
                        double cf=1+(dk*x+0.5)*FExt;
                        Coord[u][i]*=(float)cf;
                        Coord[u][i+1]*=(float)cf;
                        Coord[u][i+2]*=(float)cf;
                        Coord[u][i+k]=(float)(x+dk*(1+FSep));
                    }
                }
                double xmin=Coord[u][0],xmax=Coord[u][0];
                double ymin=Coord[u][1],ymax=Coord[u][1];
                double zmin=Coord[u][2],zmax=Coord[u][2];
                for(int i=3;i<24;i+=3) {
                    xmin=Math.Min(xmin,Coord[u][i]);
                    xmax=Math.Max(xmax,Coord[u][i]);
                    ymin=Math.Min(ymin,Coord[u][i+1]);
                    ymax=Math.Max(ymax,Coord[u][i+1]);
                    zmin=Math.Min(zmin,Coord[u][i+2]);
                    zmax=Math.Max(zmax,Coord[u][i+2]);
                }
                Coord[u][24]=(float)((xmax+xmin)/2);
                Coord[u][25]=(float)((ymax+ymin)/2);
                Coord[u][26]=(float)((zmax+zmin)/2);
                Coord[u][27]=(float)(MyMath.pyth(zmax-zmin,ymax-ymin,xmax-xmin)/2);
                u++;
            }
        }

        void InitCube() {
            for(int i=0;i<NC;i++) {
                int a=i,b=0;
                int nst=0;
                int t1=0,t2=0,t3=0,t4=0;
                for(int n=1;n<=D;n++) {
                    int p=a%N2; a/=N2;
                    if(p==0 || p==N2-1) {
                        if(b==0) b=p==0 ? n : n+7;
                        else b=-1;
                        nst++;
                    } else if(p==1 || p==N) nst++;
                    else {
                        int tier=(p-1<N-p) ? (p-1) : (N-p);
                        if(tier==1) t1++;
                        else if(tier==2) t2++;
                        else if(tier==3) t3++;
                        else if(tier==4) t4++;
                    }
                }
                Cube[i]=(byte)(b<0 ? 0 : b);
                OrbitSig[i]=(ushort)(nst|(t1<<3)|(t2<<6)|(t3<<9)|(t4<<12));
                OrbitKind[i]=BuildOrbitKind(i,nst);
            }
            HighLighted.SetAll(true);
            _orbitKeysDirty = true;
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
        public int GetStickers(out byte[] col,out int[] map,out float[][] coord,out BitArray hlight) {
            col=Cube;
            map=StkMap;
            coord=Coord;
            hlight=HighLighted;
            return NStk;
        }
    }
}
