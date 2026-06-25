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
    public partial class Form1:System.Windows.Forms.Form {
		private System.Windows.Forms.Button btnTogglePanel;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mi_ShowSidePanel;
		private bool m_panelCollapsed = false;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel m_pnlOrbitFilters;
		private System.Windows.Forms.Label m_lblOrbitFilters;
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
        private CheckBox cb_MaskStickers;
        private Label label_GripAxes;
        private CheckBox cb_GripAxis1;
        private CheckBox cb_GripAxis2;
        private CheckBox cb_GripAxis3;
        private CheckBox cb_GripAxis4;
        private CheckBox cb_GripAxis5;
        private CheckBox cb_GripAxis6;
        private CheckBox cb_GripAxis7;
        private NumericUpDown nud_GripLayer1;
        private NumericUpDown nud_GripLayer2;
        private NumericUpDown nud_GripLayer3;
        private NumericUpDown nud_GripLayer4;
        private NumericUpDown nud_GripLayer5;
        private NumericUpDown nud_GripLayer6;
        private NumericUpDown nud_GripLayer7;
        private Button btn_ResetHighlightSelection;
        private Label label9;
        private Label label10;
        private ListBox m_lbMacros;
        private ToolStripMenuItem macroToolStripMenuItem;
        private ToolStripMenuItem mi_StartRecordig;
        private ToolStripMenuItem loadMacroFileToolStripMenuItem;
        private ToolStripMenuItem saveMacroFileToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem usageGuideToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem saveMacroFileAsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem startExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem stopExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem undoExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem commutatorToolStripMenuItem;
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
        private ToolStripStatusLabel ms_RevStack;
        private CheckBox m_RunByClick;
        private ToolStripDropDownButton activeKeybind;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem editKeybinds;
        private ToolStripMenuItem mi_PuzzleSize2;
        private System.ComponentModel.IContainer components;
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
            this.cb_MaskStickers = new System.Windows.Forms.CheckBox();
            this.label_GripAxes = new System.Windows.Forms.Label();
            this.cb_GripAxis1 = new System.Windows.Forms.CheckBox();
            this.cb_GripAxis2 = new System.Windows.Forms.CheckBox();
            this.cb_GripAxis3 = new System.Windows.Forms.CheckBox();
            this.cb_GripAxis4 = new System.Windows.Forms.CheckBox();
            this.cb_GripAxis5 = new System.Windows.Forms.CheckBox();
            this.cb_GripAxis6 = new System.Windows.Forms.CheckBox();
            this.cb_GripAxis7 = new System.Windows.Forms.CheckBox();
            this.nud_GripLayer1 = new System.Windows.Forms.NumericUpDown();
            this.nud_GripLayer2 = new System.Windows.Forms.NumericUpDown();
            this.nud_GripLayer3 = new System.Windows.Forms.NumericUpDown();
            this.nud_GripLayer4 = new System.Windows.Forms.NumericUpDown();
            this.nud_GripLayer5 = new System.Windows.Forms.NumericUpDown();
            this.nud_GripLayer6 = new System.Windows.Forms.NumericUpDown();
            this.nud_GripLayer7 = new System.Windows.Forms.NumericUpDown();
            this.btn_ResetHighlightSelection = new System.Windows.Forms.Button();
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
            this.m_lblOrbitFilters = new System.Windows.Forms.Label();
            this.m_pnlOrbitFilters = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pb_ClickStatus = new System.Windows.Forms.ToolStripProgressBar();
            this.ms_Twists = new System.Windows.Forms.ToolStripStatusLabel();
            this.ms_MacroStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.ms_RevStack = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.usageGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.panel1.Controls.Add(this.cb_MaskStickers);
            this.panel1.Controls.Add(this.label_GripAxes);
            this.panel1.Controls.Add(this.cb_GripAxis1);
            this.panel1.Controls.Add(this.cb_GripAxis2);
            this.panel1.Controls.Add(this.cb_GripAxis3);
            this.panel1.Controls.Add(this.cb_GripAxis4);
            this.panel1.Controls.Add(this.cb_GripAxis5);
            this.panel1.Controls.Add(this.cb_GripAxis6);
            this.panel1.Controls.Add(this.cb_GripAxis7);
            this.panel1.Controls.Add(this.nud_GripLayer1);
            this.panel1.Controls.Add(this.nud_GripLayer2);
            this.panel1.Controls.Add(this.nud_GripLayer3);
            this.panel1.Controls.Add(this.nud_GripLayer4);
            this.panel1.Controls.Add(this.nud_GripLayer5);
            this.panel1.Controls.Add(this.nud_GripLayer6);
            this.panel1.Controls.Add(this.nud_GripLayer7);
            this.panel1.Controls.Add(this.btn_ResetHighlightSelection);
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
            this.panel1.Controls.Add(this.m_lblOrbitFilters);
            this.panel1.Controls.Add(this.m_pnlOrbitFilters);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.AutoScroll = true;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel1.Location = new System.Drawing.Point(777, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 684);
            this.panel1.TabIndex = 19;
            //
            // m_lblOrbitFilters
            //
            this.m_lblOrbitFilters.AutoSize = true;
            this.m_lblOrbitFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.m_lblOrbitFilters.Location = new System.Drawing.Point(6, 566);
            this.m_lblOrbitFilters.Name = "m_lblOrbitFilters";
            this.m_lblOrbitFilters.Size = new System.Drawing.Size(66, 13);
            this.m_lblOrbitFilters.TabIndex = 21;
            this.m_lblOrbitFilters.Text = "Orbit Filters";
            //
            // m_pnlOrbitFilters
            //
            this.m_pnlOrbitFilters.AutoScroll = true;
            this.m_pnlOrbitFilters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_pnlOrbitFilters.Location = new System.Drawing.Point(6, 584);
            this.m_pnlOrbitFilters.Name = "m_pnlOrbitFilters";
            this.m_pnlOrbitFilters.Size = new System.Drawing.Size(200, 130);
            this.m_pnlOrbitFilters.TabIndex = 22;
            //
            // m_RunByClick
            // 
            this.m_RunByClick.AutoSize = true;
            this.m_RunByClick.Checked = true;
            this.m_RunByClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_RunByClick.Location = new System.Drawing.Point(120, 842);
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
            this.m_cbQuickMacro.Location = new System.Drawing.Point(120, 820);
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
            // label10
            // 
            this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 731);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Macros:";
            // 
            // m_lbMacros
            // 
            this.m_lbMacros.FormattingEnabled = true;
			this.m_lbMacros.Location = new System.Drawing.Point(6, 748);
            this.m_lbMacros.Name = "m_lbMacros";
            this.m_lbMacros.Size = new System.Drawing.Size(100, 150);
            this.m_lbMacros.Sorted = true;
            this.m_lbMacros.TabIndex = 12;
            this.m_lbMacros.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_lbMacros_MouseDown);
            this.m_lbMacros.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_lbMacros_MouseDoubleClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(12, 497);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Show cubies:";
            // 
            // cb_Show3C
            //
            this.cb_Show3C.AutoSize = false;
            this.cb_Show3C.Checked = true;
            this.cb_Show3C.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Show3C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cb_Show3C.Location = new System.Drawing.Point(101, 510);
            this.cb_Show3C.Name = "cb_Show3C";
            this.cb_Show3C.Size = new System.Drawing.Size(32, 22);
            this.cb_Show3C.TabIndex = 10;
            this.cb_Show3C.Text = "3C";
            this.cb_Show3C.ThreeState = true;
            this.cb_Show3C.UseVisualStyleBackColor = true;
            this.cb_Show3C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show5C
            //
            this.cb_Show5C.AutoSize = false;
            this.cb_Show5C.Checked = true;
            this.cb_Show5C.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Show5C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cb_Show5C.Location = new System.Drawing.Point(65, 536);
            this.cb_Show5C.Name = "cb_Show5C";
            this.cb_Show5C.Size = new System.Drawing.Size(32, 22);
            this.cb_Show5C.TabIndex = 10;
            this.cb_Show5C.Text = "5C";
            this.cb_Show5C.ThreeState = true;
            this.cb_Show5C.UseVisualStyleBackColor = true;
            this.cb_Show5C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show1C
            //
            this.cb_Show1C.AutoSize = false;
            this.cb_Show1C.Checked = true;
            this.cb_Show1C.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Show1C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cb_Show1C.Location = new System.Drawing.Point(29, 510);
            this.cb_Show1C.Name = "cb_Show1C";
            this.cb_Show1C.Size = new System.Drawing.Size(32, 22);
            this.cb_Show1C.TabIndex = 10;
            this.cb_Show1C.Text = "1C";
            this.cb_Show1C.ThreeState = true;
            this.cb_Show1C.UseVisualStyleBackColor = true;
            this.cb_Show1C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show6C
            //
            this.cb_Show6C.AutoSize = false;
            this.cb_Show6C.Checked = true;
            this.cb_Show6C.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Show6C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cb_Show6C.Location = new System.Drawing.Point(101, 536);
            this.cb_Show6C.Name = "cb_Show6C";
            this.cb_Show6C.Size = new System.Drawing.Size(32, 22);
            this.cb_Show6C.TabIndex = 10;
            this.cb_Show6C.Text = "6C";
            this.cb_Show6C.ThreeState = true;
            this.cb_Show6C.UseVisualStyleBackColor = true;
            this.cb_Show6C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show2C
            //
            this.cb_Show2C.AutoSize = false;
            this.cb_Show2C.Checked = true;
            this.cb_Show2C.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Show2C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cb_Show2C.Location = new System.Drawing.Point(65, 510);
            this.cb_Show2C.Name = "cb_Show2C";
            this.cb_Show2C.Size = new System.Drawing.Size(32, 22);
            this.cb_Show2C.TabIndex = 10;
            this.cb_Show2C.Text = "2C";
            this.cb_Show2C.ThreeState = true;
            this.cb_Show2C.UseVisualStyleBackColor = true;
            this.cb_Show2C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            // 
            // cb_Show7C
            //
            this.cb_Show7C.AutoSize = false;
            this.cb_Show7C.Checked = true;
            this.cb_Show7C.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Show7C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cb_Show7C.Location = new System.Drawing.Point(137, 536);
            this.cb_Show7C.Name = "cb_Show7C";
            this.cb_Show7C.Size = new System.Drawing.Size(32, 22);
            this.cb_Show7C.TabIndex = 10;
            this.cb_Show7C.Text = "7C";
            this.cb_Show7C.ThreeState = true;
            this.cb_Show7C.UseVisualStyleBackColor = true;
            this.cb_Show7C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            //
            // cb_Show4C
            //
            this.cb_Show4C.AutoSize = false;
            this.cb_Show4C.Checked = true;
            this.cb_Show4C.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_Show4C.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cb_Show4C.Location = new System.Drawing.Point(137, 510);
            this.cb_Show4C.Name = "cb_Show4C";
            this.cb_Show4C.Size = new System.Drawing.Size(32, 22);
            this.cb_Show4C.TabIndex = 10;
            this.cb_Show4C.Text = "4C";
            this.cb_Show4C.ThreeState = true;
            this.cb_Show4C.UseVisualStyleBackColor = true;
            this.cb_Show4C.CheckStateChanged += new System.EventHandler(this.cb_Show1C_CheckedChanged);
            //
            // label_GripAxes
            //
            this.label_GripAxes.AutoSize = true;
            this.label_GripAxes.Location = new System.Drawing.Point(12, 392);
            this.label_GripAxes.Name = "label_GripAxes";
            this.label_GripAxes.Size = new System.Drawing.Size(62, 13);
            this.label_GripAxes.TabIndex = 20;
            this.label_GripAxes.Text = "Axes:";
            //
            // cb_GripAxis1
            //
            this.cb_GripAxis1.AutoSize = false;
            this.cb_GripAxis1.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_GripAxis1.Location = new System.Drawing.Point(29, 405);
            this.cb_GripAxis1.Name = "cb_GripAxis1";
            this.cb_GripAxis1.Size = new System.Drawing.Size(24, 22);
            this.cb_GripAxis1.TabIndex = 21;
            this.cb_GripAxis1.Text = "W";
            this.cb_GripAxis1.ThreeState = true;
            this.cb_GripAxis1.UseVisualStyleBackColor = true;
            this.cb_GripAxis1.CheckStateChanged += new System.EventHandler(this.cb_GripAxis_CheckStateChanged);
            //
            // cb_GripAxis2
            //
            this.cb_GripAxis2.AutoSize = false;
            this.cb_GripAxis2.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_GripAxis2.Location = new System.Drawing.Point(67, 405);
            this.cb_GripAxis2.Name = "cb_GripAxis2";
            this.cb_GripAxis2.Size = new System.Drawing.Size(24, 22);
            this.cb_GripAxis2.TabIndex = 22;
            this.cb_GripAxis2.Text = "X";
            this.cb_GripAxis2.ThreeState = true;
            this.cb_GripAxis2.UseVisualStyleBackColor = true;
            this.cb_GripAxis2.CheckStateChanged += new System.EventHandler(this.cb_GripAxis_CheckStateChanged);
            //
            // cb_GripAxis3
            //
            this.cb_GripAxis3.AutoSize = false;
            this.cb_GripAxis3.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_GripAxis3.Location = new System.Drawing.Point(105, 405);
            this.cb_GripAxis3.Name = "cb_GripAxis3";
            this.cb_GripAxis3.Size = new System.Drawing.Size(24, 22);
            this.cb_GripAxis3.TabIndex = 23;
            this.cb_GripAxis3.Text = "Z";
            this.cb_GripAxis3.ThreeState = true;
            this.cb_GripAxis3.UseVisualStyleBackColor = true;
            this.cb_GripAxis3.CheckStateChanged += new System.EventHandler(this.cb_GripAxis_CheckStateChanged);
            //
            // cb_GripAxis4
            //
            this.cb_GripAxis4.AutoSize = false;
            this.cb_GripAxis4.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_GripAxis4.Location = new System.Drawing.Point(143, 405);
            this.cb_GripAxis4.Name = "cb_GripAxis4";
            this.cb_GripAxis4.Size = new System.Drawing.Size(24, 22);
            this.cb_GripAxis4.TabIndex = 24;
            this.cb_GripAxis4.Text = "Y";
            this.cb_GripAxis4.ThreeState = true;
            this.cb_GripAxis4.UseVisualStyleBackColor = true;
            this.cb_GripAxis4.CheckStateChanged += new System.EventHandler(this.cb_GripAxis_CheckStateChanged);
            //
            // cb_GripAxis5
            //
            this.cb_GripAxis5.AutoSize = false;
            this.cb_GripAxis5.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_GripAxis5.Location = new System.Drawing.Point(67, 449);
            this.cb_GripAxis5.Name = "cb_GripAxis5";
            this.cb_GripAxis5.Size = new System.Drawing.Size(24, 22);
            this.cb_GripAxis5.TabIndex = 25;
            this.cb_GripAxis5.Text = "V";
            this.cb_GripAxis5.ThreeState = true;
            this.cb_GripAxis5.UseVisualStyleBackColor = true;
            this.cb_GripAxis5.CheckStateChanged += new System.EventHandler(this.cb_GripAxis_CheckStateChanged);
            //
            // cb_GripAxis6
            //
            this.cb_GripAxis6.AutoSize = false;
            this.cb_GripAxis6.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_GripAxis6.Location = new System.Drawing.Point(105, 449);
            this.cb_GripAxis6.Name = "cb_GripAxis6";
            this.cb_GripAxis6.Size = new System.Drawing.Size(24, 22);
            this.cb_GripAxis6.TabIndex = 26;
            this.cb_GripAxis6.Text = "U";
            this.cb_GripAxis6.ThreeState = true;
            this.cb_GripAxis6.UseVisualStyleBackColor = true;
            this.cb_GripAxis6.CheckStateChanged += new System.EventHandler(this.cb_GripAxis_CheckStateChanged);
            //
            // cb_GripAxis7
            //
            this.cb_GripAxis7.AutoSize = false;
            this.cb_GripAxis7.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cb_GripAxis7.Location = new System.Drawing.Point(143, 449);
            this.cb_GripAxis7.Name = "cb_GripAxis7";
            this.cb_GripAxis7.Size = new System.Drawing.Size(24, 22);
            this.cb_GripAxis7.TabIndex = 27;
            this.cb_GripAxis7.Text = "T";
            this.cb_GripAxis7.ThreeState = true;
            this.cb_GripAxis7.UseVisualStyleBackColor = true;
            this.cb_GripAxis7.CheckStateChanged += new System.EventHandler(this.cb_GripAxis_CheckStateChanged);
            //
            // nud_GripLayer1
            //
            this.nud_GripLayer1.Location = new System.Drawing.Point(29, 427);
            this.nud_GripLayer1.Name = "nud_GripLayer1";
            this.nud_GripLayer1.Size = new System.Drawing.Size(36, 20);
            this.nud_GripLayer1.Minimum = -127;
            this.nud_GripLayer1.Maximum = 127;
            this.nud_GripLayer1.Value = 1;
            this.nud_GripLayer1.TabIndex = 28;
            this.nud_GripLayer1.ValueChanged += new System.EventHandler(this.nud_GripLayer_ValueChanged);
            //
            // nud_GripLayer2
            //
            this.nud_GripLayer2.Location = new System.Drawing.Point(67, 427);
            this.nud_GripLayer2.Name = "nud_GripLayer2";
            this.nud_GripLayer2.Size = new System.Drawing.Size(36, 20);
            this.nud_GripLayer2.Minimum = -127;
            this.nud_GripLayer2.Maximum = 127;
            this.nud_GripLayer2.Value = 1;
            this.nud_GripLayer2.TabIndex = 29;
            this.nud_GripLayer2.ValueChanged += new System.EventHandler(this.nud_GripLayer_ValueChanged);
            //
            // nud_GripLayer3
            //
            this.nud_GripLayer3.Location = new System.Drawing.Point(105, 427);
            this.nud_GripLayer3.Name = "nud_GripLayer3";
            this.nud_GripLayer3.Size = new System.Drawing.Size(36, 20);
            this.nud_GripLayer3.Minimum = -127;
            this.nud_GripLayer3.Maximum = 127;
            this.nud_GripLayer3.Value = 1;
            this.nud_GripLayer3.TabIndex = 30;
            this.nud_GripLayer3.ValueChanged += new System.EventHandler(this.nud_GripLayer_ValueChanged);
            //
            // nud_GripLayer4
            //
            this.nud_GripLayer4.Location = new System.Drawing.Point(143, 427);
            this.nud_GripLayer4.Name = "nud_GripLayer4";
            this.nud_GripLayer4.Size = new System.Drawing.Size(36, 20);
            this.nud_GripLayer4.Minimum = -127;
            this.nud_GripLayer4.Maximum = 127;
            this.nud_GripLayer4.Value = 1;
            this.nud_GripLayer4.TabIndex = 31;
            this.nud_GripLayer4.ValueChanged += new System.EventHandler(this.nud_GripLayer_ValueChanged);
            //
            // nud_GripLayer5
            //
            this.nud_GripLayer5.Location = new System.Drawing.Point(67, 471);
            this.nud_GripLayer5.Name = "nud_GripLayer5";
            this.nud_GripLayer5.Size = new System.Drawing.Size(36, 20);
            this.nud_GripLayer5.Minimum = -127;
            this.nud_GripLayer5.Maximum = 127;
            this.nud_GripLayer5.Value = 1;
            this.nud_GripLayer5.TabIndex = 32;
            this.nud_GripLayer5.ValueChanged += new System.EventHandler(this.nud_GripLayer_ValueChanged);
            //
            // nud_GripLayer6
            //
            this.nud_GripLayer6.Location = new System.Drawing.Point(105, 471);
            this.nud_GripLayer6.Name = "nud_GripLayer6";
            this.nud_GripLayer6.Size = new System.Drawing.Size(36, 20);
            this.nud_GripLayer6.Minimum = -127;
            this.nud_GripLayer6.Maximum = 127;
            this.nud_GripLayer6.Value = 1;
            this.nud_GripLayer6.TabIndex = 33;
            this.nud_GripLayer6.ValueChanged += new System.EventHandler(this.nud_GripLayer_ValueChanged);
            //
            // nud_GripLayer7
            //
            this.nud_GripLayer7.Location = new System.Drawing.Point(143, 471);
            this.nud_GripLayer7.Name = "nud_GripLayer7";
            this.nud_GripLayer7.Size = new System.Drawing.Size(36, 20);
            this.nud_GripLayer7.Minimum = -127;
            this.nud_GripLayer7.Maximum = 127;
            this.nud_GripLayer7.Value = 1;
            this.nud_GripLayer7.TabIndex = 34;
            this.nud_GripLayer7.ValueChanged += new System.EventHandler(this.nud_GripLayer_ValueChanged);
            //
            // cb_HighlightByColors
            //
            this.cb_HighlightByColors.AutoSize = true;
			this.cb_HighlightByColors.Location = new System.Drawing.Point(120, 776);
            this.cb_HighlightByColors.Name = "cb_HighlightByColors";
            this.cb_HighlightByColors.Size = new System.Drawing.Size(112, 17);
            this.cb_HighlightByColors.TabIndex = 9;
            this.cb_HighlightByColors.Text = "Enable highlighting";
            this.cb_HighlightByColors.ThreeState = true;
            this.cb_HighlightByColors.UseVisualStyleBackColor = true;
            this.cb_HighlightByColors.CheckedChanged += new System.EventHandler(this.cb_HighlightByColors_CheckedChanged);
            this.cb_HighlightByColors.CheckStateChanged += new System.EventHandler(this.cb_HighlightByColors_CheckedChanged);
            //
            // cb_MaskStickers
            //
            this.cb_MaskStickers.AutoSize = true;
			this.cb_MaskStickers.Location = new System.Drawing.Point(120, 798);
            this.cb_MaskStickers.Name = "cb_MaskStickers";
            this.cb_MaskStickers.Size = new System.Drawing.Size(95, 17);
            this.cb_MaskStickers.TabIndex = 11;
            this.cb_MaskStickers.Text = "Mask stickers";
            this.cb_MaskStickers.UseVisualStyleBackColor = true;
            this.cb_MaskStickers.CheckedChanged += new System.EventHandler(this.cb_MaskStickers_CheckedChanged);
            //
            // btn_ResetHighlightSelection
            //
			this.btn_ResetHighlightSelection.Location = new System.Drawing.Point(120, 741);
            this.btn_ResetHighlightSelection.Name = "btn_ResetHighlightSelection";
            this.btn_ResetHighlightSelection.Size = new System.Drawing.Size(75, 23);
            this.btn_ResetHighlightSelection.TabIndex = 10;
            this.btn_ResetHighlightSelection.Text = "Reset Filters";
            this.btn_ResetHighlightSelection.UseVisualStyleBackColor = true;
            this.btn_ResetHighlightSelection.Click += new System.EventHandler(this.btn_ResetHighlightSelection_Click);
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
            this.activeKeybind,
            this.ms_RevStack});
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
            this.activeKeybind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // ms_RevStack
            //
            this.ms_RevStack.Name = "ms_RevStack";
            this.ms_RevStack.Size = new System.Drawing.Size(60, 18);
            this.ms_RevStack.Text = "RevStack: ";
            this.ms_RevStack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.usageGuideToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            //
            // usageGuideToolStripMenuItem
            //
            this.usageGuideToolStripMenuItem.Name = "usageGuideToolStripMenuItem";
            this.usageGuideToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.usageGuideToolStripMenuItem.Text = "Usage Guide";
            this.usageGuideToolStripMenuItem.Click += new System.EventHandler(this.usageGuideToolStripMenuItem_Click);
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
            this.ClientSize = new System.Drawing.Size(872, 654);
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
    }
}
