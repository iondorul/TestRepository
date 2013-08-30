using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace avt.ActionForm.Core.Validation
{
    public class AtLeastOneInGroup : IGroupValidator
    {
        public bool IsServerSideValidation { get { return false; } }

        public ValidationResult Validate(IDictionary<string, string> input, GroupValidatorDef validator)
        {
            foreach (var value in input.Values) {
                if (!string.IsNullOrEmpty(value))
                    return new ValidationResult() { IsValid = true };
            }

            return new ValidationResult() {
                IsValid = false,
                ErrMessage = validator.ErrorMessageDefault
            };
        }
    }
}
