using Microsoft.AspNetCore.Identity;

namespace FlowLabourApi.Options
{
    public class MyIdentityOptions
    {
        public MyIdentityOptions() { }

        //
        // 摘要:
        //     Gets or sets the Microsoft.AspNetCore.Identity.ClaimsIdentityOptions for the
        //     identity system.
        //
        // 值:
        //     The Microsoft.AspNetCore.Identity.ClaimsIdentityOptions for the identity system.
        public ClaimsIdentityOptions ClaimsIdentity { get; set; }
        //
        // 摘要:
        //     Gets or sets the Microsoft.AspNetCore.Identity.UserOptions for the identity system.
        //
        // 值:
        //     The Microsoft.AspNetCore.Identity.UserOptions for the identity system.
        public MyUserOptions User { get; set; }
        //
        // 摘要:
        //     Gets or sets the Microsoft.AspNetCore.Identity.PasswordOptions for the identity
        //     system.
        //
        // 值:
        //     The Microsoft.AspNetCore.Identity.PasswordOptions for the identity system.
        public PasswordOptions Password { get; set; }
        //
        // 摘要:
        //     Gets or sets the Microsoft.AspNetCore.Identity.LockoutOptions for the identity
        //     system.
        //
        // 值:
        //     The Microsoft.AspNetCore.Identity.LockoutOptions for the identity system.
        public LockoutOptions Lockout { get; set; }
        //
        // 摘要:
        //     Gets or sets the Microsoft.AspNetCore.Identity.SignInOptions for the identity
        //     system.
        //
        // 值:
        //     The Microsoft.AspNetCore.Identity.SignInOptions for the identity system.
        public SignInOptions SignIn { get; set; }
        //
        // 摘要:
        //     Gets or sets the Microsoft.AspNetCore.Identity.TokenOptions for the identity
        //     system.
        //
        // 值:
        //     The Microsoft.AspNetCore.Identity.TokenOptions for the identity system.
        public TokenOptions Tokens { get; set; }
        //
        // 摘要:
        //     Gets or sets the Microsoft.AspNetCore.Identity.StoreOptions for the identity
        //     system.
        //
        // 值:
        //     The Microsoft.AspNetCore.Identity.StoreOptions for the identity system.
        public StoreOptions Stores { get; set; }
    }
}
