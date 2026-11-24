using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TestMap.Forms
{
    // Sơ đồ mô hình cách đi bằng xe buýt: đồ thị trạm - tuyến - trọng số, chạy từng bước để người xem hiểu
    // vì sao thời gian ra như vậy. Giao diện nằm trong ModelForm.Designer.cs.
    public partial class ModelForm : Form
    {
        private readonly List<ModelData> models;
        private ModelData cur;
        private int step;

        public ModelForm(List<ModelData> models, int selected)
        {
            InitializeComponent();
            this.models = models;
            foreach (ModelData m in models) cboModelItin.Items.Add(m.Title);
            if (models.Count > 0) cboModelItin.SelectedIndex = Math.Max(0, Math.Min(models.Count - 1, selected));
        }

        private void ModelForm_FormClosed(object sender, FormClosedEventArgs e) { modelTimer.Stop(); }

        private static string Clock(double seconds)
        {
            int s = (int)Math.Round(seconds);
            return s < 60 ? s + " giây" : s / 60 + " phút " + s % 60 + " giây";
        }

        private void cboModelItin_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cboModelItin.SelectedIndex;
            if (i < 0 || i >= models.Count) return;
            modelTimer.Stop();
            btnModelPlay.Text = "▶ Chạy";
            cur = models[i];
            canvas.Data = cur;

            lstModelLegs.Items.Clear();
            for (int k = 0; k < cur.Legs.Count; k++)
                lstModelLegs.Items.Add((k + 1) + ".  " + cur.Legs[k].Text + "   —   " + Clock(cur.Legs[k].Seconds));
            lstModelLegs.Items.Add("");
            lstModelLegs.Items.Add("Tổng cộng:  " + (int)Math.Round(cur.TotalSeconds) + " giây  ≈  " + Math.Max(1, (int)Math.Round(cur.TotalSeconds / 60.0)) + " phút");

            trkModel.Maximum = Math.Max(0, cur.Legs.Count);
            trkModel.Enabled = cur.Legs.Count > 0;
            Seek(cur.Legs.Count);          // mở ra là thấy luôn kết quả; bấm ⟲ để xem lại từ đầu
        }

        private void Seek(int s)
        {
            if (cur == null) return;
            step = Math.Max(0, Math.Min(cur.Legs.Count, s));
            trkModel.Value = Math.Min(trkModel.Maximum, step);
            canvas.Step = step;
            canvas.Invalidate();
            lblModelStep.Text = "Bước " + step + " / " + cur.Legs.Count;

            var sb = new StringBuilder();
            if (step == 0)
            {
                sb.Append("Bấm \"▶ Chạy\" để xem từng bước.\r\n\r\n")
                  .Append("Mỗi đoạn trên sơ đồ có một TRỌNG SỐ là thời gian (giây). Thời gian của cách đi = cộng các trọng số trên đường đi từ A tới B (cả thời gian chờ xe).");
            }
            else
            {
                ModelLeg l = cur.Legs[step - 1];
                sb.Append("Bước ").Append(step).Append(": ").Append(l.Text).Append("\r\n");
                sb.Append("Trọng số: ").Append((int)Math.Round(l.Seconds)).Append(" giây");
                if (l.Note.Length > 0) sb.Append("  (").Append(l.Note).Append(")");
                sb.Append("\r\n\r\nĐã cộng:  ");
                double sum = 0;
                for (int i = 0; i < step; i++)
                {
                    sum += cur.Legs[i].Seconds;
                    if (i > 0) sb.Append(" + ");
                    sb.Append((int)Math.Round(cur.Legs[i].Seconds));
                }
                sb.Append("  =  ").Append((int)Math.Round(sum)).Append(" giây");
                if (step == cur.Legs.Count)
                    sb.Append("\r\n\r\n→ Tổng thời gian ≈ ").Append(Math.Max(1, (int)Math.Round(sum / 60.0))).Append(" phút. Đó là thời gian ước tính của cách đi này.");
            }
            lblModelExplain.Text = sb.ToString();
            if (step > 0 && step <= lstModelLegs.Items.Count) lstModelLegs.TopIndex = Math.Max(0, step - 4);
        }

        private void trkModel_Scroll(object sender, EventArgs e) { Stop(); Seek(trkModel.Value); }
        private void btnModelRestart_Click(object sender, EventArgs e) { Stop(); Seek(0); }
        private void btnModelBack_Click(object sender, EventArgs e) { Stop(); Seek(step - 1); }
        private void btnModelNext_Click(object sender, EventArgs e) { Stop(); Seek(step + 1); }

        private void btnModelPlay_Click(object sender, EventArgs e)
        {
            if (cur == null || cur.Legs.Count == 0) return;
            if (modelTimer.Enabled) { Stop(); return; }
            if (step >= cur.Legs.Count) Seek(0);
            btnModelPlay.Text = "⏸ Dừng";
            modelTimer.Start();
        }

        private void Stop() { modelTimer.Stop(); btnModelPlay.Text = "▶ Chạy"; }

        private void modelTimer_Tick(object sender, EventArgs e)
        {
            if (cur == null) { Stop(); return; }
            Seek(step + 1);
            if (step >= cur.Legs.Count) Stop();
        }

        private void chkModelWeights_CheckedChanged(object sender, EventArgs e)
        {
            canvas.ShowWeights = chkModelWeights.Checked;
            canvas.Invalidate();
        }
    }
}
