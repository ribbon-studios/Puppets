using Microsoft.VisualStudio.TestTools.UnitTesting;
using Puppets.Utils;
using System;

namespace UnitTests.Utils
{
    [TestClass]
    public class DateUtilsTests
    {
        [TestMethod]
        public void TimeTill_shouldReturnZeroIfNoTimeHasPassed()
        {
            SystemTime.SetDateTime(DateTime.Now);

            var TimeSpan = DateUtils.TimeTill(SystemTime.NowUTC());

            Assert.AreEqual(0, TimeSpan.Milliseconds);
        }

        [TestMethod]
        public void TimeTill_shouldReturnTheDifference()
        {
            var expectedMS = PTU.Faker.Random.Int(0, 500);

            SystemTime.SetDateTime(DateTime.Now);

            var TimeSpan = DateUtils.TimeTill(SystemTime.NowUTC().AddMilliseconds(expectedMS));

            Assert.AreEqual(expectedMS, TimeSpan.Milliseconds);
        }

        [TestMethod]
        public void Parse()
        {
            var date = DateTime.Now;
            var dateString = DateUtils.ToString(date);

            Assert.AreEqual(date.ToUniversalTime().ToString(), DateUtils.Parse(dateString).ToUniversalTime().ToString());
        }

        [TestMethod]
        public void Parse_shouldHandleMultipleLevels()
        {
            var date = DateTime.Now;
            var dateString = DateUtils.ToString(date);

            Assert.AreEqual(DateUtils.ToString(date.ToUniversalTime()), DateUtils.ToString(DateUtils.Parse(DateUtils.ToString(DateUtils.Parse(dateString)))));
        }
    }
}
