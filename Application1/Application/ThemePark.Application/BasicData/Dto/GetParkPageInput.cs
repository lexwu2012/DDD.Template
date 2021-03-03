using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapTo(typeof(Park))]
    public class GetParkPageInput
    {
        public int? Id { get; set; }
        /// <summary>
        /// 公园名称
        /// </summary>    
        public string ParkName { get; set; }
        /// <summary>
        /// 城市编号
        /// </summary>    
        public int? CityId { get; set; }
        /// <summary>
        /// Tel
        /// </summary>    
        public string Tel { get; set; }
        /// <summary>
        /// Email
        /// </summary>    
        public string Email { get; set; }
        /// <summary>
        /// Address
        /// </summary>    
        public string Address { get; set; }
        /// <summary>
        /// Fax
        /// </summary>    
        public string Fax { get; set; }
        /// <summary>
        /// Remark
        /// </summary>    
        public string Remark { get; set; }
        
    }
}

