using System;
using ThemePark.Common;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 显示数据
    /// </summary>
    public class DisplayItem
    {
        public DisplayItem()
        {
            
        }

        public DisplayItem(Enum item)
        {
            Key = item.ToString("G");
            Value = item;
            Text = item.DisplayName();
            ShortName = item.DisplayShortName();
            Description = item.DisplayDescription();
            GroupName = item.DisplayGroupName();
            Order = item.DisplayOrder();
            Prompt = item.DisplayPrompt();
        }

        /// <summary>
        /// 键值
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 显示说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 对字段进行分。
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 列的排序权重
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// 显示水印的值
        /// </summary>
        public string Prompt { get; set; }
    }


    public class DisplayItem<TValue> : DisplayItem
    {
        /// <summary>
        /// 值
        /// </summary>
        public new TValue Value
        {
            get { return (TValue)base.Value; }
            set { base.Value = value; }
        }
    }
}
