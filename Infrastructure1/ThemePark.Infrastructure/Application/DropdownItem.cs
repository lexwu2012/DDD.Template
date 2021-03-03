namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 只包含指定文本和值的对象
    /// </summary>
    /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
    public class DropdownItem<TPrimaryKey>
    {
        #region Properties

        /// <summary>
        /// 获取或设置选定项的文本。
        /// </summary>
        /// <returns>文本。</returns>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置选定项的值。
        /// </summary>
        /// <returns>值。</returns>
        public TPrimaryKey Value { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// 只包含指定文本和值的对象
    /// </summary>
    public class DropdownItem : DropdownItem<int>
    {
    }
}