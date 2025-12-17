namespace fsd
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    public class Dummy
    {
        [STAThread]
        public static int Main(string[] p_args)
        {
            Console.WriteLine("Running with " + p_args.Length + " arguments:");

            for (int i = 0; i < p_args.Length; i++)
            {
                Console.WriteLine("args[" + i + "]=" + p_args[i]);
            }

            bool bUsage = true;

            string strConnectionString = Db.s_strConnectionTrusted;

            for (int i = 0; i < p_args.Length; i++)
            {
                if (p_args[i].ToUpper().Equals("-USER"))
                {
                    strConnectionString = Db.s_strConnectionUser;
                }
                else if (p_args[i].ToUpper().Equals("-DEBUG"))
                {
                    Db.s_bDebug = true;
                }
            }

            ScriptGenerator generator = new ScriptGenerator(strConnectionString);

            for (int i = 0; i < p_args.Length; i++)
            {
                if (p_args[i].ToUpper().Equals("-TABLE")&& p_args.Length >= i + 3)
                {
                    bUsage = false;
                    generator.OutputDirectory = p_args[i + 1];
                    generator.generateTable(p_args[i + 2]);
                    i += 2;
                }
                else if (p_args[i].ToUpper().Equals("-AREASUBAREAID") && p_args.Length >= i + 3)
                {
                    bUsage = false;
                    generator.OutputDirectory = p_args[i + 1];
                    generator.generateAreaSubareaId(p_args[i + 2], "", "");
                    i += 2;
                }
                else if (p_args[i].ToUpper().Equals("-PRIORITY") && p_args.Length >= i + 3)
                {
                    bUsage = false;
                    generator.OutputDirectory = p_args[i + 1];
                    generator.generatePriority(p_args[i + 2]);
                    i += 3;
                }
                else if (p_args[i].ToUpper().Equals("-AREASUBAREAID") && p_args.Length >= i + 4)
                {
                    bUsage = false;
                    generator.OutputDirectory = p_args[i + 1];
                    generator.generateAreaSubareaId(p_args[i + 2], p_args[i + 3], "");
                    i += 3;
                }
                else if (p_args[i].ToUpper().Equals("-FUNCTIONTABLEID") && p_args.Length >= i + 5)
                {
                    bUsage = false;
                    generator.OutputDirectory = p_args[i + 1];
                    generator.generateFunctionTableId(p_args[i + 2], p_args[i + 3], p_args[i + 4]);
                    i += 4;
                }
                else if (p_args[i].ToUpper().Equals("-AREASUBAREAID") && p_args.Length >= i + 5)
                {
                    bUsage = false;
                    generator.OutputDirectory = p_args[i + 1];
                    generator.generateAreaSubareaId(p_args[i + 2], p_args[i + 3], p_args[i + 4]);
                    i += 4;
                }
            }

            if (bUsage)
            {
                Console.WriteLine(ScriptGenerator.s_strUsage);
            }

            return 0;
        }
    }
}

