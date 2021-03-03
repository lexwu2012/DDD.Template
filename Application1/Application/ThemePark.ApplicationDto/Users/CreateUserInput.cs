using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.ApplicationDto.Users
{
    [AutoMapTo(typeof(User))]
    public class CreateUserInput
    {
        [Required]
        [StringLength(User.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(User.MaxNameLength)]
        public string Name { get; set; }
        
        [EmailAddress]
        [StringLength(User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(User.MaxPasswordLength, MinimumLength = User.MinPasswordLength)]
        public string Password { get; set; }

        /// <summary>
        /// µç»°ºÅÂë
        /// </summary>
        [StringLength(User.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}