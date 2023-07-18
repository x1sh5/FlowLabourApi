using FlowLabourApi.Authentication;
using FlowLabourApi.Config;
using FlowLabourApi.Events;
using FlowLabourApi.Hubs;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.state;
using FlowLabourApi.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FlowLabourApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:7221",
                                            "http://localhost:5134");
                    });
            });

            builder.Services.AddControllersWithViews()
                .AddJsonOptions(o =>
                {//解决json循环引用
                    o.JsonSerializerOptions
                      .ReferenceHandler = ReferenceHandler.Preserve;
                });

            builder.Services.AddDbContext<XiangxpContext>(
                (DbContextOptionsBuilder options) =>
                    {
                        options.UseMySQL(
                            DbConfig.ConnectStr);
                    }
            );

            #region dependency injection
            builder.Services.TryAddScoped<AuthUser>();
            builder.Services.TryAddScoped<Role>();
            builder.Services.TryAddScoped<UserToken>();
            builder.Services.TryAddScoped<SigninLog>();
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


            //builder.Services.AddDistributedMemoryCache(options => { });

            //IdentityDbContext
            //builder.Services.AddIdentity<AuthUser, Role>().AddDefaultTokenProviders();
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

            builder.Services.AddSignalR();

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
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

                            //RequireSignedTokens = true,
                            //RequireExpirationTime = true,

                            NameClaimType = JwtBearerDefaults.AuthenticationScheme,
                            RoleClaimType = JwtClaimTypes.RoleClaim,
                            TokenDecryptionKey = jwtOptions.SecurityKey,

                            //ClockSkew = TimeSpan.Zero,
                        };

                        options.SaveToken = true;

                        options.SecurityTokenValidators.Clear();
                        options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

                        options.EventsType = typeof(AppJwtBearerEvents);
                    })
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            //options =>
            //    {
            //        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //        options.LoginPath = new PathString("/api/Account/Login");
            //        //options.AccessDeniedPath = new PathString("/api/Account/Login");
            //        //options.Events.OnRedirectToLogin = context =>
            //        //    {
            //        //        context.Response.Redirect("https://localhost:7221/api/Account/Login");
            //        //        return Task.CompletedTask;
            //        //    };
            //    })
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = new PathString("/Account/Login");
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };
            });



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

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            // <snippet_UseWebSockets>
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(40)
            };

            app.UseWebSockets(webSocketOptions);
            // </snippet_UseWebSockets>
            #region 新加
            app.UseDefaultFiles();

            //app.UseHttpsRedirection(); //nginx配置失败原因
            app.UseStaticFiles();



            //app.UseRouting();

            app.UseHttpsRedirection();
            #endregion

            app.MapControllers();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseStatusCodePages(async contextAccessor =>
            //{
            //    var response = contextAccessor.HttpContext.Response;

            //    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
            //        response.StatusCode == (int)HttpStatusCode.Forbidden)
            //    {
            //        response.Redirect("/api/Account/Login");
            //    }
            //});

            app.MapHub<ChatHub>("/chatHub");
            app.Run();
        }
    }
}