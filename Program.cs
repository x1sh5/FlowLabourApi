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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Timers;

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
                .AddJsonOptions(o => {//���jsonѭ������
                o.JsonSerializerOptions
                  .ReferenceHandler = ReferenceHandler.Preserve;
            });

            builder.Services.AddDbContext<XiangxpContext>((DbContextOptionsBuilder options) =>
            {
                options.UseMySQL(
                    DbConfig.ConnectStr);
            });

            //IdentityDbContext
            //builder.Services.AddIdentity<AuthUser, Role>().AddDefaultTokenProviders();

            // Learn more about configuring Swagger/OpenAPI
            // at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {//����ֶ�ע��
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "FlowLabourApi - V1",
                        Version = "v1"
                    }
                 );
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "FlowLabourApi.xml");
                c.IncludeXmlComments(filePath);
                // ����С��
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // ��header�����token�����ݵ���̨
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {

                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
            });

            builder.Services.AddSignalR();

            #region dependency injection
            builder.Services.AddScoped<AuthUser>();
            builder.Services.AddScoped<Role>();
            builder.Services.AddScoped<UserToken>();
            builder.Services.AddScoped<SigninLog>();
            builder.Services.AddHttpContextAccessor();
            // Identity services
            builder.Services.TryAddScoped<IUserValidator<AuthUser>, UserValidator<AuthUser>>();
            builder.Services.TryAddScoped<IPasswordValidator<AuthUser>, PasswordValidator<AuthUser>>();
            builder.Services.TryAddScoped<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();
            builder.Services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            builder.Services.TryAddScoped<IRoleValidator<Role>, RoleValidator<Role>>();
            // No interface for the error describer so we can add errors without rev'ing the interface
            builder.Services.TryAddScoped<IdentityErrorDescriber>();
            builder.Services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<AuthUser>>();
            builder.Services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<AuthUser>>();
            builder.Services.TryAddScoped<IUserClaimsPrincipalFactory<AuthUser>, UserClaimsPrincipalFactory<AuthUser, Role>>();
            builder.Services.TryAddScoped<IUserConfirmation<AuthUser>, DefaultUserConfirmation<AuthUser>>();
            builder.Services.TryAddScoped<UserManager<AuthUser>>();//
            builder.Services.TryAddScoped<SignInManager<AuthUser>>();//
            builder.Services.TryAddScoped<RoleManager<Role>>();//
            builder.Services.AddScoped<IUserStore<AuthUser>,FlowUserStore>();
            builder.Services.AddScoped<IRoleStore<Role>,FlowRoleStore>();
            builder.Services.AddScoped<IRoleValidator<Role>,FlowRoleValidator>();
            builder.Services.AddScoped<ILookupNormalizer,FlowLookupNormalizer>();
            builder.Services.AddScoped<AppJwtBearerEvents>();
            //builder.Services.AddSingleton<IAuthorizationHandler, RolesAuthorizationRequirement>(
            //    x=>new RolesAuthorizationRequirement(new[] { Permission.Admin }));
            //builder.Services.AddSingleton<>();
            //builder.Services.AddTransient< Provider>();
            #endregion

            //�Զ��������֤
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
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        //builder.Configuration.Bind("JwtSettings", options);
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
                            //ValidateIssuerSigningKey = true,

                            ValidateLifetime = true,

                            //RequireSignedTokens = true,
                            //RequireExpirationTime = true,

                            NameClaimType = JwtBearerDefaults.AuthenticationScheme,
                            //RoleClaimType = Permission.Admin,
                            TokenDecryptionKey = jwtOptions.SecurityKey,

                            //ClockSkew = TimeSpan.Zero,
                        };

                        options.SaveToken = true;

                        options.SecurityTokenValidators.Clear();
                        options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

                        options.EventsType = typeof(AppJwtBearerEvents);
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                        options.LoginPath = new PathString("/api/Account/Login");
                        //options.AccessDeniedPath = new PathString("/api/Account/Login");
                        //options.Events.OnRedirectToLogin = context =>
                        //    {
                        //        context.Response.Redirect("https://localhost:7221/api/Account/Login");
                        //        return Task.CompletedTask;
                        //    };
                    });
                


#pragma warning disable CS8620 // �����������͵Ŀ�Ϊ null �Բ��죬ʵ�β��������βΡ�
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.AddPolicy(Permission.Default,
                    policy => policy.RequireRole(Permission.Default).Build());//������ɫ
                options.AddPolicy(Permission.Admin,
                    policy => policy.RequireRole(Permission.Admin).Build());
                options.AddPolicy(Permission.SystemOrAmin,
                    policy => policy.RequireRole(Permission.Admin, Permission.System));//��Ĺ�ϵ
                options.AddPolicy(Permission.SystemAndAmin,
                    policy => policy.RequireRole(Permission.Admin).RequireRole(Permission.System));//�ҵĹ�ϵ
            })
                .TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, RolesAuthorizationRequirement>(
                    x => new RolesAuthorizationRequirement(
                        typeof(Permission).GetFields(BindingFlags.Public | BindingFlags.Static)
                                .Select(field => field.GetValue(null).ToString())
                        )));
#pragma warning restore CS8620 // �����������͵Ŀ�Ϊ null �Բ��죬ʵ�β��������βΡ�

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
            #region �¼�
            app.UseDefaultFiles();

            //app.UseHttpsRedirection(); //nginx����ʧ��ԭ��
            app.UseStaticFiles();

            

            //app.UseRouting();

            app.UseHttpsRedirection();
            #endregion

            app.MapControllers();

            app.UseCors();
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