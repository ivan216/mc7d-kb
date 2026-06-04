using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace _3dedit
{
	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
    public class Form1:System.Windows.Forms.Form {
        public string VERSION = "v0.8.4";

		private System.Windows.Forms.Button btnTogglePanel;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mi_ShowSidePanel;
		private bool m_panelCollapsed = false;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
        private _3dedit.DXControl dxControl2;
        private StatusStrip statusStrip1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem mi_Open;
        private ToolStripMenuItem mi_Save;
        private ToolStripMenuItem mi_SaveAs;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mi_Exit;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem mi_Reset;
        private ToolStripMenuItem mi_Undo;
        private ToolStripMenuItem mi_Redo;
        private ToolStripMenuItem puzzleToolStripMenuItem;
        private ToolStripMenuItem mi_Puzzle4D;
        private ToolStripMenuItem mi_Puzzle5D;
        private ToolStripMenuItem mi_Puzzle6D;
        private ToolStripMenuItem mi_Puzzle7D;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem mi_PuzzleSize3;
        private ToolStripMenuItem mi_PuzzleSize4;
        private ToolStripMenuItem mi_PuzzleSize5;
        private ToolStripMenuItem mi_PuzzleSize6;
        private ToolStripMenuItem mi_PuzzleSize7;
        private ToolStripMenuItem mi_FullUndo;
        private ToolStripMenuItem mi_FullScramble;
        private ToolStripMenuItem mi_ScrambleNTurns;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mi_FullRedo;
        private ToolStripStatusLabel ms_Twists;
        private ToolStripMenuItem mi_Scramble1;
        private ToolStripMenuItem mi_Scramble2;
        private ToolStripMenuItem mi_Scramble3;
        private ToolStripMenuItem mi_Scramble4;
        private ToolStripMenuItem mi_Scramble5;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar pb_ClickStatus;
        private Label label1;
        private TrackBar trk_faceSep;
        private Label label5;
        private TrackBar trk_Perspective;
        private Label label3;
        private TrackBar trk_StickerSize;
        private Label label2;
        private TrackBar trk_BlockSize;
        private Button m_bCol1;
        private GroupBox groupBox1;
        private RadioButton m_rbClick3;
        private RadioButton m_rbClick2;
        private Label label4;
        private Button m_bCol14;
        private Button m_bCol13;
        private Button m_bCol7;
        private Button m_bCol12;
        private Button m_bCol6;
        private Button m_bCol11;
        private Button m_bCol5;
        private Button m_bCol10;
        private Button m_bCol4;
        private Button m_bCol9;
        private Button m_bCol3;
        private Button m_bCol8;
        private Button m_bCol2;
        private CheckBox cb_Col1;
        private Label label6;
        private CheckBox cb_Col14;
        private CheckBox cb_Col13;
        private CheckBox cb_Col7;
        private CheckBox cb_Col12;
        private CheckBox cb_Col6;
        private CheckBox cb_Col11;
        private CheckBox cb_Col5;
        private CheckBox cb_Col10;
        private CheckBox cb_Col4;
        private CheckBox cb_Col9;
        private CheckBox cb_Col3;
        private CheckBox cb_Col8;
        private CheckBox cb_Col2;
        private Label label8;
        private Label label7;
        private TrackBar trk_LightSpec;
        private TrackBar trk_LightDiff;
        private CheckBox cb_Show3C;
        private CheckBox cb_Show5C;
        private CheckBox cb_Show1C;
        private CheckBox cb_Show6C;
        private CheckBox cb_Show2C;
        private CheckBox cb_Show7C;
        private CheckBox cb_Show4C;
        private CheckBox cb_HighlightByColors;
        private Label label9;
        private Label label10;
        private ListBox m_lbMacros;
        private ToolStripMenuItem macroToolStripMenuItem;
        private ToolStripMenuItem mi_StartRecordig;
        private ToolStripMenuItem loadMacroFileToolStripMenuItem;
        private ToolStripMenuItem saveMacroFileToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem saveMacroFileAsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem startExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem stopExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem undoExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem commutatorToolStripMenuItem;
        private TextBox m_tbRevStack;
        private Label label11;
        private ToolStripStatusLabel ms_MacroStatus;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem recalculateToolStripMenuItem;
        private Label label12;
        private TrackBar m_trkTransparency;
        private Label label13;
        private TrackBar m_trkFullUndoSpeed;
        private RadioButton m_rbClick2Inv;
        private CheckBox m_cbQuickMacro;
        private ToolStripStatusLabel m_lblCTime;
        private CheckBox m_RunByClick;
        private ToolStripDropDownButton activeKeybind;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem editKeybinds;
        private ToolStripMenuItem mi_PuzzleSize2;
        private System.ComponentModel.IContainer components;

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
            this.dxControl2.KeyDown += new KeyEventHandler(KeyDownEvt);
            this.dxControl2.KeyUp += new KeyEventHandler(KeyUpEvt);

			this.panel2.Controls.Add(this.dxControl2);
            this.panel2.Controls.SetChildIndex(this.dxControl2, 0);

            NColMask=new bool[8];
            FaceMask=new int[15];
            for(int i=0;i<8;i++) NColMask[i]=true;
            for(int i=0;i<15;i++) FaceMask[i]=0;
            LoadSettings("MC7D_settings.txt");
            Macros=new CMacroFile(GetDim(),GetSize());
            m_Timer=new System.Threading.Timer(this.UpdateTime,null,0,117);

            // UpdateTime complains about access from another thread if we try this before creating m_timer???
            LoadKeybinds("MC7D_keybinds.txt");
            Keybinds.ActiveLayoutChanged += this.CheckKeybindSet;
            Keybinds.KeybindLayoutsChanged += this.UpdateKeybindMenu;
            this.UpdateKeybindMenu(null, EventArgs.Empty);
            Keybindings.loaded = Keybinds;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		//System.Resources.ResourceManager resources = new System.Resources.ResourceManager("_3dedit.Edit3D",Assembly.GetExecutingAssembly());

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.btnTogglePanel = new System.Windows.Forms.Button();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_ShowSidePanel = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_RunByClick = new System.Windows.Forms.CheckBox();
            this.m_cbQuickMacro = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.m_tbRevStack = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.m_lbMacros = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cb_Show3C = new System.Windows.Forms.CheckBox();
            this.cb_Show5C = new System.Windows.Forms.CheckBox();
            this.cb_Show1C = new System.Windows.Forms.CheckBox();
            this.cb_Show6C = new System.Windows.Forms.CheckBox();
            this.cb_Show2C = new System.Windows.Forms.CheckBox();
            this.cb_Show7C = new System.Windows.Forms.CheckBox();
            this.cb_Show4C = new System.Windows.Forms.CheckBox();
            this.cb_HighlightByColors = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.m_trkFullUndoSpeed = new System.Windows.Forms.TrackBar();
            this.m_trkTransparency = new System.Windows.Forms.TrackBar();
            this.trk_LightSpec = new System.Windows.Forms.TrackBar();
            this.trk_LightDiff = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_Col14 = new System.Windows.Forms.CheckBox();
            this.cb_Col13 = new System.Windows.Forms.CheckBox();
            this.cb_Col7 = new System.Windows.Forms.CheckBox();
            this.cb_Col12 = new System.Windows.Forms.CheckBox();
            this.cb_Col6 = new System.Windows.Forms.CheckBox();
            this.cb_Col11 = new System.Windows.Forms.CheckBox();
            this.cb_Col5 = new System.Windows.Forms.CheckBox();
            this.cb_Col10 = new System.Windows.Forms.CheckBox();
            this.cb_Col4 = new System.Windows.Forms.CheckBox();
            this.cb_Col9 = new System.Windows.Forms.CheckBox();
            this.cb_Col3 = new System.Windows.Forms.CheckBox();
            this.cb_Col8 = new System.Windows.Forms.CheckBox();
            this.cb_Col2 = new System.Windows.Forms.CheckBox();
            this.cb_Col1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_bCol14 = new System.Windows.Forms.Button();
            this.m_bCol13 = new System.Windows.Forms.Button();
            this.m_bCol7 = new System.Windows.Forms.Button();
            this.m_bCol12 = new System.Windows.Forms.Button();
            this.m_bCol6 = new System.Windows.Forms.Button();
            this.m_bCol11 = new System.Windows.Forms.Button();
            this.m_bCol5 = new System.Windows.Forms.Button();
            this.m_bCol10 = new System.Windows.Forms.Button();
            this.m_bCol4 = new System.Windows.Forms.Button();
            this.m_bCol9 = new System.Windows.Forms.Button();
            this.m_bCol3 = new System.Windows.Forms.Button();
            this.m_bCol8 = new System.Windows.Forms.Button();
            this.m_bCol2 = new System.Windows.Forms.Button();
            this.m_bCol1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_rbClick3 = new System.Windows.Forms.RadioButton();
            this.m_rbClick2Inv = new System.Windows.Forms.RadioButton();
            this.m_rbClick2 = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.trk_Perspective = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.trk_StickerSize = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trk_BlockSize = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.trk_faceSep = new System.Windows.Forms.TrackBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pb_ClickStatus = new System.Windows.Forms.ToolStripProgressBar();
            this.ms_Twists = new System.Windows.Forms.ToolStripStatusLabel();
            this.ms_MacroStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.activeKeybind = new System.Windows.Forms.ToolStripDropDownButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_FullScramble = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_ScrambleNTurns = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Redo = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_FullUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_FullRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.recalculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editKeybinds = new System.Windows.Forms.ToolStripMenuItem();
            this.puzzleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Puzzle4D = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Puzzle5D = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Puzzle6D = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Puzzle7D = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_PuzzleSize2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_PuzzleSize3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_PuzzleSize4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_PuzzleSize5 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_PuzzleSize6 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_PuzzleSize7 = new System.Windows.Forms.ToolStripMenuItem();
            this.macroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_StartRecordig = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMacroFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMacroFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMacroFileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.startExtraTurnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopExtraTurnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoExtraTurnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commutatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_trkFullUndoSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_trkTransparency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightSpec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightDiff)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_Perspective)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_StickerSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_BlockSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_faceSep)).BeginInit();
            this.panel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // btnTogglePanel
            //
            this.btnTogglePanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTogglePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.btnTogglePanel.Location = new System.Drawing.Point(754, 312);
            this.btnTogglePanel.Name = "btnTogglePanel";
            this.btnTogglePanel.Size = new System.Drawing.Size(6, 50);
            this.btnTogglePanel.TabIndex = 18;
            this.btnTogglePanel.TabStop = false;
            this.btnTogglePanel.Text = "›";
            this.btnTogglePanel.UseVisualStyleBackColor = true;
            this.btnTogglePanel.Click += new System.EventHandler(this.btnTogglePanel_Click);
            //
            // panel1
            //
            this.panel1.Controls.Add(this.m_RunByClick);
            this.panel1.Controls.Add(this.m_cbQuickMacro);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.m_tbRevStack);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.m_lbMacros);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.cb_Show3C);
            this.panel1.Controls.Add(this.cb_Show5C);
            this.panel1.Controls.Add(this.cb_Show1C);
            this.panel1.Controls.Add(this.cb_Show6C);
            this.panel1.Controls.Add(this.cb_Show2C);
            this.panel1.Controls.Add(this.cb_Show7C);
            this.panel1.Controls.Add(this.cb_Show4C);
            this.panel1.Controls.Add(this.cb_HighlightByColors);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.m_trkFullUndoSpeed);
            this.panel1.Controls.Add(this.m_trkTransparency);
            this.panel1.Controls.Add(this.trk_LightSpec);
            this.panel1.Controls.Add(this.trk_LightDiff);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.cb_Col14);
            this.panel1.Controls.Add(this.cb_Col13);
            this.panel1.Controls.Add(this.cb_Col7);
            this.panel1.Controls.Add(this.cb_Col12);
            this.panel1.Controls.Add(this.cb_Col6);
            this.panel1.Controls.Add(this.cb_Col11);
            this.panel1.Controls.Add(this.cb_Col5);
            this.panel1.Controls.Add(this.cb_Col10);
            this.panel1.Controls.Add(this.cb_Col4);
            this.panel1.Controls.Add(this.cb_Col9);
            this.panel1.Controls.Add(this.cb_Col3);
            this.panel1.Controls.Add(this.cb_Col8);
            this.panel1.Controls.Add(this.cb_Col2);
            this.panel1.Controls.Add(this.cb_Col1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.m_bCol14);
            this.panel1.Controls.Add(this.m_bCol13);
            this.panel1.Controls.Add(this.m_bCol7);
            this.panel1.Controls.Add(this.m_bCol12);
            this.panel1.Controls.Add(this.m_bCol6);
            this.panel1.Controls.Add(this.m_bCol11);
            this.panel1.Controls.Add(this.m_bCol5);
            this.panel1.Controls.Add(this.m_bCol10);
            this.panel1.Controls.Add(this.m_bCol4);
            this.panel1.Controls.Add(this.m_bCol9);
            this.panel1.Controls.Add(this.m_bCol3);
            this.panel1.Controls.Add(this.m_bCol8);
            this.panel1.Controls.Add(this.m_bCol2);
            this.panel1.Controls.Add(this.m_bCol1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.trk_Perspective);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.trk_StickerSize);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.trk_BlockSize);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trk_faceSep);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.AutoScroll = true;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel1.Location = new System.Drawing.Point(777, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 684);
            this.panel1.TabIndex = 19;
            // 
            // m_RunByClick
            // 
            this.m_RunByClick.AutoSize = true;
            this.m_RunByClick.Checked = true;
            this.m_RunByClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_RunByClick.Location = new System.Drawing.Point(113, 633);
            this.m_RunByClick.Name = "m_RunByClick";
            this.m_RunByClick.Size = new System.Drawing.Size(86, 17);
            this.m_RunByClick.TabIndex = 17;
            this.m_RunByClick.Text = "Run by Click";
            this.m_RunByClick.UseVisualStyleBackColor = true;
            // 
            // m_cbQuickMacro
            // 
            this.m_cbQuickMacro.AutoSize = true;
            this.m_cbQuickMacro.Checked = true;
            this.m_cbQuickMacro.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbQuickMacro.Location = new System.Drawing.Point(6, 633);
            this.m_cbQuickMacro.Name = "m_cbQuickMacro";
            this.m_cbQuickMacro.Size = new System.Drawing.Size(98, 17);
            this.m_cbQuickMacro.TabIndex = 17;
            this.m_cbQuickMacro.Text = "Macros Autoref";
            this.m_cbQuickMacro.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(95, 194);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(72, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "Transparency";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 252);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Full Undo Speed";
            // 
            // m_tbRevStack
            // 
            this.m_tbRevStack.BackColor = System.Drawing.SystemColors.Control;
            this.m_tbRevStack.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.m_tbRevStack.Location = new System.Drawing.Point(67, 661);
            this.m_tbRevStack.Name = "m_tbRevStack";
            this.m_tbRevStack.ReadOnly = true;
            this.m_tbRevStack.Size = new System.Drawing.Size(138, 13);
            this.m_tbRevStack.TabIndex = 15;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 661);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "RevStack:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 431);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Macros:";
            // 
            // m_lbMacros
            // 
            this.m_lbMacros.FormattingEnabled = true;
            this.m_lbMacros.Location = new System.Drawing.Point(6, 453);
            this.m_lbMacros.Name = "m_lbMacros";
            this.m_lbMacros.Size = new System.Drawing.Size(110, 147);
            this.m_lbMacros.Sorted = true;
            this.m_lbMacros.TabIndex = 12;
            this.m_lbMacros.MouseClick += new System.Windows.Forms.MouseEventHandler(this.m_lbMacros_MouseClick);
            this.m_lbMacros.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_lbMacros_MouseDoubleClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(127, 431);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Show cubies:";
            // 
            // cb_Show3C
            // 
            this.cb_Show3C.AutoSize = true;
            this.cb_Show3C.Checked = true;
            this.cb_Show3C.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Show3C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_Show3C.Location = new System.Drawing.Point(155, 505);
            this.cb_Show3C.Name = "cb_Show3C";
            this.cb_Show3C.Size = new System.Drawing.Size(39, 17);
            this.cb_Show3C.TabIndex = 10;
            this.cb_Show3C.Text = "3C";
            this.cb_Show3C.UseVisualStyleBackColor = true;
            this.cb_Show3C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show5C
            // 
            this.cb_Show5C.AutoSize = true;
            this.cb_Show5C.Checked = true;
            this.cb_Show5C.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Show5C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_Show5C.Location = new System.Drawing.Point(155, 551);
            this.cb_Show5C.Name = "cb_Show5C";
            this.cb_Show5C.Size = new System.Drawing.Size(39, 17);
            this.cb_Show5C.TabIndex = 10;
            this.cb_Show5C.Text = "5C";
            this.cb_Show5C.UseVisualStyleBackColor = true;
            this.cb_Show5C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show1C
            // 
            this.cb_Show1C.AutoSize = true;
            this.cb_Show1C.Checked = true;
            this.cb_Show1C.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Show1C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_Show1C.Location = new System.Drawing.Point(155, 459);
            this.cb_Show1C.Name = "cb_Show1C";
            this.cb_Show1C.Size = new System.Drawing.Size(39, 17);
            this.cb_Show1C.TabIndex = 10;
            this.cb_Show1C.Text = "1C";
            this.cb_Show1C.UseVisualStyleBackColor = true;
            this.cb_Show1C.CheckedChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show6C
            // 
            this.cb_Show6C.AutoSize = true;
            this.cb_Show6C.Checked = true;
            this.cb_Show6C.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Show6C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_Show6C.Location = new System.Drawing.Point(155, 574);
            this.cb_Show6C.Name = "cb_Show6C";
            this.cb_Show6C.Size = new System.Drawing.Size(39, 17);
            this.cb_Show6C.TabIndex = 10;
            this.cb_Show6C.Text = "6C";
            this.cb_Show6C.UseVisualStyleBackColor = true;
            this.cb_Show6C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show2C
            // 
            this.cb_Show2C.AutoSize = true;
            this.cb_Show2C.Checked = true;
            this.cb_Show2C.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Show2C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_Show2C.Location = new System.Drawing.Point(155, 482);
            this.cb_Show2C.Name = "cb_Show2C";
            this.cb_Show2C.Size = new System.Drawing.Size(39, 17);
            this.cb_Show2C.TabIndex = 10;
            this.cb_Show2C.Text = "2C";
            this.cb_Show2C.UseVisualStyleBackColor = true;
            this.cb_Show2C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show7C
            // 
            this.cb_Show7C.AutoSize = true;
            this.cb_Show7C.Checked = true;
            this.cb_Show7C.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Show7C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_Show7C.Location = new System.Drawing.Point(155, 597);
            this.cb_Show7C.Name = "cb_Show7C";
            this.cb_Show7C.Size = new System.Drawing.Size(39, 17);
            this.cb_Show7C.TabIndex = 10;
            this.cb_Show7C.Text = "7C";
            this.cb_Show7C.UseVisualStyleBackColor = true;
            this.cb_Show7C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show4C
            // 
            this.cb_Show4C.AutoSize = true;
            this.cb_Show4C.Checked = true;
            this.cb_Show4C.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Show4C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_Show4C.Location = new System.Drawing.Point(155, 528);
            this.cb_Show4C.Name = "cb_Show4C";
            this.cb_Show4C.Size = new System.Drawing.Size(39, 17);
            this.cb_Show4C.TabIndex = 10;
            this.cb_Show4C.Text = "4C";
            this.cb_Show4C.UseVisualStyleBackColor = true;
            this.cb_Show4C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_HighlightByColors
            // 
            this.cb_HighlightByColors.AutoSize = true;
            this.cb_HighlightByColors.Location = new System.Drawing.Point(15, 397);
            this.cb_HighlightByColors.Name = "cb_HighlightByColors";
            this.cb_HighlightByColors.Size = new System.Drawing.Size(112, 17);
            this.cb_HighlightByColors.TabIndex = 9;
            this.cb_HighlightByColors.Text = "Highlight by colors";
            this.cb_HighlightByColors.ThreeState = true;
            this.cb_HighlightByColors.UseVisualStyleBackColor = true;
            this.cb_HighlightByColors.CheckedChanged += new System.EventHandler(this.cb_HighlightByColors_CheckedChanged);
            this.cb_HighlightByColors.CheckStateChanged += new System.EventHandler(this.cb_HighlightByColors_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(95, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "S:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(95, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "D:";
            // 
            // m_trkFullUndoSpeed
            // 
            this.m_trkFullUndoSpeed.Location = new System.Drawing.Point(97, 245);
            this.m_trkFullUndoSpeed.Maximum = 100;
            this.m_trkFullUndoSpeed.Name = "m_trkFullUndoSpeed";
            this.m_trkFullUndoSpeed.Size = new System.Drawing.Size(104, 45);
            this.m_trkFullUndoSpeed.TabIndex = 7;
            this.m_trkFullUndoSpeed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.m_trkFullUndoSpeed.Value = 15;
            this.m_trkFullUndoSpeed.ValueChanged += new System.EventHandler(this.m_trkUndoSpeed_ValueChanged);
            // 
            // m_trkTransparency
            // 
            this.m_trkTransparency.Location = new System.Drawing.Point(98, 210);
            this.m_trkTransparency.Maximum = 255;
            this.m_trkTransparency.Name = "m_trkTransparency";
            this.m_trkTransparency.Size = new System.Drawing.Size(104, 45);
            this.m_trkTransparency.TabIndex = 7;
            this.m_trkTransparency.TickStyle = System.Windows.Forms.TickStyle.None;
            this.m_trkTransparency.Value = 15;
            this.m_trkTransparency.ValueChanged += new System.EventHandler(this.m_trkTransparency_ValueChanged);
            // 
            // trk_LightSpec
            // 
            this.trk_LightSpec.Location = new System.Drawing.Point(113, 168);
            this.trk_LightSpec.Maximum = 255;
            this.trk_LightSpec.Name = "trk_LightSpec";
            this.trk_LightSpec.Size = new System.Drawing.Size(88, 45);
            this.trk_LightSpec.TabIndex = 7;
            this.trk_LightSpec.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_LightSpec.ValueChanged += new System.EventHandler(this.trk_LightSpec_ValueChanged);
            // 
            // trk_LightDiff
            // 
            this.trk_LightDiff.Location = new System.Drawing.Point(113, 141);
            this.trk_LightDiff.Maximum = 255;
            this.trk_LightDiff.Name = "trk_LightDiff";
            this.trk_LightDiff.Size = new System.Drawing.Size(88, 45);
            this.trk_LightDiff.TabIndex = 7;
            this.trk_LightDiff.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_LightDiff.ValueChanged += new System.EventHandler(this.trk_LightDiff_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(95, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Light:";
            // 
            // cb_Col14
            // 
            this.cb_Col14.AutoSize = true;
            this.cb_Col14.Checked = true;
            this.cb_Col14.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col14.Location = new System.Drawing.Point(186, 377);
            this.cb_Col14.Name = "cb_Col14";
            this.cb_Col14.Size = new System.Drawing.Size(15, 14);
            this.cb_Col14.TabIndex = 5;
            this.cb_Col14.ThreeState = true;
            this.cb_Col14.UseVisualStyleBackColor = true;
            this.cb_Col14.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col13
            // 
            this.cb_Col13.AutoSize = true;
            this.cb_Col13.Checked = true;
            this.cb_Col13.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col13.Location = new System.Drawing.Point(161, 377);
            this.cb_Col13.Name = "cb_Col13";
            this.cb_Col13.Size = new System.Drawing.Size(15, 14);
            this.cb_Col13.TabIndex = 5;
            this.cb_Col13.ThreeState = true;
            this.cb_Col13.UseVisualStyleBackColor = true;
            this.cb_Col13.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col7
            // 
            this.cb_Col7.AutoSize = true;
            this.cb_Col7.Checked = true;
            this.cb_Col7.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col7.Location = new System.Drawing.Point(186, 327);
            this.cb_Col7.Name = "cb_Col7";
            this.cb_Col7.Size = new System.Drawing.Size(15, 14);
            this.cb_Col7.TabIndex = 5;
            this.cb_Col7.ThreeState = true;
            this.cb_Col7.UseVisualStyleBackColor = true;
            this.cb_Col7.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col12
            // 
            this.cb_Col12.AutoSize = true;
            this.cb_Col12.Checked = true;
            this.cb_Col12.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col12.Location = new System.Drawing.Point(136, 377);
            this.cb_Col12.Name = "cb_Col12";
            this.cb_Col12.Size = new System.Drawing.Size(15, 14);
            this.cb_Col12.TabIndex = 5;
            this.cb_Col12.ThreeState = true;
            this.cb_Col12.UseVisualStyleBackColor = true;
            this.cb_Col12.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col6
            // 
            this.cb_Col6.AutoSize = true;
            this.cb_Col6.Checked = true;
            this.cb_Col6.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col6.Location = new System.Drawing.Point(161, 327);
            this.cb_Col6.Name = "cb_Col6";
            this.cb_Col6.Size = new System.Drawing.Size(15, 14);
            this.cb_Col6.TabIndex = 5;
            this.cb_Col6.ThreeState = true;
            this.cb_Col6.UseVisualStyleBackColor = true;
            this.cb_Col6.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col11
            // 
            this.cb_Col11.AutoSize = true;
            this.cb_Col11.Checked = true;
            this.cb_Col11.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col11.Location = new System.Drawing.Point(111, 377);
            this.cb_Col11.Name = "cb_Col11";
            this.cb_Col11.Size = new System.Drawing.Size(15, 14);
            this.cb_Col11.TabIndex = 5;
            this.cb_Col11.ThreeState = true;
            this.cb_Col11.UseVisualStyleBackColor = true;
            this.cb_Col11.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col5
            // 
            this.cb_Col5.AutoSize = true;
            this.cb_Col5.Checked = true;
            this.cb_Col5.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col5.Location = new System.Drawing.Point(136, 327);
            this.cb_Col5.Name = "cb_Col5";
            this.cb_Col5.Size = new System.Drawing.Size(15, 14);
            this.cb_Col5.TabIndex = 5;
            this.cb_Col5.ThreeState = true;
            this.cb_Col5.UseVisualStyleBackColor = true;
            this.cb_Col5.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col10
            // 
            this.cb_Col10.AutoSize = true;
            this.cb_Col10.Checked = true;
            this.cb_Col10.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col10.Location = new System.Drawing.Point(86, 377);
            this.cb_Col10.Name = "cb_Col10";
            this.cb_Col10.Size = new System.Drawing.Size(15, 14);
            this.cb_Col10.TabIndex = 5;
            this.cb_Col10.ThreeState = true;
            this.cb_Col10.UseVisualStyleBackColor = true;
            this.cb_Col10.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col4
            // 
            this.cb_Col4.AutoSize = true;
            this.cb_Col4.Checked = true;
            this.cb_Col4.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col4.Location = new System.Drawing.Point(111, 327);
            this.cb_Col4.Name = "cb_Col4";
            this.cb_Col4.Size = new System.Drawing.Size(15, 14);
            this.cb_Col4.TabIndex = 5;
            this.cb_Col4.ThreeState = true;
            this.cb_Col4.UseVisualStyleBackColor = true;
            this.cb_Col4.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col9
            // 
            this.cb_Col9.AutoSize = true;
            this.cb_Col9.Checked = true;
            this.cb_Col9.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col9.Location = new System.Drawing.Point(61, 377);
            this.cb_Col9.Name = "cb_Col9";
            this.cb_Col9.Size = new System.Drawing.Size(15, 14);
            this.cb_Col9.TabIndex = 5;
            this.cb_Col9.ThreeState = true;
            this.cb_Col9.UseVisualStyleBackColor = true;
            this.cb_Col9.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col3
            // 
            this.cb_Col3.AutoSize = true;
            this.cb_Col3.Checked = true;
            this.cb_Col3.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col3.Location = new System.Drawing.Point(86, 327);
            this.cb_Col3.Name = "cb_Col3";
            this.cb_Col3.Size = new System.Drawing.Size(15, 14);
            this.cb_Col3.TabIndex = 5;
            this.cb_Col3.ThreeState = true;
            this.cb_Col3.UseVisualStyleBackColor = true;
            this.cb_Col3.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col8
            // 
            this.cb_Col8.AutoSize = true;
            this.cb_Col8.Checked = true;
            this.cb_Col8.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col8.Location = new System.Drawing.Point(36, 377);
            this.cb_Col8.Name = "cb_Col8";
            this.cb_Col8.Size = new System.Drawing.Size(15, 14);
            this.cb_Col8.TabIndex = 5;
            this.cb_Col8.ThreeState = true;
            this.cb_Col8.UseVisualStyleBackColor = true;
            this.cb_Col8.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col2
            // 
            this.cb_Col2.AutoSize = true;
            this.cb_Col2.Checked = true;
            this.cb_Col2.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col2.Location = new System.Drawing.Point(61, 327);
            this.cb_Col2.Name = "cb_Col2";
            this.cb_Col2.Size = new System.Drawing.Size(15, 14);
            this.cb_Col2.TabIndex = 5;
            this.cb_Col2.ThreeState = true;
            this.cb_Col2.UseVisualStyleBackColor = true;
            this.cb_Col2.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // cb_Col1
            // 
            this.cb_Col1.AutoSize = true;
            this.cb_Col1.Checked = true;
            this.cb_Col1.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Col1.Location = new System.Drawing.Point(36, 327);
            this.cb_Col1.Name = "cb_Col1";
            this.cb_Col1.Size = new System.Drawing.Size(15, 14);
            this.cb_Col1.TabIndex = 5;
            this.cb_Col1.ThreeState = true;
            this.cb_Col1.UseVisualStyleBackColor = true;
            this.cb_Col1.CheckStateChanged += new System.EventHandler(this.cb_Col1_CheckStateChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 277);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Colors:";
            // 
            // m_bCol14
            // 
            this.m_bCol14.BackColor = System.Drawing.Color.Red;
            this.m_bCol14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol14.Location = new System.Drawing.Point(179, 347);
            this.m_bCol14.Name = "m_bCol14";
            this.m_bCol14.Size = new System.Drawing.Size(24, 24);
            this.m_bCol14.TabIndex = 3;
            this.m_bCol14.UseVisualStyleBackColor = false;
            this.m_bCol14.Click += new System.EventHandler(this.m_bCol14_Click);
            // 
            // m_bCol13
            // 
            this.m_bCol13.BackColor = System.Drawing.Color.Red;
            this.m_bCol13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol13.Location = new System.Drawing.Point(154, 347);
            this.m_bCol13.Name = "m_bCol13";
            this.m_bCol13.Size = new System.Drawing.Size(24, 24);
            this.m_bCol13.TabIndex = 3;
            this.m_bCol13.UseVisualStyleBackColor = false;
            this.m_bCol13.Click += new System.EventHandler(this.m_bCol13_Click);
            // 
            // m_bCol7
            // 
            this.m_bCol7.BackColor = System.Drawing.Color.Red;
            this.m_bCol7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol7.Location = new System.Drawing.Point(179, 297);
            this.m_bCol7.Name = "m_bCol7";
            this.m_bCol7.Size = new System.Drawing.Size(24, 24);
            this.m_bCol7.TabIndex = 3;
            this.m_bCol7.UseVisualStyleBackColor = false;
            this.m_bCol7.Click += new System.EventHandler(this.m_bCol7_Click);
            // 
            // m_bCol12
            // 
            this.m_bCol12.BackColor = System.Drawing.Color.Red;
            this.m_bCol12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol12.Location = new System.Drawing.Point(129, 347);
            this.m_bCol12.Name = "m_bCol12";
            this.m_bCol12.Size = new System.Drawing.Size(24, 24);
            this.m_bCol12.TabIndex = 3;
            this.m_bCol12.UseVisualStyleBackColor = false;
            this.m_bCol12.Click += new System.EventHandler(this.m_bCol12_Click);
            // 
            // m_bCol6
            // 
            this.m_bCol6.BackColor = System.Drawing.Color.Red;
            this.m_bCol6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol6.Location = new System.Drawing.Point(154, 297);
            this.m_bCol6.Name = "m_bCol6";
            this.m_bCol6.Size = new System.Drawing.Size(24, 24);
            this.m_bCol6.TabIndex = 3;
            this.m_bCol6.UseVisualStyleBackColor = false;
            this.m_bCol6.Click += new System.EventHandler(this.m_bCol6_Click);
            // 
            // m_bCol11
            // 
            this.m_bCol11.BackColor = System.Drawing.Color.Red;
            this.m_bCol11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol11.Location = new System.Drawing.Point(104, 347);
            this.m_bCol11.Name = "m_bCol11";
            this.m_bCol11.Size = new System.Drawing.Size(24, 24);
            this.m_bCol11.TabIndex = 3;
            this.m_bCol11.UseVisualStyleBackColor = false;
            this.m_bCol11.Click += new System.EventHandler(this.m_bCol11_Click);
            // 
            // m_bCol5
            // 
            this.m_bCol5.BackColor = System.Drawing.Color.Red;
            this.m_bCol5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol5.Location = new System.Drawing.Point(129, 297);
            this.m_bCol5.Name = "m_bCol5";
            this.m_bCol5.Size = new System.Drawing.Size(24, 24);
            this.m_bCol5.TabIndex = 3;
            this.m_bCol5.UseVisualStyleBackColor = false;
            this.m_bCol5.Click += new System.EventHandler(this.m_bCol5_Click);
            // 
            // m_bCol10
            // 
            this.m_bCol10.BackColor = System.Drawing.Color.Red;
            this.m_bCol10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol10.Location = new System.Drawing.Point(79, 347);
            this.m_bCol10.Name = "m_bCol10";
            this.m_bCol10.Size = new System.Drawing.Size(24, 24);
            this.m_bCol10.TabIndex = 3;
            this.m_bCol10.UseVisualStyleBackColor = false;
            this.m_bCol10.Click += new System.EventHandler(this.m_bCol10_Click);
            // 
            // m_bCol4
            // 
            this.m_bCol4.BackColor = System.Drawing.Color.Red;
            this.m_bCol4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol4.Location = new System.Drawing.Point(104, 297);
            this.m_bCol4.Name = "m_bCol4";
            this.m_bCol4.Size = new System.Drawing.Size(24, 24);
            this.m_bCol4.TabIndex = 3;
            this.m_bCol4.UseVisualStyleBackColor = false;
            this.m_bCol4.Click += new System.EventHandler(this.m_bCol4_Click);
            // 
            // m_bCol9
            // 
            this.m_bCol9.BackColor = System.Drawing.Color.Red;
            this.m_bCol9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol9.Location = new System.Drawing.Point(54, 347);
            this.m_bCol9.Name = "m_bCol9";
            this.m_bCol9.Size = new System.Drawing.Size(24, 24);
            this.m_bCol9.TabIndex = 3;
            this.m_bCol9.UseVisualStyleBackColor = false;
            this.m_bCol9.Click += new System.EventHandler(this.m_bCol9_Click);
            // 
            // m_bCol3
            // 
            this.m_bCol3.BackColor = System.Drawing.Color.Red;
            this.m_bCol3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol3.Location = new System.Drawing.Point(79, 297);
            this.m_bCol3.Name = "m_bCol3";
            this.m_bCol3.Size = new System.Drawing.Size(24, 24);
            this.m_bCol3.TabIndex = 3;
            this.m_bCol3.UseVisualStyleBackColor = false;
            this.m_bCol3.Click += new System.EventHandler(this.m_bCol3_Click);
            // 
            // m_bCol8
            // 
            this.m_bCol8.BackColor = System.Drawing.Color.Red;
            this.m_bCol8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol8.Location = new System.Drawing.Point(29, 347);
            this.m_bCol8.Name = "m_bCol8";
            this.m_bCol8.Size = new System.Drawing.Size(24, 24);
            this.m_bCol8.TabIndex = 3;
            this.m_bCol8.UseVisualStyleBackColor = false;
            this.m_bCol8.Click += new System.EventHandler(this.m_bCol8_Click);
            // 
            // m_bCol2
            // 
            this.m_bCol2.BackColor = System.Drawing.Color.Red;
            this.m_bCol2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol2.Location = new System.Drawing.Point(54, 297);
            this.m_bCol2.Name = "m_bCol2";
            this.m_bCol2.Size = new System.Drawing.Size(24, 24);
            this.m_bCol2.TabIndex = 3;
            this.m_bCol2.UseVisualStyleBackColor = false;
            this.m_bCol2.Click += new System.EventHandler(this.m_bCol2_Click);
            // 
            // m_bCol1
            // 
            this.m_bCol1.BackColor = System.Drawing.Color.Red;
            this.m_bCol1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_bCol1.Location = new System.Drawing.Point(29, 297);
            this.m_bCol1.Name = "m_bCol1";
            this.m_bCol1.Size = new System.Drawing.Size(24, 24);
            this.m_bCol1.TabIndex = 3;
            this.m_bCol1.UseVisualStyleBackColor = false;
            this.m_bCol1.Click += new System.EventHandler(this.m_bCol1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_rbClick3);
            this.groupBox1.Controls.Add(this.m_rbClick2Inv);
            this.groupBox1.Controls.Add(this.m_rbClick2);
            this.groupBox1.Location = new System.Drawing.Point(9, 125);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(79, 88);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Click Mode";
            // 
            // m_rbClick3
            // 
            this.m_rbClick3.AutoSize = true;
            this.m_rbClick3.Location = new System.Drawing.Point(7, 65);
            this.m_rbClick3.Name = "m_rbClick3";
            this.m_rbClick3.Size = new System.Drawing.Size(62, 17);
            this.m_rbClick3.TabIndex = 0;
            this.m_rbClick3.Text = "3 Clicks";
            this.m_rbClick3.UseVisualStyleBackColor = true;
            this.m_rbClick3.CheckedChanged += new System.EventHandler(this.ClickMode_CheckedChanged);
            // 
            // m_rbClick2Inv
            // 
            this.m_rbClick2Inv.AutoSize = true;
            this.m_rbClick2Inv.Location = new System.Drawing.Point(7, 42);
            this.m_rbClick2Inv.Name = "m_rbClick2Inv";
            this.m_rbClick2Inv.Size = new System.Drawing.Size(78, 17);
            this.m_rbClick2Inv.TabIndex = 0;
            this.m_rbClick2Inv.Text = "2 Clk (Opp)";
            this.m_rbClick2Inv.UseVisualStyleBackColor = true;
            this.m_rbClick2Inv.CheckedChanged += new System.EventHandler(this.ClickMode_CheckedChanged);
            // 
            // m_rbClick2
            // 
            this.m_rbClick2.AutoSize = true;
            this.m_rbClick2.Checked = true;
            this.m_rbClick2.Location = new System.Drawing.Point(7, 19);
            this.m_rbClick2.Name = "m_rbClick2";
            this.m_rbClick2.Size = new System.Drawing.Size(62, 17);
            this.m_rbClick2.TabIndex = 0;
            this.m_rbClick2.TabStop = true;
            this.m_rbClick2.Text = "2 Clicks";
            this.m_rbClick2.UseVisualStyleBackColor = true;
            this.m_rbClick2.CheckedChanged += new System.EventHandler(this.ClickMode_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Perspective";
            // 
            // trk_Perspective
            // 
            this.trk_Perspective.Location = new System.Drawing.Point(97, 89);
            this.trk_Perspective.Maximum = 50;
            this.trk_Perspective.Name = "trk_Perspective";
            this.trk_Perspective.Size = new System.Drawing.Size(104, 45);
            this.trk_Perspective.TabIndex = 0;
            this.trk_Perspective.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_Perspective.ValueChanged += new System.EventHandler(this.trk_faceSep_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Sticker Size";
            // 
            // trk_StickerSize
            // 
            this.trk_StickerSize.Location = new System.Drawing.Point(97, 66);
            this.trk_StickerSize.Maximum = 50;
            this.trk_StickerSize.Minimum = 1;
            this.trk_StickerSize.Name = "trk_StickerSize";
            this.trk_StickerSize.Size = new System.Drawing.Size(104, 45);
            this.trk_StickerSize.TabIndex = 0;
            this.trk_StickerSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_StickerSize.Value = 1;
            this.trk_StickerSize.ValueChanged += new System.EventHandler(this.trk_faceSep_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Block Size";
            // 
            // trk_BlockSize
            // 
            this.trk_BlockSize.Location = new System.Drawing.Point(97, 43);
            this.trk_BlockSize.Maximum = 50;
            this.trk_BlockSize.Minimum = 1;
            this.trk_BlockSize.Name = "trk_BlockSize";
            this.trk_BlockSize.Size = new System.Drawing.Size(104, 45);
            this.trk_BlockSize.TabIndex = 0;
            this.trk_BlockSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_BlockSize.Value = 1;
            this.trk_BlockSize.ValueChanged += new System.EventHandler(this.trk_faceSep_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Face Separation";
            // 
            // trk_faceSep
            // 
            this.trk_faceSep.Location = new System.Drawing.Point(97, 20);
            this.trk_faceSep.Maximum = 50;
            this.trk_faceSep.Name = "trk_faceSep";
            this.trk_faceSep.Size = new System.Drawing.Size(104, 45);
            this.trk_faceSep.TabIndex = 0;
            this.trk_faceSep.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_faceSep.ValueChanged += new System.EventHandler(this.trk_faceSep_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Controls.Add(this.statusStrip1);
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(774, 684);
            this.panel2.TabIndex = 20;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.pb_ClickStatus,
            this.ms_Twists,
            this.ms_MacroStatus,
            this.m_lblCTime,
            this.activeKeybind});
            this.statusStrip1.Location = new System.Drawing.Point(0, 661);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(774, 23);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(74, 18);
            this.toolStripStatusLabel1.Text = "Click Status: ";
            // 
            // pb_ClickStatus
            // 
            this.pb_ClickStatus.Name = "pb_ClickStatus";
            this.pb_ClickStatus.Size = new System.Drawing.Size(100, 17);
            this.pb_ClickStatus.Step = 25;
            this.pb_ClickStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // ms_Twists
            // 
            this.ms_Twists.Name = "ms_Twists";
            this.ms_Twists.Size = new System.Drawing.Size(63, 18);
            this.ms_Twists.Text = "  Twists: 0  ";
            // 
            // ms_MacroStatus
            // 
            this.ms_MacroStatus.Name = "ms_MacroStatus";
            this.ms_MacroStatus.Size = new System.Drawing.Size(39, 18);
            this.ms_MacroStatus.Text = "Ready";
            // 
            // m_lblCTime
            // 
            this.m_lblCTime.Name = "m_lblCTime";
            this.m_lblCTime.Size = new System.Drawing.Size(91, 18);
            this.m_lblCTime.Text = "   Time: 00:00:00";
            // 
            // activeKeybind
            // 
            this.activeKeybind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.activeKeybind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.activeKeybind.Name = "activeKeybind";
            this.activeKeybind.Size = new System.Drawing.Size(74, 21);
            this.activeKeybind.Text = "Keybinds: ";
            this.activeKeybind.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.puzzleToolStripMenuItem,
            this.macroToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(774, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Open,
            this.mi_Save,
            this.mi_SaveAs,
            this.toolStripMenuItem1,
            this.mi_Exit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mi_Open
            // 
            this.mi_Open.Name = "mi_Open";
            this.mi_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mi_Open.Size = new System.Drawing.Size(146, 22);
            this.mi_Open.Text = "Open";
            this.mi_Open.Click += new System.EventHandler(this.mi_Open_Click);
            // 
            // mi_Save
            // 
            this.mi_Save.Name = "mi_Save";
            this.mi_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mi_Save.Size = new System.Drawing.Size(146, 22);
            this.mi_Save.Text = "Save";
            this.mi_Save.Click += new System.EventHandler(this.mi_Save_Click);
            // 
            // mi_SaveAs
            // 
            this.mi_SaveAs.Name = "mi_SaveAs";
            this.mi_SaveAs.Size = new System.Drawing.Size(146, 22);
            this.mi_SaveAs.Text = "Save As...";
            this.mi_SaveAs.Click += new System.EventHandler(this.mi_SaveAs_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 6);
            // 
            // mi_Exit
            // 
            this.mi_Exit.Name = "mi_Exit";
            this.mi_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mi_Exit.Size = new System.Drawing.Size(146, 22);
            this.mi_Exit.Text = "Exit";
            this.mi_Exit.Click += new System.EventHandler(this.mi_Exit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Reset,
            this.mi_FullScramble,
            this.mi_ScrambleNTurns,
            this.toolStripSeparator1,
            this.mi_Undo,
            this.mi_Redo,
            this.mi_FullUndo,
            this.mi_FullRedo,
            this.stopToolStripMenuItem,
            this.toolStripMenuItem4,
            this.recalculateToolStripMenuItem,
            this.toolStripSeparator2,
            this.editKeybinds});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            //
            // viewToolStripMenuItem
            //
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_ShowSidePanel});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            //
            // mi_ShowSidePanel
            //
            this.mi_ShowSidePanel.Checked = true;
            this.mi_ShowSidePanel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mi_ShowSidePanel.Name = "mi_ShowSidePanel";
            this.mi_ShowSidePanel.Size = new System.Drawing.Size(165, 22);
            this.mi_ShowSidePanel.Text = "Show Side Panel";
            this.mi_ShowSidePanel.Click += new System.EventHandler(this.mi_ShowSidePanel_Click);
            //
            // mi_Reset
            //
            this.mi_Reset.Name = "mi_Reset";
            this.mi_Reset.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.mi_Reset.Size = new System.Drawing.Size(165, 22);
            this.mi_Reset.Text = "Reset";
            this.mi_Reset.Click += new System.EventHandler(this.mi_Reset_Click);
            // 
            // mi_FullScramble
            // 
            this.mi_FullScramble.Name = "mi_FullScramble";
            this.mi_FullScramble.Size = new System.Drawing.Size(165, 22);
            this.mi_FullScramble.Text = "Full Scramble";
            this.mi_FullScramble.Click += new System.EventHandler(this.mi_FullScramble_Click);
            // 
            // mi_ScrambleNTurns
            // 
            this.mi_ScrambleNTurns.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Scramble1,
            this.mi_Scramble2,
            this.mi_Scramble3,
            this.mi_Scramble4,
            this.mi_Scramble5});
            this.mi_ScrambleNTurns.Name = "mi_ScrambleNTurns";
            this.mi_ScrambleNTurns.Size = new System.Drawing.Size(165, 22);
            this.mi_ScrambleNTurns.Text = "Scramble N turns";
            // 
            // mi_Scramble1
            // 
            this.mi_Scramble1.Name = "mi_Scramble1";
            this.mi_Scramble1.Size = new System.Drawing.Size(80, 22);
            this.mi_Scramble1.Text = "1";
            this.mi_Scramble1.Click += new System.EventHandler(this.mi_Scramble1_Click);
            // 
            // mi_Scramble2
            // 
            this.mi_Scramble2.Name = "mi_Scramble2";
            this.mi_Scramble2.Size = new System.Drawing.Size(80, 22);
            this.mi_Scramble2.Text = "2";
            this.mi_Scramble2.Click += new System.EventHandler(this.mi_Scramble2_Click);
            // 
            // mi_Scramble3
            // 
            this.mi_Scramble3.Name = "mi_Scramble3";
            this.mi_Scramble3.Size = new System.Drawing.Size(80, 22);
            this.mi_Scramble3.Text = "3";
            this.mi_Scramble3.Click += new System.EventHandler(this.mi_Scramble3_Click);
            // 
            // mi_Scramble4
            // 
            this.mi_Scramble4.Name = "mi_Scramble4";
            this.mi_Scramble4.Size = new System.Drawing.Size(80, 22);
            this.mi_Scramble4.Text = "4";
            this.mi_Scramble4.Click += new System.EventHandler(this.mi_Scramble4_Click);
            // 
            // mi_Scramble5
            // 
            this.mi_Scramble5.Name = "mi_Scramble5";
            this.mi_Scramble5.Size = new System.Drawing.Size(80, 22);
            this.mi_Scramble5.Text = "5";
            this.mi_Scramble5.Click += new System.EventHandler(this.mi_Scramble5_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
            // 
            // mi_Undo
            // 
            this.mi_Undo.Name = "mi_Undo";
            this.mi_Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.mi_Undo.Size = new System.Drawing.Size(165, 22);
            this.mi_Undo.Text = "Undo";
            this.mi_Undo.Click += new System.EventHandler(this.mi_Undo_Click);
            // 
            // mi_Redo
            // 
            this.mi_Redo.Name = "mi_Redo";
            this.mi_Redo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.mi_Redo.Size = new System.Drawing.Size(165, 22);
            this.mi_Redo.Text = "Redo";
            this.mi_Redo.Click += new System.EventHandler(this.mi_Redo_Click);
            // 
            // mi_FullUndo
            // 
            this.mi_FullUndo.Name = "mi_FullUndo";
            this.mi_FullUndo.Size = new System.Drawing.Size(165, 22);
            this.mi_FullUndo.Text = "Full Undo";
            this.mi_FullUndo.Click += new System.EventHandler(this.mi_FullUndo_Click);
            // 
            // mi_FullRedo
            // 
            this.mi_FullRedo.Name = "mi_FullRedo";
            this.mi_FullRedo.Size = new System.Drawing.Size(165, 22);
            this.mi_FullRedo.Text = "Full Redo";
            this.mi_FullRedo.Click += new System.EventHandler(this.mi_FullRedo_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(162, 6);
            // 
            // recalculateToolStripMenuItem
            // 
            this.recalculateToolStripMenuItem.Name = "recalculateToolStripMenuItem";
            this.recalculateToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.recalculateToolStripMenuItem.Text = "Recalculate";
            this.recalculateToolStripMenuItem.Click += new System.EventHandler(this.recalculateToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(162, 6);
            // 
            // editKeybinds
            // 
            this.editKeybinds.Name = "editKeybinds";
            this.editKeybinds.Size = new System.Drawing.Size(165, 22);
            this.editKeybinds.Text = "Edit Keybinds";
            this.editKeybinds.Click += new System.EventHandler(this.editKeybinds_Click);
            // 
            // puzzleToolStripMenuItem
            // 
            this.puzzleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Puzzle4D,
            this.mi_Puzzle5D,
            this.mi_Puzzle6D,
            this.mi_Puzzle7D,
            this.toolStripMenuItem2,
            this.mi_PuzzleSize2,
            this.mi_PuzzleSize3,
            this.mi_PuzzleSize4,
            this.mi_PuzzleSize5,
            this.mi_PuzzleSize6,
            this.mi_PuzzleSize7});
            this.puzzleToolStripMenuItem.Name = "puzzleToolStripMenuItem";
            this.puzzleToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.puzzleToolStripMenuItem.Text = "Puzzle";
            // 
            // mi_Puzzle4D
            // 
            this.mi_Puzzle4D.Name = "mi_Puzzle4D";
            this.mi_Puzzle4D.Size = new System.Drawing.Size(103, 22);
            this.mi_Puzzle4D.Text = "4D";
            this.mi_Puzzle4D.Click += new System.EventHandler(this.mi_Puzzle4D_Click);
            // 
            // mi_Puzzle5D
            // 
            this.mi_Puzzle5D.Name = "mi_Puzzle5D";
            this.mi_Puzzle5D.Size = new System.Drawing.Size(103, 22);
            this.mi_Puzzle5D.Text = "5D";
            this.mi_Puzzle5D.Click += new System.EventHandler(this.mi_Puzzle5D_Click);
            // 
            // mi_Puzzle6D
            // 
            this.mi_Puzzle6D.Name = "mi_Puzzle6D";
            this.mi_Puzzle6D.Size = new System.Drawing.Size(103, 22);
            this.mi_Puzzle6D.Text = "6D";
            this.mi_Puzzle6D.Click += new System.EventHandler(this.mi_Puzzle6D_Click);
            // 
            // mi_Puzzle7D
            // 
            this.mi_Puzzle7D.Checked = true;
            this.mi_Puzzle7D.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mi_Puzzle7D.Name = "mi_Puzzle7D";
            this.mi_Puzzle7D.Size = new System.Drawing.Size(103, 22);
            this.mi_Puzzle7D.Text = "7D";
            this.mi_Puzzle7D.Click += new System.EventHandler(this.mi_Puzzle7D_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(100, 6);
            // 
            // mi_PuzzleSize2
            // 
            this.mi_PuzzleSize2.Name = "mi_PuzzleSize2";
            this.mi_PuzzleSize2.Size = new System.Drawing.Size(103, 22);
            this.mi_PuzzleSize2.Text = "Size 2";
            this.mi_PuzzleSize2.Click += new System.EventHandler(this.mi_PuzzleSize2_Click);
            // 
            // mi_PuzzleSize3
            // 
            this.mi_PuzzleSize3.Checked = true;
            this.mi_PuzzleSize3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mi_PuzzleSize3.Name = "mi_PuzzleSize3";
            this.mi_PuzzleSize3.Size = new System.Drawing.Size(103, 22);
            this.mi_PuzzleSize3.Text = "Size 3";
            this.mi_PuzzleSize3.Click += new System.EventHandler(this.mi_PuzzleSize3_Click);
            // 
            // mi_PuzzleSize4
            // 
            this.mi_PuzzleSize4.Name = "mi_PuzzleSize4";
            this.mi_PuzzleSize4.Size = new System.Drawing.Size(103, 22);
            this.mi_PuzzleSize4.Text = "Size 4";
            this.mi_PuzzleSize4.Click += new System.EventHandler(this.mi_PuzzleSize4_Click);
            // 
            // mi_PuzzleSize5
            // 
            this.mi_PuzzleSize5.Name = "mi_PuzzleSize5";
            this.mi_PuzzleSize5.Size = new System.Drawing.Size(103, 22);
            this.mi_PuzzleSize5.Text = "Size 5";
            this.mi_PuzzleSize5.Click += new System.EventHandler(this.mi_PuzzleSize5_Click);
            // 
            // mi_PuzzleSize6
            // 
            this.mi_PuzzleSize6.Name = "mi_PuzzleSize6";
            this.mi_PuzzleSize6.Size = new System.Drawing.Size(103, 22);
            this.mi_PuzzleSize6.Text = "Size 6";
            this.mi_PuzzleSize6.Click += new System.EventHandler(this.mi_PuzzleSize6_Click);
            // 
            // mi_PuzzleSize7
            // 
            this.mi_PuzzleSize7.Name = "mi_PuzzleSize7";
            this.mi_PuzzleSize7.Size = new System.Drawing.Size(103, 22);
            this.mi_PuzzleSize7.Text = "Size 7";
            this.mi_PuzzleSize7.Click += new System.EventHandler(this.mi_PuzzleSize7_Click);
            // 
            // macroToolStripMenuItem
            // 
            this.macroToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_StartRecordig,
            this.loadMacroFileToolStripMenuItem,
            this.saveMacroFileToolStripMenuItem,
            this.saveMacroFileAsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.startExtraTurnsToolStripMenuItem,
            this.stopExtraTurnsToolStripMenuItem,
            this.undoExtraTurnsToolStripMenuItem,
            this.commutatorToolStripMenuItem});
            this.macroToolStripMenuItem.Name = "macroToolStripMenuItem";
            this.macroToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.macroToolStripMenuItem.Text = "Macros";
            // 
            // mi_StartRecordig
            // 
            this.mi_StartRecordig.Name = "mi_StartRecordig";
            this.mi_StartRecordig.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.mi_StartRecordig.Size = new System.Drawing.Size(222, 22);
            this.mi_StartRecordig.Text = "Start/Stop Recordig";
            this.mi_StartRecordig.Click += new System.EventHandler(this.mi_StartRecordig_Click);
            // 
            // loadMacroFileToolStripMenuItem
            // 
            this.loadMacroFileToolStripMenuItem.Name = "loadMacroFileToolStripMenuItem";
            this.loadMacroFileToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.loadMacroFileToolStripMenuItem.Text = "Load Macro File";
            this.loadMacroFileToolStripMenuItem.Click += new System.EventHandler(this.loadMacroFileToolStripMenuItem_Click);
            // 
            // saveMacroFileToolStripMenuItem
            // 
            this.saveMacroFileToolStripMenuItem.Name = "saveMacroFileToolStripMenuItem";
            this.saveMacroFileToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.saveMacroFileToolStripMenuItem.Text = "Save Macro File";
            this.saveMacroFileToolStripMenuItem.Click += new System.EventHandler(this.saveMacroFileToolStripMenuItem_Click);
            // 
            // saveMacroFileAsToolStripMenuItem
            // 
            this.saveMacroFileAsToolStripMenuItem.Name = "saveMacroFileAsToolStripMenuItem";
            this.saveMacroFileAsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.saveMacroFileAsToolStripMenuItem.Text = "Save Macro File As...";
            this.saveMacroFileAsToolStripMenuItem.Click += new System.EventHandler(this.saveMacroFileAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(219, 6);
            // 
            // startExtraTurnsToolStripMenuItem
            // 
            this.startExtraTurnsToolStripMenuItem.Name = "startExtraTurnsToolStripMenuItem";
            this.startExtraTurnsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.startExtraTurnsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.startExtraTurnsToolStripMenuItem.Text = "Start Extra Turns";
            this.startExtraTurnsToolStripMenuItem.Click += new System.EventHandler(this.startExtraTurnsToolStripMenuItem_Click);
            // 
            // stopExtraTurnsToolStripMenuItem
            // 
            this.stopExtraTurnsToolStripMenuItem.Name = "stopExtraTurnsToolStripMenuItem";
            this.stopExtraTurnsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.stopExtraTurnsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.stopExtraTurnsToolStripMenuItem.Text = "Stop Extra Turns";
            this.stopExtraTurnsToolStripMenuItem.Click += new System.EventHandler(this.stopExtraTurnsToolStripMenuItem_Click);
            // 
            // undoExtraTurnsToolStripMenuItem
            // 
            this.undoExtraTurnsToolStripMenuItem.Name = "undoExtraTurnsToolStripMenuItem";
            this.undoExtraTurnsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.undoExtraTurnsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.undoExtraTurnsToolStripMenuItem.Text = "Unwind Extra Turns";
            this.undoExtraTurnsToolStripMenuItem.Click += new System.EventHandler(this.undoExtraTurnsToolStripMenuItem_Click);
            // 
            // commutatorToolStripMenuItem
            // 
            this.commutatorToolStripMenuItem.Name = "commutatorToolStripMenuItem";
            this.commutatorToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.commutatorToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.commutatorToolStripMenuItem.Text = "Commutator";
            this.commutatorToolStripMenuItem.Click += new System.EventHandler(this.commutatorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(997, 684);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnTogglePanel);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "MC7D";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_trkFullUndoSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_trkTransparency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightSpec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightDiff)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_Perspective)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_StickerSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_BlockSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_faceSep)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        int GetDim() {
            if(mi_Puzzle4D.Checked) return 4;
            if(mi_Puzzle5D.Checked) return 5;
            if(mi_Puzzle6D.Checked) return 6;
            if(mi_Puzzle7D.Checked) return 7;
            return 4;
        }
        void SetDim(int n) {
            mi_Puzzle4D.Checked=(n==4);
            mi_Puzzle5D.Checked=(n==5);
            mi_Puzzle6D.Checked=(n==6);
            mi_Puzzle7D.Checked=(n==7);
        }
        int GetSize() {
            if(mi_PuzzleSize2.Checked) return 2;
            if(mi_PuzzleSize3.Checked) return 3;
            if(mi_PuzzleSize4.Checked) return 4;
            if(mi_PuzzleSize5.Checked) return 5;
            if(mi_PuzzleSize6.Checked) return 6;
            if(mi_PuzzleSize7.Checked) return 7;
            return 3;
        }
        void SetSize(int n)
        {
            mi_PuzzleSize2.Checked=(n==2);
            mi_PuzzleSize3.Checked=(n==3);
            mi_PuzzleSize4.Checked=(n==4);
            mi_PuzzleSize5.Checked=(n==5);
            mi_PuzzleSize6.Checked = (n == 6);
            mi_PuzzleSize7.Checked = (n == 7);
        }

        Cube7D Cube;
        CubeObj CubeView;
        int TRate=500;

        const int CLICK_MODE_2=0;
        const int CLICK_MODE_2_OPP=1;
        const int CLICK_MODE_3=2;

        int ClickMode=CLICK_MODE_2;
        int NClicks=0;
        int FaceClick=0;
        int FaceFrom=0;
        bool ClickQual=true;
        
        int TwistMask;     // pressed 1-5
        bool RotateCube;   // pressed Ctrl

        Keybindings Keybinds = new Keybindings();
        Form KeybindsSetup;

        bool AltHighlight=false;
        bool[] NColMask;
        int[] FaceMask;

        int DiffLight=150;
        int SpecLight=100;
        int Transparency=64;

        int RecordingMacroStatus=REC_MACRO_NONE;
        int OldRecMacroStatus=REC_MACRO_NONE;
        const int REC_MACRO_NONE=0;
        const int REC_MACRO_STICKERS=1;
        const int REC_MACRO_CODE=2;
        const int REC_MACRO_APPLY=3;

        string SettingsFileName;

        int[] MacroStickers=new int[100];
        int LMacroStickers=0;
        int[] MStickers;
        int LMStickers;
        int MacroStart=0;
        bool MacroReverse=false;
        CMacro CurMacro;
        CMacroFile Macros;

        int[,] RevStack=new int[100,2];
        int LRevStack=0;
        bool qSolved=true;

        long m_TStart=0;
        bool m_TRun=false;

        System.Threading.Timer m_Timer;

		int ClickX,ClickY;
        double cpath=0;
		private bool WantClick(){
			return true;
		}
		
		private void MouseDownEvt(object sender, MouseEventArgs e) {
            ClickX=e.X; ClickY=e.Y; cpath=0;
			MouseEvt(sender,e);
		}
        void addPath(int x,int y) {
            cpath+=MyMath.pyth(ClickX-x,ClickY-y);
            ClickY=y; ClickX=x;
        }
		private void MouseUpEvt(object sender, MouseEventArgs e) {
            addPath(e.X,e.Y);
			if(WantClick()){
				if(e.Clicks==2 || cpath<=6){
					ProcessClick(e);
				}
			}
			MouseEvt(sender,e);
		}
		
		private void MouseEvt(object sender, MouseEventArgs e) {
            addPath(e.X,e.Y);
			if(e.Button==MouseButtons.Left){
				dxControl2.ProcessMouse(e,ETarget.TargetCamera,null);
			}else{
				OnAction func=null;
				ETarget targ=ETarget.TargetCamera;
				dxControl2.ProcessMouse(e,targ,func);
			}
		}

        private void KeyDownEvt(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            var action = Keybinds.GetAction(key);

            if (action == null) return;

            bool redraw = false, didTwist = false;
            action.OnKeyDown(ref Cube, ref redraw, ref didTwist);
            PostKeybindAction(redraw, didTwist);
        }

        private void KeyUpEvt(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            var action = Keybinds.GetAction(key);

            if (action == null) return;

            bool redraw = false, didTwist = false;
            action.OnKeyUp(ref Cube, ref redraw, ref didTwist);
            PostKeybindAction(redraw, didTwist);
        }

        private void PostKeybindAction(bool redraw, bool didTwist)
        {
            if (redraw)
            {
                ProcessHighLights();
                Redraw();
            }

            if (didTwist)
            {
                TestBuild();
            }
        }

        private void CheckKeybindSet(object sender, EventArgs e)
        {
            string active = Keybinds.activeKeybindsName;
            activeKeybind.Text = $"Keybinds: {active}";
            foreach (ToolStripMenuItem item in activeKeybind.DropDownItems)
            {
                item.Checked = item.Name == active;
            }
        }

        private void ProcessHighLights()
        {
            if (cb_HighlightByColors.CheckState != CheckState.Unchecked)
                Cube.FindStickersByMask(FaceMask, cb_HighlightByColors.CheckState == CheckState.Checked);
            else Cube.HighlightAll();

            Cube.HighLightGrip();
        }

		
		public void ProcessClick(MouseEventArgs e){
            dxControl2.ProcessPick(e,ETarget.TargetObject,new OnAction(mkPickObject));
        }
	
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		void RefreshView(){
			dxControl2.ParkCamera(false);
			dxControl2.SetSceneChanged();
		}

     	void mkPickObject(EAction act,ref Vector3 pt,ref Vector3 vec,double ang){
            m_runUndo=false;
            int stk=CubeView.FindSticker(ref pt,ref vec);
            if(stk<0) {
                NClicks=0;
                ClickQual=true;
                switch(RecordingMacroStatus) {
                    case REC_MACRO_STICKERS: RecordingMacroStatus=OldRecMacroStatus=REC_MACRO_NONE; break;
                    case REC_MACRO_APPLY: RecordingMacroStatus=OldRecMacroStatus; break;
                }
                RedrawClickStatus();
                if(AltHighlight){
                    if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                        Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                    else Cube.HighlightAll();
                    AltHighlight=false;
                    Redraw();
                }
                return;
            }
            int ncl=NClicks;
            int fclick=FaceClick;
            int ffrom=FaceFrom;

            if(ncl==0) {   // first click
                switch(act) {
                    case EAction.ActionCtrlClick: {
                            bool rr=Cube.RotateCubeBySticker(stk);
                            if(rr) {
                                Redraw();
                                return;
                            }
                            FaceClick=Cube.GetFirstSticker(stk,ClickMode,out FaceFrom);
                            if(FaceClick!=0) {
                                NClicks=ClickMode==CLICK_MODE_3 ? 1 : 2;
                                ClickQual=true;
                                RotateCube=true;
                            } else {
                                NClicks=0;
                                ClickQual=false;
                            }
                            RedrawClickStatus();
                            return;
                        }
                    case EAction.ActionShiftClick: {
                            Cube.FindOtherStickers(stk);
                            AltHighlight=true;
                            Redraw();
                            return;
                        }
                    case EAction.ActionCtrlShiftClick: {
                            Cube.FindAdjStickers(stk);
                            AltHighlight=true;
                            Redraw();
                            return;
                        }
                    default: {
                            ClickQual=true;
                            if(RecordingMacroStatus==REC_MACRO_STICKERS || RecordingMacroStatus==REC_MACRO_APPLY) {
                                if(LMacroStickers==MacroStickers.Length) {
                                    int[] mm=new int[2*LMacroStickers];
                                    Buffer.BlockCopy(MacroStickers,0,mm,0,LMacroStickers*4);
                                    MacroStickers=mm;
                                }
                                MacroStickers[LMacroStickers++]=Cube.StkMap[stk];
                                if(RecordingMacroStatus==REC_MACRO_STICKERS) {
                                    bool qx=Cube.CheckStickerSet(MacroStickers,LMacroStickers);
                                    if(qx) {
                                        LMStickers=LMacroStickers;
                                        MStickers=new int[LMStickers];
                                        Buffer.BlockCopy(MacroStickers,0,MStickers,0,LMStickers*4);

                                        RecordingMacroStatus=REC_MACRO_CODE;
                                        MacroStart=Cube.LPtr;
                                    }
                                } else {
                                    if(LMacroStickers==CurMacro.NStickers) {
                                        int[] cmap=Cube.CmpStickerSet(CurMacro.Stickers,MacroStickers,LMacroStickers);
                                        if(cmap==null) {
                                            ClickQual=false;
                                            CurMacro=null;
                                        } else {
                                            Cube.ApplyMacro(cmap,CurMacro.Code,CurMacro.LMacro,MacroReverse);
                                            if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                                                Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                                            TestBuild();
                                            Redraw();
                                        }
                                        RecordingMacroStatus=OldRecMacroStatus;
                                    }
                                }
                            } else {
                                TwistMask=0;
                                int d=GetDim();
                                for(int i=0;i<d;i++) {
                                    if((S3DirectX.GetAsyncKeyState(0x31+i) & 0x8000) != 0) TwistMask|=(1<<i);
                                }
                                if(TwistMask==0) TwistMask=1;

                                FaceClick=Cube.GetFirstSticker(stk,ClickMode,out FaceFrom);
                                if(FaceClick!=0) {
                                    NClicks=ClickMode==CLICK_MODE_3 ? 1 : 2;
                                    RotateCube=false;
                                } else {
                                    NClicks=0;
                                    ClickQual=false;
                                }
                            }
                            RedrawClickStatus();
                            return;
                        }
                }
            } else if(ncl==1){  // only 3-mode
                FaceFrom=Cube.GetSecondSticker(stk,fclick);
                if(FaceFrom==0) {
                    ClickQual=false;
                    NClicks=0;
                } else {
                    ClickQual=true;
                    NClicks=2;
                }
                RedrawClickStatus();
            }else{  // last click
                bool rr=false;
                int ff=Cube.GetSecondSticker(stk,fclick);
                if(ff!=0) {
                    if(RotateCube) {
                        rr=Cube.RotateCubeByStickers(fclick,ffrom,ff);
                    } else {
                        rr=Cube.Twist(fclick,ffrom,ff,TwistMask);
                        if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                            Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                        TestBuild();
                    }
                }
                if(!rr) {
                    ClickQual=false;
                    NClicks=0;
                    RedrawClickStatus();
                    return;
                }
                ClickQual=true;
                FaceFrom=ff;
                NClicks=0;
                RedrawClickStatus();
                Redraw();
            }
		}

        void RedrawClickStatus() {
            pb_ClickStatus.ForeColor=ClickQual ? Color.Green : Color.Red;
            pb_ClickStatus.Value=NClicks*40+10;
            Color bg=Color.Black;
            if(RecordingMacroStatus==REC_MACRO_STICKERS || RecordingMacroStatus==REC_MACRO_APPLY) bg=Color.White;
            if(!ClickQual) bg=Color.DarkRed;
            dxControl2.Scene.BGColor=bg;

            switch(RecordingMacroStatus) {
                case REC_MACRO_NONE:
                    ms_MacroStatus.Text="  Ready"; break;
                case REC_MACRO_STICKERS:
                case REC_MACRO_APPLY:
                    ms_MacroStatus.Text="  Select Stickers: "+LMacroStickers; break;
                case REC_MACRO_CODE:
                    ms_MacroStatus.Text="  Enter macro: "+Cube.GetNTwists(MacroStart,Cube.LPtr); break;
            }
            UpdateTime(null);
        }




        void UpdateTime(object xxx) {
            if(Cube==null) return;
            long s=m_TRun ? (DateTime.Now.Ticks-m_TStart) : Cube.CTime;
            s/=10000; // ms
            int tms=(int)(s%1000); s/=1000;
            int ts=(int)(s%60); s/=60;
            int tm=(int)(s%60); s/=60;
            string m=String.Format("   Time: {0}:{1:D2}:{2:D2}.{3:D3}",s,tm,ts,tms);
            m_lblCTime.Text=m;
        }

        void Redraw() {
            ShowRevStack();
            CubeView.Dispose();  // colors changed
            dxControl2.SetSceneChanged();
        }

        void TestBuild() {
            if(qSolved) return;
            if(!m_TRun) {
                m_TStart=DateTime.Now.Ticks-Cube.CTime;
                m_TRun=true;
            }
            if(Cube.CheckCube()) {
                m_TRun=false;
                Cube.CTime=DateTime.Now.Ticks-m_TStart;
                RedrawClickStatus();
                MessageBox.Show(string.Format("You have solved {0}^{1} cube scrambled by {2} twists.\r\nCongratulations!",Cube.N,Cube.D,Cube.LShuffle));
                qSolved=true;
            }
        }

        /*********** load/save scene ************/
		void NewScene(){
			dxControl2.ClearMeshes();
            CubeView=null;
            Cube=new Cube7D();
            Cube.Init(GetSize(),GetDim());
            qSolved=true;

            if(Macros==null || !Macros.CheckSize(GetDim(),GetSize())) {
                Macros=new CMacroFile(GetDim(),GetSize());
                InitMacroList();
            }
            NClicks=0; ClickQual=true;
            LRevStack=0;
            RecordingMacroStatus=OldRecMacroStatus=REC_MACRO_NONE;

            m_TRun=false;
            ShowCube();
            dxControl2.ParkCamera(true);
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
        
        void ShowCube() {
            if(CubeView==null) {
                CubeView=new CubeObj();
                dxControl2.AddMesh(CubeView);
            }

            byte[] col;
            float[,] coord;
            int[] map;
            byte[] stkncol;
            BitArray hmask;

            int nstk=Cube.GetStickers(out col,out map,out coord,out hmask,out stkncol);
            CubeView.SetCoords(col,map,coord,stkncol,hmask,NColMask,nstk);
            if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
            dxControl2.SetSceneChanged();
            ShowRevStack();
            //            dxControl2.Invalidate();
        }

        private void mi_Puzzle4D_Click(object sender,EventArgs e) {
            SetDim(4);
            NewScene();
        }

        private void mi_Puzzle5D_Click(object sender,EventArgs e) {
            SetDim(5);
            NewScene();
        }

        private void mi_Puzzle6D_Click(object sender,EventArgs e) {
            SetDim(6);
            NewScene();
        }

        private void mi_Puzzle7D_Click(object sender,EventArgs e) {
            SetDim(7);
            NewScene();
        }

        private void mi_PuzzleSize2_Click(object sender, EventArgs e)
        {
            SetSize(2);
            NewScene();
        }
        private void mi_PuzzleSize3_Click(object sender,EventArgs e) {
            SetSize(3);
            NewScene();
        }

        private void mi_PuzzleSize4_Click(object sender,EventArgs e) {
            SetSize(4);
            NewScene();
        }

        private void mi_PuzzleSize5_Click(object sender,EventArgs e) {
            SetSize(5);
            NewScene();
        }

        private void mi_PuzzleSize6_Click(object sender, EventArgs e)
        {
            SetSize(6);
            NewScene();
        }

        private void mi_PuzzleSize7_Click(object sender, EventArgs e)
        {
            SetSize(7);
            NewScene();
        }
        
        

        private void mi_Reset_Click(object sender,EventArgs e) {
            NewScene();
        }

        private void mi_Undo_Click(object sender,EventArgs e) {
            bool r=Cube.Undo();
            if(r) {
                if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                    Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                Redraw();
            }
        }

        private void mi_Redo_Click(object sender,EventArgs e) {
            bool r=Cube.Redo();
            if(r) {
                if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                    Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                Redraw();
            }
        }

        private void Form1_KeyPress(object sender,KeyPressEventArgs e) {

        }

        private void mi_FullScramble_Click(object sender,EventArgs e) {
            Scramble(-1);
        }

        private void mi_Scramble1_Click(object sender,EventArgs e) {
            Scramble(1);
        }
        private void mi_Scramble2_Click(object sender,EventArgs e) {
            Scramble(2);
        }
        private void mi_Scramble3_Click(object sender,EventArgs e) {
            Scramble(3);
        }
        private void mi_Scramble4_Click(object sender,EventArgs e) {
            Scramble(4);
        }
        private void mi_Scramble5_Click(object sender,EventArgs e) {
            Scramble(5);
        }
        void Scramble(int N) {
            Cube.Scramble(N);
            NClicks=0; ClickQual=true;
            RecordingMacroStatus=OldRecMacroStatus=REC_MACRO_NONE;
            qSolved=false;
            m_TRun=false;
            ShowCube();
        }

        bool m_runUndo=false;
        private void mi_FullUndo_Click(object sender,EventArgs e) {
            m_runUndo=true;
            while(Cube.Undo()) {                
                Redraw();
                dxControl2.SetSceneChanged();
                dxControl2.Scene.Render3DEnvironment();
                Thread.Sleep(TRate);                
                Application.DoEvents(); 
                if(!m_runUndo) break;
            }
        }

        private void mi_FullRedo_Click(object sender,EventArgs e) {
            m_runUndo=true;
            while(Cube.Redo()) {
                Redraw();
                dxControl2.SetSceneChanged();
                dxControl2.Scene.Render3DEnvironment(); 
                Thread.Sleep(TRate);
                Application.DoEvents();
                if(!m_runUndo) break;
            }
        }

        private void stopToolStripMenuItem_Click(object sender,EventArgs e) {
            m_runUndo=false;
        }

        string m_FileName=null;

        private void mi_Open_Click(object sender,EventArgs e) {
            OpenFileDialog sf=new OpenFileDialog();
            sf.DefaultExt=".log";
            sf.Filter="MC7D Log file (*.log)|*.log";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                m_FileName=sf.FileName;
                Text=m_FileName+" - MC7D";
                Cube.Load(m_FileName);
                ShowCube();
                dxControl2.ParkCamera(true);
                NClicks=0; ClickQual=true;
                LRevStack=0;
                qSolved=Cube.CheckCube();
                SetDim(Cube.D);
                SetSize(Cube.N);
            }
        }

        private void mi_Save_Click(object sender,EventArgs e) {
            if(m_FileName==null) mi_SaveAs_Click(sender,e);
            if(m_TRun) Cube.CTime=DateTime.Now.Ticks-m_TStart;
            Cube.Save(m_FileName);
        }

        private void mi_SaveAs_Click(object sender,EventArgs e) {
            SaveFileDialog sf=new SaveFileDialog();
            sf.RestoreDirectory=true;
            sf.DefaultExt=".log";
            sf.Filter="MC7D Log file (*.log)|*.log";
            if(sf.ShowDialog()==DialogResult.OK) {
                m_FileName=sf.FileName;
                Text=m_FileName+" - MC7D";
                if(m_TRun) Cube.CTime=DateTime.Now.Ticks-m_TStart;
                Cube.Save(m_FileName);
            }
        }

        bool SettingsSaved=false;
        private void mi_Exit_Click(object sender,EventArgs e) {
            if(!SettingsSaved) {
                SaveSettings("MC7D_settings.txt");
                SaveKeybinds("MC7D_keybinds.txt");
                SettingsSaved =true;
            }
            Application.Exit();
        }
        private void Form1_FormClosing(object sender,FormClosingEventArgs e) {
            if(!SettingsSaved) {
                SaveSettings("MC7D_settings.txt");
                SaveKeybinds("MC7D_keybinds.txt");
                SettingsSaved =true;
            }
        }

        bool m_setgeom=false;
        void InitGeomSettings() {
            m_setgeom=true;
            trk_BlockSize.Value=Math.Max(1,(int)(Cube7D.BSize*trk_BlockSize.Maximum+0.5));
            trk_Perspective.Value=Math.Max(0,(int)(Cube7D.FExt/2*trk_Perspective.Maximum+0.5));
            trk_faceSep.Value=Math.Max(0,(int)(Cube7D.FSep*trk_faceSep.Maximum+0.5));
            trk_StickerSize.Value=Math.Max(1,(int)(Cube7D.SSize*trk_StickerSize.Maximum+0.5));
            m_bCol1.BackColor=Color.FromArgb((int)CubeObj.Colors[1]);
            m_bCol2.BackColor=Color.FromArgb((int)CubeObj.Colors[2]);
            m_bCol3.BackColor=Color.FromArgb((int)CubeObj.Colors[3]);
            m_bCol4.BackColor=Color.FromArgb((int)CubeObj.Colors[4]);
            m_bCol5.BackColor=Color.FromArgb((int)CubeObj.Colors[5]);
            m_bCol6.BackColor=Color.FromArgb((int)CubeObj.Colors[6]);
            m_bCol7.BackColor=Color.FromArgb((int)CubeObj.Colors[7]);
            m_bCol8.BackColor=Color.FromArgb((int)CubeObj.Colors[8]);
            m_bCol9.BackColor=Color.FromArgb((int)CubeObj.Colors[9]);
            m_bCol10.BackColor=Color.FromArgb((int)CubeObj.Colors[10]);
            m_bCol11.BackColor=Color.FromArgb((int)CubeObj.Colors[11]);
            m_bCol12.BackColor=Color.FromArgb((int)CubeObj.Colors[12]);
            m_bCol13.BackColor=Color.FromArgb((int)CubeObj.Colors[13]);
            m_bCol14.BackColor=Color.FromArgb((int)CubeObj.Colors[14]);
            switch(ClickMode) {
                case CLICK_MODE_2: m_rbClick2.Checked=true; break;
                case CLICK_MODE_2_OPP: m_rbClick2Inv.Checked=true; break;
                case CLICK_MODE_3: m_rbClick3.Checked=true; break;
            }
            CheckState[] st=new CheckState[] { CheckState.Unchecked,CheckState.Indeterminate,CheckState.Checked };
            cb_Col1.CheckState=st[FaceMask[1]+1];
            cb_Col2.CheckState=st[FaceMask[2]+1];
            cb_Col3.CheckState=st[FaceMask[3]+1];
            cb_Col4.CheckState=st[FaceMask[4]+1];
            cb_Col5.CheckState=st[FaceMask[5]+1];
            cb_Col6.CheckState=st[FaceMask[6]+1];
            cb_Col7.CheckState=st[FaceMask[7]+1];
            cb_Col8.CheckState=st[FaceMask[8]+1];
            cb_Col9.CheckState=st[FaceMask[9]+1];
            cb_Col10.CheckState=st[FaceMask[10]+1];
            cb_Col11.CheckState=st[FaceMask[11]+1];
            cb_Col12.CheckState=st[FaceMask[12]+1];
            cb_Col13.CheckState=st[FaceMask[13]+1];
            cb_Col14.CheckState=st[FaceMask[14]+1];

            cb_Show1C.Checked=NColMask[1];
            cb_Show2C.Checked=NColMask[2];
            cb_Show3C.Checked=NColMask[3];
            cb_Show4C.Checked=NColMask[4];
            cb_Show5C.Checked=NColMask[5];
            cb_Show6C.Checked=NColMask[6];
            cb_Show7C.Checked=NColMask[7];

            trk_LightDiff.Value=DiffLight;
            trk_LightSpec.Value=SpecLight;
            m_trkTransparency.Value=255-CubeObj.Transparency;
            m_setgeom=false;
        }





        private void trk_faceSep_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom) {
                Cube7D.BSize=(double)trk_BlockSize.Value/trk_BlockSize.Maximum;
                Cube7D.FExt=2.0*trk_Perspective.Value/trk_Perspective.Maximum;
                Cube7D.FSep=(double)trk_faceSep.Value/trk_faceSep.Maximum;
                Cube7D.SSize=(double)trk_StickerSize.Value/trk_StickerSize.Maximum;
                if(Cube!=null) {
                    Cube.SetCoords();
                    Redraw();
                }
            }
        }

        private void m_bCol1_Click(object sender,EventArgs e) { ChangeColor(m_bCol1,1); }
        private void m_bCol2_Click(object sender,EventArgs e) { ChangeColor(m_bCol2,2); }
        private void m_bCol3_Click(object sender,EventArgs e) { ChangeColor(m_bCol3,3); }
        private void m_bCol4_Click(object sender,EventArgs e) { ChangeColor(m_bCol4,4); }
        private void m_bCol5_Click(object sender,EventArgs e) { ChangeColor(m_bCol5,5); }
        private void m_bCol6_Click(object sender,EventArgs e) { ChangeColor(m_bCol6,6); }
        private void m_bCol7_Click(object sender,EventArgs e) { ChangeColor(m_bCol7,7); }
        private void m_bCol8_Click(object sender,EventArgs e) { ChangeColor(m_bCol8,8); }
        private void m_bCol9_Click(object sender,EventArgs e) { ChangeColor(m_bCol9,9); }
        private void m_bCol10_Click(object sender,EventArgs e) { ChangeColor(m_bCol10,10); }
        private void m_bCol11_Click(object sender,EventArgs e) { ChangeColor(m_bCol11,11); }
        private void m_bCol12_Click(object sender,EventArgs e) { ChangeColor(m_bCol12,12); }
        private void m_bCol13_Click(object sender,EventArgs e) { ChangeColor(m_bCol13,13); }
        private void m_bCol14_Click(object sender,EventArgs e) { ChangeColor(m_bCol14,14); }
        void ChangeColor(Button btn,int cid){
            ColorDialog c=new ColorDialog();
            c.Color=btn.BackColor;
            if(c.ShowDialog()==DialogResult.OK) {
                btn.BackColor=c.Color;
                CubeObj.Colors[cid]=(uint)(c.Color.ToArgb());
                Redraw();
            }
        }

        private void ClickMode_CheckedChanged(object sender,EventArgs e) {
            if(m_rbClick3.Checked) ClickMode=CLICK_MODE_3;
            else if(m_rbClick2Inv.Checked) ClickMode=CLICK_MODE_2_OPP;
            else ClickMode=CLICK_MODE_2;
        }

        void SaveSettings(string fn) {
            try {
                StreamWriter sw=new StreamWriter(fn);
                sw.NewLine="\r\n";
                sw.WriteLine("MC7D Settings");
                sw.WriteLine("Size {0} {1}",GetDim(),GetSize());
                sw.WriteLine("BlockSize {0:F2}",Cube7D.BSize);
                sw.WriteLine("StickerSize {0:F2}",Cube7D.SSize);
                sw.WriteLine("FaceSeparation {0:F2}",Cube7D.FSep);
                sw.WriteLine("Perspective {0:F2}",Cube7D.FExt);
                string ln="Colors";
                for(int i=1;i<=14;i++) ln+=" "+CubeObj.Colors[i];
                sw.WriteLine(ln);
                sw.WriteLine("ClickMode {0}",ClickMode+1);
                ln="ShowFaces";
                for(int i=1;i<=14;i++) ln+=" "+FaceMask[i];
                sw.WriteLine(ln);
                ln="ShowNColors";
                for(int i=1;i<=7;i++) ln+=NColMask[i] ? " T" : " F";
                sw.WriteLine(ln);
                sw.WriteLine("DiffLight {0}",DiffLight);
                sw.WriteLine("SpecLight {0}",SpecLight);
                sw.WriteLine("Transparency {0}",CubeObj.Transparency);
                sw.WriteLine("QuickMacro {0}",m_cbQuickMacro.Checked ? "T" : "F");
                if(m_FileName!=null) sw.WriteLine("FileName "+m_FileName);
                sw.Close();
            } catch { }
        }
        void LoadSettings(string fn) {
            if(File.Exists(fn)) {
                try {
                    StreamReader sr=new StreamReader(fn);
                    string ln=sr.ReadLine();
                    if(ln!="MC7D Settings") return;
                    for(;;) {
                        ln=sr.ReadLine();
                        if(ln==null) break;
                        string[] pars=ln.Split(' ');
                        switch(pars[0]) {
                            case "Size":
                                SetDim(int.Parse(pars[1]));
                                SetSize(int.Parse(pars[2]));
                                break;
                            case "BlockSize":
                                Cube7D.BSize=double.Parse(pars[1]); break;
                            case "StickerSize":
                                Cube7D.SSize=double.Parse(pars[1]); break;
                            case "FaceSeparation":
                                Cube7D.FSep=double.Parse(pars[1]); break;
                            case "Perspective":
                                Cube7D.FExt=double.Parse(pars[1]); break;
                            case "Colors":
                                for(int i=1;i<=14 && i<pars.Length;i++) CubeObj.Colors[i]=uint.Parse(pars[i]);
                                break;
                            case "ClickMode":
                                ClickMode=int.Parse(pars[1])-1;
                                break;
                            case "FileName":
                                m_FileName=ln.Substring(9);
                                break;
                            case "DiffLight":
                                DiffLight=int.Parse(pars[1]);
                                break;
                            case "SpecLight":
                                SpecLight=int.Parse(pars[1]);
                                break;
                            case "Transparency":
                                CubeObj.Transparency=int.Parse(pars[1]);
                                break;
                            case "ShowFaces":
                                for(int i=1;i<=14;i++) FaceMask[i]=int.Parse(pars[i]);
                                break;
                            case "ShowNColors":
                                for(int i=1;i<=7;i++) NColMask[i]=(pars[i][0]=='T');
                                break;     
                            case "QuickMacro":
                                m_cbQuickMacro.Checked=(pars[1][0]=='T');
                                break;
                        }
                    }
                    sr.Close();
                } catch { }
            }

            InitGeomSettings();
            SetLight();
            if(m_FileName==null) NewScene();
            else {
                Text=m_FileName+" - MC7D";
                Cube=new Cube7D();
                Cube.Load(m_FileName);
                ShowCube();
                SetDim(Cube.D); SetSize(Cube.N);
                dxControl2.ParkCamera(true);
                NClicks=0; ClickQual=true;
                LRevStack=0;
                qSolved=Cube.CheckCube();


            }
        }

        void SaveKeybinds(string fn)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fn);
                sw.NewLine = "\r\n";
                sw.WriteLine("MC7D Keybinds");
                sw.Write(Keybinds.Serialize());
                sw.WriteLine();
                sw.Close();
            }
            catch { }
        }

        void LoadKeybinds(string fn)
        {
            StreamReader sr = null;
            if (File.Exists(fn))
            {
                try
                {
                    sr = new StreamReader(fn);
                    string ln = sr.ReadLine();
                    if (ln != "MC7D Keybinds") return;

                    int idx = 1;

                    for (; ; )
                    {
                        ln = sr.ReadLine();
                        idx++;
                        if (ln == null) break;

                        Keybinds.LoadKeybindSet(ln, idx);
                    }
                }
                catch {
                }
            }
            if (sr != null)
            {
                sr.Close();
            }
        }

        void UpdateKeybindMenu(object sender, EventArgs e)
        {
            activeKeybind.DropDownItems.Clear();
            foreach (var item in Keybinds.keybinds)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = item.Key;
                menuItem.Click += new System.EventHandler(this.KeybindMenuItem_Click);

                activeKeybind.DropDownItems.Add(menuItem);
            }
            this.CheckKeybindSet(sender, e);
        }

        void KeybindMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Keybinds.switchKeybindSet(item.Text);
        }

        void SetLight() {
            foreach(CameraLight L in dxControl2.Scene.Lights) {
                L.Specular=SpecLight;
                L.Diffuse=DiffLight;
            }
            dxControl2.SetSceneChanged();
        }

        private void trk_LightDiff_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom){
            DiffLight=trk_LightDiff.Value;
            SetLight();
            }
        }

        private void trk_LightSpec_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom){
            SpecLight=trk_LightSpec.Value;
            SetLight();
            }
        }

        int chstate(CheckState v) {
            return v==CheckState.Checked ? 1 : v==CheckState.Unchecked ? -1 : 0;
        }
        private void cb_Col1_CheckStateChanged(object sender,EventArgs e) {
            if(!m_setgeom){
                FaceMask[1]=chstate(cb_Col1.CheckState);
                FaceMask[2]=chstate(cb_Col2.CheckState);
                FaceMask[3]=chstate(cb_Col3.CheckState);
                FaceMask[4]=chstate(cb_Col4.CheckState);
                FaceMask[5]=chstate(cb_Col5.CheckState);
                FaceMask[6]=chstate(cb_Col6.CheckState);
                FaceMask[7]=chstate(cb_Col7.CheckState);
                FaceMask[8]=chstate(cb_Col8.CheckState);
                FaceMask[9]=chstate(cb_Col9.CheckState);
                FaceMask[10]=chstate(cb_Col10.CheckState);
                FaceMask[11]=chstate(cb_Col11.CheckState);
                FaceMask[12]=chstate(cb_Col12.CheckState);
                FaceMask[13]=chstate(cb_Col13.CheckState);
                FaceMask[14]=chstate(cb_Col14.CheckState);
                if(cb_HighlightByColors.CheckState!=CheckState.Unchecked){
                    Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                    Redraw();
                }
            }
        }
        private void cb_HighlightByColors_CheckedChanged(object sender,EventArgs e) {
            if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
            else Cube.HighlightAll();
            Redraw();
        }

        private void cb_Show1C_CheckedChanged(object sender,EventArgs e) {
            if(!m_setgeom) {
                NColMask[1]=cb_Show1C.Checked;
                NColMask[2]=cb_Show2C.Checked;
                NColMask[3]=cb_Show3C.Checked;
                NColMask[4]=cb_Show4C.Checked;
                NColMask[5]=cb_Show5C.Checked;
                NColMask[6]=cb_Show6C.Checked;
                NColMask[7]=cb_Show7C.Checked;
                Redraw();
            }
        }

        double[] m_macroVec;
        int[] m_macroOrient;
        string m_macroName;

        private void mi_StartRecordig_Click(object sender,EventArgs e) {
            if(RecordingMacroStatus!=REC_MACRO_CODE) {
//                MessageBox.Show("Click Reference Stickers");
                RecordingMacroStatus=REC_MACRO_STICKERS;
                m_macroVec=GetMatrix();
                m_macroOrient=(int[])Cube.Orient.Clone();
                LMacroStickers=0;
            } else {
                if(Cube.LPtr<=MacroStart) {
                    MessageBox.Show("Negative Macro Length");
                } else {
                    TextDialog edt=new TextDialog("Enter Macro Name");
                    DialogResult res=edt.ShowDialog(this);
                    if(res==DialogResult.OK) {
                        CMacro m=new CMacro(edt.Value,LMStickers,MStickers,Cube.LPtr-MacroStart,Cube.Seq,MacroStart);
                        m.Vectors=m_macroVec;
                        m.Orient=m_macroOrient;
                        Macros.AddMacro(m);
                        if(m_lbMacros.FindStringExact(m.Name)==ListBox.NoMatches) {
                            m_lbMacros.Items.Add(m.Name);
                        }
                    }
                }
                RecordingMacroStatus=REC_MACRO_NONE;
            }
            RedrawClickStatus();
        }

        void InitMacroList(){
            string[] str=Macros.GetNamesList();
            m_lbMacros.Items.Clear();
            foreach(string s in str) m_lbMacros.Items.Add(s);
        }

        private void m_lbMacros_MouseClick(object sender,MouseEventArgs e) {
            m_macroName=(string)m_lbMacros.SelectedItem;
            if(m_macroName==null) return;
            if(m_RunByClick.Checked) {
                m_macroName=(string)m_lbMacros.SelectedItem;
                appMacro(false);
                return;
            }

            ContextMenu c=new ContextMenu();
            c.MenuItems.Add(new MenuItem("Apply",ApplyMacro));
            c.MenuItems.Add(new MenuItem("Reverse",ReverseMacro));
            c.MenuItems.Add(new MenuItem("Delete",DeleteMacro));
            c.MenuItems.Add(new MenuItem("Rename",RenameMacro));
            c.Show(m_lbMacros,e.Location);
        }

        private void m_lbMacros_MouseDoubleClick(object sender,MouseEventArgs e) {
            m_macroName=(string)m_lbMacros.SelectedItem;
            if(m_macroName==null) return;
            appMacro(false);
        }

        private void ApplyMacro(object sender,EventArgs e) {
            appMacro(false);
        }
        void appMacro(bool qrev){
            CurMacro=Macros.GetMacro(m_macroName);
            if(CurMacro==null) {
                ClickQual=false;
                return;
            }
            if(m_cbQuickMacro.Checked && CurMacro.Vectors!=null) {
                int[] cmap=GetFastMacroRef(CurMacro.Vectors,CurMacro.Orient);
                Cube.ApplyMacro(cmap,CurMacro.Code,CurMacro.LMacro,qrev);
                if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                    Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                TestBuild();
                Redraw();
                return;
            }
            OldRecMacroStatus=RecordingMacroStatus;
            RecordingMacroStatus=REC_MACRO_APPLY;
            LMacroStickers=0;
            MacroReverse=qrev;
            RedrawClickStatus();
        }
        private void ReverseMacro(object sender,EventArgs e) {
            appMacro(true);
        }
        private void DeleteMacro(object sender,EventArgs e) {
            Macros.Delete(m_macroName);
            m_lbMacros.Items.Remove(m_macroName);
        }
        private void RenameMacro(object sender,EventArgs e) {
            TextDialog edt=new TextDialog("Enter New Macro Name");
            edt.Value=m_macroName;
            DialogResult res=edt.ShowDialog(this);
            if(res==DialogResult.OK) {
                Macros.Rename(m_macroName,edt.Value);
                InitMacroList();
            }
        }

        private void saveMacroFileToolStripMenuItem_Click(object sender,EventArgs e) {
            if(Macros.FileName==null) saveMacroFileAsToolStripMenuItem_Click(sender,e);
            Macros.Save();
        }

        private void loadMacroFileToolStripMenuItem_Click(object sender,EventArgs e) {
            OpenFileDialog sf=new OpenFileDialog();
            sf.DefaultExt=".dat";
            sf.Filter="MC7D Macro file (*.dat)|*.dat";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                CMacroFile mf=new CMacroFile(sf.FileName);
                if(!mf.CheckSize(GetDim(),GetSize())) {
                    MessageBox.Show("Wrong cube size");
                    return;
                }
                Macros=mf;
                InitMacroList();
            }
        }

        private void saveMacroFileAsToolStripMenuItem_Click(object sender,EventArgs e) {
            SaveFileDialog sf=new SaveFileDialog();
            sf.DefaultExt=".dat";
            sf.Filter="MC7D Macro file (*.dat)|*.dat";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                Macros.SaveAs(sf.FileName);
            }
        }

        double[] GetMatrix() {
            Camera cam=dxControl2.Scene.Camera;
            Matrix M=Microsoft.DirectX.Matrix.LookAtLH(
				cam.CameraPosition,
				cam.PointToLookAt,
				cam.UpVector);
            double[] res=new double[] { M.M11,M.M12,M.M13,M.M21,M.M22,M.M23,M.M31,M.M32,M.M33 };
            return res;
        }
        double VPr(double []v,int ia,double []w,int ib){
            return v[3*ia]*w[3*ib]+v[3*ia+1]*w[3*ib+1]+v[3*ia+2]*w[3*ib+2];
        }
        int []GetBestCorr(double []dest){
            int[] res=new int[6];
            double[] dat=GetMatrix();
            double dbest=0,dbest1=0;
            int D=Cube.D;
            for(int k=0;k<6;k++) {
                int a0=k/2,a1=(a0+k%2+1)%3,a2=3-a0-a1;
                double s0=VPr(dest,0,dat,a0);
                double s1=VPr(dest,1,dat,a1);
                double s2=VPr(dest,2,dat,a2);
                double s=s0*s0+s1*s1+s2*s2;
                if(s>dbest) {
                    dbest=s;
                    res[0]=(a0+1)*(s0<0 ? -1 : 1);
                    res[1]=(a1+1)*(s1<0 ? -1 : 1);
                    res[2]=(a2+1)*(s2<0 ? -1 : 1);
                }
                if(D>4) {
                    double ss=s0*s0;
                    if(D>5) ss+=s1*s1;
                    if(D>6) ss+=s2*s2;
                    if(D==7 || (a2==2 && (D==6 || a1==1))) {
                        if(ss>dbest1) {
                            dbest1=ss;
                            res[3]=(a0+4)*(s0<0 ? -1 : 1);
                            res[4]=(a1+4)*(s1<0 ? -1 : 1);
                            res[5]=(a2+4)*(s2<0 ? -1 : 1);
                        }
                    }
                }
            }
            return res;
        }
        int[] GetFastMacroRef(double[] mvec,int []morient) {
            int []C=GetBestCorr(mvec);
            int []res=new int[8];
            res[1]=Cube.Orient[0];
            for(int i=0;i<7;i++) {
                int p=i==0 ? Cube.Orient[0] : Cube.Orient[Math.Abs(C[i-1])]*(C[i-1]<0 ? -1 : 1);
                int q=morient[i];
                if(q<0) { q=-q; p=-p; }
                res[q]=p;
            }
            return res;
        }


        private void aboutToolStripMenuItem_Click(object sender,EventArgs e) {
            MessageBox.Show($"Original:\r\nMC7D v1.31\r\n(c)2010, Andrey Astrelin\r\n\r\nMC7D-KB {VERSION}\r\n(c)2025, Jessica Chen");
        }

        private void btnTogglePanel_Click(object sender, EventArgs e) {
            SetPanelCollapsed(!m_panelCollapsed);
        }
        private void mi_ShowSidePanel_Click(object sender, EventArgs e) {
            SetPanelCollapsed(!m_panelCollapsed);
        }
        private void SetPanelCollapsed(bool collapsed) {
            m_panelCollapsed = collapsed;
            panel1.Visible = !collapsed;
            btnTogglePanel.Text = collapsed ? "‹" : "›";
            mi_ShowSidePanel.Checked = !collapsed;

            // Update button position
            UpdateToggleButtonPosition();

            // Force layout refresh to ensure 3D area resizes correctly
            panel2.PerformLayout();
            this.PerformLayout();
        }

        private void Form1_Load(object sender, EventArgs e) {
            // Initialize button position on form load
            UpdateToggleButtonPosition();
        }

        private void Form1_Resize(object sender, EventArgs e) {
            // Update button position when window resizes
            UpdateToggleButtonPosition();
        }

        private void UpdateToggleButtonPosition() {
            int buttonY = (this.ClientSize.Height - btnTogglePanel.Height) / 2;
            if (m_panelCollapsed) {
                // Place at the very right edge of client area
                btnTogglePanel.Location = new System.Drawing.Point(this.ClientSize.Width - btnTogglePanel.Width, buttonY);
            } else {
                // Place on top of panel1's left edge, shifted left by 1px for better alignment
                btnTogglePanel.Location = new System.Drawing.Point(panel1.Location.X - 1, buttonY);
            }
            btnTogglePanel.BringToFront();
        }

        private void startExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>=RevStack.GetLength(0)) {
                int[,] stk=new int[2*LRevStack,2];
                Buffer.BlockCopy(RevStack,0,stk,0,8*LRevStack);
                RevStack=stk;
            }
            RevStack[LRevStack,0]=Cube.LPtr;
            RevStack[LRevStack,1]=-1;
            LRevStack++;
            ShowRevStack();
        }

        private void stopExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>0) {
                if(RevStack[LRevStack-1,1]>=0 || Cube.LPtr<=RevStack[LRevStack-1,0]) LRevStack--;
                else RevStack[LRevStack-1,1]=Cube.LPtr;
            }
            ShowRevStack();
        }

        private void undoExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>0) {
                int r=RevStack[LRevStack-1,1];
                if(r>=0) {
                    int p=Cube.LPtr;
                    if(p>=r) {
                        Cube.ApplySeqReverse(RevStack[LRevStack-1,0],r);
                        if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                            Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                    }
                }
                LRevStack--;
            }
            Redraw();
        }

        private void commutatorToolStripMenuItem_Click(object sender,EventArgs e) {
            if(LRevStack>0) {
                int r=RevStack[LRevStack-1,1];
                if(r>=0) {
                    int p=Cube.LPtr;
                    if(p>=r) {
                        Cube.ApplySeqReverse(RevStack[LRevStack-1,0],r);
                        Cube.ApplySeqReverse(r,p);
                        if(cb_HighlightByColors.CheckState!=CheckState.Unchecked)
                            Cube.FindStickersByMask(FaceMask,cb_HighlightByColors.CheckState==CheckState.Checked);
                    }
                }
                LRevStack--;
            }
            Redraw();
        }

        private void ShowRevStack() {
            int p=Cube.LShuffle;
            int q=Cube.LPtr;

            string l="";
            for(int i=0;i<LRevStack;i++) {
                int r=RevStack[i,0];
                if(r!=p) l+=Cube.GetNTwists(p,r).ToString()+"[";
                else l+="[";
                p=r;
                r=RevStack[i,1];
                if(r>=0) {
                    l+=Cube.GetNTwists(p,r).ToString()+"]";
                    p=r;
                }
            }
            if(p<q) l+=Cube.GetNTwists(p,q).ToString();
            
            m_tbRevStack.Text=l;
            ms_Twists.Text="  Twists: "+Cube.NTwists;
        }

        private void recalculateToolStripMenuItem_Click(object sender,EventArgs e) {
            Cube.Recalculate();
            Redraw();
        }

        private void m_trkUndoSpeed_ValueChanged(object sender,EventArgs e) {
            double p=(double)(m_trkFullUndoSpeed.Value)/m_trkFullUndoSpeed.Maximum;
            p=Math.Pow(10,3-2*p);
            TRate=(int)p;
        }

        private void m_trkTransparency_ValueChanged(object sender,EventArgs e) {
            if(!m_setgeom) {
                CubeObj.Transparency=255-m_trkTransparency.Value;
                Redraw();
            }

        }

        private void editKeybinds_Click(object sender, EventArgs e)
        {
            if (KeybindsSetup == null || KeybindsSetup.IsDisposed)
            {
                KeybindsSetup = new KeybindSetup(this.Keybinds);
            }
            KeybindsSetup.Show();
            KeybindsSetup.Focus();
            KeybindsSetup.WindowState = FormWindowState.Normal;
        }

    }
}
