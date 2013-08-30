using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace avt.ActionForm.Core.Validation
{
    public interface IGroupValidator
    {
        bool IsServerSideValidation { get; }
        ValidationResult Validate(IDictionary <string, string> input, GroupValidatorDef validator);
    }
}
