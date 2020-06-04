using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    string[] row = null;
                    if(this._fieldDelimiter == CommonFieldDelimiters.Comma && this._fieldWrapper == CommonFieldWrappers.DoubleQuote)
                    {
                        //MatchCollection matches = new Regex("((?<=\")[^\"]*(?=\"(,|$)+)|(?<=,|^)[^,\"]*(?=,|$))").Matches(tokens[def.LineIndex]);
                        //List<string> result = new List<string>();
                        //foreach (var match in matches)
                        //{
                        //    result.Add(match.ToString());
                        //}
                        //row = result.ToArray();
                        row = SplitRow(tokens[def.LineIndex]).ToArray();
                    }
                    else
                    {
                        row = this._fieldDelimiter == CommonFieldDelimiters.Comma ?
                            tokens[def.LineIndex].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None) :
                            tokens[def.LineIndex].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None);
                    }

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
                DataTable dtErrors = new DataTable("ParseErrors");
                dtErrors.Columns.Add(new DataColumn("RawData", typeof(string)));
                dtErrors.Columns.Add(new DataColumn("Exception", typeof(string)));
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
                            try
                            {
                                object[] rowSet = new object[dt.Columns.Count];
                                string[] rowTokens = null;
                                if (this._fieldDelimiter == CommonFieldDelimiters.Comma && this._fieldWrapper == CommonFieldWrappers.DoubleQuote)
                                {
                                    //MatchCollection matches = new Regex("((?<=\")[^\"]*(?=\"(,|$)+)|(?<=,|^)[^,\"]*(?=,|$))").Matches(tokens[i]);
                                    //List<string> result = new List<string>();
                                    //foreach (var match in matches)
                                    //{
                                    //    result.Add(match.ToString());
                                    //}
                                    //rowTokens = result.ToArray();
                                    rowTokens = SplitRow(tokens[i]).ToArray();
                                }
                                else
                                {
                                    rowTokens = this._fieldDelimiter == CommonFieldDelimiters.Comma ?
                                    tokens[i].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None) :
                                    tokens[i].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None);
                                }

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
                            catch(Exception ex)
                            {
                                dtErrors.Rows.Add(new string[] { tokens[i], ex.Message });
                                if(resultSet.Tables["ParseErrors"] == null)
                                {
                                    resultSet.Tables.Add(dtErrors);
                                }
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

        public async Task<Data> ParseDataAsync(string rawString)
        {
            try
            {
                Data newDt = new Data();
                string[] tokens = rawString.Split(new string[] { this._lineDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                //loop through and build headers
                foreach (HeaderDefinition def in _headerDefinitions.OrderBy(t => t.LineIndex))
                {
                    //parse header row in token index
                    string[] row = null;
                    if (this._fieldDelimiter == CommonFieldDelimiters.Comma && this._fieldWrapper == CommonFieldWrappers.DoubleQuote)
                    {
                        row = SplitRow(tokens[def.LineIndex]).ToArray();
                    }
                    else
                    {
                        row = this._fieldDelimiter == CommonFieldDelimiters.Comma ?
                            tokens[def.LineIndex].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None) :
                            tokens[def.LineIndex].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None);
                    }

                    foreach (string token in row)
                    {
                        newDt.Header.AddColumn(new Column(token.Trim(), _typeMap[token].Name));
                    }
                }
                
                for (int i = _headerDefinitions.OrderBy(t => t.LineIndex).Last().LineIndex+1; i < tokens.Length; i++)
                {
                    try
                    {
                        object[] rowSet = new object[newDt.Header.Columns.Count];
                        string[] rowTokens = null;
                        if (this._fieldDelimiter == CommonFieldDelimiters.Comma && this._fieldWrapper == CommonFieldWrappers.DoubleQuote)
                        {
                            rowTokens = SplitRow(tokens[i]).ToArray();
                        }
                        else
                        {
                            rowTokens = this._fieldDelimiter == CommonFieldDelimiters.Comma ?
                            tokens[i].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None) :
                            tokens[i].Split(new string[] { this._fieldWrapper, this._fieldDelimiter }, StringSplitOptions.None);
                        }

                        //sanity check
                        if (rowSet.Length < rowTokens.Length)
                        {
                            throw new Exception("Header and type mapping does not match parsed row token count");
                        }
                        else
                        {
                            Row row = new Row();
                            for (int z = 0; z < rowTokens.Length; z++)
                            {
                                switch (newDt.Header.Columns[z].TypeName)
                                {
                                    case "Int32":
                                        row.AddValue(newDt.Header.Columns[z].ColumnName, ParseAsInt(rowTokens[z]));
                                        //rowSet[z] = ParseAsInt(rowTokens[z]);
                                        break;
                                    case "Float":
                                        row.AddValue(newDt.Header.Columns[z].ColumnName, ParseAsFloat(rowTokens[z]));
                                        //rowSet[z] = ParseAsFloat(rowTokens[z]);
                                        break;
                                    case "Decimal":
                                        row.AddValue(newDt.Header.Columns[z].ColumnName, ParseAsDecimal(rowTokens[z]));
                                        //rowSet[z] = ParseAsDecimal(rowTokens[z]);
                                        break;
                                    case "Int64":
                                        row.AddValue(newDt.Header.Columns[z].ColumnName, ParseAsLong(rowTokens[z]));
                                        //rowSet[z] = ParseAsLong(rowTokens[z]);
                                        break;
                                    case "DateTime":
                                        row.AddValue(newDt.Header.Columns[z].ColumnName, ParseAsXmlDateTime(rowTokens[z]));
                                        //rowSet[z] = ParseAsXmlDateTime(rowTokens[z]);
                                        break;
                                    default:
                                        row.AddValue(newDt.Header.Columns[z].ColumnName, rowTokens[z]);
                                        //rowSet[z] = rowTokens[z];
                                        break;
                                }

                            }
                            newDt.Rows.Add(row);
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                return newDt;
            }
            catch (Exception exc)
            {
                throw new TextParsingException(exc.Message);
            }
        }

        public static IEnumerable<string> SplitRow(string row, char delimiter = ',')
        {
            var currentString = new StringBuilder();
            var inQuotes = false;
            var quoteIsEscaped = false; //Store when a quote has been escaped.
            row = string.Format("{0}{1}", row, delimiter); //We add new cells at the delimiter, so append one for the parser.
            foreach (var character in row.Select((val, index) => new { val, index }))
            {
                if (character.val == delimiter) //We hit a delimiter character...
                {
                    if (!inQuotes) //Are we inside quotes? If not, we've hit the end of a cell value.
                    {
                        //Console.WriteLine(currentString);
                        yield return currentString.ToString();
                        currentString.Clear();
                    }
                    else
                    {
                        currentString.Append(character.val);
                    }
                }
                else
                {
                    if (character.val != ' ')
                    {
                        if (character.val == '"') //If we've hit a quote character...
                        {
                            if (character.val == '"' && inQuotes) //Does it appear to be a closing quote?
                            {
                                if (row[character.index + 1] == character.val && !quoteIsEscaped) //If the character afterwards is also a quote, this is to escape that (not a closing quote).
                                {
                                    quoteIsEscaped = true; //Flag that we are escaped for the next character. Don't add the escaping quote.
                                }
                                else if (quoteIsEscaped)
                                {
                                    quoteIsEscaped = false; //This is an escaped quote. Add it and revert quoteIsEscaped to false.
                                    currentString.Append(character.val);
                                }
                                else if (!quoteIsEscaped && row[character.index + 1] != delimiter)
                                {
                                    currentString.Append(character.val); //...It's a quote inside a quote but is not escaped damnit Amazon
                                }
                                else
                                {
                                    inQuotes = false;
                                }
                            }
                            else
                            {
                                if (!inQuotes)
                                {
                                    inQuotes = true;
                                }
                                else
                                {
                                    currentString.Append(character.val); //...It's a quote inside a quote.
                                }
                            }
                        }
                        else
                        {
                            currentString.Append(character.val);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(currentString.ToString())) //Append only if not new cell
                        {
                            currentString.Append(character.val);
                        }
                    }
                }
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

        public async Task<Data> ParseDataAsync(FileInfo nfo)
        {
            return await ParseDataAsync(new FileStream(nfo.FullName, FileMode.Open));
        }

        public async Task<Data> ParseDataAsync(Stream stream)
        {
            try
            {
                StreamReader reader = new StreamReader(stream);
                return await ParseDataAsync(reader.ReadToEnd());

            }
            catch (Exception exc)
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
                throw new Exception(string.Format("Token value {0} could not be parsed as a long", token));
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
                //fall back to regular date time parse
                try
                {
                    if (string.IsNullOrEmpty(token)) return DateTime.MinValue;
                    return DateTime.Parse(token);
                }
                catch
                {
                    throw new Exception(string.Format("Token value {0} could not be parsed as an Xml Date Time or a generic DateTime", token));
                }
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
