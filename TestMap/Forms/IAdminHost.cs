using System;
using System.Collections.Generic;
using TestMap.Routing;

namespace TestMap.Forms
{
    // Những gì màn hình quản trị cần từ cửa sổ chính (bản đồ, dữ liệu đang mở...).
    // Tách bằng interface để AdminForm không phụ thuộc vào Form1.
    public interface IAdminHost
    {
        RoadGraph Graph { get; }                 // mạng đường + trạm đang mở
        List<BusLine> Lines { get; }             // các tuyến đang hiển thị
        string AreaName { get; }                 // tên khu vực, ví dụ "TP.HCM - Quận 1"
        string DataDir { get; }                  // thư mục dữ liệu của khu vực
        string DataRoot { get; }                 // thư mục Data (chứa admin.cfg)
        bool HasAdminLines { get; }              // đã có file tuyến do admin chỉnh sửa chưa

        event EventHandler DataReloaded;         // dữ liệu vừa được nạp lại từ file

        // Chờ người dùng bấm một điểm trên bản đồ (Esc để hủy)
        void RequestMapPick(string prompt, Action<double, double> onPicked, Action onCancel);
        void PreviewLine(BusLine line);          // hiện một tuyến lên bản đồ
        void ApplyChanges();                     // dữ liệu đã được lưu ra file: nạp lại
        void ReloadFromDisk();                   // bỏ thay đổi chưa lưu
        void RestoreOriginalLines();             // xóa tuyến admin chỉnh sửa, quay lại tuyến gốc
        void DownloadRealLines();                // tải tuyến xe buýt thật từ OpenStreetMap
        void Logout();                           // đăng xuất quyền admin
    }
}
