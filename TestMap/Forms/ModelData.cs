using System.Collections.Generic;
using System.Drawing;

namespace TestMap.Forms
{
    // Dữ liệu để vẽ "sơ đồ mô hình" một cách đi: trạm là ĐỈNH, đoạn xe chạy / đi bộ là CẠNH, thời gian (giây) là TRỌNG SỐ.
    public class ModelNode
    {
        public string Name = "";
        public double Lat, Lng;
        public bool Start, End;
        public bool Transfer;          // có từ 2 tuyến trở lên đi qua: chỗ có thể đổi tuyến
    }

    public class ModelEdge
    {
        public int A, B;               // chỉ số đỉnh
        public Color Color;
        public double Seconds;         // trọng số
        public bool Walk;
        public string Ref = "";
        public int Lane;               // thứ tự tuyến, để vẽ các tuyến chung đường song song nhau
        public int PathLeg = -1;       // thuộc bước thứ mấy của cách đi (-1: không nằm trên cách đi đang xem)
    }

    // Một bước của cách đi: chờ xe, đi xe, hoặc đi bộ
    public class ModelLeg
    {
        public string Kind = "";       // "wait" | "ride" | "walk"
        public string Text = "";
        public string Note = "";       // cách tính trọng số
        public double Seconds;
        public Color Color;
        public List<int> Edges = new List<int>();
        public int Node = -1;          // đỉnh liên quan (với bước chờ xe)
    }

    public class ModelData
    {
        public string Title = "";
        public List<ModelNode> Nodes = new List<ModelNode>();
        public List<ModelEdge> Edges = new List<ModelEdge>();
        public List<ModelLeg> Legs = new List<ModelLeg>();
        public int LaneCount = 1;

        public double TotalSeconds
        {
            get { double s = 0; foreach (ModelLeg l in Legs) s += l.Seconds; return s; }
        }
    }
}
