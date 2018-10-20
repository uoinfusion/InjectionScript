sub dim_error_when_variable_limit()
    var x = 10
    # Bad operation for this type of variable - y
    dim y[x]
    y[6] = 125
end sub
