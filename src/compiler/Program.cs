using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace compiler
{
    class Program
    {
        const char TAB = '\t';
        char Look;

        //Read New Character From Input Stream
        void GetChar()
        {
            Read(out Look);
        }

        //report an error
        void Error(string s)
        {
            Console.WriteLine();
            Console.WriteLine(s);
        }

        void Abort(string s)
        {
            Error(s);
            System.Environment.Exit(0);
        }

        void Expected(string s)
        {
            Abort(s + " expected");
        }

        void Match(char x)
        {
            if (Look == x) 
                GetChar();
            else
                Expected("'" + x + "'");
        }

        bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        char GetName()
        {
            if(!IsAlpha(Look)) Expected("Name");
            var result = Look.ToString().ToUpper().ToCharArray()[0]; //this is horrible
            GetChar();
            return result;
        }

        char GetNum()
        {
            if(!IsDigit(Look)) Expected("Integer");
            var result = Look;
            GetChar();
            return result;
        }

        void Emit(string s)
        {
            Console.Write("{0}{1}", TAB, s);
        }

        void EmitLn(string s)
        {
            Emit(s);
            Console.WriteLine();
        }

        void Init()
        {

            GetChar();
        }

        void Read(out char c)
        {
            c = ' ';
        }




        static void Main(string[] args)
        {
            new Program().Init();
        }
    }
}
