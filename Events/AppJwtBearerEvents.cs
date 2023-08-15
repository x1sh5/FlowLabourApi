using FlowLabourApi.Config;
using FlowLabourApi.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;


namespace FlowLabourApi.Events;
/// <summary>
/// Summary description for Class1
/// </summary>
//public class AppJwtBearerEvents : JwtBearerEvents
//{
//    public override Task MessageReceived(MessageReceivedContext context)
//    {
//        // 从 Http Request Header 中获取 Authorization
//        string authorization = context.Request.Headers[HeaderNames.Authorization];
//        if (string.IsNullOrEmpty(authorization))
//        {
//            context.NoResult();
//            return Task.CompletedTask;
//        }

//        // 必须为 Bearer 认证方案
//        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//        {
//            // 赋值token
//            context.Token = authorization["Bearer ".Length..].Trim();
//        }

//        if (string.IsNullOrEmpty(context.Token))
//        {
//            context.NoResult();
//            return Task.CompletedTask;
//        }

//        return Task.CompletedTask;
//    }


public class AppJwtBearerEvents : JwtBearerEvents
{
    private readonly JwtOptions _jwtOptions;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jwtOptions"></param>
    public AppJwtBearerEvents(IOptionsSnapshot<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>
    /// 当收到请求时（此时还未获取到Token）
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <remarks>
    /// 可在此自定义Token获取方式，并将获取的Token赋值到 context.Token（记得将Scheme从字符串中移除）
    /// 只要我们赋值的Token既非Null也非Empty，那后续验证就会使用该Token
    /// </remarks>
    public override Task MessageReceived(MessageReceivedContext context)
    {
        Console.WriteLine("-------------- MessageReceived Begin --------------");
        //if(context.Request.Path.StartsWithSegments("/chathub", StringComparison.OrdinalIgnoreCase))
        //{
        //    var accessToken = context.Request.Query["access_token"];
        //    if (!string.IsNullOrEmpty(accessToken))
        //    {
        //        // Read the token out of the query string
        //        context.Token = accessToken;
        //    }
        //    //context.Success();
        //    return Task.CompletedTask;
        //}
        if (context.Result != null)
        {
            return Task.CompletedTask;
        }

        Console.WriteLine($"Scheme: {context.Scheme.Name}");

        #region 以下是自定义Token获取方式示例（实际上也是默认方式）

        //string authorization = context.Request.Headers[HeaderNames.Authorization];
        StringValues authorization;
        bool hasCookie = true;
        bool hasQuery = true;
        authorization = context.Request.Cookies[CookieTypes.accessToken];
        if (string.IsNullOrEmpty(authorization))
        {
            hasCookie = false;
            context.Request.Query.TryGetValue(CookieTypes.accessToken,out authorization);
        }
        if (string.IsNullOrEmpty(authorization))
        {
            hasQuery = false;
            authorization = context.Request.Headers.Authorization;
        }
        if (string.IsNullOrEmpty(authorization))
        {
            context.NoResult();
            return Task.CompletedTask;
        }

        //if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //{
        //    context.Token = authorization["Bearer ".Length..].Trim();
        //}
        if(hasCookie || hasQuery)
        {
            context.Token = authorization;
        }
        else
        {
            string targetKey = CookieTypes.accessToken;
            string authstr = authorization.ToString();

            var accessToken = authstr.Split(';')
                              .Select(pair => pair.Split('='))
                              .FirstOrDefault(keyValue => keyValue.Length == 2 && keyValue[0].Trim() == targetKey);
            if (accessToken != null)
            {
                context.Token = accessToken[1].Trim();
            }
        }

        if (string.IsNullOrEmpty(context.Token))
        {
            context.NoResult();
            return Task.CompletedTask;
        }

        #endregion
        base.MessageReceived(context);
        Console.WriteLine("-------------- MessageReceived End --------------");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Token验证通过后
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task TokenValidated(TokenValidatedContext context)
    {
        Console.WriteLine("-------------- TokenValidated Begin --------------");

        base.TokenValidated(context);
        if (context.Result != null)
        {
            return Task.CompletedTask;
        }

        Console.WriteLine($"User Name: {context.Principal.Identity.Name}");
        Console.WriteLine($"Scheme: {context.Scheme.Name}");

        var token = context.SecurityToken;
        Console.WriteLine($"Token Id: {token.Id}");
        Console.WriteLine($"Token Issuer: {token.Issuer}");
        Console.WriteLine($"Token Valid From: {token.ValidFrom}");
        Console.WriteLine($"Token Valid To: {token.ValidTo}");

        Console.WriteLine($"Token SecurityKey: {token.SecurityKey}");

        //SymmetricSecurityKey

        if (token.SigningKey is SymmetricSecurityKey ssk)
        {
            Console.WriteLine($"Token SigningKey: {_jwtOptions.Encoding.GetString(ssk.ComputeJwkThumbprint())}");
        }
        else
        {
            Console.WriteLine($"Token SigningKey: {token.SigningKey}");
        }

        Console.WriteLine("-------------- TokenValidated End --------------");

        return Task.CompletedTask;
    }

    /// <summary>
    /// 由于认证过程中抛出异常，导致的身份认证失败后
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        Console.WriteLine("-------------- AuthenticationFailed Begin --------------");

        base.AuthenticationFailed(context);
        if (context.Result != null)
        {
            return Task.CompletedTask;
        }

        Console.WriteLine($"Scheme: {context.Scheme.Name}");
        Console.WriteLine($"Exception: {context.Exception}");

        Console.WriteLine("-------------- AuthenticationFailed End --------------");

        return Task.CompletedTask;
    }

    /// <summary>
    /// 质询
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task Challenge(JwtBearerChallengeContext context)
    {
        Console.WriteLine("-------------- Challenge Begin --------------");

        base.Challenge(context);
        if (context.Handled)
        {
            return Task.CompletedTask;
        }

        Console.WriteLine($"Scheme: {context.Scheme.Name}");
        Console.WriteLine($"Authenticate Failure: {context.AuthenticateFailure}");
        Console.WriteLine($"Error: {context.Error}");
        Console.WriteLine($"Error Description: {context.ErrorDescription}");
        Console.WriteLine($"Error Uri: {context.ErrorUri}");

        Console.WriteLine("-------------- Challenge End --------------");
        //context.Response.Headers.Location = "/login";
        return Task.CompletedTask;
    }

    /// <summary>
    /// 禁止403
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task Forbidden(ForbiddenContext context)
    {
        Console.WriteLine("-------------- Forbidden Begin --------------");

        base.Forbidden(context);
        if (context.Result != null)
        {
            return Task.CompletedTask;
        }

        Console.WriteLine($"Scheme: {context.Scheme.Name}");

        Console.WriteLine("-------------- Forbidden End --------------");

        return Task.CompletedTask;
    }
}