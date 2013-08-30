using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Core.Utils.Config
{
    public class PropertyAttribute : Attribute
    {
        public object Default { get; set; }

        public PropertyAttribute()
        {
        }

    }
}
