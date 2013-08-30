<%@ Page Language="C#" AutoEventWireup="true" Inherits="avt.ActionForm.ManageNew" CodeFile="Manage.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" data-ng-app="ActionForm">
<head>
    <title>Manage Action Form</title>

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
        //var g_moduleTitle  = <%= Module.ModuleTitle %>;
        var g_lang = 'default';
        var g_resourceVersion = '<%= avt.ActionForm.ActionFormController.Build %>';

        // check refresh
        if (window.location.hash == "#refresh")
            top.location='<%= DotNetNuke.Common.Globals.NavigateURL(Module.TabID) %>';

        function g_refresh() {
            $('#refreshing').fadeIn();
            $.getJSON(g_adminApi + '?method=refresh&alias=' + g_portalAlias + '&mid=' + g_moduleId, function(data) {
                top.location='<%= DotNetNuke.Common.Globals.NavigateURL(Module.TabID) %>';
            });
            }

        function g_localize(o) {
            return o ? o[g_lang] : "";
        }

        function g_localizeMaybeJson(o) {
            var val = g_localize(o);
            // TODO: find a better way to determine json arrays and objects - for example with a regex
            if (val && (val[0] == '[' || val[0] == '{'))
                val = $.parseJSON(val);
            return val;
        }


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
                            <li><a href="<%= Request.RawUrl %>" target="_parent" title="Open the admin in a separate window." class="visible-in-iframe"><i class="icon-resize-full icon-white"></i></a></li>
                        </ul>

                        <ul class="nav">
                            <li><a href="#General">Forma</a></li>
                            <li><a href="#FormFields">Fields</a></li>
                            <li><a href="#FormEvents">Events</a></li>
                            <li><a href="#License">License</a></li>
                            <li><a href="#LiveUpdates">Live Updates</a></li>
                        </ul>

                    </div>
                </div>
            </div>

            <%--<p class="muted" style="margin: 22px 0 0 36px;">
                <i class="icon-chevron-up"></i>
                Switch to Host portal to change default settings that affect all portals.
            </p>--%>

            <div id="refreshing" style="display: none; float: right; margin: 20px 4px 0px 0;">
                <img src="<%= TemplateSourceDirectory %>/static/images/loading.gif" style="height: 16px;" />
            </div>


            <p runat="server" id="pnlLicense" style="margin-top: 30px;"></p>

            <section id="General" data-ng-controller="FormCtrl">

                <div data-ng-show="sharedData.settings && !sharedData.settings.IsInitialized.Value" class="alert alert-block">
                    The form has been initialized with default settings.
                    Make sure to modify and save both the settings and the fields.
                </div>

                <h3 style="margin-bottom: 20px;">Form Settings</h3>
                <div class="accordion form-horizontal">

                    <div class="control-group">
                        <label class="control-label">Template</label>
                        <div class="controls">
                            <select data-ng-model="sharedData.settings.FormTemplate.Value" data-ng-options="o as o for (k,o) in templates">
                            </select>
                            <p class="muted">The template used to render the form. Custom templates can be build using XSL.</p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">jQuery Theme</label>
                        <div class="controls">
                            <select data-ng-model="sharedData.settings.jQueryTheme.Value" data-ng-options="o as o for (k,o) in jQueryThemes">
                            </select>
                            <p class="muted">The jQuery UI theme is used for rendering controls such as dialogs or date pickers..</p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Label Align</label>
                        <div class="controls">
                            <select data-ng-model="sharedData.settings.LabelAlign.Value">
                                <option value="0">Template Default</option>
                                <option value="1">Left</option>
                                <option value="2">Center</option>
                                <option value="3">Right</option>
                                <option value="4">Top</option>
                                <option value="5">Inside</option>
                            </select>
                            <p class="muted">Determines the position and alignment of the label relative to the input controls.</p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Label Width</label>
                        <div class="controls">
                            <input type="text" data-ng-model="sharedData.settings.LabelWidth.Value" class="input-xsmall" />
                            <p class="muted">This is the total space reserved for labels to which the alignment above applies.</p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Field Spacing</label>
                        <div class="controls">
                            <select data-ng-model="sharedData.settings.FieldSpacing.Value">
                                <option value="0">Loose</option>
                                <option value="1">Normal</option>
                                <option value="2">Compact</option>
                            </select>
                            <p class="muted">Determines the spacing between controls.</p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Manual Layout</label>
                        <div class="controls">
                            <input type="checkbox" data-ng-model="sharedData.settings.HasCustomLayout.Value" />
                            <a class="btn btn-link" href="<%= EditHtmlLayoutUrl %>" target="_top" data-ng-show="sharedData.settings.HasCustomLayout.Value">(Edit HTML Template)</a>
                            <p class="muted">If the drag&drop layout builder below is not enough, you can use an HTML template to achieve more complex scenarios.</p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Show Form</label>
                        <div class="controls">
                            <select data-ng-model="sharedData.settings.OpenFormMode.Value">
                                <option value="3">Initially Visible</option>
                                <option value="2">In separate page</option>
                                <option value="0">In Popup</option>
                                <option value="1">In Text</option>
                            </select>
                            <a href="" data-ng-show="sharedData.settings.OpenFormMode.Value != 3" data-ng-click="_editTemplate = true">Edit Template</a>
                            <p class="muted">Choose either this form appears initially on the page or if it should show when the user clicks a link.</p>
                        </div>
                    </div>

                    <div data-ng-show="sharedData.settings.OpenFormMode.Value == 0" class="row-fluid">
                        <div class="control-group span6">
                            <label class="control-label">Popup Width</label>
                            <div class="controls">
                                <input type="text" data-ng-model="sharedData.settings.PopupWidth.Value" class="input-xsmall" />
                            </div>
                        </div>
                        <div class="control-group span6">
                            <label class="control-label">Popup Height</label>
                            <div class="controls">
                                <input type="text" data-ng-model="sharedData.settings.PopupHeight.Value" class="input-xsmall" />
                            </div>
                        </div>
                    </div>

                    <div class="control-group offset1" data-ng-show="_editTemplate == true">
                        <textarea class="span9 richtext" data-ng-model="sharedData.settings.FrontEndTemplate.Value" data-ui-tinymce="tinymceOptions" style="height: 260px; width: 100%;"></textarea>
                        <p class="muted">
                            Make sure to create a link that points to [FormUrl] (if you do the html manually, set href="[FormUrl]"). 
                            Action Form will replace this token with the actual URL that triggers the form to appear.
                            Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a>
                        </p>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Initialization scripts</label>
                        <div class="controls">
                            <a href="" data-ng-click="_editInitScripts = true">Edit Scripts</a>
                            <textarea class="span9" data-ng-show="_editInitScripts" data-ng-model="sharedData.settings.InitJs.Value" data-ui-tinymce="tinymceOptions" style="height: 120px; width: 100%;"></textarea>
                            <p class="muted">
                                Include more scripts on the page along with this form. Enclose between &lt;script&gt; tags as applicable. Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a>
                            </p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Enable Client Side Validation</label>
                        <div class="controls">
                            <input type="checkbox" data-ng-model="sharedData.settings.ClientSideValidation.Value" />
                            <p class="muted">Disable client side validation if you want to catch validation errors on the server and execute actions.</p>
                        </div>
                    </div>

                    <div class="control-group">
                        <label class="control-label">Debug Mode</label>
                        <div class="controls">
                            <input type="checkbox" data-ng-model="sharedData.settings.IsDebug.Value" />
                            <p class="muted">In debug mode Action Form will show the XML used to compile the XSL template and also log debug information.</p>
                        </div>
                    </div>

                </div>

                <div class="alert accordion-group" style="padding: 4px;">

                    <%--<div class="pull-right">
                        <button class="btn btn-link" data-ng-disabled="!modified">Cancel</button>
                    </div>--%>
                    <div class="pull-right">
                        <button class="btn" data-ng-click="save()" type="button" data-ng-disabled="!modified && sharedData.settings.IsInitialized.Value" data-ng-class="{'btn-link': !modified && sharedData.settings.IsInitialized.Value, 'btn-warning': modified || !sharedData.settings.IsInitialized.Value}">Save</button>
                    </div>

                    <div class="clearfix"></div>

                </div>

            </section>

            <section id="FormFields" data-ng-controller="FieldsCtrl">
                <h3 style="margin-bottom: 20px;" class=" pull-left">Form Fields</h3>

                <div class="btn-group pull-right" data-toggle="buttons-radio" style="margin: 14px 0 0 12px;">
                    <button type="button" class="btn btn-default btn-mini active" data-ng-click="mode='edit'" data-ng-class="{ 'btn-info': mode=='edit' }">Edit Mode</button>
                    <button type="button" class="btn btn-default btn-mini" data-ng-click="buildGrid(); mode='layout';" data-ng-class="{ 'btn-info': mode=='layout' }">Layout Mode</button>
                </div>
                <div class="clearfix"></div>

                <div data-ng-show="mode=='edit'">
                    <div class="field-list sortable-list accordion form-horizontal">
                        <%-- data-ui-sortable="{placeholder: 'alert alert-block alert-field', handle: '.handle', stop: editOrderChanged}" data-ng-model="sharedData.fields">--%>
                        <div class="accordion-group field-root" data-ng-repeat="item in sharedData.fields" data-ng-class="{deleted: item.IsDeleted}">

                            <div class="accordion-heading btn-link-animate-trigger">

                                <button class="btn btn-link btn-small btn-link-animate show-70 pull-right" type="button" data-link-animate="danger" style="margin: 6px 6px 0 0;" data-ng-click="item.IsDeleted = !item.IsDeleted;" title="Note that the field is not actually deleted until you click the save button.">
                                    <i class="icon-trash"></i>
                                </button>

                                <%--<i class="show-50 handle pull-left icon-align-justify" title="Drag to define view order..."></i>--%>

                                <a class="accordion-toggle" data-toggle="collapse" href="#collapse{{item._uid}}">{{item.Title}}
                                    <small style="color: #777;">({{fieldDefs[item.InputTypeStr].Title}})</small>
                                </a>

                            </div>

                            <div id="collapse{{item._uid}}" class="accordion-body collapse" data-ng-class="{in: item._isOpen}">
                                <div class="accordion-inner container-fluid">

                                    <p class="muted">{{fieldDefs[item.InputTypeStr].HelpText[lang]}}</p>

                                    <div class="row-fluid">

                                        <div class="control-group span7">
                                            <label class="control-label">Type</label>
                                            <div class="controls">
                                                <div class="btn-group">

                                                    <button class="btn btn-link dropdown-toggle " data-toggle="dropdown">
                                                        {{fieldDefs[item.InputTypeStr].Title}} <span class="caret"></span>
                                                    </button>
                                                    <ul class="dropdown-menu">
                                                        <li data-ng-repeat="(key, def) in fieldDefGroups" class="dropdown-submenu">
                                                            <a href="#" onclick="return false;">{{key}}</a>
                                                            <ul class="dropdown-menu">
                                                                <li data-ng-repeat="fieldDef in fieldDefGroups[key]"><a data-ng-click="item.InputTypeStr = fieldDef.Name;">{{fieldDef.Title}}</a></li>
                                                            </ul>
                                                        </li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row-fluid">
                                        <div class="control-group span7">
                                            <label class="control-label">Title</label>
                                            <div class="controls">
                                                <input type="text" class="span12" data-ng-model="item.Title" data-ng-change="computeName(item)" />
                                                <p class="muted">The title is displayed on front-end</p>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row-fluid form-inline">
                                        <div class="control-group span12">
                                            <label class="control-label">ID</label>
                                            <div class="controls">
                                                <input type="text" class="span5" data-ng-model="item.BoundName" data-ng-disabled="item.AutoName" data-ng-change="computeName(item)" />
                                                <label class="checkbox" style="margin-left: 6px;">
                                                    <input type="checkbox" data-ng-model="item.AutoName" data-ng-change="computeName(item)" />
                                                    Auto
                                                </label>
                                                <p class="muted">The ID is used to reference fields using [ID] syntax. Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="row-fluid">
                                        <div class="control-group span12">
                                            <label class="control-label">Show Condition</label>
                                            <div class="controls">
                                                <input type="text" class="span7" data-ng-model="item.ShowCondition" placeholder="" />
                                                <p class="muted help">
                                                    This is a <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a>
                                                    expression used to determine either this field will show in the form. The expression must return something that can be represented as a boolean (true, false, 0, 1).
                                                    A common example is [HasRole:Administrators|true]
                                                </p>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row-fluid">
                                        <div class="control-group">
                                            <label class="control-label">Short Description</label>
                                            <div class="controls">
                                                <input type="text" class="span7" data-ng-model="item.ShortDesc" />
                                                <p class="muted">This is a tooltip or placeholder that helps users fill the form. Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                            </div>
                                        </div>
                                    </div>


                                    <%--<div class="row-fluid" data-ng-show="item.InputTypeStr == 'open-text-large' || item.InputTypeStr == 'open-text-rich' || item.InputTypeStr == 'static-text'">
                                        <div class="control-group">
                                            <label class="control-label">Default Value</label>
                                            <div class="controls">
                                                <textarea class="span8" rows="4" data-ng-model="item.DefaultValue"></textarea>
                                                <p class="muted">Initial value for this field. Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row-fluid" data-ng-show="!(item.InputTypeStr == 'open-text-large' || item.InputTypeStr == 'open-text-rich' || item.InputTypeStr == 'static-text')">
                                        <div class="control-group">
                                            <label class="control-label">Default Value</label>
                                            <div class="controls">
                                                <input type="text" class="span8" data-ng-model="item.DefaultValue" />
                                                <p class="muted">Initial value for this field. Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                            </div>
                                        </div>
                                    </div>--%>

                                    <%--<div class="row-fluid" data-ng-show="fieldDefs[item.InputTypeStr].HandlerFlags.indexOf('inputlist') != -1">
                                        <div class="control-group">
                                            <label class="control-label">Options</label>
                                            <div class="controls">
                                                <textarea class="span8" rows="4" data-ng-model="item.InputData"></textarea>
                                                <p class="muted">
                                                    Specify list of possible values (one per line) or an SQL select query. 
                                                    If both label and value are needed, then write them on the same line and separate them with pipe, for example "My Item|100".
                                                    Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a>
                                                </p>
                                            </div>
                                        </div>
                                    </div>--%>

                                    <div class="row-fluid">
                                        <div class="control-group">
                                            <label class="control-label">Other Options</label>
                                            <div class="controls">
                                                <button data-ng-model="item.IsEnabled" data-bs-checkbox="Enabled"></button>
                                                <button data-ng-model="item.DisableAutocomplete" data-bs-checkbox="Disable Auto Complete"></button>
                                            </div>
                                        </div>
                                    </div>

<!--                                     <div class="row-fluid">
                                        <div class="control-group">
                                            <label class="control-label">Show In</label>
                                            <div class="controls">
                                                <button data-ng-model="item.Test1" data-bs-checkbox="Form" title="Use the Layout view to position this button in the form."></button>
                                                <button data-ng-model="item.Test2" data-bs-checkbox="Buttons Pane" title="Shows this button at the bottom of the form"></button>
                                                <p class="muted">This button may also appear in the Display Message action.</p>
                                            </div>
                                        </div>
                                    </div> -->


                                    <!--Render parameters-->
                                    <div data-ng-repeat="p in fieldDefs[item.InputTypeStr].Parameters" data-ng-include="'static/templates/parameter.html?v=' + sharedData.resourceVersion">
                                    </div>

                                    <fieldset>
                                        <legend>UI Settings</legend>
                                        <div class="row-fluid" style="">
                                            <div class="control-group pull-left span5">
                                                <label class="control-label">Label CSS Classes</label>
                                                <div class="controls">
                                                    <input type="text" class="span12" data-ng-model="item.LabelCssClass" />
                                                    <p class="muted">Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                                </div>
                                            </div>

                                            <div class="control-group pull-left condensed span6">
                                                <label class="control-label">Styles</label>
                                                <div class="controls ">
                                                    <input type="text" class="span12" data-ng-model="item.LabelCssStyles" />
                                                    <p class="muted">Additional CSS styles. Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row-fluid">
                                            <div class="control-group pull-left span5">
                                                <label class="control-label">Control CSS Classes</label>
                                                <div class="controls">
                                                    <input type="text" class="span12" data-ng-model="item.CssClass" />
                                                    <p class="muted">Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                                </div>
                                            </div>

                                            <div class="control-group pull-left condensed span6">
                                                <label class="control-label">Styles</label>
                                                <div class="controls ">
                                                    <input type="text" class="span12" data-ng-model="item.CssStyles" />
                                                    <p class="muted">Additional CSS styles. Supports <a href="http://www.dnnsharp.com/dotnetnuke/modules/token-replacement/my-tokens" target="_blank">My Tokens</a></p>
                                                </div>
                                            </div>
                                        </div>
                                    </fieldset>

                                    <fieldset>
                                        <legend>Validation</legend>

                                        <div class="row-fluid">
                                            <div class="control-group span12">
                                                <div class="controls">
                                                    <label class="checkbox">
                                                        <input type="checkbox" data-ng-model="item.IsRequired" />
                                                        Required
                                                    </label>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row-fluid">
                                            <div class="control-group pull-left span6">
                                                <label class="control-label">Custom Validator #1</label>
                                                <div class="controls">
                                                    <select data-ng-options="k as k for (k, o) in validatorDefs" class="span12" data-ng-model="item.CustomValidator1">
                                                        <option value="">-- No Validator --</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="control-group pull-left condensed span5">
                                                <label class="control-label">#2</label>
                                                <div class="controls ">
                                                    <select data-ng-options="k as k for (k, o) in validatorDefs" class="span12" data-ng-model="item.CustomValidator2">
                                                        <option value="">-- No Validator --</option>
                                                    </select>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row-fluid">
                                            <div class="control-group pull-left span6">
                                                <label class="control-label">Group Validation</label>
                                                <div class="controls">
                                                    <select class="span12" data-ng-model="item.ValidationGroup">
                                                        <option value="">- No group validation -</option>
                                                        <option>group1</option>
                                                        <option>group2</option>
                                                        <option>group3</option>
                                                        <option>group4</option>
                                                        <option>group5</option>
                                                        <option>group6</option>
                                                        <option>group7</option>
                                                        <option>group8</option>
                                                        <option>group9</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="control-group pull-left condensed span5" data-ng-show="item.ValidationGroup">
                                                <div class="controls ">
                                                    <select data-ng-options="k as k for (k, o) in groupValidatorDefs" class="span12" data-ng-model="item.GroupValidator">
                                                        <option value="">-- No Validator --</option>
                                                    </select>
                                                </div>
                                            </div>
                                        </div>

                                        <%--<div class="event-root" data-ng-controller="EventCtrl" data-ng-init="init('submit')">
                                            <h4>On Submit</h4>
                                            <p style="margin-bottom: 10px;">
                                                Define a list of actions to be executed when the form is submited. 
                                                Note that the actions will be executed in the order you specify here.
                                            </p>
                                            <div data-af-actionlist=""></div>
                                        </div>--%>
                                    </fieldset>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div data-ng-show="mode=='layout'" class="layout accordion">

                    <p class="muted">
                        Drag entire rows using handles on the left or individual fiels by grabbing their title.<br />
                        Resize fields using the left and right resize arrows to.<br />
                        Action Form uses a 12 grid layout. The size and position will snap to the grid.
                    </p>

                    <p class="alert alert-block" data-ng-show="sharedData.settings.HasCustomLayout.Value">
                        <strong>Warning!</strong> Layout defined here will be ignore as long as the manual layout template is used above in Form Settings.
                    </p>

                    <div class="sortable-list accordion"
                        data-ui-sortable="{placeholder: 'alert alert-block alert-field', handle: '.handle', stop: rowOrderChanged}"
                        data-ng-model="grid">

                        <div class="row-fluid" data-ng-repeat="row in grid">
                            <div class="pull-left" style="">
                                <i class="handle icon-align-justify" title="Move the whole row..."></i>
                            </div>
                            <div class="span11 row-fluid" style="margin-left: 0;">
                                <div data-ng-repeat="col in row" class="col span{{col.span}}"
                                    data-ng-class="{deleted: col.field.IsDeleted, empty: !col.field.Title}"
                                    data-ng-model="col"
                                    data-drop="!col.field.Title"
                                    data-jqyoui-options="{ hoverClass: 'can-drop'}"
                                    jqyoui-droppable="{ hoverClass: 'can-drop', onDrop: 'fieldDroped'}" style="height: 38px;">

                                    <div class="field alert alert-info" data-ng-show="col.field.Title" data-drag="true"
                                        data-jqyoui-options="{revert: 'invalid', revertDuration: 100}"
                                        data-ng-model="col"
                                        jqyoui-draggable="{animate:false}"
                                        data-ui-span-resize="{ onResize: fieldResized }"
                                        data-ng-mouseenter="col.hovered=true"
                                        data-ng-mouseleave="col.hovered=false">
                                        <%-- data-ng-if="col.field">--%>
                                        {{col.field.Title}}
                                        <a class="btn btn-inverse btn-mini pull-right" data-ng-click="moveOnNewRow(col)" title="Move on new row" style="margin: 7px; cursor: pointer;" data-ng-show="col.hovered && col.span != 12 && siblingFields(col).length"><i class="icon-chevron-down icon-white"></i></a>
                                        <a class="btn btn-inverse btn-mini pull-right" data-ng-click="makeFullWidth(col)" title="Go full width" style="margin: 7px; cursor: pointer;" data-ng-show="col.hovered && col.span != 12 && !siblingFields(col).length"><i class="icon-resize-horizontal icon-white"></i></a>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="alert accordion-group" style="padding: 4px;">

                    <%--<div class="pull-right">
                        <button class="btn btn-link" data-ng-disabled="!fieldsModified">Cancel</button>
                    </div>--%>
                    <div class="pull-right">
                        <button class="btn" data-ng-click="save()" type="button" data-ng-disabled="!fieldsModified" data-ng-class="{'btn-link': !fieldsModified, 'btn-warning': fieldsModified}">Save</button>
                    </div>

                    <div class="btn-group btn-link-animate-trigger" data-ng-show="mode=='edit'">
                        <button class="btn btn-link btn-link-animate dropdown-toggle " data-toggle="dropdown" data-link-animate="info">
                            Add Field <span class="caret"></span>
                        </button>
                        <ul class="dropdown-menu">
                            <li data-ng-repeat="(key, def) in fieldDefGroups" class="dropdown-submenu">
                                <a href="#" onclick="return false;">{{key}}</a>
                                <ul class="dropdown-menu">
                                    <li data-ng-repeat="fieldDef in fieldDefGroups[key]"><a data-ng-click="addField(fieldDef);">{{fieldDef.Title}}</a></li>
                                </ul>
                            </li>
                        </ul>
                    </div>

                    <div class="btn-group btn-link-animate-trigger" data-ng-show="mode=='edit'">
                        <button class="btn btn-link btn-link-animate dropdown-toggle " data-toggle="dropdown" data-link-animate="info">
                            Add Predefined Field <span class="caret"></span>
                        </button>
                        <ul class="dropdown-menu">
                            <li data-ng-repeat="(key, def) in predefFields"><a data-ng-click="cloneField(def);">{{def.Title}}</a></li>
                        </ul>
                    </div>

                    <div class="clearfix"></div>

                </div>

            </section>

            <section class="form-horizontal" id="FormEvents">
                <h3 style="margin-bottom: 20px;">Form Events</h3>

                <div class="event-root control-group" data-ng-controller="EventCtrl" data-ng-init="init('init')">
                    <h4 class="control-label">On Init</h4>
                    <div class="controls">
                        <p style="margin: 16px 0 10px 0;">
                            These actions are executed before the form is displayed. 
                            Note that the actions will be executed in the order you specify here.
                        </p>
                        <div data-ng-include="'static/templates/action-list.html?v=' + sharedData.resourceVersion"></div>
                    </div>
                </div>

                <div class="event-root control-group" data-ng-controller="EventCtrl" data-ng-init="init('validation-failed', 'Any Server Validation Failed')">

                    <div class="pull-right" style="margin: 12px 0 0 6px;">
                        <div class="btn-group">
                            <button class="btn btn-info dropdown-toggle " data-toggle="dropdown">
                                <span data-ng-bind-html-unsafe="eventFriendlyTitle"></span>
                                <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu pull-right">
                                <li><a data-ng-click="init('validation-failed', 'Any Server Validation Failed')">Any Server Validation Failed</a></li>
                                <li data-ng-repeat="v in sharedData.serverValidators">
                                    <a data-ng-click="init(v.value, v.title)" data-ng-bind-html-unsafe="v.title"></a>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <h4 class="control-label">On Validation Failed</h4>

                    <div class="controls">

                        <p style="margin: 16px 0 10px 0;">
                            Define a list of actions to be executed when validation fails. 
                            Note that this can be handled per form or overriden for a specific server validator.
                            The actions will be executed in the order you specify here.
                        </p>

                        <div class="clearfix"></div>
                        <div data-ng-include="'static/templates/action-list.html?v=' + sharedData.resourceVersion"></div>
                    </div>
                </div>


                <%--before ending with this section, let's register js functions from action definitions--%>
                <div class="event-root" data-ng-controller="EventCtrl">
                    <div data-ng-repeat="act in actionDefs">
                        <div data-ng-bind-html-unsafe="act.JsFunctions"></div>
                    </div>
                </div>
            </section>


            <section id="License" style="margin-top: 20px;">
                <h3 style="margin-bottom: 20px;">License</h3>

                <p class="muted" runat="server" id="pnlNoActivations">
                    There are no registered licenses.
                </p>
                <div runat="server" id="pnlActivations">
                    <ol>
                        <asp:Repeater runat="server" ID="rpActivations">
                            <ItemTemplate>
                                <li>
                                    <strong>
                                        <%# 
                                (bool)DataBinder.Eval(Container.DataItem, "RegCode.IsTrial") 
                                    ? string.Format("Trial - {0} Days Left", DataBinder.Eval(Container.DataItem, "RegCode.DaysLeft") ) 
                                    : DataBinder.Eval(Container.DataItem, "RegistrationCode") 
                                        %>
                                    </strong>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ol>
                </div>

                <div style="margin: 30px 0px;">
                    <a href="<%= ActivateUrl %>" class="btn btn-info">Activate</a>
                    <asp:PlaceHolder runat="server" ID="btnUnlockTrial">
                        <a href="<%= UnlockTrialUrl %>" class="btn btn-warning">Unlock Trial</a>
                    </asp:PlaceHolder>
                    <a href="<%= avt.ActionForm.ActionFormController.BuyLink %>" class="btn btn-link" target="_blank">Buy License</a>
                </div>

            </section>

            <section id="LiveUpdates" style="margin-top: 20px;">
                <h3 style="margin-bottom: 20px;">Live Updates</h3>



                <div id="pnlLiveStatusActive" style="display: none;">
                    <p class="">
                        Following features are under developement:
                    </p>
                    <ul class="list">
                    </ul>
                </div>

                <div id="pnlLiveStatus" style="display: none;">
                    <p class="" style="">
                        <strong>Following are top feature requests:</strong>
                    </p>
                    <ol class="list">
                    </ol>
                    <p>
                        <a href="http://www.dnnsharp.com/Support#opturl=%2Faction-form%2Ftracker" target="_top">See all feature requests</a>
                    </p>
                    <div class="alert alert-block">
                        <strong>Make sure to vote and fund your favorite features. </strong>
                        <br />
                        This will affect the priority for development.
                    </div>
                </div>

            </section>

            <p>&#160;</p>

            <div style="font-family: verdana; padding: 10px; border-top: 1px solid #357EC7;">
                <a href="https://www.facebook.com/DnnSharp" target="_top">
                    <img border="0" src="http://static.dnnsharp.com/logo/facebook.png" alt="Like Us On Facebook" style="width: 16px; height: 16px;" /></a>&#160;
                    <a href="https://twitter.com/DnnSharp" target="_top">
                        <img border="0" src="http://static.dnnsharp.com/logo/twitter.png" alt="Follow Us On Twitter" style="width: 15px; height: 20px;" /></a>&#160;
                    <a href="http://www.youtube.com/dnnsharp" target="_top">
                        <img border="0" src="http://static.dnnsharp.com/logo/youtube.png" alt="Watch Us On Youtube" style="width: 18px; height: 15px;" /></a>
                |
                    <a href="http://action-form.dnnsharp.com/" target="_top" style="text-decoration: none; font-size: 12px;">Online Documentation</a>
                |
                    <a href="http://www.dnnsharp.com/dotnetnuke/modules/forms/action-form" target="_top" style="text-decoration: none; font-size: 12px;">Module Page</a>
                |
                    <a href="http://blog.dnnsharp.com/" target="_top" style="text-decoration: none; font-size: 12px;">Read Our Blog</a>
                |
                    <a href="http://www.dnnsharp.com/support#opturl=%2Faction-form" target="_top" style="text-decoration: none; font-size: 12px;">Contact Support</a>
            </div>

        </div>
    </form>


    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/angular.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>
    <%--<script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/angular.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>--%>
    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/jquery-ui.interactions.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>
    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/bootstrap/js/bootstrap.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>
    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/angular-sortable.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>
    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/angular-spanresize.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>
    <%--<script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/angular-dragdrop.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>--%>
    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/json2.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>
    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/bootstrap-select.min.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>
    <script type="text/javascript" src="<%= TemplateSourceDirectory%>/static/tinymce/tiny_mce.js"></script>
    <script type="text/javascript" src="<%= TemplateSourceDirectory%>/static/angular-tinymce.js"></script>
    <script type="text/javascript" src="<%= TemplateSourceDirectory%>/static/angular-dragdrop.js"></script>
    <script type="text/javascript" src="<%=TemplateSourceDirectory %>/static/admin.js?<%= avt.ActionForm.ActionFormController.Build %>"></script>


</body>
</html>
