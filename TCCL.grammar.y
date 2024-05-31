%namespace ASTBuilder
%partial
%parsertype TCCLParser
%visibility internal
%tokentype Token
%YYSTYPE AbstractNode


%start CompilationUnit

%token AND ASTERISK BANG BOOLEAN CLASS
%token COLON COMMA ELSE EQUALS HAT
%token IDENTIFIER IF INSTANCEOF INT INT_NUMBER
%token LBRACE LBRACKET LPAREN MINUSOP
%token NEW NULL OP_EQ OP_GE OP_GT
%token OP_LAND OP_LE OP_LOR OP_LT OP_NE
%token PERCENT PERIOD PIPE PLUSOP PRIVATE
%token PUBLIC QUESTION RBRACE RBRACKET RETURN
%token RPAREN RSLASH SEMICOLON STATIC STRING_LITERAL
%token SUPER STRUCT THIS TILDE VOID WHILE
%token STRING


%right EQUALS
%left  OP_LOR
%left  OP_LAND
%left  PIPE
%left  HAT
%left  AND
%left  OP_EQ, OP_NE
%left  OP_GT, OP_LT, OP_LE, OP_GE
%left  PLUSOP, MINUSOP
%left  ASTERISK, RSLASH, PERCENT
%left  UNARY 

%%

CompilationUnit     :   ClassDeclaration                                            { $$ = new CompilationUnit($1);} 
                    ;
ClassDeclaration    :   Modifiers CLASS IDENTIFIER ClassBody                        { $$ = new ClassDeclaration($1, $3, $4);} 
                    ;

Modifiers           :   PUBLIC                              { $$ = new Modifiers(ModifierType.PUBLIC);}
                    |   PRIVATE                             { $$ = new Modifiers(ModifierType.PRIVATE);}
                    |   STATIC                              { $$ = new Modifiers(ModifierType.STATIC);}
                    |   Modifiers PUBLIC                    { $1.makeSibling(new Modifiers(ModifierType.PUBLIC)); }
                    |   Modifiers PRIVATE                   { $1.makeSibling(new Modifiers(ModifierType.PRIVATE)); }
                    |   Modifiers STATIC                    { $1.makeSibling(new Modifiers(ModifierType.STATIC)); }
                    ;

ClassBody           :   LBRACE MemberDeclarations RBRACE    { $$ = new ClassBody($2);}
                    |   LBRACE RBRACE                       { $$ = new Identifier("Not Implemented: empty Class Body");}
                    ;

MemberDeclarations  :   MemberDeclaration                    { $$ = $1; }
                    |   MemberDeclarations MemberDeclaration { $1.makeSibling($2); $$ = $1;}
                    ;

MemberDeclaration   :   FieldDeclaration SEMICOLON           { $$ = new Identifier("Not Implemented: FieldDeclaration");}
                    |   MethodDeclaration                    { $$ = $1; }             
                    |   ConstructorDeclaration                     
                    |   StaticInitializer                   
                    |   StructDeclaration                   
                    ;

MethodDeclaration   :   Modifiers TypeSpecifier MethodSignature MethodBody          {$$ = new MethodDeclaration($1, $2, $3, $4); }
                    ;

MethodSignature     :   IDENTIFIER LPAREN ParameterList RPAREN                      { $$ = new MethodSignature($1, $3); }
                    |   IDENTIFIER LPAREN RPAREN                                    { $$ = new MethodSignature($1); }
                    ;

ParameterList       :   Parameter                                                   { $$ = new ParameterList($1); }
                    |   ParameterList COMMA Parameter                               { $1.adoptChildren($3); $$ = $1; }  
                    ;

Parameter           :   TypeSpecifier IDENTIFIER                                    { $$ = new Parameter($1, $2); }
                    ;

MethodBody          :   Block                                   { $$ = new Block($1); }
                    ;


StructDeclaration   :   Modifiers STRUCT IDENTIFIER ClassBody   { $$ = new Struct($1, $3, $4);}
                    ;

FieldDeclaration    :   Modifiers TypeSpecifier FieldNames      { }
                            ;

TypeSpecifier               :   TypeName                        { $$ = $1; }
                            |   ArraySpecifier                                              
                            ;

TypeName                    :   PrimitiveType                   { $$ = $1; }
                            |   QualifiedName                                               
                            ;

ArraySpecifier              :   TypeName LBRACKET RBRACKET      
                            ;
                            
PrimitiveType               :   BOOLEAN                         { $$ = new PrimitiveType(EnumPrimitiveType.BOOLEAN); }
                            |   INT                             { $$ = new PrimitiveType(EnumPrimitiveType.INT); }
                            |   VOID                            { $$ = new PrimitiveType(EnumPrimitiveType.VOID); }
                            |   STRING                          { $$ = new PrimitiveType(EnumPrimitiveType.STRING); }
                            ;

FieldNames                  :   NameList
                            ;
                            
NameList                    :   IDENTIFIER                                                                      
                            |   NameList COMMA IDENTIFIER       { $1.makeSibling($3); $$ = $1;}
                            ;


ConstructorDeclaration      :   Modifiers MethodSignature Block                             
                            ;

StaticInitializer           :   STATIC Block                                                
                            ;
        
Block                       :   LBRACE LocalItems RBRACE              { $$ = $2;} 
                            |   LBRACE RBRACE                         { $$ =  new Identifier("Not Implemented: Empty Block"); }
                            ;

LocalItems	   	            :   LocalItem						      { $$ = $1;}                            
                            |   LocalItems LocalItem                  { $1.makeSibling($2); $$ = $1;}
                            ;    

LocalItem                   :   LocalVariableDeclaration			 			       
                            |   Statement                            { $$ = $1;}              
                            ;

LocalVariableDeclaration	:   TypeSpecifier LocalVariableNames SEMICOLON    { $$ = new LocalDecl($1, $2); }
                            |   StructDeclaration                                  
                            ;

LocalVariableNames          :   NameList                             
                            ;

                            
Statement                   :   EmptyStatement                      	                        
                            |   ExpressionStatement                 	                 
                            |   SelectionStatement                	                      
                            |   IterationStatement                                              
                            |   ReturnStatement                                          
                            |   Block                               { $$ = new Block($1); }               
                            ;

EmptyStatement              :   SEMICOLON                                                   
                            ;

ExpressionStatement         :   Expression  SEMICOLON               { $$ = $1;}                                                       
                            ;

SelectionStatement          :   IF LPAREN Expression RPAREN Statement ELSE Statement    { $$ = new SelectionStatement($3, $5, $7, SelectionType.IF_ELSE); }
                            |   IF LPAREN Expression RPAREN Statement                   { $$ = new SelectionStatement($3, $5, SelectionType.IF); }
                            ;


IterationStatement          :   WHILE LPAREN Expression RPAREN Statement                { $$ = new IterationStatement($3, $5, IterationType.WHILE); }
                            ;

ReturnStatement             :   RETURN Expression SEMICOLON                             { $$ = new ReturnStatement($2); }
                            |   RETURN            SEMICOLON
                            ;

ArgumentList                :   Expression
                            |   ArgumentList COMMA Expression       { $1.makeSibling($3); }
                            ;


// TODO
Expression                  :   QualifiedName EQUALS Expression     { $$ = new Expression($1, ExprKind.EQUALS, $3); }
   /* short-circuit OR  */  |   Expression OP_LOR Expression        { $$ = new Expression($1, ExprKind.OP_LOR, $3); }       
   /* short-circuit AND */  |   Expression OP_LAND Expression       { $$ = new Expression($1, ExprKind.OP_LAND, $3); }
                            |   Expression PIPE Expression          { $$ = new Expression($1, ExprKind.PIPE, $3); }
                            |   Expression HAT Expression           { $$ = new Expression($1, ExprKind.HAT, $3); }
                            |   Expression AND Expression           { $$ = new Expression($1, ExprKind.AND, $3); }
                            |   Expression OP_EQ Expression         { $$ = new Expression($1, ExprKind.OP_EQ, $3); }
                            |   Expression OP_NE Expression         { $$ = new Expression($1, ExprKind.OP_NE, $3); }
                            |   Expression OP_GT Expression         { $$ = new Expression($1, ExprKind.OP_GT, $3); }
                            |   Expression OP_LT Expression         { $$ = new Expression($1, ExprKind.OP_LT, $3); }
                            |   Expression OP_LE Expression         { $$ = new Expression($1, ExprKind.OP_LE, $3); }
                            |   Expression OP_GE Expression         { $$ = new Expression($1, ExprKind.OP_GE, $3); }
                            |   Expression PLUSOP Expression		{ $$ = new Expression($1, ExprKind.PLUSOP, $3); }
                            |   Expression MINUSOP Expression		{ $$ = new Expression($1, ExprKind.MINUSOP, $3); }
                            |   Expression ASTERISK Expression      { $$ = new Expression($1, ExprKind.ASTERISK, $3); }
                            |   Expression RSLASH Expression        { $$ = new Expression($1, ExprKind.RSLASH, $3); }
                            |   Expression PERCENT Expression   /* remainder */
                            |   ArithmeticUnaryOperator Expression  %prec UNARY
                            |   PrimaryExpression               { $$ = $1; }
                            ;

ArithmeticUnaryOperator     :   PLUSOP
                            |   MINUSOP
                            ;
                            
PrimaryExpression           :   QualifiedName                   { $$ = $1;}   
                            |   NotJustName                     { $$ = $1;}
                            ;

NotJustName                 :   SpecialName                     { $$ = $1;}
                            |   ComplexPrimary                  { $$ = $1;}
                            ;

ComplexPrimary              :   LPAREN Expression RPAREN        { $$ = $2;}
                            |   ComplexPrimaryNoParenthesis     { $$ = $1;}
                            ;

ComplexPrimaryNoParenthesis :   STRING_LITERAL                  { $$ = $1;}
                            |   Number                          { $$ = $1;}
                            |   FieldAccess                     { $$ = $1;}   
                            |   MethodCall                      { $$ = $1;}    
                            ;

FieldAccess                 :   NotJustName PERIOD IDENTIFIER   { $$ = new Identifier("Not Implemented: FieldAccess");}   
                            ;       

MethodCall                  :   MethodReference LPAREN ArgumentList RPAREN     { $$ =  new MethodCall($1, $3); }
                            |   MethodReference LPAREN RPAREN                   { $$ = new MethodCall($1);}
                            ;

MethodReference             :   ComplexPrimaryNoParenthesis     { $$ = $1;}
                            |   QualifiedName                   { $$ = $1;}
                            |   SpecialName                     { $$ = $1;}
                            ;

QualifiedName               :   IDENTIFIER                                                  
                            |   QualifiedName PERIOD IDENTIFIER  { $1.adoptChildren($3); }       
                            ;

SpecialName                 :   THIS  
                            |   NULL
                            ;

Number                      :   INT_NUMBER                      { $$ = $1; }
                            ;

%%

