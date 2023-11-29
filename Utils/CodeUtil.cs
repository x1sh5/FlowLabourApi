using System.Text;

namespace FlowLabourApi.Utils
{
    public class CodeUtil
    {
        public static string GenerateRandomCode()
        {
            Random random = new Random();
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder code = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                int index = random.Next(characters.Length);
                code.Append(characters[index]);
            }
            return code.ToString();
        }
    }
}
