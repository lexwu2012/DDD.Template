using System.Linq.Expressions;

namespace ThemePark.Infrastructure.Application
{
    public class SortField
    {
        public string Field { get; set; }

        public Sortord Sortord { get; set; }

        public LambdaExpression Selector { get; set; }

        public SortField(LambdaExpression selector, Sortord sortord)
        {
            Selector = selector;
            Sortord = sortord;
        }
    }

    public enum Sortord
    {
        Asc = 1,
        Desc = 2
    }
}
