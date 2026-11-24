using System;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestMap.Services
{
    public class MyLocation
    {
        public double Lat, Lng;
        public string Source;      // nguồn: dịch vụ định vị của Windows, hay đoán theo địa chỉ IP
        public bool Approximate;   // true: chỉ gần đúng (theo IP, sai số có thể vài km)
    }

    // Tự xác định vị trí người dùng.
    //   1. Dịch vụ định vị của Windows (GPS / Wi-Fi) - cần bật Settings > Privacy > Location.
    //   2. Không được thì đoán theo địa chỉ IP (cần Internet, chỉ gần đúng cấp thành phố / quận).
    // Chạy trong luồng nền, trả về null nếu cả hai cách đều thất bại.
    public static class LocationService
    {
        public static Task<MyLocation> GetAsync()
        {
            return Task.Run(() =>
            {
                MyLocation r = FromWindows();
                return r ?? FromIp();
            });
        }

        private static MyLocation FromWindows()
        {
            try
            {
                using (var w = new GeoCoordinateWatcher(GeoPositionAccuracy.Default))
                {
                    if (!w.TryStart(false, TimeSpan.FromSeconds(6))) return null;
                    for (int i = 0; i < 40; i++)        // chờ tối đa ~8 giây
                    {
                        GeoCoordinate c = w.Position.Location;
                        if (c != null && !c.IsUnknown && !double.IsNaN(c.Latitude))
                            return new MyLocation { Lat = c.Latitude, Lng = c.Longitude, Source = "định vị của Windows", Approximate = false };
                        Thread.Sleep(200);
                    }
                }
            }
            catch { }
            return null;
        }

        private static MyLocation FromIp()
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create("http://ip-api.com/json/?fields=status,lat,lon,city");
                req.UserAgent = "TestMapDemo/1.0";
                req.Timeout = 6000;
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var rd = new StreamReader(resp.GetResponseStream()))
                {
                    string j = rd.ReadToEnd();
                    Match la = Regex.Match(j, "\"lat\"\\s*:\\s*(-?[0-9.]+)"), lo = Regex.Match(j, "\"lon\"\\s*:\\s*(-?[0-9.]+)");
                    if (!j.Contains("\"success\"") || !la.Success || !lo.Success) return null;
                    return new MyLocation
                    {
                        Lat = double.Parse(la.Groups[1].Value, CultureInfo.InvariantCulture),
                        Lng = double.Parse(lo.Groups[1].Value, CultureInfo.InvariantCulture),
                        Source = "địa chỉ IP", Approximate = true
                    };
                }
            }
            catch { return null; }
        }
    }
}
