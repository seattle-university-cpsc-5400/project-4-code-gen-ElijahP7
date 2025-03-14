﻿using System;
using System.Data.Common;
using System.Xml.Linq;

namespace ASTBuilder
{
    class PrintVisitor 
    {
        // The Visitor pattern implementation works by using the 'dynamic' type specification
        // to pass any node type to the function that initiates the visiting process.
        // The calls to VisitNode with 'node' as an argument then trigger dynamic lookup 
        // to find the appropriate version of that method.


        // This PrintTree method begins the tree printing process and handles
        // production of the tree structure prefix.  It will be called recursively
        // to produce indented output for each level of the abstract syntax tree.

        private Symtab table;
        private SemanticsVisitor visitor;
        private CodeGenVisitor codeGenVisitor;

        public PrintVisitor() 
        {
            table = new Symtab();
        }   

        public void DoSemantics(AbstractNode root)
        {
            visitor = new SemanticsVisitor(table, "");
            Console.WriteLine("Starting semantic checking");
            visitor.CheckSemantics(root);
            Console.WriteLine("Semantic checking complete");
        }

        public void GenerateIL(AbstractNode root, string filename)
        {
            codeGenVisitor = new CodeGenVisitor(table);
            Console.WriteLine("Starting code generation");
            codeGenVisitor.GenerateCode(root, filename);
        }

        public void PrintTree(dynamic node, string prefix = "")
        {
            // This check is here to simplify calling code, VisitChildren 
            if (node == null) { 
                return;
            }
            // Print appropriate prefix before calling VisitNode

            // The form of the prefix at this level and in the call to
            // VisitChildren depends on whether the current node is
            // the last sibling among the children of its parent node
            bool isLastChild = (node.Sib == null);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(prefix);
            Console.Write(isLastChild ? "└─ " : "├─ ");
            Console.ResetColor();

            this.VisitNode(node);

            // Propagate tree traversal to the children of node
            VisitChildren(node, prefix + (isLastChild ? "   " : "│ "));
       }
        
        // This method isn't strictly necessary, since its simple body
        // could be incorporated into PrintTree.  It is included here to be
        // consistent with the pseudocode in the textbook.
        public void VisitChildren(AbstractNode node, String prefix)
        {
            foreach (AbstractNode child in node.Children())
            {
                PrintTree(child, prefix);
            }
        }

        // VisitNode is defined here for a parameter of the parent class
        // AbstractNode, so it will be invoked on any node when there is not
        // a specialized method defined below.
        public void VisitNode(AbstractNode node)
        {
            Console.WriteLine("<" + node.ClassName() + ">");
        }

        // Here are three specialized VisitNode methods for terminals plus
        // one for Expression.  You will be adding more methods here for 
        // other nodes that hold information of interest beyond the class name.
        //
        // Note that the only differences in these methods are on the third line, 
        // where the special information for the node type is written out.  
        // Just what must be included at this point in the method depends on
        // what information is stored in the node.

        public void VisitNode(MethodDeclaration node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (node.Child.Sib.whatAmI() == "ASTBuilder.Modifiers")
            {
                dynamic type = node.Child.Sib.Sib;
                Console.WriteLine(" -- Node Type: " + table.Lookup(type.Type.ToString()).Type.ToString());
            }
            else
            {
                dynamic type = node.Child.Sib;
                Console.WriteLine(" -- Node Type: " + table.Lookup(type.Type.ToString()).Type.ToString());
            }
            Console.ResetColor();
        }

        public void VisitNode(Identifier node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Name + " -- Node Type: " + table.Lookup(node.Name).Type.ToString());
            Console.ResetColor();
        }

        public void VisitNode(Modifiers node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.ModifierTokens[0]);
            Console.ResetColor();
        }

        public void VisitNode(PrimitiveType node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Type + " -- Node Type: " + table.Lookup(node.Type.ToString().ToString()).Type.ToString());
            Console.ResetColor();
        }

        public void VisitNode(INT_CONST node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.IntVal + " -- Node Type: " + table.Lookup("INT").Type.ToString());
            Console.ResetColor();
        }

        public void VisitNode(STR_CONST node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.StrVal + " -- Node Type: " + table.Lookup("STRING").Type.ToString());
            Console.ResetColor();
        }

        public void VisitNode(Expression node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            dynamic lhs = node.Child;
            if (table.DeclaredLocally(node.exprKind.ToString()))
                Console.WriteLine(node.exprKind + " -- Node Type " + table.Lookup(node.exprKind.ToString()));
            else
                Console.WriteLine(node.exprKind + " -- Node Type: " + visitor.GetExpressionType(lhs).Type.ToString());
            Console.ResetColor();
        }

        public void VisitNode(SelectionStatement node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.selectionType);
            Console.ResetColor();
        }

        public void VisitNode(IterationStatement node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.iterationType);
            Console.ResetColor();
        }
    }
}

