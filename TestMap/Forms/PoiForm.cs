using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using TestMap.Routing;

namespace TestMap.Forms
{
    // Lọc và tìm địa điểm (bệnh viện, trường học...). Cũng quyết định loại nào được vẽ trên bản đồ.
    // Giao diện nằm trong PoiForm.Designer.cs (mở bằng Visual Studio để chỉnh).
    public partial class PoiForm : Form
    {
        private List<Poi> all = new List<Poi>();
        private readonly List<Poi> shown = new List<Poi>();
        private readonly Action<HashSet<PoiKind>> onFilter;
        private readonly Action<Poi, int> onAction;            // 0 = xem trên bản đồ, 1 = đặt làm A, 2 = đặt làm B
        private readonly Dictionary<CheckBox, PoiKind> kindOf = new Dictionary<CheckBox, PoiKind>();
        private bool loading;

        public PoiForm(HashSet<PoiKind> filter, Action<HashSet<PoiKind>> onFilter, Action<Poi, int> onAction)
        {
            InitializeComponent();
            this.onFilter = onFilter;
            this.onAction = onAction;

            kindOf[chkHospital] = PoiKind.Hospital;
            kindOf[chkSchool] = PoiKind.School;
            kindOf[chkUniversity] = PoiKind.University;
            kindOf[chkMarket] = PoiKind.Market;
            kindOf[chkPark] = PoiKind.Park;
            kindOf[chkLandmark] = PoiKind.Landmark;

            loading = true;
            foreach (KeyValuePair<CheckBox, PoiKind> kv in kindOf)
                kv.Key.Checked = filter == null || filter.Contains(kv.Value);
            loading = false;
        }

        public void SetPois(List<Poi> pois)
        {
            all = pois ?? new List<Poi>();
            Refill();
        }

        private HashSet<PoiKind> Current()
        {
            var set = new HashSet<PoiKind>();
            foreach (KeyValuePair<CheckBox, PoiKind> kv in kindOf) if (kv.Key.Checked) set.Add(kv.Value);
            return set;
        }

        // Bỏ dấu tiếng Việt để gõ "benh vien" vẫn tìm ra "Bệnh viện"
        private static string Fold(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string d = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in d)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
            return sb.ToString().Replace('đ', 'd').Replace('Đ', 'D').ToLowerInvariant();
        }

        private void Refill()
        {
            HashSet<PoiKind> kinds = Current();
            string key = Fold(txtSearch.Text.Trim());
            shown.Clear();
            lstPois.Items.Clear();

            var sorted = new List<Poi>(all);
            sorted.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
            foreach (Poi p in sorted)
            {
                if (!kinds.Contains(p.Kind)) continue;
                if (key.Length > 0 && Fold(p.Name).IndexOf(key, StringComparison.Ordinal) < 0) continue;
                shown.Add(p);
                lstPois.Items.Add("[" + Poi.KindName(p.Kind) + "]  " + p.Name);
            }
            lblCount.Text = shown.Count + " / " + all.Count + " địa điểm" +
                            (all.Count == 0 ? " (khu vực này chưa có, hãy bấm \"Tải địa điểm\")" : "");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) { Refill(); }

        private void chkKind_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            Refill();
            onFilter(Current());
        }

        private void btnKindAll_Click(object sender, EventArgs e)
        {
            bool any = false;
            foreach (CheckBox c in kindOf.Keys) if (!c.Checked) any = true;
            loading = true;
            foreach (CheckBox c in kindOf.Keys) c.Checked = any;      // còn loại chưa chọn thì chọn hết, ngược lại bỏ hết
            loading = false;
            Refill();
            onFilter(Current());
        }

        private Poi Selected()
        {
            int i = lstPois.SelectedIndex;
            return i >= 0 && i < shown.Count ? shown[i] : null;
        }

        private void lstPois_DoubleClick(object sender, EventArgs e) { Act(0); }
        private void btnPoiView_Click(object sender, EventArgs e) { Act(0); }
        private void btnPoiA_Click(object sender, EventArgs e) { Act(1); }
        private void btnPoiB_Click(object sender, EventArgs e) { Act(2); }

        private void Act(int what)
        {
            Poi p = Selected();
            if (p == null) { lblPoiHint.Text = "Hãy chọn một địa điểm trong danh sách."; return; }
            lblPoiHint.Text = "Bấm đúp một địa điểm để xem trên bản đồ.";
            onAction(p, what);
        }
    }
}
