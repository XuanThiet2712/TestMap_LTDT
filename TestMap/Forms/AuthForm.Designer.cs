namespace TestMap.Forms
{
    partial class AuthForm
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
            this.tabAuth = new System.Windows.Forms.TabControl();
            this.tabLogin = new System.Windows.Forms.TabPage();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.tabRegister = new System.Windows.Forms.TabPage();
            this.lblRegRole = new System.Windows.Forms.Label();
            this.cboRole = new System.Windows.Forms.ComboBox();
            this.lblRegUser = new System.Windows.Forms.Label();
            this.txtRegUser = new System.Windows.Forms.TextBox();
            this.lblRegPass = new System.Windows.Forms.Label();
            this.txtRegPass = new System.Windows.Forms.TextBox();
            this.lblRegPass2 = new System.Windows.Forms.Label();
            this.txtRegPass2 = new System.Windows.Forms.TextBox();
            this.lblDriverCode = new System.Windows.Forms.Label();
            this.txtDriverCode = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.tabAuth.SuspendLayout();
            this.tabLogin.SuspendLayout();
            this.tabRegister.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabAuth
            // 
            this.tabAuth.Controls.Add(this.tabLogin);
            this.tabAuth.Controls.Add(this.tabRegister);
            this.tabAuth.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabAuth.Location = new System.Drawing.Point(0, 0);
            this.tabAuth.Name = "tabAuth";
            this.tabAuth.SelectedIndex = 0;
            this.tabAuth.Size = new System.Drawing.Size(384, 300);
            this.tabAuth.TabIndex = 0;
            this.tabAuth.SelectedIndexChanged += new System.EventHandler(this.tabAuth_SelectedIndexChanged);
            // 
            // tabLogin
            // 
            this.tabLogin.BackColor = System.Drawing.Color.White;
            this.tabLogin.Controls.Add(this.lblUser);
            this.tabLogin.Controls.Add(this.txtUser);
            this.tabLogin.Controls.Add(this.lblPass);
            this.tabLogin.Controls.Add(this.txtPass);
            this.tabLogin.Controls.Add(this.btnLogin);
            this.tabLogin.Location = new System.Drawing.Point(4, 26);
            this.tabLogin.Name = "tabLogin";
            this.tabLogin.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogin.Size = new System.Drawing.Size(376, 270);
            this.tabLogin.TabIndex = 0;
            this.tabLogin.Text = "Đăng nhập";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(24, 28);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(81, 15);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "Tên đăng nhập";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(24, 48);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(328, 23);
            this.txtUser.TabIndex = 1;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(24, 88);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(55, 15);
            this.lblPass.TabIndex = 2;
            this.lblPass.Text = "Mật khẩu";
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(24, 108);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(328, 23);
            this.txtPass.TabIndex = 3;
            this.txtPass.UseSystemPasswordChar = true;
            this.txtPass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPass_KeyDown);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(24, 156);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(328, 38);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tabRegister
            // 
            this.tabRegister.BackColor = System.Drawing.Color.White;
            this.tabRegister.Controls.Add(this.lblRegRole);
            this.tabRegister.Controls.Add(this.cboRole);
            this.tabRegister.Controls.Add(this.lblRegUser);
            this.tabRegister.Controls.Add(this.txtRegUser);
            this.tabRegister.Controls.Add(this.lblRegPass);
            this.tabRegister.Controls.Add(this.txtRegPass);
            this.tabRegister.Controls.Add(this.lblRegPass2);
            this.tabRegister.Controls.Add(this.txtRegPass2);
            this.tabRegister.Controls.Add(this.lblDriverCode);
            this.tabRegister.Controls.Add(this.txtDriverCode);
            this.tabRegister.Controls.Add(this.btnRegister);
            this.tabRegister.Location = new System.Drawing.Point(4, 26);
            this.tabRegister.Name = "tabRegister";
            this.tabRegister.Padding = new System.Windows.Forms.Padding(3);
            this.tabRegister.Size = new System.Drawing.Size(376, 270);
            this.tabRegister.TabIndex = 1;
            this.tabRegister.Text = "Đăng ký";
            // 
            // lblRegRole
            // 
            this.lblRegRole.AutoSize = true;
            this.lblRegRole.Location = new System.Drawing.Point(24, 12);
            this.lblRegRole.Name = "lblRegRole";
            this.lblRegRole.Size = new System.Drawing.Size(60, 15);
            this.lblRegRole.TabIndex = 0;
            this.lblRegRole.Text = "Bạn là";
            // 
            // cboRole
            // 
            this.cboRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRole.FormattingEnabled = true;
            this.cboRole.Items.AddRange(new object[] {
            "Hành khách",
            "Tài xế"});
            this.cboRole.Location = new System.Drawing.Point(24, 30);
            this.cboRole.Name = "cboRole";
            this.cboRole.SelectedIndex = 0;
            this.cboRole.Size = new System.Drawing.Size(328, 23);
            this.cboRole.TabIndex = 1;
            this.cboRole.SelectedIndexChanged += new System.EventHandler(this.cboRole_SelectedIndexChanged);
            // 
            // lblRegUser
            // 
            this.lblRegUser.AutoSize = true;
            this.lblRegUser.Location = new System.Drawing.Point(24, 62);
            this.lblRegUser.Name = "lblRegUser";
            this.lblRegUser.Size = new System.Drawing.Size(81, 15);
            this.lblRegUser.TabIndex = 2;
            this.lblRegUser.Text = "Tên đăng nhập";
            // 
            // txtRegUser
            // 
            this.txtRegUser.Location = new System.Drawing.Point(24, 80);
            this.txtRegUser.Name = "txtRegUser";
            this.txtRegUser.Size = new System.Drawing.Size(328, 23);
            this.txtRegUser.TabIndex = 3;
            // 
            // lblRegPass
            // 
            this.lblRegPass.AutoSize = true;
            this.lblRegPass.Location = new System.Drawing.Point(24, 110);
            this.lblRegPass.Name = "lblRegPass";
            this.lblRegPass.Size = new System.Drawing.Size(55, 15);
            this.lblRegPass.TabIndex = 4;
            this.lblRegPass.Text = "Mật khẩu";
            // 
            // txtRegPass
            // 
            this.txtRegPass.Location = new System.Drawing.Point(24, 128);
            this.txtRegPass.Name = "txtRegPass";
            this.txtRegPass.Size = new System.Drawing.Size(328, 23);
            this.txtRegPass.TabIndex = 5;
            this.txtRegPass.UseSystemPasswordChar = true;
            // 
            // lblRegPass2
            // 
            this.lblRegPass2.AutoSize = true;
            this.lblRegPass2.Location = new System.Drawing.Point(24, 158);
            this.lblRegPass2.Name = "lblRegPass2";
            this.lblRegPass2.Size = new System.Drawing.Size(100, 15);
            this.lblRegPass2.TabIndex = 6;
            this.lblRegPass2.Text = "Nhập lại mật khẩu";
            // 
            // txtRegPass2
            // 
            this.txtRegPass2.Location = new System.Drawing.Point(24, 176);
            this.txtRegPass2.Name = "txtRegPass2";
            this.txtRegPass2.Size = new System.Drawing.Size(328, 23);
            this.txtRegPass2.TabIndex = 7;
            this.txtRegPass2.UseSystemPasswordChar = true;
            // 
            // lblDriverCode
            // 
            this.lblDriverCode.AutoSize = true;
            this.lblDriverCode.Location = new System.Drawing.Point(24, 206);
            this.lblDriverCode.Name = "lblDriverCode";
            this.lblDriverCode.Size = new System.Drawing.Size(120, 15);
            this.lblDriverCode.TabIndex = 8;
            this.lblDriverCode.Text = "Mã đăng ký tài xế";
            this.lblDriverCode.Visible = false;
            // 
            // txtDriverCode
            // 
            this.txtDriverCode.Location = new System.Drawing.Point(150, 203);
            this.txtDriverCode.Name = "txtDriverCode";
            this.txtDriverCode.Size = new System.Drawing.Size(202, 23);
            this.txtDriverCode.TabIndex = 9;
            this.txtDriverCode.Visible = false;
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(24, 230);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(328, 34);
            this.btnRegister.TabIndex = 10;
            this.btnRegister.Text = "Tạo tài khoản";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            this.lblMsg.Location = new System.Drawing.Point(0, 300);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Padding = new System.Windows.Forms.Padding(24, 8, 24, 0);
            this.lblMsg.Size = new System.Drawing.Size(384, 61);
            this.lblMsg.TabIndex = 1;
            // 
            // AuthForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.tabAuth);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AuthForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tài khoản";
            this.tabAuth.ResumeLayout(false);
            this.tabLogin.ResumeLayout(false);
            this.tabLogin.PerformLayout();
            this.tabRegister.ResumeLayout(false);
            this.tabRegister.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabAuth;
        private System.Windows.Forms.TabPage tabLogin;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TabPage tabRegister;
        private System.Windows.Forms.Label lblRegRole;
        private System.Windows.Forms.ComboBox cboRole;
        private System.Windows.Forms.Label lblRegUser;
        private System.Windows.Forms.TextBox txtRegUser;
        private System.Windows.Forms.Label lblRegPass;
        private System.Windows.Forms.TextBox txtRegPass;
        private System.Windows.Forms.Label lblRegPass2;
        private System.Windows.Forms.TextBox txtRegPass2;
        private System.Windows.Forms.Label lblDriverCode;
        private System.Windows.Forms.TextBox txtDriverCode;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label lblMsg;
    }
}
