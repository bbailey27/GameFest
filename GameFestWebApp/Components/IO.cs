using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace GameFestWebApp.Components
{
    public class IO
    {
        string winDir = System.Environment.GetEnvironmentVariable("windir");

        public void WriteFile(List<string> lines)
        {
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            File.WriteAllLines(@"C:\\Users\\Bridget\\Documents\\GameFestOutput.txt", lines);

        }
    }
}