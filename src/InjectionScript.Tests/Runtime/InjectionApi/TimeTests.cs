using FluentAssertions;
using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Tests.Runtime.InjectionApi
{
    [TestClass]
    public class TimeTests
    {
        private InjectionScript.Runtime.InjectionApi injectionApi;
        private TestTimeSource timeSource;

        [TestInitialize]
        public void Initialize()
        {
            timeSource = new TestTimeSource();
            injectionApi = new InjectionScript.Runtime.InjectionApi(null, new Metadata(), new Globals(), timeSource,
                new Paths(() => string.Empty), new Objects());
        }

        [TestMethod]
        public void UO_Timer_returns_tenth_of_seconds_since_start_rounding_down()
        {
            timeSource.StartTime = new DateTime(2019, 1, 2, 7, 29, 35, 523);
            timeSource.Now = timeSource.StartTime.AddMilliseconds(399);

            injectionApi.UO.Timer().Should().Be(3);
        }

        [TestMethod]
        public void Now_returns_miliseconds_since_start_rounding_down()
        {
            timeSource.StartTime = new DateTime(2019, 1, 2, 7, 29, 35, 523);
            timeSource.Now = timeSource.StartTime.AddMilliseconds(399);

            injectionApi.Now().Should().Be(399);
        }

        [TestMethod]
        public void Date_return_date_encoded_to_int()
        {
            timeSource.StartTime = new DateTime(2019, 1, 2, 7, 29, 35, 523);
            timeSource.Now = timeSource.StartTime;

            injectionApi.UO.Date().Should().Be(190102);
        }

        [TestMethod]
        public void Date_return_time_encoded_to_int()
        {
            timeSource.StartTime = new DateTime(2019, 1, 2, 7, 29, 35, 523);
            timeSource.Now = timeSource.StartTime;

            injectionApi.UO.Time().Should().Be(72935);
        }
    }
}
