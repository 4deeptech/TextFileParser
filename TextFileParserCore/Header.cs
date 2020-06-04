using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextFileParser
{
    public class Header
    {
        public List<Column> Columns = new List<Column>();
        public void AddColumn(Column column)
        {
            Columns.Add(column);
        }
    }

    public class Column
    {
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public string TypeName { get; set; }

        public Column(string columnName, string typeName)
        {
            this.ColumnName = columnName;
            this.TypeName = typeName;
        }
    }

    public class Data
    {
        public Header Header { get; set; } = new Header();
        public List<Row> Rows = new List<Row>();
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Data FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<Data>(jsonString);
        }
    }

    public class Row
    {
        public Dictionary<string, object> Values = new Dictionary<string, object>();
        public Row()
        {

        }
        public Row(string[] row)
        {

        }

        public void AddValue(string columnName, object value)
        {
            Values.Add(columnName, value);
        }
    }
}
