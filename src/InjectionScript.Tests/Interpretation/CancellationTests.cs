using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static InjectionScript.Tests.TestHelpers;
using System.Threading.Tasks;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class CancellationTests
    {
        [TestMethod]
        public void Can_cancel_endless_loop_without_call_to_cancellable_api()
        {
            var tokenSource = new CancellationTokenSource();
            var task = RunSubrutine("test", () => tokenSource.Token, @"
sub test()
    while (1)   
    wend
end sub");

            tokenSource.Cancel();
            Action testedAction = () => task.Wait(1000);
            testedAction.Should().Throw<OperationCanceledException>();

            task.Status.Should().Be(TaskStatus.Faulted);
        }
    }
}
