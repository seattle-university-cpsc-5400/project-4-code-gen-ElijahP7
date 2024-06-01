.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit hello extends [mscorlib]System.Object{
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      ldstr "Hello, World"
      call void [mscorlib]System.Console::WriteLine(string)
      ret
   }
}
