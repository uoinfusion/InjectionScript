# Abnormal program termination
sub dim_can_be_returned_from_subrutine()
   var x
   x = dim_can_be_returned_from_subrutine_sub1()
   
   tst_assert_num(321, x[6], "dim_can_be_returned_from_subrutine")      
end sub

sub dim_can_be_returned_from_subrutine_sub1()
   dim y[10]
   
   y[6] = 321
   
   return y
end sub
