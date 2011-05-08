using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDRUpdater
{
    [AttributeUsage(AttributeTargets.Property,
                    Inherited = false, AllowMultiple = false)]
    public class SqlColumnAttribute : Attribute
    {
        public string Column;

        public SqlColumnAttribute(string column)
        {
            Column = column;
        }
    }
}
