namespace TestMap.Forms
{
    partial class PoiForm
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
            this.pnlTopP = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblKinds = new System.Windows.Forms.Label();
            this.flowKinds = new System.Windows.Forms.FlowLayoutPanel();
            this.chkHospital = new System.Windows.Forms.CheckBox();
            this.chkSchool = new System.Windows.Forms.CheckBox();
            this.chkUniversity = new System.Windows.Forms.CheckBox();
            this.chkMarket = new System.Windows.Forms.CheckBox();
            this.chkPark = new System.Windows.Forms.CheckBox();
            this.chkLandmark = new System.Windows.Forms.CheckBox();
            this.btnKindAll = new System.Windows.Forms.Button();
            this.pnlBottomP = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnPoiView = new System.Windows.Forms.Button();
            this.btnPoiA = new System.Windows.Forms.Button();
            this.btnPoiB = new System.Windows.Forms.Button();
            this.lblPoiHint = new System.Windows.Forms.Label();
            this.lstPois = new System.Windows.Forms.ListBox();
            this.pnlTopP.SuspendLayout();
            this.flowKinds.SuspendLayout();
            this.pnlBottomP.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTopP
            // 
            this.pnlTopP.Controls.Add(this.lblSearch);
            this.pnlTopP.Controls.Add(this.txtSearch);
            this.pnlTopP.Controls.Add(this.lblKinds);
            this.pnlTopP.Controls.Add(this.flowKinds);
            this.pnlTopP.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlTopP.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopP.Name = "pnlTopP";
            this.pnlTopP.Size = new System.Drawing.Size(380, 150);
            this.pnlTopP.TabIndex = 2;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = false;
            this.lblSearch.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblSearch.Location = new System.Drawing.Point(12, 10);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(200, 18);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm theo tên";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(12, 30);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(356, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblKinds
            // 
            this.lblKinds.AutoSize = false;
            this.lblKinds.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblKinds.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKinds.Location = new System.Drawing.Point(12, 62);
            this.lblKinds.Name = "lblKinds";
            this.lblKinds.Size = new System.Drawing.Size(200, 18);
            this.lblKinds.TabIndex = 2;
            this.lblKinds.Text = "Lọc theo loại (cũng lọc trên bản đồ)";
            // 
            // flowKinds
            // 
            this.flowKinds.Controls.Add(this.chkHospital);
            this.flowKinds.Controls.Add(this.chkSchool);
            this.flowKinds.Controls.Add(this.chkUniversity);
            this.flowKinds.Controls.Add(this.chkMarket);
            this.flowKinds.Controls.Add(this.chkPark);
            this.flowKinds.Controls.Add(this.chkLandmark);
            this.flowKinds.Controls.Add(this.btnKindAll);
            this.flowKinds.Location = new System.Drawing.Point(12, 84);
            this.flowKinds.Name = "flowKinds";
            this.flowKinds.Size = new System.Drawing.Size(356, 58);
            this.flowKinds.TabIndex = 3;
            // 
            // chkHospital
            // 
            this.chkHospital.Name = "chkHospital";
            this.chkHospital.Size = new System.Drawing.Size(100, 22);
            this.chkHospital.TabIndex = 0;
            this.chkHospital.Text = "Bệnh viện";
            this.chkHospital.AutoSize = true;
            this.chkHospital.Checked = true;
            this.chkHospital.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHospital.Margin = new System.Windows.Forms.Padding(0, 0, 8, 2);
            this.chkHospital.CheckedChanged += new System.EventHandler(this.chkKind_CheckedChanged);
            // 
            // chkSchool
            // 
            this.chkSchool.Name = "chkSchool";
            this.chkSchool.Size = new System.Drawing.Size(100, 22);
            this.chkSchool.TabIndex = 1;
            this.chkSchool.Text = "Trường học";
            this.chkSchool.AutoSize = true;
            this.chkSchool.Checked = true;
            this.chkSchool.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSchool.Margin = new System.Windows.Forms.Padding(0, 0, 8, 2);
            this.chkSchool.CheckedChanged += new System.EventHandler(this.chkKind_CheckedChanged);
            // 
            // chkUniversity
            // 
            this.chkUniversity.Name = "chkUniversity";
            this.chkUniversity.Size = new System.Drawing.Size(100, 22);
            this.chkUniversity.TabIndex = 2;
            this.chkUniversity.Text = "Đại học";
            this.chkUniversity.AutoSize = true;
            this.chkUniversity.Checked = true;
            this.chkUniversity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUniversity.Margin = new System.Windows.Forms.Padding(0, 0, 8, 2);
            this.chkUniversity.CheckedChanged += new System.EventHandler(this.chkKind_CheckedChanged);
            // 
            // chkMarket
            // 
            this.chkMarket.Name = "chkMarket";
            this.chkMarket.Size = new System.Drawing.Size(100, 22);
            this.chkMarket.TabIndex = 3;
            this.chkMarket.Text = "Chợ / TTTM";
            this.chkMarket.AutoSize = true;
            this.chkMarket.Checked = true;
            this.chkMarket.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMarket.Margin = new System.Windows.Forms.Padding(0, 0, 8, 2);
            this.chkMarket.CheckedChanged += new System.EventHandler(this.chkKind_CheckedChanged);
            // 
            // chkPark
            // 
            this.chkPark.Name = "chkPark";
            this.chkPark.Size = new System.Drawing.Size(100, 22);
            this.chkPark.TabIndex = 4;
            this.chkPark.Text = "Công viên";
            this.chkPark.AutoSize = true;
            this.chkPark.Checked = true;
            this.chkPark.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPark.Margin = new System.Windows.Forms.Padding(0, 0, 8, 2);
            this.chkPark.CheckedChanged += new System.EventHandler(this.chkKind_CheckedChanged);
            // 
            // chkLandmark
            // 
            this.chkLandmark.Name = "chkLandmark";
            this.chkLandmark.Size = new System.Drawing.Size(100, 22);
            this.chkLandmark.TabIndex = 5;
            this.chkLandmark.Text = "Danh lam";
            this.chkLandmark.AutoSize = true;
            this.chkLandmark.Checked = true;
            this.chkLandmark.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLandmark.Margin = new System.Windows.Forms.Padding(0, 0, 8, 2);
            this.chkLandmark.CheckedChanged += new System.EventHandler(this.chkKind_CheckedChanged);
            // 
            // btnKindAll
            // 
            this.btnKindAll.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnKindAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnKindAll.FlatAppearance.BorderSize = 0;
            this.btnKindAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKindAll.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnKindAll.Name = "btnKindAll";
            this.btnKindAll.Size = new System.Drawing.Size(100, 24);
            this.btnKindAll.TabIndex = 9;
            this.btnKindAll.Text = "Tất cả / Bỏ";
            this.btnKindAll.UseVisualStyleBackColor = false;
            this.btnKindAll.Click += new System.EventHandler(this.btnKindAll_Click);
            // 
            // pnlBottomP
            // 
            this.pnlBottomP.Controls.Add(this.lblCount);
            this.pnlBottomP.Controls.Add(this.btnPoiView);
            this.pnlBottomP.Controls.Add(this.btnPoiA);
            this.pnlBottomP.Controls.Add(this.btnPoiB);
            this.pnlBottomP.Controls.Add(this.lblPoiHint);
            this.pnlBottomP.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlBottomP.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottomP.Name = "pnlBottomP";
            this.pnlBottomP.Size = new System.Drawing.Size(380, 92);
            this.pnlBottomP.TabIndex = 1;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = false;
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCount.Location = new System.Drawing.Point(12, 8);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(356, 18);
            this.lblCount.TabIndex = 0;
            this.lblCount.Text = "0 địa điểm";
            // 
            // btnPoiView
            // 
            this.btnPoiView.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnPoiView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPoiView.FlatAppearance.BorderSize = 0;
            this.btnPoiView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPoiView.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnPoiView.Location = new System.Drawing.Point(12, 32);
            this.btnPoiView.Name = "btnPoiView";
            this.btnPoiView.Size = new System.Drawing.Size(112, 30);
            this.btnPoiView.TabIndex = 1;
            this.btnPoiView.Text = "Xem trên bản đồ";
            this.btnPoiView.UseVisualStyleBackColor = false;
            this.btnPoiView.Click += new System.EventHandler(this.btnPoiView_Click);
            // 
            // btnPoiA
            // 
            this.btnPoiA.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnPoiA.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPoiA.FlatAppearance.BorderSize = 0;
            this.btnPoiA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPoiA.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnPoiA.Location = new System.Drawing.Point(130, 32);
            this.btnPoiA.Name = "btnPoiA";
            this.btnPoiA.Size = new System.Drawing.Size(112, 30);
            this.btnPoiA.TabIndex = 2;
            this.btnPoiA.Text = "Đặt làm điểm đi A";
            this.btnPoiA.UseVisualStyleBackColor = false;
            this.btnPoiA.Click += new System.EventHandler(this.btnPoiA_Click);
            // 
            // btnPoiB
            // 
            this.btnPoiB.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.btnPoiB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPoiB.FlatAppearance.BorderSize = 0;
            this.btnPoiB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPoiB.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnPoiB.Location = new System.Drawing.Point(248, 32);
            this.btnPoiB.Name = "btnPoiB";
            this.btnPoiB.Size = new System.Drawing.Size(112, 30);
            this.btnPoiB.TabIndex = 3;
            this.btnPoiB.Text = "Đặt làm điểm đến B";
            this.btnPoiB.UseVisualStyleBackColor = false;
            this.btnPoiB.Click += new System.EventHandler(this.btnPoiB_Click);
            // 
            // lblPoiHint
            // 
            this.lblPoiHint.AutoSize = false;
            this.lblPoiHint.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblPoiHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPoiHint.Location = new System.Drawing.Point(12, 66);
            this.lblPoiHint.Name = "lblPoiHint";
            this.lblPoiHint.Size = new System.Drawing.Size(356, 20);
            this.lblPoiHint.TabIndex = 4;
            this.lblPoiHint.Text = "Bấm đúp một địa điểm để xem trên bản đồ.";
            // 
            // lstPois
            // 
            this.lstPois.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lstPois.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPois.Name = "lstPois";
            this.lstPois.TabIndex = 0;
            this.lstPois.FormattingEnabled = true;
            this.lstPois.IntegralHeight = false;
            this.lstPois.ItemHeight = 17;
            this.lstPois.DoubleClick += new System.EventHandler(this.lstPois_DoubleClick);
            // 
            // PoiForm
            // 
            this.Controls.Add(this.lstPois);
            this.Controls.Add(this.pnlBottomP);
            this.Controls.Add(this.pnlTopP);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(380, 560);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PoiForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Lọc địa điểm";
            this.pnlTopP.ResumeLayout(false);
            this.flowKinds.ResumeLayout(false);
            this.pnlBottomP.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTopP;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblKinds;
        private System.Windows.Forms.FlowLayoutPanel flowKinds;
        private System.Windows.Forms.CheckBox chkHospital;
        private System.Windows.Forms.CheckBox chkSchool;
        private System.Windows.Forms.CheckBox chkUniversity;
        private System.Windows.Forms.CheckBox chkMarket;
        private System.Windows.Forms.CheckBox chkPark;
        private System.Windows.Forms.CheckBox chkLandmark;
        private System.Windows.Forms.Button btnKindAll;
        private System.Windows.Forms.Panel pnlBottomP;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnPoiView;
        private System.Windows.Forms.Button btnPoiA;
        private System.Windows.Forms.Button btnPoiB;
        private System.Windows.Forms.Label lblPoiHint;
        private System.Windows.Forms.ListBox lstPois;
    }
}
