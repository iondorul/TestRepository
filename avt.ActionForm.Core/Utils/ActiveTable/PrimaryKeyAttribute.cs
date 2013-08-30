using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DnnSharp.Common.ActiveTable
{
    internal class PrimaryKeyAttribute : Attribute
    {
        public bool IsIdentity { get; set; }

        public PrimaryKeyAttribute(bool isIdentity = true)
        {
            IsIdentity = isIdentity;
        }
    }
}
