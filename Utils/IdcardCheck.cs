using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace FlowLabourApi.Utils
{
    public class IdcardCheck
    {
        private const string SecretId = "b935f631027973a91c9326f17c09f08d";
        private const string BusinessId = "80184bd9b7a94e17919ff37ebf905d23";
        private const string Version = "v1";

        public readonly static Dictionary<string, string> Parameters = new();

        public static async Task<IdcardCheckResult> Check(string name,string cardNo)
        {
            Parameters.Add("secretId", SecretId);
            Parameters.Add("businessId", BusinessId);
            Parameters.Add("version", Version);
            Parameters.Add("timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
            Parameters.Add("nonce", Random.Shared.Next() + Parameters["timestamp"]);
            Parameters.Add("name", name);
            Parameters.Add("cardNo", cardNo);
            Parameters.Add("signature", genSignature("064bdfb505cba7a9614dddedd086d3a5", Parameters));
            var c = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://verify.dun.163.com/v1/idcard/check");
            var content = new FormUrlEncodedContent(Parameters);
            request.Content = content;
            request.Headers.Add("contentType", "application/x-www-form-urlencoded");
            HttpResponseMessage? r = await c.SendAsync(request);
            if (r != null)
            {
                if (r.StatusCode == HttpStatusCode.OK) {
                    var rscontent = await r.Content.ReadAsStringAsync();
                    dynamic res = JsonConvert.DeserializeObject(rscontent);
                    if(res.code == 200)
                    {
                        if (res["result"]["status"] == 1)
                        {
                            return new IdcardCheckResult(true, res["result"]["taskId"], null);
                        }
                        else
                        {
                            return new IdcardCheckResult(false, null, Reason(res["result"]["reasonType"]));
                        }
                    }
                    else
                    {
                        return new IdcardCheckResult(false, null, res.msg);
                    }
                    
                };
            }
            
            //r.Content.ReadAsStringAsync();
            return new IdcardCheckResult(false, null, "网络错误，请稍候再试！"); ;
        }

        private static String genSignature(String secretKey, Dictionary<String, String> parameters)
        {
            parameters = parameters.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<String, String> kv in parameters)
            {
                builder.Append(kv.Key).Append(kv.Value);
            }
            builder.Append(secretKey);
            String tmp = builder.ToString();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(tmp));
            builder.Clear();
            foreach (byte b in result)
            {
                builder.Append(b.ToString("x2").ToLower());
            }
            return builder.ToString();
        }

        private static string Reason(int code)
        {
            switch (code)
            {
                case 2:
                case 3:
                case 4:
                    return "身份信息有误";
                default:
                    return "结果获取失败，请重试";
            }
        }
    }

    public record IdcardCheckResult(bool Ok,string? TaskId,string? Reason);
}
