using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// Class ParkQueryInput.
    /// </summary>
    public class ParkQueryInput
    {
        /// <summary>
        /// Gets or sets the name of the park.
        /// </summary>
        /// <value>The name of the park.</value>
        [StringLength(20, MinimumLength = 0)]
        public string ParkName { get; set; }

        /// <summary>
        /// Gets or sets the cities.
        /// </summary>
        /// <value>The cities.</value>
        public IList<long> Cities { get; set; }
    }
}
