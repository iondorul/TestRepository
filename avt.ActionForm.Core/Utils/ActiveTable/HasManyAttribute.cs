using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DnnSharp.Common.ActiveTable
{
    internal class HasManyAttribute : Attribute
    {
        public string ForeignKeyColumnName { get; set; }
        public string OrderBy { get; set; }

        public HasManyAttribute(string foreignKeyColumnName)
        {
            ForeignKeyColumnName = foreignKeyColumnName;
        }
    }
}
