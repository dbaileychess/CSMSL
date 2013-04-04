using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using NUnit.Framework;
using Should.Fluent;
using Moq;
using CSMSL.Analysis.ExperimentalDesign;
using CSMSL.Analysis.Quantitation;
using CSMSL.Chemistry;

namespace CSMSL.Tests.Analysis.ExperimentalDesign
{
    [TestFixture]
    [Category("Experimental Design")]  
    public sealed class ExperimentalSetTestFixture
    {
        private ExperimentalSet Experiment1;
        private ExperimentalSet Experiment2;
        private ExperimentalSet Experiment3;
        
        private Sample Sample1;
        private Sample Sample2;
        private Sample Sample3;
        private Sample Sample4;
        private Sample Sample5;
        private Sample Sample6;

        private IQuantitationChannel TMT126;
        private IQuantitationChannel TMT127;
        private IQuantitationChannel TMT128;
        private IQuantitationChannel TMT129;
        private IQuantitationChannel TMT130;
        private IQuantitationChannel TMT131;

        private ExperimentalCondition ConditionA;
        private ExperimentalCondition ConditionB;
        private ExperimentalCondition ConditionC;
        private ExperimentalCondition ConditionD;
        private ExperimentalCondition ConditionE;
        private ExperimentalCondition ConditionF;

        [SetUp]
        public void SetUp()
        {
            Experiment1 = new ExperimentalSet();
            Experiment2 = new ExperimentalSet();
            Experiment3 = new ExperimentalSet();

            TMT126 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-126");
            TMT127 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-127");
            TMT128 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-128");
            TMT129 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-129");
            TMT130 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-130");
            TMT131 = new IsobaricTag(NamedChemicalFormula.TMT6plex, "TMT-131");

            //ConditionA = new ExperimentalCondition("A", "wild-type: 0 min");
            //ConditionB = new ExperimentalCondition("B", "wild-type: 5 min");
            //ConditionC = new ExperimentalCondition("C", "wild-type: 15 min");
            //ConditionD = new ExperimentalCondition("D", "mutant: 0 min");
            //ConditionE = new ExperimentalCondition("E", "mutant: 5 min");
            //ConditionF = new ExperimentalCondition("F", "mutant: 15 min");

        }

        //[Test]
        //public void AddIQuantitationChannelToExperiment()
        //{
        //    Experiment1.AddIQuantitationChannel(TMT126);
        //    Experiment1.AddIQuantitationChannel(TMT127);
        //    Experiment1.IQuantitationChannels.Count.Should().Equal(2);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void AddNullIQuantitationChannelToExperiment()
        //{
        //    TMT127 = null;
        //    Experiment1.AddIQuantitationChannel(TMT126);
        //    Experiment1.AddIQuantitationChannel(TMT127);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentException))]
        //public void AddDuplicateIQuantitationChannelToExperiment()
        //{
        //    TMT127 = TMT126;
        //    Experiment1.AddIQuantitationChannel(TMT126);
        //    Experiment1.AddIQuantitationChannel(TMT127);
        //}

        //[Test]
        //public void AddSampleToExistingExperimentIQuantitationChannel()
        //{
        //    Experiment1.AddIQuantitationChannel(TMT126);
        //    Experiment1.AddSample(new Sample(ConditionA), TMT126);
        //    Experiment1.Samples.Count.Should().Equal(1);
        //}

        //[Test]
        //public void AddSampleToNewExperimentIQuantitationChannel()
        //{
        //    Experiment1.AddSample(new Sample(ConditionA), TMT126);
        //    Experiment1.AddIQuantitationChannel(TMT127);
        //    Experiment1.IQuantitationChannels.Count.Should().Equal(2);
        //    Experiment1.Samples.Count.Should().Equal(1);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void AddNullSampleToExperiment()
        //{
        //    Sample1 = null;
        //    Experiment1.AddSample(Sample1, TMT126);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentException))]
        //public void AddDuplicateSampleToExperiment()
        //{
        //    Sample1 = new Sample(ConditionA);
        //    Experiment1.AddSample(Sample1, TMT126);
        //    Experiment1.AddSample(Sample1, TMT127);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentException))]
        //public void AddSampleToInvalidExperimentIQuantitationChannel()
        //{
        //    Sample1 = new Sample(ConditionA);
        //    Sample2 = new Sample(ConditionB);
        //    Experiment1.AddSample(Sample1, TMT126);
        //    Experiment1.AddSample(Sample2, TMT126);
        //}

        //[Test]
        //public void AddSampleToExperimentCondition()
        //{
        //    Exp1IQuantitationChannels();
        //    Exp1Samples();
        //    Exp2IQuantitationChannels();
        //    Exp2Samples();
        //    Exp3IQuantitationChannels();
        //    Exp3Samples();

        //    Experiment1.Conditions[ConditionA].Count.Should().Equal(1);
        //    Experiment2.Conditions[ConditionA].Count.Should().Equal(3);
        //    Experiment3.Conditions[ConditionA].Count.Should().Equal(2);
        //}

        //private void Exp1IQuantitationChannels()
        //{
        //    Experiment1.AddIQuantitationChannel(TMT126);
        //    Experiment1.AddIQuantitationChannel(TMT127);
        //    Experiment1.AddIQuantitationChannel(TMT128);
        //    Experiment1.AddIQuantitationChannel(TMT129);
        //    Experiment1.AddIQuantitationChannel(TMT130);
        //    Experiment1.AddIQuantitationChannel(TMT131);
        //}

        //private void Exp2IQuantitationChannels()
        //{
        //    Experiment2.AddIQuantitationChannel(TMT126);
        //    Experiment2.AddIQuantitationChannel(TMT127);
        //    Experiment2.AddIQuantitationChannel(TMT128);
        //    Experiment2.AddIQuantitationChannel(TMT129);
        //    Experiment2.AddIQuantitationChannel(TMT130);
        //    Experiment2.AddIQuantitationChannel(TMT131);
        //}

        //private void Exp3IQuantitationChannels()
        //{
        //    Experiment3.AddIQuantitationChannel(TMT126);
        //    Experiment3.AddIQuantitationChannel(TMT127);
        //    Experiment3.AddIQuantitationChannel(TMT128);
        //    Experiment3.AddIQuantitationChannel(TMT129);
        //    Experiment3.AddIQuantitationChannel(TMT130);
        //    Experiment3.AddIQuantitationChannel(TMT131);
        //}

        //private void Exp1Samples()
        //{
        //    Sample1 = new Sample(ConditionA);
        //    Sample2 = new Sample(ConditionB);
        //    Sample3 = new Sample(ConditionC);
        //    Sample4 = new Sample(ConditionD);
        //    Sample5 = new Sample(ConditionE);
        //    Sample6 = new Sample(ConditionF);

        //    Experiment1.AddSample(Sample1, TMT126);
        //    Experiment1.AddSample(Sample2, TMT127);
        //    Experiment1.AddSample(Sample3, TMT128);
        //    Experiment1.AddSample(Sample4, TMT129);
        //    Experiment1.AddSample(Sample5, TMT130);
        //    Experiment1.AddSample(Sample6, TMT131);
        //}

        //private void Exp2Samples()
        //{
        //    Sample1 = new Sample(ConditionA);
        //    Sample2 = new Sample(ConditionA);
        //    Sample3 = new Sample(ConditionA);
        //    Sample4 = new Sample(ConditionB);
        //    Sample5 = new Sample(ConditionB);
        //    Sample6 = new Sample(ConditionB);

        //    Experiment2.AddSample(Sample1, TMT126);
        //    Experiment2.AddSample(Sample2, TMT127);
        //    Experiment2.AddSample(Sample3, TMT128);
        //    Experiment2.AddSample(Sample4, TMT129);
        //    Experiment2.AddSample(Sample5, TMT130);
        //    Experiment2.AddSample(Sample6, TMT131);
        //}

        //private void Exp3Samples()
        //{
        //    Sample1 = new Sample(ConditionA);
        //    Sample2 = new Sample(ConditionB);
        //    Sample3 = new Sample(ConditionC);
        //    Sample4 = new Sample(ConditionA);
        //    Sample5 = new Sample(ConditionB);
        //    Sample6 = new Sample(ConditionC);

        //    Experiment3.AddSample(Sample1, TMT126);
        //    Experiment3.AddSample(Sample2, TMT127);
        //    Experiment3.AddSample(Sample3, TMT128);
        //    Experiment3.AddSample(Sample4, TMT129);
        //    Experiment3.AddSample(Sample5, TMT130);
        //    Experiment3.AddSample(Sample6, TMT131);
        //}
    }
}
