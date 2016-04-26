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
using System.Xml.Serialization;

namespace MoleBlaster
{
    public partial class TemplateBuilder : Form
    {
        private List<IndigoObject> _chemStructures;
        private Indigo _indigo;
        private bool showAtoms = false;
        private bool showBonds = true;
        private List<fragmentationRule> rules = new List<fragmentationRule>();
        private bool savedRule = true;
        private bool mouseDown = false;
        private Point initialImagePosition = new Point();
        private Point scrollPos = new Point();
        private bool _crossRingSelect = false;
        
        public TemplateBuilder(List<IndigoObject> chemStructures, Indigo indigo)
        {
            InitializeComponent();
            this.comboBox1.DisplayMember = "fragmentName";
            _indigo = indigo;
            //Preserve ordering.
            _indigo.setOption("serialize-preserve-ordering", true);
            _chemStructures = chemStructures;
            renderMolecule();
        }

        public TemplateBuilder(List<IndigoObject> chemStructres, Indigo indigo, List<fragmentationRule> rules)
        {
            InitializeComponent();         
            _indigo = indigo;
            //Preserve ordering.
            _indigo.setOption("serialize-preserve-ordering", true);
            _chemStructures = chemStructres;
            renderMolecule();

          foreach(fragmentationRule rule in rules)
            {
                this.comboBox1.Items.Add(rule);
            }
          comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
          this.comboBox1.DisplayMember = "fragmentName";
            
        }

        private void renderMolecule()
        {
            IndigoRenderer renderer = new IndigoRenderer(_indigo);
            _indigo.setOption("render-output-format", "emf");
            _indigo.setOption("render-margins", 10, 10);

            if (showBonds == true)
            {
                _indigo.setOption("render-bond-ids-visible", true);
            }
            else
            {
                _indigo.setOption("render-bond-ids-visible", false);
            }
            if (showAtoms == true)
            {
                _indigo.setOption("render-atom-ids-visible", true);
            }
            else
            {
                _indigo.setOption("render-atom-ids-visible", false);
            }
            _indigo.setOption("render-label-mode", "hetero");
            _indigo.setOption("render-stereo-style", "none");
            _indigo.setOption("render-bond-length", 45);

            _chemStructures[0].layout();
            MemoryStream ms = new MemoryStream(renderer.renderToBuffer(_chemStructures[0]));
            pictureBox1.Image = Image.FromStream(ms);
            ms.Close();
        }

        private void TemplateBuilder_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta <= 0)
                {
                    if (pictureBox1.Width < 475)
                        return;
                }
                else
                {
                    if (pictureBox1.Width > 10000)
                        return;
                }
                pictureBox1.Width += Convert.ToInt32(pictureBox1.Width * e.Delta / 1000);
                pictureBox1.Height += Convert.ToInt32(pictureBox1.Height * e.Delta / 1000);
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Width > 10000)
                return;
            else
            {
                pictureBox1.Width += Convert.ToInt32(pictureBox1.Width * 200 / 1000);
                pictureBox1.Height += Convert.ToInt32(pictureBox1.Height * 200 / 1000);
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Width < 475)
                return;
            else
            {
                pictureBox1.Width -= Convert.ToInt32(pictureBox1.Width * 200 / 1000);
                pictureBox1.Height -= Convert.ToInt32(pictureBox1.Height * 200 / 1000);
            }
        }

        private void atomToggle()
        {
            if (showAtoms == false)
            {
                showAtoms = true;
            }
            else
            {
                showAtoms = false;
            }
        }

        private void bondToggle()
        {
            if (showBonds == false)
            {
                showBonds = true;
            }
            else
            {
                showBonds = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bondToggle();
            renderMolecule();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            atomToggle();
            renderMolecule();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        private void button7_Click(object sender, EventArgs e)
        {
            try { 
                //make sure Bond 1 != Bond 2
                if ((_crossRingSelect == true && textBox5.Text != textBox1.Text) || (_crossRingSelect == false))
                {
                    (comboBox1.SelectedItem as fragmentationRule).fragmentName = textBox2.Text;
                    (comboBox1.SelectedItem as fragmentationRule)._atomId1 = int.Parse(label12.Text);
                    (comboBox1.SelectedItem as fragmentationRule)._atomId2 = int.Parse(label11.Text);
                    (comboBox1.SelectedItem as fragmentationRule)._bondId = int.Parse(textBox1.Text);
                    (comboBox1.SelectedItem as fragmentationRule)._MassShift1 = (double.Parse(textBox3.Text));
                    (comboBox1.SelectedItem as fragmentationRule)._MassShift2 = (double.Parse(textBox4.Text));
                    //Try to catch exception
                    _chemStructures[0].getBond((comboBox1.SelectedItem as fragmentationRule)._bondId).source();
                    _chemStructures[0].getBond((comboBox1.SelectedItem as fragmentationRule)._bondId).destination();

                    if (_crossRingSelect == true) { 
                    (comboBox1.SelectedItem as fragmentationRule)._bondId2 = int.Parse(textBox5.Text);
                    (comboBox1.SelectedItem as fragmentationRule)._atomId3 = int.Parse(label21.Text);
                    (comboBox1.SelectedItem as fragmentationRule)._atomId4 = int.Parse(label20.Text);
                    (comboBox1.SelectedItem as fragmentationRule)._MassShift3 = (double.Parse(textBox6.Text));
                    (comboBox1.SelectedItem as fragmentationRule)._MassShift4 = (double.Parse(textBox7.Text));
                    //Try to catch exception
                    _chemStructures[0].getBond((comboBox1.SelectedItem as fragmentationRule)._bondId2).source();
                    _chemStructures[0].getBond((comboBox1.SelectedItem as fragmentationRule)._bondId2).destination();
                    }
                    else
                    {
                        (comboBox1.SelectedItem as fragmentationRule)._bondId2 = -1;
                        (comboBox1.SelectedItem as fragmentationRule)._atomId3 = -1;
                        (comboBox1.SelectedItem as fragmentationRule)._atomId4 = -1;
                        (comboBox1.SelectedItem as fragmentationRule)._MassShift3 = -1;
                        (comboBox1.SelectedItem as fragmentationRule)._MassShift4 = -1;
                    }

                    //Try to get bond exception to catch
                    

                    

                    savedRule = true;

                    this.label8.Hide();

                    //bad way to update the list... but it works... so whatever.
                    this.comboBox1.DisplayMember = "fragmentFFF";
                    this.comboBox1.DisplayMember = "fragmentName";
                    this.comboBox1.Update();
                    this.comboBox1.Refresh();
                }
                else
                {
                    MessageBox.Show("Error:  Bond 1 and Bond 2 can't be the same...");
                }
            }
            catch (FormatException except)
            {
                MessageBox.Show("Error:  You must input numbers for MassShift, BondIDs, etc... Exception: " + except);
             
            }
            catch (com.ggasoftware.indigo.IndigoException except)
            {
                MessageBox.Show("Error:  Bond doesn't exist! Exception: " + except);
            }
            /*
            //Comeback.  Lets fix itnerface for now.
            int[] bonds = new int[1];
            bonds[0] = 27;
            List<IndigoObject> tests = new List<IndigoObject>();
            _chemStructures[0].removeBonds(bonds);
            _chemStructures[0].getSubmolecule(bonds);
            Console.WriteLine(_chemStructures[0].getSubmolecule(bonds));

           
            Console.Out.WriteLine(_chemStructures[0].grossFormula());
            renderMolecule();*/
        }



        private void disable_rule_controls()
        {
            this.textBox1.Enabled = false;
            this.textBox2.Enabled = false;
            this.textBox3.Enabled = false;
            this.textBox4.Enabled = false;
            this.button5.Enabled = false;
            this.textBox5.Enabled = false;
            this.checkBox1.Enabled = false;
            this.checkBox1.Checked = false;
            this.textBox6.Enabled = false;
            this.textBox7.Enabled = false;

        }
        private void enable_rule_controls()
        {
            this.checkBox1.Enabled = true;
            this.textBox1.Enabled = true;
            this.textBox2.Enabled = true;
            this.textBox3.Enabled = true;
            this.textBox4.Enabled = true;
            this.button5.Enabled = true;
            
        }

        private void fill_rule_controls()
        {
            //We need to rechange the rules etc.. when they are edited.
            textBox2.Text = (comboBox1.SelectedItem as fragmentationRule).fragmentName;
            label12.Text = (comboBox1.SelectedItem as fragmentationRule)._atomId1.ToString();
            label11.Text = (comboBox1.SelectedItem as fragmentationRule)._atomId2.ToString();
            textBox1.Text = (comboBox1.SelectedItem as fragmentationRule)._bondId.ToString();
            textBox3.Text = (comboBox1.SelectedItem as fragmentationRule)._MassShift1.ToString();
            textBox4.Text = (comboBox1.SelectedItem as fragmentationRule)._MassShift2.ToString();

            if ((comboBox1.SelectedItem as fragmentationRule)._bondId2 != -1)
            {
                checkBox1.Checked = true;
                textBox5.Text = (comboBox1.SelectedItem as fragmentationRule)._bondId2.ToString();
                textBox6.Text = (comboBox1.SelectedItem as fragmentationRule)._MassShift3.ToString();
                textBox7.Text = (comboBox1.SelectedItem as fragmentationRule)._MassShift4.ToString();
                label21.Text = (comboBox1.SelectedItem as fragmentationRule)._atomId3.ToString();
                label20.Text = (comboBox1.SelectedItem as fragmentationRule)._atomId4.ToString();
            }
            else
            {
                checkBox1.Checked = false;
                textBox5.Text = "-1";
                textBox6.Text = "0";
                textBox7.Text = "0";
                label21.Text = "";
                label20.Text = "";
                label15.Hide();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                disable_rule_controls();
            }
            else
            {
                enable_rule_controls();
                fill_rule_controls();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (savedRule == false)
            {
                label8.Show();
            }
            else
            {
                savedRule = false;
                comboBox1.DisplayMember = "fragmentName";
                fragmentationRule f = new fragmentationRule();
                comboBox1.Items.Add(f);
                Console.WriteLine(comboBox1.SelectedValue);
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                label8.Hide();
            }

            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Remove(comboBox1.SelectedItem);
            comboBox1.ResetText();
            comboBox1.SelectedIndex = -1;
            disable_rule_controls();
            savedRule = true;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            initialImagePosition = e.Location;
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.SuspendLayout();
                this.scrollPos += (Size)initialImagePosition - (Size)e.Location;
                this.panel2.AutoScrollPosition = scrollPos;
                this.ResumeLayout();
                
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string txtName = ((TextBox)sender).Name;
            try{

                if (txtName == "textBox1")
                {
                    label12.Text = (_chemStructures[0].getBond(int.Parse(textBox1.Text)).source().index().ToString());
                    label11.Text = (_chemStructures[0].getBond(int.Parse(textBox1.Text)).destination().index().ToString());
                    label13.Hide();
                }
                else if (txtName == "textBox5")
                {
                    label21.Text = (_chemStructures[0].getBond(int.Parse(textBox5.Text)).source().index().ToString());
                    label20.Text = (_chemStructures[0].getBond(int.Parse(textBox5.Text)).destination().index().ToString());
                    label15.Hide();
                }
                
            }
            catch(FormatException f){
                if (txtName == "textBox1") { 
                label13.Show();
                }
                else if (txtName == "textBox5")
                {
                    label15.Show();
                }
                Console.Out.WriteLine(f);
            }
            catch (com.ggasoftware.indigo.IndigoException f)
            {
                if (txtName == "textBox1")
                {
                    label13.Show();
                }
                else if (txtName == "textBox5")
                {
                    label15.Show();
                }
                Console.Out.WriteLine(f);
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Save routine...
            saveFileDialog1.AutoUpgradeEnabled = false;
            string file_location ="";
            if(savedRule == true && comboBox1.Items.Count > 0){
                DialogResult result = this.saveFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    file_location = saveFileDialog1.FileName;
                    String _FileName = file_location;
                    System.IO.FileStream _FileStream =
                    new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                        System.IO.FileAccess.Write);
                    _FileStream.Write(_chemStructures[0].serialize(), 0, _chemStructures[0].serialize().Length);

                    StreamWriter s = new StreamWriter(_FileStream);
                    s.WriteLine("");
                    s.WriteLine("---+++---");
                    for (int i = 0; i < comboBox1.Items.Count; i++)
                    {
                        rules.Add(comboBox1.Items[i] as fragmentationRule);
                    }
                    s.WriteLine(rules.SerializeObject());
                s.Close();
                _FileStream.Close();
                MessageBox.Show("Template Successfully Saved!");
                this.Close();

                }
                else if (result != DialogResult.OK)
                {
                    MessageBox.Show("Problem saving file.");
                }
            }
            else if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show("No Rules Defined!");
            }
            else if (savedRule == false)
            {
                MessageBox.Show("Save current rule before saving the template!");
            }             
            

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox3.Enabled) { 
            double result;
            double.TryParse(this.textBox3.Text, out result);
            result += 1.007825;
            this.textBox3.Text = result.ToString();
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (textBox4.Enabled) { 
            double result;
            double.TryParse(this.textBox4.Text, out result);
            result += 1.007825;
            this.textBox4.Text = result.ToString();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (textBox4.Enabled)
            {
                double result;
                double.TryParse(this.textBox4.Text, out result);
                result -= 1.007825;
                this.textBox4.Text = result.ToString();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (textBox3.Enabled) { 
            double result;
            double.TryParse(this.textBox3.Text, out result);
            result -= 1.007825;
            this.textBox3.Text = result.ToString();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox6.Enabled) { 
            double result;
            double.TryParse(this.textBox6.Text, out result);
            result += 1.007825;
            this.textBox6.Text = result.ToString();
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (textBox6.Enabled)
            {
                double result;
                double.TryParse(this.textBox6.Text, out result);
                result -= 1.007825;
                this.textBox6.Text = result.ToString();
            }
        }
        private void button14_Click(object sender, EventArgs e)
        {
            if (textBox7.Enabled)
            {
                double result;
                double.TryParse(this.textBox7.Text, out result);
                result += 1.007825;
                this.textBox7.Text = result.ToString();
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (textBox7.Enabled)
            {
                double result;
                double.TryParse(this.textBox7.Text, out result);
                result -= 1.007825;
                this.textBox7.Text = result.ToString();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_crossRingSelect == false)
            {
                _crossRingSelect = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox5.Enabled = true;
                label15.Show();
            }
            else
            {
                _crossRingSelect = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox5.Enabled = false;
                label15.Hide();
                
            }

        }
    }


    public class fragmentationRule
    {
        /* int _bondId;
        private int _atomId1;
        private int _atomId2;
        private double _MassShift1;
        private double _MassShift2;*/

        //TODO: remove and just use get set declarations here.
        public string fragmentName { get; set; }
        public int _bondId { get; set; }
        public int _bondId2 { get; set; }
        public int _atomId1 { get; set; }
        public int _atomId2 { get; set; }
        public int _atomId3 { get; set; }
        public int _atomId4 { get; set; }
        public double _MassShift1 { get; set; }
        public double _MassShift2 { get; set; }
        public double _MassShift3 { get; set; }
        public double _MassShift4 { get; set; }

        public fragmentationRule()
        {
            fragmentName = "<New Rule>";
            _bondId = -1;
            _bondId2 = -1;
            _atomId1 = 0;
            _atomId2 = 0;
            _MassShift1 = 0;
            _MassShift2 = 0;
            _MassShift3 = 0;
            _MassShift4 = 0;
        }

    }

    public static class XmlSaver
    {
        //public static object DeserializeObject<T>(this string toDeserialize)
        public static object DeserializeObject(this string toDeserialize)
        {
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<fragmentationRule>));
            StringReader textReader = new StringReader(toDeserialize);
            return xmlSerializer.Deserialize(textReader);
        }

        public static string SerializeObject<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        public static IndigoObject decryptIndigo (Indigo indigo, string fileName)
        {
            return indigo.unserialize(File.ReadAllBytes(fileName));
        }

        public static List<fragmentationRule> readRules (string fileName) {
            string[] line = File.ReadAllLines(fileName);
            int seek = 0;

            //Skip the byte code
         
            while (!string.Equals(line[seek], "---+++---", StringComparison.OrdinalIgnoreCase)){
                seek++;
            }
            seek++;

            string concatString = "";
            
            for(int j = seek; j < line.Length; j++)
            {
                concatString += line[j];
            }
            return (List<fragmentationRule>)DeserializeObject(concatString);
        }
    }
}
