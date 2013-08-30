using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace avt.ActionForm.Core.Validation
{
    public class ValidatorRegex : IValidator
    {
        public bool IsServerSideValidation { get { return false; } }

        public bool IsValid(ref string input, ref string msgErr, Dictionary<string, string> validatorParams)
        {
            RegexOptions opts = RegexOptions.None;
            if (validatorParams.ContainsKey("IgnoreCase") && validatorParams["IgnoreCase"].ToLower() == "true") {
                opts |= RegexOptions.IgnoreCase;
            }

            return Regex.IsMatch(input, validatorParams["Regex"], opts);
        }

        public string GenerateJsValidationCode(string jsResultVar, string jsValueVar, ValidatorDef validator, ModuleInfo module)
        {
            string opts = "";
            if (validator.ValidatorParams.ContainsKey("IgnoreCase") && validator.ValidatorParams["IgnoreCase"].ToLower() == "true") {
                opts += "i";
            }

            return string.Format("{0} = afjQuery.trim({3}).length == 0 || /{1}/{2}.test({3});", jsResultVar, validator.ValidatorParams["Regex"], opts, jsValueVar);

//            return string.Format(@"
//                function {0}(source, arguments) {{
//                    arguments.IsValid = /{1}/{2}.test(arguments.Value);
//                }}
//            ", jsFunctionName, validatorParams["Regex"], opts);
        }
    }
}
