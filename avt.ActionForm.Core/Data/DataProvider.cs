

using System;
using DotNetNuke;
using System.Data;
using System.Web.UI.WebControls;

using DotNetNuke.Framework;
using System.Data.SqlTypes;

namespace avt.ActionForm.Data
{
    public abstract class DataProvider
    {
        // singleton reference to the instantiated object 
        private static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)Reflection.CreateObject("data", "avt.ActionForm.Data", "");
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        [Obsolete]
        public abstract IDataReader GetFormSettings(int moduleId);
        [Obsolete]
        public abstract int UpdateFormSetting(int moduleId, string settingName, string settingValue);

        //public abstract int UpdateFormField(int FormFieldId, int ModuleId, string Title, string name, string shortDesc, string HelpText, 
        //    string InputType, string InputData, int ViewOrder, bool IsRequired, bool IsActive, string CssClass, 
        //    string CssStyles, string DefaultValue, int ColIndex, DateTime DateCreated, string LabelCssClass, string LabelCssStyles, 
        //    bool IsEnabled, string CustomValidator1, string CustomValidator2, string ValidationGroup, string GroupValidator, bool disableAutocomplete, int ColOffset, int ColSpan, int RowIndex); //, bool bSaveToUserProfile);
        //public abstract IDataReader GetFormFields(int moduleId);
        //public abstract void DeleteField(int formFieldId);
        //public abstract void DeleteFields(int moduleId);

        public abstract int SaveReport(int EntryId, int ModuleId, string ReferrerInfo, int UserId, string RemoteAddress, string BrowserInfo, DateTime dateSubmitted, string FormData, int SubmitStatus, string ActionStr, Guid ValidationKey, bool SkipValidation);
        public abstract IDataReader GetReport(int moduleId, SqlDateTime startDate, SqlDateTime endDate);
        public abstract IDataReader GetReportEntry(int entryId);
        public abstract IDataReader GetReportEntry(string reportKey);


    }
}
