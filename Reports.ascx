<%@ Control Language="C#" AutoEventWireup="True" Inherits="avt.ActionForm.Reports" EnableViewState = "true" Codefile="Reports.ascx.cs" %>

<div class = "avtBox sunny">

    <div class = "formHeader" style = "margin-bottom: 16px;">Download Report for form <i><asp:Label runat = "server" ID = "lbFormName"></asp:Label></i></div>
    
    <b>Start Date</b><br />
    <asp:TextBox runat = "server" ID = "tbStartDate"></asp:TextBox>
    <span style = "color: #828282; font-style: italic; font-size: 11px;">leave blank to get all entries before start date</span>
    <br /><br />

    <b>End Date</b><br />
    <asp:TextBox runat = "server" ID = "tbEndDate"></asp:TextBox>
    <span style = "color: #828282; font-style: italic; font-size: 11px;">leave blank to get all entries until today</span>
    
    <br /><br />

    <div style = "text-align: center; margin: 30px 40px 10px 0;">
        <asp:LinkButton runat = "server" OnClick = "OnDownloadReport" style = "font-size: 14px; font-weight: bold; color: #4a8094; margin-right: 10px;" CausesValidation = "false">Download as CSV</asp:LinkButton>
        <a href = "<%= DotNetNuke.Common.Globals.NavigateURL(TabId) %>" style = "font-size: 12px; font-weight: bold; color: #CC0000; font-style: normal;">Back</a>
    </div>

</div>

<div class = "avtBox sunny">

    <div class = "formHeader" style = "margin-bottom: 16px;">Upload CSV Report</div>
    <p style="color: #525252;">If you need to make modifications to the submitted data, download the CSV above then upload it back in the form below. </p>
    <b>CSV File</b><br />
    <asp:FileUpload runat = "server" ID = "uplCsv"></asp:FileUpload>
    <br />
    
    <div style = "text-align: center; margin: 30px 40px 10px 0;">
        <asp:LinkButton runat = "server" OnClick = "OnUploadReport" style = "font-size: 14px; font-weight: bold; color: #4a8094; margin-right: 10px;" CausesValidation = "false">Upload</asp:LinkButton>
        <a href = "<%= DotNetNuke.Common.Globals.NavigateURL(TabId) %>" style = "font-size: 12px; font-weight: bold; color: #CC0000; font-style: normal;">Back</a>
    </div>

    <div runat="server" id ="pnlImportStatus" style="color: #4a8094; text-align: center;"></div>

</div>


<script type = "text/javascript">


afjQuery(document).ready(function() {
    // initialize date pickers
    afjQuery("#<%= tbStartDate.ClientID %>").datepicker();
    afjQuery("#<%= tbEndDate.ClientID %>").datepicker();
});


</script>

