﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dolkens.Framework.MVC
{
    public static class DataTablesResult
    {
        private static Dictionary<Type, Dictionary<String, PropertyInfo>> _jsonMaps = new Dictionary<Type, Dictionary<String, PropertyInfo>> { };

        public static IEnumerable<RowType> Filter<RowType>(IEnumerable<RowType> input)
        {
            var request = HttpContext.Current.Request;

            RowType[] data = null;
            var query = input.AsEnumerable();

            Int32 cCount = request["iColumns"].ToInt32(0);
            Dictionary<Int32, PropertyInfo> propertyMap = new Dictionary<Int32, PropertyInfo> { };

            #region Search Support

            if (cCount > 0)
            {
                if (!_jsonMaps.ContainsKey(typeof(RowType)))
                {
                    var properties = typeof(RowType).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new
                    {
                        Property = p,
                        JsonProperty = p.GetCustomAttribute<JsonPropertyAttribute>(true),
                    });
                    _jsonMaps[typeof(RowType)] = properties.ToDictionary(k => k.JsonProperty == null ? k.Property.Name : k.JsonProperty.PropertyName, v => v.Property);
                }

                var jsonMap = _jsonMaps[typeof(RowType)];

                for (Int32 cIndex = 0; cIndex < cCount; cIndex++)
                {
                    var columnName = request[String.Format("mDataProp_{0}", cIndex)];
                    var columnSearch = request[String.Format("sSearch_{0}", cIndex)];
                    var columnRegex = request[String.Format("bRegex_{0}", cIndex)].ToBoolean(false);

                    if (!String.IsNullOrWhiteSpace(columnName))
                    {
                        if (jsonMap.ContainsKey(columnName))
                        {
                            propertyMap[cIndex] = jsonMap[columnName];

                            if (!String.IsNullOrWhiteSpace(columnSearch))
                            {
                                if (!columnRegex)
                                {
                                    if (jsonMap[columnName].PropertyType == typeof(String))
                                    {
                                        query = query.Where(r => jsonMap[columnName].GetValue(r).ToString().ToLowerInvariant().Contains(columnSearch.ToLowerInvariant()));
                                    }
                                    else
                                    {
                                        Object searchValue = columnSearch.Parse(jsonMap[columnName].PropertyType);
                                        query = query.Where(r => jsonMap[columnName].GetValue(r).Equals(searchValue));
                                        // .ToString().Equals(columnSearch, StringComparison.InvariantCultureIgnoreCase));
                                    }
                                }
                                else
                                {
                                    query = query.Where(r => Regex.IsMatch(jsonMap[columnName].GetValue(r).ToString(), columnSearch, RegexOptions.IgnoreCase));
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Sorting Support

            Int32 sCount = request["iSortingCols"].ToInt32(0);

            if (sCount > 0)
            {
                IOrderedEnumerable<RowType> sQuery = null;

                Int32 sorts = 0;
                Int32 sortColumn;
                String sortDirection;

                for (Int32 cIndex = 0; cIndex < sCount; cIndex++)
                {
                    if (Int32.TryParse(request[String.Format("iSortCol_{0}", cIndex)], out sortColumn))
                    {
                        if (propertyMap.ContainsKey(sortColumn))
                        {
                            sortDirection = request[String.Format("sSortDir_{0}", cIndex)];

                            if (sorts++ == 0)
                            {
                                if (sortDirection == "desc")
                                {
                                    sQuery = query.OrderByDescending(r => propertyMap[sortColumn].GetValue(r));
                                }
                                else
                                {
                                    sQuery = query.OrderBy(r => propertyMap[sortColumn].GetValue(r));
                                }
                            }
                            else
                            {
                                if (sortDirection == "desc")
                                {
                                    sQuery = sQuery.ThenByDescending(r => propertyMap[sortColumn].GetValue(r));
                                }
                                else
                                {
                                    sQuery = sQuery.ThenBy(r => propertyMap[sortColumn].GetValue(r));
                                }
                            }
                        }
                    }
                }

                if (sQuery != null)
                {
                    data = sQuery.ToArray();
                }
            }

            if (data == null)
            {
                data = query.ToArray();
            }

            #endregion

            return data;
        }
    }

    public class DataTablesResult<RowType> : JsonNetResult
    {
        const String JsonRequest_GetNotAllowed = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";

        public DataTablesResult()
            : base()
        {
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public new IEnumerable<RowType> Data
        {
            get { return (IEnumerable<RowType>)base.Data; }
            set { base.Data = value; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(DataTablesResult<RowType>.JsonRequest_GetNotAllowed);

            HttpRequestBase request = context.HttpContext.Request;
            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(this.ContentType))
                response.ContentType = this.ContentType;
            else
                response.ContentType = "application/json";

            if (this.ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (this.Data == null) return;

            Int32 displayStart = request["iDisplayStart"].ToInt32(0);
            Int32 displayLength = request["iDisplayLength"].ToInt32(100);

            var data = DataTablesResult.Filter<RowType>(this.Data);

            response.Write(new
            {
                sEcho = request["sEcho"],
                recordsTotal = this.Data.Count(),
                recordsFiltered = data.Count(),
                data = data.Skip(displayStart).Take(displayLength),
            }.ToJSON());
        }
    }
}