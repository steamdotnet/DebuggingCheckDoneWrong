using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DebuggingCheckDetour
{
    class Program
    {
        [DllImport("kernel32")]
        private static extern bool VirtualProtect(IntPtr addr, int size, uint newProtect, out uint oldProtect);

        static void Main(string[] args)
        {
            Console.WriteLine(string.Format("Is Debugger Attached: {0}", Debugger.IsAttached));

            var isAttachedMethodInfo = typeof(Debugger).GetProperty("IsAttached").GetGetMethod();

            var myDebuggerIsAttachedMethodInfo = typeof(Program).GetMethod("MyDebuggerIsAttached");

            // Make sure methods are JITted.
            RuntimeHelpers.PrepareMethod(isAttachedMethodInfo.MethodHandle);
            RuntimeHelpers.PrepareMethod(myDebuggerIsAttachedMethodInfo.MethodHandle);

            // Get a pointer to the method's implementation
            var originalPointer = isAttachedMethodInfo.MethodHandle.GetFunctionPointer();

            // Get MyDebuggerIsAttached method address 
            byte[] myDebuggerIsAttachedAddress = BitConverter.GetBytes(myDebuggerIsAttachedMethodInfo.MethodHandle.GetFunctionPointer().ToInt32());

            // Jump to MyDebuggerIsAttached and ret
            var newCodeBytes = new byte[]
            {
                0x68, myDebuggerIsAttachedAddress[0], myDebuggerIsAttachedAddress[1], myDebuggerIsAttachedAddress[2], myDebuggerIsAttachedAddress[3], 0xC3
            };

            uint oldProtect = 0;
            VirtualProtect(originalPointer, 60, 64U, out oldProtect);
            Marshal.Copy(newCodeBytes, 0, originalPointer, 6);
            Console.WriteLine(string.Format("Is Debugger Attached: {0}", Debugger.IsAttached));

            // Run the debug target
            Console.WriteLine("Launching DebugTarget.exe");
            AppDomain.CurrentDomain.ExecuteAssembly(@"DebugTarget.exe");

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }


        public static bool MyDebuggerIsAttached() {
            Console.WriteLine("MyDebuggerIsAttached called");
            return false;
        }

    }
}
