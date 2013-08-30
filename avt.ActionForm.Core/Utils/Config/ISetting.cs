using System;

namespace avt.ActionForm.Core.Utils.Config
{
    public interface ISetting
    {
        bool CanOverride { get; set; }
        bool Inherit { get; set; }
        DateTime LastModified { get; set; }
        int? LastModifiedBy { get; set; }
        string Name { get; set; }
        object ValueObj { get; set; }
    }
}
