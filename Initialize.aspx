<%@ Page Language="C#" AutoEventWireup="true" Inherits="avt.ActionForm.Initialize" CodeFile="Initialize.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" data-ng-app="ActionForm">
<head>
    <title>Initialize Action Form</title>

    <link type="text/css" rel="stylesheet" href="<%= TemplateSourceDirectory%>/static/bootstrap/css/bootstrap.min.css?v=<%= avt.ActionForm.ActionFormController.Build %>" />
    <link type="text/css" rel="stylesheet" href="<%= TemplateSourceDirectory%>/static/bootstrap-select.min.css?v=<%= avt.ActionForm.ActionFormController.Build %>" />
    <%--<link type="text/css" rel="stylesheet" href="<%= TemplateSourceDirectory%>/static/bootstrap/css/bootstrap-responsive.min.css?v=<%= avt.ActionForm.ActionFormController.Build %>" />--%>
    <link type="text/css" rel="stylesheet" href="<%= TemplateSourceDirectory%>/static/admin.css?v=<%= avt.ActionForm.ActionFormController.Build %>" />

    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/jquery.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>

    <script>
        var g_adminApi = '<%= TemplateSourceDirectory %>/AdminApi.ashx';
        var g_isHost = <%= DnnUser.IsSuperUser.ToString().ToLower() %>;
        var g_portalAlias = '<%= HttpUtility.UrlEncode(StrAlias) %>';
        var g_moduleId = <%= Module.ModuleID %>;
        var g_lang = 'default';

    </script>
</head>
<body data-spy="scroll" data-target=".navbar" data-offset="0" class="ng-cloak">
    <form id="form1" runat="server">

        <div>

            <div class="navbar navbar-inverse navbar-fixed-top" id="navbar">
                <div class="navbar-inner">
                    <div class="container-fluid">

                        <ul class="nav pull-right">
                            <li><a href="<%= ViewUrl %>" target="_top" style="font-weight: bold; color: #fff;" title="Return to the form. Make sure to save your settings first.">Back</a></li>
                        </ul>

                        <ul class="nav">
                            <%--<li><a href="#LiveUpdates">Live Updates</a></li>--%>
                        </ul>

                    </div>
                </div>
            </div>

            <p class="alert alert-block" style="margin-top: 40px; font-weight: bold;">
                This Action Form is not initilized.
                Select a template below or start from scratch.
            </p>
            <h1></h1>

            <%--<asp:LinkButton runat="server" CommandName="fromTemplate" CssClass="btn btn-info">Blank Form</asp:LinkButton>--%>
            <div class="container-fluid">
                <asp:Repeater runat="server" ID="rpTemplates">
                    <ItemTemplate>
                        <%# Container.ItemIndex % 3 == 0 ? ((Container.ItemIndex > 0 ? "</div>" : "") +   "<div class=\"row-fluid\">") : "" %>
                        <div class="span4">
                            <div class="well <%#  Container.ItemIndex == 0 ? "alert-info" : ""%>">
                                <div  style="height: 120px;">
                                     <h3 style="margin-bottom: 4px; line-height: 1em;"><%# DataBinder.Eval(Container.DataItem, "Name") %></h3>
                                    <p class="muted"><%# DataBinder.Eval(Container.DataItem, "Description") %></p>
                                </div>
                                <asp:LinkButton runat="server" CommandName="fromTemplate" CommandArgument =<%# DataBinder.Eval(Container.DataItem, "Name") %> CssClass="btn btn-info pull-right"><i class="icon-check icon-white"></i> Start</asp:LinkButton>
                                <div class="clearfix"></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </form>


    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/bootstrap/js/bootstrap.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>

</body>
</html>
