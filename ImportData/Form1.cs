using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;

namespace ImportData
{
    public partial class Form1 : Form
    {
    	//12345
    	//23456
        //Data Source=10.1.0.68,1304;Initial Catalog=emrxk;Persist Security Info=True;User ID=newtouch_login;Password=newtouch_login_ps;Connect Timeout=900000

        //Data Source=10.1.0.109\newtouch;Initial Catalog=SpecialDiseaseDB;Persist Security Info=True;User ID=newtouch;Password=!newtouch123;Connect Timeout=900000

        //Data Source=10.1.0.158\tech;Initial Catalog=EMRXKCS;Persist Security Info=True;User ID=sa;Password=winning;Connect Timeout=900000
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection2 = new SqlConnection(textBox2.Text))
                {
                    connection2.Open();
                    using (SqlConnection connection1 = new SqlConnection(textBox1.Text))
                    {
                        SqlDataReader myReader = null;
                        connection1.Open();
                        //SqlCommand cm = new SqlCommand(" select * from BL_QTBLJLK where cjsj>='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and cjsj<='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 23:59:59'");
                        SqlCommand cm = new SqlCommand(" select * from BL_QTBLJLK where syxh=248767");
                        cm.Connection = connection1;
                        myReader = cm.ExecuteReader();

                        SqlCommand cmdInsert = null;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(" insert into T_COM_Record values (@syxh,@bldm,@blmc,@blms,@blnr,@cjys,@shys)");
                        cmdInsert = new SqlCommand(sb.ToString());
                        cmdInsert.Connection = connection2;
                        while (myReader.Read())
                        {
                            cmdInsert.CommandText = sb.ToString();
                            cmdInsert.Parameters.Clear();
                            cmdInsert.Parameters.Add("syxh", SqlDbType.Int);
                            cmdInsert.Parameters.Add("bldm", SqlDbType.VarChar);
                            cmdInsert.Parameters.Add("blmc", SqlDbType.VarChar);
                            cmdInsert.Parameters.Add("blms", SqlDbType.VarChar);
                            cmdInsert.Parameters.Add("blnr", SqlDbType.VarChar);
                            cmdInsert.Parameters.Add("cjys", SqlDbType.VarChar);
                            cmdInsert.Parameters.Add("shys", SqlDbType.VarChar);

                            cmdInsert.Parameters["syxh"].Value = int.Parse(myReader["syxh"].ToString());
                            cmdInsert.Parameters["bldm"].Value = myReader["bldm"].ToString();
                            cmdInsert.Parameters["blmc"].Value = myReader["blmc"].ToString();
                            cmdInsert.Parameters["blms"].Value = myReader["blms"].ToString();
                            cmdInsert.Parameters["blnr"].Value = UnzipEmrXml(myReader["blnr"].ToString());
                            cmdInsert.Parameters["cjys"].Value = myReader["cjys"].ToString();
                            cmdInsert.Parameters["shys"].Value = myReader["shys"].ToString();
                            try
                            {
                                cmdInsert.ExecuteNonQuery();
                            }catch(Exception eee){
                                richTextBox1.Text += myReader["qtbljlxh"].ToString() + "导入出错\n";
                            }
                        }
                    }
                }
                richTextBox1.Text += "ok \n";
            }catch(Exception ee){
                richTextBox1.Text += ee.Message;
            }
        }

        private string UnzipEmrXml(string emrContent)
        {
            try
            {
                byte[] rbuff = Convert.FromBase64String(emrContent);
                MemoryStream ms = new MemoryStream(rbuff);
                DeflateStream dfs = new DeflateStream(ms, CompressionMode.Decompress, true);
                StreamReader sr = new StreamReader(dfs, Encoding.UTF8);
                string sXml = sr.ReadToEnd();
                sr.Close();
                dfs.Close();
                return sXml;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e);
                return emrContent;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
