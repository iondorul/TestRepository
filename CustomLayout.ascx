<%@ Control Language="C#" AutoEventWireup="True" Inherits="avt.ActionForm.CustomLayout" EnableViewState = "true" CodeFile="CustomLayout.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/texteditor.ascx" %>

<div class = "avtBox sunny">

    <p style="font-size: 1.2em; color: #222; margin: 10px 10px;">
        In the HTML layout below, form fields are represented with following tokens:
        <asp:Repeater runat="server" ID ="rpFields">
            <ItemTemplate>
                [Fields:<%# DataBinder.Eval(Container.DataItem, "TitleCompacted") %>], 
            </ItemTemplate>
        </asp:Repeater>
        [Buttons:Submit], [Buttons:Cancel]
    </p>
    <p style="font-size: 1.2em; color: #222; margin: 10px 10px 20px 10px;">
        Start building your layout from scratch or <asp:LinkButton runat="server" OnClick="OnGenerate2ColLayout">Start with a 2 column layout</asp:LinkButton>.
    </p>

    <dnn:TextEditor ID="textEditor" runat="server" Width="100%" TextRenderMode="Raw" HtmlEncode="False"
            defaultmode="Rich" height="380" choosemode="True" chooserender="True" />
    
    <br /><br />

    <div style = "text-align: center; margin: 30px 40px 10px 0;">
        <asp:LinkButton runat = "server" OnClick = "OnSave" style = "font-size: 14px; font-weight: bold; color: #4a8094; margin-right: 10px;" CausesValidation = "false">Save</asp:LinkButton>
        <a href = "<%= EditUrl("Manage") %>" id="A1" style = "font-size: 12px; font-weight: bold; color: #CC0000; font-style: normal;">Back to Manage</a>
    </div>

</div>


<script type = "text/javascript">

    afjQuery(document).ready(function ($) {
      
    });

</script>

