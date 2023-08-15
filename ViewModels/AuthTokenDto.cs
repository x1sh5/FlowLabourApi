namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthTokenDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}
