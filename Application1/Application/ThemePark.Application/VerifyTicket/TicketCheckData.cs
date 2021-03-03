using System;
using Abp.Dependency;
using Newtonsoft.Json;
using ThemePark.Application.VerifyTicket.Interfaces;
using ThemePark.Core.CoreCache.CacheItem;
using ThemePark.Core.InPark;
using ThemePark.Core.ParkSale;
using ThemePark.VerifyTicketDto.Dto;
using System.Threading.Tasks;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 检票数据
    /// </summary>
    public class TicketCheckData
    {
        /// <summary>
        ///  检票状态
        /// </summary>
        public CheckState CheckState { get; set; }

        /// <summary>
        /// 客户端的验票类型
        /// </summary>
        public VerifyType VerifyCodeType { get; set; }

        /// <summary>
        /// 客户端验票代码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 闸机号
        /// </summary>
        public int Terminal { get; set; }

        /// <summary>
        /// 服务端返回的验票类型
        /// </summary>
        public VerifyType VerifyType { get; set; }

        /// <summary>
        /// 验票消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 已入园总人数（所有公园）
        /// </summary>
        public int InPersons { get; set; }

        /// <summary>
        /// 返回给客户端的允许过闸次数
        /// </summary>
        public int AllowPersons { get; set; }

        /// <summary>
        /// 入园单允许入园总人数
        /// </summary>
        public int BillPersons { get; set; }

        /// <summary>
        /// 门票入园人数（单张门票）
        /// </summary>
        /// <returns></returns>
        public int? GetPersons()
        {
            return Qty * _ticketClassCacheItem?.Persons;
        }

        /// <summary>
        /// 票的状态
        /// </summary>
        public TicketSaleStatus TicketSaleStatus { get; set; }

        /// <summary>
        /// 入园单状态
        /// </summary>
        public InParkBillState InParkBillState { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 有效天数
        /// </summary>
        public int ValidDays { get; set; }

        /// <summary>
        /// 来源公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 票的数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 已入公园的ParkId拼接字符串
        /// </summary>
        public string InParkInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 区分普通票的类型
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 区分普通票的类型
        /// </summary>
        public int? TableId { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; set; }

        #region 基础票类缓存信息
        [JsonIgnore]
        private TicketClassCacheItem _ticketClassCacheItem;

        private Task<TicketClassCacheItem> SetTicketClassCacheItem()
        {
            var checkTicketManager = IocManager.Instance.Resolve<ICheckTicketManager>();
            //团体
            if (TableId == 1 || TableId == 2)
            {
                return checkTicketManager.GetAgencyTicketClassItem(ParkSaleTicketClassId);
            } 
            //散客或者年卡
            else if (TableId == 0 || VerifyType == VerifyType.MultiYearCard || VerifyType == VerifyType.VIPCard)
            {
                return checkTicketManager.GetParkTicketClassItem(ParkSaleTicketClassId);
            }

            throw new ApplicationException("wrong ticket. at TicketCheckData/SetTicketClassCacheItem");
        }

        /// <summary>
        /// 设置ticketClassCacheItem的值
        /// </summary>
        /// <returns></returns>
        public TicketClassCacheItem GetTicketClassCacheItem()
        {
            return _ticketClassCacheItem ?? (_ticketClassCacheItem = Nito.AsyncEx.AsyncContext.Run(SetTicketClassCacheItem));
        }

        /// <summary>
        /// 可入公园ID的字符串拼接
        /// </summary>
        /// <returns></returns>
        public string GetCanInParkIds()
        {
            return GetTicketClassCacheItem()?.CanInParkIds;
        }

        /// <summary>
        /// 票类名称
        /// </summary>
        /// <returns></returns>
        public string GetTicketClassName()
        {
            return GetTicketClassCacheItem()?.TicketClassName;
        }


        /// <summary>
        /// 入园规则
        /// </summary>
        /// <returns></returns>
        public InParkRuleCacheItem GetInParkRull()
        {
            return GetTicketClassCacheItem()?.RuleItem;
        }


        #endregion 基础票类缓存信息

        #region 服务端返回检票实体数据

        /// <summary>
        /// 服务端返回的验票数据
        /// </summary>
        public object TicketDto { get; set; }


        /// <summary>
        /// 普通票验票数据
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public VerifyCommonTicketDto CommonTicketDto
        {
            get
            {
                if (TicketDto == null)
                {
                    return null;
                }
                var dto = TicketDto as VerifyCommonTicketDto;
                if (dto != null)
                {
                    return dto;
                }

                var verifyCommonTicketDto = JsonConvert.DeserializeObject<VerifyCommonTicketDto>(TicketDto.ToString());
                TicketDto = verifyCommonTicketDto;
                return verifyCommonTicketDto;
            }
            set { TicketDto = value; }
        }

        /// <summary>
        /// 入园单验票数据
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public VerifyInparkBillDto InparkBillDto
        {
            get
            {
                if (TicketDto == null)
                {
                    return null;
                }
                var dto = TicketDto as VerifyInparkBillDto;
                if (dto != null)
                {
                    return dto;
                }
                var verifyInparkBillDto = JsonConvert.DeserializeObject<VerifyInparkBillDto>(TicketDto.ToString());
                TicketDto = verifyInparkBillDto;
                return verifyInparkBillDto;

            }
            set { TicketDto = value; }
        }

        /// <summary>
        /// 管理卡验票数据
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public VerifyManageCardDto ManageCardDto
        {
            get
            {
                if (TicketDto == null)
                {
                    return null;
                }
                var dto = TicketDto as VerifyManageCardDto;
                if (dto != null)
                {
                    return dto;
                }
                var verifyManageCardDto = JsonConvert.DeserializeObject<VerifyManageCardDto>(TicketDto.ToString());
                TicketDto = verifyManageCardDto;
                return verifyManageCardDto;
            }
            set { TicketDto = value; }
        }

        /// <summary>
        /// 获取套票验票数据
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public VerifyMultiTicketDto MultiTicketDto
        {
            get
            {
                if (TicketDto == null)
                {
                    return null;
                }
                var dto = TicketDto as VerifyMultiTicketDto;
                if (dto != null)
                {
                    return dto;
                }
                var verifyMultiTicketDto = JsonConvert.DeserializeObject<VerifyMultiTicketDto>(TicketDto.ToString());
                TicketDto = verifyMultiTicketDto;
                return verifyMultiTicketDto;
            }
            set { TicketDto = value; }
        }

        /// <summary>
        /// 指纹验票数据
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public VerifyFingerDto FingerDto
        {
            get
            {
                if (TicketDto == null)
                {
                    return null;
                }
                var dto = TicketDto as VerifyFingerDto;
                if (dto != null)
                {
                    return dto;
                }
                var verifyFingerDto = JsonConvert.DeserializeObject<VerifyFingerDto>(TicketDto.ToString());
                TicketDto = verifyFingerDto;
                return verifyFingerDto;
            }
            set { TicketDto = value; }

        }

        /// <summary>
        /// 二次入园票验票数据
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public VerifySecondTicketDto SecondTicketDto
        {
            get
            {
                if (TicketDto == null)
                {
                    return null;
                }
                var dto = TicketDto as VerifySecondTicketDto;
                if (dto != null)
                {
                    return dto;
                }
                var verifySecondTicketDto = JsonConvert.DeserializeObject<VerifySecondTicketDto>(TicketDto.ToString());
                TicketDto = verifySecondTicketDto;
                return verifySecondTicketDto;
            }

            set
            {
                TicketDto = value;
            }
        }

        /// <summary>
        /// 年卡验票数据
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public VerifyVIPCardDto VIPCardDto
        {
            get
            {
                if (TicketDto == null)
                {
                    return null;
                }
                var dto = TicketDto as VerifyVIPCardDto;
                if (dto != null)
                {
                    return dto;
                }
                var verifyVIPCardDto = JsonConvert.DeserializeObject<VerifyVIPCardDto>(TicketDto.ToString());
                TicketDto = verifyVIPCardDto;
                return verifyVIPCardDto;
            }
            set { TicketDto = value; }
        }
        #endregion 服务端返回检票实体数据        
    }

    /// <summary>
    /// 检票状态
    /// </summary>
    public enum CheckState
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 检票状态
        /// </summary>
        Checking = 10,

        /// <summary>
        /// 已使用状态
        /// </summary>
        Used = 20,

        /// <summary>
        /// 无效票
        /// </summary>
        Invalid = 30,

        /// <summary>
        /// 过期票
        /// </summary>
        Expired = 31,

        /// <summary>
        /// 二次入园
        /// </summary>
        SecondInPark = 40
    }
}
