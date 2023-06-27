namespace FlowLabourApi.Utils
{
    public class HashUtil
    {
        public static string GetHash(string input) 
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
    }
}
