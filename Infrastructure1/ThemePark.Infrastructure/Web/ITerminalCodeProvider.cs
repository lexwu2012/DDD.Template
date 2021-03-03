namespace ThemePark.Infrastructure.Web
{
    public interface ITerminalCodeProvider
    {
        /// <summary>
        /// 获取终端编号
        /// </summary>
        /// <param name="terminalId">The terminal identifier.</param>
        /// <returns>System.Int32.</returns>
        int TerminalCode(int terminalId);
    }
}
