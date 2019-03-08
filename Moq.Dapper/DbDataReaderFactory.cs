using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Moq.Dapper
{
    static class DbDataReaderFactory
    {
        internal static DbDataReader DbDataReader<TResult>(Func<TResult> result)
        {
            var val = result();
            switch (val)
            {
                case IEnumerable enumerable:
                    return new DataTableReader(EnumerableToDataTable<TResult>(enumerable));

                default:
                    return val.ToDataTable(typeof(TResult)).ToDataTableReader();
            }
        }

        private static DataTable EnumerableToDataTable<TResult>(IEnumerable enumerable)
        {
            var dataTable = new DataTable();

            // Assuming SqlMapper.Query returns always generic IEnumerable<TResult>.
            var type = typeof(TResult).GenericTypeArguments.First();

            if (type.IsPrimitive || type == typeof(string))
            {
                dataTable.Columns.Add();

                foreach (var element in enumerable)
                    dataTable.Rows.Add(element);
            }
            else
            {
                bool IsNullable(Type t) =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(Nullable<>);

                Type GetDataColumnType(Type source) =>
                    IsNullable(source) ?
                        Nullable.GetUnderlyingType(source) :
                        source;

                bool IsMatchingType(Type t) =>
                    t.IsPrimitive ||
                    t == typeof(DateTime) ||
                    t == typeof(DateTimeOffset) ||
                    t == typeof(decimal) ||
                    t == typeof(Guid) ||
                    t == typeof(string) ||
                    t == typeof(TimeSpan) ||
                    t == typeof(byte[]);

                var properties =
                    type.GetProperties()
                        .Where(info => info.CanRead &&
                                       IsMatchingType(info.PropertyType) ||
                                       IsNullable(info.PropertyType) &&
                                       IsMatchingType(Nullable.GetUnderlyingType(info.PropertyType)))
                        .ToList();

                var columns = properties.Select(property => new DataColumn(property.Name, GetDataColumnType(property.PropertyType))).ToArray();

                dataTable.Columns.AddRange(columns);

                var valuesFactory = properties.Select(info => (Func<object, object>)info.GetValue).ToArray();

                foreach (var element in enumerable)
                    dataTable.Rows.Add(valuesFactory.Select(getValue => getValue(element)).ToArray());
            }

            return dataTable;
        }
    }
}