namespace FlowLabourApi.Options
{
    public class MyUserOptions
    {
        //
        // 摘要:
        //     Gets or sets the list of allowed characters in the username used to validate
        //     user names. Defaults to abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+
        //
        // 值:
        //     The list of allowed characters in the username used to validate user names.
        public string AllowedUserNameCharacters { get; set; }
        //
        // 摘要:
        //     Gets or sets a flag indicating whether the application requires unique emails
        //     for its users. Defaults to false.
        //
        // 值:
        //     True if the application requires each user to have their own, unique email, otherwise
        //     false.
        public bool RequireUniquePhoneNo { get; set; }
    }
}