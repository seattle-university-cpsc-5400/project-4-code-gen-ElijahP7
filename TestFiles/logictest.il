.assembly extern mscorlib {}
.assembly test1 { }
   .method public static void outStrln(string s)   {
      ldarg s
      call void [mscorlib]System.Console::WriteLine(string)
      ret
   }
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
      ldstr "TCCL logic test"
      call void logictest::outStrln(string)
      ldloc i
      ldc.i4 10
      clt
      ldloc j
      ldc.i4 1
      ceq
      and
      brtrue TRUE_0
      br END_0
      TRUE_0:
      ldstr "In then part of & test"
      call void logictest::outStrln(string)
      END_0:
      ldloc j
      stloc i
      ldloc i
      ldc.i4 10
      clt
      ldloc j
      ldc.i4 0
      ceq
      and
      brtrue TRUE_1
      br END_1
      TRUE_1:
      ldstr "In then part of 2nd & test erroneously"
      call void logictest::outStrln(string)
      END_1:
      ldstr "In else part of 2nd & test"
      call void logictest::outStrln(string)
      ldloc i
      ldc.i4 10
      ceq
      ldloc j
      ldloc i
      ceq
      or
      brtrue TRUE_2
      br END_2
      TRUE_2:
      ldloc i
      stloc j
      END_2:
      ldstr "In else part of false | test"
      call void logictest::outStrln(string)
      ldloc i
      ldc.i4 1
      ceq
      ldloc j
      ldc.i4 1
      ceq
      or
      brtrue TRUE_3
      br END_3
      TRUE_3:
      ldstr "In then part of true | test"
      call void logictest::outStrln(string)
      END_3:
      ldstr "In else part of true | test erroneously"
      call void logictest::outStrln(string)
      ret
   }
