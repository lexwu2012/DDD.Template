using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 打印数据
    /// </summary>
    public class PrintInfo
    {
        /// <summary>
        /// 票面数据
        /// </summary>
        public TicketContent TicketContent { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        public string PrintTemplate { get; set; }

        /// <summary>
        /// 打印模板类型
        /// </summary>
        public PrintTemplateType PrintTemplateType { get; set; }
    }


    /// <summary>
    /// 票面打印内容
    /// </summary>
    public class TicketContent
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string Barcode = string.Empty;

        /// <summary>
        /// 预订订单号
        /// </summary>
        public string OrderId = string.Empty;
      

        /// <summary>
        /// 人数
        /// </summary>
        public string Persons = string.Empty;

        /// <summary>
        /// 总价
        /// </summary>
        public string Amount = string.Empty;

      
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime = string.Empty;

        /// <summary>
        /// 到期时间
        /// </summary>
        public string EndTime = string.Empty;

        /// <summary>
        /// 生成时间
        /// </summary>
        public string CreationTime = string.Empty;

        /// <summary>
        /// 防伪码
        /// </summary>
        public string Pbogus = string.Empty;

        /// <summary>
        /// 二维码
        /// </summary>
        public string QrCode = string.Empty;

        /// <summary>
        /// 公园名称
        /// </summary>
        public string ParkName = string.Empty;


        /// <summary>
        /// 单位
        /// </summary>
        public string Company = string.Empty;

        /// <summary>
        /// 工作类别
        /// </summary>
        public string WorkType = string.Empty;

        /// <summary>
        /// 事由
        /// </summary>
        public string Reasons = string.Empty;

      
        /// <summary>
        /// 限制入园时间
        /// </summary>
        public string Limittime = string.Empty;

        /// <summary>
        /// 申请部门
        /// </summary>
        public string ApplyDept = string.Empty;

        /// <summary>
        /// 申请人
        /// </summary>    
        public string ApplyBy = string.Empty;

        /// <summary>
        /// 入园类型(工作，参观)
        /// </summary>
        public string InParkType = string.Empty;

        /// <summary>
        /// 入园通道
        /// </summary>
        public string InParkChannel = string.Empty;

        /// <summary>
        /// 入园时段
        /// </summary>
        public string InParkTime = string.Empty;

        /// <summary>
        /// 入园须知
        /// </summary>
        public string InparkNotice = string.Empty;

        /// <summary>
        /// 审批人
        /// </summary>
        public string ApprovedBy= string.Empty;

        /// <summary>
        /// 入园单编码
        /// </summary>
        public string BillNo = string.Empty;

        /// <summary>
        /// 有效开始日期
        /// </summary>    
        public string ValidStartDate = string.Empty;


        /// <summary>
        /// 有效人数
        /// </summary>
        public string PersonNum = string.Empty;

        /// <summary>
        /// 团体名称
        /// </summary>
        public string GroupName = string.Empty;

        /// <summary>
        /// 团体类型名称
        /// </summary>
        public string GroupTypeName = string.Empty;

        /// <summary>
        /// 票价
        /// </summary>
        public string Price = string.Empty;

        /// <summary>
        /// 票面标示
        /// </summary>
        public string TicketMarker = string.Empty;

        /// <summary>
        /// 条形码标示
        /// </summary>
        public string BarcodeMarker = string.Empty;

        /// <summary>
        /// 有效天数
        /// </summary>
        public string ValidDate = string.Empty;

        /// <summary>
        /// 备注1
        /// </summary>
        public string Remark1 = string.Empty;

        /// <summary>
        /// 备注2
        /// </summary>
        public string Remark2 = string.Empty;

        //入园单专用
        /// <summary>
        /// 有效天数
        /// </summary>
        public int ValidDays { get; set; }

        /// <summary>
        /// 备注（入园单专用）
        /// </summary>
        public string Remark = string.Empty;


    }


    /// <summary>
    /// 打印设置
    /// </summary>
    public class PrintSet
    {
        /// <summary>
        /// 默认打印价格类型
        /// </summary>
        public PrintPriceType PrintPriceType { get; set; }

        /// <summary>
        /// 默认有效期
        /// </summary>
        public int ValidDays { get; set; }

        /// <summary>
        /// 团体是否打印价格
        /// </summary>
        public bool? IsPrintPrice { get; set; }


        /// <summary>
        /// 是否打印团体名称
        /// </summary>
        public bool? IsPrintAgencyName { get; set; }

    }


}
