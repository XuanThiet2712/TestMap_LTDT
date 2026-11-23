using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TestMap.Services
{
    // Đăng nhập quản trị. Mật khẩu lưu dạng băm (PBKDF2 + muối ngẫu nhiên) trong Data/admin.cfg.
    // Chưa có file thì dùng tài khoản mặc định admin / admin123 (nên đổi ngay trong màn hình quản trị).
    // Lưu ý: đây chỉ là khóa ở mức giao diện chương trình; ai sửa trực tiếp các file dữ liệu thì vẫn sửa được.
    public static class AdminAuth
    {
        public const string User = "admin";
        public const string DefaultPassword = "admin123";

        private static string FilePath(string dataRoot) { return Path.Combine(dataRoot, "admin.cfg"); }

        public static bool IsDefault(string dataRoot) { return !File.Exists(FilePath(dataRoot)); }

        public static bool Verify(string dataRoot, string user, string password)
        {
            if (!string.Equals(user, User, StringComparison.OrdinalIgnoreCase)) return false;
            if (IsDefault(dataRoot)) return password == DefaultPassword;
            try
            {
                string[] p = File.ReadAllText(FilePath(dataRoot)).Trim().Split('|');
                byte[] salt = Convert.FromBase64String(p[0]), want = Convert.FromBase64String(p[1]);
                byte[] got = Hash(password, salt);
                if (got.Length != want.Length) return false;
                int diff = 0;
                for (int i = 0; i < got.Length; i++) diff |= got[i] ^ want[i];     // so sánh không phụ thuộc thời gian
                return diff == 0;
            }
            catch { return false; }
        }

        public static void SetPassword(string dataRoot, string password)
        {
            var salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);
            Directory.CreateDirectory(dataRoot);
            File.WriteAllText(FilePath(dataRoot), Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(Hash(password, salt)));
        }

        private static byte[] Hash(string password, byte[] salt)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, 20000)) return kdf.GetBytes(32);
        }
    }
}
