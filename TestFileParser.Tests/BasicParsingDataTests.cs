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
    public class BasicParsingDataTests
    {

        [Fact(DisplayName = "Data_Test_New_Line_Tab_Delimited_With_Single_Header_and_Row_Group")]
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
            Data ds = parser.ParseDataAsync("Name\tCity\tAge\nDarren\tOKC\t1,042\nForrest\tNingbao\t25\n").Result;
            Assert.Equal(ds.Rows.Count, 2);
        }

        [Fact(DisplayName = "Data_Test_New_Line_Comma_Delimited_With_Single_Header_and_Row_Group")]
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
            Data ds = parser.ParseDataAsync("Name,City,Age\nDarren,OKC,42\nForrest,Ningbao,25\n").Result;
            Assert.Equal(ds.Rows.Count , 2);
        }

        //[Fact(DisplayName = "Data_Test_New_Line_Comma_Delimited_With_Dual_Headers_and_Row_Group")]
        //public void Test_New_Line_Comma_Delimited_With_Dual_Headers_and_Row_Group()
        //{
        //    TextParser parser
        //       = new TextFileParser.TextParser()
        //       .WithLineDelimiter(CommonLineDelimiters.NL)
        //       .WithFieldDelimiter(CommonFieldDelimiters.Comma)
        //       .DefinedByHeaderDefinition(new HeaderDefinition(0, "Person"))
        //       .DefinedByHeaderDefinition(new HeaderDefinition(3, "Address"))
        //       .AddTypeMappping("Name", typeof(string))
        //       .AddTypeMappping("City", typeof(string))
        //       .AddTypeMappping("Age", typeof(Int64))
        //       .AddTypeMappping("Address", typeof(string))
        //       .AddTypeMappping("State", typeof(string))
        //       .AddTypeMappping("Postal", typeof(string));
        //    Data ds = parser.ParseDataAsync("Name,City,Age\nDarren,OKC,42\nForrest,Ningbao,25\nAddress,State,Postal\n1813 Elmhurst,OK,73013\n").Result;
        //    Assert.Equal(ds.Tables.Count, 2);
        //    Assert.NotNull(ds.Tables["Person"]);
        //    Assert.NotNull(ds.Tables["Address"]);
        //    Assert.Equal(ds.Tables["Person"].Rows.Count, 2);
        //    Assert.Equal(ds.Tables["Address"].Rows.Count, 1);
        //}

        [Fact(DisplayName = "Data_Test_Sample_Settlement_File")]
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
            Data ds = parser.ParseDataAsync(new FileInfo("settlement_sample.csv")).Result;
            //test total rows read
            Assert.Equal(ds.Rows.Count, 3792);
            //test value in last slot
            Assert.Equal(4.22m,ds.Rows[3791].Values["other-amount"]);

            var vw = ds.Rows.AsEnumerable();
            var results = from myRow in vw
                          where myRow.Values["settlement-end-date"] != null
                          select myRow;
            Assert.NotNull(results.First().Values["settlement-end-date"]);
        }

        [Fact(DisplayName = "Data_Test_Account_Activity_Summary")]
        public void Test_Account_Activity_Summary()
        {
            TextParser parser
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
               .WithFieldDelimiter(CommonFieldDelimiters.Comma)
               .WithFieldsEnclosedBy(CommonFieldWrappers.DoubleQuote)
               .DefinedByHeaderDefinition(new HeaderDefinition(7, "AccountActivity"))
               .AddTypeMappping("date/time", typeof(string))
               .AddTypeMappping("settlement id", typeof(string))
               .AddTypeMappping("type", typeof(string))
               .AddTypeMappping("order id", typeof(string))
               .AddTypeMappping("sku", typeof(string))
               .AddTypeMappping("description", typeof(string))
               .AddTypeMappping("quantity", typeof(int))
               .AddTypeMappping("marketplace", typeof(string))
               .AddTypeMappping("fulfillment", typeof(string))
               .AddTypeMappping("order city", typeof(string))
               .AddTypeMappping("order state", typeof(string))
               .AddTypeMappping("order postal", typeof(string))
               .AddTypeMappping("product sales", typeof(decimal))
               .AddTypeMappping("shipping credits", typeof(decimal))
               .AddTypeMappping("gift wrap credits", typeof(decimal))
               .AddTypeMappping("promotional rebates", typeof(decimal))
               .AddTypeMappping("sales tax collected", typeof(decimal))
               .AddTypeMappping("selling fees", typeof(decimal))
               .AddTypeMappping("fba fees", typeof(decimal))
               .AddTypeMappping("other transaction fees", typeof(decimal))
               .AddTypeMappping("other", typeof(decimal))
               .AddTypeMappping("total", typeof(decimal))
               ;
            Data ds = parser.ParseDataAsync(new FileInfo("2015Jan1-2015Dec31_CustomTransaction_ryan.csv")).Result;
            //test total rows read
            Assert.Equal(2728,ds.Rows.Count);
            

            /********************EXPENSES************************************************************************************************/
            //validate Adjustments
            var query = (from t in ds.Rows.AsEnumerable()
                         where t.Values["type"].ToString() == "Adjustment" && (t.Values["description"].ToString() == "Balance Adjustment" || t.Values["description"].ToString() == "Buyer Recharge")
                         select Convert.ToDecimal(t.Values["total"])).Sum();
            Assert.Equal(19.37M, query);
            //validate seller fulfulled selling fees
            query = (from t in ds.Rows.AsEnumerable()
                         where t.Values["type"].ToString() == "Order" && t.Values["fulfillment"].ToString() == "Seller"
                         select Convert.ToDecimal(t.Values["selling fees"])).Sum();
            Assert.Equal(-201.41M, query);
            //validate FBA selling fees
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order" && t.Values["fulfillment"].ToString() == "Amazon"
                     select Convert.ToDecimal(t.Values["selling fees"])).Sum();
            Assert.Equal(-7392.13M, query);

            

            //validate FBA TRX fees
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order" && t.Values["fulfillment"].ToString() == "Amazon"
                     select Convert.ToDecimal(t.Values["fba fees"])).Sum();
            Assert.Equal(-8037.60M, query);

            //validate FBA TRX fee refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund" && t.Values["fulfillment"].ToString() == "Amazon"
                     select Convert.ToDecimal(t.Values["fba fees"])).Sum();
            Assert.Equal(52.91M, query);

            //validate other trx fees
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order" 
                     select Convert.ToDecimal(t.Values["other transaction fees"])).Sum();
            Assert.Equal(0.0M, query);

            //validate other TRX fee refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund" 
                     select Convert.ToDecimal(t.Values["other transaction fees"])).Sum();
            Assert.Equal(0.0M, query);

            //validate FBA inv and inbound service fees
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "FBA Inventory Fee"
                     select Convert.ToDecimal(t.Values["total"])).Sum();
            Assert.Equal(-999.91M, query);

            //validate shipping label purchases
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Shipping Services" && t.Values["description"].ToString() =="Shipping Label Purchased through Amazon"
                     select Convert.ToDecimal(t.Values["total"])).Sum();
            Assert.Equal(-237.11M, query);

            //validate service fees
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Service Fee" && t.Values["description"].ToString() == "Subscription Fee"
                     select Convert.ToDecimal(t.Values["total"])).Sum();
            Assert.Equal(-479.88M, query);

            //validate refund administration fees??
            //query = (from t in table.AsEnumerable()
            //         where t["type"].ToString() == "Service Fee" && t["description"].ToString() == "Subscription Fee"
            //         select Convert.ToDecimal(t["total"])).Sum();
            //Assert.Equal(-479.88M, query);

            //validate selling refunds ??
            //query = (from t in table.AsEnumerable()
            //         where t["type"].ToString() == "Refund" && t["fulfillment"].ToString() == "Amazon"
            //         select Convert.ToDecimal(t["selling fees"])).Sum();
            //Assert.Equal(184.42M, query);

            //validate selling fee refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund" 
                     select Convert.ToDecimal(t.Values["selling fees"])).Sum();
            //NOTE this is the  selling fee refunds less the refund admin fees...report csv reflects the refund minus the admin fee
            //the SC custom summary pdf has 184.42 in refunds and 32.87 in refund admin fees netting 151.55
            Assert.Equal(151.55M, query);

            /********************INCOME************************************************************************************************/

            //validate product sales NON-FBA
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order" && t.Values["fulfillment"].ToString() == "Seller"
                     select Convert.ToDecimal(t.Values["product sales"])).Sum();
            Assert.Equal(1019.73M, query);

            //validate product sale NON-FBA refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund" && t.Values["fulfillment"].ToString() == "Seller"
                     select Convert.ToDecimal(t.Values["product sales"])).Sum();
            Assert.Equal(0.0M, query);

            //validate product sales FBA 
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order" && t.Values["fulfillment"].ToString() == "Amazon"
                     select Convert.ToDecimal(t.Values["product sales"])).Sum();
            Assert.Equal(45204.97M, query);

            //validate product sale FBA refund
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund" && t.Values["fulfillment"].ToString() == "Amazon"
                     select Convert.ToDecimal(t.Values["product sales"])).Sum();
            var queryOtherRefund = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund" && t.Values["fulfillment"].ToString() == "Amazon"
                     select Convert.ToDecimal(t.Values["other"])).Sum();
            Assert.Equal(-1102.66M, query+queryOtherRefund);

            //validate FBA inventory credit
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Adjustment" && (t.Values["description"].ToString() == "FBA Inventory Reimbursement - Customer Return" 
                     || t.Values["description"].ToString() == "FBA Inventory Reimbursement - Damaged:Warehouse" 
                     || t.Values["description"].ToString() == "FBA Inventory Reimbursement - Lost:Warehouse"
                     || t.Values["description"].ToString() == "FBA Inventory Reimbursement - Customer Service Issue"
                     || t.Values["description"].ToString() == "FBA Inventory Reimbursement - Damaged:Inbound"
                     || t.Values["description"].ToString() == "FBA Inventory Reimbursement - Lost:Inbound")
                     select Convert.ToDecimal(t.Values["total"])).Sum();
            Assert.Equal(3350.63M, query);

            //validate shipping credits
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order"
                     select Convert.ToDecimal(t.Values["shipping credits"])).Sum();
            Assert.Equal(2777.78M, query);

            //validate shipping credit Refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund"
                     select Convert.ToDecimal(t.Values["shipping credits"])).Sum();
            Assert.Equal(-59.84M, query);

            //validate gift wrap credits
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order"
                     select Convert.ToDecimal(t.Values["gift wrap credits"])).Sum();
            Assert.Equal(63.33M, query);

            //validate gift wrap credit Refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund"
                     select Convert.ToDecimal(t.Values["gift wrap credits"])).Sum();
            Assert.Equal(-3.49M, query);

            //validate promotional rebates
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order"
                     select Convert.ToDecimal(t.Values["promotional rebates"])).Sum();
            Assert.Equal(-651.30M, query);

            //validate promotional rebate Refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund"
                     select Convert.ToDecimal(t.Values["promotional rebates"])).Sum();
            Assert.Equal(10.42M, query);

            /********************TRANSFERS************************************************************************************************/
            //validate transfers to bank account
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Transfer" 
                     select Convert.ToDecimal(t.Values["total"])).Sum();
            Assert.Equal(-33447.99M, query);

            /********************TAXES************************************************************************************************/
            //validate Sales tax collected
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Order"
                     select Convert.ToDecimal(t.Values["sales tax collected"])).Sum();
            Assert.Equal(0.0M, query);

            //validate Sales tax refunds
            query = (from t in ds.Rows.AsEnumerable()
                     where t.Values["type"].ToString() == "Refund"
                     select Convert.ToDecimal(t.Values["sales tax collected"])).Sum();
            Assert.Equal(0.0M, query);
        }

        [Fact(DisplayName = "Data_Test_GET_MERCHANT_LISTINGS_DATA_12508")]
        public void Test_GET_MERCHANT_LISTINGS_DATA_12508()
        {
            TextParser parser
               = new TextFileParser.TextParser()
                       .WithLineDelimiter(CommonLineDelimiters.NL)
                       .WithFieldDelimiter(CommonFieldDelimiters.Tab)
                       .DefinedByHeaderDefinition(new HeaderDefinition(0, "_GET_MERCHANT_LISTINGS_DATA_"))
                       .AddTypeMappping("item-name", typeof(string))
                       .AddTypeMappping("item-description", typeof(string))
                       .AddTypeMappping("listing-id", typeof(string))
                       .AddTypeMappping("seller-sku", typeof(string))
                       .AddTypeMappping("price", typeof(decimal))
                       .AddTypeMappping("quantity", typeof(int))
                       .AddTypeMappping("open-date", typeof(string))
                       .AddTypeMappping("image-url", typeof(string))
                       .AddTypeMappping("open-date", typeof(string))
                       .AddTypeMappping("item-is-marketplace", typeof(string))
                       .AddTypeMappping("product-id-type", typeof(string))
                       .AddTypeMappping("zshop-shipping-fee", typeof(string))
                       .AddTypeMappping("item-note", typeof(string))
                       .AddTypeMappping("item-condition", typeof(string))
                       .AddTypeMappping("zshop-category1", typeof(string))
                       .AddTypeMappping("zshop-browse-path", typeof(string))
                       .AddTypeMappping("zshop-storefront-feature", typeof(string))
                       .AddTypeMappping("asin1", typeof(string))
                       .AddTypeMappping("asin2", typeof(string))
                       .AddTypeMappping("asin3", typeof(string))
                       .AddTypeMappping("will-ship-internationally", typeof(string))
                       .AddTypeMappping("expedited-shipping", typeof(string))
                       .AddTypeMappping("zshop-boldface", typeof(string))
                       .AddTypeMappping("product-id", typeof(string))
                       .AddTypeMappping("bid-for-featured-placement", typeof(string))
                       .AddTypeMappping("add-delete", typeof(string))
                       .AddTypeMappping("pending-quantity", typeof(string))
                       .AddTypeMappping("fulfillment-channel", typeof(string))
               ;
            Data ds = parser.ParseDataAsync(new FileInfo(@"D:\tmp\textparser\3412571000017123_GET_MERCHANT_LISTINGS_DATA_.csv")).Result;
            
            //test total rows read
            Assert.Equal(90306, ds.Rows.Count); //parse errors
            //test value in last slot
            Assert.Equal("Afterlife: The Complete Guide [Nov 10, 1994] Goldman, Emily and Neiman, Carol", ds.Rows[90305].Values["item-name"]);

            //Assert.NotNull(ds.Tables["ParseErrors"]);


            TextParser parser2
               = new TextFileParser.TextParser()
               .WithLineDelimiter(CommonLineDelimiters.NL)
                       .WithFieldDelimiter(CommonFieldDelimiters.Tab)
                       .DefinedByHeaderDefinition(new HeaderDefinition(0, "_GET_MERCHANT_LISTINGS_DATA_LITE_"))
                       .AddTypeMappping("seller-sku", typeof(string))
                       .AddTypeMappping("quantity", typeof(int))
                       .AddTypeMappping("price", typeof(decimal))
                       .AddTypeMappping("product-id", typeof(string))
               ;
            Data ds2 = parser2.ParseDataAsync(new FileInfo(@"D:\tmp\textparser\3411664776017123_GET_MERCHANT_LISTINGS_DATA_LITE_.csv")).Result;

            TextParser parser3
               = new TextFileParser.TextParser()
                       .WithLineDelimiter(CommonLineDelimiters.CRNL)
                       .WithFieldDelimiter(CommonFieldDelimiters.Tab)
                       .DefinedByHeaderDefinition(new HeaderDefinition(0, "_GET_FBA_MYI_UNSUPPRESSED_INVENTORY_DATA_"))
                       .AddTypeMappping("sku", typeof(string))
                       .AddTypeMappping("fnsku", typeof(string))
                       .AddTypeMappping("asin", typeof(string))
                       .AddTypeMappping("product-name", typeof(string))
                       .AddTypeMappping("condition", typeof(string))
                       .AddTypeMappping("your-price", typeof(decimal))
                       .AddTypeMappping("mfn-listing-exists", typeof(string))
                       .AddTypeMappping("mfn-fulfillable-quantity", typeof(string))
                       .AddTypeMappping("afn-listing-exists", typeof(string))
                       .AddTypeMappping("afn-warehouse-quantity", typeof(int))
                       .AddTypeMappping("afn-fulfillable-quantity", typeof(int))
                       .AddTypeMappping("afn-unsellable-quantity", typeof(int))
                       .AddTypeMappping("afn-reserved-quantity", typeof(int))
                       .AddTypeMappping("afn-total-quantity", typeof(int))
                       .AddTypeMappping("per-unit-volume", typeof(string))
                       .AddTypeMappping("afn-inbound-working-quantity", typeof(string))
                       .AddTypeMappping("afn-inbound-shipped-quantity", typeof(string))
                       .AddTypeMappping("afn-inbound-receiving-quantity", typeof(string))
               ;
            Data ds3 = parser3.ParseDataAsync(new FileInfo(@"D:\tmp\textparser\3410992042017123_GET_FBA_MYI_UNSUPPRESSED_INVENTORY_DATA_.csv")).Result;

            //not test join situation
            var result = from merchantListings in ds.Rows.AsEnumerable()
                         join merchantListingsLite in ds2.Rows.AsEnumerable()
                         on new { ASIN = merchantListings.Values["product-id"], MSKU = merchantListings.Values["seller-sku"] } equals new { ASIN = merchantListingsLite.Values["product-id"], MSKU = merchantListingsLite.Values["seller-sku"] } into leftjoin
                         from r in leftjoin.DefaultIfEmpty()
                         select new
                         {
                             MSKU = merchantListings.Values["seller-sku"],
                             Title = merchantListings.Values["item-name"],
                             Description = merchantListings.Values["item-description"],
                             Quantity = merchantListings.Values["quantity"],
                             Price = merchantListings.Values["price"],
                             LITEQuantity = r == null ? 0 : r.Values["quantity"],
                             LITEPrice = r == null ? 0 : r.Values["price"]
                         };
            Assert.Equal(90306,result.Count());
            Assert.Equal(2274689.30m, result.Sum(t => (decimal)t.Price));
        }
    }
}
