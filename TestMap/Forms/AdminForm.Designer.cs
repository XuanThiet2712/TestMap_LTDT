namespace TestMap.Forms
{
    partial class AdminForm
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
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle headerStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblArea = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabStops = new System.Windows.Forms.TabPage();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearchStop = new System.Windows.Forms.TextBox();
            this.dgvStops = new System.Windows.Forms.DataGridView();
            this.colStopId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStopName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStopNode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStopLines = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnStopAdd = new System.Windows.Forms.Button();
            this.btnStopEdit = new System.Windows.Forms.Button();
            this.btnStopMove = new System.Windows.Forms.Button();
            this.btnStopDelete = new System.Windows.Forms.Button();
            this.tabLines = new System.Windows.Forms.TabPage();
            this.dgvLines = new System.Windows.Forms.DataGridView();
            this.colLineRef = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineStops = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineFare = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineHeadway = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineHours = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnLineAdd = new System.Windows.Forms.Button();
            this.btnLineEdit = new System.Windows.Forms.Button();
            this.btnLineReverse = new System.Windows.Forms.Button();
            this.btnLineDelete = new System.Windows.Forms.Button();
            this.lblLineHint = new System.Windows.Forms.Label();
            this.tabData = new System.Windows.Forms.TabPage();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.lblDataInfo = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.grpAccount = new System.Windows.Forms.GroupBox();
            this.lblOld = new System.Windows.Forms.Label();
            this.txtOldPass = new System.Windows.Forms.TextBox();
            this.lblNew = new System.Windows.Forms.Label();
            this.txtNewPass = new System.Windows.Forms.TextBox();
            this.lblNew2 = new System.Windows.Forms.Label();
            this.txtNewPass2 = new System.Windows.Forms.TextBox();
            this.btnChangePass = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabStops.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStops)).BeginInit();
            this.tabLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).BeginInit();
            this.tabData.SuspendLayout();
            this.grpData.SuspendLayout();
            this.grpAccount.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlHeader.Controls.Add(this.lblArea);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(940, 60);
            this.pnlHeader.TabIndex = 0;
            //
            // lblArea
            //
            this.lblArea.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblArea.Location = new System.Drawing.Point(20, 34);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(700, 20);
            this.lblArea.TabIndex = 1;
            this.lblArea.Text = "Khu vực";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(240, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản lý dữ liệu xe buýt";
            //
            // lblStatus
            //
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblStatus.Location = new System.Drawing.Point(0, 594);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(940, 28);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Sẵn sàng.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tabs
            //
            this.tabs.Controls.Add(this.tabStops);
            this.tabs.Controls.Add(this.tabLines);
            this.tabs.Controls.Add(this.tabData);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 60);
            this.tabs.Name = "tabs";
            this.tabs.Padding = new System.Drawing.Point(16, 6);
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(940, 534);
            this.tabs.TabIndex = 1;
            //
            // tabStops
            //
            this.tabStops.Controls.Add(this.btnStopDelete);
            this.tabStops.Controls.Add(this.btnStopMove);
            this.tabStops.Controls.Add(this.btnStopEdit);
            this.tabStops.Controls.Add(this.btnStopAdd);
            this.tabStops.Controls.Add(this.dgvStops);
            this.tabStops.Controls.Add(this.txtSearchStop);
            this.tabStops.Controls.Add(this.lblSearch);
            this.tabStops.Location = new System.Drawing.Point(4, 32);
            this.tabStops.Name = "tabStops";
            this.tabStops.Padding = new System.Windows.Forms.Padding(12);
            this.tabStops.Size = new System.Drawing.Size(932, 498);
            this.tabStops.TabIndex = 0;
            this.tabStops.Text = "Trạm xe buýt";
            this.tabStops.UseVisualStyleBackColor = true;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(16, 20);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(56, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm trạm:";
            //
            // txtSearchStop
            //
            this.txtSearchStop.Location = new System.Drawing.Point(80, 16);
            this.txtSearchStop.Name = "txtSearchStop";
            this.txtSearchStop.Size = new System.Drawing.Size(300, 23);
            this.txtSearchStop.TabIndex = 1;
            this.txtSearchStop.TextChanged += new System.EventHandler(this.txtSearchStop_TextChanged);
            //
            // dgvStops
            //
            this.dgvStops.AllowUserToAddRows = false;
            this.dgvStops.AllowUserToDeleteRows = false;
            this.dgvStops.AllowUserToResizeRows = false;
            this.dgvStops.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStops.BackgroundColor = System.Drawing.Color.White;
            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            headerStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            headerStyle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            headerStyle.SelectionBackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            headerStyle.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.dgvStops.ColumnHeadersDefaultCellStyle = headerStyle;
            this.dgvStops.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStops.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStopId, this.colStopName, this.colStopNode, this.colStopLines});
            this.dgvStops.EnableHeadersVisualStyles = false;
            this.dgvStops.Location = new System.Drawing.Point(16, 50);
            this.dgvStops.MultiSelect = false;
            this.dgvStops.Name = "dgvStops";
            this.dgvStops.ReadOnly = true;
            this.dgvStops.RowHeadersVisible = false;
            this.dgvStops.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStops.Size = new System.Drawing.Size(768, 430);
            this.dgvStops.TabIndex = 2;
            this.dgvStops.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStops_CellDoubleClick);
            //
            // colStopId
            //
            this.colStopId.FillWeight = 14F;
            this.colStopId.HeaderText = "Mã trạm";
            this.colStopId.Name = "colStopId";
            this.colStopId.ReadOnly = true;
            //
            // colStopName
            //
            this.colStopName.FillWeight = 56F;
            this.colStopName.HeaderText = "Tên trạm";
            this.colStopName.Name = "colStopName";
            this.colStopName.ReadOnly = true;
            //
            // colStopNode
            //
            this.colStopNode.FillWeight = 14F;
            this.colStopNode.HeaderText = "Giao lộ";
            this.colStopNode.Name = "colStopNode";
            this.colStopNode.ReadOnly = true;
            //
            // colStopLines
            //
            this.colStopLines.FillWeight = 16F;
            this.colStopLines.HeaderText = "Số tuyến qua";
            this.colStopLines.Name = "colStopLines";
            this.colStopLines.ReadOnly = true;
            //
            // btnStopAdd
            //
            this.btnStopAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopAdd.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnStopAdd.FlatAppearance.BorderSize = 0;
            this.btnStopAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopAdd.ForeColor = System.Drawing.Color.White;
            this.btnStopAdd.Location = new System.Drawing.Point(796, 50);
            this.btnStopAdd.Name = "btnStopAdd";
            this.btnStopAdd.Size = new System.Drawing.Size(118, 36);
            this.btnStopAdd.TabIndex = 3;
            this.btnStopAdd.Text = "Thêm trạm  +";
            this.btnStopAdd.UseVisualStyleBackColor = false;
            this.btnStopAdd.Click += new System.EventHandler(this.btnStopAdd_Click);
            //
            // btnStopEdit
            //
            this.btnStopEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopEdit.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnStopEdit.FlatAppearance.BorderSize = 0;
            this.btnStopEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopEdit.ForeColor = System.Drawing.Color.White;
            this.btnStopEdit.Location = new System.Drawing.Point(796, 94);
            this.btnStopEdit.Name = "btnStopEdit";
            this.btnStopEdit.Size = new System.Drawing.Size(118, 36);
            this.btnStopEdit.TabIndex = 4;
            this.btnStopEdit.Text = "Sửa tên";
            this.btnStopEdit.UseVisualStyleBackColor = false;
            this.btnStopEdit.Click += new System.EventHandler(this.btnStopEdit_Click);
            //
            // btnStopMove
            //
            this.btnStopMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopMove.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnStopMove.FlatAppearance.BorderSize = 0;
            this.btnStopMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopMove.ForeColor = System.Drawing.Color.White;
            this.btnStopMove.Location = new System.Drawing.Point(796, 138);
            this.btnStopMove.Name = "btnStopMove";
            this.btnStopMove.Size = new System.Drawing.Size(118, 36);
            this.btnStopMove.TabIndex = 5;
            this.btnStopMove.Text = "Dời vị trí";
            this.btnStopMove.UseVisualStyleBackColor = false;
            this.btnStopMove.Click += new System.EventHandler(this.btnStopMove_Click);
            //
            // btnStopDelete
            //
            this.btnStopDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopDelete.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.btnStopDelete.FlatAppearance.BorderSize = 0;
            this.btnStopDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopDelete.ForeColor = System.Drawing.Color.White;
            this.btnStopDelete.Location = new System.Drawing.Point(796, 182);
            this.btnStopDelete.Name = "btnStopDelete";
            this.btnStopDelete.Size = new System.Drawing.Size(118, 36);
            this.btnStopDelete.TabIndex = 6;
            this.btnStopDelete.Text = "Xóa";
            this.btnStopDelete.UseVisualStyleBackColor = false;
            this.btnStopDelete.Click += new System.EventHandler(this.btnStopDelete_Click);
            //
            // tabLines
            //
            this.tabLines.Controls.Add(this.lblLineHint);
            this.tabLines.Controls.Add(this.btnLineDelete);
            this.tabLines.Controls.Add(this.btnLineReverse);
            this.tabLines.Controls.Add(this.btnLineEdit);
            this.tabLines.Controls.Add(this.btnLineAdd);
            this.tabLines.Controls.Add(this.dgvLines);
            this.tabLines.Location = new System.Drawing.Point(4, 32);
            this.tabLines.Name = "tabLines";
            this.tabLines.Padding = new System.Windows.Forms.Padding(12);
            this.tabLines.Size = new System.Drawing.Size(932, 498);
            this.tabLines.TabIndex = 1;
            this.tabLines.Text = "Tuyến xe";
            this.tabLines.UseVisualStyleBackColor = true;
            //
            // dgvLines
            //
            this.dgvLines.AllowUserToAddRows = false;
            this.dgvLines.AllowUserToDeleteRows = false;
            this.dgvLines.AllowUserToResizeRows = false;
            this.dgvLines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLines.BackgroundColor = System.Drawing.Color.White;
            headerStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle2.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            headerStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            headerStyle2.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            headerStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            headerStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.dgvLines.ColumnHeadersDefaultCellStyle = headerStyle2;
            this.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLineRef, this.colLineName, this.colLineStops, this.colLineFare, this.colLineHeadway, this.colLineHours});
            this.dgvLines.EnableHeadersVisualStyles = false;
            this.dgvLines.Location = new System.Drawing.Point(16, 16);
            this.dgvLines.MultiSelect = false;
            this.dgvLines.Name = "dgvLines";
            this.dgvLines.ReadOnly = true;
            this.dgvLines.RowHeadersVisible = false;
            this.dgvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLines.Size = new System.Drawing.Size(768, 434);
            this.dgvLines.TabIndex = 0;
            this.dgvLines.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLines_CellDoubleClick);
            this.dgvLines.SelectionChanged += new System.EventHandler(this.dgvLines_SelectionChanged);
            //
            // colLineRef
            //
            this.colLineRef.FillWeight = 10F;
            this.colLineRef.HeaderText = "Số tuyến";
            this.colLineRef.Name = "colLineRef";
            this.colLineRef.ReadOnly = true;
            //
            // colLineName
            //
            this.colLineName.FillWeight = 42F;
            this.colLineName.HeaderText = "Chiều đi (trạm đầu → trạm cuối)";
            this.colLineName.Name = "colLineName";
            this.colLineName.ReadOnly = true;
            //
            // colLineStops
            //
            this.colLineStops.FillWeight = 10F;
            this.colLineStops.HeaderText = "Số trạm";
            this.colLineStops.Name = "colLineStops";
            this.colLineStops.ReadOnly = true;
            //
            // colLineFare
            //
            this.colLineFare.FillWeight = 12F;
            this.colLineFare.HeaderText = "Giá vé";
            this.colLineFare.Name = "colLineFare";
            this.colLineFare.ReadOnly = true;
            //
            // colLineHeadway
            //
            this.colLineHeadway.FillWeight = 12F;
            this.colLineHeadway.HeaderText = "Phút/chuyến";
            this.colLineHeadway.Name = "colLineHeadway";
            this.colLineHeadway.ReadOnly = true;
            //
            // colLineHours
            //
            this.colLineHours.FillWeight = 14F;
            this.colLineHours.HeaderText = "Giờ hoạt động";
            this.colLineHours.Name = "colLineHours";
            this.colLineHours.ReadOnly = true;
            //
            // btnLineAdd
            //
            this.btnLineAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLineAdd.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnLineAdd.FlatAppearance.BorderSize = 0;
            this.btnLineAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLineAdd.ForeColor = System.Drawing.Color.White;
            this.btnLineAdd.Location = new System.Drawing.Point(796, 16);
            this.btnLineAdd.Name = "btnLineAdd";
            this.btnLineAdd.Size = new System.Drawing.Size(118, 36);
            this.btnLineAdd.TabIndex = 1;
            this.btnLineAdd.Text = "Thêm tuyến  +";
            this.btnLineAdd.UseVisualStyleBackColor = false;
            this.btnLineAdd.Click += new System.EventHandler(this.btnLineAdd_Click);
            //
            // btnLineEdit
            //
            this.btnLineEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLineEdit.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnLineEdit.FlatAppearance.BorderSize = 0;
            this.btnLineEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLineEdit.ForeColor = System.Drawing.Color.White;
            this.btnLineEdit.Location = new System.Drawing.Point(796, 60);
            this.btnLineEdit.Name = "btnLineEdit";
            this.btnLineEdit.Size = new System.Drawing.Size(118, 36);
            this.btnLineEdit.TabIndex = 2;
            this.btnLineEdit.Text = "Sửa";
            this.btnLineEdit.UseVisualStyleBackColor = false;
            this.btnLineEdit.Click += new System.EventHandler(this.btnLineEdit_Click);
            //
            // btnLineReverse
            //
            this.btnLineReverse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLineReverse.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnLineReverse.FlatAppearance.BorderSize = 0;
            this.btnLineReverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLineReverse.ForeColor = System.Drawing.Color.White;
            this.btnLineReverse.Location = new System.Drawing.Point(796, 104);
            this.btnLineReverse.Name = "btnLineReverse";
            this.btnLineReverse.Size = new System.Drawing.Size(118, 36);
            this.btnLineReverse.TabIndex = 3;
            this.btnLineReverse.Text = "Tạo chiều về";
            this.btnLineReverse.UseVisualStyleBackColor = false;
            this.btnLineReverse.Click += new System.EventHandler(this.btnLineReverse_Click);
            //
            // btnLineDelete
            //
            this.btnLineDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLineDelete.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.btnLineDelete.FlatAppearance.BorderSize = 0;
            this.btnLineDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLineDelete.ForeColor = System.Drawing.Color.White;
            this.btnLineDelete.Location = new System.Drawing.Point(796, 148);
            this.btnLineDelete.Name = "btnLineDelete";
            this.btnLineDelete.Size = new System.Drawing.Size(118, 36);
            this.btnLineDelete.TabIndex = 4;
            this.btnLineDelete.Text = "Xóa";
            this.btnLineDelete.UseVisualStyleBackColor = false;
            this.btnLineDelete.Click += new System.EventHandler(this.btnLineDelete_Click);
            //
            // lblLineHint
            //
            this.lblLineHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLineHint.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblLineHint.Location = new System.Drawing.Point(16, 456);
            this.lblLineHint.Name = "lblLineHint";
            this.lblLineHint.Size = new System.Drawing.Size(768, 34);
            this.lblLineHint.TabIndex = 5;
            this.lblLineHint.Text = "Chọn một dòng để xem tuyến đó trên bản đồ chính. Bấm đúp hoặc nút Sửa để đổi giá vé, tần suất, màu và danh sách trạm. Mỗi chiều đi / về là một dòng riêng.";
            //
            // tabData
            //
            this.tabData.Controls.Add(this.grpAccount);
            this.tabData.Controls.Add(this.grpData);
            this.tabData.Location = new System.Drawing.Point(4, 32);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(12);
            this.tabData.Size = new System.Drawing.Size(932, 498);
            this.tabData.TabIndex = 2;
            this.tabData.Text = "Cập nhật dữ liệu";
            this.tabData.UseVisualStyleBackColor = true;
            //
            // grpData
            //
            this.grpData.Controls.Add(this.btnDownload);
            this.grpData.Controls.Add(this.btnRestore);
            this.grpData.Controls.Add(this.btnSave);
            this.grpData.Controls.Add(this.lblDataInfo);
            this.grpData.Location = new System.Drawing.Point(16, 16);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(560, 250);
            this.grpData.TabIndex = 0;
            this.grpData.TabStop = false;
            this.grpData.Text = "Dữ liệu trạm và tuyến";
            //
            // lblDataInfo
            //
            this.lblDataInfo.Location = new System.Drawing.Point(16, 26);
            this.lblDataInfo.Name = "lblDataInfo";
            this.lblDataInfo.Size = new System.Drawing.Size(528, 78);
            this.lblDataInfo.TabIndex = 0;
            this.lblDataInfo.Text = "";
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(18, 112);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(250, 40);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Lưu và áp dụng thay đổi";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnRestore
            //
            this.btnRestore.Location = new System.Drawing.Point(18, 162);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(250, 34);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "Khôi phục tuyến gốc (bỏ chỉnh sửa)";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            //
            // btnDownload
            //
            this.btnDownload.Location = new System.Drawing.Point(18, 202);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(250, 34);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "Tải tuyến thật từ OpenStreetMap";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            //
            // grpAccount
            //
            this.grpAccount.Controls.Add(this.btnLogout);
            this.grpAccount.Controls.Add(this.btnChangePass);
            this.grpAccount.Controls.Add(this.txtNewPass2);
            this.grpAccount.Controls.Add(this.lblNew2);
            this.grpAccount.Controls.Add(this.txtNewPass);
            this.grpAccount.Controls.Add(this.lblNew);
            this.grpAccount.Controls.Add(this.txtOldPass);
            this.grpAccount.Controls.Add(this.lblOld);
            this.grpAccount.Location = new System.Drawing.Point(16, 278);
            this.grpAccount.Name = "grpAccount";
            this.grpAccount.Size = new System.Drawing.Size(560, 200);
            this.grpAccount.TabIndex = 1;
            this.grpAccount.TabStop = false;
            this.grpAccount.Text = "Tài khoản quản trị";
            //
            // lblOld
            //
            this.lblOld.AutoSize = true;
            this.lblOld.Location = new System.Drawing.Point(16, 30);
            this.lblOld.Name = "lblOld";
            this.lblOld.Size = new System.Drawing.Size(80, 15);
            this.lblOld.TabIndex = 0;
            this.lblOld.Text = "Mật khẩu cũ";
            //
            // txtOldPass
            //
            this.txtOldPass.Location = new System.Drawing.Point(160, 26);
            this.txtOldPass.Name = "txtOldPass";
            this.txtOldPass.Size = new System.Drawing.Size(240, 23);
            this.txtOldPass.TabIndex = 1;
            this.txtOldPass.UseSystemPasswordChar = true;
            //
            // lblNew
            //
            this.lblNew.AutoSize = true;
            this.lblNew.Location = new System.Drawing.Point(16, 62);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(90, 15);
            this.lblNew.TabIndex = 2;
            this.lblNew.Text = "Mật khẩu mới";
            //
            // txtNewPass
            //
            this.txtNewPass.Location = new System.Drawing.Point(160, 58);
            this.txtNewPass.Name = "txtNewPass";
            this.txtNewPass.Size = new System.Drawing.Size(240, 23);
            this.txtNewPass.TabIndex = 3;
            this.txtNewPass.UseSystemPasswordChar = true;
            //
            // lblNew2
            //
            this.lblNew2.AutoSize = true;
            this.lblNew2.Location = new System.Drawing.Point(16, 94);
            this.lblNew2.Name = "lblNew2";
            this.lblNew2.Size = new System.Drawing.Size(120, 15);
            this.lblNew2.TabIndex = 4;
            this.lblNew2.Text = "Nhập lại mật khẩu mới";
            //
            // txtNewPass2
            //
            this.txtNewPass2.Location = new System.Drawing.Point(160, 90);
            this.txtNewPass2.Name = "txtNewPass2";
            this.txtNewPass2.Size = new System.Drawing.Size(240, 23);
            this.txtNewPass2.TabIndex = 5;
            this.txtNewPass2.UseSystemPasswordChar = true;
            //
            // btnChangePass
            //
            this.btnChangePass.Location = new System.Drawing.Point(160, 126);
            this.btnChangePass.Name = "btnChangePass";
            this.btnChangePass.Size = new System.Drawing.Size(150, 34);
            this.btnChangePass.TabIndex = 6;
            this.btnChangePass.Text = "Đổi mật khẩu";
            this.btnChangePass.UseVisualStyleBackColor = true;
            this.btnChangePass.Click += new System.EventHandler(this.btnChangePass_Click);
            //
            // btnLogout
            //
            this.btnLogout.Location = new System.Drawing.Point(394, 150);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(150, 34);
            this.btnLogout.TabIndex = 7;
            this.btnLogout.Text = "Đăng xuất quản trị";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            //
            // AdminForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 622);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(800, 560);
            this.Name = "AdminForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AdminForm - Quản lý dữ liệu xe buýt";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tabStops.ResumeLayout(false);
            this.tabStops.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStops)).EndInit();
            this.tabLines.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).EndInit();
            this.tabData.ResumeLayout(false);
            this.grpData.ResumeLayout(false);
            this.grpAccount.ResumeLayout(false);
            this.grpAccount.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabStops;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchStop;
        private System.Windows.Forms.DataGridView dgvStops;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStopId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStopName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStopNode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStopLines;
        private System.Windows.Forms.Button btnStopAdd;
        private System.Windows.Forms.Button btnStopEdit;
        private System.Windows.Forms.Button btnStopMove;
        private System.Windows.Forms.Button btnStopDelete;
        private System.Windows.Forms.TabPage tabLines;
        private System.Windows.Forms.DataGridView dgvLines;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineRef;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineStops;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineFare;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineHeadway;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineHours;
        private System.Windows.Forms.Button btnLineAdd;
        private System.Windows.Forms.Button btnLineEdit;
        private System.Windows.Forms.Button btnLineReverse;
        private System.Windows.Forms.Button btnLineDelete;
        private System.Windows.Forms.Label lblLineHint;
        private System.Windows.Forms.TabPage tabData;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.Label lblDataInfo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.GroupBox grpAccount;
        private System.Windows.Forms.Label lblOld;
        private System.Windows.Forms.TextBox txtOldPass;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.TextBox txtNewPass;
        private System.Windows.Forms.Label lblNew2;
        private System.Windows.Forms.TextBox txtNewPass2;
        private System.Windows.Forms.Button btnChangePass;
        private System.Windows.Forms.Button btnLogout;
    }
}
