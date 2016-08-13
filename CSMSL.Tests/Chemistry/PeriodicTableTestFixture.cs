// Copyright 2012, 2013, 2014 Derek J. Bailey
//
// This file (PeriodicTableTestFixture.cs) is part of CSMSL.Tests.
//
// CSMSL.Tests is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CSMSL.Tests is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL.Tests. If not, see <http://www.gnu.org/licenses/>.
using NUnit.Framework;
using CSMSL.Chemistry;
using System.IO;
using System.Threading;
using System.Globalization;

namespace CSMSL.Tests.Chemistry
{
    [TestFixture, Category("Periodic Table")]
    public sealed class PeriodicTableTestFixture
    {
        [TestFixtureTearDown]
        public void CleanUP()
        {
            // We need to restore the default table before each test, or other tests will fail
            PeriodicTable.RestoreDefaults();
        }      
   

        [Test]
        public void LoadUserDefineTable()
        {
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <PeriodicTable>
                      <Element Name=""Hydrogen"" Symbol=""H"" AtomicNumber=""1"" AverageMass=""1.0079407538957064"" ValenceElectrons=""1"">
                        <Isotope Id=""0"" Mass=""5.00782503207"" MassNumber=""1"" Abundance=""0.999885"" />
                        <Isotope Id=""7"" Mass=""2.0141017778"" MassNumber=""2"" Abundance=""0.000115"" />
                      </Element>
                    </PeriodicTable>";
         
            string fileName = Path.Combine(Path.GetTempPath(), "testTable.xml");
           
            File.WriteAllText(fileName, xml);

            PeriodicTable.Load(fileName);

            File.Delete(fileName);

            Assert.AreEqual(5.00782503207, PeriodicTable.GetIsotope("H", 1).AtomicMass, 0.0000001);           
        }

        [SetCulture("de")]
        [Test]
        public void LoadUserDefineTableInternational()
        {          
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <PeriodicTable>
                      <Element Name=""Hydrogen"" Symbol=""H"" AtomicNumber=""1"" AverageMass=""1,0079407538957064"" ValenceElectrons=""1"">
                        <Isotope Id=""0"" Mass=""5,00782503207"" MassNumber=""1"" Abundance=""0,999885"" />
                        <Isotope Id=""7"" Mass=""2,0141017778"" MassNumber=""2"" Abundance=""0,000115"" />
                      </Element>
                    </PeriodicTable>";

            string fileName = Path.Combine(Path.GetTempPath(), "testTableInt.xml");

            File.WriteAllText(fileName, xml);

            PeriodicTable.Load(fileName);

            File.Delete(fileName);

            Assert.AreEqual(5.00782503207, PeriodicTable.GetIsotope("H", 1).AtomicMass, 0.0000001);
        }


    }
}
