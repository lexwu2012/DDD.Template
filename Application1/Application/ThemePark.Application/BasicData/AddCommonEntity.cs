using Abp.Domain.Entities.Auditing;

namespace ThemePark.Application.BasicData
{
    public class AddCommonEntity<T> where T : FullAuditedEntity
    {
        public static T Add(T t)
        {
            return null;
        }
    }
}
