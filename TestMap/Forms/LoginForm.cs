using System;
using System.Windows.Forms;
using TestMap.Services;

namespace TestMap.Forms
{
    // Đăng nhập quyền quản trị. Giao diện nằm trong LoginForm.Designer.cs (mở bằng Visual Studio để chỉnh).
    public partial class LoginForm : Form
    {
        private readonly string dataRoot;
        private int attempts;

        public LoginForm(string dataRoot)
        {
            InitializeComponent();
            this.dataRoot = dataRoot;
            if (AdminAuth.IsDefault(dataRoot))
                lblHint.Text = "Chưa đặt mật khẩu: dùng tài khoản mặc định admin / " + AdminAuth.DefaultPassword +
                               ". Hãy đổi mật khẩu trong màn hình quản trị.";
            Shown += (s, e) => txtPass.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (AdminAuth.Verify(dataRoot, txtUser.Text.Trim(), txtPass.Text))
            {
                DialogResult = DialogResult.OK;
                return;
            }
            attempts++;
            txtPass.Clear();
            txtPass.Focus();
            if (attempts >= 5)
            {
                MessageBox.Show("Sai quá 5 lần. Hãy thử lại sau.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                return;
            }
            lblHint.Text = "Sai tài khoản hoặc mật khẩu (" + attempts + "/5).";
        }
    }
}
