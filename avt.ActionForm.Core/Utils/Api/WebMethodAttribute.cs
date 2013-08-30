using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Utils.Api
{
    public enum eResponseType { 
        Text,
        Html,
        Json,
        Xml
    }

    /// <summary>
    /// TODO: verbs, serializers
    /// </summary>
    public class WebMethodAttribute : Attribute
    {
        public eResponseType DefaultResponseType { get; set; }
        public bool RequiredEditPermissions { get; set; }

        public WebMethodAttribute()
        {
        }

    }
}
