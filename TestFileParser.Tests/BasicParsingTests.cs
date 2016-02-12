using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFileParser;
using Xunit;

namespace TestFileParser.Tests
{
    public class BasicParsingTests
    {

        [Fact(DisplayName = "Test_New_Line_Tab_Delimited_With_Single_Header_and_Row_Group")]
        public void Test_New_Line_Tab_Delimited_With_Single_Header_and_Row_Group_And_thousands_comma()
        {
            TextParser parser
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
               .WithFieldDelimiter(CommonFieldDelimiters.Tab)
               .DefinedByHeaderDefinition(new HeaderDefinition(0, "Person"))
               .AddTypeMappping("Name", typeof(string))
               .AddTypeMappping("City", typeof(string))
               .AddTypeMappping("Age", typeof(Int64))
               .AddTypeMappping("Address", typeof(string))
               .AddTypeMappping("State", typeof(string))
               .AddTypeMappping("Postal", typeof(string));
            DataSet ds = parser.ParseAsync("Name\tCity\tAge\nDarren\tOKC\t1,042\nForrest\tNingbao\t25\n").Result;
            Assert.Equal(ds.Tables.Count, 1);
            Assert.NotNull(ds.Tables["Person"]);
            Assert.Equal(ds.Tables["Person"].Rows.Count, 2);
        }

        [Fact(DisplayName = "Test_New_Line_Comma_Delimited_With_Single_Header_and_Row_Group")]
        public void Test_New_Line_Comma_Delimited_With_Single_Header_and_Row_Group()
        {
            TextParser parser
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
               .WithFieldDelimiter(CommonFieldDelimiters.Comma)
               .DefinedByHeaderDefinition(new HeaderDefinition(0,"Person"))
               .AddTypeMappping("Name", typeof(string))
               .AddTypeMappping("City", typeof(string))
               .AddTypeMappping("Age", typeof(Int64))
               .AddTypeMappping("Address", typeof(string))
               .AddTypeMappping("State", typeof(string))
               .AddTypeMappping("Postal", typeof(string));
            DataSet ds = parser.ParseAsync("Name,City,Age\nDarren,OKC,42\nForrest,Ningbao,25\n").Result;
            Assert.Equal(ds.Tables.Count,1);
            Assert.NotNull(ds.Tables["Person"]);
            Assert.Equal(ds.Tables["Person"].Rows.Count , 2);
        }

        [Fact(DisplayName = "Test_New_Line_Comma_Delimited_With_Dual_Headers_and_Row_Group")]
        public void Test_New_Line_Comma_Delimited_With_Dual_Headers_and_Row_Group()
        {
            TextParser parser
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
               .WithFieldDelimiter(CommonFieldDelimiters.Comma)
               .DefinedByHeaderDefinition(new HeaderDefinition(0, "Person"))
               .DefinedByHeaderDefinition(new HeaderDefinition(3, "Address"))
               .AddTypeMappping("Name", typeof(string))
               .AddTypeMappping("City", typeof(string))
               .AddTypeMappping("Age", typeof(Int64))
               .AddTypeMappping("Address", typeof(string))
               .AddTypeMappping("State", typeof(string))
               .AddTypeMappping("Postal", typeof(string));
            DataSet ds = parser.ParseAsync("Name,City,Age\nDarren,OKC,42\nForrest,Ningbao,25\nAddress,State,Postal\n1813 Elmhurst,OK,73013\n").Result;
            Assert.Equal(ds.Tables.Count, 2);
            Assert.NotNull(ds.Tables["Person"]);
            Assert.NotNull(ds.Tables["Address"]);
            Assert.Equal(ds.Tables["Person"].Rows.Count, 2);
            Assert.Equal(ds.Tables["Address"].Rows.Count, 1);
        }

        [Fact(DisplayName = "Int_with_AlphaNumeric_Fails_with_Parse_Failure")]
        public void Int_with_AlphaNumeric_Fails_with_Parse_Failure()
        {
            TextParser parser
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
               .WithFieldDelimiter(CommonFieldDelimiters.Comma)
               .DefinedByHeaderDefinition(new HeaderDefinition(0, "Person"))
               .AddTypeMappping("Name", typeof(string))
               .AddTypeMappping("City", typeof(string))
               .AddTypeMappping("Age", typeof(Int64))
               .AddTypeMappping("Address", typeof(string))
               .AddTypeMappping("State", typeof(string))
               .AddTypeMappping("Postal", typeof(string));
            Assert.Throws<AggregateException>(() => parser.ParseAsync("Name,City,Age\nDarren,OKC,42b\nForrest,Ningbao,25\n").Result);
        }

        

        [Fact(DisplayName = "Test_Sample_Settlement_File")]
        public void Test_Sample_Settlement_File()
        {
            TextParser parser
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
               .WithFieldDelimiter(CommonFieldDelimiters.Tab)
               .DefinedByHeaderDefinition(new HeaderDefinition(0, "SettlementItem"))
               .AddTypeMappping("settlement-id", typeof(string))
               .AddTypeMappping("settlement-start-date", typeof(DateTime))
               .AddTypeMappping("settlement-end-date", typeof(DateTime))
               .AddTypeMappping("deposit-date", typeof(DateTime))
               .AddTypeMappping("total-amount", typeof(decimal))
               .AddTypeMappping("currency", typeof(string))
               .AddTypeMappping("transaction-type", typeof(string))
               .AddTypeMappping("order-id", typeof(string))
               .AddTypeMappping("merchant-order-id", typeof(string))
               .AddTypeMappping("adjustment-id", typeof(string))
               .AddTypeMappping("shipment-id", typeof(string))
               .AddTypeMappping("marketplace-name", typeof(string))
               .AddTypeMappping("shipment-fee-type", typeof(string))
               .AddTypeMappping("shipment-fee-amount", typeof(decimal))
               .AddTypeMappping("order-fee-type", typeof(string))
               .AddTypeMappping("order-fee-amount", typeof(decimal))
               .AddTypeMappping("fulfillment-id", typeof(string))
               .AddTypeMappping("posted-date", typeof(DateTime))
               .AddTypeMappping("order-item-code", typeof(string))
               .AddTypeMappping("merchant-order-item-id", typeof(string))
               .AddTypeMappping("merchant-adjustment-item-id", typeof(string))
               .AddTypeMappping("sku", typeof(string))
               .AddTypeMappping("quantity-purchased", typeof(int))
               .AddTypeMappping("price-type", typeof(string))
               .AddTypeMappping("price-amount", typeof(decimal))
               .AddTypeMappping("item-related-fee-type", typeof(string))
               .AddTypeMappping("item-related-fee-amount", typeof(decimal))
               .AddTypeMappping("misc-fee-amount", typeof(decimal))
               .AddTypeMappping("other-fee-amount", typeof(decimal))
               .AddTypeMappping("other-fee-reason-description", typeof(string))
               .AddTypeMappping("promotion-id", typeof(string))
               .AddTypeMappping("promotion-type", typeof(string))
               .AddTypeMappping("promotion-amount", typeof(decimal))
               .AddTypeMappping("direct-payment-type", typeof(string))
               .AddTypeMappping("direct-payment-amount", typeof(decimal))
               .AddTypeMappping("other-amount", typeof(decimal))
               ;
            DataSet ds = parser.ParseAsync(new FileInfo("settlement_sample.csv")).Result;
            Assert.Equal(ds.Tables.Count, 1);
            Assert.NotNull(ds.Tables["SettlementItem"]);
            //test total rows read
            Assert.Equal(ds.Tables["SettlementItem"].Rows.Count, 3792);
            //test value in last slot
            Assert.Equal(ds.Tables["SettlementItem"].Rows[3791]["other-amount"],4.22m);
        }
    }
}
