﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using QSP.LandingPerfCalculation;
using QSP.LandingPerfCalculation.Boeing;
using QSP.LandingPerfCalculation.Boeing.PerfData;
using System;
using System.Xml.Linq;
using UnitTest.Common;

namespace UnitTest.LandingPerfCalculation.Boeing
{
    [TestClass]
    public class LandingReportGeneratorTest
    {
        private const double delta = 1E-7;

        [TestMethod]
        public void GetReportTest()
        {
            string text = new TestData().AllText;
            var doc = XDocument.Parse(text);
            var table = new PerfDataLoader().GetItem(doc);

            var para = new LandingParameters(
                55000.0,
                3000.0,
                1000.0,
                -10.0,
                -1.0,
                15.0,
                5.0,
                ReverserOption.NoRev,
                SurfaceCondition.Good,
                0,
                0);

            var report = new LandingReportGenerator(table, para).GetReport();

            assertMainResult(report, para, table);
            assertOtherResult(report, para, table);
        }

        private void assertMainResult(LandingReport report,
                                      LandingParameters para,
                                      BoeingPerfTable table)
        {
            var entry = report.SelectedBrks;
            var calc = new LandingCalculator(table, para);

            string brake = table
                           .BrakesAvailable(para.SurfaceCondition)
                           [para.BrakeIndex];

            double rwyRequired = calc.DistanceRequiredMeter();

            Assert.IsTrue(entry.BrkSetting == brake);
            Assert.AreEqual(rwyRequired, entry.ActualDisMeter, 0.5);

            double disRemain = para.RwyLengthMeter - rwyRequired;

            Assert.AreEqual(disRemain, entry.DisRemainMeter, 0.5);
        }

        private void assertOtherResult(LandingReport report,
                                       LandingParameters para,
                                       BoeingPerfTable table)
        {
            var calc = new LandingCalculator(table, para);

            foreach (var i in report.AllSettings)
            {
                int brakeIndex = Array.FindIndex(
                          table.BrakesAvailable(para.SurfaceCondition),
                          x => x == i.BrkSetting);

                PropertySetter.Set(para,
                                   "BrakeIndex",
                                   brakeIndex);

                double rwyRequired = calc.DistanceRequiredMeter();
                Assert.AreEqual(rwyRequired, i.ActualDisMeter, 0.5);

                double disRemain = para.RwyLengthMeter - rwyRequired;
                Assert.AreEqual(disRemain, i.DisRemainMeter, 0.5);
            }
        }
    }
}