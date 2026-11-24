using System;
using System.Collections.Generic;

namespace TestMap.Forms
{
    public class AlgoSimInfo
    {
        public string Algo = "";
        public int Total;               // số bước (mỗi bước chốt một giao lộ)
        public bool Found;              // thuật toán có tìm được đường không
        public string Intro = "";
    }

    public class BusSimInfo
    {
        public int Total;                                      // tổng số bước của mọi lượt
        public List<string> RoundTitles = new List<string>();  // dòng mô tả từng lượt
        public List<int> RoundStarts = new List<int>();        // số bước đã chạy trước khi lượt này bắt đầu
        public List<int> RoundEnds = new List<int>();          // số bước đã chạy khi lượt này kết thúc
        public string Intro = "";
    }

    // Những gì bảng Mô phỏng cần từ cửa sổ chính
    public interface ISimHost
    {
        // Mô phỏng thuật toán tìm đường ngắn nhất (Dijkstra / A*)
        AlgoSimInfo AlgoSimPrepare(out string error);
        string AlgoSimSeek(int step);

        // Mô phỏng bộ gợi ý tuyến xe buýt
        BusSimInfo BusSimPrepare(out string error);
        string BusSimSeek(int step);

        // Chặn đường
        bool BlockMode { get; set; }
        int BlockedCount { get; }
        void BlockOnRoute();
        void BlockRandom(int count);
        void ClearBlocked();

        void SaveMapImage();
        void SimClose();                     // bảng mô phỏng đóng: xóa các lớp vẽ mô phỏng
        event EventHandler SimInvalidated;   // dữ liệu đổi (chặn đường, đổi điểm đi/đến...) nên mô phỏng cũ hết giá trị
    }
}
