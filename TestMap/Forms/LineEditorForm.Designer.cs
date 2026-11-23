namespace TestMap.Forms
{
    partial class LineEditorForm
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
            this.lblRef = new System.Windows.Forms.Label();
            this.txtRef = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblFare = new System.Windows.Forms.Label();
            this.numFare = new System.Windows.Forms.NumericUpDown();
            this.lblHeadway = new System.Windows.Forms.Label();
            this.numHeadway = new System.Windows.Forms.NumericUpDown();
            this.lblHours = new System.Windows.Forms.Label();
            this.txtHours = new System.Windows.Forms.TextBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.btnColor = new System.Windows.Forms.Button();
            this.chkAutoColor = new System.Windows.Forms.CheckBox();
            this.grpStops = new System.Windows.Forms.GroupBox();
            this.lstStops = new System.Windows.Forms.ListBox();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.cboAddStop = new System.Windows.Forms.ComboBox();
            this.btnAddStop = new System.Windows.Forms.Button();
            this.lblStopCount = new System.Windows.Forms.Label();
            this.chkReverse = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numFare)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeadway)).BeginInit();
            this.grpStops.SuspendLayout();
            this.SuspendLayout();
            //
            // lblRef
            //
            this.lblRef.AutoSize = true;
            this.lblRef.Location = new System.Drawing.Point(18, 16);
            this.lblRef.Name = "lblRef";
            this.lblRef.Size = new System.Drawing.Size(58, 15);
            this.lblRef.TabIndex = 0;
            this.lblRef.Text = "Số tuyến";
            //
            // txtRef
            //
            this.txtRef.Location = new System.Drawing.Point(20, 36);
            this.txtRef.MaxLength = 10;
            this.txtRef.Name = "txtRef";
            this.txtRef.Size = new System.Drawing.Size(90, 23);
            this.txtRef.TabIndex = 1;
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(128, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(120, 15);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Tên / hướng đi (tùy chọn)";
            //
            // txtName
            //
            this.txtName.Location = new System.Drawing.Point(130, 36);
            this.txtName.MaxLength = 100;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(490, 23);
            this.txtName.TabIndex = 3;
            //
            // lblFare
            //
            this.lblFare.AutoSize = true;
            this.lblFare.Location = new System.Drawing.Point(18, 72);
            this.lblFare.Name = "lblFare";
            this.lblFare.Size = new System.Drawing.Size(88, 15);
            this.lblFare.TabIndex = 4;
            this.lblFare.Text = "Giá vé (đồng)";
            //
            // numFare
            //
            this.numFare.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numFare.Location = new System.Drawing.Point(20, 92);
            this.numFare.Maximum = new decimal(new int[] { 500000, 0, 0, 0 });
            this.numFare.Name = "numFare";
            this.numFare.Size = new System.Drawing.Size(120, 23);
            this.numFare.TabIndex = 5;
            this.numFare.ThousandsSeparator = true;
            this.numFare.Value = new decimal(new int[] { 7000, 0, 0, 0 });
            //
            // lblHeadway
            //
            this.lblHeadway.AutoSize = true;
            this.lblHeadway.Location = new System.Drawing.Point(158, 72);
            this.lblHeadway.Name = "lblHeadway";
            this.lblHeadway.Size = new System.Drawing.Size(120, 15);
            this.lblHeadway.TabIndex = 6;
            this.lblHeadway.Text = "Tần suất (phút/chuyến)";
            //
            // numHeadway
            //
            this.numHeadway.Location = new System.Drawing.Point(160, 92);
            this.numHeadway.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
            this.numHeadway.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numHeadway.Name = "numHeadway";
            this.numHeadway.Size = new System.Drawing.Size(100, 23);
            this.numHeadway.TabIndex = 7;
            this.numHeadway.Value = new decimal(new int[] { 10, 0, 0, 0 });
            //
            // lblHours
            //
            this.lblHours.AutoSize = true;
            this.lblHours.Location = new System.Drawing.Point(288, 72);
            this.lblHours.Name = "lblHours";
            this.lblHours.Size = new System.Drawing.Size(81, 15);
            this.lblHours.TabIndex = 8;
            this.lblHours.Text = "Giờ hoạt động";
            //
            // txtHours
            //
            this.txtHours.Location = new System.Drawing.Point(290, 92);
            this.txtHours.MaxLength = 30;
            this.txtHours.Name = "txtHours";
            this.txtHours.Size = new System.Drawing.Size(150, 23);
            this.txtHours.TabIndex = 9;
            //
            // lblColor
            //
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(458, 72);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(55, 15);
            this.lblColor.TabIndex = 10;
            this.lblColor.Text = "Màu tuyến";
            //
            // pnlColor
            //
            this.pnlColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColor.Location = new System.Drawing.Point(460, 91);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(26, 25);
            this.pnlColor.TabIndex = 11;
            //
            // btnColor
            //
            this.btnColor.Location = new System.Drawing.Point(492, 90);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(56, 27);
            this.btnColor.TabIndex = 12;
            this.btnColor.Text = "Chọn";
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            //
            // chkAutoColor
            //
            this.chkAutoColor.AutoSize = true;
            this.chkAutoColor.Location = new System.Drawing.Point(460, 120);
            this.chkAutoColor.Name = "chkAutoColor";
            this.chkAutoColor.Size = new System.Drawing.Size(113, 19);
            this.chkAutoColor.TabIndex = 13;
            this.chkAutoColor.Text = "Tự chọn màu";
            this.chkAutoColor.UseVisualStyleBackColor = true;
            this.chkAutoColor.CheckedChanged += new System.EventHandler(this.chkAutoColor_CheckedChanged);
            //
            // grpStops
            //
            this.grpStops.Controls.Add(this.lblStopCount);
            this.grpStops.Controls.Add(this.btnAddStop);
            this.grpStops.Controls.Add(this.cboAddStop);
            this.grpStops.Controls.Add(this.btnRemove);
            this.grpStops.Controls.Add(this.btnDown);
            this.grpStops.Controls.Add(this.btnUp);
            this.grpStops.Controls.Add(this.lstStops);
            this.grpStops.Location = new System.Drawing.Point(20, 150);
            this.grpStops.Name = "grpStops";
            this.grpStops.Size = new System.Drawing.Size(600, 330);
            this.grpStops.TabIndex = 14;
            this.grpStops.TabStop = false;
            this.grpStops.Text = "Các trạm theo thứ tự xe chạy";
            //
            // lstStops
            //
            this.lstStops.FormattingEnabled = true;
            this.lstStops.IntegralHeight = false;
            this.lstStops.ItemHeight = 15;
            this.lstStops.Location = new System.Drawing.Point(12, 56);
            this.lstStops.Name = "lstStops";
            this.lstStops.Size = new System.Drawing.Size(470, 232);
            this.lstStops.TabIndex = 0;
            //
            // btnUp
            //
            this.btnUp.Location = new System.Drawing.Point(492, 56);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(96, 32);
            this.btnUp.TabIndex = 1;
            this.btnUp.Text = "▲ Lên";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            //
            // btnDown
            //
            this.btnDown.Location = new System.Drawing.Point(492, 94);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(96, 32);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "▼ Xuống";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            //
            // btnRemove
            //
            this.btnRemove.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            this.btnRemove.Location = new System.Drawing.Point(492, 132);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(96, 32);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "Bỏ trạm";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            //
            // cboAddStop
            //
            this.cboAddStop.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboAddStop.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboAddStop.FormattingEnabled = true;
            this.cboAddStop.Location = new System.Drawing.Point(12, 24);
            this.cboAddStop.MaxDropDownItems = 16;
            this.cboAddStop.Name = "cboAddStop";
            this.cboAddStop.Size = new System.Drawing.Size(370, 23);
            this.cboAddStop.TabIndex = 4;
            //
            // btnAddStop
            //
            this.btnAddStop.Location = new System.Drawing.Point(390, 22);
            this.btnAddStop.Name = "btnAddStop";
            this.btnAddStop.Size = new System.Drawing.Size(92, 27);
            this.btnAddStop.TabIndex = 5;
            this.btnAddStop.Text = "+ Thêm trạm";
            this.btnAddStop.UseVisualStyleBackColor = true;
            this.btnAddStop.Click += new System.EventHandler(this.btnAddStop_Click);
            //
            // lblStopCount
            //
            this.lblStopCount.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblStopCount.Location = new System.Drawing.Point(12, 294);
            this.lblStopCount.Name = "lblStopCount";
            this.lblStopCount.Size = new System.Drawing.Size(576, 28);
            this.lblStopCount.TabIndex = 6;
            this.lblStopCount.Text = "Gõ tên trạm vào ô phía trên rồi bấm Thêm trạm. Trạm được thêm vào cuối tuyến; dùng Lên / Xuống để đổi thứ tự.";
            //
            // chkReverse
            //
            this.chkReverse.AutoSize = true;
            this.chkReverse.Checked = true;
            this.chkReverse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReverse.Location = new System.Drawing.Point(20, 494);
            this.chkReverse.Name = "chkReverse";
            this.chkReverse.Size = new System.Drawing.Size(280, 19);
            this.chkReverse.TabIndex = 15;
            this.chkReverse.Text = "Tạo luôn chiều ngược lại (đảo thứ tự các trạm)";
            this.chkReverse.UseVisualStyleBackColor = true;
            //
            // btnOk
            //
            this.btnOk.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnOk.FlatAppearance.BorderSize = 0;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.Location = new System.Drawing.Point(400, 490);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(120, 34);
            this.btnOk.TabIndex = 16;
            this.btnOk.Text = "Lưu tuyến";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(528, 490);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 34);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // LineEditorForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(640, 540);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkReverse);
            this.Controls.Add(this.grpStops);
            this.Controls.Add(this.chkAutoColor);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.txtHours);
            this.Controls.Add(this.lblHours);
            this.Controls.Add(this.numHeadway);
            this.Controls.Add(this.lblHeadway);
            this.Controls.Add(this.numFare);
            this.Controls.Add(this.lblFare);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtRef);
            this.Controls.Add(this.lblRef);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LineEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tuyến xe buýt";
            ((System.ComponentModel.ISupportInitialize)(this.numFare)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeadway)).EndInit();
            this.grpStops.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblRef;
        private System.Windows.Forms.TextBox txtRef;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblFare;
        private System.Windows.Forms.NumericUpDown numFare;
        private System.Windows.Forms.Label lblHeadway;
        private System.Windows.Forms.NumericUpDown numHeadway;
        private System.Windows.Forms.Label lblHours;
        private System.Windows.Forms.TextBox txtHours;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.CheckBox chkAutoColor;
        private System.Windows.Forms.GroupBox grpStops;
        private System.Windows.Forms.ListBox lstStops;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ComboBox cboAddStop;
        private System.Windows.Forms.Button btnAddStop;
        private System.Windows.Forms.Label lblStopCount;
        private System.Windows.Forms.CheckBox chkReverse;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
