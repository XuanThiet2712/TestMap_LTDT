namespace TestMap.Forms
{
    partial class SimForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.playTimer = new System.Windows.Forms.Timer(this.components);
            this.tabSim = new System.Windows.Forms.TabControl();
            this.tabAlgo = new System.Windows.Forms.TabPage();
            this.lblAlgoHelp = new System.Windows.Forms.Label();
            this.btnAlgoPrepare = new System.Windows.Forms.Button();
            this.lblAlgoStep = new System.Windows.Forms.Label();
            this.trkAlgo = new System.Windows.Forms.TrackBar();
            this.btnAlgoRestart = new System.Windows.Forms.Button();
            this.btnAlgoBack = new System.Windows.Forms.Button();
            this.btnAlgoPlay = new System.Windows.Forms.Button();
            this.btnAlgoNext = new System.Windows.Forms.Button();
            this.btnAlgoEnd = new System.Windows.Forms.Button();
            this.lblAlgoSpeed = new System.Windows.Forms.Label();
            this.trkAlgoSpeed = new System.Windows.Forms.TrackBar();
            this.lblAlgoNow = new System.Windows.Forms.Label();
            this.lblAlgoLegend = new System.Windows.Forms.Label();
            this.tabBus = new System.Windows.Forms.TabPage();
            this.lblBusHelp = new System.Windows.Forms.Label();
            this.btnBusPrepare = new System.Windows.Forms.Button();
            this.lstRounds = new System.Windows.Forms.ListBox();
            this.lblBusStep = new System.Windows.Forms.Label();
            this.trkBus = new System.Windows.Forms.TrackBar();
            this.btnBusRestart = new System.Windows.Forms.Button();
            this.btnBusBack = new System.Windows.Forms.Button();
            this.btnBusPlay = new System.Windows.Forms.Button();
            this.btnBusNext = new System.Windows.Forms.Button();
            this.btnBusRoundEnd = new System.Windows.Forms.Button();
            this.lblBusSpeed = new System.Windows.Forms.Label();
            this.trkBusSpeed = new System.Windows.Forms.TrackBar();
            this.lblBusNow = new System.Windows.Forms.Label();
            this.lblBusLegend = new System.Windows.Forms.Label();
            this.btnOpenModel = new System.Windows.Forms.Button();
            this.tabGraph = new System.Windows.Forms.TabPage();
            this.lblGraphHelp = new System.Windows.Forms.Label();
            this.chkShowNodes = new System.Windows.Forms.CheckBox();
            this.chkShowWeights = new System.Windows.Forms.CheckBox();
            this.chkShowNames = new System.Windows.Forms.CheckBox();
            this.lblNode1 = new System.Windows.Forms.Label();
            this.txtNode1 = new System.Windows.Forms.TextBox();
            this.lblNode2 = new System.Windows.Forms.Label();
            this.txtNode2 = new System.Windows.Forms.TextBox();
            this.btnLookup = new System.Windows.Forms.Button();
            this.lblLookup = new System.Windows.Forms.Label();
            this.tabTools = new System.Windows.Forms.TabPage();
            this.lblBlockHelp = new System.Windows.Forms.Label();
            this.chkBlockMode = new System.Windows.Forms.CheckBox();
            this.lblBlocked = new System.Windows.Forms.Label();
            this.btnBlockOnRoute = new System.Windows.Forms.Button();
            this.btnBlockRandom = new System.Windows.Forms.Button();
            this.btnBlockClear = new System.Windows.Forms.Button();
            this.btnSnapshot = new System.Windows.Forms.Button();
            this.lblToolNote = new System.Windows.Forms.Label();
            this.tabSim.SuspendLayout();
            this.tabAlgo.SuspendLayout();
            this.tabBus.SuspendLayout();
            this.tabGraph.SuspendLayout();
            this.tabTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // playTimer
            // 
            this.playTimer.Interval = 50;
            this.playTimer.Tick += new System.EventHandler(this.playTimer_Tick);
            // 
            // tabSim
            // 
            this.tabSim.Controls.Add(this.tabAlgo);
            this.tabSim.Controls.Add(this.tabBus);
            this.tabSim.Controls.Add(this.tabGraph);
            this.tabSim.Controls.Add(this.tabTools);
            this.tabSim.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSim.Name = "tabSim";
            this.tabSim.TabIndex = 0;
            this.tabSim.SelectedIndex = 0;
            // 
            // tabAlgo
            // 
            this.tabAlgo.Controls.Add(this.lblAlgoHelp);
            this.tabAlgo.Controls.Add(this.btnAlgoPrepare);
            this.tabAlgo.Controls.Add(this.lblAlgoStep);
            this.tabAlgo.Controls.Add(this.trkAlgo);
            this.tabAlgo.Controls.Add(this.btnAlgoRestart);
            this.tabAlgo.Controls.Add(this.btnAlgoBack);
            this.tabAlgo.Controls.Add(this.btnAlgoPlay);
            this.tabAlgo.Controls.Add(this.btnAlgoNext);
            this.tabAlgo.Controls.Add(this.btnAlgoEnd);
            this.tabAlgo.Controls.Add(this.lblAlgoSpeed);
            this.tabAlgo.Controls.Add(this.trkAlgoSpeed);
            this.tabAlgo.Controls.Add(this.lblAlgoNow);
            this.tabAlgo.Controls.Add(this.lblAlgoLegend);
            this.tabAlgo.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.tabAlgo.Location = new System.Drawing.Point(4, 26);
            this.tabAlgo.Name = "tabAlgo";
            this.tabAlgo.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabAlgo.Size = new System.Drawing.Size(392, 610);
            this.tabAlgo.TabIndex = 0;
            this.tabAlgo.Text = "Tìm đường";
            // 
            // lblAlgoHelp
            // 
            this.lblAlgoHelp.AutoSize = false;
            this.lblAlgoHelp.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblAlgoHelp.Location = new System.Drawing.Point(12, 10);
            this.lblAlgoHelp.Name = "lblAlgoHelp";
            this.lblAlgoHelp.Size = new System.Drawing.Size(368, 62);
            this.lblAlgoHelp.TabIndex = 0;
            this.lblAlgoHelp.Text = "Xem Dijkstra / A* chọn từng giao lộ một. Chọn điểm đi A và điểm đến B trên bản đồ (thuật toán lấy ở ô trên cùng), rồi bấm Chuẩn bị.";
            // 
            // btnAlgoPrepare
            // 
            this.btnAlgoPrepare.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnAlgoPrepare.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlgoPrepare.FlatAppearance.BorderSize = 0;
            this.btnAlgoPrepare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlgoPrepare.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnAlgoPrepare.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnAlgoPrepare.Location = new System.Drawing.Point(12, 76);
            this.btnAlgoPrepare.Name = "btnAlgoPrepare";
            this.btnAlgoPrepare.Size = new System.Drawing.Size(368, 34);
            this.btnAlgoPrepare.TabIndex = 1;
            this.btnAlgoPrepare.Text = "Chuẩn bị mô phỏng (A → B)";
            this.btnAlgoPrepare.UseVisualStyleBackColor = false;
            this.btnAlgoPrepare.Click += new System.EventHandler(this.btnAlgoPrepare_Click);
            // 
            // lblAlgoStep
            // 
            this.lblAlgoStep.AutoSize = false;
            this.lblAlgoStep.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblAlgoStep.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblAlgoStep.Location = new System.Drawing.Point(12, 122);
            this.lblAlgoStep.Name = "lblAlgoStep";
            this.lblAlgoStep.Size = new System.Drawing.Size(368, 20);
            this.lblAlgoStep.TabIndex = 2;
            this.lblAlgoStep.Text = "Bước 0 / 0";
            // 
            // trkAlgo
            // 
            this.trkAlgo.Location = new System.Drawing.Point(6, 144);
            this.trkAlgo.Name = "trkAlgo";
            this.trkAlgo.Size = new System.Drawing.Size(380, 45);
            this.trkAlgo.TabIndex = 3;
            this.trkAlgo.Maximum = 0;
            this.trkAlgo.Minimum = 0;
            this.trkAlgo.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkAlgo.Enabled = false;
            this.trkAlgo.Scroll += new System.EventHandler(this.trkAlgo_Scroll);
            // 
            // btnAlgoRestart
            // 
            this.btnAlgoRestart.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnAlgoRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlgoRestart.FlatAppearance.BorderSize = 0;
            this.btnAlgoRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlgoRestart.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnAlgoRestart.Location = new System.Drawing.Point(12, 194);
            this.btnAlgoRestart.Name = "btnAlgoRestart";
            this.btnAlgoRestart.Size = new System.Drawing.Size(40, 32);
            this.btnAlgoRestart.TabIndex = 4;
            this.btnAlgoRestart.Text = "⟲";
            this.btnAlgoRestart.UseVisualStyleBackColor = false;
            this.btnAlgoRestart.Click += new System.EventHandler(this.btnAlgoRestart_Click);
            // 
            // btnAlgoBack
            // 
            this.btnAlgoBack.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnAlgoBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlgoBack.FlatAppearance.BorderSize = 0;
            this.btnAlgoBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlgoBack.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnAlgoBack.Location = new System.Drawing.Point(56, 194);
            this.btnAlgoBack.Name = "btnAlgoBack";
            this.btnAlgoBack.Size = new System.Drawing.Size(64, 32);
            this.btnAlgoBack.TabIndex = 5;
            this.btnAlgoBack.Text = "◀ Lùi";
            this.btnAlgoBack.UseVisualStyleBackColor = false;
            this.btnAlgoBack.Click += new System.EventHandler(this.btnAlgoBack_Click);
            // 
            // btnAlgoPlay
            // 
            this.btnAlgoPlay.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnAlgoPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlgoPlay.FlatAppearance.BorderSize = 0;
            this.btnAlgoPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlgoPlay.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnAlgoPlay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAlgoPlay.Location = new System.Drawing.Point(124, 194);
            this.btnAlgoPlay.Name = "btnAlgoPlay";
            this.btnAlgoPlay.Size = new System.Drawing.Size(100, 32);
            this.btnAlgoPlay.TabIndex = 6;
            this.btnAlgoPlay.Text = "▶ Chạy";
            this.btnAlgoPlay.UseVisualStyleBackColor = false;
            this.btnAlgoPlay.Click += new System.EventHandler(this.btnAlgoPlay_Click);
            // 
            // btnAlgoNext
            // 
            this.btnAlgoNext.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnAlgoNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlgoNext.FlatAppearance.BorderSize = 0;
            this.btnAlgoNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlgoNext.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnAlgoNext.Location = new System.Drawing.Point(228, 194);
            this.btnAlgoNext.Name = "btnAlgoNext";
            this.btnAlgoNext.Size = new System.Drawing.Size(64, 32);
            this.btnAlgoNext.TabIndex = 7;
            this.btnAlgoNext.Text = "Tới ▶";
            this.btnAlgoNext.UseVisualStyleBackColor = false;
            this.btnAlgoNext.Click += new System.EventHandler(this.btnAlgoNext_Click);
            // 
            // btnAlgoEnd
            // 
            this.btnAlgoEnd.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnAlgoEnd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlgoEnd.FlatAppearance.BorderSize = 0;
            this.btnAlgoEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlgoEnd.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnAlgoEnd.Location = new System.Drawing.Point(296, 194);
            this.btnAlgoEnd.Name = "btnAlgoEnd";
            this.btnAlgoEnd.Size = new System.Drawing.Size(84, 32);
            this.btnAlgoEnd.TabIndex = 8;
            this.btnAlgoEnd.Text = "⏭ Kết quả";
            this.btnAlgoEnd.UseVisualStyleBackColor = false;
            this.btnAlgoEnd.Click += new System.EventHandler(this.btnAlgoEnd_Click);
            // 
            // lblAlgoSpeed
            // 
            this.lblAlgoSpeed.AutoSize = false;
            this.lblAlgoSpeed.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblAlgoSpeed.Location = new System.Drawing.Point(12, 240);
            this.lblAlgoSpeed.Name = "lblAlgoSpeed";
            this.lblAlgoSpeed.Size = new System.Drawing.Size(56, 20);
            this.lblAlgoSpeed.TabIndex = 9;
            this.lblAlgoSpeed.Text = "Tốc độ";
            // 
            // trkAlgoSpeed
            // 
            this.trkAlgoSpeed.Location = new System.Drawing.Point(66, 234);
            this.trkAlgoSpeed.Name = "trkAlgoSpeed";
            this.trkAlgoSpeed.Size = new System.Drawing.Size(314, 45);
            this.trkAlgoSpeed.TabIndex = 10;
            this.trkAlgoSpeed.Maximum = 8;
            this.trkAlgoSpeed.Minimum = 1;
            this.trkAlgoSpeed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkAlgoSpeed.Value = 4;
            // 
            // lblAlgoNow
            // 
            this.lblAlgoNow.AutoSize = false;
            this.lblAlgoNow.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lblAlgoNow.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblAlgoNow.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblAlgoNow.Location = new System.Drawing.Point(12, 282);
            this.lblAlgoNow.Name = "lblAlgoNow";
            this.lblAlgoNow.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.lblAlgoNow.Size = new System.Drawing.Size(368, 236);
            this.lblAlgoNow.TabIndex = 11;
            this.lblAlgoNow.Text = "Chưa có dữ liệu. Bấm \"Chuẩn bị mô phỏng\".";
            // 
            // lblAlgoLegend
            // 
            this.lblAlgoLegend.AutoSize = false;
            this.lblAlgoLegend.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblAlgoLegend.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblAlgoLegend.Location = new System.Drawing.Point(12, 526);
            this.lblAlgoLegend.Name = "lblAlgoLegend";
            this.lblAlgoLegend.Size = new System.Drawing.Size(368, 64);
            this.lblAlgoLegend.TabIndex = 12;
            this.lblAlgoLegend.Text = "Trên bản đồ:  ● xanh = giao lộ vừa chốt   ● cam = đang chờ trong hàng đợi   đường tím = đã xét";
            // 
            // tabBus
            // 
            this.tabBus.Controls.Add(this.lblBusHelp);
            this.tabBus.Controls.Add(this.btnBusPrepare);
            this.tabBus.Controls.Add(this.lstRounds);
            this.tabBus.Controls.Add(this.lblBusStep);
            this.tabBus.Controls.Add(this.trkBus);
            this.tabBus.Controls.Add(this.btnBusRestart);
            this.tabBus.Controls.Add(this.btnBusBack);
            this.tabBus.Controls.Add(this.btnBusPlay);
            this.tabBus.Controls.Add(this.btnBusNext);
            this.tabBus.Controls.Add(this.btnBusRoundEnd);
            this.tabBus.Controls.Add(this.lblBusSpeed);
            this.tabBus.Controls.Add(this.trkBusSpeed);
            this.tabBus.Controls.Add(this.lblBusNow);
            this.tabBus.Controls.Add(this.lblBusLegend);
            this.tabBus.Controls.Add(this.btnOpenModel);
            this.tabBus.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.tabBus.Location = new System.Drawing.Point(4, 26);
            this.tabBus.Name = "tabBus";
            this.tabBus.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabBus.Size = new System.Drawing.Size(392, 610);
            this.tabBus.TabIndex = 1;
            this.tabBus.Text = "Gợi ý xe buýt";
            // 
            // lblBusHelp
            // 
            this.lblBusHelp.AutoSize = false;
            this.lblBusHelp.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblBusHelp.Location = new System.Drawing.Point(12, 10);
            this.lblBusHelp.Name = "lblBusHelp";
            this.lblBusHelp.Size = new System.Drawing.Size(368, 62);
            this.lblBusHelp.TabIndex = 0;
            this.lblBusHelp.Text = "Xem bộ gợi ý tìm cách đi: lượt 1 ưu tiên nhanh nhất, rồi lượt ưu tiên ít đổi xe, rồi cấm lần lượt các tuyến vừa dùng để tìm cách khác.";
            // 
            // btnBusPrepare
            // 
            this.btnBusPrepare.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnBusPrepare.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBusPrepare.FlatAppearance.BorderSize = 0;
            this.btnBusPrepare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBusPrepare.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBusPrepare.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBusPrepare.Location = new System.Drawing.Point(12, 76);
            this.btnBusPrepare.Name = "btnBusPrepare";
            this.btnBusPrepare.Size = new System.Drawing.Size(368, 34);
            this.btnBusPrepare.TabIndex = 1;
            this.btnBusPrepare.Text = "Chuẩn bị mô phỏng (trạm A → trạm B)";
            this.btnBusPrepare.UseVisualStyleBackColor = false;
            this.btnBusPrepare.Click += new System.EventHandler(this.btnBusPrepare_Click);
            // 
            // lstRounds
            // 
            this.lstRounds.Location = new System.Drawing.Point(12, 118);
            this.lstRounds.Name = "lstRounds";
            this.lstRounds.Size = new System.Drawing.Size(368, 104);
            this.lstRounds.TabIndex = 1;
            this.lstRounds.FormattingEnabled = true;
            this.lstRounds.IntegralHeight = false;
            this.lstRounds.ItemHeight = 15;
            this.lstRounds.SelectedIndexChanged += new System.EventHandler(this.lstRounds_SelectedIndexChanged);
            // 
            // lblBusStep
            // 
            this.lblBusStep.AutoSize = false;
            this.lblBusStep.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblBusStep.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblBusStep.Location = new System.Drawing.Point(12, 228);
            this.lblBusStep.Name = "lblBusStep";
            this.lblBusStep.Size = new System.Drawing.Size(368, 20);
            this.lblBusStep.TabIndex = 2;
            this.lblBusStep.Text = "Bước 0 / 0";
            // 
            // trkBus
            // 
            this.trkBus.Location = new System.Drawing.Point(6, 250);
            this.trkBus.Name = "trkBus";
            this.trkBus.Size = new System.Drawing.Size(380, 45);
            this.trkBus.TabIndex = 3;
            this.trkBus.Maximum = 0;
            this.trkBus.Minimum = 0;
            this.trkBus.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkBus.Enabled = false;
            this.trkBus.Scroll += new System.EventHandler(this.trkBus_Scroll);
            // 
            // btnBusRestart
            // 
            this.btnBusRestart.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnBusRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBusRestart.FlatAppearance.BorderSize = 0;
            this.btnBusRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBusRestart.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBusRestart.Location = new System.Drawing.Point(12, 298);
            this.btnBusRestart.Name = "btnBusRestart";
            this.btnBusRestart.Size = new System.Drawing.Size(40, 32);
            this.btnBusRestart.TabIndex = 4;
            this.btnBusRestart.Text = "⟲";
            this.btnBusRestart.UseVisualStyleBackColor = false;
            this.btnBusRestart.Click += new System.EventHandler(this.btnBusRestart_Click);
            // 
            // btnBusBack
            // 
            this.btnBusBack.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnBusBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBusBack.FlatAppearance.BorderSize = 0;
            this.btnBusBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBusBack.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBusBack.Location = new System.Drawing.Point(56, 298);
            this.btnBusBack.Name = "btnBusBack";
            this.btnBusBack.Size = new System.Drawing.Size(64, 32);
            this.btnBusBack.TabIndex = 5;
            this.btnBusBack.Text = "◀ Lùi";
            this.btnBusBack.UseVisualStyleBackColor = false;
            this.btnBusBack.Click += new System.EventHandler(this.btnBusBack_Click);
            // 
            // btnBusPlay
            // 
            this.btnBusPlay.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnBusPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBusPlay.FlatAppearance.BorderSize = 0;
            this.btnBusPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBusPlay.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBusPlay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBusPlay.Location = new System.Drawing.Point(124, 298);
            this.btnBusPlay.Name = "btnBusPlay";
            this.btnBusPlay.Size = new System.Drawing.Size(100, 32);
            this.btnBusPlay.TabIndex = 6;
            this.btnBusPlay.Text = "▶ Chạy";
            this.btnBusPlay.UseVisualStyleBackColor = false;
            this.btnBusPlay.Click += new System.EventHandler(this.btnBusPlay_Click);
            // 
            // btnBusNext
            // 
            this.btnBusNext.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnBusNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBusNext.FlatAppearance.BorderSize = 0;
            this.btnBusNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBusNext.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBusNext.Location = new System.Drawing.Point(228, 298);
            this.btnBusNext.Name = "btnBusNext";
            this.btnBusNext.Size = new System.Drawing.Size(64, 32);
            this.btnBusNext.TabIndex = 7;
            this.btnBusNext.Text = "Tới ▶";
            this.btnBusNext.UseVisualStyleBackColor = false;
            this.btnBusNext.Click += new System.EventHandler(this.btnBusNext_Click);
            // 
            // btnBusRoundEnd
            // 
            this.btnBusRoundEnd.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnBusRoundEnd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBusRoundEnd.FlatAppearance.BorderSize = 0;
            this.btnBusRoundEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBusRoundEnd.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBusRoundEnd.Location = new System.Drawing.Point(296, 298);
            this.btnBusRoundEnd.Name = "btnBusRoundEnd";
            this.btnBusRoundEnd.Size = new System.Drawing.Size(84, 32);
            this.btnBusRoundEnd.TabIndex = 8;
            this.btnBusRoundEnd.Text = "⏭ Hết lượt";
            this.btnBusRoundEnd.UseVisualStyleBackColor = false;
            this.btnBusRoundEnd.Click += new System.EventHandler(this.btnBusRoundEnd_Click);
            // 
            // lblBusSpeed
            // 
            this.lblBusSpeed.AutoSize = false;
            this.lblBusSpeed.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblBusSpeed.Location = new System.Drawing.Point(12, 344);
            this.lblBusSpeed.Name = "lblBusSpeed";
            this.lblBusSpeed.Size = new System.Drawing.Size(56, 20);
            this.lblBusSpeed.TabIndex = 9;
            this.lblBusSpeed.Text = "Tốc độ";
            // 
            // trkBusSpeed
            // 
            this.trkBusSpeed.Location = new System.Drawing.Point(66, 338);
            this.trkBusSpeed.Name = "trkBusSpeed";
            this.trkBusSpeed.Size = new System.Drawing.Size(314, 45);
            this.trkBusSpeed.TabIndex = 10;
            this.trkBusSpeed.Maximum = 8;
            this.trkBusSpeed.Minimum = 1;
            this.trkBusSpeed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkBusSpeed.Value = 4;
            // 
            // lblBusNow
            // 
            this.lblBusNow.AutoSize = false;
            this.lblBusNow.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lblBusNow.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblBusNow.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblBusNow.Location = new System.Drawing.Point(12, 384);
            this.lblBusNow.Name = "lblBusNow";
            this.lblBusNow.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.lblBusNow.Size = new System.Drawing.Size(368, 150);
            this.lblBusNow.TabIndex = 11;
            this.lblBusNow.Text = "Chưa có dữ liệu. Chọn trạm A và trạm B rồi bấm \"Chuẩn bị mô phỏng\".";
            // 
            // lblBusLegend
            // 
            this.lblBusLegend.AutoSize = false;
            this.lblBusLegend.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblBusLegend.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBusLegend.Location = new System.Drawing.Point(12, 540);
            this.lblBusLegend.Name = "lblBusLegend";
            this.lblBusLegend.Size = new System.Drawing.Size(368, 50);
            this.lblBusLegend.TabIndex = 12;
            this.lblBusLegend.Text = "Trên bản đồ:  vòng cam = trạm đã xét   chấm xanh = trạm đang xét   cuối mỗi lượt hiện cách đi tìm được";
            // 
            // btnOpenModel
            // 
            this.btnOpenModel.BackColor = System.Drawing.Color.FromArgb(79, 70, 229);
            this.btnOpenModel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenModel.FlatAppearance.BorderSize = 0;
            this.btnOpenModel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenModel.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnOpenModel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnOpenModel.Location = new System.Drawing.Point(12, 568);
            this.btnOpenModel.Name = "btnOpenModel";
            this.btnOpenModel.Size = new System.Drawing.Size(368, 34);
            this.btnOpenModel.TabIndex = 13;
            this.btnOpenModel.Text = "🧩 Mở sơ đồ mô hình (dễ hiểu cho người xem)";
            this.btnOpenModel.UseVisualStyleBackColor = false;
            this.btnOpenModel.Click += new System.EventHandler(this.btnOpenModel_Click);
            // 
            // tabGraph
            // 
            this.tabGraph.Controls.Add(this.lblGraphHelp);
            this.tabGraph.Controls.Add(this.chkShowNodes);
            this.tabGraph.Controls.Add(this.chkShowWeights);
            this.tabGraph.Controls.Add(this.chkShowNames);
            this.tabGraph.Controls.Add(this.lblNode1);
            this.tabGraph.Controls.Add(this.txtNode1);
            this.tabGraph.Controls.Add(this.lblNode2);
            this.tabGraph.Controls.Add(this.txtNode2);
            this.tabGraph.Controls.Add(this.btnLookup);
            this.tabGraph.Controls.Add(this.lblLookup);
            this.tabGraph.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.tabGraph.Location = new System.Drawing.Point(4, 26);
            this.tabGraph.Name = "tabGraph";
            this.tabGraph.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabGraph.Size = new System.Drawing.Size(392, 610);
            this.tabGraph.TabIndex = 2;
            this.tabGraph.Text = "Đồ thị";
            // 
            // lblGraphHelp
            // 
            this.lblGraphHelp.AutoSize = false;
            this.lblGraphHelp.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblGraphHelp.Location = new System.Drawing.Point(12, 10);
            this.lblGraphHelp.Name = "lblGraphHelp";
            this.lblGraphHelp.Size = new System.Drawing.Size(368, 90);
            this.lblGraphHelp.TabIndex = 0;
            this.lblGraphHelp.Text = "Bản đồ đường là một ĐỒ THỊ: giao lộ là ĐỈNH, đoạn đường là CẠNH, chiều dài đoạn đường (mét) là TRỌNG SỐ. Thuật toán tìm đường cộng các trọng số này lại và chọn tổng nhỏ nhất.";
            // 
            // chkShowNodes
            // 
            this.chkShowNodes.Location = new System.Drawing.Point(12, 104);
            this.chkShowNodes.Name = "chkShowNodes";
            this.chkShowNodes.Size = new System.Drawing.Size(368, 24);
            this.chkShowNodes.TabIndex = 1;
            this.chkShowNodes.Text = "Hiện ĐỈNH (số của từng giao lộ)";
            this.chkShowNodes.AutoSize = false;
            this.chkShowNodes.CheckedChanged += new System.EventHandler(this.chkShowNodes_CheckedChanged);
            // 
            // chkShowWeights
            // 
            this.chkShowWeights.Location = new System.Drawing.Point(12, 130);
            this.chkShowWeights.Name = "chkShowWeights";
            this.chkShowWeights.Size = new System.Drawing.Size(368, 24);
            this.chkShowWeights.TabIndex = 2;
            this.chkShowWeights.Text = "Hiện TRỌNG SỐ (chiều dài từng đoạn đường, mét)";
            this.chkShowWeights.AutoSize = false;
            this.chkShowWeights.CheckedChanged += new System.EventHandler(this.chkShowWeights_CheckedChanged);
            // 
            // chkShowNames
            // 
            this.chkShowNames.Location = new System.Drawing.Point(12, 156);
            this.chkShowNames.Name = "chkShowNames";
            this.chkShowNames.Size = new System.Drawing.Size(368, 24);
            this.chkShowNames.TabIndex = 3;
            this.chkShowNames.Text = "Hiện tên địa điểm cụ thể (trạm, bệnh viện, trường...)";
            this.chkShowNames.AutoSize = false;
            this.chkShowNames.Checked = true;
            this.chkShowNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowNames.CheckedChanged += new System.EventHandler(this.chkShowNames_CheckedChanged);
            // 
            // lblNode1
            // 
            this.lblNode1.AutoSize = false;
            this.lblNode1.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblNode1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNode1.Location = new System.Drawing.Point(12, 196);
            this.lblNode1.Name = "lblNode1";
            this.lblNode1.Size = new System.Drawing.Size(60, 20);
            this.lblNode1.TabIndex = 4;
            this.lblNode1.Text = "Đỉnh 1";
            // 
            // txtNode1
            // 
            this.txtNode1.Location = new System.Drawing.Point(72, 193);
            this.txtNode1.Name = "txtNode1";
            this.txtNode1.Size = new System.Drawing.Size(80, 23);
            this.txtNode1.TabIndex = 5;
            // 
            // lblNode2
            // 
            this.lblNode2.AutoSize = false;
            this.lblNode2.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblNode2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNode2.Location = new System.Drawing.Point(168, 196);
            this.lblNode2.Name = "lblNode2";
            this.lblNode2.Size = new System.Drawing.Size(60, 20);
            this.lblNode2.TabIndex = 6;
            this.lblNode2.Text = "Đỉnh 2";
            // 
            // txtNode2
            // 
            this.txtNode2.Location = new System.Drawing.Point(228, 193);
            this.txtNode2.Name = "txtNode2";
            this.txtNode2.Size = new System.Drawing.Size(80, 23);
            this.txtNode2.TabIndex = 7;
            // 
            // btnLookup
            // 
            this.btnLookup.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnLookup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLookup.FlatAppearance.BorderSize = 0;
            this.btnLookup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLookup.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnLookup.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLookup.Location = new System.Drawing.Point(12, 224);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(368, 34);
            this.btnLookup.TabIndex = 8;
            this.btnLookup.Text = "Tra trọng số giữa hai đỉnh";
            this.btnLookup.UseVisualStyleBackColor = false;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            // 
            // lblLookup
            // 
            this.lblLookup.AutoSize = false;
            this.lblLookup.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lblLookup.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblLookup.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblLookup.Location = new System.Drawing.Point(12, 268);
            this.lblLookup.Name = "lblLookup";
            this.lblLookup.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.lblLookup.Size = new System.Drawing.Size(368, 260);
            this.lblLookup.TabIndex = 8;
            this.lblLookup.Text = "Gõ số đỉnh rồi bấm Tra. Để trống Đỉnh 2 thì xem các đỉnh nối với Đỉnh 1. Số đỉnh hiện trên bản đồ khi bật \"Hiện ĐỈNH\" (phóng to vừa đủ).";
            // 
            // tabTools
            // 
            this.tabTools.Controls.Add(this.lblBlockHelp);
            this.tabTools.Controls.Add(this.chkBlockMode);
            this.tabTools.Controls.Add(this.lblBlocked);
            this.tabTools.Controls.Add(this.btnBlockOnRoute);
            this.tabTools.Controls.Add(this.btnBlockRandom);
            this.tabTools.Controls.Add(this.btnBlockClear);
            this.tabTools.Controls.Add(this.btnSnapshot);
            this.tabTools.Controls.Add(this.lblToolNote);
            this.tabTools.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.tabTools.Location = new System.Drawing.Point(4, 26);
            this.tabTools.Name = "tabTools";
            this.tabTools.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabTools.Size = new System.Drawing.Size(392, 610);
            this.tabTools.TabIndex = 3;
            this.tabTools.Text = "Chặn đường";
            // 
            // lblBlockHelp
            // 
            this.lblBlockHelp.AutoSize = false;
            this.lblBlockHelp.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblBlockHelp.Location = new System.Drawing.Point(12, 10);
            this.lblBlockHelp.Name = "lblBlockHelp";
            this.lblBlockHelp.Size = new System.Drawing.Size(368, 78);
            this.lblBlockHelp.TabIndex = 0;
            this.lblBlockHelp.Text = "Giả lập đường bị tắc, ngập hay cấm. Thuật toán và xe buýt sẽ tự đi vòng. Đoạn bị chặn hiện đường đỏ nét đứt có dấu ✕; bấm lại để mở.";
            // 
            // chkBlockMode
            // 
            this.chkBlockMode.Location = new System.Drawing.Point(12, 94);
            this.chkBlockMode.Name = "chkBlockMode";
            this.chkBlockMode.Size = new System.Drawing.Size(368, 24);
            this.chkBlockMode.TabIndex = 1;
            this.chkBlockMode.Text = "Chế độ chặn đường (bấm vào đường trên bản đồ)";
            this.chkBlockMode.AutoSize = false;
            this.chkBlockMode.CheckedChanged += new System.EventHandler(this.chkBlockMode_CheckedChanged);
            // 
            // lblBlocked
            // 
            this.lblBlocked.AutoSize = false;
            this.lblBlocked.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblBlocked.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblBlocked.Location = new System.Drawing.Point(12, 124);
            this.lblBlocked.Name = "lblBlocked";
            this.lblBlocked.Size = new System.Drawing.Size(368, 20);
            this.lblBlocked.TabIndex = 2;
            this.lblBlocked.Text = "Đang chặn: 0 đoạn đường";
            // 
            // btnBlockOnRoute
            // 
            this.btnBlockOnRoute.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.btnBlockOnRoute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBlockOnRoute.FlatAppearance.BorderSize = 0;
            this.btnBlockOnRoute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBlockOnRoute.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBlockOnRoute.Location = new System.Drawing.Point(12, 152);
            this.btnBlockOnRoute.Name = "btnBlockOnRoute";
            this.btnBlockOnRoute.Size = new System.Drawing.Size(368, 34);
            this.btnBlockOnRoute.TabIndex = 3;
            this.btnBlockOnRoute.Text = "Chặn 1 đoạn trên đường đang đi";
            this.btnBlockOnRoute.UseVisualStyleBackColor = false;
            this.btnBlockOnRoute.Click += new System.EventHandler(this.btnBlockOnRoute_Click);
            // 
            // btnBlockRandom
            // 
            this.btnBlockRandom.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnBlockRandom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBlockRandom.FlatAppearance.BorderSize = 0;
            this.btnBlockRandom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBlockRandom.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBlockRandom.Location = new System.Drawing.Point(12, 192);
            this.btnBlockRandom.Name = "btnBlockRandom";
            this.btnBlockRandom.Size = new System.Drawing.Size(368, 34);
            this.btnBlockRandom.TabIndex = 4;
            this.btnBlockRandom.Text = "Chặn ngẫu nhiên 5 đoạn";
            this.btnBlockRandom.UseVisualStyleBackColor = false;
            this.btnBlockRandom.Click += new System.EventHandler(this.btnBlockRandom_Click);
            // 
            // btnBlockClear
            // 
            this.btnBlockClear.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnBlockClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBlockClear.FlatAppearance.BorderSize = 0;
            this.btnBlockClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBlockClear.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnBlockClear.Location = new System.Drawing.Point(12, 232);
            this.btnBlockClear.Name = "btnBlockClear";
            this.btnBlockClear.Size = new System.Drawing.Size(368, 34);
            this.btnBlockClear.TabIndex = 5;
            this.btnBlockClear.Text = "Mở tất cả đường";
            this.btnBlockClear.UseVisualStyleBackColor = false;
            this.btnBlockClear.Click += new System.EventHandler(this.btnBlockClear_Click);
            // 
            // btnSnapshot
            // 
            this.btnSnapshot.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnSnapshot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSnapshot.FlatAppearance.BorderSize = 0;
            this.btnSnapshot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSnapshot.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnSnapshot.Location = new System.Drawing.Point(12, 288);
            this.btnSnapshot.Name = "btnSnapshot";
            this.btnSnapshot.Size = new System.Drawing.Size(368, 34);
            this.btnSnapshot.TabIndex = 6;
            this.btnSnapshot.Text = "📷 Chụp ảnh bản đồ (PNG)";
            this.btnSnapshot.UseVisualStyleBackColor = false;
            this.btnSnapshot.Click += new System.EventHandler(this.btnSnapshot_Click);
            // 
            // lblToolNote
            // 
            this.lblToolNote.AutoSize = false;
            this.lblToolNote.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblToolNote.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblToolNote.Location = new System.Drawing.Point(12, 336);
            this.lblToolNote.Name = "lblToolNote";
            this.lblToolNote.Size = new System.Drawing.Size(368, 90);
            this.lblToolNote.TabIndex = 7;
            this.lblToolNote.Text = "Gợi ý demo: tìm đường A → B, bấm \"Chặn 1 đoạn trên đường đang đi\", xem tuyến đổi hướng; rồi mở Gợi ý xe buýt để thấy xe buýt cũng đi vòng.";
            // 
            // SimForm
            // 
            this.Controls.Add(this.tabSim);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(400, 640);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SimForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Bảng mô phỏng";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SimForm_FormClosed);
            this.tabSim.ResumeLayout(false);
            this.tabAlgo.ResumeLayout(false);
            this.tabBus.ResumeLayout(false);
            this.tabGraph.ResumeLayout(false);
            this.tabTools.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer playTimer;
        private System.Windows.Forms.TabControl tabSim;
        private System.Windows.Forms.TabPage tabAlgo;
        private System.Windows.Forms.Label lblAlgoHelp;
        private System.Windows.Forms.Button btnAlgoPrepare;
        private System.Windows.Forms.Label lblAlgoStep;
        private System.Windows.Forms.TrackBar trkAlgo;
        private System.Windows.Forms.Button btnAlgoRestart;
        private System.Windows.Forms.Button btnAlgoBack;
        private System.Windows.Forms.Button btnAlgoPlay;
        private System.Windows.Forms.Button btnAlgoNext;
        private System.Windows.Forms.Button btnAlgoEnd;
        private System.Windows.Forms.Label lblAlgoSpeed;
        private System.Windows.Forms.TrackBar trkAlgoSpeed;
        private System.Windows.Forms.Label lblAlgoNow;
        private System.Windows.Forms.Label lblAlgoLegend;
        private System.Windows.Forms.TabPage tabBus;
        private System.Windows.Forms.Label lblBusHelp;
        private System.Windows.Forms.Button btnBusPrepare;
        private System.Windows.Forms.ListBox lstRounds;
        private System.Windows.Forms.Label lblBusStep;
        private System.Windows.Forms.TrackBar trkBus;
        private System.Windows.Forms.Button btnBusRestart;
        private System.Windows.Forms.Button btnBusBack;
        private System.Windows.Forms.Button btnBusPlay;
        private System.Windows.Forms.Button btnBusNext;
        private System.Windows.Forms.Button btnBusRoundEnd;
        private System.Windows.Forms.Label lblBusSpeed;
        private System.Windows.Forms.TrackBar trkBusSpeed;
        private System.Windows.Forms.Label lblBusNow;
        private System.Windows.Forms.Label lblBusLegend;
        private System.Windows.Forms.Button btnOpenModel;
        private System.Windows.Forms.TabPage tabGraph;
        private System.Windows.Forms.Label lblGraphHelp;
        private System.Windows.Forms.CheckBox chkShowNodes;
        private System.Windows.Forms.CheckBox chkShowWeights;
        private System.Windows.Forms.CheckBox chkShowNames;
        private System.Windows.Forms.Label lblNode1;
        private System.Windows.Forms.TextBox txtNode1;
        private System.Windows.Forms.Label lblNode2;
        private System.Windows.Forms.TextBox txtNode2;
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.Label lblLookup;
        private System.Windows.Forms.TabPage tabTools;
        private System.Windows.Forms.Label lblBlockHelp;
        private System.Windows.Forms.CheckBox chkBlockMode;
        private System.Windows.Forms.Label lblBlocked;
        private System.Windows.Forms.Button btnBlockOnRoute;
        private System.Windows.Forms.Button btnBlockRandom;
        private System.Windows.Forms.Button btnBlockClear;
        private System.Windows.Forms.Button btnSnapshot;
        private System.Windows.Forms.Label lblToolNote;
    }
}
