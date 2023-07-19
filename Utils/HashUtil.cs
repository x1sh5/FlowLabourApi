using System.Security.Cryptography;

namespace FlowLabourApi.Utils
{
    public class HashUtil
    {
        public static string Sha256(string input)
        {
            using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                //string hash = BitConverter.ToString(bytes).Replace("-", string.Empty);
                return builder.ToString();
                //return hash;
            }
        }

        public static string Md5(string input)
        {
            using (System.Security.Cryptography.MD5 sha256Hash = System.Security.Cryptography.MD5.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                //string hash = BitConverter.ToString(bytes).Replace("-", string.Empty);
                return builder.ToString();
                //return hash;
            }
        }

        public static string CalculateMD5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
                
            }
        }

        public static string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
        }
    }
}
