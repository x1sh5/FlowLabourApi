using System.Globalization;
using System.Text.RegularExpressions;

namespace FlowLabourApi.Utils
{   
    /// <summary>
    /// 身份信息验证工具类
    /// </summary>
    public class IdentityValidateUtil
    {
        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="idNo"></param>
        /// <returns></returns>
        public static bool ValidateIdentityNo(string idNo)
        {
            // Check if the input is empty or null
            if (string.IsNullOrEmpty(idNo))
            {
                return false;
            }

            // Check if the input has exactly 18 characters
            if (idNo.Length != 18)
            {
                return false;
            }

            // Check if the input contains only numbers or ends with 'X'
            Regex regex = new Regex("[0-9X]{18}");
            Match match = regex.Match(idNo);
            if (!match.Success)
            {
                return false;
            }

            // Check the validity of the last digit
            int[] weights =
            {
                7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2
            };
            int[] remainders =
            {
                1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2
            };
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(idNo[i].ToString()) * weights[i];
            }
            int remainder = sum % 11;
            if (remainder == 2 && idNo[17] == 'X')
            {
                return true;
            }
            if (idNo[17] == remainders[remainder].ToString()[0])
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// calculate the birth date from the identity number
        /// </summary>
        /// <param name="idNo"></param>
        /// <returns></returns>
        public static DateTime GetBirthDateFromID(string idNo)
        {
            string birthDateStr = idNo.Substring(6, 8);
            DateTime birthDate;
            if (DateTime.TryParseExact(birthDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
            {
                return birthDate;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// 验证邮件格式
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool ValidateEmailFormat(string email)
        {
            // Check if the input is empty or null
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            // Check if the input contains only one '@'
            int atSymbolCount = email.Count(x => x == '@');
            if (atSymbolCount != 1)
            {
                return false;
            }

            // Check if the input doesn't start or end with '@'
            if (email.StartsWith("@") || email.EndsWith("@"))
            {
                return false;
            }

            // Check if the input doesn't contain invalid characters
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            Match match = regex.Match(email);
            if (!match.Success)
            {
                return false;
            }

            return true;
        }

    }
}
