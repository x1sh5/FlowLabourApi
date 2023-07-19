using FlowLabourApi.Config;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Models.context;

public partial class XiangxpContext : DbContext
{
    public XiangxpContext()
    {
    }

    public XiangxpContext(DbContextOptions options): base(options)
    { 
    }
    public static bool IsConfigured { set; get; } = false;
    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Assignmenttype> Assignmenttypes { get; set; }

    public virtual DbSet<AssignmentUser> Assignmentusers { get; set; }

    public virtual DbSet<AuthUser> AuthUsers { get; set; }

    public virtual DbSet<AuthUserGroup> AuthUserGroups { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<ContentType> ContentTypes { get; set; }

    public virtual DbSet<Groupmessage> Groupmessages { get; set; }

    public virtual DbSet<Grouprole> GroupRoles { get; set; }

    public virtual DbSet<Grouptype> Grouptypes { get; set; }

    public virtual DbSet<Groupuser> GroupUsers { get; set; }

    public virtual DbSet<IdentityInfo> Identityinfos { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<RelatedAssignment> Relatedtasks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SigninLog> SigninLogs { get; set; }

    //public virtual DbSet<TendencyUser> TendencyUsers { get; set; }

    public virtual DbSet<UserIdentity> UserIdentities { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    //public virtual DbSet<VideoInfo> VideoInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        #warning To protect potentially sensitive information in your connection string, 
        //you should move it out of source code. You can avoid scaffolding the
        //connection string by using the Name= syntax to read it from configuration -
        //see https://go.microsoft.com/fwlink/?linkid=2131148. For more
        //guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        if (!IsConfigured)
        {
            optionsBuilder.UseMySQL(DbConfig.ConnectStr);
            IsConfigured = true;
        }

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("admin_log", tb => tb.HasComment("管理员登录记录"));

            entity.HasIndex(e => e.UserId, "fk_adminlog_userid_authuser_id_idx");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.ActionType)
                .HasComment("炒作类型")
                .HasColumnType("tinyint(4)")
                .HasColumnName("action_type");
            entity.Property(e => e.Describe)
                .HasColumnType("text")
                .HasColumnName("describe");
            entity.Property(e => e.Time)
                .HasComment("操作时间")
                .HasColumnType("datetime")
                .HasColumnName("time");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_adminlog_userid_authuser_id");
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("assignment", tb => tb.HasComment("任务详细表"));

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Branchid)
                .HasComment("branch表的id外键")
                .HasColumnType("int(11)")
                .HasColumnName("branchid");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Finishtime)
                .HasColumnType("datetime")
                .HasDefaultValue(null)
                .IsRequired(false)
                .HasColumnName("finishtime");
            entity.Property(e => e.Presumedtime)
                .HasComment("单位：分钟")
                .HasColumnType("int(11)")
                .HasColumnName("presumedtime");
            entity.Property(e => e.Publishtime)
                .HasColumnType("datetime")
                .HasColumnName("publishtime");
            entity.Property(e => e.Reward)
                .HasColumnType("int(11)")
                .HasColumnName("reward");
            entity.Property(e => e.Rewardtype)
                .HasComment("1:固定值，单位：分。2：百分比，精度为小数点后两位。")
                .HasColumnType("tinyint(4)")
                .HasColumnName("rewardtype");
            entity.Property(e => e.Status)
                .HasComment("0:代接，1：已结待完成，2：已完成。")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Typeid)
                .HasComment("tasktype的id外键")
                .HasColumnType("int(11)")
                .HasColumnName("typeid");
            entity.Property(e => e.Verify)
                .HasComment("0:未审核通过，1：审核通过。")
                .HasColumnType("tinyint(4)")
                .HasColumnName("verify");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("userid");
            entity.HasOne(d => d.AuthUser)
                .WithMany(e => e.Assignments)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(entity => entity.UserId)
                .HasConstraintName("fk_assigment_userid_user_id");

        });

        modelBuilder.Entity<Assignmenttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("assignmenttype");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.Level)
                .HasColumnType("int(11)")
                .HasColumnName("level");
        });

        modelBuilder.Entity<AssignmentUser>(entity =>
        {
            entity.HasKey(e => new { e.AssignmentId, e.UserId });
            //entity.HasAlternateKey(e => e.AssignmentId);
            //entity.HasNoKey();
            entity.ToTable("assignmentuser", tb => tb.HasComment("任务接取情况"));

            entity.HasIndex(e => e.AssignmentId, "fk_agmuser_asgid_agm_id_idx");

            entity.HasIndex(e => e.UserId, "fk_agmuser_asgid_user_id_idx");
            entity.HasIndex(entity => entity.AssignmentId, "assignmentid_UNIQUE").IsUnique();

            entity.Property(e => e.AssignmentId)
                .HasColumnType("int(11)")
                .HasColumnName("assignmentid");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("userid");

            entity.HasOne(e => e.Assignment)
                .WithOne(e => e.AssignmentUser);

            entity.HasOne(e => e.User)
                .WithOne(e=>e.AssignmentUser);
        });

        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("auth_user", tb => tb.HasComment("用户表"));

            entity.HasIndex(e => e.UserName, "username").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.DateJoined)
                .HasMaxLength(6)
                .HasColumnName("date_joined");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .HasColumnName("email");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.SecurityStamp)
                .HasMaxLength(64)
                .HasColumnName("securitystamp");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(128)
                .HasColumnName("passwordhash");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(12)
                .IsFixedLength()
                .HasColumnName("phoneNo");
            entity.Property(e => e.UserName)
                .HasMaxLength(150)
                .HasComment("程序页面显示的名字")
                .HasColumnName("username");
            entity.Property(e => e.ConcurrencyStamp)
                .HasMaxLength(64)
                .HasColumnName("concurrencystamp");
            entity.Property(e => e.LockoutEnabled)
                  .HasDefaultValue(0)
                  .HasColumnType("tinyint")
                  .HasColumnName("lockoutenabled");
            entity.Property(e => e.LockoutEnd)
                  .HasDefaultValue(0)
                  .HasColumnType("int(11)")
                  .HasColumnName("lockoutend");
            entity.Property(e => e.AccessFailedCount)
                  .HasColumnName("accessfailedcount")
                  .HasColumnType("int(11)");

            //entity.HasMany(e=>e.Assignments)
            //    .WithOne(e=>e.AuthUser)
            //    .HasForeignKey(e=>e.Id)
            //    .OnDelete(DeleteBehavior.ClientNoAction);
        });

        modelBuilder.Entity<AuthUserGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("auth_user_groups", tb => tb.HasComment("用户分组表"));

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Createtime)
                .HasColumnType("datetime")
                .HasColumnName("createtime");
            entity.Property(e => e.Gooupname)
                .HasMaxLength(50)
                .HasColumnName("gooupname");
            entity.Property(e => e.Groupdescipt)
                .HasMaxLength(250)
                .HasColumnName("groupdescipt");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("branch", tb => tb.HasComment("部门类型表"));

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ContentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("content_type", tb => tb.HasComment("--not use"));

            entity.HasIndex(e => new { e.AppLabel, e.Model }, "django_content_type_app_label_model_76bd3d3b_uniq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AppLabel)
                .HasMaxLength(100)
                .HasColumnName("app_label");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
        });

        modelBuilder.Entity<Groupmessage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("groupmessage");

            entity.HasIndex(e => e.From, "fk_groupmessage_from_authuser_id_idx");

            entity.HasIndex(e => e.To, "fk_groupmessage_t0_authusergroup_id_idx");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.From)
                .HasColumnType("int(11)")
                .HasColumnName("from");
            entity.Property(e => e.Message)
                .HasMaxLength(45)
                .HasColumnName("message");
            entity.Property(e => e.Messagetype)
                .HasMaxLength(45)
                .HasColumnName("messagetype");
            entity.Property(e => e.To)
                .HasColumnType("int(11)")
                .HasColumnName("to");

            entity.HasOne(d => d.FromNavigation).WithMany()
                .HasForeignKey(d => d.From)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_groupmessage_from_authuser_id");

            entity.HasOne(d => d.ToNavigation).WithMany()
                .HasForeignKey(d => d.To)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_groupmessage_t0_authusergroup_id");
        });

        modelBuilder.Entity<Grouprole>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("grouprole");

            entity.HasIndex(e => e.Groupid, "fk_frouprole_groupid_authusergroup_id_idx");

            entity.HasIndex(e => e.Roleid, "fk_grouprole_roleid_roleid_role_id_idx");

            entity.Property(e => e.Groupid)
                .HasColumnType("int(11)")
                .HasColumnName("groupid");
            entity.Property(e => e.Roleid)
                .HasColumnType("int(11)")
                .HasColumnName("roleid");

            entity.HasOne(d => d.Group).WithMany()
                .HasForeignKey(d => d.Groupid)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("fk_grouprole_groupid_authusergroup_id");

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("fk_grouprole_roleid_roleid_role_id");
        });

        modelBuilder.Entity<Grouptype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("grouptype", tb => tb.HasComment("用户组类型表"));

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsRequired(false)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Groupuser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("groupuser");

            entity.HasIndex(e => e.Groupid, "fk_groupuser_groupid_authusergroups_id_idx");

            entity.HasIndex(e => e.Userid, "fk_groupuser_userid_authuser_id_idx");

            entity.Property(e => e.Groupid)
                .HasColumnType("int(11)")
                .HasColumnName("groupid");
            entity.Property(e => e.Userid)
                .HasColumnType("int(11)")
                .HasColumnName("userid");

            entity.HasOne(d => d.Group).WithMany()
                .HasForeignKey(d => d.Groupid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_groupuser_groupid_authusergroups_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_groupuser_userid_authuser_id");
        });

        modelBuilder.Entity<IdentityInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("identityinfo", tb => tb.HasComment("用户身份信息表"));

            entity.HasIndex(e => e.IdentityNo, "identityNo_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Age)
                .HasColumnType("tinyint(4)")
                .HasColumnName("age");
            entity.Property(e => e.Checked)
                .HasColumnType("tinyint(4)")
                .HasColumnName("checked");
            entity.Property(e => e.Checkeddate)
                .HasColumnType("datetime")
                .HasColumnName("checkeddate");
            entity.Property(e => e.IdentityNo)
                .HasMaxLength(18)
                .IsFixedLength()
                .HasColumnName("identityNo");
            entity.Property(e => e.Realname)
                .HasMaxLength(45)
                .HasColumnName("realname");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("images");
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Md5, "md5_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Url)
                .HasMaxLength(128)
                .HasColumnName("url");
            entity.Property(e => e.AssignmentId)
                .HasColumnType("int(11)")
                .HasColumnName("asgnid");
            entity.Property(e => e.Md5)
                .HasMaxLength(64)
                .HasColumnName("md5");

        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("message");

            entity.HasIndex(e => e.From, "fk_message_from_authuser_id_idx");

            entity.HasIndex(e => e.To, "fk_message_to_authuser_id_idx");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.From)
                .HasColumnType("int(11)")
                .HasColumnName("from");
            entity.Property(e => e.Content)
                .HasMaxLength(45)
                .HasColumnName("message");
            entity.Property(e => e.ContentType)
                .HasMaxLength(45)
                .HasColumnName("messagetype");
            entity.Property(e => e.To)
                .HasColumnType("int(11)")
                .HasColumnName("to");

            entity.HasOne(d => d.FromNavigation).WithMany()
                .HasForeignKey(d => d.From)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_message_from_authuser_id");

            entity.HasOne(d => d.ToNavigation).WithMany()
                .HasForeignKey(d => d.To)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_message_to_authuser_id");
        });

        modelBuilder.Entity<RelatedAssignment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("relatedassignment", tb => tb.HasComment("关联任务"));

            entity.Property(e => e.RelatedId)
                .HasColumnType("int(11)")
                .HasColumnName("relatedid");
            entity.Property(e => e.AssignmentId)
                .HasColumnType("int(11)")
                .HasColumnName("assignmentid");
            entity.HasOne(d => d.Assignment)
                .WithMany()
                .HasForeignKey(d => d.RelatedId)
                .IsRequired();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("PRIMARY");

            entity.ToTable("role", tb => tb.HasComment("赋予user，group相应的权限。"));

            entity.HasIndex(e => e.Roleid, "roleid_UNIQUE").IsUnique();

            entity.Property(e => e.Roleid)
                .HasColumnType("int(11)")
                .HasColumnName("roleid");
            entity.Property(e => e.Descrpt)
                .HasColumnType("text")
                .HasColumnName("descrpt");
            entity.Property(e => e.Privilege)
                .HasMaxLength(45)
                .HasColumnName("privilege");
            entity.Property(e => e.ConcurrencyStamp)
                .HasMaxLength(64)
                .HasColumnName("concurrencystamp");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionKey).HasName("PRIMARY");

            entity.ToTable("session");

            entity.HasIndex(e => e.ExpireDate, "django_session_expire_date_a5c62663");

            entity.Property(e => e.SessionKey)
                .HasMaxLength(40)
                .HasColumnName("session_key");
            entity.Property(e => e.ExpireDate)
                .HasMaxLength(6)
                .HasColumnName("expire_date");
            entity.Property(e => e.SessionData).HasColumnName("session_data");
        });

        modelBuilder.Entity<SigninLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("signin_log", tb => tb.HasComment("登录历史"));

            entity.Property(e => e.Lastiplocation)
                .HasMaxLength(45)
                .HasColumnName("lastiplocation");
            entity.Property(e => e.Lasttime)
                .HasColumnType("datetime")
                .HasColumnName("lasttime");
            entity.Property(e => e.Userid)
                .HasColumnType("int(11)")
                .HasColumnName("userid");
            entity.Property(e => e.LoginProvider)
                  .HasColumnType("varchar(45)")
                  .IsRequired()
                  .HasColumnName("loginprovider");
            entity.Property(e => e.ProviderKey)
                  .HasColumnType("varchar(64)")
                  .IsRequired()
                  .HasColumnName("providerkey");

            entity.HasOne(d => d.AuthUser).WithMany()
                .HasForeignKey(d => d.Userid)
                .IsRequired();
        });

        #region TendencyUser
        //modelBuilder.Entity<TendencyUser>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PRIMARY");

        //    entity.ToTable("tendency_user");

        //    entity.Property(e => e.Id)
        //        .HasColumnType("int(11)")
        //        .HasColumnName("id");
        //    entity.Property(e => e.Name)
        //        .HasMaxLength(20)
        //        .HasColumnName("name");
        //    entity.Property(e => e.Password)
        //        .HasMaxLength(20)
        //        .HasColumnName("password");
        //});
        #endregion

        modelBuilder.Entity<UserIdentity>(entity =>
        {
            entity.HasNoKey();
            entity.ToTable("useridentity");
            entity.Property(entity => entity.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("userid");
            entity.Property(entity => entity.IdentityId)
                .HasColumnType("int(11)")
                .HasColumnName("identityid");
            entity.HasOne(d => d.User).WithOne()
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasForeignKey<UserIdentity>(d => d.UserId)
                .HasConstraintName("fk_useridenty_userid_authuser_id");
            entity.HasOne(d => d.Identity).WithOne()
                .HasForeignKey<UserIdentity>(d => d.IdentityId)//
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("fk_useridenty_identityid_identityinfo_id");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.ToTable("userrole");
            entity.HasIndex(e => e.RoleId, "fk_userrole_roleid_role_id_idx");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("userid");
            entity.Property(e => e.RoleId)
                .HasColumnType("int(11)")
                .HasColumnName("roleid");
            entity.HasOne(d => d.User).WithOne()
                .HasForeignKey<UserRole>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("fk_userrole_userid_authuser_id");
            entity.HasOne(d => d.Role).WithOne()
                .HasForeignKey<UserRole>(Role => Role.RoleId)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("fk_userrole_roleid_role_id");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.UserId });
            entity.ToTable("usertoken");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("userid");
            entity.Property(e => e.Value)
                .HasMaxLength(64)
                .HasColumnName("value");
            entity.Property(entity => entity.Expires)
                .HasColumnType("datetime")
                .HasColumnName("expires");
            entity.Property(entity => entity.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(entity => entity.LoginProvider)
                .HasMaxLength(64)
                .HasColumnName("loginprovider");
            entity.Property(entity => entity.Modify)
                .HasColumnType("datetime")
                .HasColumnName("modify");
            entity.Property(entity => entity.RefreshToken)
                .HasMaxLength(64)
                .HasColumnName("refreshtoken");

            entity.HasOne(entity => entity.User)
                  .WithMany()
                  .OnDelete(DeleteBehavior.ClientNoAction)
                  .HasForeignKey(e => e.UserId)
                  .IsRequired();
        });

        #region VideoInfo
        //modelBuilder.Entity<VideoInfo>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PRIMARY");

        //    entity.ToTable("video_info");

        //    entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

        //    entity.Property(e => e.Id)
        //        .HasColumnType("int(11)")
        //        .HasColumnName("id");
        //    entity.Property(e => e.SharedUrl)
        //        .HasMaxLength(45)
        //        .HasColumnName("sharedUrl");
        //    entity.Property(e => e.Text)
        //        .HasColumnType("text")
        //        .HasColumnName("text");
        //    entity.Property(e => e.Title)
        //        .HasMaxLength(45)
        //        .HasColumnName("title");
        //});
        #endregion

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
