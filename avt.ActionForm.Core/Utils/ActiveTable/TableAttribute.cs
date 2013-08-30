using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DnnSharp.Common.ActiveTable
{
    internal class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute(string name)
        {
            Name = DotNetNuke.Common.Utilities.Config.GetDataBaseOwner() + '[' +
                DotNetNuke.Common.Utilities.Config.GetObjectQualifer() + name.Trim('[', ']') + ']';
        }
    }
}
