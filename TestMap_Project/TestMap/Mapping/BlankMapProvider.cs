using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using GMap.NET.WindowsForms;

namespace TestMap.Mapping
{
    // Nền bản đồ trơn một màu (không tải ảnh bản đồ nào).
    // Đường và trạm xe buýt được MapCanvas tự vẽ lên trên, nên bản đồ luôn khớp với dữ liệu.
    public class BlankMapProvider : GMapProvider
    {
        private static readonly Guid ProviderId = new Guid("2b0c8f0e-6a38-4a35-a2d3-7d1a5d3c9e21");
        private readonly byte[] tile;

        public BlankMapProvider(Color color)
        {
            MinZoom = 0;
            MaxZoom = 20;
            using (var bmp = new Bitmap(256, 256))
            {
                using (Graphics g = Graphics.FromImage(bmp)) g.Clear(color);
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    tile = ms.ToArray();
                }
            }
        }

        public override Guid Id { get { return ProviderId; } }
        public override string Name { get { return "Blank"; } }
        public override PureProjection Projection { get { return MercatorProjection.Instance; } }

        private GMapProvider[] overlays;
        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null) overlays = new GMapProvider[] { this };
                return overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            return GMapImageProxy.Instance.FromArray(tile);
        }
    }
}
