using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Vidly.Customs.Extensions.Models;

namespace Vidly.Customs.Extensions
{
  public static class QueryableExtension
  {

    public static IQueryable<TEntity> SortBy<TEntity>(this IQueryable<TEntity> entity,
      QueryObject query, Dictionary<string, Expression<Func<TEntity, object>>> columnsMap)
    {

      if (!string.IsNullOrWhiteSpace(query.SortBy) && columnsMap.ContainsKey(query.SortBy))
      {
        return query.OrderBy.ToLower() == "asc"
          ? entity.OrderBy(columnsMap[query.SortBy])
          : entity.OrderByDescending(columnsMap[query.SortBy]);
      }
      return entity;
    }

    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> entity,
      string propertyName, string orderBy = "desc")
    {
      var orderType = orderBy.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";
      var entityType = typeof(TEntity);

      var propertyInfo = entityType.GetProperty(propertyName);

      var arg = Expression.Parameter(entityType, "x");
      var property = Expression.Property(arg, propertyName);

      var selector = Expression.Lambda(property, arg);

      var enumerableType = typeof(Queryable);
      var method = enumerableType.GetMethods()
        .Where(m => m.Name == orderType && m.IsGenericMethodDefinition)
        .Where(m =>
        {
          var parameters = m.GetParameters().ToList();
          return parameters.Count == 2;

        }).Single();


      var genericMethod = method.MakeGenericMethod(entityType, propertyInfo?.PropertyType);

      return (IOrderedQueryable<TEntity>)genericMethod
        .Invoke(genericMethod, new object[] { entity, selector });
    }

    public static IQueryable<TEntity> SortBy<TEntity>(this IQueryable<TEntity> entity,
      QueryObject query)
    {
      return entity.OrderBy(query.OrderBy, query.SortBy);
    }

    public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> entity,
      Expression<Func<TEntity, bool>> filter) =>
      entity.Where(filter);

    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> entity, QueryObject query)
    {
      var lastPage = Convert.ToInt32(Math.Ceiling(((double)entity.Count() / (double)query.PageSize)));
      var currentPage = query.Page;

      if (query.PageSize <= 0)
        return entity;

      if (currentPage > lastPage)
        query.Page = lastPage == 0 ? 1 : lastPage;

      if (currentPage <= -1)
        return entity;

      if (query.PageSize == 0 && currentPage == 0)
        return entity;

      if (query.PageSize != 0 && currentPage == 0)
        return entity.Take(query.PageSize);

      return entity.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);


    }

    public static IQueryable<TEntity> WhereNotIn<TEntity, TValue>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, TValue>> valueSelector,
        IEnumerable<TValue> values)
        where TEntity : class
    {
      if (queryable == null)
        throw new ArgumentNullException(nameof(queryable));

      if (valueSelector == null)
        throw new ArgumentNullException(nameof(valueSelector));

      if (values == null)
        throw new ArgumentNullException(nameof(values));

      var enumerable = values as TValue[] ?? values.ToArray();
      if (!enumerable.Any())
        return queryable.Where(e => true);

      var parameterExpression = valueSelector.Parameters.Single();

      var equals = from value in enumerable
                   select Expression.NotEqual(valueSelector.Body, Expression.Constant(value, typeof(TValue)));

      var body = equals.Aggregate(Expression.And);

      return queryable.Where(Expression.Lambda<Func<TEntity, bool>>(body, parameterExpression));
    }

    public static IQueryable<TEntity> Select<TEntity>(this IQueryable<TEntity> entity, params string[] properties)
    {
      var parameter = Expression.Parameter(typeof(TEntity), "e");

      var bindings = properties
        .Select(name => Expression.PropertyOrField(parameter, name))
        .Select(member => Expression.Bind(member.Member, member));

      var body = Expression.MemberInit(Expression.New(typeof(TEntity)), bindings);

      var selector = Expression.Lambda<Func<TEntity, TEntity>>(body, parameter);

      return entity.Select(selector);
    }

    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> entity, QueryObject query)
    {
      return entity.Where(BuildPredicate<TEntity>(query.SearchBy, query.Comparison, query.Search));
    }

    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> entity, string field, string condition, string value)
    {
      return entity.Where(BuildPredicate<TEntity>(field, condition, value));
    }

    private static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
    {
      var parameter = Expression.Parameter(typeof(T), "x");
      var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
      var body = MakeComparison(left, comparison, value);
      return Expression.Lambda<Func<T, bool>>(body, parameter);
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
        case "ge":
          return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
        case "lt":
          return MakeBinary(ExpressionType.LessThan, left, value);
        case "le":
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

    public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
    {
      using var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
      var relationalCommandCache = enumerator.Private("_relationalCommandCache");
      var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
      var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

      var sqlGenerator = factory.Create();
      var command = sqlGenerator.GetCommand(selectExpression);

      var sql = command.CommandText;
      return sql;
    }

    private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
  }
}