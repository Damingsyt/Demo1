using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1.Data;
using WindowsFormsApplication1.Tools;

namespace Demo1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void btn_Insert_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件(*.xls,*.xlsx)|*.xls";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txt.Text = ofd.FileName;
            }           
        }
        private void Update_Click(object sender, EventArgs e)
        {
            string ExcelDate = null;
            DataTable tb = new DataTable();
            ExcelDate = txt.Text.Split('.')[0].Split('_')[1];                 //获取Excel的日期
            DataTable table = NPOIHelper.ExcelToDataTable(txt.Text, true);
            List<Instrument> Instr = new List<Instrument>();
            List<Instrument> InvalidInstr = new List<Instrument>(); 
            List<string> InstrunmentId = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                Instrument inst = new Instrument();
                if (row["标的代号"].ToString().Split('.')[1] == "CZC")
                {
                    inst.Symbol = row["标的代号"].ToString().Split('.')[0].ToUpper();
                }
                else
                {
                    inst.Symbol = row["标的代号"].ToString().Split('.')[0].ToLower();    //循环获取指定列的行数据，以.分隔获取前半段的标号
                }
                inst.Name = row["标的名称"].ToString();
                inst.Vol = Convert.ToDouble(row["手动修改Vol"]);
                Instr.Add(inst);
            }
            int sum = 0;
            using (var entity = new DemoEntities())
            {
                foreach (var item in Instr)
                {
                    
                    // sql = $"select T.InstrumentId from INSTRUMENT T WHERE T.symbol =('{item.Symbol}') and t.InstrumentName = ('{item.Name}') ; ";
                    var INSTRUMENT = entity.INSTRUMENT.FirstOrDefault(x => x.Symbol == item.Symbol && x.InstrumentName == item.Name);
                    var Risk = entity.RISK_FACTOR.Where(x => x.Key == "Vol").OrderByDescending(x => x.VolDate).ToList();
                    if (INSTRUMENT == null)
                    {
                        InvalidInstr.Add(item);
                        continue;
                    }
                    var value = Risk.FirstOrDefault(x => x.InstrumentId == INSTRUMENT.InstrumentId);
                    double num = 0;
                    if (item.Vol  != Math.Round( value.Value*100,2))
                    {
                        num = item.Vol * 0.01;
                        RISK_FACTOR RF = new RISK_FACTOR() { InstrumentId = (int)INSTRUMENT.InstrumentId ,Key = "Vol", ProfileId = 1 , Value = num , VolDate = DateTime.ParseExact(ExcelDate, "yyyyMMdd", null) };
                        entity.RISK_FACTOR.Add(RF);
                        entity.SaveChanges();
                        sum++;
                    }
                }
                //无效的数据形成一个表
                tb.Columns.Add("标的代号");
                tb.Columns.Add("标的名称");
              //  DataRow row = tb.NewRow();
                foreach (var item in InvalidInstr)
                {

                    DataRow row = tb.NewRow();
                    row["标的代号"] = item.Symbol;
                    row["标的名称"] = item.Name;
                    tb.Rows.Add(row);
                }
                string err = "";
                int i = 0;
                foreach (var item in InvalidInstr)
                {
                    // b[i] = item.Symbol;

                   err= string.Join(", ", err, item.Symbol,item.Name);
                    i++;
                }

                if (sum>0)
                {
                    MessageBox.Show($"插入:{sum}条数据成功,有如下数据{err}是不存在于表INSTRUMENT", "提示", MessageBoxButtons.OKCancel);
                }
                else
                {
                    MessageBox.Show($"无符合数据插入,无效的数据为：{err}", "提示", MessageBoxButtons.OKCancel);
                }             
            }
        }
    }
}
