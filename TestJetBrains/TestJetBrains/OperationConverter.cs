using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace TestJetBrains
{
    public static class OperationConverter
    {
        public static void ReplaceAdditOp(string pathFirst, string pathSecond)
        {
            AssemblyDefinition assembly = null;
            try
            {
                assembly = AssemblyDefinition.ReadAssembly(pathFirst);
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine("Check the input assembly. Maybe the file is a native executable and not a ECMA-335 compatible assembly. Mono Cecil can not read such an assembly");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (assembly == null)
            {
                return;
            }

            foreach (var typeDef in assembly.MainModule.Types)
            {
                var methods = typeDef.Methods.Where(m => m.HasBody);
                foreach (var method in methods)
                {

                    var il = method.Body.GetILProcessor();

                    for (int i = 0; i < il.Body.Instructions.Count; i++)
                    {

                        if (il.Body.Instructions[i].OpCode == OpCodes.Add)
                        {
                            il.Body.Instructions[i] = il.Create(OpCodes.Sub);
                        }


                        if (il.Body.Instructions[i].OpCode == OpCodes.Call && ((MethodReference)il.Body.Instructions[i].Operand).FullName
                            == "System.Decimal System.Decimal::op_Addition(System.Decimal,System.Decimal)")

                        {
                            il.Body.Instructions[i] = il.Create(OpCodes.Call, assembly.MainModule.ImportReference(typeof(decimal).GetMethod("op_Subtraction", new[] { typeof(decimal), typeof(decimal) })));
                        }
                    }

                }
            }
            try
            {
                assembly.Write(pathSecond);
                Console.WriteLine($"The new assembly is ready, you can find it along the path {pathSecond}");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("It looks like you entered the wrong part of the path. Check the path");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
