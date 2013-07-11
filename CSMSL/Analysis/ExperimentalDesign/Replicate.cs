
namespace CSMSL.Analysis.ExperimentalDesign
{
    public class Replicate
    {
        public Sample Sample { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public bool IsTechnical { get; set; }
        public bool IsBiological { get; set; }
    }
}
