using Abp.Dependency;
using ThemePark.EntityFramework;
using System;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;
using Newtonsoft.Json;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 要将老系统余下的票导入到数据表TicketLeft中
    /// </summary>
    public class TicketLeft
    {
        public string barcode { get; set; }
        public string tickettypename { get; set; }
        public int persons { get; set; }
        public DateTime startdate { get; set; }
        public int validdays { get; set; }
        public int status { get; set; }
        public string remark { get; set; }

    }

    /// <summary>
    /// 验老系统余票的策略
    /// </summary>
    public static class CheckLeftTicketStrategy
    {
        /// <summary>
        /// 写入园记录
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="noPast"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public static async Task<bool> WriteInPark(string verifyCode, int noPast, int terminal)
        {
            var oldPersons = await GetOldPersons(verifyCode);
            if (oldPersons == 0 || noPast == oldPersons)
            {
                return true;
            }


            int status = 0;
            if (noPast > 0)
            {
                status = 1;
            }
            string remark = DateTime.Today.ToString("yyyy-MM-dd") + " " + noPast + " " + terminal + ", ";
            string sql = "update TicketLeft set persons=" + noPast
                + ", remark=remark+'" + remark + "', status=" + status
                + " where  barcode='" + verifyCode.Substring(0, 12) + "'";

            //确保释放对象，防止内存泄漏
            using (var dbContext = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
            {
                return await dbContext.Object.Database.ExecuteSqlCommandAsync(sql) > 0;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public static async Task<int> GetOldPersons(string barcode)
        {
            // 根据条码查询票信息
            string sql = "select * from TicketLeft where barcode='" + barcode.Substring(0, 12) + "'";
            //确保释放对象，防止内存泄漏
            using (var dbContext = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
            {
                var ticket = await dbContext.Object.Database.SqlQuery<TicketLeft>(sql).FirstOrDefaultAsync();

                if (ticket != null && ticket.persons > 0)
                {
                    return ticket.persons;
                }
                else
                {
                    return 0;
                }
            }

        }


        /// <summary>
        /// 验老系统的条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public static async Task<Result<VerifyDto>> Verify(string barcode, int terminal)
        {
            // 根据条码查询票信息
            string sql = "select * from TicketLeft where barcode='" + barcode.Substring(0, 12) + "'";
            //确保释放对象，防止内存泄漏
            using (var dbContext = IocManager.Instance.ResolveAsDisposable<ThemeParkDbContext>())
            {
                var ticket = await dbContext.Object.Database.SqlQuery<TicketLeft>(sql).FirstOrDefaultAsync();

                if (ticket == null)
                    return Failed(barcode, VerifyType.CommonTicket, "无效票");

                if (ticket.status == 1
                    && ticket.persons > 0
                    && DateTime.Today - ticket.startdate.Date <= TimeSpan.FromDays(ticket.validdays))
                {
                    var dto = new VerifyCommonTicketDto()
                    {
                        Id = barcode,
                        DisplayName = ticket.tickettypename,
                        Persons = ticket.persons,
                        Remark = ""
                    };

                    var verifyDto = new VerifyDto
                    {
                        VerifyType = VerifyType.CommonTicket,

                        VerifyData = JsonConvert.SerializeObject(dto),
                        VerifyCode = barcode
                    };
                    var result = Result.FromData(verifyDto);
                    return result;
                }

                return Failed(barcode, VerifyType.CommonTicket, "无效票");

            }
        }

        private static Result<VerifyDto> Failed(string verifyCode, VerifyType verifyType, string message)
        {
            var dto = new VerifyDto()
            {
                VerifyCode = verifyCode,
                VerifyType = verifyType,
                VerifyData = ""
            };
            var result = Result.FromData(dto);
            result.Message = message;
            result.Code = ResultCode.Fail;
            return result;
        }
    }
}
