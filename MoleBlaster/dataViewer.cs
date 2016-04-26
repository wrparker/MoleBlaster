using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MoleBlaster
{
    public partial class dataViewer : Form
    {
        public dataViewer(List<fragmentationRule> rules)
        {
            InitializeComponent();
            List<string[]> datalist = new List<string[]>();

            datalist.Add(new string[]{ "Name", "BondId", "Atom Id1", "AtomId2", "MassShift1", "MassShift2", "BondId2", "Atom Id3", "AtomId4", "MassShift3", "MassShift4"});
            foreach(fragmentationRule rule in rules)
            {
                if (rule._bondId2 != -1) { 
                datalist.Add(new string[] { rule.fragmentName.ToString(), rule._bondId.ToString(), rule._atomId1.ToString(), rule._atomId2.ToString(), rule._MassShift1.ToString(), rule._MassShift2.ToString(),rule._bondId2.ToString(),rule._atomId3.ToString(), rule._atomId4.ToString(), rule._MassShift3.ToString(), rule._MassShift4.ToString() });
                }
                else
                {
                    datalist.Add(new string[] { rule.fragmentName.ToString(), rule._bondId.ToString(), rule._atomId1.ToString(), rule._atomId2.ToString(), rule._MassShift1.ToString(), rule._MassShift2.ToString()});
                }
            }
            DataTable table = ConvertListToDataTable(datalist);
            dataGridView1.DataSource = table;
        }
        public dataViewer(List<Fragment> frags, int RuleOption)
        {
            InitializeComponent();
            List<string[]> datalist = new List<string[]>();

            //WIth Rules
            if (RuleOption == 1)
            {
                datalist.Add(new string[] { "Name", "Mass", "BondId", "Atom Id1", "AtomId2", "MassShift1", "MassShift2", "BondId2", "Atom Id3", "AtomId4", "MassShift3", "MassShift4" });
                foreach (Fragment curFrag in frags)
                {
                    if (curFrag._bondId2 != -1)
                    {
                        datalist.Add(new string[] { curFrag.fragmentName.ToString(), curFrag.mass.ToString(), curFrag._bondId.ToString(), curFrag._atomId1.ToString(), curFrag._atomId2.ToString(), curFrag._MassShift1.ToString(), curFrag._MassShift2.ToString(), curFrag._bondId2.ToString(), curFrag._atomId3.ToString(), curFrag._atomId4.ToString(), curFrag._MassShift3.ToString(), curFrag._MassShift4.ToString() });
                    }
                    else
                    {
                        datalist.Add(new string[] { curFrag.fragmentName.ToString(), curFrag.mass.ToString(), curFrag._bondId.ToString(), curFrag._atomId1.ToString(), curFrag._atomId2.ToString(), curFrag._MassShift1.ToString(), curFrag._MassShift2.ToString() });
                    }
                }
            }
            //Naive
            else if(RuleOption == 2)
            {
                datalist.Add(new string[] { "BondId", "Mass", "Atom Id1", "AtomId2" });
                foreach (Fragment curFrag in frags)
                {
                    datalist.Add(new string[] { curFrag._bondId.ToString(), curFrag.mass.ToString(), curFrag._atomId1.ToString(), curFrag._atomId2.ToString() });
                }
            }
            else
            {
                MessageBox.Show("Program Error with DataViewer.  Must choose RuleOption of 1 or 2.  1 = With Rules, 2 = Naive.");
            }
            DataTable table = ConvertListToDataTable(datalist);
            dataGridView1.DataSource = table;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        static DataTable ConvertListToDataTable(List<string[]> list)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }

            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }

            return table;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void excelDataCopy()
        {
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
            dataGridView1.ClearSelection();
        }

        //Copy to Excel
        private void button2_Click(object sender, EventArgs e)
        {
            excelDataCopy();
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlexcel.Visible = true;
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
            CR.Select();
            xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result != DialogResult.OK) { 
                MessageBox.Show("Problem Saving File...");
            }
            else
            {
                string fileName = saveFileDialog1.FileName;
                var sb = new StringBuilder();

                var headers = dataGridView1.Columns.Cast<DataGridViewColumn>();
                sb.AppendLine(string.Join(",", headers.Select(column => "\"" + column.HeaderText + "\"").ToArray()));

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    var cells = row.Cells.Cast<DataGridViewCell>();
                    sb.AppendLine(string.Join(",", cells.Select(cell => "" + cell.Value + "").ToArray()));
                }
                File.WriteAllText(saveFileDialog1.FileName, sb.ToString());
                MessageBox.Show("File has been saved!");
            }
        }
    }
}
