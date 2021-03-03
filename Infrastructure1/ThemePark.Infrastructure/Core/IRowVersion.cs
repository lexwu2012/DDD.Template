namespace ThemePark.Infrastructure.Core
{
    public interface IRowVersion
    {
        byte[] Timespan { get; set; }
    }
}
