using System;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Core.Model
{
    public class Book : FullAuditedEntity<string>, IAggregateRoot<string>
    {
        /// <summary>
        /// 书名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 借书者
        /// </summary>
        public int ReaderId { get; set; }

        /// <summary>
        /// 书所属序列号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 借书应还日期
        /// </summary>
        public DateTime ShouldReturnDate { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 书的状态
        /// </summary>
        public RegistrationStatus RegistrationStatus { get; set; }

        public virtual Reader Reader { get; set; }
        
        #region Methods

        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="readerId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public Result Lend(int readerId, string comment)
        {
            if (this.RegistrationStatus == RegistrationStatus.Lend)
                return Result.FromError("该书已被借出");

            this.RegistrationStatus = RegistrationStatus.Lend;

            ShouldReturnDate = DateTime.Now.AddDays(10);

            //持久化？？
            //todo: 这个应该放在这里还是读者的领域服务？
            //应该是放在这里，因为借书必然产生借书记录，而且如果放在外面，会造成遗漏，比如很多地方调用借书这个方法，有可能会有地方忘记写这个借书记录
            var history = new BookHistory(readerId, this.Id, RegistrationStatus.Lend, comment);

            return Result.Ok();
        }

        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        public Result Return(int readerId)
        {
            if (this.RegistrationStatus == RegistrationStatus.Lend)
                return Result.FromError("该书已被借出");

            this.RegistrationStatus = RegistrationStatus.Normal;

            if (DateTime.Today > ShouldReturnDate)
            {
                //todo: 扣款
            }

            //还书不一定有留言
            var history = new BookHistory(readerId, this.Id, RegistrationStatus.Normal,string.Empty);

            return Result.Ok();
        }

        #endregion
    }

    public enum RegistrationStatus
    {
        /// <summary>
        /// 正常状态（已返还）
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 已借出
        /// </summary>
        Lend = 2,
    }
}
