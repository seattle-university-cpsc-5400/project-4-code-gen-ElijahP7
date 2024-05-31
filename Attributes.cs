using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    /****************************************************/
    /*Information about symbols held in AST nodes and the symbol table*/
    /****************************************************/
    public abstract class Attributes
    {
        public string Name { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; }
        public TypeDescriptor TypeDescriptors { get; set; }
        public TypeDescriptor Type { get; set; }
        public int Scope { get; set; }
        public string Name2 { get; set; }
        
        public Attributes() 
        {
           
        }
       
    }
    public class VariableAttributes : Attributes
    {
        public VariableAttributes(string name, TypeDescriptor type) 
        {
            Name = name;
            Type = type;
        }
    }

    public class TypeAttributes : Attributes
    {
        public TypeAttributes(TypeDescriptor type)
        {
            Type = type;
        }
    }

    public class ClassAttributes : Attributes 
    {
        public ClassAttributes(string name, List<string> modifiers, TypeDescriptor type, int scope) 
        {
            Name = name;
            Value1 = modifiers;
            Type = type;
            Scope = scope;
        }
    }
    
    public class MethodAttributes : Attributes
    {
        public MethodAttributes(string name, List<string> modifiers, TypeDescriptor type, Dictionary<string, TypeDescriptor> parameterList, string className, 
            int scope) 
        {
            Name = name;
            Value1 = modifiers;
            Type = type;
            Value2 = parameterList;
            Name2 = className;
            Scope = scope;
        }
    }

    public class ArgumentAttributes : Attributes
    {
        public ArgumentAttributes(List<TypeDescriptor> typeDescriptors) 
        {
            Value1 = typeDescriptors;
        }
    }

    public class MethodCallAttributes : Attributes
    {
        public MethodCallAttributes(string name, ArgumentAttributes arguments, TypeDescriptor type, int scope)
        {
            Name = name;
            Value1 = arguments;
            Type = type;
            Scope = scope;
        }
    }

    public class ErrorAttributes : Attributes
    {
        public ErrorAttributes(string message, TypeDescriptor type) 
        {
            Name = message;
            Type = type;
        }
    }

    public class AssignmentAttributes : Attributes
    {
        public AssignmentAttributes(string name, TypeDescriptor type, object value)
        {
            Name = name;
            Type = type;
            Value1 = value;
        }
    }

    public class ParameterAttributes : Attributes
    {
        public ParameterAttributes(string name, TypeDescriptor type)
        {
            Name = name;
            Type = type;
        }
    }

}
