sub dim_error_when_access_after_assignment_without_index()
   dim x[10]

   x[5] = 1
   # Variable used as an array - X   
   x = 123
   return x[5]    
end sub
