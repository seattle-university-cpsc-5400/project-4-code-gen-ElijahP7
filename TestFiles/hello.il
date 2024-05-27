.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit MyClass
           extends [mscorlib]System.Object
{
.method static void main()
{
   .entrypoint
   .maxstack 1
   ldstr "Hello world!"
   call void [mscorlib]System.Console::WriteLine(string)
   ret
}
}
.method static void main()
{
   .entrypoint
   .maxstack 1
   ldstr "Hello world!"
   call void [mscorlib]System.Console::WriteLine(string)
   ret
}
