namespace TestMap.Forms
{
    partial class TableForm
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
            this.tableTimer = new System.Windows.Forms.Timer(this.components);
            this.grid = new System.Windows.Forms.DataGridView();
            this.lstTableNodes = new System.Windows.Forms.ListBox();
            this.pnlTableBottom = new System.Windows.Forms.Panel();
            this.lblTableStep = new System.Windows.Forms.Label();
            this.trkTable = new System.Windows.Forms.TrackBar();
            this.btnTableRestart = new System.Windows.Forms.Button();
            this.btnTableBack = new System.Windows.Forms.Button();
            this.btnTablePlay = new System.Windows.Forms.Button();
            this.btnTableNext = new System.Windows.Forms.Button();
            this.btnTableEnd = new System.Windows.Forms.Button();
            this.lblTableLegend = new System.Windows.Forms.Label();
            this.lblTableExplain = new System.Windows.Forms.Label();
            this.pnlTableTop = new System.Windows.Forms.Panel();
            this.lblTableGraph = new System.Windows.Forms.Label();
            this.cboTableGraph = new System.Windows.Forms.ComboBox();
            this.lblTableSrc = new System.Windows.Forms.Label();
            this.cboTableSrc = new System.Windows.Forms.ComboBox();
            this.lblTableDst = new System.Windows.Forms.Label();
            this.cboTableDst = new System.Windows.Forms.ComboBox();
            this.btnTableBuild = new System.Windows.Forms.Button();
            this.pnlTableBottom.SuspendLayout();
            this.pnlTableTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableTimer
            // 
            this.tableTimer.Interval = 1000;
            this.tableTimer.Tick += new System.EventHandler(this.tableTimer_Tick);
            // 
            // grid
            // 
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Name = "grid";
            this.grid.TabIndex = 0;
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.BackgroundColor = System.Drawing.Color.White;
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grid.EnableHeadersVisualStyles = false;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            // 
            // lstTableNodes
            // 
            this.lstTableNodes.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lstTableNodes.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lstTableNodes.Dock = System.Windows.Forms.DockStyle.Right;
            this.lstTableNodes.Name = "lstTableNodes";
            this.lstTableNodes.Size = new System.Drawing.Size(230, 300);
            this.lstTableNodes.TabIndex = 1;
            this.lstTableNodes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTableNodes.FormattingEnabled = true;
            this.lstTableNodes.IntegralHeight = false;
            this.lstTableNodes.ItemHeight = 17;
            this.lstTableNodes.SelectionMode = System.Windows.Forms.SelectionMode.None;
            // 
            // pnlTableBottom
            // 
            this.pnlTableBottom.Controls.Add(this.lblTableStep);
            this.pnlTableBottom.Controls.Add(this.trkTable);
            this.pnlTableBottom.Controls.Add(this.btnTableRestart);
            this.pnlTableBottom.Controls.Add(this.btnTableBack);
            this.pnlTableBottom.Controls.Add(this.btnTablePlay);
            this.pnlTableBottom.Controls.Add(this.btnTableNext);
            this.pnlTableBottom.Controls.Add(this.btnTableEnd);
            this.pnlTableBottom.Controls.Add(this.lblTableLegend);
            this.pnlTableBottom.Controls.Add(this.lblTableExplain);
            this.pnlTableBottom.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlTableBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTableBottom.Name = "pnlTableBottom";
            this.pnlTableBottom.Size = new System.Drawing.Size(1100, 250);
            this.pnlTableBottom.TabIndex = 2;
            // 
            // lblTableStep
            // 
            this.lblTableStep.AutoSize = false;
            this.lblTableStep.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblTableStep.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTableStep.Location = new System.Drawing.Point(12, 6);
            this.lblTableStep.Name = "lblTableStep";
            this.lblTableStep.Size = new System.Drawing.Size(600, 20);
            this.lblTableStep.TabIndex = 0;
            this.lblTableStep.Text = "Bước 0 / 0";
            // 
            // trkTable
            // 
            this.trkTable.Location = new System.Drawing.Point(6, 26);
            this.trkTable.Name = "trkTable";
            this.trkTable.Size = new System.Drawing.Size(640, 45);
            this.trkTable.TabIndex = 1;
            this.trkTable.Maximum = 0;
            this.trkTable.Minimum = 0;
            this.trkTable.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkTable.Enabled = false;
            this.trkTable.Scroll += new System.EventHandler(this.trkTable_Scroll);
            // 
            // btnTableRestart
            // 
            this.btnTableRestart.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnTableRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTableRestart.FlatAppearance.BorderSize = 0;
            this.btnTableRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTableRestart.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnTableRestart.Location = new System.Drawing.Point(12, 74);
            this.btnTableRestart.Name = "btnTableRestart";
            this.btnTableRestart.Size = new System.Drawing.Size(40, 32);
            this.btnTableRestart.TabIndex = 2;
            this.btnTableRestart.Text = "⟲";
            this.btnTableRestart.UseVisualStyleBackColor = false;
            this.btnTableRestart.Click += new System.EventHandler(this.btnTableRestart_Click);
            // 
            // btnTableBack
            // 
            this.btnTableBack.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnTableBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTableBack.FlatAppearance.BorderSize = 0;
            this.btnTableBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTableBack.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnTableBack.Location = new System.Drawing.Point(56, 74);
            this.btnTableBack.Name = "btnTableBack";
            this.btnTableBack.Size = new System.Drawing.Size(64, 32);
            this.btnTableBack.TabIndex = 3;
            this.btnTableBack.Text = "◀ Lùi";
            this.btnTableBack.UseVisualStyleBackColor = false;
            this.btnTableBack.Click += new System.EventHandler(this.btnTableBack_Click);
            // 
            // btnTablePlay
            // 
            this.btnTablePlay.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnTablePlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTablePlay.FlatAppearance.BorderSize = 0;
            this.btnTablePlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTablePlay.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnTablePlay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTablePlay.Location = new System.Drawing.Point(124, 74);
            this.btnTablePlay.Name = "btnTablePlay";
            this.btnTablePlay.Size = new System.Drawing.Size(100, 32);
            this.btnTablePlay.TabIndex = 4;
            this.btnTablePlay.Text = "▶ Chạy";
            this.btnTablePlay.UseVisualStyleBackColor = false;
            this.btnTablePlay.Click += new System.EventHandler(this.btnTablePlay_Click);
            // 
            // btnTableNext
            // 
            this.btnTableNext.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnTableNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTableNext.FlatAppearance.BorderSize = 0;
            this.btnTableNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTableNext.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnTableNext.Location = new System.Drawing.Point(228, 74);
            this.btnTableNext.Name = "btnTableNext";
            this.btnTableNext.Size = new System.Drawing.Size(64, 32);
            this.btnTableNext.TabIndex = 5;
            this.btnTableNext.Text = "Tới ▶";
            this.btnTableNext.UseVisualStyleBackColor = false;
            this.btnTableNext.Click += new System.EventHandler(this.btnTableNext_Click);
            // 
            // btnTableEnd
            // 
            this.btnTableEnd.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnTableEnd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTableEnd.FlatAppearance.BorderSize = 0;
            this.btnTableEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTableEnd.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnTableEnd.Location = new System.Drawing.Point(296, 74);
            this.btnTableEnd.Name = "btnTableEnd";
            this.btnTableEnd.Size = new System.Drawing.Size(84, 32);
            this.btnTableEnd.TabIndex = 6;
            this.btnTableEnd.Text = "⏭ Kết quả";
            this.btnTableEnd.UseVisualStyleBackColor = false;
            this.btnTableEnd.Click += new System.EventHandler(this.btnTableEnd_Click);
            // 
            // lblTableLegend
            // 
            this.lblTableLegend.AutoSize = false;
            this.lblTableLegend.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblTableLegend.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblTableLegend.Location = new System.Drawing.Point(396, 76);
            this.lblTableLegend.Name = "lblTableLegend";
            this.lblTableLegend.Size = new System.Drawing.Size(640, 30);
            this.lblTableLegend.TabIndex = 7;
            this.lblTableLegend.Text = "Bảng: ô = (khoảng cách ngắn nhất hiện biết, đỉnh đứng trước).  Ô xanh đậm = đỉnh được chốt tiếp.  Ô vàng = vừa cập nhật.  \"–\" = đã chốt.  Trên bản đồ: chấm cam = đã chốt, chấm xanh = đang xét.";
            // 
            // lblTableExplain
            // 
            this.lblTableExplain.AutoSize = false;
            this.lblTableExplain.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lblTableExplain.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblTableExplain.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblTableExplain.Location = new System.Drawing.Point(12, 114);
            this.lblTableExplain.Name = "lblTableExplain";
            this.lblTableExplain.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.lblTableExplain.Size = new System.Drawing.Size(860, 130);
            this.lblTableExplain.TabIndex = 8;
            this.lblTableExplain.Text = "";
            // 
            // pnlTableTop
            // 
            this.pnlTableTop.Controls.Add(this.lblTableGraph);
            this.pnlTableTop.Controls.Add(this.cboTableGraph);
            this.pnlTableTop.Controls.Add(this.lblTableSrc);
            this.pnlTableTop.Controls.Add(this.cboTableSrc);
            this.pnlTableTop.Controls.Add(this.lblTableDst);
            this.pnlTableTop.Controls.Add(this.cboTableDst);
            this.pnlTableTop.Controls.Add(this.btnTableBuild);
            this.pnlTableTop.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.pnlTableTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTableTop.Name = "pnlTableTop";
            this.pnlTableTop.Size = new System.Drawing.Size(1100, 46);
            this.pnlTableTop.TabIndex = 3;
            // 
            // lblTableGraph
            // 
            this.lblTableGraph.AutoSize = false;
            this.lblTableGraph.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblTableGraph.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTableGraph.Location = new System.Drawing.Point(12, 15);
            this.lblTableGraph.Name = "lblTableGraph";
            this.lblTableGraph.Size = new System.Drawing.Size(50, 20);
            this.lblTableGraph.TabIndex = 0;
            this.lblTableGraph.Text = "Đồ thị";
            // 
            // cboTableGraph
            // 
            this.cboTableGraph.Location = new System.Drawing.Point(64, 11);
            this.cboTableGraph.Name = "cboTableGraph";
            this.cboTableGraph.Size = new System.Drawing.Size(250, 23);
            this.cboTableGraph.TabIndex = 1;
            this.cboTableGraph.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTableGraph.FormattingEnabled = true;
            this.cboTableGraph.SelectedIndexChanged += new System.EventHandler(this.cboTableGraph_SelectedIndexChanged);
            // 
            // lblTableSrc
            // 
            this.lblTableSrc.AutoSize = false;
            this.lblTableSrc.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblTableSrc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTableSrc.Location = new System.Drawing.Point(330, 15);
            this.lblTableSrc.Name = "lblTableSrc";
            this.lblTableSrc.Size = new System.Drawing.Size(28, 20);
            this.lblTableSrc.TabIndex = 2;
            this.lblTableSrc.Text = "Từ";
            // 
            // cboTableSrc
            // 
            this.cboTableSrc.Location = new System.Drawing.Point(360, 11);
            this.cboTableSrc.Name = "cboTableSrc";
            this.cboTableSrc.Size = new System.Drawing.Size(190, 23);
            this.cboTableSrc.TabIndex = 3;
            this.cboTableSrc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTableSrc.FormattingEnabled = true;
            // 
            // lblTableDst
            // 
            this.lblTableDst.AutoSize = false;
            this.lblTableDst.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblTableDst.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTableDst.Location = new System.Drawing.Point(566, 15);
            this.lblTableDst.Name = "lblTableDst";
            this.lblTableDst.Size = new System.Drawing.Size(36, 20);
            this.lblTableDst.TabIndex = 4;
            this.lblTableDst.Text = "Tới";
            // 
            // cboTableDst
            // 
            this.cboTableDst.Location = new System.Drawing.Point(604, 11);
            this.cboTableDst.Name = "cboTableDst";
            this.cboTableDst.Size = new System.Drawing.Size(190, 23);
            this.cboTableDst.TabIndex = 5;
            this.cboTableDst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTableDst.FormattingEnabled = true;
            // 
            // btnTableBuild
            // 
            this.btnTableBuild.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnTableBuild.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTableBuild.FlatAppearance.BorderSize = 0;
            this.btnTableBuild.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTableBuild.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnTableBuild.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTableBuild.Location = new System.Drawing.Point(810, 9);
            this.btnTableBuild.Name = "btnTableBuild";
            this.btnTableBuild.Size = new System.Drawing.Size(120, 28);
            this.btnTableBuild.TabIndex = 6;
            this.btnTableBuild.Text = "Lập bảng";
            this.btnTableBuild.UseVisualStyleBackColor = false;
            this.btnTableBuild.Click += new System.EventHandler(this.btnTableBuild_Click);
            // 
            // TableForm
            // 
            this.Controls.Add(this.grid);
            this.Controls.Add(this.lstTableNodes);
            this.Controls.Add(this.pnlTableBottom);
            this.Controls.Add(this.pnlTableTop);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "TableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bảng Dijkstra từng bước";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TableForm_FormClosed);
            this.pnlTableBottom.ResumeLayout(false);
            this.pnlTableTop.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer tableTimer;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.ListBox lstTableNodes;
        private System.Windows.Forms.Panel pnlTableBottom;
        private System.Windows.Forms.Label lblTableStep;
        private System.Windows.Forms.TrackBar trkTable;
        private System.Windows.Forms.Button btnTableRestart;
        private System.Windows.Forms.Button btnTableBack;
        private System.Windows.Forms.Button btnTablePlay;
        private System.Windows.Forms.Button btnTableNext;
        private System.Windows.Forms.Button btnTableEnd;
        private System.Windows.Forms.Label lblTableLegend;
        private System.Windows.Forms.Label lblTableExplain;
        private System.Windows.Forms.Panel pnlTableTop;
        private System.Windows.Forms.Label lblTableGraph;
        private System.Windows.Forms.ComboBox cboTableGraph;
        private System.Windows.Forms.Label lblTableSrc;
        private System.Windows.Forms.ComboBox cboTableSrc;
        private System.Windows.Forms.Label lblTableDst;
        private System.Windows.Forms.ComboBox cboTableDst;
        private System.Windows.Forms.Button btnTableBuild;
    }
}
