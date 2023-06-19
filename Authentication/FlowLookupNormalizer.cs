using Microsoft.AspNetCore.Identity;

namespace FlowLabourApi.Authentication
{
    public class FlowLookupNormalizer : ILookupNormalizer
    {
        public string NormalizeEmail(string email)
        {
            return email;
        }

        public string NormalizeName(string name)
        {
            return name;
        }
    }
}
