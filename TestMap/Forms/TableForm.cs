using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TestMap.Forms
{
    // Bảng Dijkstra từng bước (giống bảng trong sách): mỗi dòng là trạng thái sau khi chốt thêm một đỉnh.
    // Vừa bấm "Chạy" vừa thấy bảng đầy dần, lời giải thích từng phép cộng, và các đỉnh đã chốt sáng lên trên bản đồ.
    // Giao diện nằm trong TableForm.Designer.cs.
    public partial class TableForm : Form
    {
        private readonly List<GraphModel> models;
        private readonly Action<int, HashSet<int>, int> onStep;      // (loại đồ thị, các đỉnh đã chốt, đỉnh đang xét); null = xóa lớp vẽ
        private GraphModel model;
        private int kind;
        private TableResult result;
        private int step;
        private bool loading;

        private static readonly Color Gray = Color.FromArgb(148, 163, 184);

        public TableForm(List<GraphModel> models, int kind, Action<int, HashSet<int>, int> onStep)
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

            foreach (GraphModel m in models) cboTableGraph.Items.Add(m.Name);
            cboTableGraph.SelectedIndex = Math.Max(0, Math.Min(models.Count - 1, kind));
        }

        private void TableForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            tableTimer.Stop();
            onStep(0, null, -1);
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

            tableTimer.Stop();
            btnTablePlay.Text = "▶ Chạy";
            result = DijkstraTable.Run(model, src, dst);

            grid.Columns.Clear();
            var step0 = new DataGridViewTextBoxColumn { HeaderText = "Bước", Width = 56, SortMode = DataGridViewColumnSortMode.NotSortable };
            grid.Columns.Add(step0);
            for (int i = 0; i < model.Labels.Count; i++)
            {
                var c = new DataGridViewTextBoxColumn { HeaderText = i.ToString(), Width = 86, SortMode = DataGridViewColumnSortMode.NotSortable, ToolTipText = model.Labels[i] };
                if (i == src) { c.HeaderCell.Style.BackColor = Color.FromArgb(22, 163, 74); }
                else if (i == dst) { c.HeaderCell.Style.BackColor = Color.FromArgb(220, 38, 38); }
                grid.Columns.Add(c);
            }

            trkTable.Maximum = result.Rows.Count - 1;
            trkTable.Enabled = true;
            Seek(result.Rows.Count - 1);       // mở ra là thấy luôn bảng đầy đủ; bấm ⟲ để xem lại từ đầu
        }

        private string Cell(TableRow r, int i, int src)
        {
            if (r.Final[i]) return "–";
            if (double.IsInfinity(r.Dist[i])) return "(∞, –)";
            if (r.Relaxed < 0 && i == src) return "0";
            return "(" + model.Fmt(r.Dist[i]) + ", " + (r.Prev[i] < 0 ? "–" : r.Prev[i].ToString()) + ")";
        }

        private void Seek(int s)
        {
            if (result == null) return;
            step = Math.Max(0, Math.Min(result.Rows.Count - 1, s));
            trkTable.Value = Math.Min(trkTable.Maximum, step);
            int src = cboTableSrc.SelectedIndex;

            grid.Rows.Clear();
            for (int k = 0; k <= step; k++)
            {
                TableRow r = result.Rows[k];
                var cells = new object[model.Labels.Count + 1];
                cells[0] = k;
                for (int i = 0; i < model.Labels.Count; i++) cells[i + 1] = Cell(r, i, src);
                int idx = grid.Rows.Add(cells);
                DataGridViewRow row = grid.Rows[idx];
                row.Cells[0].Style.ForeColor = Gray;
                for (int i = 0; i < model.Labels.Count; i++)
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

            lblTableStep.Text = "Bước " + step + " / " + (result.Rows.Count - 1) + "      (đơn vị trọng số: " + model.UnitName + ")";
            lblTableExplain.Text = result.Rows[step].Explain;

            // báo cho bản đồ: những đỉnh đã chốt + đỉnh đang xét
            TableRow cur = result.Rows[step];
            var fin = new HashSet<int>();
            for (int i = 0; i < cur.Final.Length; i++) if (cur.Final[i]) fin.Add(i);
            onStep(kind, fin, cur.Chosen >= 0 ? cur.Chosen : cur.Relaxed);
        }

        private void trkTable_Scroll(object sender, EventArgs e) { StopPlay(); Seek(trkTable.Value); }
        private void btnTableRestart_Click(object sender, EventArgs e) { StopPlay(); Seek(0); }
        private void btnTableBack_Click(object sender, EventArgs e) { StopPlay(); Seek(step - 1); }
        private void btnTableNext_Click(object sender, EventArgs e) { StopPlay(); Seek(step + 1); }
        private void btnTableEnd_Click(object sender, EventArgs e) { StopPlay(); if (result != null) Seek(result.Rows.Count - 1); }

        private void btnTablePlay_Click(object sender, EventArgs e)
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
    }
}
