<%@ Control Language="C#" AutoEventWireup="True" Inherits="avt.ActionForm.Manage" EnableViewState="true" CodeFile="Manage.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<%--New admin UI starts here--%>
<div class="avtBox">
    <iframe id="frmAdmin" runat="server" style="width: 100%; height: 700px; border: 1px solid #ccc;"></iframe>
</div>

<p style="font-size: 0.85em; color: #888; margin: 2px 10px;">
    Version <%= avt.ActionForm.ActionFormController.Build.Substring(0, avt.ActionForm.ActionFormController.Build.LastIndexOf('.')) %>
</p>

<!--Start OpenTraits widget-->
<script>window.jQuery || document.write('<script src="' + ('https:' == document.location.protocol ? 'https:' : 'http:') + +'//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"><\/script>')</script>
<script>

    var _optw = _optw || {
        project: '//support.dnnsharp.com/action-form',
        bgColor: '#FF8800',
        color: '#ffffff',
        position: ['bottom', 'left']
    };

    (function () {
        var opt = document.createElement('script'); opt.type = 'text/javascript'; opt.async = true;
        opt.src = ('https:' == document.location.protocol ? 'https:' : 'http:') + '//cdn.opentraits.com/js/optwidget.min.js.gzip?v=1.3.0.042';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(opt, s);
    })();


    afjQuery(window).bind("message", function (event) {
        afjQuery('#<%= frmAdmin.ClientID %>').attr("scrolling", "no");
        var msg;
        try { msg = JSON.parse(event.originalEvent.data); } catch (e) { return; }
        if (msg && typeof msg == "object") {
            if (msg.type == "af-height") {
                afjQuery('#<%= frmAdmin.ClientID %>').stop(true, false).animate({ height: msg.height }, 100);
            } else if (msg.type == "af-scroll") {
                $('html, body').animate({
                    scrollTop: $('#<%= frmAdmin.ClientID %>').offset().top + msg.offset
                }, 500);
            }
        }
    });

</script>
<!--End OpenTraits widget-->
