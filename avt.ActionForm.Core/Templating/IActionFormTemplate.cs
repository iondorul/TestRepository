using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Validation;
using DotNetNuke.Entities.Modules;
using System.Collections.Generic;
using System.Web.UI;

namespace avt.ActionForm.Templating
{
    public delegate void SubmitTemplateHandler(IActionFormTemplate template, FormData data);
    public delegate void ValidationFailedHandler();

    public interface IActionFormTemplate
    {
        void LoadForm(ActionFormSettings settings, FormData initData);
        string CancelUrl { set; }
        void Error(string message);

        void InitControls(ActionFormSettings settings);
        void Render(Control inControl);

        string InitScript(string baseRelPath);

        void RegisterClientValidator(ValidatorDef validDef, Page page, ModuleInfo module);
    }
}
