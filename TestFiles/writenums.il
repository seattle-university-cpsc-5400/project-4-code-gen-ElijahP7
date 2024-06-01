.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit writenums extends [mscorlib]System.Object{
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      ldc.i4 5
      call void [mscorlib]System.Console::WriteLine(int32)
      ldc.i4 5
      ldc.i4 2
      mul
      ldc.i4 6
      ldc.i4 4
      div
      add
      call void [mscorlib]System.Console::WriteLine(int32)
      ret
   }
}
