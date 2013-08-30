using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace avt.ActionForm.Core.Form.Result
{
    public interface IFormEventResult
    {
        //// TODO: do we really need this UserActionRequired
        ///// <summary>
        ///// Marks that a user action is required before submission is completed.
        ///// This can be for example a confirmation or a payment.
        ///// </summary>
        //bool UserActionRequired { get; }

        /// <summary>
        /// A form result can be intermediate or final.
        /// It's intermediat if for example requires user confirmation or payment.
        /// </summary>
        eSubmitStatus Status { get; }

        /// <summary>
        /// Executes a result in current HTTP context. This is action form 1.x way of submitting data.
        /// In Action Form 2.x data submits through SubmitData service which returns a json that is evaluated on client side.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="baseControl"></param>
        void Execute(HttpContext context, Control baseControl);

        /// <summary>
        /// Returns a json response that is evaluated on the client side.
        /// This can be a message, a redirect, a repost
        /// </summary>
        /// <returns></returns>
        string ToJson();
    }
}
