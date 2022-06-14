// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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