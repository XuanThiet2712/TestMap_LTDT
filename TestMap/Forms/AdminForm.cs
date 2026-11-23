using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TestMap.Routing;
using TestMap.Services;

namespace TestMap.Forms
{
    // Màn hình quản trị: thêm / sửa / xóa trạm và tuyến xe, lưu dữ liệu, đổi mật khẩu.
    // Giao diện nằm trong AdminForm.Designer.cs (mở bằng Visual Studio để kéo thả chỉnh sửa).
    //
    // Admin làm việc trên BẢN SAO của trạm và tuyến. Chỉ khi bấm "Lưu và áp dụng" thì dữ liệu mới ghi ra file
    // (stops.csv và lines_admin.csv trong thư mục dữ liệu của khu vực) và cửa sổ chính nạp lại.
    public partial class AdminForm : Form
    {
        private readonly IAdminHost host;
        private List<BusStop> stops = new List<BusStop>();
        private List<BusLine> lines = new List<BusLine>();
        private readonly Dictionary<BusLine, BusLine> origin = new Dictionary<BusLine, BusLine>();   // bản sao -> tuyến gốc ở cửa sổ chính
        private bool dirty;
        private bool loading;

        public AdminForm(IAdminHost host)
        {
            InitializeComponent();
            this.host = host;
            host.DataReloaded += Host_DataReloaded;
            FormClosing += AdminForm_FormClosing;
            FormClosed += (s, e) => host.DataReloaded -= Host_DataReloaded;
            LoadFromHost();
        }

        // =====================================================================
        //  NẠP DỮ LIỆU
        // =====================================================================
        private void LoadFromHost()
        {
            stops = AdminData.CloneStops(host.Graph.Stops);
            lines = new List<BusLine>();
            origin.Clear();
            foreach (BusLine l in host.Lines)
            {
                BusLine c = AdminData.CloneLine(l);
                lines.Add(c);
                origin[c] = l;
            }
            dirty = false;
            lblArea.Text = "Khu vực: " + host.AreaName;
            RefreshStops();
            RefreshLines();
            RefreshDataInfo();
            SetStatus("Đã nạp " + stops.Count + " trạm, " + lines.Count + " tuyến.");
        }

        // Cửa sổ chính vừa nạp lại dữ liệu (sau khi lưu, khôi phục hoặc tải tuyến thật)
        private void Host_DataReloaded(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { BeginInvoke(new Action(() => Host_DataReloaded(sender, e))); return; }
            if (!dirty) LoadFromHost();
        }

        private void SetStatus(string text) { lblStatus.Text = text; }

        private void MarkDirty()
        {
            dirty = true;
            SetStatus("Có thay đổi chưa lưu. Vào tab \"Cập nhật dữ liệu\" và bấm \"Lưu và áp dụng thay đổi\".");
            RefreshDataInfo();
        }

        private void RefreshDataInfo()
        {
            lblDataInfo.Text = "Khu vực: " + host.AreaName + "\n" + stops.Count + " trạm, " + lines.Count + " tuyến (mỗi chiều đi / về tính một tuyến).\n" +
                               (host.HasAdminLines ? "Đang dùng tuyến do admin chỉnh sửa (lines_admin.csv)." : "Đang dùng tuyến gốc (chưa có chỉnh sửa nào được lưu).") + "\n" +
                               (dirty ? "CÓ THAY ĐỔI CHƯA LƯU." : "Không có thay đổi chưa lưu.");
        }

        // =====================================================================
        //  TAB TRẠM
        // =====================================================================
        private void RefreshStops()
        {
            int keep = SelectedStop();
            string filter = txtSearchStop.Text.Trim().ToLowerInvariant();
            loading = true;
            dgvStops.Rows.Clear();
            for (int i = 0; i < stops.Count; i++)
            {
                if (filter.Length > 0 && stops[i].Name.ToLowerInvariant().IndexOf(filter, StringComparison.Ordinal) < 0) continue;
                int r = dgvStops.Rows.Add("T" + (i + 1).ToString("000"), stops[i].Name, stops[i].Node, AdminData.LinesUsing(lines, i));
                dgvStops.Rows[r].Tag = i;
                if (i == keep) dgvStops.Rows[r].Selected = true;
            }
            loading = false;
        }

        // Chỉ số trạm đang chọn trong bảng, -1 nếu chưa chọn
        private int SelectedStop()
        {
            if (dgvStops.SelectedRows.Count == 0) return -1;
            object tag = dgvStops.SelectedRows[0].Tag;
            return tag is int ? (int)tag : -1;
        }

        private void txtSearchStop_TextChanged(object sender, EventArgs e)
        {
            RefreshStops();
        }

        private void dgvStops_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnStopEdit_Click(sender, e);
        }

        // Thêm trạm: ẩn màn hình này, chờ người dùng bấm một điểm trên bản đồ chính
        private void btnStopAdd_Click(object sender, EventArgs e)
        {
            Hide();
            host.RequestMapPick("QUẢN TRỊ: bấm lên bản đồ (gần một ngã tư) để đặt trạm mới",
                (lat, lng) =>
                {
                    Show();
                    Activate();
                    double snap;
                    int node = host.Graph.Nearest(lat, lng);
                    snap = RoadGraph.Distance(new LatLng(lat, lng), host.Graph.Nodes[node]);
                    using (var dlg = new StopEditorForm(true, "", lat, lng, snap))
                    {
                        if (dlg.ShowDialog(this) != DialogResult.OK) return;
                        AdminData.AddStop(host.Graph, stops, dlg.StopName, lat, lng, out snap);
                    }
                    RefreshStops();
                    MarkDirty();
                },
                () => { Show(); Activate(); });
        }

        private void btnStopEdit_Click(object sender, EventArgs e)
        {
            int i = SelectedStop();
            if (i < 0) { SetStatus("Hãy chọn một trạm trong bảng."); return; }
            using (var dlg = new StopEditorForm(false, stops[i].Name, stops[i].RawLat, stops[i].RawLng, -1))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                stops[i].Name = dlg.StopName;
            }
            RefreshStops();
            MarkDirty();
        }

        private void btnStopMove_Click(object sender, EventArgs e)
        {
            int i = SelectedStop();
            if (i < 0) { SetStatus("Hãy chọn một trạm trong bảng."); return; }
            Hide();
            host.RequestMapPick("QUẢN TRỊ: bấm lên bản đồ để dời trạm \"" + stops[i].Name + "\" tới vị trí mới",
                (lat, lng) =>
                {
                    Show();
                    Activate();
                    double snap = AdminData.MoveStop(host.Graph, stops, i, lat, lng);
                    RefreshStops();
                    MarkDirty();
                    SetStatus("Đã dời trạm (gắn vào giao lộ gần nhất, cách " + Math.Round(snap) + " m).");
                },
                () => { Show(); Activate(); });
        }

        private void btnStopDelete_Click(object sender, EventArgs e)
        {
            int i = SelectedStop();
            if (i < 0) { SetStatus("Hãy chọn một trạm trong bảng."); return; }
            int used = AdminData.LinesUsing(lines, i);
            string msg = "Xóa trạm \"" + stops[i].Name + "\"?" +
                         (used > 0 ? "\n\n" + used + " tuyến đang đi qua trạm này sẽ bị bỏ trạm này (tuyến còn dưới 2 trạm sẽ bị xóa)." : "");
            if (MessageBox.Show(msg, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            // bỏ các tuyến sẽ bị xóa khỏi bảng ánh xạ tuyến gốc
            int before = lines.Count;
            AdminData.RemoveStop(stops, lines, i);
            if (lines.Count != before) SetStatus("Đã xóa trạm và " + (before - lines.Count) + " tuyến còn dưới 2 trạm.");
            RefreshStops();
            RefreshLines();
            MarkDirty();
        }

        // =====================================================================
        //  TAB TUYẾN
        // =====================================================================
        private void RefreshLines()
        {
            int keep = SelectedLineIndex();
            loading = true;
            dgvLines.Rows.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                BusLine l = lines[i];
                string title = stops[l.Stops[0]].Name + " → " + stops[l.Stops[l.Stops.Count - 1]].Name;
                int r = dgvLines.Rows.Add(l.Ref, title, l.Stops.Count, (l.Fare / 1000.0).ToString("0.#") + "k", l.HeadwayMin, l.Hours);
                dgvLines.Rows[r].Tag = i;
                dgvLines.Rows[r].Cells[0].Style.BackColor = l.Color;
                dgvLines.Rows[r].Cells[0].Style.ForeColor = System.Drawing.Color.White;
                dgvLines.Rows[r].Cells[0].Style.SelectionBackColor = l.Color;
                dgvLines.Rows[r].Cells[0].Style.SelectionForeColor = System.Drawing.Color.White;
                if (i == keep) dgvLines.Rows[r].Selected = true;
            }
            loading = false;
        }

        private int SelectedLineIndex()
        {
            if (dgvLines.SelectedRows.Count == 0) return -1;
            object tag = dgvLines.SelectedRows[0].Tag;
            return tag is int ? (int)tag : -1;
        }

        // Chọn một dòng thì hiện luôn tuyến đó trên bản đồ chính (nếu tuyến đã được lưu)
        private void dgvLines_SelectionChanged(object sender, EventArgs e)
        {
            if (loading) return;
            int i = SelectedLineIndex();
            if (i < 0 || i >= lines.Count) return;
            BusLine o;
            if (origin.TryGetValue(lines[i], out o) && !dirty) host.PreviewLine(o);
        }

        private void dgvLines_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnLineEdit_Click(sender, e);
        }

        private void btnLineAdd_Click(object sender, EventArgs e)
        {
            if (stops.Count < 2) { SetStatus("Cần có ít nhất 2 trạm trước khi thêm tuyến."); return; }
            var line = new BusLine { Fare = 7000, HeadwayMin = 10 };
            using (var dlg = new LineEditorForm(stops, line, true))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                lines.Add(line);
                if (dlg.CreateReverse) lines.Add(AdminData.Reverse(line));
            }
            RefreshLines();
            MarkDirty();
        }

        private void btnLineEdit_Click(object sender, EventArgs e)
        {
            int i = SelectedLineIndex();
            if (i < 0) { SetStatus("Hãy chọn một tuyến trong bảng."); return; }
            BusLine work = AdminData.CloneLine(lines[i]);          // sửa trên bản nháp, bấm Hủy thì không mất gì
            using (var dlg = new LineEditorForm(stops, work, false))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
            }
            BusLine o;
            if (origin.TryGetValue(lines[i], out o)) { origin.Remove(lines[i]); }
            lines[i] = work;
            RefreshLines();
            MarkDirty();
        }

        private void btnLineReverse_Click(object sender, EventArgs e)
        {
            int i = SelectedLineIndex();
            if (i < 0) { SetStatus("Hãy chọn một tuyến trong bảng."); return; }
            lines.Insert(i + 1, AdminData.Reverse(lines[i]));
            RefreshLines();
            MarkDirty();
            SetStatus("Đã tạo chiều ngược lại của tuyến " + lines[i].Ref + " (đảo thứ tự các trạm).");
        }

        private void btnLineDelete_Click(object sender, EventArgs e)
        {
            int i = SelectedLineIndex();
            if (i < 0) { SetStatus("Hãy chọn một tuyến trong bảng."); return; }
            if (MessageBox.Show("Xóa tuyến " + lines[i].Ref + " (" + stops[lines[i].Stops[0]].Name + " → " +
                                stops[lines[i].Stops[lines[i].Stops.Count - 1]].Name + ")?",
                                Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            lines.RemoveAt(i);
            RefreshLines();
            MarkDirty();
        }

        // =====================================================================
        //  TAB CẬP NHẬT DỮ LIỆU
        // =====================================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            // kiểm tra trước khi ghi
            foreach (BusLine l in lines)
                if (l.Ref.Trim().Length == 0 || l.Stops.Count < 2)
                {
                    MessageBox.Show("Có tuyến thiếu số tuyến hoặc có dưới 2 trạm. Hãy sửa lại trước khi lưu.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            try
            {
                Directory.CreateDirectory(host.DataDir);
                AdminData.SaveStops(stops, Path.Combine(host.DataDir, "stops.csv"));
                AdminData.SaveLines(stops, lines, Path.Combine(host.DataDir, "lines_admin.csv"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dirty = false;
            SetStatus("Đã lưu " + stops.Count + " trạm, " + lines.Count + " tuyến. Đang nạp lại dữ liệu...");
            host.ApplyChanges();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bỏ toàn bộ chỉnh sửa tuyến và quay về tuyến gốc (tuyến thật nếu đã tải, hoặc tuyến demo)?\n\n" +
                                "Các trạm bạn đã thêm hoặc đổi tên vẫn được giữ.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            dirty = false;
            host.RestoreOriginalLines();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            host.DownloadRealLines();
            SetStatus("Đang tải tuyến thật từ OpenStreetMap, xem tiến trình ở thanh dưới cửa sổ chính.");
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            if (!AdminAuth.Verify(host.DataRoot, AdminAuth.User, txtOldPass.Text))
            {
                MessageBox.Show("Mật khẩu cũ không đúng.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtNewPass.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới cần ít nhất 6 ký tự.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtNewPass.Text != txtNewPass2.Text)
            {
                MessageBox.Show("Hai lần nhập mật khẩu mới không giống nhau.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            AdminAuth.SetPassword(host.DataRoot, txtNewPass.Text);
            txtOldPass.Clear(); txtNewPass.Clear(); txtNewPass2.Clear();
            SetStatus("Đã đổi mật khẩu quản trị.");
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            host.Logout();
            Close();
        }

        // =====================================================================
        //  ĐÓNG
        // =====================================================================
        private void AdminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!dirty) return;
            DialogResult r = MessageBox.Show("Còn thay đổi chưa lưu. Lưu trước khi đóng?", Text,
                                             MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (r == DialogResult.Cancel) { e.Cancel = true; return; }
            if (r == DialogResult.Yes)
            {
                btnSave_Click(this, EventArgs.Empty);
                if (dirty) e.Cancel = true;            // lưu thất bại thì không đóng
            }
            else
            {
                dirty = false;                         // bỏ thay đổi: dữ liệu chính chưa bị đụng tới nên không cần nạp lại
            }
        }
    }
}
