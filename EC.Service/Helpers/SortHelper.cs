using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;

namespace EC.Service.Helpers
{
    public  class SortHelper<T>
    {
        public  static IEnumerable<T> ApplySort(IEnumerable<T> source, string orderBy)
        {
            if (!source.Any())
                return source;

            if (string.IsNullOrWhiteSpace(orderBy))
                return source;
            var OrderParms = orderBy.Trim().Split(',');

            var propertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var OrderQueryBuilder = new StringBuilder();
            foreach (var param in OrderParms)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(' ')[0];

                var ObjectProperty = propertyInfo.FirstOrDefault(p => p.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (ObjectProperty == null)
                    continue;

                var sortingOrder = param.EndsWith(" desc") ? " descending" : " ascending";

                OrderQueryBuilder.Append($"{ObjectProperty.Name}{sortingOrder}, ");

            }
            var orderQuery = OrderQueryBuilder.ToString().TrimEnd(',', ' ');
            if (string.IsNullOrWhiteSpace(orderQuery))
                return source;

            return source;
        }

    }
}
