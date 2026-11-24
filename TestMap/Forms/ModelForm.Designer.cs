namespace TestMap.Forms
{
    partial class ModelForm
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
            this.modelTimer = new System.Windows.Forms.Timer(this.components);
            this.canvas = new TestMap.Forms.ModelCanvas();
            this.pnlModelBottom = new System.Windows.Forms.Panel();
            this.lstModelLegs = new System.Windows.Forms.ListBox();
            this.lblModelStep = new System.Windows.Forms.Label();
            this.trkModel = new System.Windows.Forms.TrackBar();
            this.btnModelRestart = new System.Windows.Forms.Button();
            this.btnModelBack = new System.Windows.Forms.Button();
            this.btnModelPlay = new System.Windows.Forms.Button();
            this.btnModelNext = new System.Windows.Forms.Button();
            this.chkModelWeights = new System.Windows.Forms.CheckBox();
            this.lblModelExplain = new System.Windows.Forms.Label();
            this.pnlModelTop = new System.Windows.Forms.Panel();
            this.lblModelPick = new System.Windows.Forms.Label();
            this.cboModelItin = new System.Windows.Forms.ComboBox();
            this.pnlModelBottom.SuspendLayout();
            this.pnlModelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelTimer
            // 
            this.modelTimer.Interval = 900;
            this.modelTimer.Tick += new System.EventHandler(this.modelTimer_Tick);
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Name = "canvas";
            this.canvas.TabIndex = 0;
            // 
            // pnlModelBottom
            // 
            this.pnlModelBottom.Controls.Add(this.lstModelLegs);
            this.pnlModelBottom.Controls.Add(this.lblModelStep);
            this.pnlModelBottom.Controls.Add(this.trkModel);
            this.pnlModelBottom.Controls.Add(this.btnModelRestart);
            this.pnlModelBottom.Controls.Add(this.btnModelBack);
            this.pnlModelBottom.Controls.Add(this.btnModelPlay);
            this.pnlModelBottom.Controls.Add(this.btnModelNext);
            this.pnlModelBottom.Controls.Add(this.chkModelWeights);
            this.pnlModelBottom.Controls.Add(this.lblModelExplain);
            this.pnlModelBottom.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlModelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlModelBottom.Name = "pnlModelBottom";
            this.pnlModelBottom.Size = new System.Drawing.Size(1000, 236);
            this.pnlModelBottom.TabIndex = 1;
            // 
            // lstModelLegs
            // 
            this.lstModelLegs.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lstModelLegs.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lstModelLegs.Dock = System.Windows.Forms.DockStyle.Right;
            this.lstModelLegs.Name = "lstModelLegs";
            this.lstModelLegs.Size = new System.Drawing.Size(430, 236);
            this.lstModelLegs.TabIndex = 0;
            this.lstModelLegs.FormattingEnabled = true;
            this.lstModelLegs.IntegralHeight = false;
            this.lstModelLegs.ItemHeight = 17;
            this.lstModelLegs.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstModelLegs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            // 
            // lblModelStep
            // 
            this.lblModelStep.AutoSize = false;
            this.lblModelStep.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblModelStep.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblModelStep.Location = new System.Drawing.Point(12, 8);
            this.lblModelStep.Name = "lblModelStep";
            this.lblModelStep.Size = new System.Drawing.Size(540, 20);
            this.lblModelStep.TabIndex = 1;
            this.lblModelStep.Text = "Bước 0 / 0";
            // 
            // trkModel
            // 
            this.trkModel.Location = new System.Drawing.Point(6, 30);
            this.trkModel.Name = "trkModel";
            this.trkModel.Size = new System.Drawing.Size(552, 45);
            this.trkModel.TabIndex = 2;
            this.trkModel.Maximum = 0;
            this.trkModel.Minimum = 0;
            this.trkModel.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkModel.Enabled = false;
            this.trkModel.Scroll += new System.EventHandler(this.trkModel_Scroll);
            // 
            // btnModelRestart
            // 
            this.btnModelRestart.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnModelRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModelRestart.FlatAppearance.BorderSize = 0;
            this.btnModelRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModelRestart.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnModelRestart.Location = new System.Drawing.Point(12, 78);
            this.btnModelRestart.Name = "btnModelRestart";
            this.btnModelRestart.Size = new System.Drawing.Size(40, 32);
            this.btnModelRestart.TabIndex = 3;
            this.btnModelRestart.Text = "⟲";
            this.btnModelRestart.UseVisualStyleBackColor = false;
            this.btnModelRestart.Click += new System.EventHandler(this.btnModelRestart_Click);
            // 
            // btnModelBack
            // 
            this.btnModelBack.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnModelBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModelBack.FlatAppearance.BorderSize = 0;
            this.btnModelBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModelBack.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnModelBack.Location = new System.Drawing.Point(56, 78);
            this.btnModelBack.Name = "btnModelBack";
            this.btnModelBack.Size = new System.Drawing.Size(64, 32);
            this.btnModelBack.TabIndex = 4;
            this.btnModelBack.Text = "◀ Lùi";
            this.btnModelBack.UseVisualStyleBackColor = false;
            this.btnModelBack.Click += new System.EventHandler(this.btnModelBack_Click);
            // 
            // btnModelPlay
            // 
            this.btnModelPlay.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnModelPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModelPlay.FlatAppearance.BorderSize = 0;
            this.btnModelPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModelPlay.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnModelPlay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnModelPlay.Location = new System.Drawing.Point(124, 78);
            this.btnModelPlay.Name = "btnModelPlay";
            this.btnModelPlay.Size = new System.Drawing.Size(100, 32);
            this.btnModelPlay.TabIndex = 5;
            this.btnModelPlay.Text = "▶ Chạy";
            this.btnModelPlay.UseVisualStyleBackColor = false;
            this.btnModelPlay.Click += new System.EventHandler(this.btnModelPlay_Click);
            // 
            // btnModelNext
            // 
            this.btnModelNext.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnModelNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModelNext.FlatAppearance.BorderSize = 0;
            this.btnModelNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModelNext.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btnModelNext.Location = new System.Drawing.Point(228, 78);
            this.btnModelNext.Name = "btnModelNext";
            this.btnModelNext.Size = new System.Drawing.Size(64, 32);
            this.btnModelNext.TabIndex = 6;
            this.btnModelNext.Text = "Tới ▶";
            this.btnModelNext.UseVisualStyleBackColor = false;
            this.btnModelNext.Click += new System.EventHandler(this.btnModelNext_Click);
            // 
            // chkModelWeights
            // 
            this.chkModelWeights.Location = new System.Drawing.Point(308, 82);
            this.chkModelWeights.Name = "chkModelWeights";
            this.chkModelWeights.Size = new System.Drawing.Size(250, 24);
            this.chkModelWeights.TabIndex = 7;
            this.chkModelWeights.Text = "Hiện trọng số (giây)";
            this.chkModelWeights.AutoSize = false;
            this.chkModelWeights.Checked = true;
            this.chkModelWeights.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkModelWeights.CheckedChanged += new System.EventHandler(this.chkModelWeights_CheckedChanged);
            // 
            // lblModelExplain
            // 
            this.lblModelExplain.AutoSize = false;
            this.lblModelExplain.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.lblModelExplain.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblModelExplain.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblModelExplain.Location = new System.Drawing.Point(12, 118);
            this.lblModelExplain.Name = "lblModelExplain";
            this.lblModelExplain.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.lblModelExplain.Size = new System.Drawing.Size(552, 112);
            this.lblModelExplain.TabIndex = 8;
            this.lblModelExplain.Text = "";
            // 
            // pnlModelTop
            // 
            this.pnlModelTop.Controls.Add(this.lblModelPick);
            this.pnlModelTop.Controls.Add(this.cboModelItin);
            this.pnlModelTop.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.pnlModelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlModelTop.Name = "pnlModelTop";
            this.pnlModelTop.Size = new System.Drawing.Size(1000, 40);
            this.pnlModelTop.TabIndex = 2;
            // 
            // lblModelPick
            // 
            this.lblModelPick.AutoSize = false;
            this.lblModelPick.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblModelPick.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblModelPick.Location = new System.Drawing.Point(12, 11);
            this.lblModelPick.Name = "lblModelPick";
            this.lblModelPick.Size = new System.Drawing.Size(70, 20);
            this.lblModelPick.TabIndex = 0;
            this.lblModelPick.Text = "Cách đi";
            // 
            // cboModelItin
            // 
            this.cboModelItin.Location = new System.Drawing.Point(80, 8);
            this.cboModelItin.Name = "cboModelItin";
            this.cboModelItin.Size = new System.Drawing.Size(700, 23);
            this.cboModelItin.TabIndex = 1;
            this.cboModelItin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModelItin.FormattingEnabled = true;
            this.cboModelItin.SelectedIndexChanged += new System.EventHandler(this.cboModelItin_SelectedIndexChanged);
            // 
            // ModelForm
            // 
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.pnlModelBottom);
            this.Controls.Add(this.pnlModelTop);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(820, 560);
            this.Name = "ModelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sơ đồ mô hình gợi ý tuyến xe buýt";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModelForm_FormClosed);
            this.pnlModelBottom.ResumeLayout(false);
            this.pnlModelTop.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer modelTimer;
        private TestMap.Forms.ModelCanvas canvas;
        private System.Windows.Forms.Panel pnlModelBottom;
        private System.Windows.Forms.ListBox lstModelLegs;
        private System.Windows.Forms.Label lblModelStep;
        private System.Windows.Forms.TrackBar trkModel;
        private System.Windows.Forms.Button btnModelRestart;
        private System.Windows.Forms.Button btnModelBack;
        private System.Windows.Forms.Button btnModelPlay;
        private System.Windows.Forms.Button btnModelNext;
        private System.Windows.Forms.CheckBox chkModelWeights;
        private System.Windows.Forms.Label lblModelExplain;
        private System.Windows.Forms.Panel pnlModelTop;
        private System.Windows.Forms.Label lblModelPick;
        private System.Windows.Forms.ComboBox cboModelItin;
    }
}
