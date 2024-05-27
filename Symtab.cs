using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ASTBuilder
{

	/// Your implementation of Symtab should be MERGED with this file.
	/// Note what has been added to the constructor here.

	public class Symtab : SymtabInterface
	{
		/// The name makes the use of this field obvious
		/// It should never have a negative integer as its value
		public int CurrentNestLevel { get; private set; }
        private Stack<Dictionary<string, Attributes>> symbolTable;
		private Dictionary<string, Attributes> globalScope;
		private Dictionary<string, Attributes> currentScope;


        public Symtab()
        {
			this.CurrentNestLevel = 0;

            // *** Do any setup necessary to create a global name scope
            // *** and then initialize it with built-in names ...
            symbolTable = new Stack<Dictionary<string, Attributes>>();
			globalScope = new Dictionary<string, Attributes>();
			currentScope = globalScope;
            this.CurrentNestLevel = 0;
			symbolTable.Push(globalScope);
            EnterPredefinedNames();

		}
		/// <summary>
		/// Enter predefined names into symbol table.  
		/// </summary>
		public void EnterPredefinedNames()
		{
			TypeAttributes attr = new TypeAttributes(new IntegerTypeDescriptor());
			Enter("INT", attr);
			attr = new TypeAttributes(new BooleanTypeDescriptor());
			Enter("BOOLEAN", attr);
			attr = new TypeAttributes(new FloatTypeDescriptor());
			Enter("FLOAT", attr);
			attr = new TypeAttributes(new VoidTypeDescriptor());
			Enter("VOID", attr);
			attr = new TypeAttributes(new StringTypeDescriptor());
			Enter("STRING", attr);
			attr = new TypeAttributes(new MSCorLibTypeDescriptor());
			Enter("Write", attr);
			attr = new TypeAttributes(new MSCorLibTypeDescriptor());
			Enter("WriteLine", attr);
        }

		/// <summary>
		/// Opens a new scope, retaining outer ones 
		/// </summary>
		public void OpenScope()
		{
            Dictionary<string, Attributes> newScope = new Dictionary<string, Attributes>();
			symbolTable.Push(newScope);
            currentScope = newScope;
            CurrentNestLevel++;
        }

		/// <summary>
		/// Closes the innermost scope </summary>
		/// </summary>
		public void CloseScope()
		{
            if (symbolTable.Count > 1)
            {
                symbolTable.Pop();
				currentScope = symbolTable.Peek();
                CurrentNestLevel--;
            }
            else
                err("Cannot close the global scope");
        }

		/// <summary>
		/// Enter the given symbol information into the symbol table.  If the given
		///    symbol is already present at the current nest level, produce an error 
		///    message, but do NOT throw any exceptions from this method.
		/// </summary>
		public void Enter(string s, Attributes info)
		{
            if (!DeclaredLocally(s))
            {
				currentScope[s] = info;
            }
            else
                err($"Symbol is already present at {s}");
        }

		/// <summary>
		/// Returns the information associated with the innermost currently valid
		///     declaration of the given symbol.  If there is no such valid declaration,
		///     return null.  Do NOT throw any excpetions from this method.
		/// </summary>
		public Attributes Lookup(string s)
		{
            foreach (var scope in symbolTable)
            {
                if (scope.ContainsKey(s))
                {
                    return scope[s];
                }
            }
            return null;    
		}

        /// <summary>
        /// Returns a Boolean indicating whether the given symbol has already
        /// been declared in the innermost open scope.
        /// </summary>
        public bool DeclaredLocally(string s)
        {
            if (symbolTable.Count > 0)
            {
                return symbolTable.Peek().ContainsKey(s);
            }
            return false;
        }

		public bool Declared(string s)
		{
            foreach (var scope in symbolTable)
            {
                if (scope.ContainsKey(s))
                {
                    return true;
                }
            }
            return false;
        }

        public void @Out(string s)
		{
			string tab = "";
			for (int i = 1; i <= CurrentNestLevel; ++i)
			{
				tab += "  ";
			}
			Console.WriteLine(tab + s);
		}

		public void err(string s)
		{
			@Out("Error: " + s);
			//Console.Error.WriteLine("Error: " + s);
			Environment.Exit(-1);
		}
	}
}

