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
    public partial class MoleculeBuilder : Form
    {
        private List<IndigoObject> _chemStructures = new List<IndigoObject>();
        private List<fragmentationRule> _rules;
        private Indigo _indigo = new Indigo();
        private bool showBonds = false;
        private bool showAtoms = true;
        private bool mouseDown = false;
        private Point initialPos = new Point();
        private Point scrollPos = new Point();
        private List<Label> rlabels = new List<Label>();
        private List<GenericGroup> rmasses = new List<GenericGroup>();
        private List<int> rGrpIndex = new List<int>();

        public MoleculeBuilder()
        {
            InitializeComponent();
            //Preserve ordering.
            _indigo.setOption("serialize-preserve-ordering", true);

        }

        private void loadInput()
        {
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("Not a valid file! \"" + textBox1.Text + "\" does not exist.  Please try again.");
            }
            else { 
            this.tableLayoutPanel1.SuspendLayout();
            IndigoObject savedStrut = XmlSaver.decryptIndigo(_indigo, openFileDialog1.FileName);
            _chemStructures.Add(savedStrut);
            _rules = XmlSaver.readRules(openFileDialog1.FileName);
            renderMolecule(_chemStructures[0]);
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowCount = 0;
            tableLayoutPanel1.ColumnCount = 0;
            tableLayoutPanel1.Controls.Clear();

            //Load R groups
            int i = 0;
            foreach (IndigoObject rgroup in _chemStructures[0].iterateAtoms())
            {
                if (rgroup.symbol().Equals("R")) {
                    Label temp = new Label();
                    temp.Name = rgroup.index().ToString();
                    temp.Text = "Mass at R # " + rgroup.index();
                    rlabels.Add(temp);

                    GenericGroup rGroup = new GenericGroup();
                    TextBox temp2 = new TextBox();
                    temp2.Name = rgroup.index().ToString();
                    temp2.Text = "<Input Mass at R # " + rgroup.index() + ">";
                    temp2.Width = 150;
                    rGroup.interfaceText = temp2;
                    rGroup.index = rgroup.index();
                    rmasses.Add(rGroup);

                    tableLayoutPanel1.Controls.Add(rlabels.Last(), 0 /* Column Index */, i /* Row index */);
                    tableLayoutPanel1.Controls.Add(rmasses.Last().interfaceText, 1 /* Column Index */, i /* Row index */);
                    i++;

                    //rGrpINdex to help save stuff
                    rGrpIndex.Add(rgroup.index());
                }

            }
            Console.WriteLine(i);
            if (i == 0)
            {
                Label temp = new Label();
                temp.Text = "No R Groups Found. ";
                tableLayoutPanel1.Controls.Add(temp, 0 /* Column Index */, i /* Row index */);
            }

            this.tableLayoutPanel1.ResumeLayout();
            this.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadInput();
            this.button2.Enabled = false;
            this.button1.Enabled = false;
            this.button11.Enabled = true;
        }

        private void renderMolecule(IndigoObject input)
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

            input.layout();
            MemoryStream ms = new MemoryStream(renderer.renderToBuffer(input));
            pictureBox1.Image = Image.FromStream(ms);
            ms.Close();
        }


        private void renderMolecule2(IndigoObject input, string mw)
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
            _indigo.setOption("render-atom-ids-visible", false);
            _indigo.setOption("render-atom-ids-visible", true);
            _indigo.setOption("render-label-mode", "hetero");
            _indigo.setOption("render-stereo-style", "none");
            _indigo.setOption("render-bond-length", 45);

            input.layout();
            Form1 f = new Form1();
            MemoryStream ms = new MemoryStream(renderer.renderToBuffer(input));
            f.pictureBox1.Image = Image.FromStream(ms);
            f.label1.Text = mw;
            f.Show();
            ms.Close();
        }


        private void button6_Click(object sender, EventArgs e)
        {
            if (showAtoms == false)
            {
                showAtoms = true;
            }
            else
            {
                showAtoms = false;
            }
            renderMolecule(_chemStructures[0]);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (showBonds == false)
            {
                showBonds = true;
            }
            else
            {
                showBonds = false;
            }
            renderMolecule(_chemStructures[0]);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Width > 10000)
                return;
            else
            {
                pictureBox1.Width += Convert.ToInt32(pictureBox1.Width * 200 / 1000);
                pictureBox1.Height += Convert.ToInt32(pictureBox1.Height * 200 / 1000);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Width < 200)
                return;
            else
            {
                pictureBox1.Width -= Convert.ToInt32(pictureBox1.Width * 200 / 1000);
                pictureBox1.Height -= Convert.ToInt32(pictureBox1.Height * 200 / 1000);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            initialPos = e.Location;
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
                this.scrollPos += (Size)initialPos - (Size)e.Location;
                this.panel2.AutoScrollPosition = scrollPos;
                this.ResumeLayout();

            }
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

        private void button9_Click(object sender, EventArgs e)
        {
            try { 
            dataViewer f = new dataViewer(_rules);
                f.Show();
                }
            catch (NullReferenceException except)
            {
                MessageBox.Show("You need to load a file first... or some other error " + except);
            }
            
        }



        private void button7_Click(object sender, EventArgs e)
        {
            try { 
            dataViewer f = new dataViewer(generateFragByRules(), 1);
            f.ShowDialog();
                }
            catch (NullReferenceException except)
            {
                MessageBox.Show("You need to load a file first Or some other exception " + except);
            }
            catch (ArgumentOutOfRangeException except)
            {
                MessageBox.Show("No molecule loaded... " + except);
            }
         }

        //private Fragment combinatoricGeneration(IndigoObject structure, )
        

        //Need Combainatorics still...
        private List<Fragment> generateFragByRules()
        {
            int percent = 0;
            label3.Text = percent.ToString() + "%";
            progressBar1.Maximum = _rules.Count;
            progressBar1.Value = 0;
            progressBar1.Step = 1;

            //Do this to preserve hydrogen numbers so that breaking bonds doesn't cause re-ordering.
            foreach (IndigoObject atom in _chemStructures[0].iterateAtoms())
            {
                try { 
                atom.countHydrogens();
                    }
                catch
                {
                    Console.Out.Write("Pseudo atoms have no H's");
                }
            }

            List<Fragment> _frags = new List<Fragment>();
                _frags.AddRange(recursiveFragGeneration(_chemStructures[0], _rules));

                progressBar1.PerformStep();
                percent = (int)(progressBar1.Value / (progressBar1.Maximum * 100));
                label3.Text = percent.ToString() + "%";
                label3.Refresh();

                return _frags;
        }

        private double calculateRgrpMassShift(fragmentationRule rule, IndigoObject structure)
        {
            Console.Out.WriteLine(rule._MassShift1);
            double mshift = 0.00;

            foreach (IndigoObject atom in structure.iterateAtoms())
            {
                    if (atom.symbol().Equals("R"))
                    {

                        try
                        {
                            double RgrpMassShift = double.Parse(rmasses[rmasses.IndexOf(rmasses.Find(x => x.index == atom.index()))].interfaceText.Text);
                            mshift += RgrpMassShift;
                        }
                        catch (FormatException e)
                        {
                            MessageBox.Show("All R-Groups must have masses... R Group #" + atom.index() + " does not!");
                        }

                }
            }
            Console.Out.WriteLine("MASS SHIFT FINAL " + mshift);
            return mshift;

        }

       private List<Fragment> recursiveFragGeneration(IndigoObject prefrag, List<fragmentationRule> _copyRules, string currName = "" )
        {
            List<fragmentationRule> copyRules = new List<fragmentationRule>();
            copyRules.AddRange(_copyRules);
            List<Fragment> _frags = new List<Fragment>();
            foreach (IndigoObject structure in prefrag.iterateComponents())
            {
                foreach (fragmentationRule rule in _copyRules)
                {
                    IndigoObject temp = structure.clone();
                    try
                    {
                        temp.getBond(rule._bondId).remove();
                        if (rule._bondId2 != -1)
                        {
                            temp.getBond(rule._bondId2).remove();
                        }
                        currName = currName + "+" + rule.fragmentName;
                        if (_copyRules.Count == _rules.Count)
                        {
                            currName = rule.fragmentName;
                        }

                        Console.Out.WriteLine("RULE NAME " + currName);

                        for (int i = 0; i < temp.countComponents(); i++)
                        {
                            IndigoObject currFragment = temp.component(i).clone();
                            double totalMassShift = 0.00;
                            totalMassShift = calculateRgrpMassShift(rule, currFragment);
                            Fragment currFragAdd = new Fragment(rule);

                            Console.Out.WriteLine("MASS BEFORE " + totalMassShift);
                            foreach (IndigoObject atoms in temp.component(i).iterateAtoms())
                            {
                                if (atoms.componentIndex() == i)
                                {
                                    if (atoms.index() == rule._atomId1)
                                    {
                                        totalMassShift = totalMassShift + rule._MassShift1;
                                    }
                                    if (atoms.index() == rule._atomId2)
                                    {
                                        totalMassShift = totalMassShift + rule._MassShift2;
                                    }
                                }
                            }
                            currFragAdd.mass = (currFragment.monoisotopicMass() + totalMassShift);                                                     
                            currFragAdd.fragmentName = currName;
                            _frags.Add(currFragAdd);
                            copyRules.Remove(rule);
                            }
                        _frags.AddRange(recursiveFragGeneration(temp, copyRules, currName));
                        }
                    
                    catch (Exception e)
                    {
                        Console.Out.WriteLine("That Atom doesn't exist in thsi structure!" + e);
                    }
                }
            }

           // renderMolecule2(prefrag, prefrag.monoisotopicMass().ToString());
            return _frags;
        }

        //start naive
        private List<Fragment> generateFragByNaive()
        {
            int percent = 0;
            progressBar1.Maximum = _chemStructures[0].countBonds();
            int PBar_Percent = 0;
            progressBar1.Value = PBar_Percent;
            progressBar1.Step = 1;


            //Do this to preserve hydrogen numbers so that breaking bonds doesn't cause re-ordering.
            foreach (IndigoObject atom in _chemStructures[0].iterateAtoms())
            {
                atom.countHydrogens();
            }

            List<Fragment> _frags = new List<Fragment>();

            
            foreach (IndigoObject bond in _chemStructures[0].iterateBonds())
            {
                progressBar1.PerformStep();
                IndigoObject tempStructure = _chemStructures[0].clone();
                percent = (int)(progressBar1.Value / (progressBar1.Maximum * 100));
                label3.Text = percent.ToString() + "%";

                fragmentationRule naiveFragment = new fragmentationRule();
                naiveFragment._atomId1 = bond.source().index();
                naiveFragment._atomId2 = bond.destination().index();
                naiveFragment._bondId = bond.index();

                tempStructure.getBond(bond.index()).remove();

            if(tempStructure.countComponents() > 1)
                {
                    for (int i = 0; i < tempStructure.countComponents(); i++)
                    {
                        IndigoObject currFragment = tempStructure.component(i).clone();
                        double totalMassShift = 0.00;
                        //Get R Group Masses
                        foreach (IndigoObject rgroup in _chemStructures[0].iterateAtoms())
                        {
                            if (rgroup.symbol().Equals("R"))
                            {
                                if (rgroup.componentIndex() == i)
                                {
                                    try
                                    {
                                        double RgrpMassShift = double.Parse(rmasses[rmasses.IndexOf(rmasses.Find(x => x.index == rgroup.index()))].interfaceText.Text);
                                        Console.Out.WriteLine("BEFORE " + totalMassShift);
                                        totalMassShift += RgrpMassShift;
                                        Console.Out.WriteLine("AFTER " + totalMassShift);
                                    }
                                    catch (FormatException e)
                                    {
                                        MessageBox.Show("All R-Groups must have masses... R Group #" + rgroup.index() + " does not!");
                                    }
                                    //Console.Out.WriteLine(rgroup.index() + " has a mass of " +RgrpMassShift);
                                }

                            }
                        }

                        Fragment currFragAdd = new Fragment(naiveFragment);
                        currFragAdd.mass = (currFragment.monoisotopicMass() + totalMassShift);
                        _frags.Add(currFragAdd);
                    }
                }

            }
            return _frags;
        }
        //end naive

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                dataViewer f = new dataViewer(generateFragByNaive(),2);
                f.ShowDialog();
            }
            catch (NullReferenceException except)
            {
                MessageBox.Show("You need to load a file first Or some other exception " + except);
            }
            catch (ArgumentOutOfRangeException except)
            {
                MessageBox.Show("No molecule loaded... " + except);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MoleculeBuilder f = new MoleculeBuilder();
            this.Close();
            f.Show();
            
            
        }
    }

    public class GenericGroup
    {
        public TextBox interfaceText { get; set; }
        public int index { get; set; }
        public GenericGroup()
        {
            index = -1;
            interfaceText = new TextBox();
        }
    } 

    public class Fragment : fragmentationRule{
        public double mass { get; set; }

        public Fragment(fragmentationRule rule)
        {
            fragmentName = rule.fragmentName;
            _bondId = rule._bondId;
            _atomId1 = rule._atomId1;
            _atomId2 = rule._atomId2;
            
            _bondId2 = rule._bondId2;
            _atomId3 = rule._atomId3;
            _atomId4 = rule._atomId4;

            _MassShift1 = rule._MassShift1;
            _MassShift2 = rule._MassShift2;
            _MassShift3 = rule._MassShift3;
            _MassShift4 = rule._MassShift4;


            mass = 0;
        }
    }
}
