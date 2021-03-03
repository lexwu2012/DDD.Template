namespace ThemePark.Infrastructure.Web
{
    /// <summary>
    /// claim types
    /// </summary>
    public class ThemeParkClaimTypes
    {
        /// <summary>
        /// The user type claim
        /// </summary>
        public const string UserType = "http://www.themepark.com/identity/claims/usertype";

        /// <summary>
        /// for ParkArea claim
        /// </summary>
        public const string ParkArea = "http://www.themepark.com/identity/claims/parkarea";

        /// <summary>
        /// The agency claim
        /// </summary>
        public const string Agency = "http://www.themepark.com/identity/claims/agency";

        /// <summary>
        /// The terminal claim
        /// </summary>
        public const string Terminal = "http://www.themepark.com/identity/claims/terminal";

        /// <summary>
        /// The FingerType claim
        /// </summary>
        public const string FingerType = "http://www.themepark.com/identity/claims/FingerType";
    }
}
