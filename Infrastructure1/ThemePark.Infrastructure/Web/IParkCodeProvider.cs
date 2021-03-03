namespace ThemePark.Infrastructure.Web
{
    public interface IParkCodeProvider
    {
        /// <summary>
        /// 获取公园编号
        /// </summary>
        /// <param name="parkId">The park identifier.</param>
        /// <returns>System.Int32.</returns>
        int ParkCode(int parkId);
    }
}
