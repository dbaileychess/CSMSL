using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.IO;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using CSMSL.Analysis.Quantitation;
using CSMSL.Chemistry;
using CSMSL.IO.Thermo;
using CSMSL.IO.OMSSA;

namespace CSMSL.Examples
{
    public class TMT6plexExample
    {
        public static void Start()
        {
            // Initial input files
            string fastaFile = "Resources/yeast_uniprot_120226.fasta";
            string psmFile = "Resources/TMT_6plex_yeast_omssa_psms.csv";
            MSDataFile dataFile = new ThermoRawFile("Resources/TMT_6plex_yeast.raw");
            IProtease protease = Protease.Trypsin;

            // PSM loading
            List<PeptideSpectralMatch> psms;

            using (PsmReader psmReader = new OmssaCsvPsmReader(psmFile))
            {                
                psmReader.LoadProteins(fastaFile);
                psmReader.AddMSDataFile(dataFile);

                psmReader.AddFixedModification(NamedChemicalFormula.Carbamidomethyl, ModificationSites.C);
                psmReader.AddFixedModification(QuantitationChannelSet.TMT6Plex, ModificationSites.K | ModificationSites.NPep);
                psmReader.AddVariableModification(NamedChemicalFormula.Oxidation, "oxidation of M");
                psmReader.AddVariableModification(QuantitationChannelSet.TMT6Plex, "TMT_Tyrosine");
                
                psms = psmReader.ReadNextPsm().ToList();
            }

            IList<QuantifiedPeptide> quantPeptides = QuantifiedPeptide.GenerateQuantifiedPeptides(psms);

        }
    }
}
