using FlowLabourApi.Utils;
using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户身份信息表
/// </summary>
public partial class IdentityInfo
{
    private sbyte? _age;
    private string _identityNo;

    public int Id { get; set; }

    public string Realname { get; set; } = null!;

    public string IdentityNo
    {
        get
        {
            return _identityNo;
        }
        set
        {
            _identityNo = value;
            DateTime dateTime = IdentityNoUtil.GetBirthDateFromID(value);
            _age = (sbyte?)(DateTime.Now.Year - dateTime.Year);
        }
    }

    public sbyte Checked { get; set; }

    public sbyte? Age
    {
        private set
        {
            _age = value;
        }
        get { return _age; }
    }

    public DateTime Checkeddate { get; set; }

}
