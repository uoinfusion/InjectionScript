using Microsoft.VisualStudio.TestTools.UnitTesting;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class GotoTests
    {
        [TestMethod]
        public void Jump_forward() => TestSubrutine(2, "sub1", @"sub sub1()
var x = 1
goto label1
x = x + 1

label1:
x = x + 1

return x

end sub
");

        [TestMethod]
        public void Label_can_contain_dot() => TestSubrutine(2, "sub1", @"sub sub1()
var x = 1
goto label.1.
x = x + 1

label.1.:
x = x + 1

return x

end sub
");

        [TestMethod]
        public void Jump_backward() => TestSubrutine(333, "sub1", @"sub sub1()
var x = 1
label1:
if x > 1 then
    return 333
end if

x = x + 1
goto label1

return 111
end sub
");

        [TestMethod]
        public void Labels_are_case_insensitive()
        {
            TestSubrutine(2, "sub1", @"sub sub1()
var x = 1
goto LaBeL
x = x + 1

lAbEl:
x = x + 1

return x
end sub
");
        }
    }

}
