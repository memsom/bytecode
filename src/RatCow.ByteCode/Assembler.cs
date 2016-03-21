using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RatCow.ByteCode
{
    public partial class Assembler
    {
        Dictionary<string, Instructions> instructionList = new Dictionary<string, Instructions>();
        void InitInstructionList()
        {
            var list = Enum.GetValues(typeof(Instructions)).Cast<Instructions>();
            foreach (var item in list)
            {
                instructionList.Add(item.ToString(), item);
            }
        }

        Dictionary<string, uint> labels = new Dictionary<string, uint>();
        Dictionary<uint, uint> lines = new Dictionary<uint, uint>();
        byte lineoffset = 0;
        Dictionary<uint, uint> fixuplist = new Dictionary<uint, uint>();

        public uint[] Assemble(string filename)
        {
            var result = new List<uint>();

            uint[] resultbytes = null;

            if (System.IO.File.Exists(filename))
            {
                InitInstructionList();

                var assembleyCode = new StringBuilder();

                using (var file = System.IO.File.OpenText(filename))
                {
                    assembleyCode.Append(file.ReadToEnd());
                }

                var assemblyLines = assembleyCode.ToString()
                    .Replace("\r", "")
                    .Replace("\t", " ")
                    .Split('\n');

                ParseLabels(assemblyLines);

                for (var i = 0; i < assemblyLines.Length; i++)
                {
                    var line = assemblyLines[i];
                    var tokens = line.Trim().Split(' ');
                    ParseTokens(result, tokens, (uint)i);
                }

                resultbytes = result.ToArray();

                foreach (var item in fixuplist)
                {
                    var address = GetAddressOfLine(item.Value);

                    resultbytes[item.Key] = address;
                }
            }

            foreach (var b in resultbytes)
            {
                System.Diagnostics.Debug.Write(String.Format("{0}, ", b));
            }

            return resultbytes;
        }

        private uint GetAddressOfLine(uint line)
        {
            uint offset = 0;
            //we now want to work out if there is any offset, so we need to look at the instructions logged for previous lines
            for (uint i = 0; i < line; i++)
            {
                if (lines.ContainsKey(i))
                {
                    offset += lines[i];
                }
            }

            return offset;
        }

        private void ParseLabels(string[] assemblyLines)
        {
            for (byte i = 0; i < assemblyLines.Length; i++)
            {
                if (assemblyLines[i].Contains(':'))
                {
                    var tlabel = assemblyLines[i].Remove(assemblyLines[i].IndexOf(':'));
                    var line = i;
                    labels.Add(tlabel, i);
                }

                if (assemblyLines[i].Trim().StartsWith("DW_"))
                {
                    var tokens = assemblyLines[i].Trim().Split(' ');
                    var tlabel = tokens[1];
                    var line = i;
                    labels.Add(tlabel, i);
                }
            }
        }

        private void ParseTokens(List<uint> result, string[] tokens, uint line)
        {
            //token 1 has to be an instruction or might have a label
            string tinstruction;
            byte offset = 1;
            if (tokens[0].Contains(':'))
            {
                tinstruction = String.Empty;
                for (int i = 1; i < tokens.Length; i++)
                {
                    if (tokens[i] == String.Empty) continue;

                    tinstruction = tokens[i];
                    offset = (byte)(i + 1);
                    break;
                }

            }
            else
            {
                tinstruction = tokens[0];
            }

            var instruction = instructionList.ContainsKey(tinstruction) ? instructionList[tinstruction] : Instructions.LBL;

            if (instruction == Instructions.LBL)
                return;

            if (!ExcludeIntruction(instruction))
                result.Add((byte)instruction);

            uint loffset = 0;

            switch (instruction)
            {
                case Instructions.ORG:

                    lines.Add(line, (2));
                    loffset = labels[tokens[offset]];
                    fixuplist.Add((byte)result.Count, loffset);
                    result.Add(loffset);

                    break;

                case Instructions.END:
                    lines.Add(line, (1));
                    break;

                case Instructions.RSR:
                    lines.Add(line, (1));
                    break;


                case Instructions.ADD_B:
                case Instructions.ADD_W:
                case Instructions.SUB_B:
                case Instructions.SUB_W:
                case Instructions.MUL_B:
                case Instructions.MUL_W:
                case Instructions.DIV_B:
                case Instructions.DIV_W:
                case Instructions.DIVF_B:
                case Instructions.DIVF_W:
                case Instructions.MOD_B:
                case Instructions.MOD_W:
                    lines.Add(line, (1));
                    break;

                case Instructions.ST_B:
                case Instructions.ST_W:
                    //the next token should be a number that fits in to a byte
                    byte stbins = 0;
                    if (byte.TryParse(tokens[offset], out stbins))
                    {
                        result.Add(stbins);
                        lines.Add(line, (2));
                    }
                    else
                    {
                        Console.WriteLine("Unsupported instruction :: {0} ::: {1}", instruction.ToString(), tokens[offset]);
                        return;
                    }
                    break;


                case Instructions.LD_B:
                case Instructions.LD_W:
                    //the next instruction will be a register name
                    if (tokens[offset] == Registers.V1.ToString())
                    {
                        result.Add((byte)Registers.V1);
                        lines.Add(line, (2));
                    }
                    else if (tokens[offset] == Registers.V2.ToString())
                    {
                        result.Add((byte)Registers.V2);
                        lines.Add(line, (2));
                    }
                    else
                    {
                        Console.WriteLine("Unsupported instruction :: {0} ::: {1}", instruction.ToString(), tokens[offset]);
                        return;
                    }
                    break;

                case Instructions.SM_B:
                case Instructions.SM_W:
                    //the next instruction will be a register name
                    if (tokens[offset] == Registers.V1.ToString())
                    {
                        result.Add((byte)Registers.V1);
                        lines.Add(line, (2));
                    }
                    else if (tokens[offset] == Registers.V2.ToString())
                    {
                        result.Add((byte)Registers.V2);
                    }
                    else
                    {
                        Console.WriteLine("Unsupported instruction :: {0} ::: {1}", instruction.ToString(), tokens[offset]);
                        return;
                    }
                    break;

                case Instructions.TRP:
                    //the next token should be a number that fits in to a byte
                    byte trp = 0;
                    if (byte.TryParse(tokens[offset], out trp))
                    {
                        result.Add(trp);
                        lines.Add(line, (2));
                    }
                    else
                    {
                        Console.WriteLine("Unsupported instruction :: {0} ::: {1}", instruction.ToString(), tokens[offset]);
                        return;
                    }
                    break;

                case Instructions.ST_S:
                    string s = tokens[offset];
                    byte count = MakeString(result, tokens, offset, s);
                    lines.Add(line, ((uint)(1 + count)));

                    break;


                case Instructions.CMP_B:
                case Instructions.CMP_W:
                    //the next token should be a number that fits in to a byte
                    byte cmp = 0;
                    if (byte.TryParse(tokens[offset], out cmp))
                    {
                        result.Add(cmp);
                        lines.Add(line, 2);
                    }
                    else
                    {
                        Console.WriteLine("Unsupported instruction :: {0} ::: {1}", instruction.ToString(), tokens[offset]);
                        return;
                    }
                    break;

                case Instructions.DM_S:

                    //add a label
                    string s2 = tokens[offset + 1];
                    byte count2 = MakeString(result, tokens, (uint)(offset + 1), s2);
                    lines.Add(line, count2);

                    break;

                //case instructions.STD_S:
                case Instructions.JNE:
                case Instructions.JMP:
                case Instructions.JSR:
                    string lp = tokens[offset];
                    if (labels.ContainsKey(lp))
                    {
                        lines.Add(line, (2));
                        loffset = labels[lp];
                        fixuplist.Add((uint)result.Count, loffset);
                        result.Add(loffset);
                    }
                    else
                    {
                        Console.WriteLine("Unsupported instruction :: {0} ::: {1}", instruction.ToString(), tokens[offset]);
                        return;
                    }
                    break;

                case Instructions.NOP:
                    lines.Add(line, 1);
                    break;

                default:
                    Console.WriteLine("Unsupported instruction :: {0}", instruction.ToString());
                    break;
            }
        }

        private bool ExcludeIntruction(Instructions instruction)
        {
            return instruction == Instructions.DM_B | instruction == Instructions.DM_S | instruction == Instructions.DM_W;
        }

        private byte MakeString(List<uint> result, string[] tokens, uint offset, string s)
        {
            var lineresult = result.Count;

            if (tokens.Length - offset > 1)
            {
                for (uint ii = offset + 1; ii < tokens.Length; ii++)
                {
                    s += " " + tokens[ii];
                }
            }

            bool delim = false;
            char ch = ' ';
            int i = 0;
            while (ch != '$')
            {
                ch = s[i];
                if (ch == '\'')
                {
                    delim = !delim;
                    i++;
                    continue;
                }
                if (!delim && ch == ',')
                {
                    //next is a char literal
                    var value = String.Empty;
                    while (i + 1 < s.Length && s[i + 1] != ',')
                    {
                        i++;
                        value += s[i];
                    }

                    if (value.StartsWith("$"))
                    {
                        result.Add(0);
                        break;
                    }
                    else
                    {
                        byte bvalue;
                        if (byte.TryParse(value, out bvalue))
                        {
                            result.Add(bvalue);
                        }

                    }
                }
                else
                    result.Add((byte)ch);


                i++;
            }

            return (byte)(result.Count - lineresult);
        }
    }

    partial class Assembler
    {
        public string Disassemble(uint[] bcode)
        {
            Instructions op = Instructions.NOP;
            uint pc = 0;
            uint line = 0;
            uint address = 0;


            var result = new StringBuilder();

            while (op != Instructions.END)
            {
                op = (Instructions)bcode[pc];
                address = pc;
                string args = String.Empty;

                switch (op)
                {
                    case Instructions.ST_W:
                    case Instructions.ST_B:
                    case Instructions.LD_B:
                    case Instructions.SM_B:
                    case Instructions.LD_W:
                    case Instructions.SM_W:
                    case Instructions.LD_S:
                    case Instructions.SM_S:
                    case Instructions.JE:
                    case Instructions.JNE:
                    case Instructions.JMP:
                    case Instructions.JSR:
                    case Instructions.CMP_B:
                    case Instructions.TRP:
                        pc++;
                        args = bcode[pc].ToString();
                        break;

                    case Instructions.ORG:
                        pc++;
                        var startlocation = bcode[pc];
                        args = startlocation.ToString();
                        pc++;

                        if (startlocation - pc != 1)
                        {
                            args += "\r\n\t";
                            var oline = 0;
                            while (pc < startlocation)
                            {
                                args += " " + bcode[pc].ToString();
                                oline++;
                                if (oline == 9 || bcode[pc] == 0)
                                {
                                    args += "\r\n\t";
                                    oline = 0;
                                }
                                pc++;
                            }
                        }

                        pc = startlocation - 1;

                        break;

                    case Instructions.ST_S:

                        bool delim = false;
                        while (bcode[pc] != 0)
                        {
                            pc++;

                            if (bcode[pc] > 31)
                            {
                                if (!delim)
                                {
                                    args += "'";
                                    delim = true;
                                }

                                args += (char)bcode[pc];
                            }
                            else
                            {
                                if (delim)
                                {
                                    args += "'";
                                    delim = false;
                                }

                                if (bcode[pc] == 0)
                                {
                                    args += ",$";
                                }
                                else
                                    args += "," + bcode[pc].ToString();

                            }

                        }


                        break;

                    case Instructions.RSR:
                    case Instructions.NOP:
                    case Instructions.ADD_B:
                    case Instructions.SUB_B:
                        break;


                }

                result.AppendLine(String.Format("{0}::{1}  {2} {3}", address, line, op.ToString(), args));

                line++;
                pc++;
            }

            return result.ToString();
        }
    }
}
