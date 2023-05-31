using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;

namespace CIMB_trial
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStr"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("./Login.aspx");
            }
        }

        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Fileupload.HasFile)
                {
                    Fileupload.SaveAs("D:\\testing\\" + Fileupload.FileName);
                }
                else
                {
                    lbMsg.Text = "Please Choose Excel File first";
                }
            }
            catch
            {

            }
            
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session["username"] = null;
            Response.Redirect("./Login.aspx");
        }

        protected void BtnImportExcel_Click (object sender, EventArgs e)
        {
            try
            {
                string DirUpload = "";
                if (Fileupload.HasFile)
                {
                    DirUpload = "D:\\testing\\" + Fileupload.FileName;
                    Fileupload.SaveAs("D:\\testing\\" + Fileupload.FileName);
                    ImportDataFromExcel(DirUpload);
                }
                else
                {
                    lbMsg.Text = "Please Choose Excel File first";
                }
            }
            catch
            {

            }           
            
        }

        public void ImportDataFromExcel(string excelFilePath)
        {
            string tblSQL = "tbl_details";
            string ExcelQuery = "select name,hobby from [Sheet1$]";
            try
            {
                //create our connection strings
                string excelconnectionstring = @"provider=Microsoft.ACE.OLEDB.12.0;data source=" + excelFilePath +
                ";extended properties=" + "\"Excel 12.0;HDR=YES;\"";
                
                OleDbConnection oledbconn = new OleDbConnection(excelconnectionstring);
                OleDbCommand oledbcmd = new OleDbCommand(ExcelQuery, oledbconn);
                oledbconn.Open();
                OleDbDataReader dr = oledbcmd.ExecuteReader();
                SqlBulkCopy bulkcopy = new SqlBulkCopy(connectionString);
                bulkcopy.DestinationTableName = tblSQL;
                while (dr.Read())
                {
                    bulkcopy.WriteToServer(dr);
                }
                dr.Close();
                oledbconn.Close();
                lbMsg.Text = "File imported successfully.";
                lbMsg.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                //handle exception
            }
        }
    }
}