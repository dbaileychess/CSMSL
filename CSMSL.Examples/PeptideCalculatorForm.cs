using System;
using System.Windows.Forms;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace CSMSL.Examples
{
    public partial class PeptideCalculatorForm : Form
    {
        public PeptideCalculatorForm()
        {
            InitializeComponent();
        }
        
        private void DisplayPeptide(string sequence)
        {
            try
            {
                Peptide peptide = new Peptide(sequence);
                peptideMassTB.Text = peptide.MonoisotopicMass.ToString();
                peptideMZTB.Text = peptide.ToMz(1).ToString();
            }
            catch (Exception)
            {
                peptideMZTB.Text = peptideMassTB.Text = "Not a valid Sequence";
            }
        }

        private void sequenceTB_TextChanged(object sender, EventArgs e)
        {
            DisplayPeptide(sequenceTB.Text);
        }

       
    }
}
