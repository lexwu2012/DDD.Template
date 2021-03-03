using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Castle.Components.DictionaryAdapter;
using ThemePark.Application.SaleTicekt.Interfaces;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 打印门票模型
    /// </summary>
    public class RePrintTicketModel
    {
        /// <summary>
        /// cotr
        /// </summary>
        public RePrintTicketModel()
        {
            Inputs = new EditableList<RePrintTicketAddInput>();
        }

        /// <summary>
        /// 是否重新生成发票号
        /// </summary>
        [Required]
        public bool RenewInvoice { get; set; }

        /// <summary>
        /// 打印输入
        /// </summary>
        public List<RePrintTicketAddInput> Inputs { get; set; }

        /// <summary>
        /// 打印的种类（散客、团体、网络、年卡。。）
        /// </summary>
        public PrintTicketType PrintTicketType { get; set; }

        /// <summary>
        /// 输入的发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 重打印说明
        /// </summary>
        public string Remark { get; set; }

    }
}
