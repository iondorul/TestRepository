using avt.ActionForm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Utils.Config
{
    public class Setting<T> : ISetting
    {
        public Setting()
        {
            LastModified = DateTime.Now;
        }

        public string Name { get; set; }
        
        [ScriptIgnore]
        public object ValueObj { get; set; }
        public T Value { 
            get { return ConvertUtils.Cast<T>(ValueObj, default(T)); }
            set { ValueObj = value; }
        }

        public bool CanOverride { get; set; }
        public bool Inherit { get; set; }

        [ScriptIgnore]
        public DateTime LastModified { get; set; }
        [ScriptIgnore]
        public int? LastModifiedBy { get; set; }

        public T GetValueOrDefault(T defaultValue)
        {
            if (string.IsNullOrEmpty(ToString()))
                return defaultValue;
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
