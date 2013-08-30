using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;

namespace avt.ActionForm.Core.Actions
{
    public class RedirectToPageAction : IAction
    {
        public int PageId { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            PageId = settings.GetValue("PageId", -1);
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            return new RedirectToUrl() {
                Url = DotNetNuke.Common.Globals.NavigateURL(PageId)
            };
        }
    }
}
