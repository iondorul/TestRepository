<%@ Page Language="C#" AutoEventWireup="true" Inherits="avt.ActionForm.WndTextEditor" EnableViewState = "true" CodeFile="WndTextEditor.aspx.cs" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/texteditor.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title>Edit Text</title>
</head>

<body id="Body" runat="server" >
<form runat="server" id="Form" enctype="multipart/form-data">
    
    <asp:ScriptManager runat="server" ID="ScriptManager"></asp:ScriptManager>
    <input type="hidden" runat="server" ID="__dnnVariable" />
    <asp:HiddenField runat="server" id="hdnText" />
    <asp:HiddenField runat="server" id="hdnLoaded" />
    <div runat="server">
        <dnn:TextEditor ID="textEditor" runat="server" Width="100%" TextRenderMode="Raw" HtmlEncode="False"
            defaultmode="Rich" height="380" choosemode="True" chooserender="False" Visible='false' />
    </div>
    <asp:Button runat="server" ID="btnSubmit" style="display: none;" OnClick="SubmitContents" />
</form>
</body>
</html>
