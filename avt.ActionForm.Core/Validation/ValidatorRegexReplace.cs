using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace avt.ActionForm.Core.Validation
{
    public class ValidatorRegexReplace : IValidator
    {
        public bool IsServerSideValidation { get { return false; } }

        public bool IsValid(ref string input, ref string msgErr, Dictionary<string, string> validatorParams)
        {
            RegexOptions opts = RegexOptions.None;
            if (validatorParams.ContainsKey("IgnoreCase") && validatorParams["IgnoreCase"].ToLower() == "true") {
                opts |= RegexOptions.IgnoreCase;
            }

            input = Regex.Replace(input, validatorParams["Regex"], validatorParams["ReplaceWith"], opts);
            return true;
        }

        public string GenerateJsValidationCode(string jsResultVar, string jsValueVar, ValidatorDef validator, ModuleInfo module)
        {
            return string.Format("{0} = true;", jsResultVar);
        }
    }
}
