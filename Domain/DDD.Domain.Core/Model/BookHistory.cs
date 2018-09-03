using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    /// <summary>
    /// 从表面上看，BookHistory同样属于实体对象（一般中间表作为实体而不是聚合），它的每一条记录都是唯一的，即使存在两条历史记录，具有相同的读者ID与书籍ID，
    /// 我们仍将其视为不同的记录，因为它们的借阅时间并不相同。不过，对于系统的调用者而言，通常不会去关注所有的借阅记录，而是查询某位读者的借阅记录，
    /// 因此，我们可以将其作为与Reader放在一起的聚合。然而，随着对需求的深入分析，我们发现定义这样的聚合存在问题，
    /// 因为我们可能还需要查询某本书的借阅记录（例如，希望知道哪本书最受欢迎，跟踪每本书的借阅情况等）。
    /// 由于Reader和Book应该分属于不同的聚合，BookHistory就存在无法划定聚合的问题。既然如此，我们应该将其分离出来，作为一个单独的聚合根。
    /// </summary>
    public class BookHistory : FullAuditedEntity<string>, IAggregateRoot<string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerId"></param>
        /// <param name="bookId"></param>
        /// <param name="status"></param>
        /// <param name="comment"></param>
        public BookHistory(int readerId, string bookId, RegistrationStatus status,string comment)
        {
            BookId = bookId;
            ReaderId = readerId;
            Status = status;
            Remark = comment;
        }

        /// <summary>
        /// 涉及的书
        /// </summary>
        public string BookId { get; set; }

        /// <summary>
        /// 涉及的用户
        /// </summary>
        public int ReaderId { get; set; }

        /// <summary>
        /// 该次是借出还是返还
        /// </summary>
        public RegistrationStatus Status { get; set; }

        /// <summary>
        /// 什么人借的书
        /// </summary>
        public virtual Reader Reader { get; set; }

        /// <summary>
        /// 借的什么书
        /// </summary>
        public virtual Book Book { get; set; }

        /// <summary>
        /// 借书留言
        /// </summary>
        public string Remark { get; set; }
    }
}
