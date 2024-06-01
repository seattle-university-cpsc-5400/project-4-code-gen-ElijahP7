.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit compute extends [mscorlib]System.Object{
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      .locals init (
         [0] int32 w,
         [1] int32 x
      )
      ldc.i4 3
      ldc.i4 4
      add
      stloc x
      ldstr "Value of x = "
      call void [mscorlib]System.Console::Write(string)
      ldloc x
      call void [mscorlib]System.Console::WriteLine(int32)
      ldloc x
      ldc.i4 5
      sub
      stloc w
      ldstr "Value of w = "
      call void [mscorlib]System.Console::Write(string)
      ldloc w
      call void [mscorlib]System.Console::WriteLine(int32)
      ldstr "value of big expression = "
      call void [mscorlib]System.Console::Write(string)
      ldloc w
      ldloc x
      mul
      ldc.i4 12
      ldloc w
      div
      add
      call void [mscorlib]System.Console::WriteLine(int32)
      ret
   }
}
