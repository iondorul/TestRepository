using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Core.Utils.Config
{
    /// <summary>
    /// This attributes marks field that apply to all settings in current object
    /// </summary>
    public class SectionAttribute : Attribute
    {
        public SectionAttribute()
        {
        }

    }
}
