sub unary_operators()
   tst_assert_num(-1, -1, "-1")
   tst_assert_num(1, --1, "--1")
   tst_assert_num(0, not 1, "not 1")
   tst_assert_num(0, not 5, "not 5")
   tst_assert_num(1, not not 5, "not not 5")
   tst_assert_num(1, not 0, "not 0")
   tst_assert_num(0, not -1, "not -1")
end sub

sub binary_operators()
   tst_assert_num(2, 1 + 1, "1 + 1")
   tst_assert_num(3, 1 + 1 + 1, "1 + 1 + 1")
   tst_assert_num(4, 5 - 1, "5 - 1")
   
   tst_assert_num(0, 0 && 1, "0 && 1")
   tst_assert_num(1, 2 && 5, "2 && 5")
   tst_assert_num(1, 1 && 1, "1 && 1")
   tst_assert_num(0, 0 and 1, "0 and 1")
   tst_assert_num(1, 1 and 1, "1 and 1")
   
   tst_assert_num(0, 0 || 0, "0 || 0")
   tst_assert_num(1, 1 || 0, "1 || 0")
   tst_assert_num(0, 0 or 0, "0 or 0")
   tst_assert_num(1, 1 or 0, "1 or 0")
   
   tst_assert_num(1, 5 == 5, "5 == 5")
   tst_assert_num(0, 5 == 6, "5 == 6")
   tst_assert_num(0, 5 <> 5, "5 <> 5")
   tst_assert_num(1, 5 <> 6, "5 <> 6")
end sub

sub operators_precedence()
   tst_assert_num(7, 1 + 2 * 3, "1 + 2 * 3; multiplicative precede aditive")
   tst_assert_num(7, 1 + (2 * 3), "1 + (2 * 3)")
   tst_assert_num(9, (1 + 2) * 3, "(1 + 2) * 3")
   
   tst_assert_num(1, 5 - 1 && 1, "5 - 1 && 1; additive precede logical")
   tst_assert_num(1, 5 - 1 || 1, "5 - 1 || 1; additive precede logical")
   
   tst_assert_num(4, 5 - (1 && 1), "5 - (1 && 1)")
   tst_assert_num(4, 5 - (1 || 1), "5 - (1 || 1)")
   
   tst_assert_num(1, 6 / 2 || 1, "6 / 2 || 1; multiplicative precede logical")
   tst_assert_num(1, 6 / 2 || 1, "6 / 2 || 1")
   
   tst_assert_num(1, 1 + 0 == 0 + 1, "1 + 0 == 0 + 1; additive precede comparative")
   tst_assert_num(3, 1 + (0 == 0) + 1, "1 + (0 == 0) + 1")
   
   tst_assert_num(1, 5 * 1 == 5, "5 * 1 == 5; additive precede multiplicative")
   tst_assert_num(0, 5 * (1 == 5), "5 * (1 == 5)")
end sub

sub for_out_of_range_without_next()
   var i, x = 1
   for i = 1 to 1
      x = x + 1
      tst_assert_num(2, x, "for_out_of_range_without_next")
end sub

sub for_out_of_range()
   var i, x = 1
   for i = 1 to 1
      x = x + 1
   next
   
   tst_assert_num(2, x, "for_out_of_range")
end sub

sub for_jumping_to_if()
   var i, x = 1
   if 1 then
      for i = 1 to 2
         x = x + 1
      end if
   next
   
   tst_assert_num(3, x, "for_jumping_to_if")
end sub

sub for_jumping_to_false_if()
   var i, x = 1
   if x then
      x = 0
      for i = 0 to 10
      end if
   next
   
   return i
   
   tst_assert_num(10, x, "for_jumping_to_false_if")
end sub

sub repeat_without_until()
   var i = 0
   repeat
      tst_assert_num(0, i, "repeat_without_until")
end sub

sub until_without_repeat()
   var i = 0
until i <> 0
tst_assert_num(0, i, "repeat_without_until")
end sub

sub false_while_with_two_wends()
   var i = 0
   while 0
      if 1 then
      wend
      i = 1
   end if
wend
tst_assert_num(1, i, "false_while_with_two_wends")
end sub

sub dim_with_constant_limit()
   dim x[10]
   x[5] = 123
   
   tst_assert_num(123, x[5], "dim_with_constant_limit")
end sub

sub dim_multiple_on_one_line_with_constant_limit()
   dim x[10], y[10]
   x[5] = 123
   y[6] = 125
   
   tst_assert_num(123, x[5], "dim_with_constant_limit - x[5]")
   tst_assert_num(125, y[6], "dim_with_constant_limit - y[6]")
end sub

sub dim_zero_index_access()
   dim x[10]
   
   x[0] = 123
   tst_assert_num(123, x[0], "dim_zero_index_access")
end sub

sub dim_limit_index_access()
   dim x[10]
   
   x[10] = 123
   tst_assert_num(123, x[10], "dim_limit_index_access")
end sub

sub dim_assignment_without_index()
   dim x[10]
   
   x = 123
   tst_assert_num(123, x, "dim_assignment_without_index - x")    
end sub

sub globals()
    UO.SetGlobal("globname", "some text")
    tst_assert_str("some text", UO.GetGlobal("globname"), "globals")
    tst_assert_str("N/A", UO.GetGlobal("nonexisting"), "globals - empty")
end sub

sub call_tests()
   tst_assert_num(5, call_tests_sub1(2, 3, 5), "call_tests")
   tst_assert_num(5, call_tests_sub1(2), "call_tests")
end sub

sub call_tests_sub1(a, b)
   return a + b
end sub
sub call_tests_sub1(a)
   return a * 10
end sub

sub goto_backward()
   var x = 0

label1:
   x = x + 1
   if (x == 2) then
      tst_assert_num(2, x, "goto_backward")
      return
   end if
   goto label1
end sub

sub goto_forward()
   var x = 0
   goto label1
   x = 3

label1:
   tst_assert_num(0, x, "goto_forward")
end sub

sub string_expressions_sum()
   tst_assert_str("asdfqwer", "asdf"+"qwer", "string_expressions_sum - 2 string")
   tst_assert_str("", ""+"", "string_expressions_sum - 2 empty strings")
   tst_assert_str("asdfqwerzxcv", "asdf"+"qwer"+"zxcv", "string_expressions_sum - 3 string")
end sub

sub string_expressions_str()
   tst_assert_str("asdf1", "asdf"+str(1), "string_expressions_str - sum with int number")
   tst_assert_str("asdf6844", "asdf"+str(0x1abc), "string_expressions_str - sum with hex number")
   tst_assert_str("asdf4.99", "asdf"+str(4.99), "string_expressions_str - sum with dec number")
end sub

sub string_expressions_comparison()
   tst_assert_num(0, "asdf"=="qwer", "string_expressions_comparison - equal - false")
   tst_assert_num(1, "asdf"=="asdf", "string_expressions_comparison - equal - true")
   tst_assert_num(0, "ASDF"=="asdf", "string_expressions_comparison - equal - false, case sensitivity")
   tst_assert_num(1, "asdf"<>"qwer", "string_expressions_comparison - nonequal - true")
   tst_assert_num(0, "asdf"<>"asdf", "string_expressions_comparison - nonequal - false")
   tst_assert_num(1, "ASDF"<>"asdf", "string_expressions_comparison - nonequal - false, case sensitivity")
end sub

sub string_expression_val()
   tst_assert_num(123, val("123"), "string_expression_val - int")
   tst_assert_num(4.99, val("4.99"), "string_expression_val - dec")
   tst_assert_num(0, val("asdf"), "string_expression_val - invalid format")
   tst_assert_num(0, val("abc"), "string_expression_val - invalid format (but could be a hex number)")
   tst_assert_num(0, val("0x123"), "string_expression_val - invalid format (but could be a hex number prefixed with 0x)")
end sub

sub string_expression_len()
   tst_assert_num(4, len("asdf"), "string_expression_len")
   tst_assert_num(0, len(""), "string_expression_len - empty string")
   tst_assert_num(0, len(123), "string_expression_len - int")
   tst_assert_num(0, len(4.99), "string_expression_len - dec")
end sub

sub tst_assert_num(expected, actual, description)
   if (expected <> actual) then
      UO.Print("FAILURE " + description + " - " + "expected: " + str(expected) + ", actual: " + str(actual))
   else
      UO.Print("SUCCESS " + description)        
   end if
end sub

sub tst_assert_str(expected, actual, description)
   if (expected <> actual) then
      UO.Print("FAILURE " + description + " - " + "expected: " + expected + ", actual: " + actual)
   else
      UO.Print("SUCCESS " + description)        
   end if
end sub