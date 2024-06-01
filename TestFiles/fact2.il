.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit fact2 extends [mscorlib]System.Object{
   .method public static int32 factorial(int32 x)   {
      ldarg x
      ldc.i4 0
      ceq
      brtrue TRUE_0
      ldarg x
      ldarg x
      ldc.i4 1
      sub
      call int32 fact2::factorial(int32)
      mul
      ret
      br END_0
      TRUE_0:
      ldc.i4 1
      ret
      END_0:
      ret
   }
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      .locals init (
         [0] int32 i,
         [1] int32 gobble
      )
      ldc.i4 0
      stloc i
      ldstr "TCCL recursive factorial test"
      call void [mscorlib]System.Console::WriteLine(string)
      LOOP_START_1:
      ldloc i
      ldc.i4 15
      clt
      brfalse LOOP_END_1
      ldloc i
      call int32 fact2::factorial(int32)
      stloc gobble
      ldloc gobble
      call void [mscorlib]System.Console::WriteLine(int32)
      ldloc i
      ldc.i4 1
      add
      stloc i
      br LOOP_START_1
      LOOP_END_1:
      ret
   }
}
