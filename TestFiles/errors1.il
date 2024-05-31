.assembly extern mscorlib {}
.assembly test1 { }
.method static void main()
{
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
   ldloc x
   ldloc x
   mul
   stloc w
   ldloc y
   ldc.i4 4
   add
   stloc x
   ldloc x
   ldloc w
   div
   ldc.i4 1
   add
   stloc z
   ldloc x
   sub
   stloc w
   ldloc x
   ldloc x
   ldc.i4 0
   mul
   stloc w
   stloc x
   ldloc w
   ldc.i4 5
   stloc x
   ret
}
