using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobilki;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testy
{
    [TestClass()]
    public class KalkulatorTests
    {
        [TestMethod()]
        public void DodawanieTest()
        {
            Assert.AreEqual(Kalkulator.Dodawanie(3, 2), 5);
            Assert.AreNotEqual(Kalkulator.Dodawanie(3, 3), 5);
        }

        [TestMethod()]
        public void OdejmowanieTest()
        {
            Assert.AreEqual(Kalkulator.Odejmowanie(3, 2), 1);
            Assert.AreNotEqual(Kalkulator.Odejmowanie(3, 3), 5);
        }

        [TestMethod()]
        public void MnozenieTest()
        {
            Assert.AreEqual(Kalkulator.Mnozenie(3, 2), 6);
            Assert.AreNotEqual(Kalkulator.Mnozenie(3, 3), 4);
        }

        [TestMethod()]
        public void DzielenieTest()
        {
            Assert.AreEqual(Kalkulator.Dzielenie(6, 2), 3);
            Assert.AreNotEqual(Kalkulator.Dzielenie(3, 3), 2);
        }
    }
}
