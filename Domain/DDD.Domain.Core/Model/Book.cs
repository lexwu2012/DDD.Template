using System;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    public class Book : FullAuditedEntity<string>, IAggregateRoot<string>
    {
        /// <summary>
        /// 书名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 书的状态
        /// </summary>
        public RegistrationStatus RegistrationStatus { get; set; }
        public string Remark { get ; set; }

        #region Methods

        /// <summary>
        /// 设置该书已借出
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public bool Lend(Book book)
        {
            if (book.RegistrationStatus == RegistrationStatus.Lend)
                throw new Exception();

            book.RegistrationStatus = RegistrationStatus.Lend;

            //持久化？？

            return true;
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
