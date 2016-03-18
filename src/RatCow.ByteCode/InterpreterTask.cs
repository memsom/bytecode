using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatCow.ByteCode
{
    public class InterpreterTask
    {
        public static Task RunTask(Interpreter interpreter, uint[] bytecode)
        {
            var result = Task.Factory.StartNew(() => {

                interpreter.Run(bytecode);
            
            });

            return result;
        }
    }
}
