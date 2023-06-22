using FlowLabourApi.Authentication;
using FlowLabourApi.Config;
using FlowLabourApi.Hubs;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowLabourApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

                // User settings.
                //options.User.AllowedUserNameCharacters =
                //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniquePhoneNo = true;
            });

            // Force Identity's security stamp to be validated every minute.
            builder.Services.Configure<SecurityStampValidatorOptions>(o =>
                               o.ValidationInterval = TimeSpan.FromMinutes(1));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options => builder.Configuration.Bind("JwtSettings", options))
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options => new CookieAuthenticationOptions
                {
                    ExpireTimeSpan = TimeSpan.FromMinutes(5)
                });

            //IdentityDbContext
            builder.Services.AddIdentity<AuthUser, Role>().AddDefaultTokenProviders();

            //�Զ��������֤
            //builder.Services.AddIdentityCore<AuthUser>(o =>
            //{
            //    o.Stores.MaxLengthForKeys = 128;
            //    o.SignIn.RequireConfirmedAccount = true;
            //})
            //    .AddDefaultTokenProviders();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseCors();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}