namespace TestMap
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.busTimer = new System.Windows.Forms.Timer(this.components);
            this.liveTimer = new System.Windows.Forms.Timer(this.components);
            this.driverTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlTop = new System.Windows.Forms.Panel();
            this.flowTop = new System.Windows.Forms.FlowLayoutPanel();
            this.cboArea = new System.Windows.Forms.ComboBox();
            this.cboAlgo = new System.Windows.Forms.ComboBox();
            this.chkStops = new System.Windows.Forms.CheckBox();
            this.chkLines = new System.Windows.Forms.CheckBox();
            this.chkBuses = new System.Windows.Forms.CheckBox();
            this.chkPoi = new System.Windows.Forms.CheckBox();
            this.chkSearch = new System.Windows.Forms.CheckBox();
            this.btnCompare = new System.Windows.Forms.Button();
            this.btnLoadLines = new System.Windows.Forms.Button();
            this.btnLoadPoi = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnAccount = new System.Windows.Forms.Button();
            this.btnPoiFilter = new System.Windows.Forms.Button();
            this.btnSim = new System.Windows.Forms.Button();
            this.lblAccount = new System.Windows.Forms.Label();
            this.btnAdmin = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pnlSide = new System.Windows.Forms.Panel();
            this.pnlBus = new System.Windows.Forms.Panel();
            this.lstItins = new System.Windows.Forms.ListBox();
            this.pnlSteps = new System.Windows.Forms.Panel();
            this.lstSteps = new System.Windows.Forms.ListBox();
            this.pnlStepsHeader = new System.Windows.Forms.Panel();
            this.lblStepsTitle = new System.Windows.Forms.Label();
            this.lnkCopyBus = new System.Windows.Forms.LinkLabel();
            this.lblItinMsg = new System.Windows.Forms.Label();
            this.pnlItinFilter = new System.Windows.Forms.Panel();
            this.lblSort = new System.Windows.Forms.Label();
            this.cboItinSort = new System.Windows.Forms.ComboBox();
            this.lblItinCount = new System.Windows.Forms.Label();
            this.lblWalkLimit = new System.Windows.Forms.Label();
            this.cboWalkLimit = new System.Windows.Forms.ComboBox();
            this.chkWalkSuggest = new System.Windows.Forms.CheckBox();
            this.lblDemoBus = new System.Windows.Forms.Label();
            this.pnlShortest = new System.Windows.Forms.Panel();
            this.lstStops = new System.Windows.Forms.ListBox();
            this.pnlListTitle = new System.Windows.Forms.Panel();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.lnkCopy = new System.Windows.Forms.LinkLabel();
            this.pnlStats = new System.Windows.Forms.Panel();
            this.lblDist = new System.Windows.Forms.Label();
            this.lblDistCap = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblTimeCap = new System.Windows.Forms.Label();
            this.lblStopCount = new System.Windows.Forms.Label();
            this.lblStopCountCap = new System.Windows.Forms.Label();
            this.lblVisited = new System.Windows.Forms.Label();
            this.btnBus = new System.Windows.Forms.Button();
            this.pnlLines = new System.Windows.Forms.Panel();
            this.lstLines = new System.Windows.Forms.ListBox();
            this.lblDemoLines = new System.Windows.Forms.Label();
            this.pnlTabs = new System.Windows.Forms.Panel();
            this.btnTabBus = new System.Windows.Forms.Button();
            this.btnTabShortest = new System.Windows.Forms.Button();
            this.btnTabLines = new System.Windows.Forms.Button();
            this.btnTabDriver = new System.Windows.Forms.Button();
            this.pnlDriver = new System.Windows.Forms.Panel();
            this.lstDriverStops = new System.Windows.Forms.ListBox();
            this.pnlDriverTop = new System.Windows.Forms.Panel();
            this.lblDriverTitle = new System.Windows.Forms.Label();
            this.cboDriverLine = new System.Windows.Forms.ComboBox();
            this.btnShift = new System.Windows.Forms.Button();
            this.lblDriverInfo = new System.Windows.Forms.Label();
            this.pnlPlan = new System.Windows.Forms.Panel();
            this.lblPlanTitle = new System.Windows.Forms.Label();
            this.lblA = new System.Windows.Forms.Label();
            this.lblB = new System.Windows.Forms.Label();
            this.cboFrom = new System.Windows.Forms.ComboBox();
            this.cboTo = new System.Windows.Forms.ComboBox();
            this.btnSwap = new System.Windows.Forms.Button();
            this.btnLocate = new System.Windows.Forms.Button();
            this.cboFav = new System.Windows.Forms.ComboBox();
            this.btnFavAdd = new System.Windows.Forms.Button();
            this.cboRides = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.mapCanvas = new TestMap.Mapping.MapCanvas();
            this.pnlTop.SuspendLayout();
            this.flowTop.SuspendLayout();
            this.pnlSide.SuspendLayout();
            this.pnlBus.SuspendLayout();
            this.pnlSteps.SuspendLayout();
            this.pnlItinFilter.SuspendLayout();
            this.pnlStepsHeader.SuspendLayout();
            this.pnlShortest.SuspendLayout();
            this.pnlListTitle.SuspendLayout();
            this.pnlStats.SuspendLayout();
            this.pnlLines.SuspendLayout();
            this.pnlDriver.SuspendLayout();
            this.pnlDriverTop.SuspendLayout();
            this.pnlTabs.SuspendLayout();
            this.pnlPlan.SuspendLayout();
            this.pnlMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 30;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // busTimer
            // 
            this.busTimer.Interval = 30;
            this.busTimer.Tick += new System.EventHandler(this.BusTimer_Tick);
            // 
            // liveTimer
            // 
            this.liveTimer.Interval = 80;
            this.liveTimer.Tick += new System.EventHandler(this.LiveTimer_Tick);
            // 
            // driverTimer
            // 
            this.driverTimer.Interval = 100;
            this.driverTimer.Tick += new System.EventHandler(this.DriverTimer_Tick);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlTop.Controls.Add(this.flowTop);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1340, 84);
            this.pnlTop.TabIndex = 3;
            // 
            // flowTop
            // 
            this.flowTop.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.flowTop.Controls.Add(this.cboArea);
            this.flowTop.Controls.Add(this.cboAlgo);
            this.flowTop.Controls.Add(this.chkStops);
            this.flowTop.Controls.Add(this.chkLines);
            this.flowTop.Controls.Add(this.chkBuses);
            this.flowTop.Controls.Add(this.chkPoi);
            this.flowTop.Controls.Add(this.chkSearch);
            this.flowTop.Controls.Add(this.btnCompare);
            this.flowTop.Controls.Add(this.btnLoadLines);
            this.flowTop.Controls.Add(this.btnLoadPoi);
            this.flowTop.Controls.Add(this.btnReload);
            this.flowTop.Controls.Add(this.btnAccount);
            this.flowTop.Controls.Add(this.lblAccount);
            this.flowTop.Controls.Add(this.btnPoiFilter);
            this.flowTop.Controls.Add(this.btnSim);
            this.flowTop.Controls.Add(this.btnAdmin);
            this.flowTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowTop.Location = new System.Drawing.Point(0, 0);
            this.flowTop.Name = "flowTop";
            this.flowTop.Padding = new System.Windows.Forms.Padding(12, 10, 12, 0);
            this.flowTop.Size = new System.Drawing.Size(1340, 84);
            this.flowTop.TabIndex = 0;
            this.flowTop.WrapContents = true;
            this.flowTop.SetFlowBreak(this.btnReload, true);
            // 
            // cboArea
            // 
            this.cboArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboArea.FormattingEnabled = true;
            this.cboArea.Location = new System.Drawing.Point(12, 13);
            this.cboArea.Margin = new System.Windows.Forms.Padding(0, 3, 14, 0);
            this.cboArea.Name = "cboArea";
            this.cboArea.Size = new System.Drawing.Size(180, 23);
            this.cboArea.TabIndex = 0;
            this.cboArea.SelectionChangeCommitted += new System.EventHandler(this.cboArea_SelectionChangeCommitted);
            // 
            // cboAlgo
            // 
            this.cboAlgo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlgo.FormattingEnabled = true;
            this.cboAlgo.Items.AddRange(new object[] {
            "Dijkstra",
            "A*"});
            this.cboAlgo.Location = new System.Drawing.Point(206, 13);
            this.cboAlgo.Margin = new System.Windows.Forms.Padding(0, 3, 14, 0);
            this.cboAlgo.Name = "cboAlgo";
            this.cboAlgo.SelectedIndex = 1;
            this.cboAlgo.Size = new System.Drawing.Size(90, 23);
            this.cboAlgo.TabIndex = 1;
            // 
            // chkStops
            // 
            this.chkStops.AutoSize = true;
            this.chkStops.Checked = true;
            this.chkStops.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStops.ForeColor = System.Drawing.Color.Gainsboro;
            this.chkStops.Location = new System.Drawing.Point(310, 16);
            this.chkStops.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.chkStops.Name = "chkStops";
            this.chkStops.Size = new System.Drawing.Size(81, 19);
            this.chkStops.TabIndex = 2;
            this.chkStops.Text = "Hiện trạm";
            this.chkStops.CheckedChanged += new System.EventHandler(this.chkStops_CheckedChanged);
            // 
            // chkLines
            // 
            this.chkLines.AutoSize = true;
            this.chkLines.Checked = true;
            this.chkLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLines.ForeColor = System.Drawing.Color.Gainsboro;
            this.chkLines.Location = new System.Drawing.Point(401, 16);
            this.chkLines.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.chkLines.Name = "chkLines";
            this.chkLines.Size = new System.Drawing.Size(84, 19);
            this.chkLines.TabIndex = 3;
            this.chkLines.Text = "Hiện tuyến";
            this.chkLines.CheckedChanged += new System.EventHandler(this.chkView_CheckedChanged);
            // 
            // chkBuses
            // 
            this.chkBuses.AutoSize = true;
            this.chkBuses.Checked = true;
            this.chkBuses.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBuses.ForeColor = System.Drawing.Color.Gainsboro;
            this.chkBuses.Location = new System.Drawing.Point(495, 16);
            this.chkBuses.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.chkBuses.Name = "chkBuses";
            this.chkBuses.Size = new System.Drawing.Size(70, 19);
            this.chkBuses.TabIndex = 4;
            this.chkBuses.Text = "Xe chạy";
            this.chkBuses.CheckedChanged += new System.EventHandler(this.chkView_CheckedChanged);
            // 
            // chkPoi
            // 
            this.chkPoi.AutoSize = true;
            this.chkPoi.Checked = true;
            this.chkPoi.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPoi.ForeColor = System.Drawing.Color.Gainsboro;
            this.chkPoi.Location = new System.Drawing.Point(575, 16);
            this.chkPoi.Margin = new System.Windows.Forms.Padding(0, 6, 10, 0);
            this.chkPoi.Name = "chkPoi";
            this.chkPoi.Size = new System.Drawing.Size(100, 19);
            this.chkPoi.TabIndex = 5;
            this.chkPoi.Text = "Hiện địa điểm";
            this.chkPoi.CheckedChanged += new System.EventHandler(this.chkPoi_CheckedChanged);
            // 
            // chkSearch
            // 
            this.chkSearch.AutoSize = true;
            this.chkSearch.ForeColor = System.Drawing.Color.Gainsboro;
            this.chkSearch.Location = new System.Drawing.Point(575, 16);
            this.chkSearch.Margin = new System.Windows.Forms.Padding(0, 6, 14, 0);
            this.chkSearch.Name = "chkSearch";
            this.chkSearch.Size = new System.Drawing.Size(109, 19);
            this.chkSearch.TabIndex = 5;
            this.chkSearch.Text = "Xem thuật toán";
            // 
            // btnCompare
            // 
            this.btnCompare.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnCompare.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCompare.FlatAppearance.BorderSize = 0;
            this.btnCompare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCompare.ForeColor = System.Drawing.Color.White;
            this.btnCompare.Location = new System.Drawing.Point(698, 10);
            this.btnCompare.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(76, 32);
            this.btnCompare.TabIndex = 6;
            this.btnCompare.Text = "So sánh";
            this.btnCompare.UseVisualStyleBackColor = false;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // btnLoadLines
            // 
            this.btnLoadLines.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnLoadLines.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadLines.FlatAppearance.BorderSize = 0;
            this.btnLoadLines.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadLines.ForeColor = System.Drawing.Color.White;
            this.btnLoadLines.Location = new System.Drawing.Point(782, 10);
            this.btnLoadLines.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnLoadLines.Name = "btnLoadLines";
            this.btnLoadLines.Size = new System.Drawing.Size(110, 32);
            this.btnLoadLines.TabIndex = 7;
            this.btnLoadLines.Text = "Tải tuyến thật";
            this.btnLoadLines.UseVisualStyleBackColor = false;
            this.btnLoadLines.Click += new System.EventHandler(this.btnLoadLines_Click);
            // 
            // btnLoadPoi
            // 
            this.btnLoadPoi.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnLoadPoi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadPoi.FlatAppearance.BorderSize = 0;
            this.btnLoadPoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadPoi.ForeColor = System.Drawing.Color.White;
            this.btnLoadPoi.Location = new System.Drawing.Point(900, 10);
            this.btnLoadPoi.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnLoadPoi.Name = "btnLoadPoi";
            this.btnLoadPoi.Size = new System.Drawing.Size(118, 32);
            this.btnLoadPoi.TabIndex = 7;
            this.btnLoadPoi.Text = "Tải địa điểm";
            this.btnLoadPoi.UseVisualStyleBackColor = false;
            this.btnLoadPoi.Click += new System.EventHandler(this.btnLoadPoi_Click);
            // 
            // btnReload
            // 
            this.btnReload.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.ForeColor = System.Drawing.Color.White;
            this.btnReload.Location = new System.Drawing.Point(900, 10);
            this.btnReload.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(116, 32);
            this.btnReload.TabIndex = 8;
            this.btnReload.Text = "Tải lại dữ liệu";
            this.btnReload.UseVisualStyleBackColor = false;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnAccount
            // 
            this.btnAccount.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnAccount.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAccount.FlatAppearance.BorderSize = 0;
            this.btnAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccount.ForeColor = System.Drawing.Color.White;
            this.btnAccount.Location = new System.Drawing.Point(12, 50);
            this.btnAccount.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnAccount.Name = "btnAccount";
            this.btnAccount.Size = new System.Drawing.Size(120, 32);
            this.btnAccount.TabIndex = 7;
            this.btnAccount.Text = "Đăng nhập";
            this.btnAccount.UseVisualStyleBackColor = false;
            this.btnAccount.Click += new System.EventHandler(this.btnAccount_Click);
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = false;
            this.lblAccount.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.lblAccount.Location = new System.Drawing.Point(140, 50);
            this.lblAccount.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(300, 32);
            this.lblAccount.TabIndex = 9;
            this.lblAccount.Text = "Chưa đăng nhập (dùng với tư cách khách)";
            this.lblAccount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPoiFilter
            // 
            this.btnPoiFilter.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnPoiFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPoiFilter.FlatAppearance.BorderSize = 0;
            this.btnPoiFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPoiFilter.ForeColor = System.Drawing.Color.White;
            this.btnPoiFilter.Location = new System.Drawing.Point(0, 0);
            this.btnPoiFilter.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnPoiFilter.Name = "btnPoiFilter";
            this.btnPoiFilter.Size = new System.Drawing.Size(118, 32);
            this.btnPoiFilter.TabIndex = 10;
            this.btnPoiFilter.Text = "Lọc địa điểm";
            this.btnPoiFilter.UseVisualStyleBackColor = false;
            this.btnPoiFilter.Click += new System.EventHandler(this.btnPoiFilter_Click);
            // 
            // btnSim
            // 
            this.btnSim.BackColor = System.Drawing.Color.FromArgb(79, 70, 229);
            this.btnSim.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSim.FlatAppearance.BorderSize = 0;
            this.btnSim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSim.ForeColor = System.Drawing.Color.White;
            this.btnSim.Location = new System.Drawing.Point(0, 0);
            this.btnSim.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnSim.Name = "btnSim";
            this.btnSim.Size = new System.Drawing.Size(118, 32);
            this.btnSim.TabIndex = 10;
            this.btnSim.Text = "⚙ Mô phỏng";
            this.btnSim.UseVisualStyleBackColor = false;
            this.btnSim.Click += new System.EventHandler(this.btnSim_Click);
            // 
            // btnAdmin
            // 
            this.btnAdmin.BackColor = System.Drawing.Color.FromArgb(180, 83, 9);
            this.btnAdmin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdmin.FlatAppearance.BorderSize = 0;
            this.btnAdmin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdmin.ForeColor = System.Drawing.Color.White;
            this.btnAdmin.Location = new System.Drawing.Point(508, 50);
            this.btnAdmin.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdmin.Name = "btnAdmin";
            this.btnAdmin.Size = new System.Drawing.Size(92, 32);
            this.btnAdmin.TabIndex = 9;
            this.btnAdmin.Text = "Quản trị";
            this.btnAdmin.Visible = false;
            this.btnAdmin.UseVisualStyleBackColor = false;
            this.btnAdmin.Click += new System.EventHandler(this.btnAdmin_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoEllipsis = true;
            this.lblInfo.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.lblInfo.Location = new System.Drawing.Point(0, 750);
            this.lblInfo.AutoSize = false;
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.lblInfo.Size = new System.Drawing.Size(1340, 30);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Đang khởi động...";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSide
            // 
            this.pnlSide.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlSide.Controls.Add(this.pnlBus);
            this.pnlSide.Controls.Add(this.pnlShortest);
            this.pnlSide.Controls.Add(this.pnlLines);
            this.pnlSide.Controls.Add(this.pnlDriver);
            this.pnlSide.Controls.Add(this.pnlTabs);
            this.pnlSide.Controls.Add(this.pnlPlan);
            this.pnlSide.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSide.Location = new System.Drawing.Point(940, 84);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(400, 666);
            this.pnlSide.TabIndex = 1;
            // 
            // pnlBus  (tab 1: gợi ý xe buýt)
            // 
            this.pnlBus.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlBus.Controls.Add(this.lstItins);
            this.pnlBus.Controls.Add(this.pnlSteps);
            this.pnlBus.Controls.Add(this.lblItinMsg);
            this.pnlBus.Controls.Add(this.pnlItinFilter);
            this.pnlBus.Controls.Add(this.lblDemoBus);
            this.pnlBus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBus.Location = new System.Drawing.Point(0, 238);
            this.pnlBus.Name = "pnlBus";
            this.pnlBus.Size = new System.Drawing.Size(400, 460);
            this.pnlBus.TabIndex = 4;
            // 
            // lstItins
            // 
            this.lstItins.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lstItins.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstItins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstItins.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstItins.FormattingEnabled = true;
            this.lstItins.IntegralHeight = false;
            this.lstItins.ItemHeight = 168;
            this.lstItins.Location = new System.Drawing.Point(0, 50);
            this.lstItins.Name = "lstItins";
            this.lstItins.Size = new System.Drawing.Size(400, 210);
            this.lstItins.TabIndex = 0;
            this.lstItins.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstItins_DrawItem);
            this.lstItins.SelectedIndexChanged += new System.EventHandler(this.lstItins_SelectedIndexChanged);
            // 
            // pnlSteps
            // 
            this.pnlSteps.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlSteps.Controls.Add(this.lstSteps);
            this.pnlSteps.Controls.Add(this.pnlStepsHeader);
            this.pnlSteps.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSteps.Location = new System.Drawing.Point(0, 260);
            this.pnlSteps.Name = "pnlSteps";
            this.pnlSteps.Size = new System.Drawing.Size(400, 200);
            this.pnlSteps.TabIndex = 1;
            // 
            // lstSteps
            // 
            this.lstSteps.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lstSteps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSteps.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstSteps.FormattingEnabled = true;
            this.lstSteps.IntegralHeight = false;
            this.lstSteps.ItemHeight = 50;
            this.lstSteps.Location = new System.Drawing.Point(0, 28);
            this.lstSteps.Name = "lstSteps";
            this.lstSteps.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstSteps.Size = new System.Drawing.Size(400, 172);
            this.lstSteps.TabIndex = 0;
            this.lstSteps.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstSteps_DrawItem);
            // 
            // pnlStepsHeader
            // 
            this.pnlStepsHeader.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.pnlStepsHeader.Controls.Add(this.lblStepsTitle);
            this.pnlStepsHeader.Controls.Add(this.lnkCopyBus);
            this.pnlStepsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStepsHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlStepsHeader.Name = "pnlStepsHeader";
            this.pnlStepsHeader.Size = new System.Drawing.Size(400, 28);
            this.pnlStepsHeader.TabIndex = 1;
            // 
            // lblStepsTitle
            // 
            this.lblStepsTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStepsTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblStepsTitle.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblStepsTitle.Location = new System.Drawing.Point(0, 0);
            this.lblStepsTitle.AutoSize = false;
            this.lblStepsTitle.Name = "lblStepsTitle";
            this.lblStepsTitle.Size = new System.Drawing.Size(322, 28);
            this.lblStepsTitle.TabIndex = 1;
            this.lblStepsTitle.Text = "  CHI TIẾT CÁCH ĐI";
            this.lblStepsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lnkCopyBus
            // 
            this.lnkCopyBus.ActiveLinkColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lnkCopyBus.AutoSize = false;
            this.lnkCopyBus.Dock = System.Windows.Forms.DockStyle.Right;
            this.lnkCopyBus.LinkColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lnkCopyBus.Location = new System.Drawing.Point(322, 0);
            this.lnkCopyBus.Name = "lnkCopyBus";
            this.lnkCopyBus.Size = new System.Drawing.Size(78, 28);
            this.lnkCopyBus.TabIndex = 0;
            this.lnkCopyBus.TabStop = true;
            this.lnkCopyBus.Text = "Sao chép";
            this.lnkCopyBus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkCopyBus.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCopyBus_LinkClicked);
            // 
            // pnlItinFilter  (bảng lọc / sắp xếp gợi ý)
            // 
            this.pnlItinFilter.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.pnlItinFilter.Controls.Add(this.lblSort);
            this.pnlItinFilter.Controls.Add(this.cboItinSort);
            this.pnlItinFilter.Controls.Add(this.lblItinCount);
            this.pnlItinFilter.Controls.Add(this.lblWalkLimit);
            this.pnlItinFilter.Controls.Add(this.cboWalkLimit);
            this.pnlItinFilter.Controls.Add(this.chkWalkSuggest);
            this.pnlItinFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlItinFilter.Location = new System.Drawing.Point(0, 50);
            this.pnlItinFilter.Name = "pnlItinFilter";
            this.pnlItinFilter.Size = new System.Drawing.Size(400, 68);
            this.pnlItinFilter.TabIndex = 4;
            // 
            // lblSort
            // 
            this.lblSort.AutoSize = false;
            this.lblSort.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblSort.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblSort.Location = new System.Drawing.Point(16, 9);
            this.lblSort.Name = "lblSort";
            this.lblSort.Size = new System.Drawing.Size(70, 20);
            this.lblSort.TabIndex = 0;
            this.lblSort.Text = "Sắp xếp";
            // 
            // cboItinSort
            // 
            this.cboItinSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboItinSort.FormattingEnabled = true;
            this.cboItinSort.Items.AddRange(new object[] {
            "Nhanh nhất",
            "Ít đổi tuyến nhất",
            "Rẻ nhất",
            "Ít đi bộ nhất"});
            this.cboItinSort.Location = new System.Drawing.Point(90, 6);
            this.cboItinSort.Name = "cboItinSort";
            this.cboItinSort.SelectedIndex = 0;
            this.cboItinSort.Size = new System.Drawing.Size(140, 23);
            this.cboItinSort.TabIndex = 1;
            this.cboItinSort.SelectedIndexChanged += new System.EventHandler(this.cboItinSort_SelectedIndexChanged);
            // 
            // lblItinCount
            // 
            this.lblItinCount.AutoSize = false;
            this.lblItinCount.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblItinCount.Location = new System.Drawing.Point(240, 9);
            this.lblItinCount.Name = "lblItinCount";
            this.lblItinCount.Size = new System.Drawing.Size(144, 20);
            this.lblItinCount.TabIndex = 2;
            this.lblItinCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblWalkLimit
            // 
            this.lblWalkLimit.AutoSize = false;
            this.lblWalkLimit.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblWalkLimit.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblWalkLimit.Location = new System.Drawing.Point(16, 39);
            this.lblWalkLimit.Name = "lblWalkLimit";
            this.lblWalkLimit.Size = new System.Drawing.Size(70, 20);
            this.lblWalkLimit.TabIndex = 3;
            this.lblWalkLimit.Text = "Đi bộ tối đa";
            // 
            // cboWalkLimit
            // 
            this.cboWalkLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWalkLimit.FormattingEnabled = true;
            this.cboWalkLimit.Items.AddRange(new object[] {
            "Tùy ý",
            "300 m",
            "500 m",
            "800 m",
            "1200 m"});
            this.cboWalkLimit.Location = new System.Drawing.Point(90, 36);
            this.cboWalkLimit.Name = "cboWalkLimit";
            this.cboWalkLimit.SelectedIndex = 0;
            this.cboWalkLimit.Size = new System.Drawing.Size(90, 23);
            this.cboWalkLimit.TabIndex = 4;
            this.cboWalkLimit.SelectedIndexChanged += new System.EventHandler(this.cboWalkLimit_SelectedIndexChanged);
            // 
            // chkWalkSuggest
            // 
            this.chkWalkSuggest.AutoSize = true;
            this.chkWalkSuggest.Checked = true;
            this.chkWalkSuggest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWalkSuggest.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.chkWalkSuggest.Location = new System.Drawing.Point(196, 38);
            this.chkWalkSuggest.Name = "chkWalkSuggest";
            this.chkWalkSuggest.Size = new System.Drawing.Size(150, 19);
            this.chkWalkSuggest.TabIndex = 5;
            this.chkWalkSuggest.Text = "Gợi ý đi bộ (khi không có xe)";
            this.chkWalkSuggest.CheckedChanged += new System.EventHandler(this.chkWalkSuggest_CheckedChanged);
            // 
            // lblItinMsg
            // 
            this.lblItinMsg.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lblItinMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblItinMsg.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblItinMsg.Location = new System.Drawing.Point(0, 0);
            this.lblItinMsg.AutoSize = false;
            this.lblItinMsg.Name = "lblItinMsg";
            this.lblItinMsg.Padding = new System.Windows.Forms.Padding(16, 8, 16, 0);
            this.lblItinMsg.Size = new System.Drawing.Size(400, 50);
            this.lblItinMsg.TabIndex = 2;
            this.lblItinMsg.Text = "Chọn điểm đi và điểm đến (bấm vào trạm trên bản đồ) để xem các cách đi bằng xe buýt.";
            // 
            // lblDemoBus  (dải cảnh báo vàng, chỉ hiện khi đang dùng tuyến demo)
            // 
            this.lblDemoBus.BackColor = System.Drawing.Color.FromArgb(254, 243, 199);
            this.lblDemoBus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDemoBus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDemoBus.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.lblDemoBus.Location = new System.Drawing.Point(0, 0);
            this.lblDemoBus.AutoSize = false;
            this.lblDemoBus.Name = "lblDemoBus";
            this.lblDemoBus.Padding = new System.Windows.Forms.Padding(14, 8, 14, 6);
            this.lblDemoBus.Size = new System.Drawing.Size(400, 58);
            this.lblDemoBus.TabIndex = 3;
            this.lblDemoBus.Text = "Đang dùng tuyến DEMO (tự tạo, số tuyến D1, D2...). Bấm \"Tải tuyến thật\" ở thanh trên để lấy tuyến xe buýt thật từ OpenStreetMap.";
            this.lblDemoBus.Visible = false;
            // 
            // pnlShortest  (tab 2: đường ngắn nhất)
            // 
            this.pnlShortest.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlShortest.Controls.Add(this.lstStops);
            this.pnlShortest.Controls.Add(this.pnlListTitle);
            this.pnlShortest.Controls.Add(this.pnlStats);
            this.pnlShortest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShortest.Location = new System.Drawing.Point(0, 238);
            this.pnlShortest.Name = "pnlShortest";
            this.pnlShortest.Size = new System.Drawing.Size(400, 460);
            this.pnlShortest.TabIndex = 5;
            // 
            // lstStops
            // 
            this.lstStops.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lstStops.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstStops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStops.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstStops.FormattingEnabled = true;
            this.lstStops.IntegralHeight = false;
            this.lstStops.ItemHeight = 46;
            this.lstStops.Location = new System.Drawing.Point(0, 172);
            this.lstStops.Name = "lstStops";
            this.lstStops.Size = new System.Drawing.Size(400, 288);
            this.lstStops.TabIndex = 0;
            this.lstStops.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstStops_DrawItem);
            this.lstStops.SelectedIndexChanged += new System.EventHandler(this.lstStops_SelectedIndexChanged);
            // 
            // pnlListTitle
            // 
            this.pnlListTitle.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.pnlListTitle.Controls.Add(this.lblListTitle);
            this.pnlListTitle.Controls.Add(this.lnkCopy);
            this.pnlListTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlListTitle.Location = new System.Drawing.Point(0, 142);
            this.pnlListTitle.Name = "pnlListTitle";
            this.pnlListTitle.Size = new System.Drawing.Size(400, 30);
            this.pnlListTitle.TabIndex = 1;
            // 
            // lblListTitle
            // 
            this.lblListTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblListTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblListTitle.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblListTitle.Location = new System.Drawing.Point(0, 0);
            this.lblListTitle.AutoSize = false;
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Size = new System.Drawing.Size(322, 30);
            this.lblListTitle.TabIndex = 1;
            this.lblListTitle.Text = "  CÁC TRẠM ĐI QUA";
            this.lblListTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lnkCopy
            // 
            this.lnkCopy.ActiveLinkColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lnkCopy.AutoSize = false;
            this.lnkCopy.Dock = System.Windows.Forms.DockStyle.Right;
            this.lnkCopy.LinkColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lnkCopy.Location = new System.Drawing.Point(322, 0);
            this.lnkCopy.Name = "lnkCopy";
            this.lnkCopy.Size = new System.Drawing.Size(78, 30);
            this.lnkCopy.TabIndex = 0;
            this.lnkCopy.TabStop = true;
            this.lnkCopy.Text = "Sao chép";
            this.lnkCopy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkCopy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCopy_LinkClicked);
            // 
            // pnlStats
            // 
            this.pnlStats.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlStats.Controls.Add(this.lblDist);
            this.pnlStats.Controls.Add(this.lblDistCap);
            this.pnlStats.Controls.Add(this.lblTime);
            this.pnlStats.Controls.Add(this.lblTimeCap);
            this.pnlStats.Controls.Add(this.lblStopCount);
            this.pnlStats.Controls.Add(this.lblStopCountCap);
            this.pnlStats.Controls.Add(this.lblVisited);
            this.pnlStats.Controls.Add(this.btnBus);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStats.Location = new System.Drawing.Point(0, 0);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Size = new System.Drawing.Size(400, 142);
            this.pnlStats.TabIndex = 2;
            // 
            // lblDist
            // 
            this.lblDist.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblDist.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblDist.Location = new System.Drawing.Point(16, 10);
            this.lblDist.AutoSize = false;
            this.lblDist.Name = "lblDist";
            this.lblDist.Size = new System.Drawing.Size(104, 32);
            this.lblDist.TabIndex = 0;
            this.lblDist.Text = "—";
            // 
            // lblDistCap
            // 
            this.lblDistCap.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblDistCap.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDistCap.Location = new System.Drawing.Point(16, 42);
            this.lblDistCap.AutoSize = false;
            this.lblDistCap.Name = "lblDistCap";
            this.lblDistCap.Size = new System.Drawing.Size(104, 20);
            this.lblDistCap.TabIndex = 1;
            this.lblDistCap.Text = "Quãng đường";
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTime.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblTime.Location = new System.Drawing.Point(126, 10);
            this.lblTime.AutoSize = false;
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(104, 32);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "—";
            // 
            // lblTimeCap
            // 
            this.lblTimeCap.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblTimeCap.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblTimeCap.Location = new System.Drawing.Point(126, 42);
            this.lblTimeCap.AutoSize = false;
            this.lblTimeCap.Name = "lblTimeCap";
            this.lblTimeCap.Size = new System.Drawing.Size(104, 20);
            this.lblTimeCap.TabIndex = 3;
            this.lblTimeCap.Text = "Thời gian";
            // 
            // lblStopCount
            // 
            this.lblStopCount.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblStopCount.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblStopCount.Location = new System.Drawing.Point(236, 10);
            this.lblStopCount.AutoSize = false;
            this.lblStopCount.Name = "lblStopCount";
            this.lblStopCount.Size = new System.Drawing.Size(104, 32);
            this.lblStopCount.TabIndex = 4;
            this.lblStopCount.Text = "—";
            // 
            // lblStopCountCap
            // 
            this.lblStopCountCap.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblStopCountCap.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStopCountCap.Location = new System.Drawing.Point(236, 42);
            this.lblStopCountCap.AutoSize = false;
            this.lblStopCountCap.Name = "lblStopCountCap";
            this.lblStopCountCap.Size = new System.Drawing.Size(104, 20);
            this.lblStopCountCap.TabIndex = 5;
            this.lblStopCountCap.Text = "Số trạm";
            // 
            // lblVisited
            // 
            this.lblVisited.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblVisited.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblVisited.Location = new System.Drawing.Point(16, 66);
            this.lblVisited.AutoSize = false;
            this.lblVisited.Name = "lblVisited";
            this.lblVisited.Size = new System.Drawing.Size(320, 20);
            this.lblVisited.TabIndex = 6;
            // 
            // btnBus
            // 
            this.btnBus.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnBus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBus.FlatAppearance.BorderSize = 0;
            this.btnBus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBus.ForeColor = System.Drawing.Color.White;
            this.btnBus.Location = new System.Drawing.Point(16, 96);
            this.btnBus.Name = "btnBus";
            this.btnBus.Size = new System.Drawing.Size(130, 34);
            this.btnBus.TabIndex = 7;
            this.btnBus.Text = "► Chạy thử";
            this.btnBus.UseVisualStyleBackColor = false;
            this.btnBus.Click += new System.EventHandler(this.btnBus_Click);
            // 
            // pnlLines  (tab 3: các tuyến)
            // 
            this.pnlLines.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlLines.Controls.Add(this.lstLines);
            this.pnlLines.Controls.Add(this.lblDemoLines);
            this.pnlLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLines.Location = new System.Drawing.Point(0, 238);
            this.pnlLines.Name = "pnlLines";
            this.pnlLines.Size = new System.Drawing.Size(400, 460);
            this.pnlLines.TabIndex = 6;
            // 
            // lstLines
            // 
            this.lstLines.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lstLines.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLines.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstLines.FormattingEnabled = true;
            this.lstLines.IntegralHeight = false;
            this.lstLines.ItemHeight = 66;
            this.lstLines.Location = new System.Drawing.Point(0, 58);
            this.lstLines.Name = "lstLines";
            this.lstLines.Size = new System.Drawing.Size(400, 402);
            this.lstLines.TabIndex = 0;
            this.lstLines.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstLines_DrawItem);
            this.lstLines.SelectedIndexChanged += new System.EventHandler(this.lstLines_SelectedIndexChanged);
            // 
            // lblDemoLines  (dải cảnh báo vàng, chỉ hiện khi đang dùng tuyến demo)
            // 
            this.lblDemoLines.BackColor = System.Drawing.Color.FromArgb(254, 243, 199);
            this.lblDemoLines.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDemoLines.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDemoLines.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.lblDemoLines.Location = new System.Drawing.Point(0, 0);
            this.lblDemoLines.AutoSize = false;
            this.lblDemoLines.Name = "lblDemoLines";
            this.lblDemoLines.Padding = new System.Windows.Forms.Padding(14, 8, 14, 6);
            this.lblDemoLines.Size = new System.Drawing.Size(400, 58);
            this.lblDemoLines.TabIndex = 1;
            this.lblDemoLines.Text = "Đang dùng tuyến DEMO (tự tạo, số tuyến D1, D2...). Bấm \"Tải tuyến thật\" ở thanh trên để lấy tuyến xe buýt thật từ OpenStreetMap.";
            this.lblDemoLines.Visible = false;
            // 
            // pnlDriver  (tab 4: ca làm việc của tài xế, chỉ hiện khi đăng nhập tài xế)
            // 
            this.pnlDriver.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlDriver.Controls.Add(this.lstDriverStops);
            this.pnlDriver.Controls.Add(this.pnlDriverTop);
            this.pnlDriver.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDriver.Location = new System.Drawing.Point(0, 238);
            this.pnlDriver.Name = "pnlDriver";
            this.pnlDriver.Size = new System.Drawing.Size(400, 428);
            this.pnlDriver.TabIndex = 7;
            // 
            // lstDriverStops
            // 
            this.lstDriverStops.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lstDriverStops.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstDriverStops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDriverStops.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lstDriverStops.FormattingEnabled = true;
            this.lstDriverStops.IntegralHeight = false;
            this.lstDriverStops.ItemHeight = 17;
            this.lstDriverStops.Location = new System.Drawing.Point(0, 196);
            this.lstDriverStops.Name = "lstDriverStops";
            this.lstDriverStops.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstDriverStops.Size = new System.Drawing.Size(400, 232);
            this.lstDriverStops.TabIndex = 0;
            // 
            // pnlDriverTop
            // 
            this.pnlDriverTop.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlDriverTop.Controls.Add(this.lblDriverTitle);
            this.pnlDriverTop.Controls.Add(this.cboDriverLine);
            this.pnlDriverTop.Controls.Add(this.btnShift);
            this.pnlDriverTop.Controls.Add(this.lblDriverInfo);
            this.pnlDriverTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDriverTop.Location = new System.Drawing.Point(0, 0);
            this.pnlDriverTop.Name = "pnlDriverTop";
            this.pnlDriverTop.Size = new System.Drawing.Size(400, 196);
            this.pnlDriverTop.TabIndex = 1;
            // 
            // lblDriverTitle
            // 
            this.lblDriverTitle.AutoSize = false;
            this.lblDriverTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblDriverTitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblDriverTitle.Location = new System.Drawing.Point(16, 12);
            this.lblDriverTitle.Name = "lblDriverTitle";
            this.lblDriverTitle.Size = new System.Drawing.Size(368, 20);
            this.lblDriverTitle.TabIndex = 0;
            this.lblDriverTitle.Text = "CA LÀM VIỆC CỦA TÀI XẾ";
            // 
            // cboDriverLine
            // 
            this.cboDriverLine.DropDownHeight = 320;
            this.cboDriverLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDriverLine.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboDriverLine.FormattingEnabled = true;
            this.cboDriverLine.IntegralHeight = false;
            this.cboDriverLine.Location = new System.Drawing.Point(16, 38);
            this.cboDriverLine.Name = "cboDriverLine";
            this.cboDriverLine.Size = new System.Drawing.Size(368, 25);
            this.cboDriverLine.TabIndex = 1;
            // 
            // btnShift
            // 
            this.btnShift.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnShift.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShift.FlatAppearance.BorderSize = 0;
            this.btnShift.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShift.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnShift.ForeColor = System.Drawing.Color.White;
            this.btnShift.Location = new System.Drawing.Point(16, 72);
            this.btnShift.Name = "btnShift";
            this.btnShift.Size = new System.Drawing.Size(368, 36);
            this.btnShift.TabIndex = 2;
            this.btnShift.Text = "▶ Bắt đầu ca";
            this.btnShift.UseVisualStyleBackColor = false;
            this.btnShift.Click += new System.EventHandler(this.btnShift_Click);
            // 
            // lblDriverInfo
            // 
            this.lblDriverInfo.AutoSize = false;
            this.lblDriverInfo.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblDriverInfo.Location = new System.Drawing.Point(16, 118);
            this.lblDriverInfo.Name = "lblDriverInfo";
            this.lblDriverInfo.Size = new System.Drawing.Size(368, 70);
            this.lblDriverInfo.TabIndex = 3;
            this.lblDriverInfo.Text = "Chọn tuyến được phân công rồi bấm \"Bắt đầu ca\". Xe của bạn sẽ chạy dọc tuyến và dừng ở từng trạm.";
            // 
            // pnlTabs  (3 nút chuyển tab)
            // 
            this.pnlTabs.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlTabs.Controls.Add(this.btnTabBus);
            this.pnlTabs.Controls.Add(this.btnTabShortest);
            this.pnlTabs.Controls.Add(this.btnTabLines);
            this.pnlTabs.Controls.Add(this.btnTabDriver);
            this.pnlTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTabs.Location = new System.Drawing.Point(0, 192);
            this.pnlTabs.Name = "pnlTabs";
            this.pnlTabs.Size = new System.Drawing.Size(400, 46);
            this.pnlTabs.TabIndex = 3;
            // 
            // btnTabBus
            // 
            this.btnTabBus.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnTabBus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabBus.FlatAppearance.BorderSize = 0;
            this.btnTabBus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabBus.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnTabBus.ForeColor = System.Drawing.Color.White;
            this.btnTabBus.Location = new System.Drawing.Point(16, 8);
            this.btnTabBus.Name = "btnTabBus";
            this.btnTabBus.Size = new System.Drawing.Size(92, 30);
            this.btnTabBus.TabIndex = 0;
            this.btnTabBus.Text = "Gợi ý xe buýt";
            this.btnTabBus.UseVisualStyleBackColor = false;
            this.btnTabBus.Click += new System.EventHandler(this.btnTabBus_Click);
            // 
            // btnTabShortest
            // 
            this.btnTabShortest.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnTabShortest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabShortest.FlatAppearance.BorderSize = 0;
            this.btnTabShortest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabShortest.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnTabShortest.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnTabShortest.Location = new System.Drawing.Point(112, 8);
            this.btnTabShortest.Name = "btnTabShortest";
            this.btnTabShortest.Size = new System.Drawing.Size(92, 30);
            this.btnTabShortest.TabIndex = 1;
            this.btnTabShortest.Text = "Ngắn nhất";
            this.btnTabShortest.UseVisualStyleBackColor = false;
            this.btnTabShortest.Click += new System.EventHandler(this.btnTabShortest_Click);
            // 
            // btnTabLines
            // 
            this.btnTabLines.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnTabLines.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabLines.FlatAppearance.BorderSize = 0;
            this.btnTabLines.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabLines.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnTabLines.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnTabLines.Location = new System.Drawing.Point(208, 8);
            this.btnTabLines.Name = "btnTabLines";
            this.btnTabLines.Size = new System.Drawing.Size(92, 30);
            this.btnTabLines.TabIndex = 2;
            this.btnTabLines.Text = "Các tuyến";
            this.btnTabLines.UseVisualStyleBackColor = false;
            this.btnTabLines.Click += new System.EventHandler(this.btnTabLines_Click);
            // 
            // btnTabDriver
            // 
            this.btnTabDriver.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnTabDriver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabDriver.FlatAppearance.BorderSize = 0;
            this.btnTabDriver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabDriver.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnTabDriver.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnTabDriver.Location = new System.Drawing.Point(304, 8);
            this.btnTabDriver.Name = "btnTabDriver";
            this.btnTabDriver.Size = new System.Drawing.Size(80, 30);
            this.btnTabDriver.TabIndex = 3;
            this.btnTabDriver.Text = "Tài xế";
            this.btnTabDriver.UseVisualStyleBackColor = false;
            this.btnTabDriver.Visible = false;
            this.btnTabDriver.Click += new System.EventHandler(this.btnTabDriver_Click);
            // 
            // pnlPlan  (chọn điểm đi / đến)
            // 
            this.pnlPlan.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlPlan.Controls.Add(this.lblPlanTitle);
            this.pnlPlan.Controls.Add(this.lblA);
            this.pnlPlan.Controls.Add(this.lblB);
            this.pnlPlan.Controls.Add(this.cboFrom);
            this.pnlPlan.Controls.Add(this.cboTo);
            this.pnlPlan.Controls.Add(this.btnSwap);
            this.pnlPlan.Controls.Add(this.btnLocate);
            this.pnlPlan.Controls.Add(this.cboFav);
            this.pnlPlan.Controls.Add(this.btnFavAdd);
            this.pnlPlan.Controls.Add(this.cboRides);
            this.pnlPlan.Controls.Add(this.btnRun);
            this.pnlPlan.Controls.Add(this.btnReset);
            this.pnlPlan.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPlan.Location = new System.Drawing.Point(0, 0);
            this.pnlPlan.Name = "pnlPlan";
            this.pnlPlan.Size = new System.Drawing.Size(400, 192);
            this.pnlPlan.TabIndex = 4;
            // 
            // lblPlanTitle
            // 
            this.lblPlanTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPlanTitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblPlanTitle.Location = new System.Drawing.Point(16, 12);
            this.lblPlanTitle.AutoSize = false;
            this.lblPlanTitle.Name = "lblPlanTitle";
            this.lblPlanTitle.Size = new System.Drawing.Size(120, 20);
            this.lblPlanTitle.TabIndex = 0;
            this.lblPlanTitle.Text = "LỘ TRÌNH";
            // 
            // lblA
            // 
            this.lblA.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblA.ForeColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.lblA.Location = new System.Drawing.Point(16, 40);
            this.lblA.AutoSize = false;
            this.lblA.Name = "lblA";
            this.lblA.Size = new System.Drawing.Size(22, 24);
            this.lblA.TabIndex = 1;
            this.lblA.Text = "A";
            // 
            // lblB
            // 
            this.lblB.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblB.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.lblB.Location = new System.Drawing.Point(16, 72);
            this.lblB.AutoSize = false;
            this.lblB.Name = "lblB";
            this.lblB.Size = new System.Drawing.Size(22, 24);
            this.lblB.TabIndex = 2;
            this.lblB.Text = "B";
            // 
            // cboFrom
            // 
            this.cboFrom.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboFrom.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboFrom.DropDownHeight = 320;
            this.cboFrom.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboFrom.FormattingEnabled = true;
            this.cboFrom.IntegralHeight = false;
            this.cboFrom.Location = new System.Drawing.Point(42, 38);
            this.cboFrom.Name = "cboFrom";
            this.cboFrom.Size = new System.Drawing.Size(292, 25);
            this.cboFrom.TabIndex = 3;
            this.cboFrom.SelectedIndexChanged += new System.EventHandler(this.cboFrom_SelectedIndexChanged);
            // 
            // cboTo
            // 
            this.cboTo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboTo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboTo.DropDownHeight = 320;
            this.cboTo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboTo.FormattingEnabled = true;
            this.cboTo.IntegralHeight = false;
            this.cboTo.Location = new System.Drawing.Point(42, 70);
            this.cboTo.Name = "cboTo";
            this.cboTo.Size = new System.Drawing.Size(292, 25);
            this.cboTo.TabIndex = 4;
            this.cboTo.SelectedIndexChanged += new System.EventHandler(this.cboTo_SelectedIndexChanged);
            // 
            // btnSwap
            // 
            this.btnSwap.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnSwap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSwap.FlatAppearance.BorderSize = 0;
            this.btnSwap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwap.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSwap.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnSwap.Location = new System.Drawing.Point(340, 38);
            this.btnSwap.Name = "btnSwap";
            this.btnSwap.Size = new System.Drawing.Size(44, 56);
            this.btnSwap.TabIndex = 5;
            this.btnSwap.Text = "⇅";
            this.btnSwap.UseVisualStyleBackColor = false;
            this.btnSwap.Click += new System.EventHandler(this.btnSwap_Click);
            // 
            // btnLocate
            // 
            this.btnLocate.BackColor = System.Drawing.Color.FromArgb(224, 231, 255);
            this.btnLocate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLocate.FlatAppearance.BorderSize = 0;
            this.btnLocate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocate.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnLocate.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnLocate.Location = new System.Drawing.Point(200, 6);
            this.btnLocate.Name = "btnLocate";
            this.btnLocate.Size = new System.Drawing.Size(184, 26);
            this.btnLocate.TabIndex = 10;
            this.btnLocate.Text = "📍 Vị trí của tôi";
            this.btnLocate.UseVisualStyleBackColor = false;
            this.btnLocate.Click += new System.EventHandler(this.btnLocate_Click);
            // 
            // cboFav
            // 
            this.cboFav.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFav.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboFav.FormattingEnabled = true;
            this.cboFav.Items.AddRange(new object[] {
            "★ Yêu thích..."});
            this.cboFav.Location = new System.Drawing.Point(190, 106);
            this.cboFav.Name = "cboFav";
            this.cboFav.SelectedIndex = 0;
            this.cboFav.Size = new System.Drawing.Size(140, 25);
            this.cboFav.TabIndex = 11;
            this.cboFav.Visible = false;
            this.cboFav.SelectionChangeCommitted += new System.EventHandler(this.cboFav_SelectionChangeCommitted);
            // 
            // btnFavAdd
            // 
            this.btnFavAdd.BackColor = System.Drawing.Color.FromArgb(254, 243, 199);
            this.btnFavAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFavAdd.FlatAppearance.BorderSize = 0;
            this.btnFavAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFavAdd.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnFavAdd.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.btnFavAdd.Location = new System.Drawing.Point(336, 105);
            this.btnFavAdd.Name = "btnFavAdd";
            this.btnFavAdd.Size = new System.Drawing.Size(48, 27);
            this.btnFavAdd.TabIndex = 12;
            this.btnFavAdd.Text = "☆ Lưu";
            this.btnFavAdd.UseVisualStyleBackColor = false;
            this.btnFavAdd.Visible = false;
            this.btnFavAdd.Click += new System.EventHandler(this.btnFavAdd_Click);
            // 
            // cboRides
            // 
            this.cboRides.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRides.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboRides.FormattingEnabled = true;
            this.cboRides.Items.AddRange(new object[] {
            "Tối đa 1 chuyến",
            "Tối đa 2 chuyến",
            "Tối đa 3 chuyến"});
            this.cboRides.Location = new System.Drawing.Point(42, 106);
            this.cboRides.Name = "cboRides";
            this.cboRides.SelectedIndex = 1;
            this.cboRides.Size = new System.Drawing.Size(140, 25);
            this.cboRides.TabIndex = 7;
            this.cboRides.SelectedIndexChanged += new System.EventHandler(this.cboRides_SelectedIndexChanged);
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRun.FlatAppearance.BorderSize = 0;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRun.ForeColor = System.Drawing.Color.White;
            this.btnRun.Location = new System.Drawing.Point(16, 144);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(240, 36);
            this.btnRun.TabIndex = 8;
            this.btnRun.Text = "Tìm đường";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnReset.Location = new System.Drawing.Point(264, 144);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 36);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "Xóa";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // pnlMap  (khung chứa bản đồ + 2 nút zoom nổi lên trên)
            // 
            this.pnlMap.Controls.Add(this.btnZoomIn);
            this.pnlMap.Controls.Add(this.btnZoomOut);
            this.pnlMap.Controls.Add(this.mapCanvas);
            this.pnlMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMap.Location = new System.Drawing.Point(0, 84);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(940, 666);
            this.pnlMap.TabIndex = 0;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomIn.BackColor = System.Drawing.Color.White;
            this.btnZoomIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZoomIn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnZoomIn.FlatAppearance.BorderSize = 1;
            this.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomIn.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnZoomIn.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnZoomIn.Location = new System.Drawing.Point(892, 578);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(34, 34);
            this.btnZoomIn.TabIndex = 0;
            this.btnZoomIn.TabStop = false;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = false;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomOut.BackColor = System.Drawing.Color.White;
            this.btnZoomOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZoomOut.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnZoomOut.FlatAppearance.BorderSize = 1;
            this.btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomOut.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnZoomOut.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnZoomOut.Location = new System.Drawing.Point(892, 618);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(34, 34);
            this.btnZoomOut.TabIndex = 1;
            this.btnZoomOut.TabStop = false;
            this.btnZoomOut.Text = "−";
            this.btnZoomOut.UseVisualStyleBackColor = false;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // mapCanvas
            // 
            this.mapCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapCanvas.Location = new System.Drawing.Point(0, 0);
            this.mapCanvas.Name = "mapCanvas";
            this.mapCanvas.Size = new System.Drawing.Size(940, 666);
            this.mapCanvas.TabIndex = 2;
            this.mapCanvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MapCanvas_MouseClick);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1340, 780);
            this.Controls.Add(this.pnlMap);
            this.Controls.Add(this.pnlSide);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1300, 700);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestMap - Gợi ý lộ trình xe buýt";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.pnlTop.ResumeLayout(false);
            this.flowTop.ResumeLayout(false);
            this.flowTop.PerformLayout();
            this.pnlSide.ResumeLayout(false);
            this.pnlBus.ResumeLayout(false);
            this.pnlSteps.ResumeLayout(false);
            this.pnlItinFilter.ResumeLayout(false);
            this.pnlStepsHeader.ResumeLayout(false);
            this.pnlShortest.ResumeLayout(false);
            this.pnlListTitle.ResumeLayout(false);
            this.pnlStats.ResumeLayout(false);
            this.pnlLines.ResumeLayout(false);
            this.pnlDriver.ResumeLayout(false);
            this.pnlDriverTop.ResumeLayout(false);
            this.pnlTabs.ResumeLayout(false);
            this.pnlPlan.ResumeLayout(false);
            this.pnlMap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // ----- Điều khiển (khai báo ở đây, kéo thả trong Form1.cs [Design]) -----
        private System.Windows.Forms.Timer timer;       // hoạt ảnh thuật toán và hoạt ảnh vẽ tuyến
        private System.Windows.Forms.Timer busTimer;    // xe chạy thử dọc đường ngắn nhất
        private System.Windows.Forms.Timer liveTimer;   // xe buýt đang chạy (mô phỏng) trên các tuyến
        private System.Windows.Forms.Timer driverTimer; // xe của tài xế đang chạy ca

        private System.Windows.Forms.Panel pnlMap;
        private TestMap.Mapping.MapCanvas mapCanvas;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.FlowLayoutPanel flowTop;
        private System.Windows.Forms.ComboBox cboArea;
        private System.Windows.Forms.ComboBox cboAlgo;
        private System.Windows.Forms.CheckBox chkStops;
        private System.Windows.Forms.CheckBox chkLines;
        private System.Windows.Forms.CheckBox chkBuses;
        private System.Windows.Forms.CheckBox chkPoi;
        private System.Windows.Forms.Button btnLoadPoi;
        private System.Windows.Forms.Button btnAccount;
        private System.Windows.Forms.Button btnPoiFilter;
        private System.Windows.Forms.Button btnSim;
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.CheckBox chkSearch;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnLoadLines;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnAdmin;

        private System.Windows.Forms.Label lblInfo;

        private System.Windows.Forms.Panel pnlSide;
        private System.Windows.Forms.Panel pnlPlan;
        private System.Windows.Forms.Label lblPlanTitle;
        private System.Windows.Forms.Label lblA;
        private System.Windows.Forms.Label lblB;
        private System.Windows.Forms.ComboBox cboFrom;
        private System.Windows.Forms.ComboBox cboTo;
        private System.Windows.Forms.Button btnSwap;
        private System.Windows.Forms.ComboBox cboRides;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnLocate;
        private System.Windows.Forms.ComboBox cboFav;
        private System.Windows.Forms.Button btnFavAdd;

        private System.Windows.Forms.Panel pnlTabs;
        private System.Windows.Forms.Button btnTabBus;
        private System.Windows.Forms.Button btnTabShortest;
        private System.Windows.Forms.Button btnTabLines;
        private System.Windows.Forms.Button btnTabDriver;

        private System.Windows.Forms.Panel pnlDriver;
        private System.Windows.Forms.ListBox lstDriverStops;
        private System.Windows.Forms.Panel pnlDriverTop;
        private System.Windows.Forms.Label lblDriverTitle;
        private System.Windows.Forms.ComboBox cboDriverLine;
        private System.Windows.Forms.Button btnShift;
        private System.Windows.Forms.Label lblDriverInfo;

        private System.Windows.Forms.Panel pnlBus;
        private System.Windows.Forms.ListBox lstItins;
        private System.Windows.Forms.Panel pnlSteps;
        private System.Windows.Forms.ListBox lstSteps;
        private System.Windows.Forms.Panel pnlStepsHeader;
        private System.Windows.Forms.Label lblStepsTitle;
        private System.Windows.Forms.LinkLabel lnkCopyBus;
        private System.Windows.Forms.Label lblItinMsg;
        private System.Windows.Forms.Panel pnlItinFilter;
        private System.Windows.Forms.Label lblSort;
        private System.Windows.Forms.ComboBox cboItinSort;
        private System.Windows.Forms.Label lblItinCount;
        private System.Windows.Forms.Label lblWalkLimit;
        private System.Windows.Forms.ComboBox cboWalkLimit;
        private System.Windows.Forms.CheckBox chkWalkSuggest;
        private System.Windows.Forms.Label lblDemoBus;

        private System.Windows.Forms.Panel pnlShortest;
        private System.Windows.Forms.ListBox lstStops;
        private System.Windows.Forms.Panel pnlListTitle;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.LinkLabel lnkCopy;
        private System.Windows.Forms.Panel pnlStats;
        private System.Windows.Forms.Label lblDist;
        private System.Windows.Forms.Label lblDistCap;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblTimeCap;
        private System.Windows.Forms.Label lblStopCount;
        private System.Windows.Forms.Label lblStopCountCap;
        private System.Windows.Forms.Label lblVisited;
        private System.Windows.Forms.Button btnBus;

        private System.Windows.Forms.Panel pnlLines;
        private System.Windows.Forms.ListBox lstLines;
        private System.Windows.Forms.Label lblDemoLines;
    }
}
