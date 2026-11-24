using System;
using System.Windows.Forms;

namespace TestMap.Forms
{
    // Bảng mô phỏng: xem thuật toán tìm đường và bộ gợi ý xe buýt chạy TỪNG BƯỚC, chặn đường để thử đi vòng.
    // Giao diện nằm trong SimForm.Designer.cs (mở bằng Visual Studio để chỉnh).
    public partial class SimForm : Form
    {
        private readonly ISimHost host;
        private readonly int[] total = new int[2];     // [0] tìm đường, [1] gợi ý xe buýt
        private readonly int[] cur = new int[2];
        private int playing = -1;                      // kênh đang tự chạy (-1 = không)
        private double acc;
        private BusSimInfo busInfo;
        private bool suppressRound;
        private static readonly double[] StepsPerSecond = { 2, 5, 10, 20, 50, 100, 250, 600 };

        public SimForm(ISimHost host)
        {
            InitializeComponent();
            this.host = host;
            host.SimInvalidated += Host_SimInvalidated;
            SetEnabled(0, false);
            SetEnabled(1, false);
            RefreshBlocked();
        }

        private void SimForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            playTimer.Stop();
            host.SimInvalidated -= Host_SimInvalidated;
            host.SimClose();
        }

        // ----- tiện ích theo kênh -----
        private TrackBar Trk(int ch) { return ch == 0 ? trkAlgo : trkBus; }
        private Label StepLbl(int ch) { return ch == 0 ? lblAlgoStep : lblBusStep; }
        private Label NowLbl(int ch) { return ch == 0 ? lblAlgoNow : lblBusNow; }
        private Button PlayBtn(int ch) { return ch == 0 ? btnAlgoPlay : btnBusPlay; }
        private TrackBar SpeedTrk(int ch) { return ch == 0 ? trkAlgoSpeed : trkBusSpeed; }

        private void SetEnabled(int ch, bool on)
        {
            if (ch == 0) { btnAlgoRestart.Enabled = btnAlgoBack.Enabled = btnAlgoPlay.Enabled = btnAlgoNext.Enabled = btnAlgoEnd.Enabled = on; }
            else { btnBusRestart.Enabled = btnBusBack.Enabled = btnBusPlay.Enabled = btnBusNext.Enabled = btnBusRoundEnd.Enabled = on; }
            Trk(ch).Enabled = on;
        }

        private void Seek(int ch, int step)
        {
            step = Math.Max(0, Math.Min(total[ch], step));
            cur[ch] = step;
            TrackBar t = Trk(ch);
            t.Value = Math.Max(t.Minimum, Math.Min(t.Maximum, step));
            StepLbl(ch).Text = "Bước " + step + " / " + total[ch];
            NowLbl(ch).Text = ch == 0 ? host.AlgoSimSeek(step) : host.BusSimSeek(step);
            if (ch == 1) SelectRoundFor(step);
        }

        private void SetTotal(int ch, int n)
        {
            total[ch] = n;
            cur[ch] = 0;
            Trk(ch).Maximum = Math.Max(0, n);
            Trk(ch).Value = 0;
            StepLbl(ch).Text = "Bước 0 / " + n;
            SetEnabled(ch, n > 0);
        }

        // ----- chạy tự động -----
        private void TogglePlay(int ch)
        {
            if (total[ch] == 0) return;
            if (playing == ch) { StopPlay(); return; }
            StopPlay();
            if (cur[ch] >= total[ch]) Seek(ch, 0);
            playing = ch;
            acc = 0;
            PlayBtn(ch).Text = "⏸ Dừng";
            playTimer.Start();
        }

        private void StopPlay()
        {
            if (playing >= 0) PlayBtn(playing).Text = "▶ Chạy";
            playing = -1;
            playTimer.Stop();
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            if (playing < 0) return;
            acc += StepsPerSecond[Math.Max(1, SpeedTrk(playing).Value) - 1] * playTimer.Interval / 1000.0;
            int adv = (int)acc;
            if (adv < 1) return;
            acc -= adv;
            int ch = playing;
            Seek(ch, cur[ch] + adv);
            if (cur[ch] >= total[ch]) StopPlay();
        }

        // ----- tab Tìm đường -----
        private void btnAlgoPrepare_Click(object sender, EventArgs e)
        {
            StopPlay();
            string err;
            AlgoSimInfo info = host.AlgoSimPrepare(out err);
            if (info == null) { SetTotal(0, 0); lblAlgoNow.Text = err; return; }
            SetTotal(0, info.Total);
            lblAlgoNow.Text = info.Intro;
            Seek(0, 0);
        }

        private void trkAlgo_Scroll(object sender, EventArgs e) { StopPlay(); Seek(0, trkAlgo.Value); }
        private void btnAlgoRestart_Click(object sender, EventArgs e) { StopPlay(); Seek(0, 0); }
        private void btnAlgoBack_Click(object sender, EventArgs e) { StopPlay(); Seek(0, cur[0] - 1); }
        private void btnAlgoPlay_Click(object sender, EventArgs e) { TogglePlay(0); }
        private void btnAlgoNext_Click(object sender, EventArgs e) { StopPlay(); Seek(0, cur[0] + 1); }
        private void btnAlgoEnd_Click(object sender, EventArgs e) { StopPlay(); Seek(0, total[0]); }

        // ----- tab Gợi ý xe buýt -----
        private void btnBusPrepare_Click(object sender, EventArgs e)
        {
            StopPlay();
            string err;
            busInfo = host.BusSimPrepare(out err);
            lstRounds.Items.Clear();
            if (busInfo == null) { SetTotal(1, 0); lblBusNow.Text = err; return; }
            foreach (string t in busInfo.RoundTitles) lstRounds.Items.Add(t);
            SetTotal(1, busInfo.Total);
            lblBusNow.Text = busInfo.Intro;
            Seek(1, 0);
        }

        private void trkBus_Scroll(object sender, EventArgs e) { StopPlay(); Seek(1, trkBus.Value); }
        private void btnBusRestart_Click(object sender, EventArgs e) { StopPlay(); Seek(1, 0); }
        private void btnBusBack_Click(object sender, EventArgs e) { StopPlay(); Seek(1, cur[1] - 1); }
        private void btnBusPlay_Click(object sender, EventArgs e) { TogglePlay(1); }
        private void btnBusNext_Click(object sender, EventArgs e) { StopPlay(); Seek(1, cur[1] + 1); }

        // Chọn một lượt trong danh sách: nhảy tới cuối lượt đó để thấy kết quả
        private void lstRounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressRound || busInfo == null) return;
            int i = lstRounds.SelectedIndex;
            if (i < 0 || i >= busInfo.RoundEnds.Count) return;
            StopPlay();
            Seek(1, busInfo.RoundEnds[i]);
        }

        private void btnBusRoundEnd_Click(object sender, EventArgs e)
        {
            if (busInfo == null) return;
            StopPlay();
            int r = RoundOf(cur[1]);
            if (r < 0) r = 0;
            // đang ở cuối lượt này rồi thì sang lượt kế tiếp
            if (cur[1] >= busInfo.RoundEnds[r] && r + 1 < busInfo.RoundEnds.Count) r++;
            Seek(1, busInfo.RoundEnds[r]);
        }

        // Lượt chứa bước step (-1 nếu chưa bắt đầu)
        private int RoundOf(int step)
        {
            if (busInfo == null || step <= 0) return -1;
            int r = -1;
            for (int i = 0; i < busInfo.RoundStarts.Count; i++)
                if (busInfo.RoundStarts[i] < step) r = i;
            return r;
        }

        private void SelectRoundFor(int step)
        {
            int r = RoundOf(step);
            suppressRound = true;
            try { lstRounds.SelectedIndex = r; }
            finally { suppressRound = false; }
        }

        // ----- tab Chặn đường -----
        private void RefreshBlocked()
        {
            lblBlocked.Text = "Đang chặn: " + host.BlockedCount + " đoạn đường";
        }

        private void chkBlockMode_CheckedChanged(object sender, EventArgs e) { host.BlockMode = chkBlockMode.Checked; }
        private void btnBlockOnRoute_Click(object sender, EventArgs e) { host.BlockOnRoute(); RefreshBlocked(); }
        private void btnBlockRandom_Click(object sender, EventArgs e) { host.BlockRandom(5); RefreshBlocked(); }
        private void btnBlockClear_Click(object sender, EventArgs e) { host.ClearBlocked(); RefreshBlocked(); }
        private void btnSnapshot_Click(object sender, EventArgs e) { host.SaveMapImage(); }

        // Dữ liệu đổi: mô phỏng cũ không còn đúng nữa
        private void Host_SimInvalidated(object sender, EventArgs e)
        {
            StopPlay();
            RefreshBlocked();
            if (total[0] > 0) { SetTotal(0, 0); lblAlgoNow.Text = "Dữ liệu đã đổi (đường bị chặn hoặc đổi điểm đi/đến). Bấm \"Chuẩn bị mô phỏng\" lại."; }
            if (total[1] > 0) { SetTotal(1, 0); lstRounds.Items.Clear(); busInfo = null; lblBusNow.Text = "Dữ liệu đã đổi. Bấm \"Chuẩn bị mô phỏng\" lại."; }
        }
    }
}
