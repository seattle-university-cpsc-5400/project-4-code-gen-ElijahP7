using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ASTBuilder
{
    public class SemanticsVisitor
    {
        //Uncomment the next line once you include your symbol table implementation
        private Symtab table;

        // used to produce a readable trace when desired
        protected String prefix = "";

        private string currentModifier;
        private string prevModifier;
        List<string> nameList;
        Dictionary<string, TypeDescriptor> parameters;
        private TypeAttributes type;


        public SemanticsVisitor(Symtab symTable, String oldPrefix = "")
        // Parameter oldPrefix allows creation of this visitor during a traversal
        {
            table = symTable;
            prefix = oldPrefix + "   ";
            parameters = new Dictionary<string, TypeDescriptor>();
            nameList = new List<string>();

        }

        public virtual string ClassName()
        {
            return this.GetType().Name;
        }

        private bool TraceFlag = true;  // Set to true or false to control trace

        private void Trace(AbstractNode node, string suffix = "")
        {
            if (TraceFlag)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.prefix + ">> In VisitNode for " + node.ClassName() + " " + suffix);
                Console.ResetColor();
            }
        }

        // The following are not needed now; they follow implementation from Chapter 8

        //protected static MethodAttributes currentMethod = null;
        //protected void SetCurrentMethod(MethodAttributes m)
        //{
        //    currentMethod = m;
        //}
        //protected MethodAttributes GetCurrentMethod()
        //{
        //    return currentMethod;
        //}

        //protected static ClassAttributes currentClass = null;

        //protected void SetCurrentClass(ClassAttributes c)
        //{
        //    currentClass = c;
        //}
        //protected ClassAttributes GetCurrentClass()
        //{
        //    return currentClass;
        //}

        // Call this method to begin the semantic checking process
        public void CheckSemantics(dynamic node)
        {
            if (node == null) return;

            if (TraceFlag)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.prefix + "Started Check Semantics for " + node.ClassName());
                Console.ResetColor();
            }
            VisitNode(node);
        }

        // This version of VisitNode is invoked if a more specialized one is not defined below.
        public virtual void VisitNode(AbstractNode node)
        {
            if (TraceFlag)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.prefix + ">> In general VisitNode for " + node.ClassName());
                Console.ResetColor();
            }
            VisitChildren(node);
        }
        
        public virtual void VisitChildren (AbstractNode node)
        {
            // Nothing to do if node has no children
            if (node.Child == null) return;

            if (TraceFlag)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.prefix + " └─In VistChildren for " + node.ClassName());
                Console.ResetColor();
            }

            // Increase prefix while visiting children
            String oldPrefix = this.prefix;
            this.prefix += "   ";

            foreach (dynamic child in node.Children())
            {
                VisitNode(child);
            };

            this.prefix = oldPrefix;
        }

        //Starting Node of an AST
        public void VisitNode(CompilationUnit node)
        {
            Trace(node);

            VisitChildren(node);
        }

        // This method is needed to implement special handling of certain expressions
        public virtual bool VisitNode(Expression node)
        {
            dynamic exprLhs = node.Child;
            dynamic exprRhs = node.Child.Sib;
            TypeDescriptor lhsType = null;
            TypeDescriptor rhsType = null;

            if (!IsDeclared(exprLhs))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.prefix + ">> ERROR: Variable Undeclared");
                Console.ResetColor();
                ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                ErrorAttributes errorAttr = new ErrorAttributes("Variable Undeclared", errorType);
                table.Enter(exprLhs.Name.ToString(), errorAttr);
                return false;
            }
            if (!IsDeclared(exprLhs))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.prefix + ">> ERROR: Variable Undeclared");
                Console.ResetColor();
                ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                ErrorAttributes errorAttr = new ErrorAttributes("Variable Undeclared", errorType);
                table.Enter(exprRhs.Name.ToString(), errorAttr);
                return false;
            }
            else
            {
                if (node.exprKind == ExprKind.EQUALS)
                {
                    lhsType = GetExpressionType(exprLhs).Type;
                    if (exprRhs.whatAmI() == "ASTBuilder.Expression")
                        VisitNode(exprRhs);
                    else
                    {
                        rhsType = GetExpressionType(exprRhs).Type;
                        if (lhsType.ToString() != rhsType.ToString())
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(this.prefix + ">> ERROR: Can't Assign " + lhsType.ToString() + " to " + rhsType.ToString());
                            Console.ResetColor();
                            ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                            ErrorAttributes errorAttr = new ErrorAttributes("Can't Assign " +
                                lhsType.ToString() + " to " + rhsType.ToString(), errorType);
                            table.Enter(node.exprKind.ToString(), errorAttr);
                            return false;
                        }
                    }
                }
                else if (node.exprKind == ExprKind.PLUSOP)
                {
                    if (exprLhs.whatAmI() == "ASTBuilder.Expression")
                        VisitNode(exprLhs);
                    else if (exprRhs.whatAmI() == "ASTBuilder.Expression")
                        VisitNode(exprRhs);
                    else
                    {
                        lhsType = GetExpressionType(exprLhs).Type;
                        rhsType = GetExpressionType(exprRhs).Type;

                        if (lhsType.ToString() != "ASTBuilder.IntegerTypeDescriptor" &&
                            rhsType.ToString() != "ASTBuilder.IntegerTypeDescriptor")
                        {

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(this.prefix + ">> ERROR: Can't Apply Operator to " + lhsType.ToString() + " and " + rhsType.ToString());
                            Console.ResetColor();
                            ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                            ErrorAttributes errorAttr = new ErrorAttributes("Can't Apply Operator to " +
                                lhsType.ToString() + " and " + rhsType.ToString(), errorType);
                            table.Enter(node.exprKind.ToString(), errorAttr);
                            return false;
                        }
                        if (lhsType.ToString() != "ASTBuilder.IntegerTypeDescriptor" &&
                            lhsType.ToString() != "ASTBuilder.StringTypeDescriptor")
                        {

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(this.prefix + ">> ERROR: Can't Apply Operator to " + lhsType.ToString() + " and " + rhsType.ToString());
                            Console.ResetColor();
                            ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                            ErrorAttributes errorAttr = new ErrorAttributes("Can't Apply Operator to " +
                                lhsType.ToString() + " and " + rhsType.ToString(), errorType);
                            table.Enter(node.exprKind.ToString(), errorAttr);
                            return false;
                        }
                    }
                }
                else if (node.exprKind == ExprKind.MINUSOP || node.exprKind == ExprKind.ASTERISK ||
                    node.exprKind == ExprKind.RSLASH || node.exprKind == ExprKind.PERCENT)
                {
                    if (exprLhs.whatAmI() == "ASTBuilder.Expression")
                        VisitNode(exprLhs);
                    else if (exprRhs.whatAmI() == "ASTBuilder.Expression")
                        VisitNode(exprRhs);
                    else
                    {
                        lhsType = GetExpressionType(exprLhs).Type;
                        rhsType = GetExpressionType(exprRhs).Type;
                        if (lhsType.ToString() != "ASTBuilder.IntegerTypeDescriptor" ||
                            rhsType.ToString() != "ASTBuilder.IntegerTypeDescriptor")
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(this.prefix + ">> ERROR: Can't Apply Operator to " + lhsType.ToString() + " and " + rhsType.ToString());
                            Console.ResetColor();
                            ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                            ErrorAttributes errorAttr = new ErrorAttributes("Can't Apply Operator to " +
                                lhsType.ToString() + " and " + rhsType.ToString(), errorType);
                            table.Enter(node.exprKind.ToString(), errorAttr);
                            return false;
                        }
                    }
                }
                else
                {
                    if (exprLhs.whatAmI() == "ASTBuilder.Expression")
                        VisitNode(exprLhs);
                    else if (exprRhs.whatAmI() == "ASTBuilder.Expression")
                        VisitNode(exprRhs);
                    else
                    {
                        if (node.Parent.whatAmI() == "ASTBuilder.Expression")
                        {
                            Expression parent = (Expression)node.Parent;
                            if (parent.exprKind != ExprKind.PLUSOP || parent.exprKind != ExprKind.MINUSOP ||
                                parent.exprKind != ExprKind.ASTERISK || parent.exprKind != ExprKind.RSLASH)
                            {
                                lhsType = GetExpressionType(exprLhs).Type;
                                rhsType = GetExpressionType(exprRhs).Type;
                                if (lhsType.ToString() != rhsType.ToString())
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(this.prefix + ">> ERROR: Can't Apply Operator to " + 
                                        lhsType.ToString() + " and " + rhsType.ToString());
                                    Console.ResetColor();
                                    ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                                    ErrorAttributes errorAttr = new ErrorAttributes("Can't Apply Operator to " + 
                                        lhsType.ToString() + " and " + rhsType.ToString(), errorType);
                                    table.Enter(node.exprKind.ToString(), errorAttr);
                                    return false;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(this.prefix + ">> ERROR: Can't Apply Operator To Arithmatic Expression");
                                    Console.ResetColor();
                                    ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                                    ErrorAttributes errorAttr = new ErrorAttributes("Can't Apply Operator To Arithmatic Expression", errorType);
                                    table.Enter(node.exprKind.ToString(), errorAttr);
                                    return false;
                                }
                            }
                        }  
                    }
                }
            }
            return true;

            // If node.exprKind is EQUALS, left child must be checked of assignability
            // (to handle the job of LHSSemantic Visitor)
        }

        public Attributes GetExpressionType(dynamic node)
        {
            dynamic nodeType = null;
            if (node.whatAmI() == "ASTBuilder.INT_CONST")
                nodeType = new TypeAttributes(new IntegerTypeDescriptor());
            else if (node.whatAmI() == "ASTBuilder.STR_CONST")
                nodeType = new TypeAttributes(new StringTypeDescriptor());
            else if (node.whatAmI() == "ASTBuilder.MethodCall")
            {
                VisitNode(node);
                nodeType = table.Lookup(node.Child.Name);
            }
            else if (node.whatAmI() == "ASTBuilder.Expression")
            {
                VisitNode(node);
                nodeType = table.Lookup(node.Child.Name);
            }
            else
                nodeType = table.Lookup(node.Name);
            return nodeType;
        }

        private bool IsDeclared(dynamic node)
        {
            if (node.whatAmI() == "ASTBuilder.Identifier")
                return table.Declared(node.Name);
            return true;
        }
        
        // Node for Tokens have no children and include additional information that is useful in the trace
        public virtual string VisitNode(Identifier node)
        {
            Trace(node, node.Name);
            return node.Name;
        }

        public virtual string VisitNode(PrimitiveType node)
        {
            Trace(node, node.Type.ToString());
            return node.Type.ToString();
        }

        public virtual void VisitParameterNodes(PrimitiveType node1, Identifier node2)
        {
            TypeAttributes type = (TypeAttributes)VisitType(node1.Type.ToString());
            VariableAttributes parameterAttr = new VariableAttributes(node2.Name, type.Type);
            table.Enter(node2.Name, parameterAttr);
            parameters.Add(node2.Name, type.Type);
        }

        public virtual void VisitNode(Parameter node)
        {
            VisitParameterNodes((PrimitiveType)node.Child, (Identifier)node.Child.Sib);
        }

        private Attributes VisitType(string type)
        {
            switch (type)
            {
                case "INT":
                    return table.Lookup(type);
                case "BOOLEAN":
                    return table.Lookup(type);
                case "VOID":
                    return table.Lookup(type);
                case "FLOAT":
                    return table.Lookup(type);
                case "STRING":
                    return table.Lookup(type);
                default: return null;
            }
        }

        //define method attribute, increase scope, and check body 
        public void VisitNode(MethodDeclaration node)
        {
            List<string> mods = new List<string>();
            Trace(node);
            // Increase indent for trace before visiting children
            String oldPrefix = this.prefix;
            this.prefix += "   ";

            dynamic modifiers = node.Child;
            prevModifier = modifiers.ModifierTokens[0].ToString();
            mods = VisitNode(modifiers);
            if (modifiers.Sib.whatAmI() == "ASTBuilder.Modifiers")
            {
                modifiers = modifiers.Sib;
                currentModifier = modifiers.ModifierTokens[0].ToString();
                mods = VisitNode(modifiers);
            }
            dynamic typeSpec = modifiers.Sib;
            VisitNode(typeSpec);
            dynamic signature = typeSpec.Sib;
            VisitNode(signature);
            string name = signature.Child.Name;
            
            Attributes typeDescriptor = VisitType(typeSpec.Type.ToString());

            MethodAttributes attr = new MethodAttributes(name, mods, typeDescriptor.Type, parameters, table.CurrentNestLevel);
            table.Enter(name, attr);


            table.OpenScope();
            dynamic methodBody = signature.Sib;
            VisitNode(methodBody);

            //Method Body


            // Restore prefix
            this.prefix = oldPrefix;
        }

        public void VisitNode(LocalDecl node)
        {
            Trace(node);
            dynamic type = node.Child;
            dynamic typeSpec = VisitType(type.Type.ToString());
            VariableAttributes attr;

            foreach (dynamic sibling in type.Siblings())
            {
                attr = new VariableAttributes(sibling.Name, typeSpec.Type);
                table.Enter(sibling.Name, attr);
            };

            // In operational version, instead of visiting children use
            // VisitType(node.Child);  to process the type name or specification -- instead of TypeVisitor
            // Declare the names on the NameList (node.Child.Sib) using type from first child (node.Child.type)
        }

        public void VisitNode(MethodCall node)
        {
            Trace(node);
            Identifier id = (Identifier)node.Child;
            dynamic args = id.Siblings();
            TypeDescriptor argType;
            List<TypeDescriptor> methodArgsAttributes = new List<TypeDescriptor>();
            dynamic exprArg;

            dynamic methodCall = table.Lookup(id.Name);
            if (methodCall != null && methodCall.Type.ToString() == "ASTBuilder.ClassTypeDescriptor")
            {
                dynamic methodId = id.Child;
                string methodName = methodId.Name.ToString();
                methodCall = table.Lookup(methodName);
            }
            foreach (dynamic arg in args)
            {
                if (arg.ToString() == "ASTBuilder.Identifier")
                {
                    argType = table.Lookup(arg.Name).Type;
                    methodArgsAttributes.Add(argType);
                }
                else if (arg.ToString() == "ASTBuilder.Expression")
                {
                    exprArg = arg;
                    if (VisitNode(arg))
                    {
                        while (exprArg.ToString() == "ASTBuilder.Expression")
                            exprArg = exprArg.Child;

                        argType = GetExpressionType(exprArg).Type;
                        methodArgsAttributes.Add(argType);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(this.prefix + ">> ERROR: Invalid Argument for " + methodCall.ToString());
                        Console.ResetColor();
                        ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                        ErrorAttributes errorAttr = new ErrorAttributes("Invalid Argument", errorType);
                        table.Enter("Invalid Argument", errorAttr);
                    }
                }
                else
                {
                    argType = ConstToAttribute(arg.ToString()).Type;
                    methodArgsAttributes.Add(argType);
                }
            }
            if (CompareParametersToArgs(methodCall, methodArgsAttributes))
            {
                string name = methodCall.Name + "MethodCall";
                if (!table.Declared(name))
                {
                    ArgumentAttributes argumentAttributes = new ArgumentAttributes(methodArgsAttributes);
                    MethodCallAttributes methodCallAttributes = new MethodCallAttributes(methodCall.Name,
                        argumentAttributes, methodCall.Type, table.CurrentNestLevel);
                    table.Enter(name, methodCallAttributes);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.prefix + ">> ERROR: Invalid MethodCall");
                Console.ResetColor();
                ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
                ErrorAttributes errorAttr = new ErrorAttributes("Invalid MethodCall", errorType);
                table.Enter("Invalid MethodCall", errorAttr);
            }
            
        }

        private TypeAttributes ConstToAttribute(string s)
        {
            TypeAttributes attr = null;
            if (s == "ASTBuilder.INT_CONST")
                attr = new TypeAttributes(new IntegerTypeDescriptor());
            else if (s == "ASTBuilder.STR_CONST")
                attr = new TypeAttributes(new StringTypeDescriptor());
            else
                attr = (TypeAttributes)table.Lookup(s);

            return attr;
        }

        private bool CompareParametersToArgs(dynamic method, dynamic args)
        {
            if (method.ToString() == "ASTBuilder.MethodAttributes")
            {
                dynamic methodParameters = method.Value2;
                foreach (dynamic parameter in methodParameters)
                {
                    if (args.Count == methodParameters.Count)
                    {
                        if (args.Contains(parameter.Value))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        public void VisitNode(ClassDeclaration node)
        {
            List<string> mods = new List<string>();
            Trace(node);
            String oldPrefix = this.prefix;
            
            this.prefix += "   ";

            dynamic modifiers = node.Child;
            prevModifier = modifiers.ModifierTokens[0].ToString();
            mods = VisitNode(modifiers);
            if (modifiers.Sib.whatAmI() == "ASTBuilder.Modifiers")
            {
                modifiers = modifiers.Sib;
                currentModifier = modifiers.ModifierTokens[0].ToString();
                mods = VisitNode(modifiers);
            }
            dynamic className = modifiers.Sib;
            VisitNode(className);
            ClassTypeDescriptor classTypeDescriptor = new ClassTypeDescriptor();
            ClassAttributes attr = new ClassAttributes(className.Name, mods, classTypeDescriptor, table.CurrentNestLevel);
            table.Enter(className.Name, attr);
            dynamic classBody = className.Sib;
            table.OpenScope();
            VisitNode(classBody);
           
        }

        public List<string> VisitNode(Modifiers node)
        {
            List<string> mods = new List<string>();
            if (TraceFlag)
            {
                if (currentModifier == "PUBLIC" && prevModifier == "PUBLIC")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> ERROR: Duplicate access modifier " + node.ModifierTokens[0]);
                    Console.ResetColor();
                }
                else if (currentModifier == "PRIVATE" && prevModifier == "PRIVATE")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> ERROR: Duplicate access modifier " + node.ModifierTokens[0]);
                    Console.ResetColor();
                }
                else if (currentModifier == "STATIC" && prevModifier == "STATIC")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> ERROR: Duplicate access modifier " + node.ModifierTokens[0]);
                    Console.ResetColor();
                }
                else if (currentModifier == "PUBLIC" && prevModifier == "PRIVATE")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> ERROR: Conflicting access modifier " + node.ModifierTokens[0]);
                    Console.ResetColor();
                }
                else if (currentModifier == "PRIVATE" && prevModifier == "PUBLIC")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> ERROR: Conflicting access modifier " + node.ModifierTokens[0]);
                    Console.ResetColor();
                }
                else if (currentModifier == "PUBLIC" && prevModifier == "STATIC")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> ERROR: Access modifiers out of order " + node.ModifierTokens[0]);
                    Console.ResetColor();
                }
                else if (currentModifier == "PRIVATE" && prevModifier == "STATIC")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> Access modifiers out of order " + node.ModifierTokens[0]);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(this.prefix + ">> In Modifiers VisitNode for ModifierType: " + node.ModifierTokens[0]);
                    Console.ResetColor();
                    mods.Add(prevModifier);
                    if (currentModifier != null)
                        mods.Add(currentModifier);
                }
            }
            return mods;
        }

        public void VisitNode(SelectionStatement node)
        {
            if (node.Child.whatAmI() == "ASTBuilder.MethodCall" || node.Child.whatAmI() == "ASTBuilder.Expression")
            {
                if (node.Child.whatAmI() == "ASTBuilder.Expression")
                {
                    //Check if the expression is valid
                    //Error if invalid
                    //If valid, openscope and progress through the children of the expression
                    //Then move to sibling of selectionstatement and openscope and progress through its children
                }

                //Invalid selection statement
            }

            //Check for expression or bool methodCall
            //If expression, check if valid
            //Openscope for IF
            //Progress through if
            //Openscope for else
            //Progress through else
        }
    }
}
    
