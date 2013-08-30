using System;

namespace avt.ActionForm
{
    public partial class WndTextEditor : System.Web.UI.Page
    {
        protected void Page_Load(Object Sender, EventArgs args)
        {
        }


        protected void SubmitContents(Object Sender, EventArgs args)
        {
            DotNetNuke.UI.UserControls.TextEditor te = textEditor as DotNetNuke.UI.UserControls.TextEditor;

            if (hdnLoaded.Value != "true") {
                // initialize
                te.Text = hdnText.Value;
                te.Visible = true;
                hdnLoaded.Value = "true";
            } else {
                // value submitted, load it back to the hidden field
                hdnText.Value = te.Text;
                te.Visible = false;
            }
        }
    }
}