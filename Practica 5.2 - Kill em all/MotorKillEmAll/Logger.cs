using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KillEmAll
{
    public class Logger
    {


        private static StreamWriter sr = new StreamWriter("log.txt");


        public static void Log(string mensaje)
        {
            sr.WriteLine(mensaje);
            sr.Flush();
        }

        


    }
}
