using Microsoft.VisualStudio.TestTools.UnitTesting;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class CommentTests
    {
        [TestMethod]
        public void Can_load_file_with_newline_before_subrutine_definition()
        {
            TestSubrutine(123, "sub1", @"

sub sub1()
    return 123
end sub
");
        }

        [TestMethod]
        public void Can_load_file_with_whitespace_before_subrutine_definition()
        {
            TestSubrutine(123, "sub1", @"    sub sub1()
    return 123
end sub
");
        }

        [TestMethod]
        public void Can_load_file_with_comments_before_subrutine_definition()
        {
            TestSubrutine(123, "sub1", @"# comment 1
; comment 2
sub sub1()
    return 123
end sub
");
        }

        [TestMethod]
        public void Can_load_file_with_comments_after_subrutine_definition()
        {
            TestSubrutine(123, "sub1", @"sub sub1()
    return 123
end sub
# comment 1
; comment 2
");
        }

        [TestMethod]
        public void File_can_contain_comment_between_subs()
        {
            TestSubrutine(123, "sub2", @"
sub sub1()
   sub2()
end sub
#
sub sub2()
    return 123
end sub");
        }

        [TestMethod]
        public void Sub_can_contain_multiple_comments()
        {
            TestSubrutine(123, "sub1", @"
sub sub1()
    #
    ;
    return 123
end sub
");
        }
    }
}
