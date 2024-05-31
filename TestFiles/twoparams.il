.assembly extern mscorlib {}
.assembly test1 { }
.class public auto ansi beforefieldinit twoparams extends [mscorlib]System.Object{
   .method public static void outTwoInts(int32 p1, int32 p2)   {
      ldarg p1
      call void [mscorlib]System.Console::WriteLine(int32)
      ldstr " is the value of the first parameter"
      call void [mscorlib]System.Console::WriteLine(string)
      ldarg p2
      call void [mscorlib]System.Console::WriteLine(int32)
      ldstr " is the value of the second parameter"
      call void [mscorlib]System.Console::WriteLine(string)
      ret
   }
   .method public static void main()   {
      .entrypoint
      .maxstack 10
      .locals init (
         [0] int32 w,
         [1] int32 x,
         [2] int32 y
      )
      ldstr "TCCL test of method with two parameters test"
      call void [mscorlib]System.Console::WriteLine(string)
      ldc.i4 3
      ldc.i4 4
      add
   stloc y
      ldc.i4 5
      ldc.i4 7
      mul
   stloc x
      ldloc y
      ldloc x
      call void twoparams::outTwoInts(int32, int32)
      ret
   }
}
