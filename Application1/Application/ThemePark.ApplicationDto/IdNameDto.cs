using System.Collections.Generic;

namespace ThemePark.ApplicationDto
{
    public class IdNameDto : IdNameDto<int>, IEqualityComparer<IdNameDto>
    {
        public bool Equals(IdNameDto x, IdNameDto y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;

            return int.Equals(x.Id, y.Id);
        }

        public int GetHashCode(IdNameDto obj)
        {
            return obj?.Id.GetHashCode() ?? 0;
        }
    }


    public class IdNameDto<T> : IEqualityComparer<IdNameDto<T>>
    {
        public T Id { get; set; }
        
        public string Name { get; set; }

        public bool Equals(IdNameDto<T> x, IdNameDto<T> y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;

            return EqualityComparer<T>.Default.Equals(x.Id, y.Id);
        }

        public int GetHashCode(IdNameDto<T> obj)
        {
            return obj?.Id.GetHashCode() ?? 0;
        }
    }
}
