using FlowLabourApi.Utils;

namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 身份信息返回视图
    /// </summary>
    public class IdentityinfoView
    {
        private sbyte? _age;
        private string _identityNo;

        /// <summary>
        /// 是否合法
        /// </summary>
        private bool _isValidate;

        /// <summary>
        /// 年龄
        /// </summary>
        public sbyte? Age
        {
            private set
            {
                _age = value;
            }
            get { return _age; }
        }

        /// <summary>
        /// 是否验证
        /// </summary>
        public sbyte Checked { get; set; }

        /// <summary>
        /// 验证日期
        /// </summary>
        public DateTime Checkeddate { get; set; }

        /// <summary>
        /// 记录ID
        /// </summary>
        public int Id { get; set; }

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
                _identityNo = value;
                _isValidate = IdentityValidateUtil.ValidateIdentityNo(value);
                if (_isValidate)
                {
                    DateTime dateTime = IdentityValidateUtil.GetBirthDateFromID(value);
                    Age = (sbyte?)(DateTime.Now.Year - dateTime.Year);
                }
            }
        }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Realname { get; set; } = null!;

    }
}