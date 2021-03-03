using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ThemePark.Common
{
    public static class ExpressionExtension
    {
        public static MemberExpression Property(this Expression expression, string propertyName)
        {
            return Expression.Property(expression, propertyName);
        }

        public static List<PropertyInfo> GetComplexPropertyAccess(this LambdaExpression propertyAccessExpression)
        {
            List<PropertyInfo> propertyPath = propertyAccessExpression.Parameters.Single<ParameterExpression>()
                .MatchComplexPropertyAccess(propertyAccessExpression.Body);

            if (propertyPath == null)
                throw new InvalidExpressionException(propertyAccessExpression.Body + "is not valid Expression");

            return propertyPath;
        }

        private static List<PropertyInfo> MatchComplexPropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            return parameterExpression.MatchPropertyAccess(propertyAccessExpression);
        }

        private static List<PropertyInfo> MatchPropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
            MemberExpression memberExpression;
            do
            {
                memberExpression = propertyAccessExpression.RemoveConvert() as MemberExpression;

                PropertyInfo propertyInfo = memberExpression?.Member as PropertyInfo;
                if (propertyInfo == null)
                    return null;

                propertyInfoList.Insert(0, propertyInfo);
                propertyAccessExpression = memberExpression.Expression;
            }
            while (memberExpression.Expression != parameterExpression);

            return propertyInfoList;
        }

        public static Expression RemoveConvert(this Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }
    }
}
