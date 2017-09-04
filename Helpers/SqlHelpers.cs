using Boerman.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;

namespace Boerman.Core.Helpers
{
	public static class SqlHelpers
    {
        private static readonly IReadOnlyDictionary<Type, DbType> TypeMappings = new Dictionary<Type, DbType>()
        {
            { typeof(byte), DbType.Byte},
            { typeof(sbyte), DbType.Int16},
            { typeof(ushort), DbType.UInt16},
            { typeof(int), DbType.Int32},
            { typeof(uint), DbType.UInt32},
            { typeof(long), DbType.Int64},
            { typeof(ulong), DbType.UInt64 },
            { typeof(float), DbType.Single },
            { typeof(double), DbType.Double},
            { typeof(decimal), DbType.Decimal},
            { typeof(bool), DbType.Boolean},
            { typeof(string), DbType.String },
            { typeof(char), DbType.StringFixedLength},
            { typeof(Guid), DbType.Guid},
            { typeof(DateTime), DbType.DateTime},
            { typeof(DateTimeOffset), DbType.DateTimeOffset },
            { typeof(byte[]), DbType.Binary},
            { typeof(byte?), DbType.Byte},
            { typeof(sbyte?), DbType.SByte },
            { typeof(short?), DbType.Int16},
            { typeof(ushort?), DbType.UInt16},
            { typeof(int?), DbType.Int32},
            { typeof(uint?), DbType.UInt32},
            { typeof(long?), DbType.Int64},
            { typeof(ulong?), DbType.UInt64},
            { typeof(float?), DbType.Single},
            { typeof(double?), DbType.Double},
            { typeof(decimal?), DbType.Decimal},
            { typeof(bool?), DbType.Boolean},
            { typeof(char?), DbType.StringFixedLength},
            { typeof(Guid?), DbType.Guid},
            { typeof(DateTime?), DbType.DateTime },
            { typeof(DateTimeOffset?), DbType.DateTimeOffset},
            { typeof(Binary), DbType.Binary},
            { typeof(TimeSpan), DbType.Time },
            { typeof(TimeSpan?), DbType.Time },
        };

        public static DbType SqlType(this Type systype)
        {
            DbType resulttype;
            TypeMappings.TryGetValue(systype, out resulttype);
            return resulttype;
        }

        public static string CreateSqlTable(DataTable dt, string tablename)
        {
            return
$@"IF NOT EXISTS (
    SELECT 
        * 
    FROM 
        sys.objects 
    WHERE 
        object_id = OBJECT_ID(N'[dbo].[{tablename}]')
        AND type in (N'U')
)
BEGIN
    CREATE TABLE {tablename} (
    {new Func<IEnumerable<string>>(() =>
        {
            var columnstring = new List<string>();

            for (var i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn column = dt.Columns[i];

                var columnSize = column.DataType == typeof(string)
                    ? (column.MaxLength > 0 ? $"({column.MaxLength})" : "(MAX)")
                    : String.Empty;
                
                columnstring.Add(
                    $"{column.ColumnName} {column.DataType.SqlType()}{columnSize}{(i == dt.Columns.Count - 1 ? "," : "")}");
            }
            return columnstring;
        })().Join(Environment.NewLine)}
    )
END";
        }
    }
}
