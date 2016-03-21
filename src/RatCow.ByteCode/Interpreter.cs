using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RatCow.ByteCode
{
    //at the moment, WORDS and STRINGS are in BIG ENDIAN format
    public partial class Interpreter
    {
        public delegate void ConsoleOutputDelegate(string text);
        public event ConsoleOutputDelegate ConsoleOutputEvent = null;
        private void CallConsoleOutputEvent(string text)
        {
            var tConsoleOutputEvent = ConsoleOutputEvent;
            if (tConsoleOutputEvent != null)
            {
                tConsoleOutputEvent(text);
            }
        }

        public delegate void ConsoleIntputDelegate(out byte key);
        public event ConsoleIntputDelegate ConsoleIntputEvent = null;
        private void CallConsoleIntputEvent(out byte key)
        {
            var tConsoleIntputEvent = ConsoleIntputEvent;
            if (tConsoleIntputEvent != null)
            {
                tConsoleIntputEvent(out key);
            }
            else
            {
                key = 0;
            }
        }

        public delegate void GraphicsOutputDelegate(uint x, uint y, byte colour);
        public event GraphicsOutputDelegate GraphicsOutputEvent = null;
        private void CallGraphicsOutputEvent(uint x, uint y, byte colour)
        {
            var tGraphicsOutputEvent = GraphicsOutputEvent;
            if (tGraphicsOutputEvent != null)
            {
                tGraphicsOutputEvent(x, y, colour);
            }
        }


        uint[] program = null;

        const byte TRP_CONSOLEWRITE = 21;
        const byte TRP_CONSOLEREAD = 22; //reads a character from the keyboard - there is no waiting.
        const byte TRP_CONSOLEREAD_WAIT = 23; //reads with a wait
        const byte TRP_GUIDRAW = 25; //takes the last 3 stack items in reverse order (colour, y, x) and passes to the graphics system

        uint pc = 0;

        object v1 = null;
        object v2 = null;


        uint Next()
        {
            var result = program[pc];
            pc++;
            return result;
        }

        string NextString()
        {
            var s = new StringBuilder();
            uint value = 254;

            while (value > 0)
            {
                value = Next();

                if (value > 0)
                    s.Append((char)value);
            }

            return s.ToString();
        }

        Stack<object> stack = new Stack<object>();

        public void Run(uint[] bytecode)
        {
            program = bytecode;

            Instructions op = Instructions.NOP;
            do
            {
                op = (Instructions)Next();

                byte register = 0;
                uint address = 0;

                switch (op)
                {
                    case Instructions.ORG:
                        pc = Next();
                        break;

                    case Instructions.LD_B:
                        register = System.Convert.ToByte(Next());
                        LD_Register(register);
                        break;

                    case Instructions.SM_B:
                        register = System.Convert.ToByte(Next());
                        SM_Register(register);
                        break;

                    case Instructions.ST_B:
                        var bvalue = Next();
                        ST_B_Stack(System.Convert.ToByte(bvalue));
                        break;

                    case Instructions.LD_W:
                        register = System.Convert.ToByte(Next());
                        LD_Register(register);
                        break;

                    case Instructions.SM_W:
                        register = System.Convert.ToByte(Next());
                        SM_Register(register);
                        break;

                    case Instructions.ST_W:
                        var wvalue = (UInt16)Next();
                        ST_W_Stack(wvalue);
                        break;

                    case Instructions.LD_S:
                        register = System.Convert.ToByte(Next());
                        LD_Register(register);
                        break;

                    case Instructions.SM_S:
                        register = System.Convert.ToByte(Next());
                        SM_Register(register);
                        break;

                    case Instructions.ST_S:
                        var svalue = NextString();
                        ST_S_Stack(svalue);
                        break;

                    case Instructions.ADD_B:
                        var aval2 = System.Convert.ToByte(stack.Pop());
                        var aval1 = System.Convert.ToByte(stack.Pop());
                        stack.Push(System.Convert.ToByte(aval1 + aval2));
                        break;

                    case Instructions.ADD_W:
                        var avalw2 = (UInt16)stack.Pop();
                        var avalw1 = (UInt16)stack.Pop();
                        stack.Push((UInt16)(avalw1 + avalw2));
                        break;

                    case Instructions.JMP:
                        address = Next();
                        pc = address;
                        break;

                    case Instructions.JNE:
                        address = Next();
                        bool CMP_result = (bool)stack.Pop();
                        if (!CMP_result)
                        {
                            pc = address;
                        }

                        break;

                    case Instructions.JSR:
                        address = Next();
                        //stack.Push(pc); //return address, though we create no stack frame, so this could all go horribly wrong....
                        stack.Push(new StackFrame() { ReturnAddress = System.Convert.ToByte(pc) });
                        pc = address;
                        break;

                    case Instructions.RSR:
                        var pca = UnwindStack();
                        pc = Convert.ToByte(pca);
                        break;

                    case Instructions.CMP_B:
                        var comparison = Next();
                        var comparitor = System.Convert.ToByte(stack.Pop());
                        stack.Push(comparitor == comparison);
                        break;

                    case Instructions.CMP_W:
                        var comparisonw = Next();
                        var comparitorw = (UInt16)stack.Pop();
                        stack.Push(comparitorw == comparisonw);
                        break;

                    case Instructions.TRP:
                        var trpCommand = Next();
                        switch (trpCommand)
                        {
                            case TRP_CONSOLEWRITE:
                                string s = PopString();
                                CallConsoleOutputEvent(s);
                                break;

                            case TRP_GUIDRAW:
                                var gc = System.Convert.ToByte(stack.Pop());
                                var gy = System.Convert.ToUInt32(stack.Pop());
                                var gx = System.Convert.ToUInt32(stack.Pop());

                                CallGraphicsOutputEvent(gx, gy, gc);
                                break;

                            default:
                                throw new Exception(String.Format("Trap not implemented {0}", trpCommand));
                        }
                        break;

                    case Instructions.NOP:
                        break;
                }

            } while (op != Instructions.END);
        }

        private uint UnwindStack()
        {
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                if (item is StackFrame)
                {
                    return (item as StackFrame).ReturnAddress;
                }
            }

            throw new StackOverflowException("Stack has no items left");
        }

        private void ST_B_Stack(byte value)
        {
            stack.Push(value);
        }

        private void ST_W_Stack(UInt16 value)
        {
            stack.Push(value);
        }

        //this needs to become a byte array at some point as this is a cheat
        private void ST_S_Stack(string value)
        {
            stack.Push(value);
        }

        private void SM_Register(byte register)
        {
            var value = GetRegisterValue(register);
            stack.Push(value);
        }


        private void LD_Register(byte register)
        {
            var value = System.Convert.ToByte(stack.Pop());
            SetRegisterValue(register, value);
        }

        private void SetRegisterValue(byte register, object p)
        {
            switch ((Registers)register)
            {
                case Registers.V1:
                    v1 = p;
                    break;
                case Registers.V2:
                    v2 = p;
                    break;
                default:
                    throw new Exception(String.Format("Invalid register assignment {0}", register));
            }
        }

        private object GetRegisterValue(byte register)
        {
            switch ((Registers)register)
            {
                case Registers.V1:
                    return v1;

                case Registers.V2:
                    return v2;

                default:
                    throw new Exception(String.Format("Invalid register assignment {0}", register));
            }
        }

        private string PopString()
        {
            return stack.Pop().ToString();
        }
    }
}
