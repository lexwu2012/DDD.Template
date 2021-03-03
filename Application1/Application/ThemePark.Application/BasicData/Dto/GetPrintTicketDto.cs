using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapFrom(typeof(ParkSaleTicketClass))]
    public class GetPrintTicketDto
    {
        /// <summary>
        /// 促销票类ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 促销票类名称
        /// </summary>
        public string SaleTicketClassName { get; set; }

    }

    public static class PrintTicketDtoEx
    {
        /// <summary>
        /// 转换类型，把TicketClassMode票类型转为PrintTicketMode打印票类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<GetPrintTicketDto> TODtos(this List<ParkSaleTicketClass> source)
        {
            List<GetPrintTicketDto> dtos = new List<GetPrintTicketDto>();
           
            foreach (var entity in source)
            {
                var dto = new GetPrintTicketDto()
                {
                    Id = entity.Id,
                    SaleTicketClassName = entity.SaleTicketClassName,
                };

                dtos.Add(dto);
            }
            //添加入园单
            dtos.Add(new GetPrintTicketDto()
            {
                Id = PrintTemplateSetting.ExcessFareTicketClasId,
                SaleTicketClassName = PrintTemplateSetting.ExcessFareTicketName,

            });

            //添加补票
            dtos.Add(new GetPrintTicketDto()
            {
                Id = PrintTemplateSetting.InParkBillTicketClassId,
                SaleTicketClassName = PrintTemplateSetting.InParkBillName,
            });

            return dtos;
        }
    }
}
