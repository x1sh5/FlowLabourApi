using FlowLabourApi.Authentication;
using FlowLabourApi.Config;
using FlowLabourApi.Events;
using FlowLabourApi.Hubs;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.state;
using FlowLabourApi.Options;
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
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FlowLabourApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            //����������Ҫ������
            //IHostEnvironment env = builder.Environment;
            //builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

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
                {//���jsonѭ������
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

            //IdentityDbContext
            builder.Services.AddIdentityCore<AuthUser>().AddRoles<Role>().AddDefaultTokenProviders();

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

            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

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
                    });
                //.AddGoogle(googleOptions =>
                //{
                //    googleOptions.ClientId = "588162123232-04cs7bopvtes67f74m1p6rudh7lgaprd.apps.googleusercontent.com";
                //    googleOptions.ClientSecret = "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";
                //});

#pragma warning disable CS8620 // �����������͵Ŀ�Ϊ null �Բ��죬ʵ�β��������βΡ�
            builder.Services.AddAuthorization(options =>
            {
                //options.FallbackPolicy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                options.AddPolicy(Permission.Default,
                    policy => policy.RequireRole(Permission.Default)
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));//������ɫ
                options.AddPolicy(Permission.Admin,
                    policy => policy.RequireRole(Permission.Admin).RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
                options.AddPolicy(Permission.SystemOrAmin,
                    policy => policy.RequireRole(Permission.Admin, Permission.System)
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));//��Ĺ�ϵ
                options.AddPolicy(Permission.SystemAndAmin,
                    policy => policy.RequireRole(Permission.Admin).RequireRole(Permission.System)
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));//�ҵĹ�ϵ
            })
                .TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, RolesAuthorizationRequirement>(
                    x => new RolesAuthorizationRequirement(
                        typeof(Permission).GetFields(BindingFlags.Public | BindingFlags.Static)
                                .Select(field => field.GetValue(null).ToString())
                        )));
#pragma warning restore CS8620 // �����������͵Ŀ�Ϊ null �Բ��죬ʵ�β��������βΡ�

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHsts();
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
                KeepAliveInterval = TimeSpan.FromMinutes(40)
            };

            app.UseWebSockets(webSocketOptions);
            // </snippet_UseWebSockets>
            #region �¼�
            app.UseDefaultFiles();

            app.UseHttpsRedirection(); //nginx����ʧ��ԭ��
            app.UseStaticFiles();



            //app.UseRouting();

            //app.UseHttpsRedirection();
            #endregion

            app.MapControllers();

            //����
            app.UseCors(builder =>
            {
                builder
                //.AllowCredentials()
                //.AllowAnyOrigin()  //������������ע�͸���
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

            app.MapHub<ChatHub>("/chatHub");
            //app.Use((context,next)=>
            //{
            //    context.Response.Headers.AccessControlExposeHeaders.Append("Set-Cookie");
            //    return next.Invoke();
            //});
            app.Run();
        }
    }
}