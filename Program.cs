using FlowLabourApi.Authentication;
using FlowLabourApi.Config;
using FlowLabourApi.Hubs;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
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
                .AddJsonOptions(o => {//���jsonѭ������
                o.JsonSerializerOptions
                  .ReferenceHandler = ReferenceHandler.Preserve;
            });
            builder.Services.AddDbContext<XiangxpContext>((options) =>
            {
                options.UseMySQL(
                    DbConfig.ConnectStr);
            });

            builder.Services.AddScoped<AuthUser>().AddScoped<Role>();

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

            builder.Services.AddScoped<AuthUser>();
            builder.Services.AddScoped<IUserStore<AuthUser>,FlowUserStore>();
            builder.Services.AddScoped<IRoleStore<Role>,FlowRoleStore>();
            builder.Services.AddScoped<IRoleValidator<Role>,FlowRoleValidator>();
            builder.Services.AddScoped<ILookupNormalizer,FlowLookupNormalizer>();
            builder.Services.AddScoped<Role>();
            builder.Services.AddScoped<UserToken>();
            builder.Services.AddScoped<SigninLog>();
            //builder.Services.AddSingleton<>();

            //IdentityDbContext
            //builder.Services.AddIdentity<AuthUser, Role>().AddDefaultTokenProviders();
            //�Զ��������֤
            //AddDefaultTokenProviders

            //builder.Services.AddTransient< Provider>();

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

            builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options => builder.Configuration.Bind("JwtSettings", options))
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
                    }
                );

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.AddPolicy("default", policy => policy.RequireRole("default").Build());//������ɫ
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));//��Ĺ�ϵ
                options.AddPolicy("SystemAndAdmin", policy => policy.RequireRole("Admin").RequireRole("System"));//�ҵĹ�ϵ
            });

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

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}