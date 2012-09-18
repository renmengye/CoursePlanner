using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Linq;

namespace Panta.Tests
{
    [TestClass]
    public class CourseSectionTimeTests
    {
        [TestMethod]
        public void TryParseTimeSpanIntBasics()
        {
            byte result;
            UTCourseSectionTimeSpan.TryParseTimeSpanInt("11:15", out result);
            Assert.AreEqual(45, result);
            UTCourseSectionTimeSpan.TryParseTimeSpanInt("11:30", out result);
            Assert.AreEqual(46, result);
            UTCourseSectionTimeSpan.TryParseTimeSpanInt("11", out result);
            Assert.AreEqual(44, result);
        }

        [TestMethod]
        public void TryParseTimeSpanIntPrecision()
        {
            byte result;
            Assert.AreEqual(false, UTCourseSectionTimeSpan.TryParseTimeSpanInt("11:32", out result));
        }

        [TestMethod]
        public void TryParseTimeSpanIntTooLong()
        {
            byte result;
            Assert.AreEqual(false, UTCourseSectionTimeSpan.TryParseTimeSpanInt("11:30:30", out result));
        }

        [TestMethod]
        public void TryParseTimeSpanIntNotInt()
        {
            byte result;
            Assert.AreEqual(false, UTCourseSectionTimeSpan.TryParseTimeSpanInt("a:30:30", out result));
        }

        [TestMethod]
        public void TryParseRawTimeSpanBasics()
        {
            CourseSectionTimeSpan span;
            UTCourseSectionTimeSpan.TryParseRawTimeSpan("T", out span);
            Assert.AreEqual("Tuesday 0:00-0:00", span.ToString());
            UTCourseSectionTimeSpan.TryParseRawTimeSpan("F9", out span);
            Assert.AreEqual("Friday 9:00-10:00", span.ToString());
            UTCourseSectionTimeSpan.TryParseRawTimeSpan("W2:30", out span);
            Assert.AreEqual("Wednesday 14:30-15:30", span.ToString());
            UTCourseSectionTimeSpan.TryParseRawTimeSpan("M2-4", out span);
            Assert.AreEqual("Monday 14:00-16:00", span.ToString());
            UTCourseSectionTimeSpan.TryParseRawTimeSpan("W10:30-12", out span);
            Assert.AreEqual("Wednesday 10:30-12:00", span.ToString());
            UTCourseSectionTimeSpan.TryParseRawTimeSpan("W7-9", out span);
            Assert.AreEqual("Wednesday 19:00-21:00", span.ToString());
        }

        [TestMethod]
        public void TryParseRawTimeBasics()
        {
            CourseSectionTime time;
            UTCourseSectionTime.TryParseRawTime("T", out time);
            Assert.AreEqual("Tuesday 0:00-0:00", time.ToString());
            UTCourseSectionTime.TryParseRawTime("TF", out time);
            Assert.AreEqual("Tuesday 0:00-0:00 Friday 0:00-0:00", time.ToString());
            UTCourseSectionTime.TryParseRawTime("TF9", out time);
            Assert.AreEqual("Tuesday 9:00-10:00 Friday 9:00-10:00", time.ToString());
            UTCourseSectionTime.TryParseRawTime("W10-12:15TF1:30", out time);
            Assert.AreEqual("Wednesday 10:00-12:15 Tuesday 13:30-14:30 Friday 13:30-14:30", time.ToString());
            UTCourseSectionTime.TryParseRawTime("MWF2", out time);
            Assert.AreEqual(3, time.MeetTimes.Count());
            UTCourseSectionTime.TryParseRawTime("WRF12", out time);
            Assert.AreEqual("Wednesday 12:00-13:00 Thursday 12:00-13:00 Friday 12:00-13:00", time.ToString());
        }

        [TestMethod]
        public void TryParseTimeStringBasics()
        {
            CourseSectionTime time;
            UTCourseSectionTime.TryParseTimeString("Tuesday 0:00-0:00", out time);
            Assert.AreEqual("Tuesday 0:00-0:00", time.ToString());
            UTCourseSectionTime.TryParseTimeString("Tuesday 10:00-13:00", out time);
            Assert.AreEqual("Tuesday 10:00-13:00", time.ToString());
            UTCourseSectionTime.TryParseTimeString("Tuesday 10:00-13:00 Monday 9:00-10:00", out time);
            Assert.AreEqual("Tuesday 10:00-13:00 Monday 9:00-10:00", time.ToString());
        }

        [TestMethod]
        public void TryParseTimeStringEng()
        {
            CourseSectionTime time;
            UTEngCourseSectionTime.TryParseRawTime("tue 0:00 0:00", out time);
            Assert.AreEqual("Tuesday 0:00-0:00", time.ToString());
            UTEngCourseSectionTime.TryParseRawTime("TUE 10:00 13:00", out time);
            Assert.AreEqual("Tuesday 10:00-13:00", time.ToString());
            UTEngCourseSectionTime.TryParseRawTime("Tue 10:00 13:00 Mon 9:00 10:00", out time);
            Assert.AreEqual("Tuesday 10:00-13:00 Monday 9:00-10:00", time.ToString());
            UTEngCourseSectionTime.TryParseRawTime("Tue 10:00 12:00 Mon 9:00 10:00", out time);
            Assert.AreEqual("Tuesday 10:00-12:00 Monday 9:00-10:00", time.ToString());
        }
    }
}
