namespace FlowLabourApi.ViewModels
{
    public class AuthUserView
    {

        public DateTime DateJoined { get; set; }

        public string Email { get; set; } = null!;
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLogin { get; set; }

        public string Passwordhash { get; set; } = null!;

        public string PhoneNo { get; set; } = null!;

        /// <summary>
        /// 程序页面显示的名字
        /// </summary>
        public string Username { get; set; } = null!;
    }
}