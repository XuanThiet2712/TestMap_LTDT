using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestMap.Forms
{
    // Thêm / sửa tên một trạm xe buýt
    public partial class StopEditorForm : Form
    {
        public string StopName { get { return txtName.Text.Trim(); } }

        // snapMeters < 0: không hiển thị thông tin vị trí (chỉ đổi tên)
        public StopEditorForm(bool isNew, string name, double lat, double lng, double snapMeters)
        {
            InitializeComponent();
            Text = isNew ? "Thêm trạm xe buýt" : "Sửa trạm xe buýt";
            txtName.Text = name;
            if (snapMeters < 0)
            {
                lblPos.Visible = lblSnap.Visible = false;
            }
            else
            {
                lblPos.Text = "Vị trí: " + lat.ToString("F6") + ", " + lng.ToString("F6");
                bool far = snapMeters > 40;
                lblSnap.ForeColor = far ? Color.FromArgb(180, 83, 9) : Color.FromArgb(22, 101, 52);
                lblSnap.Text = "Trạm sẽ được gắn vào giao lộ gần nhất, cách " + Math.Round(snapMeters) + " m." +
                               (far ? " Khá xa: xe sẽ \"dừng\" ở giao lộ đó. Bấm Hủy rồi chọn lại điểm gần ngã tư hơn nếu cần." : "");
            }
            Shown += (s, e) => { txtName.Focus(); txtName.SelectAll(); };
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (StopName.Length == 0)
            {
                MessageBox.Show("Hãy nhập tên trạm.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.None;      // giữ hộp thoại mở
            }
        }
    }
}
