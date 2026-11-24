using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TestMap.Services
{
    public enum UserRole { Passenger, Driver }

    public class UserAccount
    {
        public string Name;
        public UserRole Role;
        public string RoleText { get { return Role == UserRole.Driver ? "Tài xế" : "Hành khách"; } }
    }

    // Một địa điểm yêu thích của hành khách (lưu theo tọa độ để dùng được ở mọi khu vực bản đồ)
    public class FavPlace
    {
        public string Name;
        public double Lat, Lng;
        public override string ToString() { return Name; }
    }

    // Tài khoản hành khách / tài xế. Lưu trong Data/users.csv: tên,vai trò,muối,băm (PBKDF2).
    // Đăng ký tài xế cần "mã đăng ký tài xế" (file Data/driver_code.txt, mặc định TAIXE123; admin tự đổi).
    // Lưu ý: đây là khóa ở mức chương trình đơn máy, ai sửa trực tiếp file dữ liệu vẫn sửa được.
    public static class UserStore
    {
        public const string DefaultDriverCode = "TAIXE123";

        private static string UsersFile(string root) { return Path.Combine(root, "users.csv"); }
        private static string CodeFile(string root) { return Path.Combine(root, "driver_code.txt"); }
        private static string FavFile(string root, string user) { return Path.Combine(root, "users", user.ToLowerInvariant() + ".fav"); }

        public static string DriverCode(string root)
        {
            try
            {
                if (!File.Exists(CodeFile(root)))
                {
                    Directory.CreateDirectory(root);
                    File.WriteAllText(CodeFile(root), DefaultDriverCode);
                }
                return File.ReadAllText(CodeFile(root)).Trim();
            }
            catch { return DefaultDriverCode; }
        }

        private class Row { public string Name, Salt, Hash; public UserRole Role; }

        private static List<Row> ReadAll(string root)
        {
            var res = new List<Row>();
            if (!File.Exists(UsersFile(root))) return res;
            foreach (string line in File.ReadAllLines(UsersFile(root), Encoding.UTF8))
            {
                string[] p = line.Split(',');
                if (p.Length < 4) continue;
                res.Add(new Row { Name = p[0], Role = p[1] == "driver" ? UserRole.Driver : UserRole.Passenger, Salt = p[2], Hash = p[3] });
            }
            return res;
        }

        // Trả về null nếu thành công, ngược lại là câu báo lỗi
        public static string Register(string root, string user, string password, UserRole role, string driverCode)
        {
            user = (user ?? "").Trim();
            if (!Regex.IsMatch(user, @"^[A-Za-z0-9_.]{3,20}$")) return "Tên đăng nhập 3-20 ký tự, chỉ gồm chữ không dấu, số, dấu _ và dấu chấm.";
            if (string.Equals(user, AdminAuth.User, StringComparison.OrdinalIgnoreCase)) return "Tên này đã dành cho quản trị.";
            if ((password ?? "").Length < 6) return "Mật khẩu cần ít nhất 6 ký tự.";
            if (role == UserRole.Driver && (driverCode ?? "").Trim() != DriverCode(root)) return "Mã đăng ký tài xế không đúng.";
            try
            {
                List<Row> all = ReadAll(root);
                foreach (Row r in all)
                    if (string.Equals(r.Name, user, StringComparison.OrdinalIgnoreCase)) return "Tên đăng nhập đã có người dùng.";

                var salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);
                Directory.CreateDirectory(root);
                File.AppendAllText(UsersFile(root),
                    user + "," + (role == UserRole.Driver ? "driver" : "passenger") + "," +
                    Convert.ToBase64String(salt) + "," + Convert.ToBase64String(Hash(password, salt)) + "\n", Encoding.UTF8);
                return null;
            }
            catch (Exception ex) { return "Không lưu được tài khoản: " + ex.Message; }
        }

        public static UserAccount Login(string root, string user, string password)
        {
            user = (user ?? "").Trim();
            try
            {
                foreach (Row r in ReadAll(root))
                {
                    if (!string.Equals(r.Name, user, StringComparison.OrdinalIgnoreCase)) continue;
                    byte[] want = Convert.FromBase64String(r.Hash);
                    byte[] got = Hash(password ?? "", Convert.FromBase64String(r.Salt));
                    if (got.Length != want.Length) return null;
                    int diff = 0;
                    for (int i = 0; i < got.Length; i++) diff |= got[i] ^ want[i];
                    return diff == 0 ? new UserAccount { Name = r.Name, Role = r.Role } : null;
                }
            }
            catch { }
            return null;
        }

        private static byte[] Hash(string password, byte[] salt)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, 20000)) return kdf.GetBytes(32);
        }

        // ----- Địa điểm yêu thích -----
        public static List<FavPlace> LoadFavorites(string root, string user)
        {
            var res = new List<FavPlace>();
            try
            {
                string f = FavFile(root, user);
                if (!File.Exists(f)) return res;
                foreach (string line in File.ReadAllLines(f, Encoding.UTF8))
                {
                    string[] p = line.Split(new[] { '|' }, 3);
                    double la, ln;
                    if (p.Length < 3 ||
                        !double.TryParse(p[0], NumberStyles.Float, CultureInfo.InvariantCulture, out la) ||
                        !double.TryParse(p[1], NumberStyles.Float, CultureInfo.InvariantCulture, out ln)) continue;
                    res.Add(new FavPlace { Lat = la, Lng = ln, Name = p[2] });
                }
            }
            catch { }
            return res;
        }

        public static void SaveFavorites(string root, string user, List<FavPlace> list)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(root, "users"));
                var sb = new StringBuilder();
                foreach (FavPlace f in list)
                    sb.Append(f.Lat.ToString("F6", CultureInfo.InvariantCulture)).Append('|')
                      .Append(f.Lng.ToString("F6", CultureInfo.InvariantCulture)).Append('|')
                      .Append(f.Name.Replace('|', ' ').Replace('\n', ' ')).Append('\n');
                File.WriteAllText(FavFile(root, user), sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }
    }
}
