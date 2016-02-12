using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TextFileParser
{
    public class TextParser
    {
        private string _lineDelimiter = CommonLineDelimiters.NL;
        private string _fieldDelimiter = CommonFieldDelimiters.Comma;
        private string _fieldWrapper = CommonFieldWrappers.None;
        private List<HeaderDefinition> _headerDefinitions = new List<HeaderDefinition>();
        private Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();
        public TextParser()
        {

        }

        public TextParser WithLineDelimiter(string lineDelimiter)
        {
            _lineDelimiter = lineDelimiter;
            return this;
        }

        public TextParser WithFieldDelimiter(string fieldDelimiter)
        {
            _fieldDelimiter = fieldDelimiter;
            return this;
        }

        public TextParser WithFieldsEnclosedBy(string fieldwrapper)
        {
            _fieldWrapper = fieldwrapper;
            return this;
        }

        public TextParser DefinedByHeaderDefinition(HeaderDefinition definition)
        {
            _headerDefinitions.Add(definition);
            return this;
        }

        public TextParser AddTypeMappping(string fieldName, Type type)
        {
            if(!_typeMap.ContainsKey(fieldName))
                _typeMap.Add(fieldName,type);
            return this;
        }

        public async Task<DataSet> ParseAsync(string rawString)
        {
            try
            {
                DataSet resultSet = new DataSet();
                string[] tokens = rawString.Split(new string[] { this._lineDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                //loop through and build headers
                foreach (HeaderDefinition def in _headerDefinitions.OrderBy(t => t.LineIndex))
                {
                    //parse header row in token index
                    string[] row = tokens[def.LineIndex].Split(new string[] { this._fieldDelimiter }, StringSplitOptions.None);
                    DataTable newDt = new DataTable(def.TableName);
                    foreach (string token in row)
                    {
                        if (_typeMap.ContainsKey(token))
                        {
                            newDt.Columns.Add(token.Trim(), _typeMap[token]);
                        }
                        else
                        {
                            //default to string
                            newDt.Columns.Add(token.Trim(), typeof(string));
                        }
                    }

                    resultSet.Tables.Add(newDt);
                }
                DataTable dt = null;
                for (int i = 0; i < tokens.Length; i++)
                {

                    if (_headerDefinitions.Where(t => t.LineIndex == i).Count() == 1)
                    {
                        dt = resultSet.Tables[_headerDefinitions.Where(t => t.LineIndex == i).First().TableName];
                    }
                    else
                    {
                        if (dt != null)
                        {
                            object[] rowSet = new object[dt.Columns.Count];
                            string[] rowTokens = tokens[i].Split(new string[] { this._fieldDelimiter }, StringSplitOptions.None);
                            //sanity check
                            if (rowSet.Length < rowTokens.Length)
                            {
                                throw new Exception("Header and type mapping does not match parsed row token count");
                            }
                            else
                            {
                                for (int z = 0; z < rowTokens.Length; z++)
                                {
                                    switch (dt.Columns[z].DataType.Name)
                                    {
                                        case "Int32":
                                            rowSet[z] = ParseAsInt(rowTokens[z]);
                                            break;
                                        case "Float":
                                            rowSet[z] = ParseAsFloat(rowTokens[z]);
                                            break;
                                        case "Decimal":
                                            rowSet[z] = ParseAsDecimal(rowTokens[z]);
                                            break;
                                        case "Int64":
                                            rowSet[z] = ParseAsLong(rowTokens[z]);
                                            break;
                                        case "DateTime":
                                            rowSet[z] = ParseAsXmlDateTime(rowTokens[z]);
                                            break;
                                        default:
                                            rowSet[z] = rowTokens[z];
                                            break;
                                    }

                                }
                                dt.Rows.Add(rowSet);
                            }
                        }
                    }
                }
                return resultSet;
            }
            catch(Exception exc)
            {
                throw new TextParsingException(exc.Message);
            }
        }

        public async Task<DataSet> ParseAsync(FileInfo nfo)
        {
            return await ParseAsync(new FileStream(nfo.FullName,FileMode.Open));
        }

        public async Task<DataSet> ParseAsync(Stream stream)
        {
            try
            {
                StreamReader reader = new StreamReader(stream);
                return await ParseAsync(reader.ReadToEnd());

            }
            catch(Exception exc)
            {
                throw;
            }
        }

        private int ParseAsInt(string token)
        {
            int valu = 0;
            if (string.IsNullOrEmpty(token)) return valu;
            bool result = int.TryParse(token, NumberStyles.AllowLeadingSign 
                | NumberStyles.AllowLeadingWhite 
                | NumberStyles.AllowThousands 
                | NumberStyles.AllowTrailingSign 
                | NumberStyles.AllowTrailingWhite, CultureInfo.CurrentCulture, out valu);
            if (result)
            {
                return valu;
            }
            else
                throw new Exception(string.Format("Token value {0} could not be parsed as an int",token));
        }

        private long ParseAsLong(string token)
        {
            long valu = 0;
            if (string.IsNullOrEmpty(token)) return valu;
            bool result = long.TryParse(token, NumberStyles.AllowLeadingSign
                | NumberStyles.AllowLeadingWhite
                | NumberStyles.AllowThousands
                | NumberStyles.AllowTrailingSign
                | NumberStyles.AllowTrailingWhite, CultureInfo.CurrentCulture, out valu);
            if (result)
            {
                return valu;
            }
            else
                throw new Exception(string.Format("Token value {0} could not be parsed as an int", token));
        }

        private float ParseAsFloat(string token)
        {
            float valu = 0;
            if (string.IsNullOrEmpty(token)) return valu;
            bool result = float.TryParse(token, NumberStyles.Number, CultureInfo.CurrentCulture, out valu);
            if (result)
            {
                return valu;
            }
            else
                throw new Exception(string.Format("Token value {0} could not be parsed as a float", token));
        }

        private double ParseAsDouble(string token)
        {
            double valu = 0;
            if (string.IsNullOrEmpty(token)) return valu;
            bool result = double.TryParse(token, NumberStyles.Number, CultureInfo.CurrentCulture, out valu);
            if (result)
            {
                return valu;
            }
            else
                throw new Exception(string.Format("Token value {0} could not be parsed as a float", token));
        }

        private decimal ParseAsDecimal(string token)
        {
            decimal valu = 0;
            if (string.IsNullOrEmpty(token)) return valu;
            bool result = decimal.TryParse(token,NumberStyles.Number,CultureInfo.CurrentCulture, out valu);
            if (result)
            {
                return valu;
            }
            else
                throw new Exception(string.Format("Token value {0} could not be parsed as a float", token));
        }

        private DateTime ParseAsXmlDateTime(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return DateTime.MinValue;
                return XmlConvert.ToDateTime(token, "yyyy-MM-ddTHH:mm:sszzz");
            }
            catch
            {
                throw new Exception(string.Format("Token value {0} could not be parsed as an Xml Date Time", token));
            }
        }
    }

    

    public class HeaderDefinition
    {
        public int LineIndex { get; set; }
        public string TableName { get; set; }


        public HeaderDefinition(int lineIndex, string tableName ="Header")
        {
            LineIndex = lineIndex;
            TableName = tableName;
        }

    }

    public static class CommonLineDelimiters
    {
        public const string CR = "\r";
        public const string NL = "\n";
        public const string CRNL = "\r";
    }

    public static class CommonFieldDelimiters
    {
        public const string Comma = ",";
        public const string Tab = "\t";
        public const string FixedLength = "";
    }

    public static class CommonFieldWrappers
    {
        public const string None = "";
        public const string SingleQuote = "'";
        public const string DoubleQuote = "\"\"";
    }

    public class TextParsingException : Exception
    {
        public TextParsingException(string message):base(message)
        {

        }
    }
}
