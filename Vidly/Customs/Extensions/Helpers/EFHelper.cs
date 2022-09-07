using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Vidly.Customs.Extensions.Helpers
{
    
    public static class EfHelper
    {
        public static Func<T, T> DynamicSelectGenerator<T>(string fields = "")
        {
            var entityFields = fields == "" ? typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToArray() : fields.Split(',');

            // input parameter "o"
            var xParameter = Expression.Parameter(typeof(T), "o");

            // new statement "new Data()"
            var xNew = Expression.New(typeof(T));

            // create initializers
            var bindings = entityFields.Select(o => o.Trim())
                .Select(o => {

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
        
        static Expression<Func<TSource, dynamic>> DynamicFields<TSource>(IEnumerable<string> fields)
        {
            var source = Expression.Parameter(typeof(TSource), "o");
            var properties = fields
                .Select(f => typeof(TSource).GetProperty(f))
                .Select(p => new DynamicProperty(p.Name, p.PropertyType))
                .ToList();
            var resultType = DynamicClassFactory.CreateType(properties, false);
            var bindings = properties.Select(p => Expression.Bind(resultType.GetProperty(p.Name), Expression.Property(source, p.Name)));
            var result = Expression.MemberInit(Expression.New(resultType), bindings);
            return Expression.Lambda<Func<TSource, dynamic>>(result, source);
        }
        
        
    }
}