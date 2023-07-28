using FlowLabourApi.Utils;

namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 前端注册信息
    /// </summary>
    public class IdentityinfoRegister
    {
        private string _identityNo;
        private bool _isValidate;

        /// <summary>
        /// 信息是否合法
        /// </summary>
        public bool IsValidate
        {
            get
            {
                return _isValidate;
            }
        }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdentityNo
        {
            get
            {
                return _identityNo;
            }
            set
            {
                _isValidate = IdentityValidateUtil.ValidateIdentityNo(value);
                if (_isValidate)
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

        /// <summary>
        /// 真实名字
        /// </summary>
        public string Realname { get; set; } = null!;

    }
}