using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextFileParser
{
    public class Header
    {
        public List<Column> Columns { get; set; } 

        public Header()
        {
            Columns = new List<Column>();
        }
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

        public Column() { 
        }

        public Column(string columnName, string typeName)
        {
            this.ColumnName = columnName;
            this.TypeName = typeName;
        }

        public Column(string columnName, string displayName, string typeName)
        {
            this.ColumnName = columnName;
            this.TypeName = typeName;
            this.DisplayName = displayName;
        }
    }

    public class Data
    {
        public Header Header { get; set; }
        public List<Row> Rows { get; set; }

        System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
        {
            DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public Data()
        {
            Header = new Header();
            Rows = new List<Row>();
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Data FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<Data>(jsonString);
        }

        public byte[] ToSystemTextJson()
        {
            
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(this);
        }

        public static Data FromSystemTextJson(byte[] data)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Data>(new ReadOnlySpan<byte>(data));
        }
    }

    public class Row
    {
        public Dictionary<string, object> Values { get; set; }
        public Row()
        {
            Values = new Dictionary<string, object>();
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
