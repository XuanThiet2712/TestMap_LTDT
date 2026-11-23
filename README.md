# TestMap - Demo gợi ý lộ trình xe buýt trên một khu vực nhỏ

C# WinForms (.NET Framework 4.8). Bản đồ đơn giản: chỉ có **đường** và **trạm xe buýt**,
tất cả tự vẽ từ dữ liệu OpenStreetMap nên đường đi luôn bám đúng đường trên bản đồ.

## Chạy
1. Mở `TestMap/TestMap.csproj` bằng Visual Studio 2022 (lần đầu tự tải NuGet `GMap.NET.WinForms`).
2. Nhấn F5. **Lần đầu cần Internet**: chương trình tự tải đường + trạm của khu vực đang chọn
   (mặc định Quận 1, mất khoảng 10-60 giây) và lưu vào `Data\<khu vực>\`. Các lần sau chạy offline.
3. Chuột phải lên bản đồ: lần 1 chọn điểm đi (A), lần 2 chọn điểm đến (B). Điểm chọn tự bắt vào trạm xe buýt gần nhất.
4. Chọn Dijkstra hoặc A*, bấm **Tìm đường**.

## Tính năng
- Chọn khu vực (Quận 1/3/5/10 TP.HCM, Hoàn Kiếm, Ba Đình Hà Nội, Hải Châu Đà Nẵng), có nút Tải lại dữ liệu.
- Đường vẽ theo cấp (đường lớn dày hơn), trạm xe buýt hiện tên khi zoom gần, rê chuột lên trạm để xem tên.
- Đường đi màu cam, các trạm trên đường đi được đánh dấu; danh sách trạm đi qua ở bảng bên phải (bấm vào để bay tới trạm).
- Quãng đường, thời gian ước tính, số trạm, số giao lộ đã xét. Nút **Đổi chiều** và **So sánh** Dijkstra với A*.

## Thêm khu vực khác
Mở `Form1.cs`, thêm một dòng vào mảng `Areas` (nam, tây, bắc, đông). Lấy tọa độ bằng nút Export trên openstreetmap.org.
Nên giữ hộp khoảng 2-3 km mỗi cạnh.

## Cấu trúc
- `Routing/OsmImporter.cs`  tải OSM (Overpass) và đổi ra nodes.csv, edges.csv, stops.csv
- `Routing/RoadGraph.cs`    mạng đường, trạm
- `Routing/PathFinder.cs`   Dijkstra và A*
- `Mapping/MapCanvas.cs`    vẽ đường, trạm, đường đi
- `Form1.cs`                giao diện (dựng bằng code)

Dữ liệu © OpenStreetMap contributors (giấy phép ODbL).
