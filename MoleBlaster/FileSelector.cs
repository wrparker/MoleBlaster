using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.ggasoftware.indigo;
using System.IO;

namespace MoleBlaster
{
    public partial class FileSelector : Form
    {

        public FileSelector()
        {
            InitializeComponent();
   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                Console.Out.WriteLine(Path.GetExtension(openFileDialog1.FileName));
                if (String.Compare(Path.GetExtension(openFileDialog1.FileName), ".CDX",true) == 0)
                {
                    MessageBox.Show("WARNING:  R-Groups are currently only supported by CML files.  If this molecule has R-Groups, use CML extension type.");
                }

                this.textBox1.Text = openFileDialog1.FileName;
            }
            
        }
        public void button2_Click(object sender, EventArgs e)
        {
            Indigo indigo = new Indigo();
            List<IndigoObject> chemStrucutres = new List<IndigoObject>();

            //Mol Files
            if (String.Equals(Path.GetExtension(textBox1.Text), ".cml", StringComparison.OrdinalIgnoreCase))
            {
                foreach (IndigoObject item in indigo.iterateCMLFile(this.textBox1.Text))
                {
                    chemStrucutres.Add(item);
                }
            }
            //CDX Files
            if (String.Equals(Path.GetExtension(textBox1.Text), ".cdx", StringComparison.OrdinalIgnoreCase))
            {
                foreach (IndigoObject item in indigo.iterateCDXFile(this.textBox1.Text))
                {
                    chemStrucutres.Add(item);
                }

            }
            //If > 1 structure will need to loop.
            if (chemStrucutres.Count > 1)
            {
                MultiStructureSelector f = new MultiStructureSelector(chemStrucutres, indigo);
                f.ShowDialog(Application.OpenForms["MainMenu"]);
                this.Close();
            }
            else if (chemStrucutres.Count == 1)
            {
                TemplateBuilder f = new TemplateBuilder(chemStrucutres, indigo);
                this.Close();
                f.ShowDialog(Application.OpenForms["MainMenu"]);
            }
            else
            {
                //TODO: Make a better err message... rather than just changing the label.
                this.label1.Text = "THERE WAS AN ERROR!!!!";
                MessageBox.Show("There are " + chemStrucutres.Count + " structs in file.");
            }
            
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox1.Text != null) { 
            this.button2.Enabled = true;
            }
        }

    }
}
