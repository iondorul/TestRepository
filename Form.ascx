<%@ Control Language="C#" AutoEventWireup="True" Inherits="avt.ActionForm.FormPage" 
EnableViewState = "true" CodeFile="Form.ascx.cs" %>

<asp:Label runat ="server" ID = "lblInitJs"></asp:Label>
<div runat ="server" ID = "phTemplate"></div>

<div runat="server" id = "pnlMessage" visible="false" style="text-align: center; margin: 30px;">
    <div class="ui-state-highlight" style="padding: 20px; max-width: 400px; margin: auto;">
        <asp:Literal runat="server" ID = "lblMessage"></asp:Literal>
    </div>
</div>

<script type="text/javascript">

    afjQuery(document).ready(function () {
        afjQuery(".af-init-onchange").each(function () {
            if (afjQuery(this).val())
                afjQuery(this).change();
        });

        <%= FormTemplate.InitScript(TemplateSourceDirectory) %>
    });

</script>
