using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace _3dedit
{

    public partial class Form1:System.Windows.Forms.Form {
        public string VERSION = "v0.8.4";
		public Form1() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// 
			// dxControl2
			// 
			this.dxControl2 = new _3dedit.DXControl();


			this.dxControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dxControl2.Location = new System.Drawing.Point(0, 28);
            this.dxControl2.Name = "dxControl2";
            this.dxControl2.Size = new System.Drawing.Size(793, 544);
            this.dxControl2.TabIndex = 0;
            this.dxControl2.MouseUp += new MouseEventHandler(MouseUpEvt);
            this.dxControl2.MouseDown += new MouseEventHandler(MouseDownEvt);
            this.dxControl2.MouseMove += new MouseEventHandler(MouseEvt);
            this.dxControl2.PreviewKeyDown += new PreviewKeyDownEventHandler(dxControl2_PreviewKeyDown);
            this.dxControl2.KeyDown += new KeyEventHandler(KeyDownEvt);
            this.dxControl2.KeyUp += new KeyEventHandler(KeyUpEvt);
            this.dxControl2.Leave += new EventHandler(dxControl2_Leave);

			this.panel2.Controls.Add(this.dxControl2);
            this.panel2.Controls.SetChildIndex(this.dxControl2, 0);

            NColMask=new int[8];
            FaceMask=new int[15];
            for(int i=0;i<8;i++) NColMask[i]=0;  // 0 = Indeterminate (normal/neutral)
            for(int i=0;i<15;i++) FaceMask[i]=0;
            for(int i=1;i<=7;i++) { GripAxisMask[i]=0; GripLayerNum[i]=1; }

            m_orbChipMap=new Dictionary<int,CheckBox>();
            m_orbitMaskCache=new Dictionary<int,int>();
            m_pendingOrbitChipStates=new Dictionary<int,CheckState>();

            LoadSettings("MC7D_settings.txt");
            Macros=new CMacroFile(GetDim(),GetSize());
            m_Timer=new System.Threading.Timer(this.UpdateTime,null,0,117);

            // UpdateTime complains about access from another thread if we try this before creating m_timer???
            LoadKeybinds("MC7D_keybinds.txt");
            Keybinds.ActiveLayoutChanged += this.CheckKeybindSet;
            Keybinds.KeybindLayoutsChanged += this.UpdateKeybindMenu;
            this.UpdateKeybindMenu(null, EventArgs.Empty);
            Keybindings.loaded = Keybinds;

            // Add "Keybinds Reference" menu item after "Edit Keybinds"
            _refMenuItem = new ToolStripMenuItem("Keybinds Reference");
            _refMenuItem.Click += (s, me) => ToggleKeybindsReference();
            viewToolStripMenuItem.DropDownItems.Add(_refMenuItem);
            menuStrip1.Dock = DockStyle.Top;
            menuStrip1.CanOverflow = false;
            HookMenuReleaseHandlers(viewToolStripMenuItem);
            InitializeStatusScrollHost();
            InitializeMenuOverflow();
            InitializeSidePanelResize();
            UpdateMenuOverflowLayout();
            panel1.VisibleChanged += (s, me) => { UpdateMenuOverflowLayout(); RefreshStatusScrollHost(); };
            panel2.SizeChanged += (s, me) => { UpdateMenuOverflowLayout(); RefreshStatusScrollHost(); };

            // Wire up macro hotkey execution
            Keybindings.ExecuteMacroById = ExecuteMacroByIdCmd;

            // Block wheel on TrackBar/NumericUpDown; redirect to parent panel for scrolling
            Application.AddMessageFilter(new WheelGuard(this));
            // Catch ALL key up/down messages before any control filters them
            Application.AddMessageFilter(new KeybindsRefreshFilter(this));
            this.Shown += (s, me) =>
            {
                CaptureInitialSidePanelWidth();
                // Ensure the DirectX control has focus at startup so keybinds work immediately
                dxControl2.Focus();
            };
            // Click sidebar background → move focus away from sidebar controls
            panel1.MouseDown += (s, me) => { if (!IsSidePanelResizeHit(me.Location)) dxControl2.Focus(); };

            // Release grip/key state when menu is opened (dxControl2 loses focus
            // but keyboard events may not fire cleanly during menu navigation).
            menuStrip1.MenuActivate += (s, me) => ReleaseAllKeyboardState();

        }

        Cube7D Cube;
        CubeObj CubeView;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

#if false
        void LoadScene(){
			OpenFileDialog sf=new OpenFileDialog();
			sf.DefaultExt=".scn";
			sf.Filter="Сцена (*.scn)|*.scn";
			sf.InitialDirectory=Application.StartupPath+"\\scenes";
			if(sf.ShowDialog()==DialogResult.OK){
				NewScene();
				LoadScene(sf.FileName);
				sceneName=System.IO.Path.GetFileNameWithoutExtension(sf.FileName);
				foreach(MeshObj M in m_objList){
					dxControl2.AddMesh(M);
				}
			}
			dxControl2.ParkCamera(true);
		}

		void SaveScene(){
			SaveFileDialog sf=new SaveFileDialog();
			sf.DefaultExt=".scn";
			sf.Filter="Сцена (*.scn)|*.scn";
			sf.InitialDirectory=Application.StartupPath+"\\scenes";
			if(sceneName!=null){
				sf.FileName=sceneName;
				sf.OverwritePrompt=false;
			}
			if(sf.ShowDialog()==DialogResult.OK){
				SaveScene(sf.FileName);
				sceneName=System.IO.Path.GetFileNameWithoutExtension(sf.FileName);
			}

		}
#endif

    }
}
