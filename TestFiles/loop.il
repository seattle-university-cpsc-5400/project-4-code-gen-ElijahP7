.assembly extern mscorlib {}
.assembly test1 { }
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      .locals init (
         [0] int32 i,
         [1] int32 gobble
      )
      ldc.i4 0
      stloc i
      ldc.i4 1
      stloc gobble
      ldstr "Beginning loop test"
      call void [mscorlib]System.Console::WriteLine(string)
      LOOP_START_0:
      ldloc i
      ldc.i4 10
      clt
      brfalse LOOP_END_0
      ldloc gobble
      call void [mscorlib]System.Console::WriteLine(int32)
      ldloc i
      ldc.i4 1
      add
      stloc i
      ldloc gobble
      ldloc i
      mul
      stloc gobble
      br LOOP_START_0
      LOOP_END_0:
      ret
   }
