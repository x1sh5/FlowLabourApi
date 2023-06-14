using FlowLabourApi.Utils;
using System.Text.RegularExpressions;

namespace FlowLabourApi.ViewModels
{
    public class IdentityinfoRegister
    {
        private string _identityNo;
        private bool _isValidate;

        public bool IsValidate
        {
            get
            {
                return _isValidate;
            }
        }

        public string IdentityNo
        {
            get
            {
                return _identityNo;
            }
            set
            {
                _isValidate = IdentityNoUtil.ValidateIdentityNo(value);
                if(_isValidate)
                {
                    _identityNo = value;
                    _isValidate = true;
                }
                else
                {
                    _isValidate = false;
                }
            }
        }

        public string Realname { get; set; } = null!;

    }
}