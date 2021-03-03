using System;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 多园年卡续卡Dto
    /// </summary>
    [Serializable]
    public class MulYearCardRenewDto
    {
        /// <summary>
        /// 是否换新卡
        /// </summary>
        public bool ChangeCard { get; set; }

        public int ParkId { get; set; }

        public MulVipCardDto NewVIPCard { get; set; }

        public MulVipCardDto OldVIPCard { get; set; }

        public MulIcoperDetailDto IcoperDetail { get; set; }

        public MulFillCardDto FillCard { get; set; }
    }
}
