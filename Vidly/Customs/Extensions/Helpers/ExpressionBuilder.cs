using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Vidly.Customs.Models;

namespace Vidly.Customs.Extensions.Helpers
{
  public static class ExpressionBuilder
  {
    private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains");
    private static readonly MethodInfo StartsWithMethod =
    typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
    private static readonly MethodInfo EndsWithMethod =
    typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

    public static Expression<Func<T, bool>> GetExpression<T>(IList<Filter> filters)
    {
      if (filters.Count == 0)
        return null;

      var param = Expression.Parameter(typeof(T), "t");
      Expression exp = null;

      if (filters.Count == 1)
        exp = GetExpression<T>(param, filters[0]);
      else if (filters.Count == 2)
        exp = GetExpression<T>(param, filters[0], filters[1]);
      else
      {
        while (filters.Count > 0)
        {
          var f1 = filters[0];
          var f2 = filters[1];

          if (exp == null)
            exp = GetExpression<T>(param, filters[0], filters[1]);
          else
            exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

          filters.Remove(f1);
          filters.Remove(f2);

          if (filters.Count == 1)
          {
            exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
            filters.RemoveAt(0);
          }
        }
      }

      return Expression.Lambda<Func<T, bool>>(exp, param);
    }

    private static Expression GetExpression<T>(ParameterExpression param, Filter filter)
    {
      if (param == null) throw new ArgumentNullException(nameof(param));

      var member = Expression.Property(param, filter.PropertyName);
      var constant = Expression.Constant(filter.Value);

      return filter.Operation switch
      {
        Operator.EqualsTo => Expression.Equal(member, constant),
        Operator.GreaterThan => Expression.GreaterThan(member, constant),
        Operator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
        Operator.LessThan => Expression.LessThan(member, constant),
        Operator.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
        Operator.Contains => Expression.Call(member, ContainsMethod, constant),
        Operator.StartsWith => Expression.Call(member, StartsWithMethod, constant),
        Operator.EndsWith => Expression.Call(member, EndsWithMethod, constant),
        _ => null
      };
    }

    private static BinaryExpression GetExpression<T>
    (ParameterExpression param, Filter filter1, Filter filter2)
    {
      var bin1 = GetExpression<T>(param, filter1);
      var bin2 = GetExpression<T>(param, filter2);

      return Expression.AndAlso(bin1, bin2);
    }

    public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
    {

      var item = Expression.Parameter(typeof(T), "x");
      Expression member = item;
      foreach (var prop in propertyName.Split('.')) member = Expression.Property(member, prop);
      var constant = Expression.Constant(value);
      var left = propertyName.Split('.').Aggregate((Expression)item, Expression.Property);
      var body = MakeComparison(left, comparison, value);
      return Expression.Lambda<Func<T, bool>>(body, item);
    }
    public static Expression<Func<T, bool>> GetFilter<T>(string propertyName, string value)
    {
      var item = Expression.Parameter(typeof(T), "item");

      var member = propertyName.Split('.').Aggregate<string, Expression>(item, Expression.Property);

      var constant = Expression.Constant(value);
      var body = Expression.Call(member, ContainsMethod, constant);

      return Expression.Lambda<Func<T, bool>>(body, item);
    }

    private static Expression MakeComparison(Expression left, string comparison, string value)
    {
      switch (comparison.ToLower())
      {
        case "eq":
          return MakeBinary(ExpressionType.Equal, left, value);
        case "ne":
          return MakeBinary(ExpressionType.NotEqual, left, value);
        case "gt":
          return MakeBinary(ExpressionType.GreaterThan, left, value);
        case "gte":
          return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
        case "lt":
          return MakeBinary(ExpressionType.LessThan, left, value);
        case "lte":
          return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
        case "contains":
        case "starts-with":
        case "ends-with":
          return Expression.Call(MakeString(left), comparison, Type.EmptyTypes, Expression.Constant(value, typeof(string)));
        default:
          throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
      }
    }

    private static Expression MakeString(Expression source) => source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
    private static Expression MakeBinary(ExpressionType type, Expression left, string value)
    {
      object typedValue = value;
      if (left.Type != typeof(string))
      {
        if (string.IsNullOrEmpty(value))
        {
          typedValue = null;
          if (Nullable.GetUnderlyingType(left.Type) == null)
            left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
        }
        else
        {
          var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
          typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
            valueType == typeof(Guid) ? Guid.Parse(value) :
            Convert.ChangeType(value, valueType);
        }
      }
      var right = Expression.Constant(typedValue, left.Type);
      return Expression.MakeBinary(type, left, right);
    }
  }
}