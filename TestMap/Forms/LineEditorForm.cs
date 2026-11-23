using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TestMap.Routing;

namespace TestMap.Forms
{
    // Thêm / sửa một tuyến xe: số tuyến, giá vé, tần suất, màu và danh sách trạm theo thứ tự.
    // Giao diện nằm trong LineEditorForm.Designer.cs (mở bằng Visual Studio để chỉnh).
    public partial class LineEditorForm : Form
    {
        private readonly List<BusStop> stops;
        private readonly BusLine line;
        private readonly List<int> order;          // thứ tự các trạm của tuyến (chỉ số trong stops)
        private Color color;

        // Có tạo luôn chiều ngược lại không (chỉ hỏi khi thêm tuyến mới)
        public bool CreateReverse { get { return chkReverse.Visible && chkReverse.Checked; } }

        public LineEditorForm(List<BusStop> stops, BusLine line, bool isNew)
        {
            InitializeComponent();
            this.stops = stops;
            this.line = line;
            order = new List<int>(line.Stops);
            color = line.Color;

            Text = isNew ? "Thêm tuyến xe buýt" : "Sửa tuyến " + line.Ref;
            chkReverse.Visible = isNew;
            txtRef.Text = line.Ref;
            txtName.Text = line.Name;
            numFare.Value = Math.Max(numFare.Minimum, Math.Min(numFare.Maximum, line.Fare));
            numHeadway.Value = Math.Max(numHeadway.Minimum, Math.Min(numHeadway.Maximum, line.HeadwayMin));
            txtHours.Text = line.Hours;
            chkAutoColor.Checked = !line.CustomColor;
            ShowColor();

            // Danh sách trạm có thể thêm (sắp theo tên)
            var all = new List<BusStop>(stops);
            all.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase));
            foreach (BusStop s in all) cboAddStop.Items.Add(s);

            RefreshStops(-1);
        }

        private void ShowColor()
        {
            pnlColor.BackColor = color;
            btnColor.Enabled = !chkAutoColor.Checked;
        }

        private void RefreshStops(int select)
        {
            lstStops.Items.Clear();
            for (int i = 0; i < order.Count; i++) lstStops.Items.Add((i + 1) + ".  " + stops[order[i]].Name);
            if (select >= 0 && select < order.Count) lstStops.SelectedIndex = select;
            lblStopCount.Text = order.Count + " trạm." + (order.Count < 2 ? " Cần ít nhất 2 trạm." : "") +
                                " Gõ tên trạm vào ô phía trên rồi bấm Thêm trạm; dùng Lên / Xuống để đổi thứ tự.";
        }

        private void btnAddStop_Click(object sender, EventArgs e)
        {
            BusStop s = cboAddStop.SelectedItem as BusStop;
            if (s == null)
            {
                MessageBox.Show("Hãy chọn một trạm trong danh sách (gõ vài chữ để lọc).", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int idx = stops.IndexOf(s);
            order.Add(idx);
            RefreshStops(order.Count - 1);
            cboAddStop.SelectedIndex = -1;
            cboAddStop.Text = "";
            cboAddStop.Focus();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int i = lstStops.SelectedIndex;
            if (i < 0) return;
            order.RemoveAt(i);
            RefreshStops(Math.Min(i, order.Count - 1));
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int i = lstStops.SelectedIndex;
            if (i <= 0) return;
            int t = order[i]; order[i] = order[i - 1]; order[i - 1] = t;
            RefreshStops(i - 1);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int i = lstStops.SelectedIndex;
            if (i < 0 || i >= order.Count - 1) return;
            int t = order[i]; order[i] = order[i + 1]; order[i + 1] = t;
            RefreshStops(i + 1);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog { Color = color, FullOpen = true })
                if (dlg.ShowDialog(this) == DialogResult.OK) { color = dlg.Color; ShowColor(); }
        }

        private void chkAutoColor_CheckedChanged(object sender, EventArgs e)
        {
            ShowColor();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string err = null;
            if (txtRef.Text.Trim().Length == 0) err = "Hãy nhập số tuyến (ví dụ 06).";
            else if (order.Count < 2) err = "Tuyến cần ít nhất 2 trạm.";
            else
                for (int i = 1; i < order.Count; i++)
                    if (order[i] == order[i - 1]) { err = "Có hai trạm giống nhau đứng liền nhau (số " + i + " và " + (i + 1) + ")."; break; }
            if (err != null)
            {
                MessageBox.Show(err, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;                                   // giữ hộp thoại mở
            }

            line.Ref = txtRef.Text.Trim();
            line.Name = txtName.Text.Trim();
            line.Fare = (int)numFare.Value;
            line.HeadwayMin = (int)numHeadway.Value;
            line.Hours = txtHours.Text.Trim();
            line.CustomColor = !chkAutoColor.Checked;
            if (line.CustomColor) line.Color = color;
            line.Stops = new List<int>(order);
            DialogResult = DialogResult.OK;
        }
    }
}
