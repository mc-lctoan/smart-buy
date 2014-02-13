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
                cmd.CommandText = "SELECT * FROM [sheet1$A2:D2000]";

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
                      //  SellProductModel sellProductError = new SellProductModel();
                        sellProduct.RowNumber = Int32.Parse(row.ItemArray[0].ToString()) +2;
                        sellProduct.Name = row.ItemArray[1].ToString();
                        sellProduct.MarketName = row.ItemArray[2].ToString();


                        if (sellProduct.Name.Length < 5 || sellProduct.Name.Length > 100)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên sản phẩm phải từ 5 đến 100 ký tự");
                            errorName = invalidNumberException.Message;
                            errorCount++;
                        }


                        if (sellProduct.MarketName.Length < 5 || sellProduct.MarketName.Length > 20)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên chợ phải từ 5 đến 20 ký tự");
                            errorMarket = invalidNumberException.Message;
                            errorCount++;
                        }
                        try
                        {
                            var price = 0;
                            Int32.TryParse(row.ItemArray[3].ToString(), out price);
                            sellProduct.Price = price;
                            if (sellProduct.Price < 1 || sellProduct.Price > 10000)
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
        //List error
        public List<SellProductModel> ReadDataCorrect(string path)
        {
            OleDbConnection oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
          path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
           
            try
            {
                
                oledbConn.Open();

                OleDbCommand cmd = new OleDbCommand();

                //CHua' data tren RAM
                DataSet ds = new DataSet();


                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
                // cmd.CommandText = "SELECT [Tên Sản Phẩm],[Tên Chợ],[Giá] FROM [Sheet1$]";
                cmd.CommandText = "SELECT * FROM [sheet1$A2:D2000]";

                OleDbDataAdapter oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds, "SellProduct");

                DataTableCollection tables = ds.Tables;
                List<SellProductModel> sellProductCorrectCollection = new List<SellProductModel>();
                for (int i = 0; i < tables.Count; i++)
                {
                    
                    DataTable table = tables[i];
                    foreach (DataRow row in table.Rows)
                    {
                        bool error = false;
                        SellProductModel sellProductCorrect = new SellProductModel();
                        sellProductCorrect.RowNumber = Int32.Parse(row.ItemArray[0].ToString()) +2;
                        sellProductCorrect.Name = row.ItemArray[1].ToString();
                        sellProductCorrect.MarketName = row.ItemArray[2].ToString();

                        if (sellProductCorrect.Name.Length < 5 || sellProductCorrect.Name.Length > 100)
                        {
                            error = true;
                        }


                        if (sellProductCorrect.MarketName.Length < 5 || sellProductCorrect.MarketName.Length > 20)
                        {
                            error = true;
                        }
                        try
                        {
                            int price = 0;
                            Int32.TryParse(row.ItemArray[3].ToString(), out price);
                            sellProductCorrect.Price = price;

                            if (sellProductCorrect.Price < 1 || sellProductCorrect.Price > 10000)
                            {
                                error = true;
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
                            if (error == false)
                            {
                                sellProductCorrectCollection.Add(sellProductCorrect);
                            }
                        }
                    }
                    return sellProductCorrectCollection;
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
        public List<SellProductModel> ReadDataError(string path, out string errorName, out string errorMarket, out string errorPrice, out int errorCount)
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
                cmd.CommandText = "SELECT * FROM [sheet1$A2:D2000]";

                OleDbDataAdapter oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds, "SellProduct");

                DataTableCollection tables = ds.Tables;
                errorName = "";
                errorMarket = "";
                errorPrice = "";
                List<SellProductModel> sellProductErrorCollection = new List<SellProductModel>();
                for (int i = 0; i < tables.Count; i++)
                {
                   

                    DataTable table = tables[i];
                    foreach (DataRow row in table.Rows)
                    {
                        bool error = false;
                        SellProductModel sellProductError = new SellProductModel();
                        sellProductError.RowNumber = Int32.Parse(row.ItemArray[0].ToString()) +2;
                        sellProductError.Name = row.ItemArray[1].ToString();
                        sellProductError.MarketName = row.ItemArray[2].ToString();

                        if (sellProductError.Name.Length < 5 || sellProductError.Name.Length > 100)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên sản phẩm phải từ 5 đến 100 ký tự");
                            errorName = invalidNumberException.Message;
                            error = true;
                            errorCount++;
                        }


                        if (sellProductError.MarketName.Length < 5 || sellProductError.MarketName.Length > 20)
                        {
                            InvalidNumberException invalidNumberException = new InvalidNumberException("Tên chợ phải từ 5 đến 20 ký tự");
                            errorMarket = invalidNumberException.Message;
                            error = true;
                            errorCount++;
                        }
                        try
                        {
                            int price = 0;
                            Int32.TryParse(row.ItemArray[3].ToString(), out price);
                            sellProductError.Price = price;
                            if (sellProductError.Price < 1 || sellProductError.Price > 10000)
                            {
                                InvalidNumberException invalidNumberException = new InvalidNumberException("Giá phải từ 1 đến 10000");
                                errorPrice = invalidNumberException.Message;
                                error = true;
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
                            if (error)
                            {
                                sellProductErrorCollection.Add(sellProductError);
                            }
                        }
                    }
                    return sellProductErrorCollection;
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