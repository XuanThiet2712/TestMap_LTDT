using System;
using System.Windows.Forms;
using TestMap.Services;

namespace TestMap.Forms
{
    // Đăng nhập / đăng ký tài khoản hành khách hoặc tài xế.
    // Giao diện nằm trong AuthForm.Designer.cs (mở bằng Visual Studio để chỉnh).
    public partial class AuthForm : Form
    {
        private readonly string dataRoot;
        private int attempts;

        // Tài khoản vừa đăng nhập thành công (null nếu đóng form)
        public UserAccount Account { get; private set; }

        public AuthForm(string dataRoot)
        {
            InitializeComponent();
            this.dataRoot = dataRoot;
            Shown += (s, e) => txtUser.Focus();
        }

        private void tabAuth_SelectedIndexChanged(object sender, EventArgs e) { lblMsg.Text = ""; }

        private void cboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool driver = cboRole.SelectedIndex == 1;
            lblDriverCode.Visible = txtDriverCode.Visible = driver;
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnLogin_Click(sender, e); }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            UserAccount acc = UserStore.Login(dataRoot, txtUser.Text, txtPass.Text);
            if (acc != null) { Account = acc; DialogResult = DialogResult.OK; return; }

            attempts++;
            txtPass.Clear();
            txtPass.Focus();
            if (attempts >= 5)
            {
                MessageBox.Show("Sai quá 5 lần. Hãy thử lại sau.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                return;
            }
            lblMsg.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            lblMsg.Text = "Sai tên đăng nhập hoặc mật khẩu (" + attempts + "/5).";
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            lblMsg.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            if (txtRegPass.Text != txtRegPass2.Text) { lblMsg.Text = "Hai lần nhập mật khẩu không giống nhau."; return; }

            UserRole role = cboRole.SelectedIndex == 1 ? UserRole.Driver : UserRole.Passenger;
            string err = UserStore.Register(dataRoot, txtRegUser.Text, txtRegPass.Text, role, txtDriverCode.Text);
            if (err != null) { lblMsg.Text = err; return; }

            Account = UserStore.Login(dataRoot, txtRegUser.Text, txtRegPass.Text);   // đăng ký xong vào luôn
            DialogResult = DialogResult.OK;
        }
    }
}
