using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace avt.ActionForm.Core.Config
{
    public interface IConfigItem
    {
        void LoadFromXml(XmlNode xmlConfig);
        string GetKey();
    }
}
