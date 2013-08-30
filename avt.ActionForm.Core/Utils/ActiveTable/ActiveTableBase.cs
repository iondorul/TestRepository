using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using avt.ActionForm.Utils;
using avt.ActionForm.Core.Utils;

namespace DnnSharp.Common.ActiveTable
{
    /// <summary>
    /// This class is public because Entity classes derive from this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActiveTableBase<T>
    {
        #region Metadata

        static string ConnString { get; set; }
        static string TableName { get; set; }
        static bool IsIdentity { get; set; }
        static IEnumerable<PropertyInfo> PrimaryKey { get; set; }
        static IEnumerable<PropertyInfo> Fields { get; set; }
        static IEnumerable<PropertyInfo> AllFields { get; set; }
        static IEnumerable<PropertyInfo> ManyRelations { get; set; }

        // get it once with the static constructor, so all instances can use (read) these
        static ActiveTableBase()
        {
            ConnString = DotNetNuke.Common.Utilities.Config.GetConnectionString();
            TableName = (typeof(T).GetCustomAttributes(false).FirstOrDefault(x => x is TableAttribute) as TableAttribute).Name;

            PrimaryKey = GetFieldsByAttribute<PrimaryKeyAttribute>();
            if (PrimaryKey.Count() == 0)
                throw new ArgumentException("Could not find a primary key");

            IsIdentity = PrimaryKey.Count() == 1 && PrimaryKey.First().PropertyType == typeof(int) &&
                (PrimaryKey.First().GetCustomAttributes(false).FirstOrDefault(x => x is PrimaryKeyAttribute) as PrimaryKeyAttribute).IsIdentity;

            Fields = GetFieldsByAttribute<FieldAttribute>();
            AllFields = new List<PropertyInfo>(PrimaryKey);
            (AllFields as List<PropertyInfo>).AddRange(Fields);

            ManyRelations = GetFieldsByAttribute<HasManyAttribute>();
            if (!IsIdentity && ManyRelations.Count() > 0)
                throw new ArgumentException("HasMany only works with identity columns now");
        }

        static IEnumerable<PropertyInfo> GetFieldsByAttribute<TA>()
            where TA : Attribute
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => Attribute.IsDefined(x, typeof(TA)));
        }

        #endregion


        public ActiveTableBase()
        {

        }

        public virtual void Save()
        {
            // compute SQL query
            string sql = SqlUpdate();

            if (IsIdentity) {
                PrimaryKey.First().SetValue(this, Convert.ToInt32(SqlHelper.ExecuteScalar(ConnString, CommandType.Text, sql, null)), null);
            } else {
                SqlHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql, null);
            }

            // TODO also bulk update relations
            //foreach (var rel in ManyRelations) {

            //    var subItems = rel.GetValue(this, null);


            //    var ids = new Dictionary<int, object>();
            //    ids[(int)PrimaryKey.First().GetValue(this, null)] = this;

            //    // also initialize
            //    rel.SetValue(ids[(int)PrimaryKey.First().GetValue(this, null)], Activator.CreateInstance(typeof(List<>).MakeGenericType(rel.PropertyType.GetGenericArguments()[0])), null);

            //    var col = ((HasManyAttribute)rel.GetCustomAttributes(false).First(x => x is HasManyAttribute)).ForeignKeyColumnName;

            //    // get the GetAllByProperty for the relation, so we call it via reflection
            //    var method = rel.PropertyType.GetGenericArguments()[0].GetMethod("GetAllByProperty", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            //    var subItems = method.Invoke(null, new object[] { col, ids.Keys });

            //    // we have the list of subitems, load it into the list of items
            //    foreach (var sub in (IEnumerable)subItems) {
            //        var parentId = (int)sub.GetType().GetProperty(col).GetValue(sub, null);

            //        (rel.GetValue(ids[parentId], null) as IList).Add(sub);
            //    }
            //}
        }

        string SqlUpdate()
        {
            if (IsIdentity)
                return ((int)PrimaryKey.First().GetValue(this, null)) > 0 ? SqlEdit() : SqlAdd();

            return @"
                IF EXISTS (SELECT 1 FROM " + TableName + " Where " + JoinConditions(" AND ") + @")
                    " + SqlEdit() + @"
                ELSE
                    " + SqlAdd() + @"
                ";
        }

        string SqlAdd()
        {
            StringBuilder sbSql = new StringBuilder();
            if (IsIdentity) {
                sbSql.AppendFormat("INSERT INTO {0} ({1}) VALUES ", TableName, string.Join(",", Fields.Select(x => x.Name).ToArray()));
            } else {
                // append insert check to avoid duplicates
                sbSql.AppendFormat("IF NOT EXISTS(Select 1 FROM {0} WHERE ", TableName);
                foreach (var key in PrimaryKey)
                    sbSql.AppendFormat("{0}{1} AND ", key.Name, SqlEquals(key.GetValue(this, null)));
                sbSql.Remove(sbSql.Length - " AND ".Length, " AND ".Length);

                sbSql.AppendFormat(") INSERT INTO {0} ({1}) VALUES ", TableName, string.Join(",", Fields.Select(x => x.Name).ToArray()));
            }

            // append input
            sbSql.Append('(');
            if (!IsIdentity) {
                foreach (var pKey in PrimaryKey)
                    sbSql.AppendFormat("{0},", EncodeSql(pKey.GetValue(this, null)));
            }

            foreach (var field in Fields)
                sbSql.AppendFormat("{0},", EncodeSql(field.GetValue(this, null)));

            // remove last coma
            if (sbSql[sbSql.Length - 1] == ',')
                sbSql.Remove(sbSql.Length - 1, 1);

            sbSql.Append(')');
            sbSql.Append("\n Select SCOPE_IDENTITY()");

            return sbSql.ToString();
        }

        string SqlEdit()
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("UPDATE {0} SET ", TableName);

            // append input
            foreach (var field in Fields)
                sbSql.AppendFormat("{0}={1},", field.Name, EncodeSql(field.GetValue(this, null)));

            // remove last coma
            if (sbSql[sbSql.Length - 1] == ',')
                sbSql = sbSql.Remove(sbSql.Length - 1, 1);

            // append wheere
            sbSql.Append(" WHERE ");
            sbSql.Append(JoinConditions(" AND "));

            if (IsIdentity)
                sbSql.AppendFormat("\n \n SELECT {0}", PrimaryKey.First().GetValue(this, null));

            return sbSql.ToString();
        }

        public static IList<T> GetAllByProperty(string orderBy = null, params KeyValuePair<string, object>[] criteria)
        {
            // TODO: validate propname?
            string sql = string.Format("SELECT {1} FROM {0} WHERE {2}{3}",
                TableName, string.Join(",", AllFields.Select(x => x.Name).ToArray()),
                string.Join(" AND ", (from k in criteria
                                      select k.Key + SqlEquals(k.Value)).ToArray()),
                string.IsNullOrEmpty(orderBy) ? "" : " Order By " + orderBy);
            IList<T> data = CBO.FillCollection<T>(SqlHelper.ExecuteReader(ConnString, CommandType.Text, sql, null));

            // satisfy HasMany for all objects at once
            // HasMany only works with identity primary keys for now
            foreach (var rel in ManyRelations) {
                var ids = new Dictionary<int, object>();
                foreach (var item in data) {
                    ids[(int)PrimaryKey.First().GetValue(item, null)] = item;

                    // also initialize
                    rel.SetValue(ids[(int)PrimaryKey.First().GetValue(item, null)], Activator.CreateInstance(typeof(List<>).MakeGenericType(rel.PropertyType.GetGenericArguments()[0])), null);
                }

                if (ids.Count == 0)
                    continue;

                var hasManyCol = ((HasManyAttribute)rel.GetCustomAttributes(false).First(x => x is HasManyAttribute)).ForeignKeyColumnName;
                var hasManyOrderBy = ((HasManyAttribute)rel.GetCustomAttributes(false).First(x => x is HasManyAttribute)).OrderBy;

                // get the GetAllByProperty for the relation, so we call it via reflection
                var method = rel.PropertyType.GetGenericArguments()[0].GetMethod("GetAllByProperty", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                var subItems = method.Invoke(null, new object[] { hasManyOrderBy, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(hasManyCol, ids.Keys) } });

                // we have the list of subitems, load it into the list of items
                foreach (var sub in (IEnumerable)subItems) {
                    var parentId = (int?)sub.GetType().GetProperty(hasManyCol).GetValue(sub, null);
                    if (parentId.HasValue)
                        (rel.GetValue(ids[parentId.Value], null) as IList).Add(sub);
                }
            }


            return data;
        }

        public static T GetOneByProperty(string propName, object value)
        {
            // TODO: validate propname?
            string sql = string.Format("SELECT top 1 {1} FROM {0} WHERE {2}{3}",
                TableName, string.Join(",", AllFields.Select(x => x.Name).ToArray()), propName, SqlEquals(value));
            T data = CBO.FillObject<T>(SqlHelper.ExecuteReader(ConnString, CommandType.Text, sql, null));

            // satisfy HasMany for all objects at once
            // HasMany only works with identity primary keys for now
            foreach (var rel in ManyRelations) {
                var ids = new Dictionary<int, object>();
                ids[(int)PrimaryKey.First().GetValue(data, null)] = data;

                // also initialize
                rel.SetValue(ids[(int)PrimaryKey.First().GetValue(data, null)], Activator.CreateInstance(typeof(List<>).MakeGenericType(rel.PropertyType.GetGenericArguments()[0])), null);

                var col = ((HasManyAttribute)rel.GetCustomAttributes(false).First(x => x is HasManyAttribute)).ForeignKeyColumnName;

                // get the GetAllByProperty for the relation, so we call it via reflection
                var method = rel.PropertyType.GetGenericArguments()[0].GetMethod("GetAllByProperty", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                var subItems = method.Invoke(null, new object[] { col, ids.Keys });

                // we have the list of subitems, load it into the list of items
                foreach (var sub in (IEnumerable)subItems) {
                    var parentId = (int)sub.GetType().GetProperty(col).GetValue(sub, null);

                    (rel.GetValue(ids[parentId], null) as IList).Add(sub);
                }
            }


            return data;
        }

        public virtual void Delete()
        {
            string sql = string.Format("DELETE FROM {0} WHERE {1}", TableName, JoinConditions(" AND "));
            SqlHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql, null);
        }

        public static void DeleteAllByProperty(string propName, object value)
        {
            // TODO: validate propname?
            string sql = string.Format("DELETE FROM {0} WHERE {1}{2}",
                TableName, propName, SqlEquals(value));

            SqlHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql, null);
        }

        //public void UpdateField(string fieldName, object data, string whereCond)
        //{
        //    string sql = string.Format("UPDATE {0} SET {1}={2} {3}", TableName, fieldName, EncodeSql(data), string.IsNullOrEmpty(whereCond) ? "" : "WHERE " + whereCond);
        //    SqlHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql, null);
        //}

        //public void Delete(params object[] pKeys)
        //{
        //    string sql = string.Format("DELETE FROM {0} WHERE {1}", TableName, JoinConditions(" AND "));
        //    SqlHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql, null);
        //}

        //public void Delete(string whereCond)
        //{
        //    string sql = string.Format("DELETE FROM {0} WHERE {1}", TableName, whereCond);
        //    SqlHelper.ExecuteNonQuery(ConnString, CommandType.Text, sql, null);
        //}

        //// TODO: enhance this with specifics (where, orders, etc)
        //public IDataReader Get(string appendSql)
        //{
        //    string sql = string.Format("SELECT {1} FROM {0} {2}", TableName, string.Join(",", AllFields.Select(x => x.Name).ToArray()), appendSql);
        //    return SqlHelper.ExecuteReader(ConnString, CommandType.Text, sql, null);
        //}

        //public IDataReader Get(int top, string appendSql)
        //{
        //    string sql = string.Format("SELECT TOP {3} {1} FROM {0} {2}", TableName, string.Join(",", AllFields.Select(x => x.Name).ToArray()), appendSql, top);
        //    return SqlHelper.ExecuteReader(ConnString, CommandType.Text, sql, null);
        //}

        //public IDataReader GetDistinct(string[] columns, string appendSql)
        //{
        //    string sql = string.Format("SELECT DISTINCT {1} FROM {0} {2}", TableName, string.Join(",", columns), appendSql);
        //    return SqlHelper.ExecuteReader(ConnString, CommandType.Text, sql, null);
        //}

        //public object GetScalar(string column, string appendSql)
        //{
        //    string sql = string.Format("SELECT {1} FROM {0} {2}", TableName, column, appendSql);
        //    return SqlHelper.ExecuteScalar(ConnString, CommandType.Text, sql, null);
        //}


        // UTILs

        string JoinConditions(string sep)
        {
            StringBuilder sbCond = new StringBuilder();
            foreach (var key in PrimaryKey)
                sbCond.AppendFormat("{0}{1}{2}", key.Name, SqlEquals(key.GetValue(this, null)), sep);

            sbCond = sbCond.Remove(sbCond.Length - sep.Length, sep.Length);
            return sbCond.ToString();
        }

        static string SqlEquals(object val)
        {
            if (ConvertUtils.IsNullable(val) && val == null)
                return " IS NULL";

            if (typeof(string) == val.GetType())
                return "=" + EncodeSql(val);

            if (typeof(IEnumerable).IsAssignableFrom(val.GetType())) {
                var sb = new StringBuilder();
                sb.Append(" IN (");
                foreach (var o in (IEnumerable)val)
                    sb.AppendFormat("{0},", EncodeSql(o));
                if (sb[sb.Length - 1] == ',')
                    sb.Remove(sb.Length - 1, 1);
                sb.Append(")");

                return sb.ToString();
            }

            return "=" + EncodeSql(val);
        }

        public static string EncodeSql(object data)
        {
            Type dataType;
            try {
                dataType = data.GetType();
            } catch {
                return "NULL";
            }

            if (dataType == typeof(string)) {
                return string.Format("N'{0}'", data.ToString().Replace("'", "''"));
            } else if (dataType == typeof(DateTime)) {
                return string.Format("N'{0}'", ((DateTime)data).ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture).Replace("'", "''"));
            } else if (dataType == typeof(SqlDateTime)) {
                return string.Format("N'{0}'", ((SqlDateTime)data).Value.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture).Replace("'", "''"));
            } else if (dataType == typeof(int)) {
                if ((int)data == int.MinValue)
                    return "NULL";
                return data.ToString();
            } else if (dataType == typeof(int?)) {
                if (((int?)data).HasValue)
                    return ((int?)data).Value.ToString();
                return "NULL";
            } else if (dataType == typeof(double)) {
                if ((double)data == double.MinValue)
                    return "NULL";
                return data.ToString();
            } else if (dataType == typeof(double?)) {
                if (((double?)data).HasValue)
                    return ((double?)data).Value.ToString();
                return "NULL";
            } else if (dataType == typeof(bool)) {
                return (bool)data ? "1" : "0";
            } else if (dataType == typeof(bool?)) {
                if (((bool?)data).HasValue)
                    return ((bool?)data).Value ? "1" : "0";
                return "NULL";
            }

            throw new Exception("Input data type not supported for update");
        }
    }
}
