using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1.Tools
{
   public static class Tool
    {
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int GetDataTable(string sql)
        {
            try
            {
                int result = 0;
                string strConn = "server=DESKTOP-HRSVT7F;uid=sa;pwd=123;database=Demo";
                SqlConnection conn = new SqlConnection(strConn);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();
                conn.Close();
                if (i > 0)
                {
                    return result = i;
                }
                else return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重复插入数据{ex}", "提示", MessageBoxButtons.OKCancel);
                Application.Exit();
                throw ex;               
            }
            
        }
    }
}
