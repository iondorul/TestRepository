using avt.ActionForm.Core.Utils.Data;
using avt.ActionForm.Utils;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace avt.ActionForm.Core.Utils.Config
{
    public class DbConfig
    {
        public string TableName { get; set; }

        public DbConfig(string tableName)
        {
            TableName = DotNetNuke.Common.Utilities.Config.GetDataBaseOwner() + '[' +
                DotNetNuke.Common.Utilities.Config.GetObjectQualifer() + tableName.Trim('[', ']') + ']';

            // initialize all properties to their default
            foreach (var field in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {

                var attr = field.GetCustomAttributes(false).FirstOrDefault(x => x is PropertyAttribute) as PropertyAttribute;
                if (attr == null)
                    continue;

                if (!typeof(ISetting).IsAssignableFrom(field.PropertyType))
                    throw new ArgumentException("Setting " + field.Name + " should be a Setting object");

                var setting = Activator.CreateInstance(field.PropertyType) as ISetting;
                setting.Name = field.Name;
                setting.ValueObj = attr.Default;
                field.SetValue(this, setting, null);
            }
        }

        public virtual void Load()
        {
            IDictionary<string, object> sectionData = ParseSectionAttributes();

            var sbSql = new StringBuilder();
            sbSql.AppendFormat("SELECT * FROM {0} Where ", TableName);
            foreach (var s in sectionData)
                sbSql.AppendFormat("{0}={1} AND ", s.Key, SqlTable.EncodeSql(s.Value));
            sbSql.Remove(sbSql.Length - 5, 4);

            using (var dr = SqlHelper.ExecuteReader(DotNetNuke.Common.Utilities.Config.GetConnectionString(), System.Data.CommandType.Text, sbSql.ToString())) {
                while (dr.Read()) {
                    var name = dr["Name"].ToString();
                    var field = (from f in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 where f.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && typeof(ISetting).IsAssignableFrom(f.PropertyType)
                                 select f).FirstOrDefault();

                    if (field == null)
                        continue;

                    var setting = Activator.CreateInstance(field.PropertyType) as ISetting;
                    setting.Name = name;
                    setting.ValueObj = dr["Value"] is DBNull ? null : dr["Value"];
                    setting.CanOverride = (bool) dr["CanOverride"];
                    setting.Inherit = (bool) dr["Inherit"];
                    setting.LastModified = (DateTime) dr["LastModified"];
                    setting.LastModifiedBy = dr["LastModifiedBy"] is DBNull ? null : (int?) dr["LastModifiedBy"];
                    field.SetValue(this, setting, null);
                }
            }

            //var dr = SqlHelper.ExecuteReader(DotNetNuke.Common.Utilities.Config.GetConnectionString(), System.Data.CommandType.Text, sbSql.ToString());
            //foreach (var setting in DotNetNuke.Common.Utilities.CBO.FillCollection<Setting>(dr)) {
            //    var field = (from f in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //                 where f.PropertyType == typeof(Setting) && f.Name.Equals(setting.Name, StringComparison.OrdinalIgnoreCase)
            //                 select f).FirstOrDefault();
            //    if (field != null)
            //        field.SetValue(this, setting, null);
            //}
        }

        public virtual void Save()
        {
            IDictionary<string, object> sectionData = ParseSectionAttributes();

            var sbSql = new StringBuilder();
            sbSql.AppendLine("BEGIN TRANSACTION");

            // now iterate all properties and update each one
            foreach (var field in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {

                var attr = field.GetCustomAttributes(false).FirstOrDefault(x => x is PropertyAttribute) as PropertyAttribute;
                if (attr == null)
                    continue;

                if (!typeof(ISetting).IsAssignableFrom(field.PropertyType))
                    throw new ArgumentException("Setting " + field.Name + " should be a Setting object");

                var setting = field.GetValue(this, null) as ISetting;

                sbSql.AppendFormat("IF NOT EXISTS(Select 1 FROM {0} WHERE ", TableName);
                foreach (var s in sectionData)
                    sbSql.AppendFormat("{0}={1} AND ", s.Key, SqlTable.EncodeSql(s.Value));
                sbSql.AppendFormat("Name={0} ", SqlTable.EncodeSql(setting.Name));

                // this is the insert statement
                sbSql.AppendFormat(") INSERT INTO {0} ({1},Name,Value,CanOverride,Inherit,LastModified,LastModifiedBy) VALUES (", TableName, string.Join(",", sectionData.Keys.ToArray()));
                foreach (var s in sectionData)
                    sbSql.AppendFormat("{0},", SqlTable.EncodeSql(s.Value));
                sbSql.AppendFormat("{0},{1},{2},{3},{4},{5})",
                    SqlTable.EncodeSql(setting.Name), SqlTable.EncodeSql(setting.ToString()),
                    SqlTable.EncodeSql(setting.CanOverride), SqlTable.EncodeSql(setting.Inherit), 
                    SqlTable.EncodeSql(setting.LastModified), SqlTable.EncodeSql(setting.LastModifiedBy));

                // folowed by the update
                sbSql.AppendFormat("ELSE UPDATE {0} SET ", TableName);
                foreach (var s in sectionData)
                    sbSql.AppendFormat("{0}={1},", s.Key, SqlTable.EncodeSql(s.Value));
                sbSql.AppendFormat("Name={0},Value={1},CanOverride={2},Inherit={3},LastModified={4},LastModifiedBy={5}\n\n",
                    SqlTable.EncodeSql(setting.Name), SqlTable.EncodeSql(setting.ToString()), 
                    SqlTable.EncodeSql(setting.CanOverride), SqlTable.EncodeSql(setting.Inherit), 
                    SqlTable.EncodeSql(setting.LastModified), SqlTable.EncodeSql(setting.LastModifiedBy));
                
                sbSql.Append("WHERE ");
                foreach (var s in sectionData)
                    sbSql.AppendFormat("{0}={1} AND ", s.Key, SqlTable.EncodeSql(s.Value));
                sbSql.AppendFormat("Name={0} ", SqlTable.EncodeSql(setting.Name));

            }

            sbSql.AppendLine("COMMIT");

            SqlHelper.ExecuteNonQuery(DotNetNuke.Common.Utilities.Config.GetConnectionString(), System.Data.CommandType.Text, sbSql.ToString());

        }

        IDictionary<string, object> ParseSectionAttributes()
        {
            IDictionary<string, object> sectionData = new Dictionary<string, object>();

            // extract section attributes
            foreach (var field in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                var attr = field.GetCustomAttributes(false).FirstOrDefault(x => x is SectionAttribute) as SectionAttribute;
                if (attr == null)
                    continue;

                sectionData[field.Name] = field.GetValue(this, null);
            }

            return sectionData;
        }
    }
}
