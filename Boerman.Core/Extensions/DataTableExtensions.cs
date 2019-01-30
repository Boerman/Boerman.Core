﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Boerman.Core.Extensions
{
    public static partial class Extensions
    {
        public static IEnumerable<dynamic> AsDynamicEnumerable(this DataTable table)
        {
            throw new NotImplementedException();
            //return table.AsEnumerable().Select(row => new DynamicRow(row));
        }

        private sealed class DynamicRow : DynamicObject
        {
            private readonly DataRow _row;

            internal DynamicRow(DataRow row) { _row = row; }

            // Interprets a member-access as an indexer-access on the 
            // contained DataRow.
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var retVal = _row.Table.Columns.Contains(binder.Name);
                result = retVal ? _row[binder.Name] : null;
                return retVal;
            }
        }
    }
}
