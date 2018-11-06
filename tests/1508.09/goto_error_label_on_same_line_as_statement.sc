sub goto_error_label_on_same_line_as_statement()
var x = 0
# parse error
label1: x = x + 1
goto label1
end sub