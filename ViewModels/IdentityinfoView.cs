using FlowLabourApi.Utils;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FlowLabourApi.ViewModels
{
    public class IdentityinfoView
    {
        private sbyte? _age;
        private string _identityNo;

        private bool _isValidate;

        public sbyte? Age {
            private set
            {
                _age = value;
            }
            get { return _age; }
        } 
        public sbyte Checked { get; set; }

        public DateTime Checkeddate { get; set; }
        public int Id { get; set; }

        public string IdentityNo { 
            get 
            { 
                return _identityNo;
            }
            set { 
                _identityNo = value;
                _isValidate = IdentityNoUtil.ValidateIdentityNo(value);
                if(_isValidate)
                {
                    DateTime dateTime = IdentityNoUtil.GetBirthDateFromID(value);
                    Age = (sbyte?)(DateTime.Now.Year - dateTime.Year);
                }
            } 
        }

        public string Realname { get; set; } = null!;

    }
}