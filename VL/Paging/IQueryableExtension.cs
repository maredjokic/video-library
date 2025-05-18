using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Video_Library_Api.Models;

namespace Video_Library_Api.Paging
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> source, int page, int pageSize)
        {
            return source
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, string AscDesc)
        {
            if(AscDesc=="DESC")
                return source.OrderByDescending(ToLambda<T>(propertyName));
            else
                return source.OrderBy(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        public static object GetPropertyDynamic<Tobj>(this Tobj self, string propertyName) where Tobj : class
        {
            var param = Expression.Parameter(typeof(Tobj), "value");
            var getter = Expression.Property(param, propertyName);
            var boxer = Expression.TypeAs(getter, typeof(object));
            var getPropValue = Expression.Lambda<Func<Tobj, object>>(boxer, param).Compile();
            return getPropValue(self);
        }

        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }
    }
}
