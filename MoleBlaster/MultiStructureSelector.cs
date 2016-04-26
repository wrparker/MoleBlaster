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
    public partial class MultiStructureSelector : Form
    {
        private List<IndigoObject> _chemStructures;
        private Indigo _indigo;
        public MultiStructureSelector(List<IndigoObject> chemStructures, Indigo indigo)
        {
            InitializeComponent();
            _chemStructures = chemStructures;
            _indigo = indigo;
            showSelections();

        }
        private void selection_Click(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            // identify which button was clicked and perform necessary actions
            int f = tableLayoutPanel1.GetRow(button);
            if (button.Checked)
            {
                this.tableLayoutPanel1.GetControlFromPosition(0, int.Parse(button.Name)).BackColor = Color.LightGray;
            }
            else if(button.Checked == false)
            {
                this.tableLayoutPanel1.GetControlFromPosition(0, int.Parse(button.Name)).BackColor = Color.Transparent;
            }

        }
        private void showSelections()
        {
            IndigoRenderer renderer = new IndigoRenderer(_indigo);
            _indigo.setOption("render-output-format", "emf");
            //_indigo.setOption("render-margins", 10, 10);
            _indigo.setOption("render-bond-ids-visible", true);
            _indigo.setOption("render-label-mode", "hetero");
            _indigo.setOption("render-stereo-style", "none");
            //_indigo.setOption("render-bond-length", 45);
            _indigo.setOption("render-image-size", 200, 250);
            List<Panel> scroll = new List<Panel>();
            List<PictureBox> renders = new List<PictureBox>();
            List<Button> zoomInButton = new List<Button>();
            List<Button> zoomOutButton = new List<Button>();

            this.tableLayoutPanel1.RowCount = 0;
            this.tableLayoutPanel1.RowStyles.Clear();
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.AutoSize = true;

            foreach(IndigoObject item in _chemStructures)
            {
                item.layout();
                MemoryStream ms = new MemoryStream(renderer.renderToBuffer(item));
                renders.Add(new PictureBox());
                scroll.Add(new Panel());
                scroll.Last().Size = new System.Drawing.Size(200, 250);

                renders.Last().SizeMode = PictureBoxSizeMode.StretchImage;
                renders.Last().Size = new System.Drawing.Size(200, 250);
                renders.Last().Image = Image.FromStream(ms);
                ms.Close();
                int row = tableLayoutPanel1.RowCount;
               
                tableLayoutPanel1.Controls.Add(scroll.Last(), 0 /* Column Index */, row /* Row index */);
                scroll.Last().Controls.Add(renders.Last());

                RadioButton selection = new RadioButton();
                selection.CheckedChanged += new EventHandler(selection_Click);
                
                selection.Name = (tableLayoutPanel1.RowCount).ToString();
                this.tableLayoutPanel1.Controls.Add(selection, 1 /* Column Index */, row /* Row index */);
                this.tableLayoutPanel1.RowCount++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<IndigoObject> _chosenStructure = new List<IndigoObject>();
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if (c is RadioButton)
                {
                    RadioButton radio = c as RadioButton;
                    if (radio is RadioButton && radio.Checked == true) { 
                        _chosenStructure.Add(_chemStructures[int.Parse(c.Name)]);
                        TemplateBuilder f = new TemplateBuilder(_chosenStructure, _indigo);
                        this.Close();
                        f.Show();
                    }
                }
            }
        }

        private void MultiStructureSelector_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
