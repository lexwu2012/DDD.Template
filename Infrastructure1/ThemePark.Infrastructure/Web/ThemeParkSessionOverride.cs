namespace ThemePark.Infrastructure.Web
{
    public class ThemeParkSessionOverride
    {
        public ThemeParkSessionOverride(int agencyId, int userType)
        {
            AgencyId = agencyId;
            UserType = userType;
        }

        /// <summary>
        /// Gets the agency identifier.
        /// </summary>
        /// <value>The agency identifier.</value>
        public int AgencyId { get; set; }

        /// <summary>
        /// Gets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        public int UserType { get; set; }
    }
}
