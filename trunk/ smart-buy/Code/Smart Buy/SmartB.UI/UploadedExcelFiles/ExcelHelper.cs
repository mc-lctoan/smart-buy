using ExcelUtilities;
using SmartB.UI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace SmartB.UI.UploadedExcelFiles
{
    public class ExcelHelper
    {
        public List<SellProductModel> ReadData(string path, out string errorName, out string errorMarket, out string errorPrice)
        {
            OleDbConnection oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
          path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
            errorName = "";
            errorMarket = "";
            errorPrice = "";
            try
            {

                oledbConn.Open();

                OleDbCommand cmd = new OleDbCommand();

                //CHua' data tren RAM
                DataSet ds = new DataSet();


                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT [TenSanPham],[TenCho],[Gia] FROM [Sheet1$]";

                OleDbDataAdapter oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds, "SellProduct");

                DataTableCollection tables = ds.Tables;
                List<SellProductModel> sellProductCollection = new List<SellProductModel>();

                for (int i = 0; i < tables.Count; i++)
                {
                    DataTable table = tables[i];
                    foreach (DataRow row in table.Rows)
                    {
                        SellProductModel sellProduct = new SellProductModel();
                        sellProduct.Name = row.ItemArray[0].ToString();
                        if (sellProduct.Name.Length < 5 || sellProduct.Name.Length > 20)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên sản phẩm phải từ 5 đến 20 ký tự");
                            errorName = invalidNumberException.Message;
                        }
                        sellProduct.MarketName = row.ItemArray[1].ToString();
                        if (sellProduct.MarketName.Length < 5 || sellProduct.MarketName.Length > 20)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên chợ phải từ 5 đến 20 ký tự");
                            errorMarket = invalidNumberException.Message;
                        }
                        int price = 0;

                        try
                        {
                            price = Int32.Parse(row.ItemArray[2].ToString());
                            if (price < 1 || price > 10000)
                            {
                                InvalidNumberException invalidNumberException = new InvalidNumberException("Giá phải từ 1 đến 10000");
                                errorPrice = invalidNumberException.Message;
                            }
                        }
                        catch (ArgumentNullException argumentNullException)
                        {
                            throw argumentNullException;
                        }
                        catch (FormatException formatException)
                        {
                            throw formatException;
                        }
                        catch (OverflowException overflowException)
                        {
                            throw overflowException;
                        }
                        finally
                        {
                            sellProduct.Price = price;
                            sellProductCollection.Add(sellProduct);
                        }
                    }
                    return sellProductCollection;
                }
            }

            catch (System.InvalidOperationException exception)
            {

                throw exception;
            }

            catch (System.Data.OleDb.OleDbException exception2)
            {

                throw exception2;
            }
            return null;
        }
    }
}