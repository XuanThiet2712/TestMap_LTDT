using System;
using System.Net;
using System.Windows.Forms;

namespace TestMap
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // .NET Framework 4.8 cần bật TLS 1.2 để tải dữ liệu qua https
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
