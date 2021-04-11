using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestJetBrains;

namespace UnitTests
{
    [TestClass]
    public class OperationConverterTest
    {
        [TestMethod]
        public void TestReplaceAdditOpInExe()
        {
            //CLOSE EVERY POP-UP APPLICATION
            string assemblyPath = Path.Combine(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\")), @"TestJetBrains\UnitTests\Assemblies\TestAssembly.exe");
            string newAssemblyPath = assemblyPath.Replace("TestAssembly.exe", "NewAssembly.exe");
            var process = new Process();
            process.StartInfo.FileName = assemblyPath;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            process.WaitForExit();
            OperationConverter.ReplaceAdditOp(assemblyPath, newAssemblyPath);
            process.StartInfo.FileName = newAssemblyPath;
            process.Start();
            process.WaitForExit();
        }

        [TestMethod]
        public void TestReplaceAdditOpInDll()
        {
            string assemblyPath = Path.Combine(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\")), @"TestJetBrains\UnitTests\Assemblies\TestClassLibrary.dll");
            string newAssemblyPath = assemblyPath.Replace("TestClassLibrary.dll", "NewClassLibrary.dll");
            try
            {
                Assembly dll = Assembly.LoadFile(assemblyPath);
                if (dll != null)
                {
                    Type opType = dll.GetType("TestClassLibrary.Operations");
                    ConstructorInfo opConstructor = opType.GetConstructor(Type.EmptyTypes);
                    object opClassObject = opConstructor.Invoke(new object[] { });
                    if (opClassObject != null)
                    {
                        MethodInfo methodInt = opClassObject.GetType().GetMethod("CalculateInt");
                        MethodInfo methodFloat = opClassObject.GetType().GetMethod("CalculateFloat");
                        MethodInfo methodDecimal = opClassObject.GetType().GetMethod("CalculateDecimal");
                        Assert.AreEqual(5, (int)methodInt.Invoke(opClassObject, new object[] { 2, 3 }));
                        Assert.AreEqual(9.76f, (float)methodFloat.Invoke(opClassObject, new object[] { 6.55f, 3.21f }));
                        Assert.AreEqual(17.5m, (decimal)methodDecimal.Invoke(opClassObject, new object[] { 14m, 3.5m }));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            OperationConverter.ReplaceAdditOp(assemblyPath, newAssemblyPath);
            try
            {
                Assembly dll = Assembly.LoadFile(newAssemblyPath);
                if (dll != null)
                {
                    Type opType = dll.GetType("TestClassLibrary.Operations");
                    ConstructorInfo opConstructor = opType.GetConstructor(Type.EmptyTypes);
                    object opClassObject = opConstructor.Invoke(new object[] { });
                    if (opClassObject != null)
                    {
                        MethodInfo methodInt = opClassObject.GetType().GetMethod("CalculateInt");
                        MethodInfo methodFloat = opClassObject.GetType().GetMethod("CalculateFloat");
                        MethodInfo methodDecimal = opClassObject.GetType().GetMethod("CalculateDecimal");
                        Assert.AreEqual(-1, (int)methodInt.Invoke(opClassObject, new object[] { 2, 3 }));
                        Assert.AreEqual(3f, (float)methodFloat.Invoke(opClassObject, new object[] { 6f, 3f }));
                        Assert.AreEqual(10.5m, (decimal)methodDecimal.Invoke(opClassObject, new object[] { 14m, 3.5m }));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
