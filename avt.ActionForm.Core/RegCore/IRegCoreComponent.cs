﻿using System;
using System.Collections.Generic;
using System.Text;

namespace avt.ActionForm.RegCore
{
    public interface IRegCoreComponent
    {
        void InitRegCore(bool isAdmin, string regCoreServer, string productName, string productCode, string productKey, string version, string regCoreManageUrl, Type controller);
    }
}
