﻿using FlowLabourApi.Utils;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户身份信息表，对应数据库表identityinfo
/// </summary>
public partial class IdentityInfo
{
    private sbyte? _age;
    private string _identityNo;

    /// <summary>
    /// 记录唯一标识ID
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 真实名字
    /// </summary>
    /// <example>张三</example>
    public string Realname { get; set; } = null!;

    /// <summary>
    /// 身份证号码
    /// </summary>
    /// <example>512431197812131037</example>
    public string IdentityNo
    {
        get
        {
            return _identityNo;
        }
        set
        {
            _identityNo = value;
            DateTime dateTime = IdentityValidateUtil.GetBirthDateFromID(value);
            _age = (sbyte?)(DateTime.Now.Year - dateTime.Year);
        }
    }

    /// <summary>
    /// 是否验证通过
    /// </summary>
    /// <example>1</example>
    public sbyte Checked { get; set; }

    /// <summary>
    /// 年龄，根据身份证号码自动推算
    /// </summary>
    /// <example>18</example>
    public sbyte? Age
    {
        private set
        {
            _age = value;
        }
        get { return _age; }
    }

    /// <summary>
    /// 身份验证通过日期
    /// </summary>
    public DateTime Checkeddate { get; set; }

}
