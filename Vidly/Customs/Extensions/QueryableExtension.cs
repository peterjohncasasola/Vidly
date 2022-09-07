using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using log4net;
using NLog;
using Vidly.Customs.Extensions.Helpers;
using Vidly.Customs.Extensions.Models;
using WebGrease;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;


namespace Vidly.Customs.Extensions
{
  public static class QueryableExtension
  {
    private static readonly MethodInfo Contains = typeof(string).GetMethod("Contains");
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

    private static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> entity,
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
    
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> entity, QueryObject query)
    {
      var lastPage = Convert.ToInt32(Math.Ceiling(((double)entity.Count() / (double)query.PageSize)));
      var currentPage = query.Page;

      if (query.Page <=0)
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
    
    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> entity, QueryObject query)
    {
      return entity.Where(BuildPredicate<TEntity>(query.SearchBy, query.Comparison, query.Search));
    }
    
    public static IQueryable<TEntity> WhereContains<TEntity>(this IQueryable<TEntity> entity, QueryObject query)
    {
      return entity.Where(GetFilter<TEntity>(query.SearchBy, query.Search));
    }
    
    public static IQueryable SelectColumns(this IQueryable entity, string columns)
    {
      var strColumns = "new { " + columns + " }";
      return entity.Select(strColumns);
    }
    
    // public static IQueryable<T> Select<T>(
    //   this IQueryable source,
    //   string selector,
    //   params object[] args)
    // {
    //   return source.Select(ParsingConfig.Default, selector, args);
    // }
    
    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> entity, string field, string condition, string value)
    {
      return entity.Where(BuildPredicate<TEntity>(field, condition, value));
    }
    private static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
    {
      var item = Expression.Parameter(typeof(T), "x");
      Expression member = item;
      foreach (var prop in propertyName.Split('.')) member = Expression.Property(member, prop);
      var constant = Expression.Constant(value);
      var left = propertyName.Split('.').Aggregate((Expression)item, Expression.Property);
      var body = MakeComparison(left, comparison, value);
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
    
   
    
    private static Expression<Func<T, bool>> GetFilter<T>(string propertyName, string value)
    {
      var item = Expression.Parameter(typeof(T), "item");

      var member = propertyName.Split('.').Aggregate<string, Expression>(item, Expression.Property);

      var constant = Expression.Constant(value);
      var body = Expression.Call(member, Contains, constant);

      return Expression.Lambda<Func<T, bool>>(body, item);
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

    
    
    // public static IQueryable SelectDynamic(this IQueryable source, IEnumerable<string> fieldNames)
    // {
    //   var sourceProperties = fieldNames.ToDictionary(name => name, name => source.ElementType.GetProperty(name));
    //   var dynamicType = LinqRuntimeTypeBuilder.GetDynamicType(sourceProperties.Values);
    //
    //   var sourceItem = Expression.Parameter(source.ElementType, "t");
    //   var bindings = dynamicType.GetFields().Select(p => Expression.Bind(p, Expression.Property(sourceItem, sourceProperties[p.Name]))).OfType<MemberBinding>();
    //
    //   Expression selector = Expression.Lambda(Expression.MemberInit(
    //     Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)!), bindings), sourceItem);
    //
    //   return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select", new Type[] { source.ElementType, dynamicType },
    //     source.Expression, selector));
    // }
    //
    // private static class LinqRuntimeTypeBuilder
    // {
    //   private static readonly ILog Log =
    //     log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    //   private static readonly AssemblyName AssemblyName = new AssemblyName() { Name = "DynamicLinqTypes" };
    //   private static readonly ModuleBuilder ModuleBuilder = null;
    //   private static readonly Dictionary<string, Type> BuiltTypes = new Dictionary<string, Type>();
    //
    // static LinqRuntimeTypeBuilder()
    // {
    //   ModuleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
    // }
    //
    // private static string GetTypeKey(Dictionary<string, Type> fields)
    // {
    //     var key = string.Empty;
    //     foreach (var field in fields)
    //         key += field.Key + ";" + field.Value.Name + ";";
    //
    //     return key;
    // }
    //
    // private static Type GetDynamicType(Dictionary<string, Type> fields)
    // {
    //     if (null == fields)
    //         throw new ArgumentNullException(nameof(fields));
    //     if (0 == fields.Count)
    //         throw new ArgumentOutOfRangeException(nameof(fields), @"fields must have at least 1 field definition");
    //
    //     try
    //     {
    //         Monitor.Enter(BuiltTypes);
    //         var className = GetTypeKey(fields);
    //
    //         if (BuiltTypes.ContainsKey(className))
    //             return BuiltTypes[className];
    //
    //         var typeBuilder = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
    //
    //         foreach (var field in fields)                    
    //             typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);
    //
    //         BuiltTypes[className] = typeBuilder.CreateType();
    //
    //         return BuiltTypes[className];
    //     }
    //     catch (Exception ex)
    //     {
    //         Log.Error(ex);
    //     }
    //     finally
    //     {
    //         Monitor.Exit(BuiltTypes);
    //     }
    //
    //     return null;
    // }
    //
    //
    // public static Type GetDynamicType(IEnumerable<PropertyInfo> fields)
    // {
    //     var type = GetDynamicType(fields.ToDictionary(f => f.Name, f => f.PropertyType));
    //     return type;
    // }
  //}
    /*
    private static Expression<Func<T>> DynamicSelectGenerator<T>(params string[] fields)
    {
        var entityFields = fields;
        if (fields == null || fields.Length == 0)
            // get Properties of the T
            entityFields = typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToArray();

        // input parameter "x"
        var xParameter = Expression.Parameter(typeof(T), "x");

        // new statement "new Data()"
        var xNew = Expression.New(typeof(T));

        // create initializers
        var bindings = entityFields
            .Select(x =>
            {
                var xFieldAlias = x.Split(':');
                var field = xFieldAlias[0];

                var fieldSplit = field.Split('.');
                if (fieldSplit.Length > 1)
                {
                    // original value "x.Nested.Field1"
                    Expression exp = xParameter;
                    foreach (var item in fieldSplit)
                        exp = Expression.PropertyOrField(exp, item);

                    // property "Field1"
                    PropertyInfo member2 = null;
                    member2 = xFieldAlias.Length > 1 ? typeof(T).GetProperty(xFieldAlias[1]) : typeof(T).GetProperty(fieldSplit[fieldSplit.Length - 1]);

                    // set value "Field1 = x.Nested.Field1"
                    if (member2 != null)
                    {
                      var res = Expression.Bind(member2, exp);
                      return res;
                    }
                }
                // property "Field1"
                var mi = typeof(T).GetProperty(field);
                PropertyInfo member;
                if (xFieldAlias.Length > 1)
                    member = typeof(T).GetProperty(xFieldAlias[1]);
                else member = typeof(T).GetProperty(field);

                // original value "x.Field1"
                var xOriginal = Expression.Property(xParameter, mi);

                // set value "Field1 = x.Field1"
                return Expression.Bind(member, xOriginal);
            }
        );

        // initialization "new Data { Field1 = x.Field1, Field2 = x.Field2 }"
        var xInit = Expression.MemberInit(xNew, bindings);

        // expression "x => new Data { Field1 = x.Field1, Field2 = x.Field2 }"
        var lambda = Expression.Lambda<Func<T>>(xInit, xParameter);

        return lambda;
    }

    private static Func<T, T> DynamicSelect<T>(string[] fields )
    {
      if (fields == null) throw new ArgumentNullException(nameof(fields));
      // get Properties of the T
      fields = typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToArray();

      // input parameter "o"
      var xParameter = Expression.Parameter(typeof(T), "o");

      // new statement "new Data()"
      var xNew = Expression.New(typeof(T));

      // create initializers
      var bindings = fields.Split( ',' ).Select(o => o.Trim())
        .Select(o =>
          {

            // property "Field1"
            var mi = typeof(T).GetProperty(o);

            // original value "o.Field1"
            var xOriginal = Expression.Property(xParameter, mi);

            // set value "Field1 = o.Field1"
            return Expression.Bind(mi, xOriginal);
          }
        );

      // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
      var xInit = Expression.MemberInit(xNew, bindings);

      // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
      var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

      // compile to Func<Data, Data>
      return lambda.Compile();
    }
    */
  }
  
}