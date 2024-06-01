using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Security.Cryptography;
using System.Dynamic;

namespace ASTBuilder
{
    class CodeGenVisitor 
    {

        private StreamWriter file;  // IL code written to this file

        // used to produce a readable trace when desired
        protected String prefix = "";
        private Symtab table;
        private int uniqueId;

        public CodeGenVisitor(Symtab symTable)
        {
            table = symTable;
            uniqueId = 0;
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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(this.prefix + ">> In VistNode for " + node.ClassName() + " " + suffix);
                Console.ResetColor();
            }
        }
        public void GenerateCode(dynamic node, string filename)
        // node is the root of the AST for the entire TCCL program
        {
            if (node == null) return;  // No code generation for empty AST

            file = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), filename + ".il"));

            if (TraceFlag)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(this.prefix + "Started code Generation for " + filename + ".txt");
                Console.ResetColor();
            }
            // Since node is the root node of the AST, this call begins the code generation traversal
            VisitNode(node);

            file.Close();

        }

        public virtual void VisitNode(AbstractNode node)
        {
            if (TraceFlag)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(this.prefix + ">> In general VistNode for " + node.ClassName());
                Console.ResetColor();
            }

            VisitChildren(node);
        }
        public virtual void VisitChildren(AbstractNode node)
        {
            // Nothing to do if node has no children
            if (node.Child == null) return;

            if (TraceFlag)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
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
        public virtual void VisitNode(CompilationUnit node)
        {
            Trace(node);

            // The two lines that follow generate the prelude required in all .il files
            file.WriteLine(".assembly extern mscorlib {}");
            file.WriteLine(".assembly test1 { }");

            VisitChildren(node);

            // The following lines are present so that an executable .il file is generated even
            // before you have implemented any VisitNode routines.  It generated the body for the
            // hello.txt program regardless of what file is parsed to create the AST.
            // DELETE THESE LINES ONCE YOU HAVE IMPLEMENTED VisitNode FOR MethodDeclaration
            // DELETE TRHOUGH HERE

        }

        public virtual void VisitNode(ClassDeclaration node)
        {
            string name = "";

            foreach (dynamic child in node.Children())
            {
                if (child.whatAmI() == "ASTBuilder.Identifier")
                {
                    name = child.Name;
                    break;
                }    
            }

            dynamic classAttr = table.Lookup(name);

            file.Write(".class ");
            foreach (dynamic val in classAttr.Value1)
            {
                file.Write(val.ToLower() + " ");
            }
            file.Write("auto ansi beforefieldinit ");
            file.Write($"{classAttr.Name} ");
            file.Write("extends [mscorlib]System.Object");
            file.WriteLine("{");
            foreach (dynamic child in node.Children())
            {
                if (child.whatAmI() == "ASTBuilder.ClassBody")
                    VisitChildren(child);
            }
            file.WriteLine("}");
        }

        public virtual void VisitNode(MethodDeclaration node)
        {
            string name = "";
            int count = 0;
            int totalParams = 0;
            foreach (dynamic child in node.Children())
            {
                if (child.whatAmI() == "ASTBuilder.MethodSignature")
                {
                    name = child.Child.Name;
                    break;
                }
            }

            dynamic methodAttr = table.Lookup(name);
            totalParams = methodAttr.Value2.Count;
            file.Write("   .method public static ");
            /*
             * Since CIL requires a static entrypoint and we aren't dealing with private,
             * I hard coded the modifiers
             */
            file.Write(methodAttr.Type.type);
            file.Write($" {methodAttr.Name}(");
            foreach (dynamic param in methodAttr.Value2)
            {
                if (count < totalParams - 1)
                    file.Write($"{param.Value.type} {param.Key}, ");
                else
                    file.Write($"{param.Value.type} {param.Key}");
                count++;
            }
            file.Write(")");
            file.WriteLine("   {");
            if (methodAttr.Name == "main")
            {
                file.WriteLine("      .entrypoint");
                file.WriteLine("      .maxstack 10");
            }
            foreach (dynamic child in node.Children())
            {
                if (child.whatAmI() == "ASTBuilder.Block")
                    VisitChildren(child);
            }
            file.WriteLine("      ret");
            file.WriteLine("   }");
        }
        
        private void VisitNode(Expression node)
        {
            if (table.Lookup(node.whatAmI()) != null)
                Console.WriteLine("ERROR");
            else
                IntegerExpressions(node);
        }

        private void IntegerExpressions(dynamic node)
        {
            switch (node)
            {
                case Expression exprNode:
                    GenerateExpressionCode(exprNode);
                    break;
                case INT_CONST intNode:
                    GenerateIntConstantCode(intNode);
                    break;
                case Identifier idNode:
                    dynamic methodType = table.Lookup(idNode.Name);
                    if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                        GenerateValFromParam(idNode);
                    else
                        GenerateValFromVariable(idNode);
                    break;
                case MethodCall methodCall:
                    if (methodCall.Child.Sib.whatAmI() == "ASTBuilder.Expression")
                        GenerateExpressionCode(methodCall.Child.Sib);
                    else if (methodCall.Child.Sib.whatAmI() == "ASTBuilder.Identifier")
                    {
                        dynamic methodId = methodCall.Child.Sib;
                        if (table.Lookup(methodId.Name).Type.ToString() == "ASTBuilder.StringTypeDescriptor")
                        {
                            if (table.Lookup(methodId.Name).ToString() == "ASTBuilder.ParameterAttributes")
                                file.WriteLine("      ldarg " + methodId.Name);
                            else
                                file.WriteLine("      ldloc " + methodId.Name);
                        }
                        else if (table.Lookup(methodId.Name).Type.ToString() == "ASTBuilder.IntegerTypeDescriptor")
                        {
                            if (table.Lookup(methodId.Name).ToString() == "ASTBuilder.ParameterAttributes")
                                file.WriteLine("      ldarg " + methodId.Name);
                            else
                                file.WriteLine("      ldloc " + methodId.Name);
                        }
                    }
                    string method = BuildMethodCall(methodCall.Child);
                    file.WriteLine(method);
                    break;
            }
        }

        private void GenerateExpressionCode(dynamic node)
        {
            if (node.exprKind == ExprKind.EQUALS)
            {
                string identifier = node.Child.Name;
                IntegerExpressions(node.Child.Sib);
                file.WriteLine($"      stloc {identifier}");
            }
            else
            {
                IntegerExpressions(node.Child);
                IntegerExpressions(node.Child.Sib);

                switch (node.exprKind)
                {
                    case ExprKind.PLUSOP:
                        file.WriteLine("      add");
                        break;
                    case ExprKind.MINUSOP:
                        file.WriteLine("      sub");
                        break;
                    case ExprKind.ASTERISK:
                        file.WriteLine("      mul");
                        break;
                    case ExprKind.RSLASH:
                        file.WriteLine("      div");
                        break;
                    case ExprKind.OP_GT:
                        file.WriteLine("      cgt");
                        break;
                    case ExprKind.OP_LT:
                        file.WriteLine("      clt");
                        break;
                    case ExprKind.OP_GE:
                        file.WriteLine("      clt");
                        file.WriteLine("      ldc.i4 0");
                        file.WriteLine("      ceq");
                        break;
                    case ExprKind.OP_LE:
                        file.WriteLine("      cgt");
                        file.WriteLine("      ldc.i4 0");
                        file.WriteLine("      ceq");
                        break;
                    case ExprKind.PIPE:
                        file.WriteLine("      or");
                        break;
                    case ExprKind.AND:
                        file.WriteLine("      and");
                        break;
                    case ExprKind.OP_EQ:
                        file.WriteLine("      ceq");
                        break;
                }
            }
        }

        private void GenerateIntConstantCode(dynamic node)
        {
            file.WriteLine($"      ldc.i4 {node.IntVal}");
        }

        private void GenerateValFromVariable(dynamic node)
        {
            file.WriteLine($"      ldloc {node.Name}");
        }

        private void GenerateValFromParam(dynamic node)
        {
            file.WriteLine($"      ldarg {node.Name}");
        }

        public virtual void VisitNode(LocalDecl node)
        {
            int totalChildren = GetVariableCount(node);
            int variables = 0;
            file.WriteLine("      .locals init (");
            foreach (dynamic child in node.Children())
            {
                if (child.whatAmI() == "ASTBuilder.Identifier")
                {
                    dynamic type = table.Lookup(child.Name);
                    string id = type.Type.type;
                    if (variables < totalChildren - 1)
                        file.WriteLine($"         [{variables}] {id} {child.Name},");
                    else
                        file.WriteLine($"         [{variables}] {id} {child.Name}");
                    variables++;
                }
            }
            file.WriteLine("      )");
        }

        private int GetVariableCount(dynamic node)
        {
            int count = 0;
            foreach (dynamic child in node.Children())
            {
                if (child.whatAmI() == "ASTBuilder.Identifier")
                    count++;
            }
            return count;
        }

        public virtual void VisitNode(MethodCall node)
        {
            dynamic method = node.Child;
            if (method.Name == "WriteLine")
            {
                if (method.Sib.whatAmI() == "ASTBuilder.STR_CONST")
                {
                    string val = "\"" + method.Sib.StrVal + "\"";
                    file.WriteLine("      ldstr " + val);
                    file.WriteLine("      call void [mscorlib]System.Console::WriteLine(string)");
                }
                else if (method.Sib.whatAmI() == "ASTBuilder.INT_CONST")
                {
                    int val = int.Parse(method.Sib.IntVal);
                    file.WriteLine("      ldc.i4 " + val);
                    file.WriteLine("      call void [mscorlib]System.Console::WriteLine(int32)");
                }
                else if (method.Sib.whatAmI() == "ASTBuilder.Expression")
                {
                    VisitNode(method.Sib);
                    file.WriteLine("      call void [mscorlib]System.Console::WriteLine(int32)");
                }
                else if (method.Sib.whatAmI() == "ASTBuilder.Identifier")
                {
                    dynamic methodType = table.Lookup(method.Sib.Name);
                    if (methodType.Type.ToString() == "ASTBuilder.StringTypeDescriptor")
                    {
                        if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                            file.WriteLine("      ldarg " + method.Sib.Name);
                        else
                            file.WriteLine("      ldloc " + method.Sib.Name);
                        file.WriteLine("      call void [mscorlib]System.Console::WriteLine(string)");
                    }
                    else if (methodType.Type.ToString() == "ASTBuilder.IntegerTypeDescriptor")
                    {
                        if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                            file.WriteLine("      ldarg " + method.Sib.Name);
                        else
                            file.WriteLine("      ldloc " + method.Sib.Name);
                        file.WriteLine("      call void [mscorlib]System.Console::WriteLine(int32)");
                    }  
                }
            }
            else if (method.Name == "Write")
            {
                if (method.Sib.whatAmI() == "ASTBuilder.STR_CONST")
                {
                    string val = "\"" + method.Sib.StrVal + "\"";
                    file.WriteLine("      ldstr " + val);
                    file.WriteLine("      call void [mscorlib]System.Console::Write(string)");
                }
                else if (method.Sib.whatAmI() == "ASTBuilder.INT_CONST")
                {
                    int val = int.Parse(method.Sib.IntVal);
                    file.WriteLine("      ldc.i4 " + val);
                    file.WriteLine("      call void [mscorlib]System.Console::Write(int32)");
                }
                else if (method.Sib.whatAmI() == "ASTBuilder.Expression")
                {
                    IntegerExpressions(method.Sib);
                    file.WriteLine("      call void [mscorlib]System.Console::Write(int32)");
                }
                else if (method.Sib.whatAmI() == "ASTBuilder.Identifier")
                {
                    dynamic methodType = table.Lookup(method.Sib.Name);
                    if (methodType.Type.ToString() == "ASTBuilder.StringTypeDescriptor")
                    {
                        if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                            file.WriteLine("      ldarg " + method.Sib.Name);
                        else
                            file.WriteLine("      ldloc " + method.Sib.Name);
                        file.WriteLine("      call void [mscorlib]System.Console::WriteLine(string)");
                    }
                    else if (methodType.Type.ToString() == "ASTBuilder.IntegerTypeDescriptor")
                    {
                        if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                            file.WriteLine("      ldarg " + method.Sib.Name);
                        else
                            file.WriteLine("      ldloc " + method.Sib.Name);
                        file.WriteLine("      call void [mscorlib]System.Console::WriteLine(int32)");
                    }
                }
            }
            else
            {
                if (!table.Declared("Invalid Method"))
                {
                    foreach (dynamic sib in method.Siblings())
                    {
                        if (sib.whatAmI() == "ASTBuilder.STR_CONST")
                        {
                            string val = "\"" + method.Sib.StrVal + "\"";
                            file.WriteLine("      ldstr " + val);
                        }
                        else if (sib.whatAmI() == "ASTBuilder.INT_CONST")
                        {
                            int val = int.Parse(method.Sib.IntVal);
                            file.WriteLine("      ldc.i4 " + val);
                        }
                        else if (method.Sib.whatAmI() == "ASTBuilder.Expression")
                        {
                            IntegerExpressions(method.Sib);
                        }
                        else if (sib.whatAmI() == "ASTBuilder.Identifier")
                        {
                            dynamic methodType = table.Lookup(sib.Name);
                            if (methodType.Type.ToString() == "ASTBuilder.StringTypeDescriptor")
                            {
                                if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                                    file.WriteLine("      ldarg " + sib.Name);
                                else
                                    file.WriteLine("      ldloc " + sib.Name);
                            }
                            else if (methodType.Type.ToString() == "ASTBuilder.IntegerTypeDescriptor")
                            {
                                if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                                    file.WriteLine("      ldarg " + sib.Name);
                                else
                                    file.WriteLine("      ldloc " + sib.Name);
                            }
                        }
                    }
                    string methodCall = BuildMethodCall(method);
                    file.WriteLine(methodCall);
                }
            }
        }

        private string BuildMethodCall(dynamic method)
        {
            int count = 0;
            int totalParams = 0;
            StringBuilder sb = new StringBuilder();
            dynamic methodCall = table.Lookup(method.Name);
            if (methodCall.Type.ToString() == "ASTBuilder.ClassTypeDescriptor")
            {
                methodCall = table.Lookup(method.Child.Name);
            }
            totalParams = methodCall.Value2.Count;
            sb.Append("      call ");
            sb.Append($"{methodCall.Type.type} ");
            sb.Append($"{methodCall.Name2}::");
            sb.Append($"{methodCall.Name}(");
            if (totalParams == 0)
                sb.Append(")");
            else
            {
                foreach (dynamic param in methodCall.Value2)
                {
                    if (count < totalParams - 1)
                        sb.Append($"{param.Value.type}, ");
                    else
                        sb.Append($"{param.Value.type})");
                    count++;
                }
            }
            return sb.ToString();
        }

        public virtual void VisitNode(SelectionStatement node)
        {
            if (node.selectionType == SelectionType.IF_ELSE)
            {
                VisitSpecificNode(node.Child);

                file.WriteLine($"      brtrue TRUE_{uniqueId}");

                dynamic elseStatement = node.Child.GetLastSibling();
                VisitSpecificNode(elseStatement);
                file.WriteLine($"      br END_{uniqueId}");

                file.WriteLine($"      TRUE_{uniqueId}:");
                VisitSpecificNode(node.Child.Sib);

                file.WriteLine($"      END_{uniqueId}:");
                uniqueId++;

            }
        }

        public virtual void VisitNode(IterationStatement node)
        {
            file.WriteLine($"      LOOP_START_{uniqueId}:");

            VisitSpecificNode(node.Child);
            file.WriteLine($"      brfalse LOOP_END_{uniqueId}");

            VisitSpecificNode(node.Child.Sib);
            file.WriteLine($"      br LOOP_START_{uniqueId}");

            file.WriteLine($"      LOOP_END_{uniqueId}:");
            uniqueId++;
        }

        public virtual void VisitNode(ReturnStatement node)
        {
            dynamic returnVal = node.Child;
            if (returnVal.whatAmI() == "ASTBuilder.STR_CONST")
            {
                string val = "\"" + returnVal.StrVal + "\"";
                file.WriteLine("      ldstr " + val);
                file.WriteLine("      ret");
            }
            else if (returnVal.whatAmI() == "ASTBuilder.INT_CONST")
            {
                int val = int.Parse(returnVal.IntVal);
                file.WriteLine("      ldc.i4 " + val);
                file.WriteLine("      ret");
            }
            else if (returnVal.whatAmI() == "ASTBuilder.Expression")
            {
                VisitNode(returnVal);
                file.WriteLine("      ret");
            }
            else if (returnVal.whatAmI() == "ASTBuilder.Identifier")
            {
                dynamic methodType = table.Lookup(returnVal.Name);
                if (methodType.Type.ToString() == "ASTBuilder.StringTypeDescriptor")
                {
                    if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                        file.WriteLine("      ldarg " + returnVal.Name);
                    else
                        file.WriteLine("      ldloc " + returnVal.Name);
                    file.WriteLine("      ret");
                }
                else if (methodType.Type.ToString() == "ASTBuilder.IntegerTypeDescriptor")
                {
                    if (methodType.ToString() == "ASTBuilder.ParameterAttributes")
                        file.WriteLine("      ldarg " + returnVal.Name);
                    else
                        file.WriteLine("      ldloc " + returnVal.Name);
                    file.WriteLine("      ret");
                }
            }
        }

        private void VisitSpecificNode(dynamic node)
        {
            if (node.whatAmI() == "ASTBuilder.MethodCall")
            {
                MethodCall methodCall = (MethodCall)node;
                VisitNode(methodCall);
            }
            if (node.whatAmI() == "ASTBuilder.Expression")
            {
                Expression expression = (Expression)node;
                VisitNode(expression);
            }
            if (node.whatAmI() == "ASTBuilder.Block")
            {
                Block block = (Block)node;
                VisitNode(block);
            }
            if (node.whatAmI() == "ASTBuilder.SelectionStatement")
            {
                SelectionStatement selectionStatement = (SelectionStatement)node;
                VisitNode(selectionStatement);
            }
            if (node.whatAmI() == "ASTBuilder.IterationStatement")
            {
                IterationStatement iterationStatement = (IterationStatement)node;
                VisitNode(iterationStatement);

            }
            if (node.whatAmI() == "ASTBuilder.ReturnStatement")
            {
                ReturnStatement returnStatement = (ReturnStatement)node;
                VisitNode(returnStatement);
            }
        }
    }
}
