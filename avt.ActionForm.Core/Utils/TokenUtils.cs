using avt.ActionForm.Core.Form;
using DotNetNuke.Services.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace avt.ActionForm.Utils
{
    public class TokenUtils
    {
        public static string Tokenize(string strContent)
        {
            return Tokenize(strContent, null, null, false, true);
        }

        public static bool TokenizeAsBool(string strContent)
        {
            var result = Tokenize(strContent, null, null, false, true);

            bool isValid;
            if (bool.TryParse(result, out isValid))
                return isValid;

            // also parse it as a number
            return result == "1";
        }

       public  static  string Tokenize(string strContent, DotNetNuke.Entities.Modules.ModuleInfo modInfo, DotNetNuke.Entities.Users.UserInfo user, bool forceDebug, bool bRevertToDnn)
        {
            string cacheKey_Installed = "avt.MyTokens2.Installed";
            string cacheKey_MethodReplace = "avt.MyTokens2.MethodReplace";

            string bMyTokensInstalled = "no";
            System.Reflection.MethodInfo methodReplace = null;

            bool bDebug = forceDebug;
            if (!bDebug) {
                try { bDebug = DotNetNuke.Common.Globals.IsEditMode(); } catch { }
            }

            lock (typeof(DotNetNuke.Services.Tokens.TokenReplace)) {
                // first, determine if MyTokens is installed
                if (HttpRuntime.Cache.Get(cacheKey_Installed) == null) {

                    // check again, maybe current thread was locked by another which did all the work
                    if (HttpRuntime.Cache.Get(cacheKey_Installed) == null) {

                        // it's not in cache, let's determine if it's installed
                        try {
                            Type myTokensRepl = DotNetNuke.Framework.Reflection.CreateType("avt.MyTokens.MyTokensReplacer", true);
                            if (myTokensRepl == null)
                                throw new Exception(); // handled in catch

                            bMyTokensInstalled = "yes";

                            // we now know MyTokens is installed, get ReplaceTokensAll methods
                            methodReplace = myTokensRepl.GetMethod(
                                "ReplaceTokensAll",
                                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                                null,
                                System.Reflection.CallingConventions.Any,
                                new Type[] { 
                                    typeof(string), 
                                    typeof(DotNetNuke.Entities.Users.UserInfo), 
                                    typeof(bool),
                                    typeof(DotNetNuke.Entities.Modules.ModuleInfo)
                                },
                                null
                            );

                            if (methodReplace == null) {
                                // this shouldn't really happen, we know MyTokens is installed
                                throw new Exception();
                            }

                        } catch {
                            bMyTokensInstalled = "no";
                        }

                        // cache values so next time the funciton is called the reflection logic is skipped
                        HttpRuntime.Cache.Insert(cacheKey_Installed, bMyTokensInstalled);
                        if (bMyTokensInstalled == "yes") {
                            HttpRuntime.Cache.Insert(cacheKey_MethodReplace, methodReplace);
                        }
                    }
                }
            }

            bMyTokensInstalled = HttpRuntime.Cache.Get(cacheKey_Installed).ToString();
            if (bMyTokensInstalled == "yes") {
                methodReplace = (System.Reflection.MethodInfo)HttpRuntime.Cache.Get(cacheKey_MethodReplace);
                if (methodReplace == null) {
                    HttpRuntime.Cache.Remove(cacheKey_Installed);
                    return Tokenize(strContent, modInfo, user, forceDebug, bRevertToDnn);
                }
            } else {
                // if it's not installed return string or tokenize with DNN replacer
                if (!bRevertToDnn) {
                    return strContent;
                } else {
                    DotNetNuke.Services.Tokens.TokenReplace dnnTknRepl = new DotNetNuke.Services.Tokens.TokenReplace();
                    dnnTknRepl.AccessingUser = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
                    dnnTknRepl.User = user ?? DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
                    dnnTknRepl.DebugMessages = bDebug;
                    if (modInfo != null)
                        dnnTknRepl.ModuleInfo = modInfo;

                    // MyTokens is not installed, execution ends here
                    return dnnTknRepl.ReplaceEnvironmentTokens(strContent);
                }
            }

            // we have MyTokens installed, proceed to token replacement
            return (string)methodReplace.Invoke(
                null,
                new object[] {
                    strContent,
                    user ?? DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo(),
                    bDebug,
                    modInfo
                }
            );

        }

    }

    public class AfFieldTokenReplacer
    {
        public ActionFormSettings Settings { get; set; }
        XslCompiledTransform Transform { get; set; }

        public AfFieldTokenReplacer(ActionFormSettings settings)
        {
            Settings = settings;
            Transform = LoadTransform("main");
        }

        XslCompiledTransform LoadTransform(string name)
        {
            var transform = new XslCompiledTransform();
            XsltSettings xmlSettings = new XsltSettings();
            xmlSettings.EnableScript = true;

            var tplPath = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm\\templates\\Form\\" + Settings.FormTemplate);
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture.ToString();

            // fallback to default template
            if (!Directory.Exists(tplPath)) {
                tplPath = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm\\templates\\Form\\" + ActionFormSettings.DefaultFormTemplate);
            }

            string tpl = string.Format("controls\\{0}.{1}.xsl", name, currentCulture);
            if (!File.Exists(Path.Combine(tplPath, tpl)))
                tpl = string.Format("controls\\{0}.xsl", name);

            transform.Load(Path.Combine(tplPath, tpl), xmlSettings, new XmlUrlResolver());
            return transform;
        }

        public string GetHtml(string fieldName, FormData InitData, string xmlSettings)
        {
            if (!Settings.FieldsByName.ContainsKey(fieldName) || !Settings.FieldsByName[fieldName].IsVisibile)
                return "";

            //Settings.FieldsByName[fieldName].Parameters.Remove("ShowIn");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);

            // render on field at a time
            xmlDoc.DocumentElement["Fields"].InnerXml = xmlDoc.DocumentElement["Fields"].SelectSingleNode("Field[Name=\"" + fieldName + "\"]").OuterXml;

            XsltArgumentList args = new XsltArgumentList();
            args.AddExtensionObject("af:tokens", new XslUtils());

            System.IO.StringWriter output = new System.IO.StringWriter();
            Transform.Transform(xmlDoc, args, output);

            return output.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template">btn-submit/btn-cancel</param>
        /// <returns></returns>
        public string GetHtmlForTemplate(string template)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Settings.SerializeToXmlStr(null, true));

            XsltArgumentList args = new XsltArgumentList();
            args.AddExtensionObject("af:tokens", new XslUtils());

            System.IO.StringWriter output = new System.IO.StringWriter();
            LoadTransform(template).Transform(xmlDoc, args, output);
            return output.ToString();
        }
    }
}
