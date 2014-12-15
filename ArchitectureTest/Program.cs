using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureTest
{
    class Program

    {
        static void Main(string[] args)
        {
            String[] files = new String[] { 
                @"C:\Users\Wes\Document\Sample\Sample\bin\Debug\Sample.dll", //compiled with the target architecture set to 64
                @"C:\work\tmp\server.dll",  //pulled from the server
                @"C:\work\tmp\32-bit-developer-box.dll", //pulled from a 32-bit developer box
                @"C:\work\tmp\ACA.Entity.wes.64.dll" // ran /editbin /largeaddressaware 
            };
            foreach(String file in files){
                Console.Out.WriteLine(file + ":"+ IsLargeAware(file));
            }
        }

        static bool IsLargeAware(string file)
        {
            using (var fs = File.OpenRead(file))
            {
                return IsLargeAware(fs);
            }
        }

        static bool IsLargeAware(Stream stream)
        {
            const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;

            var br = new BinaryReader(stream);

            if (br.ReadInt16() != 0x5A4D)       //No MZ Header
                return false;

            br.BaseStream.Position = 0x3C;
            var peloc = br.ReadInt32();         //Get the PE header location.

            br.BaseStream.Position = peloc;
            if (br.ReadInt32() != 0x4550)       //No PE header
                return false;

            br.BaseStream.Position += 0x12;
            return (br.ReadInt16() & IMAGE_FILE_LARGE_ADDRESS_AWARE) == IMAGE_FILE_LARGE_ADDRESS_AWARE;
        }
    }
}
