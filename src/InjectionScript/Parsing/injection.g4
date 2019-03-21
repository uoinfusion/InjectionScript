grammar injection;

file: fileSection*?;
fileSection: emptyLine | subrutine | globalVar;
globalVar: var;

emptyLine: NEWLINE;
subrutine: SUB subrutineName '(' parameters? ')' NEWLINE codeBlock? END_SUB (NEWLINE | EOF);
subrutineName: SYMBOL;
parameters:  parameterName (',' parameterName)*;
parameterName: SYMBOL;
codeBlock: statement+?;
statement: label | if | while | wend | break | repeat | until | var | dim | assignStatement
    | callStatement | emptyStatement | returnStatement | for | next | goto | missplacedEndif | incompleteWhile
    | emptyLine;

if: IF expression THEN NEWLINE codeBlock? else? END_IF (NEWLINE | EOF);
missplacedEndif: END_IF;
else: ELSE NEWLINE codeBlock?;
while: WHILE expression NEWLINE codeBlock? WEND NEWLINE;
incompleteWhile: WHILE expression NEWLINE;
wend: WEND NEWLINE;
break: BREAK;
repeat: REPEAT NEWLINE;
until: UNTIL expression NEWLINE;
for: FOR (assignment | forVarDef) TO expression step? NEWLINE;
forVarDef: VAR assignment;
step: STEP expression;
next: NEXT NEWLINE;
assignStatement: assignment NEWLINE;
callStatement: call NEWLINE;
emptyStatement: NEWLINE;
returnStatement: RETURN expression? NEWLINE;
goto: GOTO SYMBOL invalid=~(NEWLINE)* NEWLINE;
label: SYMBOL ':' NEWLINE;

var: VAR varDef (',' varDef)* (NEWLINE | EOF);
varDef: SYMBOL | assignment;
dim: DIM dimDef (',' dimDef)* (NEWLINE | EOF);
dimDef: SYMBOL '[' expression ']' dimDefAssignment?;
dimDefAssignment: '=' expression;

call: SYMBOL argumentList;
argumentList: '(' arguments? ')';
arguments: argument (',' argument)*;
argument: expression | literal;

assignment: lvalue '=' expression;
lvalue: SYMBOL | indexedSymbol;

expression: logicalOperand logicalOperation*;
logicalOperation: logicalOperator logicalOperand;
logicalOperator: OR | AND;
logicalOperand: comparativeOperand comparativeOperation*;

comparativeOperation: comparativeOperator comparativeOperand;
comparativeOperator: EQUAL | NOT_EQUAL | LESS_THAN | LESS_THAN_STRICT | MORE_THAN | MORE_THAN_STRICT;
comparativeOperand: additiveOperand additiveOperation*;

additiveOperation: additiveOperator additiveOperand;
additiveOperator: PLUS | MINUS;
additiveOperand: op1=signedOperand multiplicativeOperation*;

multiplicativeOperation: multiplicativeOperator signedOperand;
multiplicativeOperator: MULTIPLY | DIVIDE;

signedOperand: unaryOperator signedOperand | operand;
operand: call | subExpression | number | SYMBOL | literal | indexedSymbol;
subExpression: '(' expression ')' ;
unaryOperator: MINUS | NOT;
number: HEX_NUMBER | INT_NUMBER | DEC_NUMBER;

literal: DOUBLEQUOTED_LITERAL | SINGLEQUOTED_LITERAL;
indexedSymbol: SYMBOL '[' expression ']';

LineComment: ('#' | ';') ~[\r\n]* -> channel(HIDDEN);
END_SUB: [eE][nN][dD] WS* [sS][uU][bB];
SUB: [sS][uU][bB];
END_IF: [eE][nN][dD] WS* [iI][fF];
IF: [iI][fF];
ELSE: [eE][lL][sS][eE];
THEN: [tT][hH][eE][nN];
WHILE: [wW][hH][iI][lL][eE];
WEND: [wW][eE][nN][dD];
BREAK: [bB][rR][eE][aA][kK];
REPEAT: [rR][eE][pP][eE][aA][tT];
UNTIL: [uU][nN][tT][iI][lL];
VAR: [vV][aA][rR];
DIM: [dD][iI][mM];
RETURN: [rR][eE][tT][uU][rR][nN];
FOR: [fF][oO][rR];
GOTO: [gG][oO][tT][oO];
TO: [tT][oO];
NEXT: [nN][eE][xX][tT];
STEP: [sS][tT][eE][pP];

PLUS: '+' ;
MINUS: '-' ;
MULTIPLY: '*';
DIVIDE: '/';
NOT_EQUAL: '<>';
MORE_THAN: '>=';
LESS_THAN: '<=';
MORE_THAN_STRICT: '>';
LESS_THAN_STRICT: '<';
EQUAL: '==';
OR: [oO][rR] | '||';
AND: [aA][nN][dD] | '&&';
NOT: [nN][oO][tT];

SYMBOL: VALID_SYMBOL_START VALID_SYMBOL_CHAR*;
INT_NUMBER: ('0'..'9')+;
DEC_NUMBER: ('0'..'9')+ '.' ('0'..'9')+;
HEX_NUMBER: '0x' HEX_DIGIT* ;
NEWLINE: ('\r'? '\n');
WS: (' '|'\r'|'\n'|'\t') -> channel(HIDDEN);
DOUBLEQUOTED_LITERAL: '"' ~('"')*? '"';
SINGLEQUOTED_LITERAL: '\'' ~('\'')*? '\'';

fragment VALID_SYMBOL_START: ('a' .. 'z') | ('A' .. 'Z') | '_';
fragment VALID_SYMBOL_CHAR: VALID_SYMBOL_START | DEC_DIGIT | '.';
fragment DEC_DIGIT : ('0' .. '9');
fragment HEX_DIGIT : DEC_DIGIT | ('a' .. 'f') | ('A' .. 'F');
