 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.OleDb;

namespace ExcelUpload.Models
    {
        public static class Utility
        {
            public static DataTable ConvertCSVtoDataTable(string strFilePath)
            {
                DataTable dt = new DataTable();
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        if (rows.Length > 1)
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                dr[i] = rows[i].Trim();
                            }
                            dt.Rows.Add(dr);
                        }
                    }

                }


                return dt;
            }

            public static DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
            {
                OleDbConnection oledbConn = new OleDbConnection(connString);
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                try
                {

                    oledbConn.Open();
                    using (DataTable Sheets = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null))
                    {

                        for (int i = 0; i < Sheets.Rows.Count; i++)
                        {
                            string worksheets = Sheets.Rows[i]["TABLE_NAME"].ToString();
                            OleDbCommand cmd = new OleDbCommand(String.Format("SELECT * FROM [{0}]", worksheets), oledbConn);
                            OleDbDataAdapter oleda = new OleDbDataAdapter();
                            oleda.SelectCommand = cmd;

                            oleda.Fill(ds);
                        }

                        dt = ds.Tables[0];
                    }

                }
                catch (Exception ex)
                {
                WebLog.Log(ex.Message);
               }
                finally
                {

                    oledbConn.Close();
                }

                return dt;

            }

        public static string After(this string @this, string a)
        {
            try
            {
                var posA = @this.LastIndexOf(a, StringComparison.Ordinal);
                if (posA == -1)
                {
                    return "";
                }
                var adjustedPosA = posA + a.Length;
                return adjustedPosA >= @this.Length ? "" : @this.Substring(adjustedPosA);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public static string Before(this string @this, string a)
        {
            try
            {
                var posA = @this.IndexOf(a, StringComparison.Ordinal);
                return posA == -1 ? "" : @this.Substring(0, posA);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
    }
