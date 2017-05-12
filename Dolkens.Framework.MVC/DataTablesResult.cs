using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExtensionMethods = Dolkens.Framework.Extensions.ExtensionMethods;

namespace Dolkens.Framework.MVC
{
    public static class DataTablesResult
    {
        private static Dictionary<Type, Dictionary<String, PropertyInfo>> _jsonMaps = new Dictionary<Type, Dictionary<String, PropertyInfo>> { };

        private static Boolean AnyItem(this Object data, Type dataType, String searchTerm, Boolean exactMatch = false)
        {
            if (data == null)
            {
                return false;
            }
            else if (typeof(String) == dataType)
            {
                if (exactMatch) return ((String)data).Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase);

                return ((String)data).ToLowerInvariant().Contains(searchTerm.ToLowerInvariant());
            }
            else if (typeof(IEnumerable).IsAssignableFrom(dataType))
            {
                foreach (var item in (IEnumerable)data)
                {
                    if (item.AnyItem(item.GetType(), searchTerm, exactMatch)) return true;
                }
                
                return false;
            }
            else
            {
                Object searchValue = searchTerm.Parse(dataType);
                return data.Equals(searchValue);
            }
        }

        private static Boolean AnyItem(this Object data, Type dataType, Regex searchTerm)
        {
            if (data == null)
            {
                return false;
            }
            else if (typeof(String) == dataType)
            {
                return searchTerm.IsMatch((String)data);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(dataType))
            {
                foreach (var item in (IEnumerable)data)
                {
                    if (item.AnyItem(item.GetType(), searchTerm))
                        return true;
                }

                return false;
            }
            else
            {
                return searchTerm.IsMatch(data.ToString());
            }
        }
       
        private static Object SortItem(this Object data, Type dataType)
        {
            if (data == null)
            {
                return data;
            }
            else if (typeof(String) != dataType && typeof(IEnumerable).IsAssignableFrom(dataType))
            {
                var enumerator = ((IEnumerable)data).GetEnumerator();

                if (enumerator.MoveNext())
                {
                    return enumerator.Current.SortItem(enumerator.Current.GetType());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return data;
            }
        }

        public static IEnumerable<RowType> Filter<RowType>(IEnumerable<RowType> input, Boolean exactMatch = false)
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
                                    query = query.Where(r => jsonMap[columnName].GetValue(r).AnyItem(jsonMap[columnName].PropertyType, columnSearch, exactMatch));
                                }
                                else
                                {
                                    Regex match = new Regex(columnSearch, RegexOptions.IgnoreCase);

                                    query = query.Where(r => jsonMap[columnName].GetValue(r).AnyItem(jsonMap[columnName].PropertyType, match));
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
                String sortDirection;

                for (Int32 cIndex = 0; cIndex < sCount; cIndex++)
                {
                    Int32 sortColumn;

                    if (Int32.TryParse(request[String.Format("iSortCol_{0}", cIndex)], out sortColumn))
                    {
                        if (propertyMap.ContainsKey(sortColumn))
                        {
                            sortDirection = request[String.Format("sSortDir_{0}", cIndex)];
                            
                            if (sorts++ == 0)
                            {
                                if (sortDirection == "desc")
                                {
                                    sQuery = query.OrderByDescending(r => propertyMap[sortColumn].GetValue(r).SortItem(propertyMap[sortColumn].PropertyType));
                                }
                                else
                                {
                                    sQuery = query.OrderBy(r => propertyMap[sortColumn].GetValue(r).SortItem(propertyMap[sortColumn].PropertyType));
                                }
                            }
                            else
                            {
                                if (sortDirection == "desc")
                                {
                                    sQuery = sQuery.ThenByDescending(r => propertyMap[sortColumn].GetValue(r).SortItem(propertyMap[sortColumn].PropertyType));
                                }
                                else
                                {
                                    sQuery = sQuery.ThenBy(r => propertyMap[sortColumn].GetValue(r).SortItem(propertyMap[sortColumn].PropertyType));
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

        public static IEnumerable<RowType> FilterColumn<RowType>(IEnumerable<RowType> input, String column, Boolean exactMatch = false)
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

                    if (column.Equals(columnName))
                    {
                        if (!String.IsNullOrWhiteSpace(columnName))
                        {
                            if (jsonMap.ContainsKey(columnName))
                            {
                                propertyMap[cIndex] = jsonMap[columnName];

                                if (!String.IsNullOrWhiteSpace(columnSearch))
                                {
                                    if (!columnRegex)
                                    {
                                        query = query.Where(r => jsonMap[columnName].GetValue(r).AnyItem(jsonMap[columnName].PropertyType, columnSearch, exactMatch));
                                    }
                                    else
                                    {
                                        Regex match = new Regex(columnSearch, RegexOptions.IgnoreCase);

                                        query = query.Where(r => jsonMap[columnName].GetValue(r).AnyItem(jsonMap[columnName].PropertyType, match));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            if (data == null)
            {
                data = query.ToArray();
            }

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