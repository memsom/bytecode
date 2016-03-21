using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RatCow.ByteCode;

namespace GUITestbed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();           
        }

        Bitmap drawbuffer = new Bitmap(256, 256);

        private void Form1_Load(object sender, EventArgs e)
        {
            uint[] bcode = new Assembler().Assemble("prog.asm");

            var s = new Assembler().Disassemble(bcode);

            System.Diagnostics.Debug.WriteLine("\r\n-----------------------");
            System.Diagnostics.Debug.WriteLine(s);
            System.Diagnostics.Debug.WriteLine("-----------------------\r\n");

            timer1.Enabled = true;

            var interpreter1 = new Interpreter();
            interpreter1.GraphicsOutputEvent += interpreter1_GraphicsOutputEvent;
            var task1 = InterpreterTask.RunTask(interpreter1, bcode);

            task1.ContinueWith(
                t =>
                t.Exception.Handle(ex =>
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message); 
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    return false;
                }), 
                TaskContinuationOptions.OnlyOnFaulted
            );
        }

        void interpreter1_GraphicsOutputEvent(uint x, uint y, byte colour)
        {
            lock (drawbuffer)
            {
                drawbuffer.SetPixel((int)x, (int)y, GetColor(colour)); 
            }
        }

        /// <summary>
        /// Almost Speccy colours
        /// </summary>
        private Color GetColor(byte colour)
        {
            switch (colour)
            {
                case 0:
                    return Color.Black;
                case 1:
                    return Color.Blue;
                case 2:
                    return Color.Red;
                case 3: return 
                    Color.Magenta;
                case 4:
                    return Color.Green;
                case 5:
                    return Color.Cyan;
                case 6:
                    return Color.Yellow;
                    
                default:
                    return Color.White;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (drawbuffer)
            {
                var g = this.CreateGraphics();
                g.DrawImage(drawbuffer, 0, 0);
            }
        }
    }
}
