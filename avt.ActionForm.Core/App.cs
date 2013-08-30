using avt.ActionForm.Core.FileStorage;
using avt.ActionForm.Utils;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Url.FriendlyUrl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;

namespace avt.ActionForm.Core
{
    public class App
    {
        public static readonly string ModuleRelativePath = string.Format("{0}/DesktopModules/AvatarSoft/ActionForm", 
            HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'));

        #region Singleton

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static App Instance { get; private set; }

        /// <summary>
        /// Singleton initialization
        /// </summary>
        static App()
        {
            Instance = new App();
        }

        #endregion


        #region Initialization

        private App()
        {
            // app init
            //InitLogging();

            // init container
            Container = new LiteContainer();
            Container.RegisterProperty("ConnectionString", () => DotNetNuke.Common.Utilities.Config.GetConnectionString());
            Container.RegisterService<IFileStorage>(() => new LocalFileStorage());
        }

        #endregion


        const string MasterCacheKey = "avt.ActionForm";
        public static string GetMasterCacheKey()
        {
            if (HttpRuntime.Cache[MasterCacheKey] != null)
                HttpRuntime.Cache.Insert(MasterCacheKey, new object());

            return MasterCacheKey;
        }

        public LiteContainer Container { get; private set; }

    }


}
