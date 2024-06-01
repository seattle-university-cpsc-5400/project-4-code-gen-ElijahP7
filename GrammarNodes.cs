using System.Collections.Generic;

namespace ASTBuilder
{

    public class CompilationUnit : AbstractNode
    {
        // just for the CompilationUnit because it's the top node
        //public override AbstractNode LeftMostSibling => this;
        public override AbstractNode Sib => null;

        public CompilationUnit(AbstractNode classDecl)
        {
            adoptChildren(classDecl);
        }

    }

    public class ClassDeclaration : AbstractNode
    {
        public ClassDeclaration(
            AbstractNode modifiers,
            AbstractNode className,
            AbstractNode classBody)
        {
            adoptChildren(modifiers);
            adoptChildren(className);
            adoptChildren(classBody);
        }

    }

    public class ClassBody : AbstractNode
    {
        public ClassBody(AbstractNode members)
        {
            adoptChildren(members);
        }
    }

    public enum ModifierType { PUBLIC, STATIC, PRIVATE }

    public class Modifiers : AbstractNode
    {
        public List<ModifierType> ModifierTokens { get; set; } = new List<ModifierType>();

        public void AddModType(ModifierType type)
        {
            ModifierTokens.Add(type);
        }

        public Modifiers(ModifierType type)
        {
            AddModType(type);
        }

    }

    public class Identifier : AbstractNode
    {
        public virtual string Name { get; protected set; }

        public Identifier(string s)
        {
            Name = s;
        }
    }

    public class INT_CONST : AbstractNode
    {
        public virtual string IntVal { get; protected set; }

        public INT_CONST(string s)
        {
            IntVal = s;
        }
    }

    public class STR_CONST : AbstractNode
    {
        public virtual string StrVal { get; protected set; }

        public STR_CONST(string s)
        {
            StrVal = s;
        }
    }

    public class MethodDeclaration : AbstractNode
    {
        public MethodDeclaration(
            AbstractNode modifiers,
            AbstractNode typeSpecifier,
            AbstractNode methodSignature,
            AbstractNode methodBody)
        {
            adoptChildren(modifiers);
            adoptChildren(typeSpecifier);
            adoptChildren(methodSignature);
            adoptChildren(methodBody);
        }
    }

    public enum EnumPrimitiveType { BOOLEAN, INT, VOID, STRING }
    public class PrimitiveType : AbstractNode
    {
        public EnumPrimitiveType Type { get; set; }
        public PrimitiveType(EnumPrimitiveType type)
        {
            Type = type;
        }
    }
    
     public class MethodBody : AbstractNode
    {
        public MethodBody(AbstractNode localItems)
        {
            adoptChildren(localItems);
        }
    }

    public class Parameter : AbstractNode
    {
        public Parameter(AbstractNode typeSpec, AbstractNode declName) : base()
        {
            adoptChildren(typeSpec);
            adoptChildren(declName);
        }
    }

    public class ParameterList : AbstractNode
    {
        public ParameterList(AbstractNode parameter) : base()
        {
            adoptChildren(parameter);
        }
    }

    public class MethodSignature : AbstractNode
    {
        public MethodSignature(AbstractNode name)
        {
            adoptChildren(name);
        }

        public MethodSignature(AbstractNode name, AbstractNode paramList)
        {
            adoptChildren(name);
            adoptChildren(paramList);
        }
    }
    
    public enum ExprKind
    {
        EQUALS, OP_LOR, OP_LAND, PIPE, HAT, AND, OP_EQ,
        OP_NE, OP_GT, OP_LT, OP_LE, OP_GE, PLUSOP, MINUSOP,
        ASTERISK, RSLASH, PERCENT, UNARY, PRIMARY
    }
    public class Expression : AbstractNode
    {
        public ExprKind exprKind { get; set; }
        public Expression(AbstractNode expr, ExprKind kind)
        {
            adoptChildren(expr);
            exprKind = kind;
        }
        public Expression(AbstractNode lhs, ExprKind kind, AbstractNode rhs)
        {
            adoptChildren(lhs);
            adoptChildren(rhs);
            exprKind = kind;
        }
    }

    public class MethodCall : AbstractNode
    {
        public MethodCall(
            AbstractNode methodReference,
            AbstractNode argumentList = null)
        {
            adoptChildren(methodReference);
            adoptChildren(argumentList);
        }
    }

    public class LocalDecl : AbstractNode 
    {
        public LocalDecl(AbstractNode type, AbstractNode name) 
        {
            adoptChildren(type);
            adoptChildren(name);
        }
    }

    public class NameList : AbstractNode
    {
        public NameList(AbstractNode list) : base()
        {
            adoptChildren(list);
        }
    }

    public class Statement : AbstractNode
    {
        public Statement(AbstractNode statement)
        {
            adoptChildren(statement);
        }
    }

    public enum SelectionType { IF_ELSE, IF }

    public class SelectionStatement : AbstractNode
    {
        public SelectionType selectionType { get; set; }

        public SelectionStatement(AbstractNode expression, AbstractNode statement, AbstractNode elseBlock, SelectionType type)
        {
            adoptChildren(expression);
            adoptChildren(statement);
            adoptChildren(elseBlock);
            selectionType = type;
        }

        public SelectionStatement(AbstractNode expression, AbstractNode statement, SelectionType type) 
        {
            adoptChildren(expression);
            adoptChildren(statement);
            selectionType = type;
        }
    }

    public enum IterationType { WHILE }

    public class IterationStatement : AbstractNode
    {
        public IterationType iterationType { get; set; }

        public IterationStatement(AbstractNode expression, AbstractNode localItems, IterationType type)
        {
            adoptChildren(expression);
            adoptChildren(localItems);
            iterationType = type;
        }
    }

    public class ReturnStatement : AbstractNode
    {
        public ReturnStatement(AbstractNode expression = null)
        {
            adoptChildren(expression);
        }
    }

    public class Block : AbstractNode
    {
        public Block(AbstractNode node)
        {
            adoptChildren(node);
        }
    }

    public class Struct : AbstractNode
    {
        public Struct(AbstractNode modifiers, AbstractNode identifier, AbstractNode classBody)
        {
            adoptChildren(modifiers);
            adoptChildren(identifier);
            adoptChildren(classBody);
        }
    }
}

