using avt.ActionForm.Core.Utils.Data;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Data;
using System.Data.SqlTypes;

namespace avt.ActionForm.Data
{
    public class SqlDataProvider : DataProvider
    {

        #region "Private Members"

        private const string ProviderType = "data";

        SqlTable _TableFormFields;
        SqlTable _TableFormSettings;
        SqlTable _TableReport;
        SqlTable _TableActions;

        private DotNetNuke.Framework.Providers.ProviderConfiguration _providerConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;

        #endregion

        #region "Constructors"

        public SqlDataProvider()
        {

            // Read the configuration specific information for this provider
            DotNetNuke.Framework.Providers.Provider objProvider = (DotNetNuke.Framework.Providers.Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            //Get Connection string from web.config
            _connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

            if (_connectionString == "") {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false) {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false) {
                _databaseOwner += ".";
            }

            _TableFormFields = new SqlTable(
                ConnectionString,
                DatabaseOwner + ObjectQualifier + "avtActionForm_FormFields",
                true,
                new string[] { "FormFieldId" },
                "ModuleId", "Title", "Name", "ShortDesc", "HelpText", "InputTypeStr", "InputData", "ViewOrder",
                "IsRequired", "IsActive", "CssClass", "CssStyles", "DefaultValue", "ColIndex", "DateCreated",
                "LabelCssClass", "LabelCssStyles", "IsEnabled",
                "CustomValidator1", "CustomValidator2", "ValidationGroup", "GroupValidator", "DisableAutocomplete", 
                "ColOffset", "ColSpan", "RowIndex" // , "SaveToUserProfile"
            );

            _TableFormSettings = new SqlTable(
                ConnectionString,
                DatabaseOwner + ObjectQualifier + "avtActionForm_FormSettings",
                false,
                new string[] { "ModuleId", "Name" }, "Value"
            );

            _TableReport = new SqlTable(
                ConnectionString,
                DatabaseOwner + ObjectQualifier + "avtActionForm_Reports",
                true,
                new string[] { "ReportEntryId" },
                "ModuleId", "ReferrerInfo", "UserId", "RemoteAddress", "BrowserInfo", "DateSubmitted", "FormData", "SubmitStatus", "ActionStr", "ValidationKey", "SkipValidation"
            );

            _TableActions = new SqlTable(
                ConnectionString,
                DatabaseOwner + ObjectQualifier + "avtActionForm_FormActions",
                true,
                new string[] { "Id" },
                "ModuleId", "Description", "EventName", "ActionType", "ActionData", "Condition", "OrderIndex", "LastModified", "LastModifiedBy"
            );
        }

        #endregion

        #region "Properties"

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public string ProviderPath
        {
            get { return _providerPath; }
        }

        public string ObjectQualifier
        {
            get { return _objectQualifier; }
        }

        public string DatabaseOwner
        {
            get { return _databaseOwner; }
        }

        #endregion



        #region Public Methods

        public override IDataReader GetFormSettings(int moduleId)
        {
            return _TableFormSettings.Get("Where ModuleId=" + moduleId);
        }

        public override int UpdateFormSetting(int moduleId, string settingName, string settingValue)
        {
            return _TableFormSettings.Update(
                    new object[] { moduleId, settingName },
                    settingValue);
        }

        //public override int UpdateFormField(int FormFieldId, int ModuleId, string Title, string name, string shortDesc,
        //    string HelpText, string InputType, string InputData, int ViewOrder, bool IsRequired, bool IsActive, string CssClass,
        //    string CssStyles, string DefaultValue, int ColIndex, DateTime DateCreated, string LabelCssClass, string LabelCssStyles,
        //    bool IsEnabled, string CustomValidator1, string CustomValidator2, string ValidationGroup, string GroupValidator, bool disableAutocomplete, int ColOffset, int ColSpan, int RowIndex) //, bool bSaveToUserProfile)
        //{
        //    return _TableFormFields.Update(
        //            new object[] { FormFieldId },
        //            ModuleId, Title, name, shortDesc, HelpText, InputType, InputData, ViewOrder,
        //            IsRequired, IsActive, CssClass, CssStyles, DefaultValue, ColIndex, DateCreated,
        //            LabelCssClass, LabelCssStyles, IsEnabled,
        //            CustomValidator1, CustomValidator2, ValidationGroup, GroupValidator, disableAutocomplete, ColOffset, ColSpan, RowIndex //, bSaveToUserProfile 
        //        );
        //}

        //public override IDataReader GetFormFields(int moduleId)
        //{
        //    return _TableFormFields.Get("Where ModuleId=" + moduleId + " Order By ViewOrder");
        //}

        //public override void DeleteField(int formFieldId)
        //{
        //    _TableFormFields.Delete(new object[] { formFieldId });
        //}

        //public override void DeleteFields(int moduleId)
        //{
        //    _TableFormFields.Delete("ModuleID=" + moduleId);
        //}


        public override int SaveReport(int EntryId, int ModuleId, string ReferrerInfo, int UserId, string RemoteAddress, string BrowserInfo,
            DateTime dateSubmitted, string FormData, int SubmitStatus, string ActionStr, Guid ValidationKey, bool SkipValidation)
        {
            return _TableReport.Update(new object[] { EntryId }, ModuleId, ReferrerInfo, UserId, RemoteAddress, BrowserInfo, dateSubmitted,
                FormData, SubmitStatus, ActionStr, ValidationKey, SkipValidation);
        }

        public override IDataReader GetReport(int moduleId, SqlDateTime startDate, SqlDateTime endDate)
        {
            string sql = string.Format(
                "SELECT * FROM {0} Where ModuleId = {1} AND DateSubmitted >= {2} AND DateSubmitted <= {3} Order By DateSubmitted",
                DatabaseOwner + ObjectQualifier + "avtActionForm_Reports",
                moduleId,
                SqlTable.EncodeSql(startDate),
                SqlTable.EncodeSql(endDate)
            );
            return SqlHelper.ExecuteReader(_connectionString, CommandType.Text, sql);
        }

        public override IDataReader GetReportEntry(int entryId)
        {
            return _TableReport.Get("where ReportEntryId = " + entryId);
        }

        public override IDataReader GetReportEntry(string reportKey)
        {
            return _TableReport.Get("where ValidationKey = " + SqlTable.EncodeSql(reportKey));
        }

        //public override int UpdateActionInfo(int id, int moduleId, string description, string eventName, string actionType, string actionData, string condition, int orderIndex, DateTime lastModified, int lastModifiedBy)
        //{
        //    return _TableActions.Update(new object[] { id }, moduleId, description, eventName, actionType, actionData, condition, orderIndex, lastModified, lastModifiedBy);
        //}

        //public override IDataReader GetActions(int moduleId)
        //{
        //    return _TableActions.Get("Where ModuleId=" + moduleId + " Order By OrderIndex");
        //}

        //public override IDataReader GetActions(int moduleId, string eventName)
        //{
        //    return _TableActions.Get("Where ModuleId=" + moduleId + " and EventName=" + SqlTable.EncodeSql(eventName) + " Order By OrderIndex");
        //}

        //public override IDataReader GetAction(int actionId)
        //{
        //    return _TableActions.Get("Where Id=" + actionId);
        //}

        //public override void DeleteAction(int id)
        //{
        //    _TableActions.Delete(id);
        //}

        //public override void DeleteAllActions(int moduleId)
        //{
        //    _TableActions.Delete("ModuleId=" + moduleId);

        //}


        #endregion

    }
}
