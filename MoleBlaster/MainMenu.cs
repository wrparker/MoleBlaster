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
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about f = new about();
            f.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void testFormToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }




        private void button1_Click(object sender, EventArgs e)
        {
            FileSelector g = new FileSelector();
            g.ShowDialog();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MoleculeBuilder f = new MoleculeBuilder();
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TemplateSelector g = new TemplateSelector();
            g.ShowDialog();
        }
    }

    public class TemplateSelector : FileSelector{
        
        public TemplateSelector()
        {
            InitializeComponent();
            
        }

        private void InitializeComponent()
        {
            this.label1.Text = "Select a Mole Blaster Tempalte File (MBTF).";
            this.openFileDialog1.Filter= "Mole Blaster Template File (MBTF)|*.MBTF";
            this.button2.Click -= new System.EventHandler(this.button2_Click);
            this.button2.Click += new System.EventHandler(this.Button2_Click);
        }

        //Edit dialogue.
        private void Button2_Click(object sender, EventArgs e)
        {
            Indigo _indigo = new Indigo();
            IndigoObject savedStrut = XmlSaver.decryptIndigo(_indigo, openFileDialog1.FileName);

            List<IndigoObject> chemStructures = new List<IndigoObject>();
            chemStructures.Add(savedStrut);

            List<fragmentationRule> _rules = XmlSaver.readRules(openFileDialog1.FileName);
            TemplateBuilder g = new TemplateBuilder(chemStructures, _indigo, _rules);
            g.ShowDialog();

            this.Close();
        } 
    }
}
