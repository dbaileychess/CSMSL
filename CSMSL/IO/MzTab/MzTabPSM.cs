using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.IO.MzTab
{
    public class MzTabPSM
    {
        public string Sequence { get; set; }
        public int ID { get; set; }
        public string Accession { get; set; }
        public bool Unique { get; set; }
        public string Database { get; set; }
        public string DatabaseVersion { get; set; }
        public string SearchEngines { get; set; }
        public string SearchEngineScores { get; set; }
        public int? Reliability { get; set; }
        public string Modifications { get; set; }
        public double RetentionTime { get; set; }
        public int Charge { get; set; }
        public double ExperimentalMZ { get; set; }
        public double TheoreticalMZ { get; set; }
        public string URI { get; set; }
        public string SpectraReference { get; set; }
        public char PreviousAminoAcid { get; set; }
        public char FollowingAminoAcid { get; set; }
        public int StartResiduePosition { get; set; }
        public int EndResiduePosition { get; set; }
        public Dictionary<string, string> OptionalData { get; set; }

        private Dictionary<int, string> _engineScores; 

        public void SetEngineScore(int index, string score)
        {
            if (_engineScores == null)
                _engineScores = new Dictionary<int, string>();

            _engineScores.Add(index, score);
        }

        public void AddOptionalField(string key, string value)
        {
            if (OptionalData == null)
                OptionalData = new Dictionary<string, string>();

            OptionalData.Add(key, value);
        }

        public override string ToString()
        {
            return string.Format("(#{0}) {1}",ID, Sequence);
        }
    }
}
