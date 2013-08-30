using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace avt.ActionForm.Core.Validation
{
    public interface IValidator
    {
        bool IsServerSideValidation { get; }

        bool IsValid(ref string input, ref string msgErr, Dictionary<string, string> validatorParams);
        string GenerateJsValidationCode(string jsResultVar, string jsValueVar, ValidatorDef validatorParams, ModuleInfo module);
    }
}
