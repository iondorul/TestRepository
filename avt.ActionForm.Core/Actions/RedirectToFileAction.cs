using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using System.IO;

namespace avt.ActionForm.Core.Actions
{
    public class RedirectToFileAction : IAction
    {
        public string File { get; set; }
        public bool Force { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            File = settings.GetValue("File", "");
            Force = settings.GetValue("Force", true);
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            return new RedirectToUrl() {
                Url = Path.Combine(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().HomeDirectory, File)
            };

        }
    }
}
