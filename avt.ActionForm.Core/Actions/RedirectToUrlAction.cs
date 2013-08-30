using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;

namespace avt.ActionForm.Core.Actions
{
    public class RedirectToUrlAction : IAction
    {
        public string Url { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            Url = settings.GetValue("Url", "");
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            if (string.IsNullOrEmpty(Url))
                return null;

            return new RedirectToUrl() { 
                Url = data.ApplyAllTokens(Url) 
            };
        }
    }
}
