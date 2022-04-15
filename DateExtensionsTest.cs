using System;
using System.Collections.Generic;
using Doctors.Service.Gen.ViewModels.Output;
using Doctors.Service.Models;
using Doctors.Service.ViewModels.Output;
using TimeZoneConverter;
using Xunit;

namespace Doctors.Service.Extensions
{
    public class DateExtensionsTest
    {
        private const string Zone = "Pacific Standard Time";
        private const string DateWithDayLightSaving = "2022-03-13 03:00";
        private const string DateWithoutDayLightSaving = "2022-11-06 03:00";

        [Theory]
        [InlineData(Zone, DateWithoutDayLightSaving, "-08:00")]
        [InlineData(Zone, DateWithDayLightSaving,    "-07:00")]
        public void ShouldGetTimeZoneOffset_RespectDayLightSaving(string zone, string dateTime, string offset)
        {
            var date = DateTime.Parse(dateTime);
            var expected = TimeSpan.Parse(offset);

            var actual = TZConvert.GetTimeZoneInfo(zone).GetUtcOffset(date);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldChangeDateTimeOffset()
        {
            var dateTime = DateTime.Parse(DateWithoutDayLightSaving);

            var irvineTime = dateTime.ConvertByTimeZone(Zone);
            var actual = irvineTime.DateTime.Subtract(dateTime);
            var expected = TimeSpan.Parse("-08:00");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldSetTimeForDateTime()
        {
            var today = DateTime.Now;
            var time = TimeSpan.Parse("08:00");

            var combined = today.SetTime(time);
            Assert.True(TimeSpan.Equals(time, combined.TimeOfDay));
        }

        [Fact]
        public void ShouldSetTimeForDateTimeOffset()
        {
            var today = DateTimeOffset.Now;
            var time = TimeSpan.Parse("08:00");

            var combined = today.SetTime(time);
            Assert.True(TimeSpan.Equals(time, combined.TimeOfDay));
        }
        
        [Fact]
        public void ShouldGetTimeOffsetOfZone()
        {
            var tz = "Asia/Tehran";
            var expectedOffset = new TimeSpan(3, 30, 0);
            var actual = DateExtensions.GetTimeZoneOffset(tz, false);

            Assert.Equal(actual, expectedOffset);
        }

        [Theory]
        [InlineData("UTC", "00:00:00")]
        [InlineData("Pacific Standard Time", "-8:00")]
        [InlineData("Central Standard Time", "-6:00")]
        public void Test_GetTimeZoneOffset(string zone, string offsetToUtc)
        {
            var actual = DateExtensions.GetTimeZoneOffset(zone, false);
            var expected = TimeSpan.Parse(offsetToUtc);

            Assert.Equal(actual, expected);
        }

        
        /// <summary>
        /// The test is used to 
        /// </summary>
        [Fact]
        public void Test_Convert_Extension_Method()
        {
            var date1 = DateTimeOffset.Parse("3/10/2022 22:00:00 -08:00");
            var startTime = new TimeSpan(18, 30, 0);
            var duration = TimeSpan.FromMinutes(30);
            var endTime = startTime + duration;

            var flatAvailableDate = new FlatAvailableDateModel()
            {
                Date = date1,
                DoctorOffice = new ClinicOutputViewModel() {ClinicId = 1},
                FreeTime = new FreeTimeOutputViewModel()
                {
                    AvailableTime = startTime,
                    EndTime = endTime
                }
            };

            new List<FlatAvailableDateModel>() {flatAvailableDate}
                .ConvertTo("Pacific Standard Time", "Iran Standard Time");


            var expectedStartTime = new TimeSpan(30, 0, 0);
            var expectedEndTime = expectedStartTime + duration;

            Assert.Equal(expectedStartTime.Hours, flatAvailableDate.FreeTime.AvailableTime.Hours);
            Assert.Equal(expectedEndTime.Hours, flatAvailableDate.FreeTime.EndTime.Hours);

            if (!DateTime.Equals(date1.Date, flatAvailableDate.Date.Date))
                Assert.Equal(1, Math.Abs(date1.Date.Subtract(flatAvailableDate.Date.Date).Days));
        }

        [Theory]
        [InlineData("Iran Standard Time", "UTC", "03:30")]
        [InlineData("UTC", "Iran Standard Time", "-3:30")]
        [InlineData("Central Standard Time", "UTC", "-5:00")]
        [InlineData("Central Standard Time", "Pacific Standard Time", "02:00")]
        public void ShouldCalculateTimeZoneDifferenceBetweenTwoZones(string sourceZone, string targetZone, string difference)
        {
            var expectedOffset = TimeSpan.Parse(difference);
            var actualOffset = DateExtensions.GetTimeZoneOffset(sourceZone, targetZone);
            Assert.Equal(expectedOffset, actualOffset);
        }
    }
}