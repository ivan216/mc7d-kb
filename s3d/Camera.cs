using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit{

	public delegate	void OnPositionChangedHandler(Camera cam);

	public	enum	ECameraPosition {
		PositionUndef = -1,
		PositionLeft,
		PositionRight,
		PositionUp,
		PositionDown,
		PositionFar,
		PositionNear,
		PositionBird
	}
	public class Camera	:	IDXObject {
		protected Vector3 m_pointToLookAt;
		protected Vector3 m_cameraPosition;
		protected Vector3 m_upVector;
		protected Vector3 m_Center; // for dynamic_znear 
		protected float m_angle;

		//		public ushort width;
		//		public ushort height;

		protected float o_width=100.0f;
		protected float o_height=100.0f;
		protected float o_width_nozoom=100.0f;
		protected float o_height_nozoom=100.0f;

		private float m_W = 100000;

		public bool	 lock_dist = true;
		public float max_dist = 1e5f;

		//		public Matrix	matProj;
		//		public Matrix	matView;
		//		public Viewport	Viewport;

		public float OrtParam {
			get {
				return (this.o_width + this.o_height);
			}
		}

		public Camera() {
			m_angle=(float)Math.PI/4;
			DXUtils.SetPoint(ref m_pointToLookAt, 0,0,-1000);
			DXUtils.SetPoint(ref m_cameraPosition, 0,0,0);
			DXUtils.SetPoint(ref m_upVector,0,1,0);
		}
		public Vector3 PointToLookAt{
			get{ return m_pointToLookAt; }
			set { 
				m_pointToLookAt = value; 
				if (m_OnPositionChanged != null)
					m_OnPositionChanged(this);
			}
		}
		public Vector3 CameraPosition{
			get{ return m_cameraPosition; }
			set {
				m_cameraPosition = value;
				if (m_OnPositionChanged != null)
					m_OnPositionChanged(this);
			}
		}
		public Vector3 UpVector{
			get{ return m_upVector; }
			set {
				m_upVector = value;
				if (m_OnPositionChanged != null)
					m_OnPositionChanged(this);
			}
		}

		public Vector3 Center {
			get{ return m_Center; }
			set {
				m_Center = value;
			}
		}

		public void SetPoints(Vector3 _pointToLookAt, Vector3 _cameraPosition, Vector3 _upVector) {
			m_pointToLookAt=_pointToLookAt;
			m_cameraPosition=_cameraPosition;
			m_upVector=_upVector;
			if (m_OnPositionChanged != null)
				m_OnPositionChanged(this);
		}

		public float W {
			set {	
				m_W = value;
				pCam = null;
			}
			get {
			 	return (m_W);	}
		}

		public void SetAreaO(float _width,float _height) {
			o_width=_width;
			o_height=_height;
			o_width_nozoom=_width;
			o_height_nozoom=_height;
		}

		public float Angle {
			get{ return this.m_angle; }
			set{
				if(value < Math.PI/180) return;
				if(value >= Math.PI) return;
				this.m_angle = value;
			}
		}
		public void ResetZoom() {
			o_width = o_width_nozoom;
			o_height = o_height_nozoom;
			m_angle=(float)Math.PI/4;
		}

		public void ZoomO(float d, bool zmove){
			float w = this.o_width +d;
			if(w < 0.0) return;
			float h = this.o_height +d;
			if(h < 0.0) return;
			this.o_width = w;
			this.o_height = h;
			if (zmove) {
				o_width_nozoom=w;
				o_height_nozoom=h;
			}
		}
		public float Distance {
			get {
				Vector3 dv=DXUtils.Minus(m_pointToLookAt,m_cameraPosition);
				return dv.Length();
			}
		}
		public void ChangeDistance(bool qTarget,float ddis){
			if(ddis==0) return;
			Vector3 tmp = DXUtils.Minus(m_pointToLookAt, m_cameraPosition);
			Vector3 dv=DXUtils.Mul(tmp,ddis/this.Distance);
			Vector3 _oldpt = m_pointToLookAt;
			Vector3 _oldcp = m_cameraPosition; 
			if(qTarget)
				DXUtils.PlusE(ref m_pointToLookAt, ref dv);
			else
				DXUtils.MinusE(ref m_cameraPosition,ref dv);
			if (lock_dist) {
				float _dst = Math.Abs(m_cameraPosition.Z);
				if (_dst > max_dist) {
					m_pointToLookAt = _oldpt;
					m_cameraPosition = _oldcp;
				}
			}
		}
		public void Slide(float dx,float dy,float dz){
			if(dx==0 && dy==0 && dz==0) return;
			Vector3 sh=new Vector3();
			GetShift(ref sh,this,dx,dy,dz);
			DXUtils.PlusE(ref m_pointToLookAt, ref sh);
			DXUtils.PlusE(ref m_cameraPosition,ref sh);
			//qSceneChanged=true;
		}

		public void Shift(Vector3 d) {
			if (	(d.X == 0)
				&&	(d.Y == 0)
				&&	(d.Z == 0))
				return;
			DXUtils.PlusE(ref m_pointToLookAt, ref d);
			DXUtils.PlusE(ref m_cameraPosition,ref d);
			//qSceneChanged=true;
		}
		public static	void GetShift(ref Vector3 res,Camera cam,float dx,float dy,float dz){
			Vector3 dv=DXUtils.Minus(cam.PointToLookAt, cam.CameraPosition);
			Vector3 vz, vx;
			vz = Vector3.Normalize(dv);
			vx = Vector3.Cross(vz, cam.UpVector);
			res=DXUtils.Mul(vx,dx);
			Vector3 tmp = DXUtils.Mul(cam.UpVector,dy);
			DXUtils.PlusE(ref res, ref tmp);
			tmp = DXUtils.Mul(vz,dz);
			DXUtils.PlusE(ref res,ref tmp);
		}

		public	enum	RotateMode {
			XY_rotate,
			Z_rotate,
			XYZ_rotate
		}

		public static void GetRotate(ref Vector3 res, ref float ang,Camera cam, float x,float y,float dx,float dy, RotateMode mode) {
			if(dx==0 && dy==0){
				ang=0;
				res.X=0;res.Y=0;res.Z=1;
				return;
			}
			float r=(float)Math.Sqrt(x*x+y*y);
			if(r>1){
				x/=r; y/=r;
			}
			Vector3 RB = new Vector3();
			float q=x*dy-y*dx;

			switch(mode) {
				case RotateMode.XY_rotate:
					GetShift(ref RB,cam,-dy,dx,0);
					break;
				case RotateMode.Z_rotate:
					GetShift(ref RB,cam,0,0,q/r);
					break;
				default:
					GetShift(ref RB,cam,-dy+q*x,dx+q*y,q);
					break;
			}
			ang=RB.Length();
			res = Vector3.Normalize(RB);
		}
		public void RotateView(bool qTarget, ref Vector3 vec, float ang) {
			if(ang==0) return;
			if(qTarget){
				ang=-ang;
			}
			Vector3 dv=DXUtils.Minus(m_pointToLookAt,m_cameraPosition);
			float Len=dv.Length();

			Matrix rot= Matrix.RotationAxis(vec,ang);

			Vector3 temp = Vector3.TransformCoordinate(dv, rot);
			dv = temp;
			temp = Vector3.TransformCoordinate(m_upVector, rot);
			m_upVector = temp;
			if(qTarget){
				m_pointToLookAt=m_cameraPosition;
				DXUtils.PlusE(ref m_pointToLookAt,ref dv);
			}
			else m_cameraPosition=DXUtils.Minus(m_pointToLookAt,dv);

			if (m_OnPositionChanged != null)
				m_OnPositionChanged(this);
		}

		OnPositionChangedHandler	m_OnPositionChanged = null;
		public	OnPositionChangedHandler	OnPositionChanged {
			get {
				return m_OnPositionChanged;
			}
			set {
				m_OnPositionChanged = value;
			}
		}

		public void TargetTo(ref Vector3 pt){
			this.m_pointToLookAt=pt;
			this.ChangeDistance(false,-this.Distance/2.0f);
		}

		void SetAside(RPoint min, RPoint max, bool chPos) {
			double Wx=max.X-min.X, Wy=max.Y-min.Y;
			double cx=(max.X+min.X)/2,cy=(max.Y+min.Y)/2,cz=(max.Z+min.Z)/2;
			double W=Math.Max((max.Z-min.Z)/2,Math.Max(Wx,Wy));
			double R=MyMath.pyth(Wx,Wy,max.Z-min.Z)/2;
			double zz=W*3;
			Vector3 newpoint = new Vector3((float)cx,(float)cy,(float)cz);
			Vector3 neworigin = new Vector3((float)cx,(float)cy,(float)(cz+zz));
			Vector3 newup = new Vector3(0,1,0);
			if(chPos) this.SetPoints(newpoint,neworigin,newup);
			this.W = (float)R;
			this.m_Center=newpoint;
			this.SetAreaO((float)Wx*1.5f, (float)Wy*1.5f);
		}

		void SetToZero(RPoint min, RPoint max, bool chPos) {
			this.SetAside(min, max, chPos);
			if(!chPos) return;
			Vector3 newpoint = new Vector3(0,0,this.W);
			Vector3 neworigin = new Vector3(0,0,0);
			Vector3 newup = new Vector3(0,1,0);
			this.SetPoints(newpoint,neworigin,newup);			
			SetAreaO(o_width/6, o_height/6);
		}

		public	bool	Park(ArrayList dxobj, ECameraPosition position, bool center) {
			bool	_boundsfound = false;
			RPoint min=new RPoint(float.MaxValue,float.MaxValue,float.MaxValue);
			RPoint max=new RPoint(float.MinValue,float.MinValue,float.MinValue);
			foreach (IDXObject _obj in dxobj) {
				RPoint _tmin;
				RPoint _tmax;
				if (_obj.GetBounds(out _tmin, out _tmax)) {
					if(min.X > _tmin.X) min.X = _tmin.X;
					if(min.Y > _tmin.Y) min.Y = _tmin.Y;
					if(min.Z > _tmin.Z) min.Z = _tmin.Z;
					if(max.X < _tmax.X) max.X = _tmax.X;
					if(max.Y < _tmax.Y) max.Y = _tmax.Y;
					if(max.Z < _tmax.Z) max.Z = _tmax.Z;
					_boundsfound = true;
				}
			}
			if (_boundsfound) {
				bool chPos=(position!=ECameraPosition.PositionUndef);
				if (center)
					SetToZero(min, max, chPos);
				else
					SetAside(min, max, chPos);
				if(!chPos){
					m_OnPositionChanged(this);
					return true;
				}

				Matrix rotate = Matrix.Identity;
				Matrix rotateup = Matrix.Identity;
				Vector3 vy = m_upVector;
				Vector3	vz = m_cameraPosition - m_pointToLookAt;
				vz.Normalize();
				Vector3 vx = Vector3.Cross(center ? vz*-1 : vz, m_upVector);
				switch (position) {
					case ECameraPosition.PositionFar:
						rotate = Matrix.RotationAxis(vy, (float)Math.PI);
						break;
					case ECameraPosition.PositionLeft:
						rotate = Matrix.RotationAxis(vy, (float)Math.PI/2);
						break;
					case ECameraPosition.PositionRight:
						rotate = Matrix.RotationAxis(vy, ((float)Math.PI/2)*3);
						break;
					case ECameraPosition.PositionUp:
						rotate = Matrix.RotationAxis(vx, (float)Math.PI/2);
						rotateup = rotate;
						break;
					case ECameraPosition.PositionDown:
						rotate = Matrix.RotationAxis(vx, ((float)Math.PI/2)*3);
						rotateup = rotate;
						break;
				}
				if (center)
					m_pointToLookAt = Vector3.TransformCoordinate(m_pointToLookAt - m_cameraPosition, rotate) + m_cameraPosition;
				else
					m_cameraPosition = Vector3.TransformCoordinate(m_cameraPosition - m_pointToLookAt, rotate) + m_pointToLookAt;
				m_upVector = Vector3.TransformCoordinate(m_upVector, rotateup);
				m_OnPositionChanged(this);
			}
			return _boundsfound;
		}

		bool	m_Active = false;
		public	bool	Active {
			get {
				return m_Active;
			}
			set {
				m_Active = value;
			}
		}

		public Camera Clone() {
			Camera _cam = new Camera();
			_cam.SetPoints(PointToLookAt, CameraPosition, UpVector);
			_cam.SetAreaO(o_width, o_height);
			_cam.W = this.W;
			_cam.Angle = this.Angle;
			_cam.lock_dist = this.lock_dist;
			_cam.m_Center = this.m_Center;
			_cam.RenderMul = this.RenderMul;
			return _cam;
		}
		Mesh[]	pCam = null;

		float	m_mul = 10;
		public	float	RenderMul {
			set {
				m_mul = value;
				pCam = null;
			}
			get {
				return m_mul;
			}
		}

		bool	m_ShowPOV =	true;
		public	bool	ShowPOV {
			get {
				return m_ShowPOV;
			}
			set {
				m_ShowPOV = value;
			}
		}

		public	Color	m_FGColor	=	Color.Blue;

		bool	m_bVisible = true;
		public bool Visible {
			set {
				m_bVisible = value;
			}
			get {
				return m_bVisible;
			}
		}


		public void Render(S3DirectX d3dDevice) {
			float _rad = this.W/200*m_mul;
			if (	(!m_bVisible)
				||	(_rad <= 0))
				return;
			if (pCam == null) {
				pCam = new Mesh[5];
				pCam[0] = Mesh.Box(d3dDevice.Renderer,_rad,2*_rad,3*_rad);
				pCam[1] = Mesh.Cylinder(d3dDevice.Renderer,_rad/2,_rad,_rad,8,12);
				pCam[2] = Mesh.Cylinder(d3dDevice.Renderer,_rad,_rad,_rad/2,8,12);
				pCam[3] = Mesh.Cylinder(d3dDevice.Renderer,_rad,_rad,_rad/2,8,12);
				pCam[4] = Mesh.Sphere(d3dDevice.Renderer, _rad/2,8,12);
			}
			d3dDevice.SetupObjectMaterial(m_FGColor);
			if (!this.Active) {
				Vector3	vz = m_pointToLookAt - m_cameraPosition;
				vz.Normalize();
				Vector3 vx = Vector3.Cross(vz, m_upVector);
				Matrix mRotate = Matrix.Identity;
				mRotate.M11 = vx.X;	mRotate.M12 = vx.Y;	mRotate.M13 = vx.Z;
				mRotate.M21 = m_upVector.X;	mRotate.M22 = m_upVector.Y;	mRotate.M23 = m_upVector.Z;
				mRotate.M31 = vz.X;	mRotate.M32 = vz.Y;	mRotate.M33 = vz.Z;
				Matrix mMove = mRotate*Matrix.Translation(m_cameraPosition);
				d3dDevice.SetupWorldMatrix(mMove);
				pCam[0].DrawSubset(0);
				mMove = Matrix.Translation(0,0,2*_rad)*mRotate*Matrix.Translation(m_cameraPosition);
				d3dDevice.SetupWorldMatrix(mMove);
				pCam[1].DrawSubset(0);
				mMove =		Matrix.Translation(_rad,2*_rad,0)
					*	Matrix.RotationY((float)Math.PI/2)
					*	mRotate
					*	Matrix.Translation(m_cameraPosition);
				d3dDevice.SetupWorldMatrix(mMove);
				pCam[2].DrawSubset(0);
				mMove =		Matrix.Translation(-_rad,2*_rad,0)
					*	Matrix.RotationY((float)Math.PI/2)
					*	mRotate
					*	Matrix.Translation(m_cameraPosition);
				d3dDevice.SetupWorldMatrix(mMove);
				pCam[3].DrawSubset(0);
			}
			if (ShowPOV) {
				Matrix	mMove = Matrix.Translation(m_pointToLookAt);
				d3dDevice.SetupWorldMatrix(mMove);
				pCam[4].DrawSubset(0);
			}
			d3dDevice.SetupWorldMatrix(Matrix.Identity);
		}

		//		bool	IsPicked(Camera camera, int x, int y)
		//		{
		//			Vector3 vPickRayDir, vPickRayOrig;
		//			if (	(pCam != null)
		//				&&	(camera.DirAndOrig(out vPickRayDir, out vPickRayOrig,x,y)))
		//				return (Geometry.SphereBoundProbe(m_cameraPosition, 2.0f*rad, vPickRayOrig, vPickRayDir));
		//			return false;
		//		}

		
		static public void ProcessMouseMove(S3DirectX scene,EAction action, ETarget tg, Point pt,int DX,int DY, OnAction proc) {
			Camera camera = scene.Camera;

			Vector3 vec = new Vector3();
			Vector3 shft = new Vector3();
			int wid=scene.width;
			int hgt=scene.height;
			float acf=3.0F/hgt; // pi/3 for half of screen

			float dist=camera.Distance;
			float ddis=dist*(float)(Math.Pow(0.5,DY/200.0)-1);
			float ang=0.0F;			
			
			switch	(action) {
				case EAction.ActionRotate: 
				case EAction.ActionZRotate:{
					ang=0.0F;
					float _dist=dist;
					float dx=(float)(DX*acf);
					float dy=(float)(DY*acf);
					float x0=(float)(pt.X-wid/2)*acf;
					float y0 = (float)(pt.Y-hgt/2)*acf;
					Camera.GetRotate(ref vec,ref ang,camera, x0,y0,dx,dy, 
						action==EAction.ActionRotate ? RotateMode.XY_rotate : RotateMode.Z_rotate);
					if(action==EAction.ActionRotate) ang*=camera.Angle;
					if(tg==ETarget.TargetCamera){
						camera.RotateView(false,ref vec,ang);
					}else{
						Vector3 ctr=camera.Center;
						proc(EAction.ActionRotate,ref ctr,ref vec,ang);
					}
					break;
				}
				case EAction.ActionPan: {
					float _dist=dist;
					acf=camera.Angle/hgt; // maxphi/size ???
					float dx=(float)(DX*acf);
					float dy=(float)(DY*acf);
					if(tg==ETarget.TargetCamera){
						float x0=(float)(pt.X-wid/2)*acf;
						float y0 = (float)(pt.Y-hgt/2)*acf;
						Camera.GetRotate(ref vec,ref ang,camera, x0,y0,dx,dy, RotateMode.XY_rotate);
//						ang*=camera.Angle/3.0F;
						camera.RotateView(true,ref vec,ang);
					}else{
						Camera.GetShift(ref shft,camera,dx*_dist,dy*_dist,0);
						proc(EAction.ActionSlide,ref shft,ref shft,0);
					}
					break;
				}
				case EAction.ActionZoom:{
					if(DY==0.0F) return;
					float height = scene.height;
					if(tg!=ETarget.TargetCamera){
						proc(EAction.ActionZoom,ref shft,ref shft,DY/height);
						return;
					}
					ang = camera.Angle;
					ang += ang*DY/height;
					camera.Angle = ang;
					break;
				}
				case EAction.ActionSlide:
					if(tg==ETarget.TargetCamera){
						camera.Slide(0,0,ddis);
					}else{
						Camera.GetShift(ref shft,camera,0,0,-ddis);
						proc(EAction.ActionSlide,ref shft,ref shft,0);
					}
					break;
				case EAction.ActionXYSlide:{
					acf=camera.Angle/hgt; // maxphi/size ???
					float dx=(float)(DX*acf)*dist;
					float dy=(float)(DY*acf)*dist;
					if(tg==ETarget.TargetCamera){
						camera.Slide(dx,dy,0);
					}else{
						Camera.GetShift(ref shft,camera,-dx,-dy,0);
						proc(EAction.ActionSlide,ref shft,ref shft,0);
					}
					break;
				}
				case EAction.ActionMoveTo:{
					if(tg==ETarget.TargetCamera){
						camera.ChangeDistance(false,-ddis); // 8 times for height
					}else{
						Camera.GetShift(ref shft,camera,0,0,-ddis);
						proc(EAction.ActionMoveTo,ref shft,ref shft,ddis);
					}
					break;
				}
				case EAction.ActionClick:
				case EAction.ActionShiftClick:
				case EAction.ActionCtrlShiftClick:
				case EAction.ActionCtrlClick:
				case EAction.ActionCtrlRightClick:{
					scene.DirAndOrig(out vec,out shft,pt.X,pt.Y);
					vec.Normalize();
					proc(action,ref shft,ref vec,0);
					break;
				}
			}
			scene.SceneChanged=true;
		}

		public	bool GetBounds(out RPoint min, out RPoint max) {
			min = new RPoint(0,0,0);
			max = new RPoint(0,0,0);
			return false;
		}

		public void CleanUp() {
		}

		public bool NeedLightning {
			get	{	return true;	}
		}
	}
}
