using System;
using System.Drawing;
using System.Collections;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit {
    public class CubeObj:IDXObject {
        ArrayList Meshes;
        public static uint[] Colors=new uint[]{0xffffffff,
            0xff804000,0xff800040,0xff400080,0xff004080,0xff00C040,0xff408000,0xff404040,
            0xff8080ff,0xff00ffff,0xff80ff80,0xffffff00,0xffff8080,0xffff00ff,0xffc0c0c0};
        public static int Transparency=32;
        int[] cver=new int[] { 0,1,2,2,1,3, 0,4,1,1,4,5, 0,2,4,4,2,6, 5,4,6,5,6,7, 6,2,3,6,3,7,3,1,5,3,5,7 };

        byte[] col;
        float[,] coord;
        int[] map;
        int nstk;
        byte[] stkNcol;
        BitArray hmask;

        public CubeObj() { }

        public void Dispose() {
            if(Meshes!=null) foreach(Mesh m in Meshes) m.Dispose();
            Meshes=null;
        }

        public void SetCoords(byte[] _col,int[] _map,float[,] _coord,byte[] _stkNcol,BitArray _hmask,int []_ncmask,int _nstk) {
            Dispose();
            col=_col;
            map=_map;
            coord=_coord;
            nstk=_nstk;
            stkNcol=_stkNcol;
            hmask=_hmask;
            // NColMask no longer used - keeping parameter for compatibility
        }


        public void SetMesh(Device dev) {
            if(Meshes!=null) return;
            Meshes=new ArrayList();
            if(col==null) {
                return;
            }

            int nv=5000;
            MYVERTEX[] vert=new MYVERTEX[nv*8];
            ushort[] fac=new ushort[nv*36];

            int smask=(Transparency<<24)|0xffffff;

            int p=0;
            for(int a=0;a<nstk;a++) {
                int mm=map[a];
                int c=(int)Colors[col[mm]];
                if(!hmask[mm]) c&=smask;

                if(p==nv) {
                    Mesh m=(Mesh)S3DirectX.Create3DMesh(dev,p*8,vert,p*36,fac);
                    Meshes.Add(m);
                    p=0;
                }
                for(int i=0;i<8;i++) {
                    float x=coord[a,3*i];
                    float y=coord[a,3*i+1];
                    float z=coord[a,3*i+2];
                    vert[8*p+i].p.X=x;
                    vert[8*p+i].p.Y=y;
                    vert[8*p+i].p.Z=z;
                    float dx=x-coord[a,24];
                    float dy=y-coord[a,25];
                    float dz=z-coord[a,26];
                    double dd=MyMath.pyth(dx,dy,dz);
                    vert[8*p+i].n.X=(float)(dx/dd);
                    vert[8*p+i].n.Y=(float)(dy/dd);
                    vert[8*p+i].n.Z=(float)(dz/dd);
                    vert[8*p+i].dc=c;
                }
                for(int i=0;i<36;i++) fac[36*p+i]=(ushort)(8*p+cver[i]);
                p++;
            }
            if(p>0) {
                Mesh m=(Mesh)S3DirectX.Create3DMesh(dev,p*8,vert,p*36,fac);
                Meshes.Add(m);
            }
        }

        public void Render(S3DirectX d3dDevice) {
            d3dDevice.SetupObjectMaterial(Color.FromArgb(unchecked((int)0x10404040)));
            d3dDevice.Renderer.SetTransform(TransformType.World,Matrix.Identity);
            SetMesh(d3dDevice.DxDevice);

            foreach(Mesh bm in Meshes) {
                bm.DrawSubset(0);
            }
        }

        public bool GetBounds(out RPoint min,out RPoint max) {
            min=new RPoint(-1.5,-1.5,-1.5);
            max=new RPoint(1.5,1.5,1.5);
            return true;
        }

        public void CleanUp() {
            Dispose();
        }

        public bool NeedLightning {
            get { return true; }
        }


        int[,] c_edges=new int[,] { { 0,1 },{ 0,2 },{ 1,3 },{ 2,3 },{ 0,4 },{ 1,5 },{ 2,6 },{ 3,7 },{ 4,5 },{ 4,6 },{ 5,7 },{ 6,7 } };
        public int FindSticker(ref Vector3 pt,ref Vector3 dir) {
            if(coord==null) return -1;
            double dmin=double.MaxValue;
            int res=-1;
            double[,] M=new double[8,3];
            bool[] S=new bool[12];
            for(int i=0;i<nstk;i++) {
                double dx=coord[i,24]-pt.X,dy=coord[i,25]-pt.Y,dz=coord[i,26]-pt.Z;
                double l=dx*dir.X+dy*dir.Y+dz*dir.Z;
                if(l<0 || l>dmin) continue;
                if(MyMath.sqr(l*dir.X-dx)+MyMath.sqr(l*dir.Y-dy)+MyMath.sqr(l*dir.Z-dz)>MyMath.sqr(coord[i,27])) continue;
                for(int j=0;j<8;j++) {
                    M[j,0]=coord[i,3*j]-pt.X;
                    M[j,1]=coord[i,3*j+1]-pt.Y;
                    M[j,2]=coord[i,3*j+2]-pt.Z;
                }
                for(int j=0;j<12;j++) {
                    int a=c_edges[j,0],b=c_edges[j,1];
                    S[j]=(M[a,0]*(M[b,1]*dir.Z-M[b,2]*dir.Y)+M[a,1]*(M[b,2]*dir.X-M[b,0]*dir.Z)+M[a,2]*(M[b,0]*dir.Y-M[b,1]*dir.X))>0;
                }
                if((S[0]&&S[2]&&!S[3]&&!S[1]) || (S[0]&&S[5]&&!S[8]&&!S[4]) || (S[1]&&S[6]&&!S[9]&&!S[4])||
                    (S[8]&&S[10]&&!S[11]&&!S[9]) || (S[3]&&S[7]&&!S[11]&&!S[6]) || (S[2]&&S[7]&&!S[10]&&!S[5])||
                    (!S[0]&&!S[2]&&S[3]&&S[1]) || (!S[0]&&!S[5]&&S[8]&&S[4]) || (!S[1]&&!S[6]&&S[9]&&S[4])||
                    (!S[8]&&!S[10]&&S[11]&&S[9]) || (!S[3]&&!S[7]&&S[11]&&S[6]) || (!S[2]&&!S[7]&&S[10]&&S[5])) {
                    res=i; dmin=l;
                }
            }
            return res;
        }

    }

}
