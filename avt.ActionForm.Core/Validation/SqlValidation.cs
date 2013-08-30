using avt.ActionForm.Core.Utils.Data;
using avt.ActionForm.Data;
using DotNetNuke.Entities.Modules;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace avt.ActionForm.Core.Validation
{
    public class SqlValidation : IValidator
    {
        public bool IsServerSideValidation { get { return true; } }

        public bool IsValid(ref string input, ref string msgErr, Dictionary<string, string> validatorParams)
        {
            if (!validatorParams.ContainsKey("Sql"))
                throw new ArgumentException("Missing SQL for SqlValidation");

            var sql = validatorParams["Sql"];
            sql = Regex.Replace(sql, "\\{databaseOwner\\}", DotNetNuke.Common.Utilities.Config.GetDataBaseOwner());
            sql = Regex.Replace(sql, "\\{objectQualifier\\}", DotNetNuke.Common.Utilities.Config.GetObjectQualifer());
            sql = Regex.Replace(sql, "\\{value\\}", SqlTable.EncodeSql(input, false));

            using (var dr = SqlHelper.ExecuteReader(DotNetNuke.Common.Utilities.Config.GetConnectionString(), System.Data.CommandType.Text, sql)) {
                if (dr != null && dr.HasRows) {
                    if (dr.Read() && dr.FieldCount > 0) {
                        msgErr = dr[0].ToString();
                        return false;
                    }
                }
            }

            return true;
        }

        public string GenerateJsValidationCode(string jsResultVar, string jsValueVar, ValidatorDef validator, ModuleInfo module)
        {
            // TODO: try to reduce calls by using a timeout

            return string.Format(@"
                var $ = afjQuery;

                {0} = true;
                $.ajax({{
                    type: 'POST',
                    url: '{1}',
                    data: {{
                        validator: '{2}',
                        value: {3},
                        fieldId: $(element).attr('data-fieldid')
                    }},
                    success: function(data) {{
                        if (data.success) {{
                            $(element).nextAll('.alert').stop(true, true).slideUp('fast', function() {{ $(this).remove() }});
                            return;
                        }}

                        parseFormResponse(data, {{
                            error: function (err) {{
                                {0} = false;

                                $(element).rules('add', {{
                                    //'{4}': true,
                                    messages: {{
                                        '{4}': data.error
                                    }}
                                }});
                            }},
                            message: function (msg) {{
                                {0} = false;
                                if ($(element).nextAll('.alert').size()) {{
                                    $(element).nextAll('.alert').html(msg);
                                }} else {{
                                    $(element).after('<div class=""alert alert-block"" style=""display: none;"">' + msg + '</div>');
                                    $(element).nextAll('.alert').stop(true, true).slideDown('fast');
                                }}
                            }}
                        }});

                    }},
                    dataType: 'json',
                    async:false
                }});",
                jsResultVar,
                HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/') + "/DesktopModules/AvatarSoft/ActionForm/Validation.ashx?mid=" + module.ModuleID,
                ActionFormController.JsonEncode(validator.Title), jsValueVar, validator.JsValidatorName + module.ModuleID);

//            return string.Format(@"
//                function {0}(source, arguments) {{
//                    arguments.IsValid = /{1}/{2}.test(arguments.Value);
//                }}
//            ", jsFunctionName, validatorParams["Regex"], opts);
        }

    }
}
