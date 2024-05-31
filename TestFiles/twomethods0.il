.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit twomethods0 extends [mscorlib]System.Object{
   .method public static void check()   {
      ldstr "In the check method"
      call void [mscorlib]System.Console::WriteLine(string)
      ret
   }
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      call void twomethods0::check()
      ldstr "Back in main"
      call void [mscorlib]System.Console::WriteLine(string)
      ret
   }
}
