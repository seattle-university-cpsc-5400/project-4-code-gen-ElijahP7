.assembly extern mscorlib {}
.assembly test1 { }
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      .locals init (
         [0] int32 i,
         [1] int32 j
      )
      ldc.i4 0
      stloc i
      ldc.i4 1
      stloc j
      ldstr "TCCL if test"
      call void [mscorlib]System.Console::WriteLine(string)
      ldloc i
      ldc.i4 10
      clt
      brtrue TRUE_0
      br END_0
      TRUE_0:
      END_0:
      ldloc j
      stloc i
      ldloc i
      ldc.i4 10
      ceq
      brtrue TRUE_1
      br END_1
      TRUE_1:
      ldloc i
      stloc j
      ldstr "This statement is in then part and should not be executed"
      call void [mscorlib]System.Console::WriteLine(string)
      END_1:
      ldstr "In else part"
      call void [mscorlib]System.Console::WriteLine(string)
      ret
   }
