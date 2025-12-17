using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryToBase64
{
    class Program
    {
        public void EncodeWithString(string filename)
        {
            System.IO.FileStream inFile;
            byte[] binaryData;
            string outputFileName = filename + ".base64";

            try
            {
                inFile = new System.IO.FileStream(filename,
                                                  System.IO.FileMode.Open,
                                                  System.IO.FileAccess.Read);
                binaryData = new Byte[inFile.Length];
                long bytesRead = inFile.Read(binaryData, 0,
                                            (int)inFile.Length);
                inFile.Close();
            }
            catch (System.Exception exp)
            {
                // Error creating stream or reading from it.
                System.Console.WriteLine("{0}", exp.Message);
                return;
            }

            // Convert the binary input into Base64 UUEncoded output.
            string base64String;
            try
            {
                base64String =
                   System.Convert.ToBase64String(binaryData,
                                                 0,
                                                 binaryData.Length);
            }
            catch (System.ArgumentNullException)
            {
                System.Console.WriteLine("Binary data array is null.");
                return;
            }

            // Write the UUEncoded version to the output file.
            System.IO.StreamWriter outFile;
            try
            {
                outFile = new System.IO.StreamWriter(outputFileName,
                                            false,
                                            System.Text.Encoding.ASCII);
                outFile.Write(base64String);
                outFile.Close();

                System.Console.WriteLine(string.Format("Created file {0}", outputFileName));
            }
            catch (System.Exception exp)
            {
                // Error creating stream or writing to it.
                System.Console.WriteLine("{0}", exp.Message);
            }
        }
        static void Main(string[] args)
        {
            System.Console.WriteLine("Convert binary file to base64 text.");
            System.Console.WriteLine("Usage: BianryToBase64 <filename>");
            if (args.Length == 1)
            {
                Program p = new Program();
                p.EncodeWithString(args[0]);
            }
        }
    }
}
