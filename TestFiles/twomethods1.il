.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit twomethods1 extends [mscorlib]System.Object{
   .method public static void check(int32 x)   {
      ldstr "The argument to check was "
      call void [mscorlib]System.Console::Write(string)
      ldarg x
      call void [mscorlib]System.Console::WriteLine(int32)
      ret
   }
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      ldc.i4 12345
      call void twomethods1::check(int32)
      ldstr "Back in main"
      call void [mscorlib]System.Console::WriteLine(string)
      ret
   }
}
