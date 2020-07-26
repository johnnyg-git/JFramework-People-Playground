using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace JFramework
{
    /// <summary>
    /// Used for creating and managing the console
    /// </summary>
    internal class ConsoleManager
    {
        #region imports
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleTitle(string lpConsoleTitle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName
            , [MarshalAs(UnmanagedType.U4)] DesiredAccess dwDesiredAccess
            , [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode
            , uint lpSecurityAttributes
            , [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition
            , [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes
            , uint hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetStdHandle(StdHandle nStdHandle, IntPtr hHandle);
        #endregion

        private enum StdHandle : int
        {
            Input = -10,
            Output = -11,
            Error = -12
        }

        [Flags]
        enum DesiredAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000
        }

        /// <summary>
        /// Creates the console and reroutes logging to it
        /// </summary>
        public static void CreateConsole()
        {
            if(Console.Read()!=-1)
            {
                Console.WriteLine("Attempt to make a console but one already exists");
                return;
            }
            // If console is created
            if (AllocConsole())
            {
                // Get the handle to CONOUT$.    
                var stdOutHandle = CreateFile("CONOUT$", DesiredAccess.GenericRead | DesiredAccess.GenericWrite, FileShare.ReadWrite, 0, FileMode.Open, FileAttributes.Normal, 0);

                if (stdOutHandle == new IntPtr(-1))
                    return;

                if (!SetStdHandle(StdHandle.Output, stdOutHandle))
                    return;

                // Creates new output
                var standardOutput = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

                // Sets output
                Console.SetOut(standardOutput);

                // Writes test line
                Console.WriteLine("Console created successfully!");
                //Reroute logging
                Application.logMessageReceived += HandleLog;
            }
        }

        /// <summary>
        /// Called on reroute of any log function
        /// </summary>
        /// <param name="logString"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        static void HandleLog(string logString, string stackTrace, LogType type)
        {
            Console.WriteLine(logString);
        }
    }
}