using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFileParser;

namespace TextFileParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //TextParser parser
            //    = new TextFileParser.TextParser()
            //    .WithLineDelimiter(CommonLineDelimiters.NL)
            //    .WithFieldDelimiter(",")
            //    .DefinedByHeaderDefinition(new HeaderDefinition(0))
            //    .DefinedByHeaderDefinition(new HeaderDefinition(3))
            //    .AddTypeMappping("Name", typeof(string))
            //    .AddTypeMappping("City", typeof(string))
            //    .AddTypeMappping("Age", typeof(Int64))
            //    .AddTypeMappping("Address", typeof(string))
            //    .AddTypeMappping("State", typeof(string))
            //    .AddTypeMappping("Postal", typeof(string));
            string tabSpacingForOutput = "\t\t\t\t";
            //DataSet ds = parser.ParseAsync("Name,City,Age\nDarren,OKC,42b\nForrest,Ningbao,25\nAddress,State,Postal\n1813 Elmhurst,OK,73013\n").Result;
            TextParser parser
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
               .WithFieldDelimiter(CommonFieldDelimiters.Tab)
               .DefinedByHeaderDefinition(new HeaderDefinition(0, "SettlementItem"))
               .AddTypeMappping("settlement-id", typeof(string))
               .AddTypeMappping("settlement-start-date", typeof(string))
               .AddTypeMappping("settlement-end-date", typeof(string))
               .AddTypeMappping("deposit-date", typeof(string))
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
               .AddTypeMappping("posted-date", typeof(string))
               .AddTypeMappping("order-item-code", typeof(string))
               .AddTypeMappping("merchant-order-item-id", typeof(string))
               .AddTypeMappping("merchant-adjustment-item-id", typeof(string))
               .AddTypeMappping("sku", typeof(string))
               .AddTypeMappping("quantity-purchased", typeof(int))
               .AddTypeMappping("price-type", typeof(string))
               .AddTypeMappping("price-amount", typeof(decimal))
               .AddTypeMappping("item-related-fee-type", typeof(string))
               .AddTypeMappping("item-related-fee-amount", typeof(decimal))
               .AddTypeMappping("misc-fee-amount", typeof(string))
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
            foreach (DataTable dt in ds.Tables)
            {
                Console.WriteLine("DataTable name = " + dt.TableName);
                foreach(DataColumn dc in dt.Columns)
                {
                    Console.Write(dc.ColumnName);
                    Console.Write(tabSpacingForOutput);
                    
                }
                Console.Write("\n");
                foreach (DataRow dr in dt.Rows)
                {
                    for(int i=0;i<dr.ItemArray.Length; i++)
                    {
                        Console.Write(dr[i]);
                        Console.Write(tabSpacingForOutput);
                    }
                    Console.Write("\n");
                }
            }

            Console.ReadKey();
        }
    }
}
