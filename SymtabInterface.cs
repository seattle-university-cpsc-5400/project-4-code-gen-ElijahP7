namespace ASTBuilder
{
	
	public interface SymtabInterface
	{
		/// Open a new nested symbol table scope
		void OpenScope();
	  
		/// Close an existng nested scope
		/// There must be an existng scope to close
		void CloseScope();

		int CurrentNestLevel {get;}

		void Enter(string id, Attributes attr);

		Attributes Lookup(string id);

		bool Declared(string s);
    }
}
