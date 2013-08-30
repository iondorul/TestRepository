using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Core.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrMessage { get; set; }
        public string FieldName { get; set; }
    }
}
