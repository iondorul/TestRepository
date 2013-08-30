<%@ Control Language="C#" AutoEventWireup="true" Inherits="avt.ActionForm.Main" EnableViewState = "true" 
Codefile="Main.ascx.cs" %>

<asp:Label runat ="server" ID = "lblInitJs"></asp:Label>
<asp:Label runat ="server" ID = "lblContent"></asp:Label>
<div runat ="server" id = "cFormTemplate">
    <div runat ="server" ID = "phFormTemplate" style="margin: 12px;"></div>

    <div runat="server" id = "pnlMessage" visible="false" style="text-align: center; margin: 30px;">
        <div class="ui-state-highlight" style="padding: 20px; max-width: 400px; margin: auto;">
            <asp:Literal runat="server" ID = "lblMessage"></asp:Literal>
        </div>
    </div>

</div>


<div id = "pnlDialogContainer" runat="server" style="position: absolute; left:0px; top: 0px;"></div>

<div id = "pnlScriptAlways" runat = "server" visible = "false">
<script type ="text/javascript">
    afjQuery(function() {
        <%= FormTemplate.InitScript(TemplateSourceDirectory) %>
    });
</script>
</div>

<div id = "pnlScriptInline" runat = "server" visible = "false">
<script type ="text/javascript">
    function showFormInline<%=ModuleId %>() {
        afjQuery("#<%= lblContent.ClientID %>").slideUp('fast');
        afjQuery("#<%= phFormTemplate.ClientID %>").slideDown('fast');
        <%= FormTemplate.InitScript(TemplateSourceDirectory) %>
    }
    function hideFormInline<%=ModuleId %>() {
        afjQuery("#<%= lblContent.ClientID %>").slideDown();
        afjQuery("#<%= phFormTemplate.ClientID %>").slideUp();
    }
    
</script>
</div>

<div id = "pnlScriptPopup" runat ="server" visible="false">
<script type ="text/javascript">
    
    function showFormPopup<%=ModuleId %>() {
        afjQuery("#<%= phFormTemplate.ClientID %>").dialog("open");
        afjQuery(".ui-widget-overlay").appendTo(afjQuery("form"));
    }

    function hideFormPopup<%=ModuleId %>() {
        afjQuery("#<%= phFormTemplate.ClientID %>").dialog("close");
    }

    afjQuery(document).ready(function() {

        afjQuery("#<%= phFormTemplate.ClientID %>").dialog({ 
            modal: true, 
            overlay: { 
                opacity: 0.5, 
                background: "black"
            },
            show: "fade",
            hide: "fade",
            resizable: false,
            width: <%= AfSettings.PopupWidth.GetValueOrDefault(600) %>,
            <%= AfSettings.PopupHeight.Value > 0 ? "height:" + AfSettings.PopupHeight.Value + ",": "" %>
            title: "<%= ModuleConfiguration.ModuleTitle %>",
            closeOnEscape: true,
            autoOpen:false,

            open: function( event, ui ) {
                <%= FormTemplate.InitScript(TemplateSourceDirectory) %>
            }
        }).parents('.ui-dialog:first').addClass('dnnFormPopup');

        afjQuery("#<%= phFormTemplate.ClientID %>").parent().appendTo(afjQuery("form"))
            .wrap("<div class='"+ afjQuery("#<%= pnlDialogContainer.ClientID %>").attr("class") +"'></div>");

        if (typeof afReopenDlg<%=ModuleId %> != "undefined") {
            showFormPopup<%=ModuleId %>();
        }

//        afjQuery("form").submit(function() {
//            afjQuery("#<%= phFormTemplate.ClientID %>").dialog("close");
//        });

    });//end JQUery Module Title


</script>
</div>


<script type="text/javascript">

    afjQuery(function () {
        afjQuery(".af-init-onchange").each(function () {
            if (afjQuery(this).val())
                afjQuery(this).change();
        });

    });

</script>

