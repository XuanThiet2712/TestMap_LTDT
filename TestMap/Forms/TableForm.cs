using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TestMap.Forms
{
    // Bảng Dijkstra / A* từng bước (giống bảng trong sách): mỗi dòng là trạng thái sau khi chốt thêm một đỉnh.
    // Bấm "Chạy" thì bảng đầy dần, có lời giải thích từng phép cộng, và các đỉnh đã chốt sáng lên trên bản đồ.
    // Phím tắt: ← → lùi / tới một bước, Space chạy / dừng, Home / End về đầu / cuối.
    // Giao diện nằm trong TableForm.Designer.cs.
    public partial class TableForm : Form
    {
        private readonly List<GraphModel> models;
        private readonly Action<int, HashSet<int>, int, int, string> onStep;   // (loại đồ thị, đỉnh đã chốt, đỉnh đang xét, đỉnh đích hoặc -1, chữ "h = ..."); finalized = null: xóa lớp vẽ
        private GraphModel model;
        private int kind;
        private TableResult result;
        private int step;
        private bool loading;
        private bool aStar;

        private static readonly Color Gray = Color.FromArgb(148, 163, 184);

        public TableForm(List<GraphModel> models, int kind, int algo, Action<int, HashSet<int>, int, int, string> onStep)
        {
            InitializeComponent();
            this.models = models;
            this.onStep = onStep;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 34;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            grid.DefaultCellStyle.SelectionBackColor = grid.DefaultCellStyle.BackColor;
            grid.DefaultCellStyle.SelectionForeColor = grid.DefaultCellStyle.ForeColor;
            grid.RowTemplate.Height = 30;
            grid.GridColor = Color.FromArgb(226, 232, 240);

            loading = true;
            cboTableAlgo.SelectedIndex = algo == 1 ? 1 : 0;
            loading = false;
            foreach (GraphModel m in models) cboTableGraph.Items.Add(m.Name);
            cboTableGraph.SelectedIndex = Math.Max(0, Math.Min(models.Count - 1, kind));
        }

        private void TableForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            tableTimer.Stop();
            onStep(0, null, -1, -1, "");
        }

        // Đổi loại đồ thị (đường / xe buýt)
        private void cboTableGraph_SelectedIndexChanged(object sender, EventArgs e)
        {
            kind = cboTableGraph.SelectedIndex;
            if (kind < 0) return;
            model = models[kind];
            tableTimer.Stop();
            btnTablePlay.Text = "▶ Chạy";

            loading = true;
            cboTableSrc.Items.Clear();
            cboTableDst.Items.Clear();
            lstTableNodes.Items.Clear();
            grid.Columns.Clear();
            grid.Rows.Clear();
            result = null;

            if (model.Error != null)
            {
                loading = false;
                lblTableExplain.Text = model.Error;
                lblTableStep.Text = "Bước 0 / 0";
                trkTable.Enabled = false;
                return;
            }

            foreach (string l in model.Labels)
            {
                cboTableSrc.Items.Add(l);
                cboTableDst.Items.Add(l);
                lstTableNodes.Items.Add(l);
            }
            cboTableSrc.SelectedIndex = Math.Max(0, Math.Min(model.Labels.Count - 1, model.DefaultSrc));
            cboTableDst.SelectedIndex = Math.Max(0, Math.Min(model.Labels.Count - 1, model.DefaultDst));
            loading = false;
            Build();
        }

        private void btnTableBuild_Click(object sender, EventArgs e) { Build(); }

        private void Build()
        {
            if (loading || model == null || model.Error != null) return;
            int src = cboTableSrc.SelectedIndex, dst = cboTableDst.SelectedIndex;
            if (src < 0 || dst < 0) return;
            if (src == dst) { lblTableExplain.Text = "Điểm xuất phát và đích đang trùng nhau. Hãy chọn hai đỉnh khác nhau."; return; }

            aStar = cboTableAlgo.SelectedIndex == 1;
            if (aStar && model.H == null)
            {
                aStar = false;
                loading = true; cboTableAlgo.SelectedIndex = 0; loading = false;
            }

            tableTimer.Stop();
            btnTablePlay.Text = "▶ Chạy";
            result = aStar ? AStarTable.Run(model, src, dst) : DijkstraTable.Run(model, src, dst);

            lblTableLegend.Text = aStar
                ? "Ô = (f, đỉnh đứng trước) với f = g + h.  Dòng \"h\" = ước lượng đường chim bay còn lại tới đích.  Xanh đậm = đỉnh f nhỏ nhất, được chốt tiếp.  Vàng = vừa cập nhật.  \"–\" = đã chốt.  Bản đồ: chấm cam = đã chốt, chấm xanh = đang xét, nét đứt tím = đường chim bay h."
                : "Ô = (khoảng cách ngắn nhất hiện biết, đỉnh đứng trước).  Xanh đậm = đỉnh nhỏ nhất, được chốt tiếp.  Vàng = vừa cập nhật.  \"–\" = đã chốt.  Bản đồ: chấm cam = đã chốt, chấm xanh = đang xét.";

            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Bước", Width = 56, SortMode = DataGridViewColumnSortMode.NotSortable });
            for (int i = 0; i < model.Labels.Count; i++)
            {
                var c = new DataGridViewTextBoxColumn { HeaderText = i.ToString(), Width = 90, SortMode = DataGridViewColumnSortMode.NotSortable, ToolTipText = model.Labels[i] };
                if (i == src) c.HeaderCell.Style.BackColor = Color.FromArgb(22, 163, 74);
                else if (i == dst) c.HeaderCell.Style.BackColor = Color.FromArgb(220, 38, 38);
                grid.Columns.Add(c);
            }

            trkTable.Maximum = result.Rows.Count - 1;
            trkTable.Enabled = true;
            Seek(result.Rows.Count - 1);       // mở ra là thấy luôn bảng đầy đủ; bấm ⟲ để xem lại từ đầu
        }

        // Nội dung một ô của bảng
        private string Cell(TableRow r, int i, int src)
        {
            if (r.Final[i]) return "–";
            if (double.IsInfinity(r.Dist[i])) return "(∞, –)";
            string prev = r.Prev[i] < 0 ? "–" : r.Prev[i].ToString();
            if (aStar) return "(" + model.Fmt(r.F[i]) + ", " + prev + ")";
            if (r.Relaxed < 0 && i == src) return "0";
            return "(" + model.Fmt(r.Dist[i]) + ", " + prev + ")";
        }

        private void Seek(int s)
        {
            if (result == null) return;
            step = Math.Max(0, Math.Min(result.Rows.Count - 1, s));
            trkTable.Value = Math.Min(trkTable.Maximum, step);
            int src = cboTableSrc.SelectedIndex;
            int n = model.Labels.Count;

            grid.Rows.Clear();
            if (aStar)
            {
                // dòng "h": ước lượng chim bay của mọi đỉnh tới đích
                var hc = new object[n + 1];
                hc[0] = "h";
                for (int i = 0; i < n; i++) hc[i + 1] = model.Fmt(result.H[i]);
                int hi = grid.Rows.Add(hc);
                foreach (DataGridViewCell c in grid.Rows[hi].Cells)
                {
                    c.Style.BackColor = Color.FromArgb(224, 231, 255);
                    c.Style.ForeColor = Color.FromArgb(67, 56, 202);
                    c.Style.Font = new Font("Segoe UI", 10f, FontStyle.Italic | FontStyle.Bold);
                }
            }

            for (int k = 0; k <= step; k++)
            {
                TableRow r = result.Rows[k];
                var cells = new object[n + 1];
                cells[0] = k;
                for (int i = 0; i < n; i++) cells[i + 1] = Cell(r, i, src);
                DataGridViewRow row = grid.Rows[grid.Rows.Add(cells)];
                row.Cells[0].Style.ForeColor = Gray;
                for (int i = 0; i < n; i++)
                {
                    DataGridViewCell cell = row.Cells[i + 1];
                    if (r.Final[i]) cell.Style.ForeColor = Gray;
                    if (r.Changed[i]) cell.Style.BackColor = Color.FromArgb(254, 249, 195);               // vừa cập nhật
                    if (r.Chosen == i)
                    {
                        cell.Style.BackColor = Color.FromArgb(187, 247, 208);                             // sắp được chốt
                        cell.Style.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                    }
                }
            }
            if (grid.Rows.Count > 0) grid.FirstDisplayedScrollingRowIndex = grid.Rows.Count - 1;

            lblTableStep.Text = (aStar ? "A*" : "Dijkstra") + "  ·  Bước " + step + " / " + (result.Rows.Count - 1) + "      (đơn vị trọng số: " + model.UnitName + ")";
            lblTableExplain.Text = result.Rows[step].Explain;

            // báo cho bản đồ: những đỉnh đã chốt + đỉnh đang xét (+ đường chim bay h tới đích nếu là A*)
            TableRow cur = result.Rows[step];
            var fin = new HashSet<int>();
            for (int i = 0; i < cur.Final.Length; i++) if (cur.Final[i]) fin.Add(i);
            int now = cur.Chosen >= 0 ? cur.Chosen : cur.Relaxed;
            int goal = aStar ? cboTableDst.SelectedIndex : -1;
            string hText = aStar && now >= 0 ? "h = " + model.Fmt(result.H[now]) + " " + model.UnitName : "";
            onStep(kind, fin, now, goal, hText);
        }

        // ----- phát / tua -----
        private void trkTable_Scroll(object sender, EventArgs e) { StopPlay(); Seek(trkTable.Value); }
        private void btnTableRestart_Click(object sender, EventArgs e) { StopPlay(); Seek(0); }
        private void btnTableBack_Click(object sender, EventArgs e) { StopPlay(); Seek(step - 1); }
        private void btnTableNext_Click(object sender, EventArgs e) { StopPlay(); Seek(step + 1); }
        private void btnTableEnd_Click(object sender, EventArgs e) { StopPlay(); if (result != null) Seek(result.Rows.Count - 1); }
        private void btnTablePlay_Click(object sender, EventArgs e) { TogglePlay(); }

        private void TogglePlay()
        {
            if (result == null) return;
            if (tableTimer.Enabled) { StopPlay(); return; }
            if (step >= result.Rows.Count - 1) Seek(0);
            btnTablePlay.Text = "⏸ Dừng";
            tableTimer.Start();
        }

        private void StopPlay() { tableTimer.Stop(); btnTablePlay.Text = "▶ Chạy"; }

        private void tableTimer_Tick(object sender, EventArgs e)
        {
            if (result == null) { StopPlay(); return; }
            Seek(step + 1);
            if (step >= result.Rows.Count - 1) StopPlay();
        }

        private void TableForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (ActiveControl is ComboBox || result == null) return;
            switch (e.KeyCode)
            {
                case Keys.Right: StopPlay(); Seek(step + 1); break;
                case Keys.Left: StopPlay(); Seek(step - 1); break;
                case Keys.Home: StopPlay(); Seek(0); break;
                case Keys.End: StopPlay(); Seek(result.Rows.Count - 1); break;
                case Keys.Space: TogglePlay(); break;
                default: return;
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        // ----- xuất bảng -----
        // Bảng đầy đủ dạng văn bản (cột cách nhau bằng Tab): dán thẳng vào Word / Excel
        private string FullTableText()
        {
            int src = cboTableSrc.SelectedIndex, n = model.Labels.Count;
            var sb = new StringBuilder();
            sb.Append("Bước");
            for (int i = 0; i < n; i++) sb.Append('\t').Append(i);
            sb.AppendLine();
            if (aStar)
            {
                sb.Append("h");
                for (int i = 0; i < n; i++) sb.Append('\t').Append(model.Fmt(result.H[i]));
                sb.AppendLine();
            }
            for (int k = 0; k < result.Rows.Count; k++)
            {
                sb.Append(k);
                for (int i = 0; i < n; i++) sb.Append('\t').Append(Cell(result.Rows[k], i, src));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void btnTableCopy_Click(object sender, EventArgs e)
        {
            if (result == null) return;
            try
            {
                Clipboard.SetText(FullTableText());
                lblTableLegend.Text = "Đã sao chép bảng đầy đủ vào clipboard. Dán vào Word / Excel là thành bảng.";
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                lblTableLegend.Text = "Không sao chép được (clipboard đang bị chương trình khác giữ). Thử lại sau.";
            }
        }

        // Lưu bảng + lời giải thích từng bước thành trang HTML để chèn vào báo cáo / mở bằng trình duyệt
        private void btnTableSave_Click(object sender, EventArgs e)
        {
            if (result == null) return;
            using (var dlg = new SaveFileDialog { Filter = "Trang HTML|*.html", FileName = (aStar ? "bang-astar" : "bang-dijkstra") + ".html", Title = "Lưu bảng" })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    File.WriteAllText(dlg.FileName, BuildHtml(), new UTF8Encoding(true));
                    lblTableLegend.Text = "Đã lưu: " + dlg.FileName;
                }
                catch (Exception ex) { lblTableLegend.Text = "Không lưu được: " + ex.Message; }
            }
        }

        private string BuildHtml()
        {
            int src = cboTableSrc.SelectedIndex, dst = cboTableDst.SelectedIndex, n = model.Labels.Count;
            Func<string, string> H = WebUtility.HtmlEncode;
            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html lang=\"vi\"><head><meta charset=\"utf-8\"><title>").Append(aStar ? "Bảng A*" : "Bảng Dijkstra").Append("</title><style>")
              .Append("body{font-family:Segoe UI,Arial,sans-serif;margin:24px;color:#0f172a}h1{font-size:20px}table{border-collapse:collapse;margin:12px 0}")
              .Append("th{background:#0f172a;color:#fff;padding:6px 10px}td{border:1px solid #e2e8f0;padding:6px 10px;text-align:center;min-width:56px}")
              .Append(".h td{background:#e0e7ff;color:#4338ca;font-weight:bold;font-style:italic}.fin{color:#94a3b8}.chg{background:#fef9c3}.pick{background:#bbf7d0;font-weight:bold}")
              .Append("pre{background:#f1f5f9;padding:10px;border-radius:8px;white-space:pre-wrap}</style></head><body>");
            sb.Append("<h1>").Append(aStar ? "Thuật toán A*" : "Thuật toán Dijkstra").Append(": đỉnh ").Append(src).Append(" → đỉnh ").Append(dst).Append("</h1>");
            sb.Append("<p>Đồ thị: ").Append(H(model.Name)).Append(". Đơn vị trọng số: ").Append(H(model.UnitName)).Append(".</p><ul>");
            foreach (string l in model.Labels) sb.Append("<li>").Append(H(l)).Append("</li>");
            sb.Append("</ul><table><tr><th>Bước</th>");
            for (int i = 0; i < n; i++) sb.Append("<th>").Append(i).Append("</th>");
            sb.Append("</tr>");
            if (aStar)
            {
                sb.Append("<tr class=\"h\"><td>h</td>");
                for (int i = 0; i < n; i++) sb.Append("<td>").Append(model.Fmt(result.H[i])).Append("</td>");
                sb.Append("</tr>");
            }
            for (int k = 0; k < result.Rows.Count; k++)
            {
                TableRow r = result.Rows[k];
                sb.Append("<tr><td>").Append(k).Append("</td>");
                for (int i = 0; i < n; i++)
                {
                    string cls = r.Chosen == i ? "pick" : (r.Changed[i] ? "chg" : (r.Final[i] ? "fin" : ""));
                    sb.Append("<td class=\"").Append(cls).Append("\">").Append(H(Cell(r, i, src))).Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table><h2>Giải thích từng bước</h2>");
            for (int k = 0; k < result.Rows.Count; k++)
                sb.Append("<h3>Bước ").Append(k).Append("</h3><pre>").Append(H(result.Rows[k].Explain)).Append("</pre>");
            sb.Append("</body></html>");
            return sb.ToString();
        }
    }
}
