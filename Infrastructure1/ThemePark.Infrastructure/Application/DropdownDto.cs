using System.Collections.Generic;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 只包含指定文本和值的对象 的列表
    /// </summary>
    /// <typeparam name="TPrimaryKey">The type of the t primary key.</typeparam>
    /// <seealso cref="System.Collections.Generic.List{ThemePark.ApplicationDto.DropdownItem{TPrimaryKey}}"/>
    public class DropdownDto<TPrimaryKey> : List<DropdownItem<TPrimaryKey>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropdownDto{TPrimaryKey}"/> class.
        /// </summary>
        /// <param name="collection">一个集合，其元素被复制到新列表中。</param>
        public DropdownDto(IEnumerable<DropdownItem<TPrimaryKey>> collection) : base(collection)
        {

        }
    }

    /// <summary>
    /// 只包含指定文本和值的对象 的列表
    /// </summary>
    public class DropdownDto : DropdownDto<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropdownDto" /> class.
        /// </summary>
        /// <param name="collection">一个集合，其元素被复制到新列表中。</param>
        public DropdownDto(IEnumerable<DropdownItem> collection) : base(collection)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropdownDto" /> class.
        /// </summary>
        /// <param name="collection">一个集合，其元素被复制到新列表中。</param>
        public DropdownDto(IEnumerable<DropdownItem<int>> collection) : base(collection)
        {

        }
    }
}