#define linux
using Essensoft.Paylink.WeChatPay;
using FlowLabourApi.Authentication;
using FlowLabourApi.Config;
using FlowLabourApi.Events;
using FlowLabourApi.Hubs;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.Services;
using FlowLabourApi.Models.state;
using FlowLabourApi.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;

namespace FlowLabourApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if linux
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");
#endif

            var builder = WebApplication.CreateBuilder(args);

            //IHostEnvironment env = builder.Environment;
            //builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

// NLog: Setup NLog for Dependency injection
#if linux
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
#endif

            builder.Services.AddCors();
            builder.Services.AddWeChatPay();

            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(//解决json循环引用
                    op =>op.SerializerSettings
                    .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            builder.Services.AddDbContext<FlowContext>(
                (DbContextOptionsBuilder options) =>
                    {
                        options.EnableSensitiveDataLogging();
                        options.UseMySQL(builder.Configuration.GetConnectionString("Connection"));
                    }
            );

            #region dependency injection
            builder.Services.TryAddScoped<AuthUser>();
            builder.Services.TryAddScoped<Role>();
            builder.Services.TryAddScoped<UserToken>();
            builder.Services.TryAddScoped<SigninLog>();
            builder.Services.TryAddScoped<MessageService>();
            builder.Services.TryAddScoped<SignInManager<AuthUser>>();
            builder.Services.TryAddScoped<IAuthTokenService, AuthTokenService>();
            builder.Services.AddHttpContextAccessor();
            // Identity services
            builder.Services.TryAddScoped<IUserStore<AuthUser>, FlowUserStore>();
            builder.Services.TryAddScoped<IRoleStore<Role>, FlowRoleStore>();
            builder.Services.TryAddScoped<IRoleValidator<Role>, FlowRoleValidator>();
            builder.Services.TryAddScoped<ILookupNormalizer, FlowLookupNormalizer>();
            builder.Services.TryAddScoped<AppJwtBearerEvents>();
            //signalR
            builder.Services.TryAddSingleton<IUserIdProvider, FlowUserIdProvider>();
            //builder.Services.AddSingleton<IAuthorizationHandler, RolesAuthorizationRequirement>(
            //    x=>new RolesAuthorizationRequirement(new[] { Permission.Admin }));
            //builder.Services.AddSingleton<>();
            //builder.Services.AddTransient< Provider>();
            #endregion

            //IdentityDbContext
            builder.Services.AddIdentityCore<AuthUser>().AddRoles<Role>().AddDefaultTokenProviders();

            // Learn more about configuring Swagger/OpenAPI
            // at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {//添加字段注释
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "FlowLabourApi - V1",
                        Version = "v1"
                    }
                 );
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "FlowLabourApi.xml");
                c.IncludeXmlComments(filePath);
                // 开启小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // 在header中添加token，传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {

                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });

            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
            }).AddNewtonsoftJsonProtocol();

            builder.Services.Configure<WeChatPayOptions>(builder.Configuration.GetSection("WeChatPay"));
            //自定义身份验证
            //AddDefaultTokenProviders
            builder.Services.Configure<MyIdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedAccount = true;

                // User settings.
                //options.User.AllowedUserNameCharacters =
                //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniquePhoneNo = true;
                //token settings
                //options.Tokens.AuthenticatorTokenProvider
            });

            // Force Identity's security stamp to be validated every minute.
            builder.Services.Configure<SecurityStampValidatorOptions>(o =>
                               o.ValidationInterval = TimeSpan.FromMinutes(1));


            JwtOptions? jwtOptions = new JwtOptions();

            builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        //builder.Configuration.Bind("JwtSettings", options);
                        //options.Authority = "https://localhost:7221";
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256 },
                            ValidTypes = new[] { JwtConstants.HeaderType },

                            ValidIssuer = jwtOptions.Issuer,
                            ValidateIssuer = true,

                            ValidAudience = jwtOptions.Audience,
                            ValidateAudience = true,

                            IssuerSigningKey = jwtOptions.SecurityKey,
                            //SignatureValidator=jwtOptions.SignatureValidator,
                            ValidateIssuerSigningKey = true,

                            ValidateLifetime = true,
                            //LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                            
                            //RequireSignedTokens = true,
                            RequireExpirationTime = true,

                            NameClaimType = JwtBearerDefaults.AuthenticationScheme,
                            RoleClaimType = JwtClaimTypes.RoleClaim,
                            TokenDecryptionKey = jwtOptions.SecurityKey,

                            ClockSkew = TimeSpan.FromSeconds(60),
                        };
                        
                        options.SaveToken = true;

                        options.SecurityTokenValidators.Clear();
                        options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

                        options.EventsType = typeof(AppJwtBearerEvents);
                    });
            //.AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = "588162123232-04cs7bopvtes67f74m1p6rudh7lgaprd.apps.googleusercontent.com";
            //    googleOptions.ClientSecret = "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";
            //});

#pragma warning disable CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
            builder.Services.AddAuthorization(options =>
            {
                //options.FallbackPolicy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                options.AddPolicy(Permission.Default,
                    policy => policy.RequireRole(Permission.Default)
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));//单独角色
                options.AddPolicy(Permission.Admin,
                    policy => policy.RequireRole(Permission.Admin).RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
                options.AddPolicy(Permission.SystemOrAmin,
                    policy => policy.RequireRole(Permission.Admin, Permission.System)
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));//或的关系
                options.AddPolicy(Permission.SystemAndAmin,
                    policy => policy.RequireRole(Permission.Admin).RequireRole(Permission.System)
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));//且的关系
            })
                .TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, RolesAuthorizationRequirement>(
                    x => new RolesAuthorizationRequirement(
                        typeof(Permission).GetFields(BindingFlags.Public | BindingFlags.Static)
                                .Select(field => field.GetValue(null).ToString())
                        )));
#pragma warning restore CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHsts();
#if linux
            app.UseExceptionHandler("/Home/Error");//服务器环境添加此行
#endif


            //Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            // <snippet_UseWebSockets>
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(20)
            };

            app.UseWebSockets(webSocketOptions);
            // </snippet_UseWebSockets>
            #region 新加
            app.UseDefaultFiles();

            app.UseHttpsRedirection(); //nginx配置失败原因
            app.UseStaticFiles();

            //app.UseRouting();

            #endregion

            app.MapControllers();

            //跨域
            app.UseCors(builder =>
            {
                builder
                //.AllowCredentials()
#if linux
                //.AllowAnyOrigin()
#else
                .AllowAnyOrigin()
#endif
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages(async contextAccessor =>
            {
                var response = contextAccessor.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Redirect)
                {
                    if(response.Headers.Location == "/Account/Login")
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        response.Headers.Remove("location");
                    }
                    
                }
            });

            app.MapHub<ChatHub>("/chatHub");//use HubConnectionHandler<THub> actually

            //app.Use((context,next)=>
            //{
            //    context.Response.Headers.AccessControlExposeHeaders.Append("Set-Cookie");
            //    return next.Invoke();
            //});

#if linux
            try
            {
                app.Run();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
#else
            app.Run();
#endif
        }
    }
}