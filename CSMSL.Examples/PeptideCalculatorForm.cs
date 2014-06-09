// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (PeptideCalculatorForm.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

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