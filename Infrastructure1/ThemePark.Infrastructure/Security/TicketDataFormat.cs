using ThemePark.Infrastructure.Security.DataProtection;
using ThemePark.Infrastructure.Security.Serializer;
using ThemePark.Infrastructure.Security.Serializer.Encoder;

namespace ThemePark.Infrastructure.Security
{
    /// <summary>
    /// 身份验证凭据数据格式化
    /// </summary>
    public class TicketDataFormat : SecureDataFormat<AuthenticationTicket>
    {
        /// <summary>
        /// 身份验证凭据数据格式化
        /// </summary>
        public TicketDataFormat(IDataProtector protector)
          : base(DataSerializers.Ticket, protector, TextEncodings.Base64Url)
        {

        }
    }
}
