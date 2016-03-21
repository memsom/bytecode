using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RatCow.ByteCode;

namespace bytecode
{
    class Program
    {
         static void Main(string[] args)
        {
            uint[] bcode = new Assembler().Assemble("prog.asm");

            var s = new Assembler().Disassemble(bcode);

            System.Diagnostics.Debug.WriteLine("\r\n-----------------------");
            System.Diagnostics.Debug.WriteLine(s);
            System.Diagnostics.Debug.WriteLine("-----------------------\r\n");




            var interpreter1 = new Interpreter();
            interpreter1.ConsoleOutputEvent += interpreter_ConsoleOutputEvent;
            var task1 = InterpreterTask.RunTask(interpreter1, bcode);
            
            //var interpreter2 = new Interpreter();
            //interpreter2.ConsoleOutputEvent += interpreter_ConsoleOutputEvent;
            //var task2 = InterpreterTask.RunTask(interpreter2, bcode);
            
            //var interpreter3 = new Interpreter();
            //interpreter3.ConsoleOutputEvent += interpreter_ConsoleOutputEvent;
            //var task3 = InterpreterTask.RunTask(interpreter3, bcode);
            
            //var interpreter4 = new Interpreter();
            //interpreter4.ConsoleOutputEvent += interpreter_ConsoleOutputEvent;
            //var task4 = InterpreterTask.RunTask(interpreter4, bcode);

            //task4.Wait();
            //task3.Wait();
            //task2.Wait();
            task1.Wait();

            Console.ReadLine();
        }

         static void interpreter_ConsoleOutputEvent(string text)
         {
             System.Console.Write(text);
         }
    }
}
