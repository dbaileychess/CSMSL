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
