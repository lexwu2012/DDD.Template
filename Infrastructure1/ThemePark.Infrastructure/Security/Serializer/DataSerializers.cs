namespace ThemePark.Infrastructure.Security.Serializer
{
    /// <summary>
    /// 数据序列化
    /// </summary>
    public static class DataSerializers
    {
        /// <summary>
        /// <see cref="AuthenticationTicket"/> 序列化工具
        /// </summary>
        public static IDataSerializer<AuthenticationTicket> Ticket { get; set; }

        /// <summary>
        /// <see cref="AuthenticationProperties"/> 序列化工具
        /// </summary>
        public static IDataSerializer<AuthenticationProperties> Properties { get; set; } 


        static DataSerializers()
        {
            Ticket = new TicketSerializer();
            Properties = new PropertiesSerializer();
        }
    }
}
