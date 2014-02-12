using SmartB.UI.UploadedExcelFiles;
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
        public List<SellProductModel> ReadData(string path, out string errorName, out string errorMarket, out string errorPrice, out int errorCount)
        {
            OleDbConnection oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
          path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
            errorName = "";
            errorMarket = "";
            errorPrice = "";
            errorCount = 0;
            try
            {

                oledbConn.Open();

                OleDbCommand cmd = new OleDbCommand();

                //CHua' data tren RAM
                DataSet ds = new DataSet();


                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
              // cmd.CommandText = "SELECT [Tên Sản Phẩm],[Tên Chợ],[Giá] FROM [Sheet1$]";
                cmd.CommandText = "SELECT * FROM [sheet1$B2:D2000]";

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
                        if (sellProduct.Name.Length < 5 || sellProduct.Name.Length > 100)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên sản phẩm phải từ 5 đến 100 ký tự");
                            errorName = invalidNumberException.Message;
                            errorCount++;
                        }
                        sellProduct.MarketName = row.ItemArray[1].ToString();
                        if (sellProduct.MarketName.Length < 5 || sellProduct.MarketName.Length > 20)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên chợ phải từ 5 đến 20 ký tự");
                            errorMarket = invalidNumberException.Message;
                            errorCount++;
                        }
                        int price = 0;

                        try
                        {
                            //if (row.ItemArray[2] != @"^[0-9]+$") 
                            //{
                            //    InvalidNumberException invalidNumberException = new InvalidNumberException("Giá phải từ 1 đến 10000");
                            //    errorPrice = invalidNumberException.Message;
                            //    price = row.ItemArray[2].ToString().FirstOrDefault();
                            //}else 
                            price = Int32.Parse(row.ItemArray[2].ToString());
                            if (price < 1 || price > 10000 )
                            {
                                InvalidNumberException invalidNumberException = new InvalidNumberException("Giá phải từ 1 đến 10000");
                                errorPrice = invalidNumberException.Message;
                                errorCount++;
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