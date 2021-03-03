namespace ThemePark.Infrastructure.Security.Serializer
{
    /// <summary>
    /// 定义数据序列化接口
    /// </summary>
    /// <typeparam name="TModel">序列化的数据</typeparam>
    public interface IDataSerializer<TModel>
    {
        /// <summary>
        /// 序列化数据
        /// </summary>
        byte[] Serialize(TModel model);

        /// <summary>
        /// 反序列化数据
        /// </summary>
        TModel Deserialize(byte[] data);
    }
}
