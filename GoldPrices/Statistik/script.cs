namespace fsd
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    public class ScriptGenerator
    {
        private const string COLUMN_TYPE_STRING = "STRING";
        private const string COLUMN_TYPE_NUMBER = "NUMBER";
        private const string COLUMN_TYPE_FLAG   = "FLAG";
        private const string COLUMN_TYPE_HEX    = "HEX";

        private SqlConnection   m_sqlConnection;
        private SqlCommand      m_sqlCommand;
        private SqlDataReader   m_sqlDataReader;

        private SqlConnection   m_sqlConnection2;
        private SqlCommand      m_sqlCommand2;
        private SqlDataReader   m_sqlDataReader2;

        private ArrayList       m_arTFUN;
        private ArrayList       m_arTTAB;
        private ArrayList       m_arTTFU;
        private ArrayList       m_arTableData;

        private ProgressBar     m_progress;

        private string          m_strDirectory;
        private bool            m_fUseSimpleBuildtest;

        public const string s_strUsage = 
              "\nUsage:"
            + "\n-table <directory> <tablename>"
            + "\n-priority <directory> <priority range list>"
            + "\n-areaSubareaId <directory> <area> [[<subarea>] [<id>]]"
            + "\n-functiontableid <directory> <function name> <table name> <id>";

        private const string s_strSQL_COMMON = "SELECT ttfu, ttfu_comment, ttfu_flag, tfun.tfun, tfun_name, ttab.ttab, ttab_name, dtty_id, ttcd";

        public ScriptGenerator(string p_strConnectionString) : this(null, p_strConnectionString)
        {
        }

        public ScriptGenerator(ProgressBar p_progressBar, string p_strConnectionString)
        {

            m_strDirectory = System.IO.Path.GetTempPath() + @"\\sxstest-include";
            m_fUseSimpleBuildtest = false;

            m_progress = p_progressBar;

            m_arTTFU = new ArrayList();
            m_arTFUN = new ArrayList();
            m_arTTAB = new ArrayList();
            m_arTableData = new ArrayList();

            Db.ConnectionString = p_strConnectionString;

            m_sqlConnection = Db.getNewSqlConnection();
            m_sqlConnection2 = Db.getNewSqlConnection();

            m_sqlCommand = m_sqlConnection.CreateCommand();
            m_sqlCommand2 = m_sqlConnection2.CreateCommand();
        }

        ~ScriptGenerator()
        {
        }

        public string OutputDirectory
        {
            get { return m_strDirectory; }
            set { m_strDirectory = value; }
        }

        public bool UseSimpleBuildTests
        {
            get { return m_fUseSimpleBuildtest; }
            set { m_fUseSimpleBuildtest = value; }
        }

        private void generateCommon(string p_strSql)
        {
            int nTTAB = -1;
            TableData td = null;

            m_sqlCommand.CommandText = p_strSql;
            m_sqlDataReader = m_sqlCommand.ExecuteReader();
    
            while (m_sqlDataReader.Read())
            {
                CTTAB ttab = new CTTAB(m_sqlDataReader.GetInt32(5), 
                    m_sqlDataReader.GetString(6),
                    m_sqlDataReader.GetString(7));
                CTFUN tfun = new CTFUN(m_sqlDataReader.GetInt32(3), m_sqlDataReader.GetString(4));
                CTTFU ttfu = new CTTFU(tfun, ttab, m_sqlDataReader.GetString(1), m_sqlDataReader.GetString(2));

                Util.addToListIfNotExists(m_arTTFU, ttfu);
                Util.addToListIfNotExists(m_arTFUN, tfun);
                Util.addToListIfNotExists(m_arTTAB, ttab);

                if (nTTAB != m_sqlDataReader.GetInt32(5))
                {
                    nTTAB = m_sqlDataReader.GetInt32(5);
                    if (td != null)
                    {
                        Util.addToListIfNotExists(m_arTableData, td);
                    }
                    td = new TableData(ttab, TableData.GENERATE_MODE.SELECTION);
                }
                td.AddTTCD(m_sqlDataReader.GetInt32(8));
            }
            m_sqlDataReader.Close();
            if (td != null)
            {
                Util.addToListIfNotExists(m_arTableData, td);
            }

            generate();
        }

        //
        // all call that use this test case data
        //
        public void generateTTCD(int p_nTTCD)
        {
            string strSql = s_strSQL_COMMON
                + " FROM ttfu"
                + " INNER JOIN tfun ON ttfu.tfun=tfun.tfun"
                + " INNER JOIN ttcd ON ttfu.ttab=ttcd.ttab"
                + " INNER JOIN ttab ON ttfu.ttab=ttab.ttab"
                + " INNER JOIN dtty ON ttab.dtty=dtty.dtty"
                + " WHERE"
                + " ttcd=" + p_nTTCD
                + " ORDER BY ttfu";

            generateCommon(strSql);
        }

        public void generateFunctionTableId(string p_strFunctionName, string p_strTableName, string p_strId)
        {
            string strSql = s_strSQL_COMMON
                + " FROM ttfu"
                + " INNER JOIN tfun ON ttfu.tfun=tfun.tfun"
                + " INNER JOIN ttcd ON ttfu.ttab=ttcd.ttab"
                + " INNER JOIN ttab ON ttfu.ttab=ttab.ttab"
                + " INNER JOIN dtty ON ttab.dtty=dtty.dtty"
                + " WHERE"
                + " ttab_name='" + p_strTableName + "'"
                + " AND tfun_name='" + p_strFunctionName + "'"
                + " AND ttcd_id='" + p_strId + "'"
                + " ORDER BY ttfu, ttcd";

            generateCommon(strSql);
        }

        public void generateAreaSubareaId(string p_strArea, string p_strSubarea, string p_strId)
        {
            string strSql = s_strSQL_COMMON
                + " FROM ttfu"
                + " INNER JOIN tfun ON ttfu.tfun=tfun.tfun"
                + " INNER JOIN ttcd ON ttfu.ttab=ttcd.ttab"
                + " INNER JOIN ttab ON ttfu.ttab=ttab.ttab"
                + " INNER JOIN dtty ON ttab.dtty=dtty.dtty"
                + " LEFT OUTER JOIN dare tfun_dare ON tfun.dare=tfun_dare.dare"
                + " LEFT OUTER JOIN dare ttcd_dare ON ttcd.dare=ttcd_dare.dare";

            if (p_strSubarea.Length > 0)
            {
                strSql += " INNER JOIN dsae tfun_dsae ON tfun.dsae=tfun_dsae.dsae";
                strSql += " INNER JOIN dsae ttcd_dsae ON ttcd.dsae=ttcd_dsae.dsae";
            }

            strSql += " WHERE";
            strSql += " (tfun_dare.dare_id='" + p_strArea + "' OR ttcd_dare.dare_id='" + p_strArea + "')";

            if (p_strSubarea.Length > 0)
            {
                strSql += " AND (tfun_dsae.dsae_id='" + p_strSubarea + "' OR ttcd_dsae.dsae_id='" + p_strSubarea + "')";
            }

            if (p_strId.Length > 0)
            {
                strSql += " AND ttcd_id='" + p_strId + "'";
            }

            strSql += " ORDER BY ttfu, ttcd";

            generateCommon(strSql);
        }

        public void generatePriority(string p_strPriorityRangeList)
        {
            string flatList = flatListFromRangeList(p_strPriorityRangeList);

            string strSql = s_strSQL_COMMON
                + " FROM ttfu"
                + " INNER JOIN tfun ON ttfu.tfun=tfun.tfun"
                + " INNER JOIN ttcd ON ttfu.ttab=ttcd.ttab"
                + " INNER JOIN ttab ON ttfu.ttab=ttab.ttab"
                + " INNER JOIN dtty ON ttab.dtty=dtty.dtty"
                + " WHERE"
                + " tfun_priority in (" + flatList + ") OR ttcd_priority IN (" + flatList + ")"
                + " ORDER BY ttfu, ttcd";

            generateCommon(strSql);
        }

        public void generateAll()
        {
            string strSql = s_strSQL_COMMON
                + " FROM ttfu"
                + " INNER JOIN tfun ON ttfu.tfun=tfun.tfun"
                + " INNER JOIN ttcd ON ttfu.ttab=ttcd.ttab"
                + " INNER JOIN ttab ON ttfu.ttab=ttab.ttab"
                + " INNER JOIN dtty ON ttab.dtty=dtty.dtty"
                + " ORDER BY ttfu, ttcd";

            generateCommon(strSql);
        }

        public void generateTable(string p_strTableName)
        {
            string strSql = s_strSQL_COMMON
                + " FROM ttfu"
                + " INNER JOIN tfun ON ttfu.tfun=tfun.tfun"
                + " INNER JOIN ttcd ON ttfu.ttab=ttcd.ttab"
                + " INNER JOIN ttab ON ttfu.ttab=ttab.ttab"
                + " INNER JOIN dtty ON ttab.dtty=dtty.dtty"
                + " WHERE ttab_name='" + p_strTableName + "'"
                + " ORDER BY ttfu, ttcd";

            generateCommon(strSql);
        }

        private void generate()
        {
            FileStream fs;
            StreamWriter sw;
            string strFileName;
            int i;

            progressInitialize();

            //
            // Create path
            //
            System.IO.Directory.CreateDirectory(m_strDirectory);

            //
            // TABLE_Win32Apps.js
            //
            if (m_arTTAB.Count > 0)
            {
                strFileName = m_strDirectory + "\\" + "TABLE_Win32Apps.js";
                Console.WriteLine("Writing " + m_arTTAB.Count + " entries to file " + strFileName);
                fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);

                sw.WriteLine(@"//");
                sw.WriteLine(@"// File TABLE_Win32Apps.js");
                sw.WriteLine(@"//");
                sw.WriteLine(@"// Generated " + DateTime.Now.ToLongDateString());
                sw.WriteLine(@"//");
                sw.WriteLine();

                for (i = 0; i < m_arTTAB.Count; i++)
                {
                    progressUpdate();
                    CTTAB ttab = (CTTAB) m_arTTAB[i];
                    sw.WriteLine("#include <" + ttab.TTAB_NAME.Trim() + ".js>");
                }
                sw.Flush();
                sw.Close();
            }

            if (m_arTFUN.Count > 0)
            {
                //
                // TC_Win32Apps.js
                //
                strFileName = m_strDirectory + "\\" + "TC_Win32Apps.js";
                Console.WriteLine("Writing " + m_arTFUN.Count + " entries to file " + strFileName);
                fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);

                sw.WriteLine(@"//");
                sw.WriteLine(@"// File TC_Win32Apps.js");
                sw.WriteLine(@"//");
                sw.WriteLine(@"// Generated " + DateTime.Now.ToLongDateString());
                sw.WriteLine(@"//");
                sw.WriteLine();

                for (i = 0; i < m_arTFUN.Count; i++)
                {
                    progressUpdate();
                    CTFUN tfun = (CTFUN) m_arTFUN[i];
                    sw.WriteLine("#include <" + tfun.TFUN_NAME.Trim() + ".js>");
                }
                sw.Flush();
                sw.Close();
            }

            if (m_arTTFU.Count > 0)
            {
                string strBareFunctionName;
                int nIndex;

                //
                // Run_Win32Apps.js
                //
                strFileName = m_strDirectory + "\\" + "Run_AllTests.js";
                Console.WriteLine("Writing " + m_arTTFU.Count + " entries to file " + strFileName);
                fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);

                sw.WriteLine(@"//");
                sw.WriteLine(@"// File Run_AllTests.js");
                sw.WriteLine(@"//");
                sw.WriteLine(@"// Generated " + DateTime.Now.ToLongDateString());
                sw.WriteLine(@"//");
                sw.WriteLine();
                sw.WriteLine();
                if (m_fUseSimpleBuildtest)
                {
                    sw.WriteLine("#include <buildtest_simple_h.js>");
                    sw.WriteLine();
                    sw.WriteLine();
                }
                sw.WriteLine("if (Initialize())");
                sw.WriteLine("{");
                sw.WriteLine("    RunAllGeneratedTests();");
                sw.WriteLine("}");
                sw.WriteLine("else");
                sw.WriteLine("{");
                sw.WriteLine("    ERROR(\"Initialization failure\");");
                sw.WriteLine("    alert(\"Error during initialization, check log for details\");");
                sw.WriteLine("}");
                sw.WriteLine();
                sw.WriteLine("Finalize();");
                sw.WriteLine();
                sw.WriteLine("function RunAllGeneratedTests()");
                sw.WriteLine("{");
                for (i = 0; i < m_arTTFU.Count; i++)
                {
                    progressUpdate();
                    CTTFU ttfu = (CTTFU) m_arTTFU[i];
                    strBareFunctionName = ttfu.TFUN.TFUN_NAME.Trim();
                    nIndex = strBareFunctionName.LastIndexOf("\\");
                    if (nIndex != -1)
                    {
                        strBareFunctionName = strBareFunctionName.Substring(nIndex + 1);
                    }
                    sw.WriteLine("    if (glFlags & " + ttfu.TTFU_FLAG + ")");
                    sw.WriteLine("    {");
                    sw.WriteLine("        tstRunAllSections(\"" + ttfu.TTFU_COMMENT 
                        + "\", Create" + ttfu.TTAB.TTAB_NAME + "Section(), Create" 
                        + ttfu.TTAB.TTAB_NAME + "(), " + strBareFunctionName + ");");
                    sw.WriteLine("    }");
                }
                sw.WriteLine("}");
                sw.Flush();
                sw.Close();
            }

            //
            // Each individual TABLE_*.js file
            //
            for (i = 0; i < m_arTableData.Count; i++)
            {
                progressUpdate();
                TableData td = (TableData) m_arTableData[i];

                Console.WriteLine("Writing file " + td.TTAB.TTAB_NAME + ".js");
                
                if (td.TTAB.DTTY_ID.Equals(Db.s_strDEFTABLE_DTTY_DEFAULT))
                {
                    generateTableSingle(td);
                }
                else
                {
                    generateTableCommand(td);
                }
            }

            progressDone();
        }

        private void generateTableSingle(TableData p_td)
        {
            int nTTAB;
            int nTTCD;

            int iMask;

            int nSection;
            int lngStartRow = -1;
            int lngRowsinSection;

            string strTableName;
            string strSectionTableName;
            string strOutFile;
            string strSectionName = "";
            string strValue;
            string strDCTY_ID;
            string strOut;
            string strListTTCD = IntArrayToList(p_td.TTCDList, ",");

            bool bFieldWritten;

            ArrayList colSections = new ArrayList();

            m_sqlCommand.CommandText = "SELECT ttab, ttab_name from ttab where ttab_name = '" + p_td.TTAB.TTAB_NAME + "'";
            m_sqlDataReader = m_sqlCommand.ExecuteReader();
            if (!m_sqlDataReader.Read())
            {
                m_sqlDataReader.Close();
                return;
            }

            nTTAB = m_sqlDataReader.GetInt32(0);
            strTableName = m_sqlDataReader.GetString(1).Trim();
            strSectionTableName = strTableName + "Section";
            m_sqlDataReader.Close();

            strOutFile = strTableName + ".js";

            FileStream fs = new FileStream(m_strDirectory + @"\" + strOutFile,
                FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter txtFile = new StreamWriter(fs);

            txtFile.WriteLine("#ifndef _" + strTableName.ToUpper() + "_JS_");
            txtFile.WriteLine ("#define _" + strTableName.ToUpper() + "_JS_");
            txtFile.WriteLine("");
            txtFile.WriteLine("");

            txtFile.WriteLine ("////////////////////////////////////////");
            txtFile.WriteLine ("//");
            txtFile.WriteLine ("// TABLE: " + strTableName.ToUpper());
            txtFile.WriteLine ("//");
            txtFile.WriteLine ("// generated from sxstest database on " + DateTime.Now.ToLongDateString());
            txtFile.WriteLine ("//");
            txtFile.WriteLine ("////////////////////////////////////////");
            txtFile.WriteLine("");
            txtFile.WriteLine("");
    
    
            //
            // row function header
            //
            string strRowFuncName = "Create_" + strTableName + "_Row";
            string strSectionFuncName = "Create_" + strTableName + "Section";
    
            m_sqlCommand.CommandText = "SELECT tcol_name FROM tcol WHERE ttab = " + nTTAB + " order by tcol_colno";
            m_sqlDataReader = m_sqlCommand.ExecuteReader();
        
            string strLine = "function " + strRowFuncName + "(Area, SubArea, Id, Priority, Result, Description, VerboseDescription";
            while (m_sqlDataReader.Read())
            {
                strLine += ", " + m_sqlDataReader.GetString(0);
            }
            m_sqlDataReader.Close();
            strLine = strLine + ")";
            txtFile.WriteLine(strLine);
    
    
            //
            // row function text
            //
            txtFile.WriteLine ("{");
            txtFile.WriteLine ("    this.Area = Area;");
            txtFile.WriteLine ("    this.SubArea = SubArea;");
            txtFile.WriteLine ("    this.Id = Id;");
            txtFile.WriteLine ("    this.Priority = Priority;");
            txtFile.WriteLine ("    this.Result = Result;");
            txtFile.WriteLine ("    this.Description = Description;");
            txtFile.WriteLine ("    this.VerboseDescription = VerboseDescription;");

            m_sqlDataReader = m_sqlCommand.ExecuteReader();
            while (m_sqlDataReader.Read())
            {
                txtFile.WriteLine ("    this." +  m_sqlDataReader.GetString(0) + " = " + m_sqlDataReader.GetString(0) + ";");
            }
            m_sqlDataReader.Close();

            txtFile.WriteLine ("}");


            //
            // test case data table
            //
            txtFile.WriteLine();
            txtFile.WriteLine ("var g_" + strTableName + " = null;");
            txtFile.WriteLine();
            
            txtFile.WriteLine ("function Create" + strTableName + "()");
            txtFile.WriteLine ("{");
            txtFile.WriteLine ("  if (g_" + strTableName + " == null)");
            txtFile.WriteLine ("  {");
            txtFile.WriteLine ("    g_" + strTableName + " = new Array(");
            txtFile.WriteLine ("    new " + strRowFuncName + "(null, null, null, null, null, " + "\"" + strTableName + "\"" + ")");

            //
            // cycle through all test case data records
            //
            m_sqlCommand.CommandText = "SELECT ttcd, dare_id, dsae_id, ttcd_id, ttcd_priority, ttcd_result"
                + ", ttcd_description, ttcd_verbosedescription, tsec_name, ttcd.tsec_no"
                + " FROM tsec INNER JOIN ttcd ON tsec.ttab=ttcd.ttab AND tsec.tsec_no=ttcd.tsec_no"
                + " LEFT OUTER JOIN dare ON ttcd.dare=dare.dare"
                + " LEFT OUTER JOIN dsae ON ttcd.dsae=dsae.dsae"
                + " WHERE tsec.ttab=" + nTTAB;
            if (p_td.Mode == TableData.GENERATE_MODE.SELECTION && p_td.TTCDList.Count > 0)
            {
                m_sqlCommand.CommandText += " AND ttcd IN " + strListTTCD;
            }
            m_sqlCommand.CommandText += " ORDER BY ttcd.tsec_no, ttcd_no";

            m_sqlDataReader = m_sqlCommand.ExecuteReader();
    
            bool blnFirstSection = true;
            lngRowsinSection = 0;
            int lngRptCnt = 0;
            int nTSEC_NO = 0;
            while (m_sqlDataReader.Read())
            {
                nTTCD = m_sqlDataReader.GetInt32(0);
                if (nTSEC_NO != m_sqlDataReader.GetInt32(9))
                {
                    //
                    // new section
                    //
                    nTSEC_NO = m_sqlDataReader.GetInt32(9);
                    if (!blnFirstSection)
                    {
                        colSections.Add("\"" + strSectionName + "\"" + "," + lngStartRow.ToString() + "," + (lngStartRow + lngRowsinSection - 1).ToString());
                        lngStartRow = lngStartRow + lngRowsinSection;
                        lngRowsinSection = 0;
                    }
                    else
                    {
                        lngStartRow = 1;
                        blnFirstSection = false;
                    }
                    strSectionName = m_sqlDataReader.GetString(8);
                }
            CanonicalDoSameLineFromTableAgain:

                lngRowsinSection++;
                strLine = "    ,new " + strRowFuncName + "(";
                bFieldWritten = false;
                for (int iField = 1; iField < 8; iField++)
                {
                    if (bFieldWritten)
                    {
                        strLine += ",";
                    }
                    if (m_sqlDataReader.IsDBNull(iField))
                    {
                        //
                        // If test case data has no area, this has to be null because
                        // the row function has area.
                        //
                        strLine += "null";
                        bFieldWritten = true;
                    }
                    else
                    {
                        strValue = (string) Util.getValueFromSqlDataReader(m_sqlDataReader, iField);
                        if ((strTableName.ToUpper().IndexOf("TABLE_CANONICALTESTS") != -1) 
                            &&  m_sqlDataReader.GetName(iField).ToUpper().Equals("TTCD_ID"))
                        {
                            iMask = dbCanGetMask(nTTCD);
                            if ((iMask != 0) && (((2 ^ lngRptCnt) & iMask) == 0))
                            {
                                lngRowsinSection--;
                                lngRptCnt = (lngRptCnt + 1) % 3;
                                if (lngRptCnt == 0)
                                {
                                    goto CanonicalNextRecord;
                                }
                                goto CanonicalDoSameLineFromTableAgain;
                            }
                            // Make three lines for each line in the table
                            strValue = strValue + "." + (lngRptCnt + 1).ToString();
                            lngRptCnt = (lngRptCnt + 1) % 3;
                        }
            
                        if (m_sqlDataReader.GetName(iField).ToUpper().Equals("TTCD_RESULT"))
                        {
                            strLine += convertType(COLUMN_TYPE_STRING, strValue);
                        }
                        else
                        {
                            if (Db.IsCharField(m_sqlDataReader.GetDataTypeName(iField)))
                            {
                                strLine += "\"" + strValue + "\"";
                            }
                            else
                            {
                                strLine += strValue;    
                            }
                        }
                        bFieldWritten = true;
                    }
                }
    
                //
                // Other parameters belonging to this test case
                //
                m_sqlCommand2.CommandText = "SELECT tcel_value,dcty_id"
                    + " FROM tcel INNER JOIN tcol ON tcel.tcol=tcol.tcol"
                    + " INNER JOIN dcty ON tcol.dcty=dcty.dcty"
                    + " WHERE ttcd=" + 
                    nTTCD + " ORDER BY tcol_colno";
                m_sqlDataReader2 = m_sqlCommand2.ExecuteReader();
                while (m_sqlDataReader2.Read())
                {
                    strValue = m_sqlDataReader2.GetString(0).Trim();
                    strDCTY_ID = m_sqlDataReader2.GetString(1).Trim();
                    strValue = convertType(strDCTY_ID, strValue);
                    strLine += "," + strValue;
                }
                m_sqlDataReader2.Close();

                strLine += ")";
                
                txtFile.WriteLine (strLine);
    
                if (lngRptCnt != 0)
                {
                    goto CanonicalDoSameLineFromTableAgain;
                }

            CanonicalNextRecord:
                ;
            }
            m_sqlDataReader.Close();

            if (!blnFirstSection)
            {
                colSections.Add("\"" + strSectionName + "\"" + "," + lngStartRow.ToString() + "," 
                    + (lngStartRow + lngRowsinSection - 1).ToString());
            }
   
            txtFile.WriteLine("    );");
            txtFile.WriteLine("  }");
            txtFile.WriteLine("  return g_" + strTableName + ";");
            txtFile.WriteLine("}");

            if (colSections.Count > 0)
            {
                txtFile.WriteLine("");
                txtFile.WriteLine("");
                // Create the Function to Create a Section Row\
                // function header
                txtFile.WriteLine ("function " + strSectionFuncName + "Row(SectionName, StartRow, EndRow)");
                txtFile.WriteLine ("{");
                txtFile.WriteLine ("    this.SectionName = SectionName;");
                txtFile.WriteLine ("    this.StartRow = StartRow;");
                txtFile.WriteLine ("    this.EndRow = EndRow;");
                txtFile.WriteLine ("}");
                txtFile.WriteLine("");
                txtFile.WriteLine("");
        
                // Create the Table for Section Name
                txtFile.WriteLine ("var g_" + strSectionTableName + " = null;");
                txtFile.WriteLine();
                txtFile.WriteLine ("function Create" + strSectionTableName + "()");
                txtFile.WriteLine ("{");
                txtFile.WriteLine ("  if (g_" + strSectionTableName + " == null)");
                txtFile.WriteLine ("  {");
                txtFile.WriteLine ("    g_" + strSectionTableName + " =  new Array (");
                for (nSection = 0;  nSection < colSections.Count; nSection++)
                {   
                    strOut = "    new " + strSectionFuncName + "Row(" + (string) colSections[nSection] + ")";
                    if (nSection < colSections.Count - 1)
                    {
                        strOut += ",";
                    }
                    txtFile.WriteLine(strOut);
                }

                txtFile.WriteLine ("    );");
                txtFile.WriteLine ("  }");
                txtFile.WriteLine ("  return g_" + strSectionTableName + ";");
                txtFile.WriteLine ("}");
            }
    
            txtFile.WriteLine("");
            txtFile.WriteLine("");
            txtFile.WriteLine ("#endif //_" + strTableName.ToUpper() + "_JS_");
            txtFile.Close();
        }

        private int dbCanGetMask(int p_nTTCD)
        {
            int n;
            //
            // Get the mask value which is in column 7
            //
            // tcel_row must always be 1, as this is of type dtty = single
            //
            m_sqlCommand2.CommandText = "SELECT tcel_value FROM tcel"
                + " INNER JOIN tcol ON tcel.tcol=tcol.tcol"
                + " WHERE tcel_row = 1 AND ttcd = " + p_nTTCD + " AND tcol_colno = 7";
            m_sqlDataReader2 = m_sqlCommand2.ExecuteReader();
            m_sqlDataReader2.Read();
            n = Int32.Parse(m_sqlDataReader2.GetString(0));
            m_sqlDataReader2.Close();
            return n;
        }

        /*
         * This function needs to do the equivalent of what is mapped in 
         * gentable.xls::CreateTable():
         * 
         * - values from "HEX" columns are converted to decimal
         * - values from "NUMBER" columns:
         *      PASS -> 1
         *      FAIL -> 0
         * 
         * - values from "STRING" columns:
         *      - OMIT -> ""
         *      - other value -> "other value"
         * 
         * - values from "FLAG" columns:
         *      - OMIT -> "N"
         *      = other value -> "other value"
         */
        private string convertType(string strType, string strEntry)
        {
            strType = strType.ToUpper().Trim();
            strEntry = strEntry.ToUpper().Trim();

            if (strType.Equals(COLUMN_TYPE_NUMBER))
            {
                if (strEntry.Length > 0)
                {
                    if (strEntry.Equals("PASS" ))
                    {
                        strEntry = "1";
                    }
                    else if (strEntry.Equals("FAIL"))
                    {
                        strEntry = "0";
                    }
                }
            }
            else if (strType.Equals(COLUMN_TYPE_STRING))
            {
                // Add quotes to STRING and FLAG types
                if (strEntry.Equals("OMIT"))
                {
                    strEntry = "\"\"";
                }
                else
                {
                    strEntry = "\"" + strEntry + "\"";
                }
            }
            else if (strType.Equals(COLUMN_TYPE_FLAG))
            {
                if (strEntry.Equals("OMIT"))
                {
                    strEntry = "\"N\"";
                }
                else
                {
                    strEntry = "\"" + strEntry + "\"";
                }
            }
            else if (strType.Equals(COLUMN_TYPE_HEX))
            {
                strEntry = MyHex2Dec(strEntry);
            }

            return strEntry;
        }

        private string IntArrayToList(ArrayList p_arInt, string p_strSeparator)
        {
            string strList = "";

            for (int i = 0; i < p_arInt.Count; i++)
            {
                if (i > 0)
                {
                    strList += p_strSeparator;
                }
                strList += (int) p_arInt[i];
            }
            return "(" + strList + ")";
        }

        private string MyHex2Dec(string p_strHex)
        {
            long nDecimal = 0;
            char[] arHex = p_strHex.ToUpper().ToCharArray();
            string sHex;
            char c;

            for (int i = 0; i < arHex.Length; i++)
            {
                nDecimal *= 16;
                c = arHex[i];
                if (c >= '0' && c <= '9')
                {
                    nDecimal += c - '0';
                }
                else if (c >= 'A' && c <= 'F')
                {
                    nDecimal += 10 + c - 'A';
                }
            }
            
            sHex = nDecimal.ToString();

            return sHex;
        }


        private void generateTableCommand(TableData p_td)
        {
            string strRowFuncName ;
            string strTableName ;
            string strSectionFuncName ;
            string strSectionTableName  ;
            string strOutFile ;
            string strLine ;
            int iField ;
            int nTTAB ;
            int nTTCD ;
            int nTSEC_NO ;
            int nTCEL_ROW ;
            int nTableFrom ;
            int nTableTo ;
            string strSectionName ;
            string strValue ;
            string strDCTY_ID ;
            int nCountColumns ;
            int nCountCommands ;
            string strListTTCD = IntArrayToList(p_td.TTCDList, ",");

            strTableName = p_td.TTAB.TTAB_NAME;
            nTTAB = p_td.TTAB.TTAB;
            
            strOutFile = strTableName + ".js";
            string strFileName = m_strDirectory + @"\" + strOutFile;
            FileStream fs = new FileStream(strFileName,
                FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter txtFile = new StreamWriter(fs);
            
            txtFile.WriteLine ("#ifndef _" + strTableName + "_JS_");
            txtFile.WriteLine ("#define _" + strTableName + "_JS_");
            txtFile.WriteLine();
            txtFile.WriteLine ("////////////////////////////////////////");
            txtFile.WriteLine ("//");
            txtFile.WriteLine ("// TABLE: " + strTableName);
            txtFile.WriteLine ("//");
            txtFile.WriteLine ("// generated from database sxstest on " + DateTime.Now.ToLongDateString());
            txtFile.WriteLine ("//");
            txtFile.WriteLine ("////////////////////////////////////////");
            txtFile.WriteLine();
            
            
            //
            // row function header
            //
            strRowFuncName = "Create_" + strTableName + "_Row";
            strLine = "function " + strRowFuncName + "(Area, SubArea, Id, Description, VerboseDescription, Priority, Result, Commands)";
            txtFile.WriteLine (strLine);
            
            //
            // row function text
            //
            txtFile.WriteLine ("{");
            txtFile.WriteLine ("    this.Area = Area;");
            txtFile.WriteLine ("    this.SubArea = SubArea;");
            txtFile.WriteLine ("    this.Id = Id;");
            txtFile.WriteLine ("    this.Description = Description;");
            txtFile.WriteLine ("    this.VerboseDescription = VerboseDescription;");
            txtFile.WriteLine ("    this.Priority = Priority;");
            txtFile.WriteLine ("    this.Result = Result;");
            txtFile.WriteLine ("    this.Commands = Commands;");
            txtFile.WriteLine ("}");

            //
            // test data table
            //
            txtFile.WriteLine();
            txtFile.WriteLine ("var g_" + strTableName + " = null;");
            txtFile.WriteLine();
            
            txtFile.WriteLine ("function Create" + strTableName + "()");
            txtFile.WriteLine ("{");
            txtFile.WriteLine ("  if (g_" + strTableName + " == null)");
            txtFile.WriteLine ("  {");
            txtFile.WriteLine ("    g_" + strTableName + " = new Array(");
            txtFile.WriteLine ("      new " + strRowFuncName + "(null, null, null, " + "\"" + strTableName + "\"" + ")");

            //
            // predefined values in TTCD
            //
            m_sqlCommand.CommandText = "SELECT ttcd, dare_id, dsae_id, ttcd_id,"
                + " ttcd_description, ttcd_verbosedescription, ttcd_priority, ttcd_result"
                + " FROM ttcd"
                + " LEFT OUTER JOIN dare ON ttcd.dare=dare.dare"
                + " LEFT OUTER JOIN dsae ON ttcd.dsae=dsae.dsae"
                + " WHERE ttcd.ttab = " + nTTAB;

            if (p_td.Mode == TableData.GENERATE_MODE.SELECTION && p_td.TTCDList.Count > 0)
            {
                m_sqlCommand.CommandText += " AND ttcd IN " + strListTTCD;
            }
            m_sqlCommand.CommandText += " ORDER BY tsec_no, ttcd_no";

            m_sqlDataReader = m_sqlCommand.ExecuteReader();
            while (m_sqlDataReader.Read())
            {
                //
                // main test data
                //
                nTTCD = m_sqlDataReader.GetInt32(0);
                strLine = "      ,new " + strRowFuncName + "(";
                for (iField = 1; iField < m_sqlDataReader.FieldCount; iField++)
                {
                    if (iField > 1)
                    {
                        strLine += ",";
                    }
                    if (Db.IsCharField(m_sqlDataReader.GetDataTypeName(iField)))
                    {
                        strLine += "\"" + m_sqlDataReader.GetString(iField).Trim() + "\"";
                    }
                    else
                    {
                        strLine += Util.getValueFromSqlDataReader(m_sqlDataReader, iField);
                    }
                }
                txtFile.WriteLine (strLine);

                //
                // all TCEL records belonging to this test data
                //
                m_sqlCommand2.CommandText = "SELECT tcel_row, tcol_colno, tcel_value, dcty_id"
                    + " FROM tcel"
                    + " INNER JOIN tcol ON tcel.tcol=tcol.tcol "
                    + " INNER JOIN dcty ON tcol.dcty=dcty.dcty "
                    + " WHERE ttcd = " + nTTCD + " ORDER BY tcel_row, tcol_colno";
                m_sqlDataReader2 = m_sqlCommand2.ExecuteReader();
                if (m_sqlDataReader2.Read())
                {
                    strLine = "          ,new Array(";
                    txtFile.WriteLine (strLine);
                    strLine = "";
                    
                    nTCEL_ROW = 1;   // if there is any data, the first row must always start with 1
                    nCountColumns = 0;
                    nCountCommands = 0;
                    do
                    {
                        if (m_sqlDataReader2.GetInt16(0) != nTCEL_ROW)
                        {
                            //
                            // new row
                            //
                            nCountCommands++;
                            if (nCountColumns < 2)
                            {
                                strLine = strLine + ", \"\"";
                            }
                            if (nCountCommands > 1)
                            {
                                strLine = "              ,new CreateCommand(" + strLine + ")";
                            }
                            else
                            {
                                strLine = "              new CreateCommand(" + strLine + ")";
                            }
                            txtFile.WriteLine (strLine);
                            strLine = "";
                            nTCEL_ROW = m_sqlDataReader2.GetInt16(0);
                            nCountColumns = 0;
                        }
                        nCountColumns++;
                        strDCTY_ID = m_sqlDataReader2.GetString(3);
                        strValue = convertType(strDCTY_ID, m_sqlDataReader2.GetString(2));
                        if (nCountColumns > 1)
                        {
                            strLine = strLine + ", ";
                        }
                        strLine += strValue;
                    } while (m_sqlDataReader2.Read());

                    //
                    // last row
                    //
                    if (nCountColumns < 2)
                    {
                        strLine += ", \"\"";
                    }
                    if (nCountCommands > 0)
                    {
                        strLine = "              ,new CreateCommand(" + strLine + ")";
                    }
                    else
                    {
                        strLine = "              new CreateCommand(" + strLine + ")";
                    }
                    txtFile.WriteLine (strLine);
                    txtFile.WriteLine ("          )");
                }
                m_sqlDataReader2.Close();
                txtFile.WriteLine ("      )");
            }
            m_sqlDataReader.Close();
            txtFile.WriteLine("    );");
            txtFile.WriteLine("  }");
            txtFile.WriteLine("  return g_" + strTableName + ";");
            txtFile.WriteLine("}");

            //
            // Section row function
            //
            txtFile.WriteLine();
            strSectionFuncName = "Create_" + strTableName + "Section";
            txtFile.WriteLine ("function " + strSectionFuncName + "Row(SectionName,StartRow,EndRow)");
            txtFile.WriteLine ("{");
            txtFile.WriteLine ("    " + "this.SectionName = SectionName;");
            txtFile.WriteLine ("    " + "this.StartRow = StartRow;");
            txtFile.WriteLine ("    " + "this.EndRow = EndRow;");
            txtFile.WriteLine ("}");
            txtFile.WriteLine();

            
            //
            // Section table
            //
            strSectionTableName = strTableName+ "Section";

            txtFile.WriteLine ("var g_" + strSectionTableName + " = null;");
            txtFile.WriteLine();
            
            txtFile.WriteLine ("function Create" + strSectionTableName + "()");
            txtFile.WriteLine ("{");
            txtFile.WriteLine ("  if (g_" + strSectionTableName + " == null)");
            txtFile.WriteLine ("  {");
            txtFile.WriteLine ("    g_" + strSectionTableName + " =  new Array (");
            m_sqlCommand.CommandText = "SELECT tsec_name, ttcd.tsec_no, ttcd_no"
                + " FROM tsec"
                + " INNER JOIN ttcd ON tsec.ttab=ttcd.ttab AND tsec.tsec_no=ttcd.tsec_no"
                + " WHERE tsec.ttab = " + nTTAB;

            if (p_td.Mode == TableData.GENERATE_MODE.SELECTION && p_td.TTCDList.Count > 0)
            {
                m_sqlCommand.CommandText += " AND ttcd IN " + strListTTCD;
            }
            m_sqlCommand.CommandText += " ORDER BY ttcd.tsec_no, ttcd.ttcd_no";

            m_sqlDataReader = m_sqlCommand.ExecuteReader();
            if (m_sqlDataReader.Read())
            {
                nTableFrom = 1;
                nTableTo = 1;
                strSectionName = m_sqlDataReader.GetString(0);
                nTSEC_NO = m_sqlDataReader.GetInt32(1);
                while (m_sqlDataReader.Read())
                {
                    if (nTSEC_NO != m_sqlDataReader.GetInt32(1))
                    {
                        //
                        // section changed
                        //
                        strLine = "      new " + strSectionFuncName + "Row(\"" + strSectionName 
                            + "\"," + nTableFrom + "," + nTableTo + "),";
                        txtFile.WriteLine (strLine);
                        nTableFrom = nTableTo + 1;
                        nTableTo = nTableFrom;
                        strSectionName = m_sqlDataReader.GetString(0);
                        nTSEC_NO = m_sqlDataReader.GetInt32(1);
                    }
                    else
                    {
                        nTableTo = nTableTo + 1;
                    }
                }
                strLine = "      new " + strSectionFuncName + "Row(\"" + strSectionName 
                    + "\"," + nTableFrom + "," + nTableTo + ")";
                txtFile.WriteLine (strLine);
                
                txtFile.WriteLine ("    );");
                txtFile.WriteLine ("  }");
                txtFile.WriteLine ("  return g_" + strSectionTableName + ";");
                txtFile.WriteLine ("}");
                txtFile.WriteLine();
                txtFile.WriteLine ("#endif //_" + strTableName  + "_JS_");
                
                txtFile.Close();
            }
            m_sqlDataReader.Close();
        }

        private string flatListFromRangeList(string p_strRangeList)
        {
            char[] commaSeparator = { ',' };
            char[] dashSeparator = { '-' };
            string strList = "";

            string[] strRanges = p_strRangeList.Split(commaSeparator);

            for (int i = 0; i < strRanges.Length; i++)
            {
                string[] strRange = strRanges[i].Split(dashSeparator);
            
                if (strRange.Length > 2)
                {
                    throw new Exception("Error: bad format for range, must be 'n' or 'n-m'");
                }
                if (strList.Length > 0)
                {
                    strList += ",";
                }
                if (strRange.Length == 1)
                {
                    strList += strRange[0];
                }
                else
                {
                    int nFrom = Int32.Parse(strRange[0]);
                    int nTo = Int32.Parse(strRange[1]);
                    for (int j = nFrom; j <= nTo; j++)
                    {
                        strList += "," + j;
                    }
                }
            }
            return strList;
        }

        private void progressDone()
        {
            if (m_progress != null)
            {
                m_progress.Value = m_progress.Minimum;
            }
        }

        private void progressInitialize()
        {
            if (m_progress != null)
            {
                m_progress.Minimum = 1;
                m_progress.Maximum = m_arTFUN.Count + m_arTTAB.Count + m_arTTFU.Count + m_arTableData.Count;
                if (m_progress.Maximum < 1)
                {
                    m_progress.Maximum = 1;
                }
                m_progress.Value = 1;
                m_progress.Step = 1;
            }
        }

        private void progressUpdate()
        {
            progressUpdate(m_progress.Value.ToString());
        }

        private void progressUpdate(string p_strText)
        {
            if (m_progress != null)
            {
                m_progress.PerformStep();
                m_progress.Text = p_strText;
            }
        }
    }
}

