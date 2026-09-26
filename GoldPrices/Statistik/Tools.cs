using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;

namespace CMaurer.Common
{
    public static class Tools
    {
        public const string LogKeyRun = "exec";
        public const string LogKeyFileCopy = "filecopy";
        public const string LogKeyFileMove = "filemove";
        public const string LogKeyFileGenerate = "generate";
        public const string LogKeyDoc = "doc";
        public const string LogKeyMks = "mks";
        public const string LogKeyGit = "git";
        public const string LogKeyInfo = "info";
        public const string LogKeyError = "error";

        public static string _sandbox = "";
        public static List<FileInfo> _sandBoxFileNames = new List<FileInfo>();

        /// <summary>
        /// key: the path to a sandbox folder
        /// value: a list of all files in that folder
        /// All files are cached in this data, so that we must not read the entire folder each time.
        /// </summary>
        public static Dictionary<string, List<string>> _getAllFileNamesData = new Dictionary<string, List<string>>();

        /*        
                private Tools() 
                {
                    // FXCop: StaticHolderTypesShouldNotHaveConstructors
                }
         */

        public static bool DeleteFolder(string folder)
        {
            bool success = false;

            try
            {
                Directory.Delete(folder, true);
                success = true;
            }
            catch
            {
            }

            return success;
        }

        public static bool CreateFolder(string folder)
        {
            bool success = false;

            try
            {
                Directory.CreateDirectory(folder);
                success = true;
            }
            catch
            {
            }

            return success;
        }

        public static bool StartProcess(string fileName)
        {
            bool success = false;

            try
            {
                Process process = Process.Start(fileName);
                if (process.ExitCode == 0)
                {
                    success = true;
                }
            }
            catch (Exception)
            {
            }

            return success;
        }

        public static bool StartProcess(string fileName, string arguments)
        {
            bool success = false;

            try
            {
                Process process = Process.Start(fileName, arguments);
                if (process.ExitCode == 0)
                {
                    success = true;
                }
            }
            catch (Exception)
            {
            }

            return success;
        }

        public static string CutString(string text, int length)
        {
            return CutString(text, length, false);
        }

        /// <summary>
        /// Replace some text in the middle by three dots to reduce the length. Not tested.
        /// "D:\\casdev\\sbxs\\ffm-mks3\\projectdocs\\BMW\\BMW Common\\11.Tools\\CMaurer\\EepromChecker\\bin\\Debug\\FiatKitFileTool.cfg"
        /// ->
        /// "D:\\casdev\\sbxs\\ffm-mks3\\projectdocs\\BMW\\BMW Com...omChecker\\bin\\Debug\\FiatKitFileTool.cfg"
        /// On any error the original text is returned.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ReduceLongFileName(string text, int length)
        {
            string result = text;

            try
            {
                if (text.Length > length)
                {
                    int diff = text.Length - length;
                    int from = (text.Length / 2) - (diff / 2);
                    int to = (text.Length / 2) + (diff / 2);

                    text = text.Substring(0, from) + "..." + text.Substring(to);

                    result = text;
                }
            }
            catch
            {
                result = text;
            }

            return result;
        }


        public static string CutString(string text, int length, bool appendDots)
        {
            string s = text;

            if (s.Length > length)
            {
                s = s.Substring(0, length);
                if (appendDots)
                {
                    s += "...";
                }
            }

            return s;
        }

        /// <summary>
        /// Liefert DBNull.Null oder einen DateTime Wert.
        /// <code>
        /// dataRow["DatumVon"] = Tools.InputTextDate2NullableDatabaseDateTime(this.txtDatumVon.Text);
        /// </code>
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static object InputTextDate2NullableDatabaseDateTime(string date)
        {
            object dt = DBNull.Value;

            if (date != null && date.Length > 0)
            {
                dt = InputTextDate2DateTime(date);
            }
            return dt;
        }

        public static DateTime InputTextDateTime2DateTime(string date, string time)
        {
            int day = Int32.Parse(date.Substring(0, 2), CultureInfo.InvariantCulture);
            int month = Int32.Parse(date.Substring(3, 2), CultureInfo.InvariantCulture);
            int year = Int32.Parse(date.Substring(6, 4), CultureInfo.InvariantCulture);

            int hour = Int32.Parse(time.Substring(0, 2), CultureInfo.InvariantCulture);
            int minute = Int32.Parse(time.Substring(3, 2), CultureInfo.InvariantCulture);

            return new DateTime(year, month, day, hour, minute, 0);
        }

        /// <summary>
        /// Macht aus einem Text im Format dd.mm.yyyy ein DateTime mit Uhrzeit 00:00:00
        /// Wirft eine Exception, wenn strDate kein gültiges Format hat.
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime InputTextDate2DateTime(string date)
        {
            int day = Int32.Parse(date.Substring(0, 2), CultureInfo.InvariantCulture);
            int month = Int32.Parse(date.Substring(3, 2), CultureInfo.InvariantCulture);
            int year = Int32.Parse(date.Substring(6, 4), CultureInfo.InvariantCulture);

            return new DateTime(year, month, day, 0, 0, 0);
        }

        /// <summary>
        /// Macht aus einem Text im Format dd.mm.yyyy ein DateTime mit Uhrzeit 23:59:59
        /// Wirft eine Exception, wenn strDate kein gültiges Format hat.
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime InputTextDate2DateTimeEnd(string date)
        {
            int day = Int32.Parse(date.Substring(0, 2), CultureInfo.InvariantCulture);
            int month = Int32.Parse(date.Substring(3, 2), CultureInfo.InvariantCulture);
            int year = Int32.Parse(date.Substring(6, 4), CultureInfo.InvariantCulture);

            return new DateTime(year, month, day, 23, 59, 59);
        }

        public static DateTime? InputTextDate2NullableDateTime(string date)
        {
            DateTime? dt = null;

            if (date != null && date.Length > 0)
            {
                int nDay = Int32.Parse(date.Substring(0, 2), CultureInfo.InvariantCulture);
                int nMonth = Int32.Parse(date.Substring(3, 2), CultureInfo.InvariantCulture);
                int nYear = Int32.Parse(date.Substring(6, 4), CultureInfo.InvariantCulture);
                dt = new DateTime(nYear, nMonth, nDay, 0, 0, 0);
            }
            return dt;
        }

        /// <summary>
        /// Liefert DBNull.Null oder einen DateTime Wert ohne Datum aber mit Zeit.
        /// <code>
        /// dataRow["DatumVon"] = Tools.InputTextTime2NullableDatabaseDateTime(this.txtDatumVon.Text);
        /// </code>
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static object InputTextTime2NullableDatabaseDateTime(string date)
        {
            object dt = DBNull.Value;

            if (date != null && date.Length > 0)
            {
                dt = InputTextTime2DateTime(date);
            }
            return dt;
        }

        /// <summary>
        /// Convert a string to a DateTime, setting the Time part only.
        /// </summary>
        /// <param name="strTime">"hh:mm"</param>
        /// <returns>a DateTime with date 1.1.1900 and time as supplied in strTime</returns>
        public static DateTime InputTextTime2DateTime(string time)
        {
            int hour = Int32.Parse(time.Substring(0, 2), CultureInfo.InvariantCulture);
            int minute = Int32.Parse(time.Substring(3, 2), CultureInfo.InvariantCulture);

            DateTime dt = new DateTime(1900, 1, 1, hour, minute, 0);
            return dt;
        }

        /// <summary>
        /// Control.Text = Tools.NullableDateTime2DateTimeString(oDataRow["Datum"])
        /// Liefert ein Datum im Format "tt.mm.jjjj hh:mm" oder "" wenn der Wert null ist.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DBNullableDateTime2DateTimeString(object value)
        {
            string s = "";

            if (value != DBNull.Value)
            {
                DateTime dt = (DateTime)value;

                s = DateTime2DateTimeString(dt);
            }

            return s;
        }
        public static string DateTime2DateTimeString(DateTime dt)
        {
            int nDay = dt.Day;
            int nMonth = dt.Month;
            int nYear = dt.Year;

            int hour = dt.Hour;
            int minute = dt.Minute;

            string s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000} {3:00}:{4:00}",
                nDay, nMonth, nYear,
                hour, minute);

            return s;
        }

        public static string DateTime2DateString(DateTime dt)
        {
            int nDay = dt.Day;
            int nMonth = dt.Month;
            int nYear = dt.Year;

            string s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000}",
                nDay, nMonth, nYear);

            return s;
        }

        public static string DateTime2DateStringYY(DateTime dt)
        {
            int nDay = dt.Day;
            int nMonth = dt.Month;
            int nYear = dt.Year % 100;

            string s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:00}",
                nDay, nMonth, nYear);

            return s;
        }

        public static string DateTime2DateStringMMYY(DateTime dt)
        {
            int month = dt.Month;
            int year = dt.Year % 100;

            string s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}", month, year);

            return s;
        }

        public static string DateTime2ShortDateString(DateTime dt)
        {
            int nDay = dt.Day;
            int nMonth = dt.Month;
            int nYear = dt.Year;

            string s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:00}",
                nDay, nMonth, nYear % 1000);

            return s;
        }

        /// <summary>
        /// Control.Text = Tools.NullableDateTime2DateTimeString(oDataRow["Datum"])
        /// Liefert ein Datum im Format "tt.mm.jjjj hh:mm" oder "" wenn der Wert null ist.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DBNullableDateTime2SortableDateTimeString(object value)
        {
            string s = "";

            if (value != DBNull.Value)
            {
                DateTime dt = (DateTime)value;
                int nDay = dt.Day;
                int nMonth = dt.Month;
                int nYear = dt.Year;

                int hour = dt.Hour;
                int minute = dt.Minute;

                s = string.Format(CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00} {3:00}:{4:00}",
                    nYear, nMonth, nDay,
                    hour, minute);
            }

            return s;
        }

        public static string DBNullableDateTime2DateString(object value)
        {
            string s = "";

            if (value != null && value != DBNull.Value)
            {
                DateTime dt = (DateTime)value;
                int nDay = dt.Day;
                int nMonth = dt.Month;
                int nYear = dt.Year;

                s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000}", nDay, nMonth, nYear);
            }

            return s;
        }

        public static string DBNullableDateTime2SortableDateString(object value)
        {
            string s = "";

            if (value != null && value != DBNull.Value)
            {
                DateTime dt = (DateTime)value;
                int nDay = dt.Day;
                int nMonth = dt.Month;
                int nYear = dt.Year;

                s = string.Format(CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00}", nYear, nMonth, nDay);
            }

            return s;
        }
        public static string DBDateTime2DateString(object value)
        {
            DateTime dt = (DateTime)value;
            int day = dt.Day;
            int month = dt.Month;
            int year = dt.Year;

            string s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000}", day, month, year);

            return s;
        }

        public static string DBNullableInt2String(object value)
        {
            string s = "";

            if (value != DBNull.Value)
            {
                s = value.ToString();
            }

            return s;
        }

        public static string DBNullableString2String(object value)
        {
            string s = "";

            if (value != DBNull.Value)
            {
                s = (string)value;
            }

            return s;
        }

        public static string NullableDateTime2DateString(DateTime? dt)
        {
            string s = "";

            if (dt.HasValue)
            {
                int nDay = dt.Value.Day;
                int nMonth = dt.Value.Month;
                int nYear = dt.Value.Year;

                s = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000}", nDay, nMonth, nYear);
            }

            return s;
        }

        /// <summary>
        /// Control.Text = Tools.NullableDateTime2TimeString(oDataRow["Zeit"])
        /// Liefert ein Datum im Format "hh:mm" oder "" wenn der Wert null ist.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DBNullableDateTime2TimeString(object value)
        {
            string s = "";

            if (value != DBNull.Value)
            {
                DateTime dt = (DateTime)value;
                int nHour = dt.Hour;
                int nMinute = dt.Minute;

                s = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", nHour, nMinute);
            }

            return s;
        }

        /// <summary>
        /// 01234567890
        /// 14.05.1965
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static bool DateIsValidGermanDate(string date)
        {
            bool success = false;

            if (date.Length == 10
                    && date.Substring(2, 1) == "."
                    && date.Substring(5, 1) == "."
                )
            {
                try
                {
                    int day = Int32.Parse(date.Substring(0, 2), CultureInfo.InvariantCulture);
                    int month = Int32.Parse(date.Substring(3, 2), CultureInfo.InvariantCulture);
                    int year = Int32.Parse(date.Substring(6, 4), CultureInfo.InvariantCulture);

                    DateTime dateTime = new DateTime(year, month, day, 0, 0, 0);

                    if (DatabaseDateTimeMinValue <= dateTime && dateTime <= DatabaseDateTimeMaxValue)
                    {
                        success = true;
                    }
                }
                catch
                {
                }
            }
            return success;
        }

        public static bool TimeIsValidGermanTime(string time)
        {
            bool success = false;

            if (time.Length == 5)
            {
                try
                {
                    int hour = Int32.Parse(time.Substring(0, 2), CultureInfo.InvariantCulture);
                    int minute = Int32.Parse(time.Substring(3, 2), CultureInfo.InvariantCulture);
                    DateTime dt = new DateTime(1900, 1, 1, hour, minute, 0);
                    success = true;
                }
                catch
                {
                }
            }
            return success;
        }

        public static string MultipleLineText2SingleLineText(string text)
        {
            StringBuilder sb = new StringBuilder(text);

            sb.Replace("\n", " ");
            sb.Replace("\t", " ");
            sb.Replace("\r", " ");
            sb.Replace("  ", " ");
            sb.Replace("  ", " ");
            sb.Replace("  ", " ");

            return sb.ToString();
        }

        /// <summary>
        /// Get the application startup folder. If started from within Visual Studio,
        /// go up from the bin\Debug or bin\Release path.
        /// </summary>
        /// <param name="startupPath"></param>
        /// <param name="subDir"></param>
        /// <returns></returns>
        public static string GetAppSubDir(string startupPath, string subDir)
        {
            string path = startupPath;
            int index = path.LastIndexOf(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "release");

            if (index == -1)
            {
                index = path.LastIndexOf(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "debug");
            }
            if (index != -1)
            {
                path = path.Substring(0, index);
            }
            if (subDir.Length > 0)
            {
                path += Path.DirectorySeparatorChar + subDir;
            }
            return path;
        }

        /// <summary>
        /// Das größte Datum alles kleinsten Datümer von allen Datenbanken.
        /// MySql:      '1000-01-01 00:00:00'
        /// SqlServer:  
        /// Access:
        /// </summary>
        static public DateTime DatabaseDateTimeMinValue
        {
            get { return new DateTime(1753, 1, 1, 0, 0, 0); }
        }
        /// <summary>
        /// Das kleinste aller Maxima von allen Datenbanken.
        /// MySql:      '9999-12-31 23:59:59'
        /// SqlServer:  
        /// Access:
        /// </summary>
        static public DateTime DatabaseDateTimeMaxValue
        {
            get { return new DateTime(9999, 12, 31, 23, 59, 59); }
        }

        /// <summary>
        ///  Liefert true wenn min &lt;= n &lt;= max
        /// und false sonst
        /// </summary>
        /// <param name="text">Eine Zahl als Text</param>
        /// <param name="min">Das Minimum inklusive</param>
        /// <param name="max">Das Maximum inklusive</param>
        /// <returns></returns>
        public static bool IsIntBetween(string text, int min, int max)
        {
            bool success = false;

            int n;
            if (Int32.TryParse(text, out n))
            {
                if (min <= n && n <= max)
                {
                    success = true;
                }
            }

            return success;
        }

        public static void GetOperationSystem(out System.PlatformID platform, out int major, out int minor)
        {
            System.OperatingSystem osInfo = System.Environment.OSVersion;

            platform = osInfo.Platform;
            major = osInfo.Version.Major;
            minor = osInfo.Version.Minor;
        }

        /// <summary>
        /// Windows XP Home Edition, Windows XP Professional x64 Edition, Windows Server 2003 Platform 
        /// Siehe http://support.microsoft.com/kb/304283/de
        ///                     Major   Minor
        /// Visa Home Basic     6       0
        /// </summary>
        /// <returns>true, wenn das OS den MArquee style supported</returns>
        public static bool ProgressBarMarqueeSupported()
        {
            // Marquee erst ab XP
            //              95  98  Me      NT4.0   2000    XP  Vista
            // Plattform-ID  1  1   1       2       2       2   2
            // Hauptversion  4  4   4       4       5       5   6
            // Nebenversion  0  10  90      0       0       1   0

            bool success = false;

            System.OperatingSystem osInfo = System.Environment.OSVersion;

            if (osInfo.Platform == System.PlatformID.Win32NT)
            {
                if (
                    (osInfo.Version.Major >= 5 && osInfo.Version.Minor >= 1)
                    ||
                    (osInfo.Version.Major >= 6))
                {
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// Return the number of lines in a file using a given encoding.
        /// </summary>
        /// <param name="encoding">Encoding of the file</param>
        /// <param name="fileName">Full path and file name</param>
        /// <returns></returns>
        public static int CountLines(Encoding encoding, string fileName)
        {
            int count = 0;
            StreamReader reader = null;
            string line;

            try
            {
                // Hier ist das Encoding egal, weil nur die Zeilen gezählt werden sollen
                reader = new StreamReader(fileName, encoding);
                do
                {
                    line = reader.ReadLine();
                    count++;
                } while (line != null);
            }
            catch
            {
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return count;
        }

        public static bool DeleteFile(string fileName)
        {
            bool success = true;

            if (!System.IO.File.Exists(fileName))
            {
                goto Exit;
            }

            try
            {
                File.Delete(fileName);
            }
            catch
            {
                try
                {
                    // Löschen hat nicht geklappt, evtl. ist die Datei schreibgeschützt, 
                    // also den Schreibschutz entfernen.
                    System.IO.File.SetAttributes(fileName, FileAttributes.Normal);
                    System.IO.File.Delete(fileName);
                }
                catch
                {
                    success = false;
                }

            }
        Exit:
            return success;
        }

        /// <summary>
        /// Move a file.
        /// </summary>
        /// <param name="src">Source file</param>
        /// <param name="dst">Destination file</param>
        /// <returns>true if src exists and could be moved successfully, false in any other case</returns>
        public static bool MoveFile(string src, string dst, ref string errorMsg)
        {
            bool success = false;

            if (!System.IO.File.Exists(src))
            {
                goto Exit;
            }

            try
            {
                DeleteFile(dst);
                File.Move(src, dst);
                success = true;
            }
            catch (Exception exception)
            {
                success = false;
                errorMsg = exception.Message;
            }

        Exit:
            return success;
        }

        static public string GetTempDirectoryName()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(path);

            return path;
        }

        public static bool CopyFile(IProgressCallBack view, string src, string dst, bool displayProgress)
        {
            bool success = false;

            try
            {
                File.Copy(src, dst, true);
                success = true;
            }
            catch 
            {
                try
                {
                    //
                    // Kopieren hat nicht geklappt, evtl. ist die Datei schreibgeschützt, 
                    // also den Schreibschutz entfernen.
                    //
                    System.IO.File.SetAttributes(dst, FileAttributes.Normal);
                    System.IO.File.Copy(src, dst, true);
                    success = true;
                }
                catch
                {

                }
            }

            if (success)
            {
                if (displayProgress)
                {
                    Console.WriteLine("Copied file: " + src + " --> " + dst);
                }

                if (view != null)
                {
                    view.Progress(LogKeyFileCopy, src, dst);
                }
            }
            else
            {
                if (displayProgress)
                {
                    Console.WriteLine("Error copying file: " + src + " --> " + dst);
                }

                if (view != null)
                {
                    view.Progress(LogKeyError, src, dst);
                }
            }

            return success;
        }

        public static bool CopyFile(string src, string dst, bool displayProgress)
        {
            bool success = false;

            if (displayProgress)
            {
                Console.WriteLine("Copying file: " + src + " --> " + dst);
            }

            try
            {
                System.IO.File.Copy(src, dst, true);
                success = true;
            }
            catch
            {
                try
                {
                    //
                    // Kopieren hat nicht geklappt, evtl. ist die Datei schreibgeschützt, 
                    // also den Schreibschutz entfernen.
                    //
                    System.IO.File.SetAttributes(dst, FileAttributes.Normal);
                    System.IO.File.Copy(src, dst, true);
                    success = true;
                }
                catch
                {

                }
            }

            return success;
        }

        /// <summary>
        /// Write a string as Java modified UTF8 string
        /// Write the length as two bytes in big endian first, and then the string bytes as UTF8
        /// </summary>
        /// <param name="line">The text to write, the length in UTF8 bytes must fit into 2 bytes</param>
        static public void WriteJavaModifiedUtf8(BinaryWriter writer, string line)
        {
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(line);

            if (utf8Bytes.Length >= Int16.MaxValue)
            {
                throw new Exception("Cannot write Java-modified-UTF8 string of length >= " + Int16.MaxValue);
            }

            //
            // write length in big endian
            // int length = 0x12345678
            //
            // big endian:      0x12 0x34 0x56 0x78
            // little endian    0x78 0x56 0x34 0x12
            //
            //  BitConverter.GetBytes(length) will return 4 bytes which are ordered
            //  depending on the Endianess of the system
            //
            //  arLength    little endian   big endian
            //  0           0x78            0x12
            //  1           0x56            0x34
            //  2           0x34            0x56
            //  3           0x12            0x78
            //
            byte[] arLength = System.BitConverter.GetBytes(utf8Bytes.Length);

            //
            // As we can only write two bytes, our max string length must be 16 bit
            // so int length from the example above is 0x00005678
            // and we need to write byte 0x56 and then byte 0x78
            //
            if (BitConverter.IsLittleEndian)
            {
                //
                //               0  1  2  3 
                // 0x00005678 -> 78 56 00 00
                //
                writer.Write(arLength[1]);
                writer.Write(arLength[0]);
            }
            else
            {
                //
                //               0  1  2  3
                // 0x00005678 -> 00 00 56 78
                //
                writer.Write(arLength[2]);
                writer.Write(arLength[3]);
            }

            writer.Write(utf8Bytes);
        }

        static public string ReadJavaModifiedUtf8(BinaryReader reader)
        {
            string line = null;

            try
            {
                byte high = reader.ReadByte();
                byte low = reader.ReadByte();
                int length = high * 256 + low;

                byte[] utf8Bytes = reader.ReadBytes(length);
                line = System.Text.Encoding.UTF8.GetString(utf8Bytes);
            }
            catch (Exception exc)
            {
                Trace.WriteLine(exc.ToString());
            }

            return line;
        }

        static public string GetFullFileName(string folder, string pattern, int index)
        {
            return GetFileName(folder, pattern, true, index);
        }

        static public string GetFullFileName(string folder, string pattern)
        {
            return GetFileName(folder, pattern, true);
        }

        static public string GetFileName(string folder, string pattern)
        {
            return GetFileName(folder, pattern, false);
        }

        static public string GetFileName(string folder, string pattern, int index)
        {
            return GetFileName(folder, pattern, false, index);
        }

        static public string GetFileName(string folder, string pattern, bool fullName)
        {
            return GetFileName(folder, pattern, fullName, 0);
        }

        static private string GetFileName(string folder, string pattern, bool fullName, int index)
        {
            string fileName = null;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder);

                FileInfo[] files = dirInfo.GetFiles(pattern);

                if (index < files.Length)
                {
                    if (fullName)
                    {
                        fileName = files[index].FullName;
                    }
                    else
                    {
                        fileName = files[index].Name;
                    }
                }
            }
            catch
            {
                Console.Error.WriteLine("Missing file in folder '" + folder + "', pattern = '" + pattern + "'");
            }

            return fileName;
        }

        static public string GetElfFileName(string folder, string pattern)
        {
            return GetElfFileName(folder, pattern, false);
        }

        /// <summary>
        /// Get the normal e.elf file from a folder. Do NOT return any of the other generated .elf files
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        static public string GetElfFileName(string folder, string pattern, bool fileNameOnly)
        {
            string fileName = null;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder);

                FileInfo[] files = dirInfo.GetFiles(pattern);

                for (int i = 0; i < files.Length; i++)
                {
                    //
                    // AK3RAJ00015.elf
                    // AK3RAJ00015_cps.elf
                    // AK3RAJ00015_main.elf
                    // AK3RAJ00015_noparam.elf
                    //
                    string text = files[i].Name;
                    if (text.Length == 15)
                    {
                        if (fileNameOnly)
                        {
                            fileName = files[i].Name;
                        }
                        else
                        {
                            fileName = files[i].FullName;
                        }
                        break;
                    }
                }
            }
            catch
            {
                Console.Error.WriteLine("Missing file in folder '" + folder + "', pattern = '" + pattern + "'");
            }

            return fileName;
        }

        static public string[] GetAllFileNames(string folder, string pattern)
        {
            string[] fileNames = null;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder);

                FileInfo[] files = dirInfo.GetFiles(pattern);

                fileNames = new string[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    fileNames[i] = files[i].Name;
                }
            }
            catch
            {
                Console.Error.WriteLine("Missing file in folder '" + folder + "', pattern = '" + pattern + "'");
            }

            return fileNames;
        }

        static public List<string> GetAllFileNamesCached(string folder, bool fullName)
        {
            List<string> fileNames = null;

            try
            {
                if (!_getAllFileNamesData.TryGetValue(folder, out fileNames))
                {
                    //
                    // The files for "folder" are requested for the first time
                    //
                    fileNames = new List<string>();

                    DirectoryInfo dirInfo = new DirectoryInfo(folder);

                    FileInfo[] files = dirInfo.GetFiles();

                    for (int i = 0; i < files.Length; i++)
                    {
                        if (fullName)
                        {
                            fileNames.Add(files[i].FullName);
                        }
                        else
                        {
                            fileNames.Add(files[i].Name);
                        }
                    }
                }
            }
            catch
            {
                Console.Error.WriteLine("Missing files in folder '" + folder + "'");
            }

            return fileNames;
        }

        static public string[] GetAllFileNames(string folder, bool fullName)
        {
            string[] fileNames = null;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder);

                FileInfo[] files = dirInfo.GetFiles();

                fileNames = new string[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    if (fullName)
                    {
                        fileNames[i] = files[i].FullName;
                    }
                    else
                    {
                        fileNames[i] = files[i].Name;
                    }
                }
            }
            catch
            {
                Console.Error.WriteLine("Missing files in folder '" + folder + "'");
            }

            return fileNames;
        }

        static public void GetAllFileNames(string folder, string pattern, Boolean recursive, List<FileInfo> fileNames)
        {
            Tools.GetAllFileNames(null, folder, pattern, recursive, fileNames);
        }

        /// <summary>
        /// find all files in a sandbox 'folder' which match the filename 'pattern'
        /// </summary>
        /// <param name="view"></param>
        /// <param name="folder"></param>
        /// <param name="pattern"></param>
        /// <param name="recursive"></param>
        /// <param name="fileNames"></param>
        static public void GetAllFileNamesInSandbox(IProgressCallBack view, string sandbox, string fileName, List<FileInfo> fileNames)
        {
            if (sandbox != _sandbox)
            {
                _sandbox = sandbox;
                _sandBoxFileNames.Clear();
                GetAllFileNames(view, _sandbox, _sandBoxFileNames);
            }

            fileNames.Clear();

            try
            {
                foreach (FileInfo fileInfo in _sandBoxFileNames)
                {
                    if (fileInfo.Name.ToLower() == fileName.ToLower())
                    {
                        fileNames.Add(fileInfo);
                    }
                }
            }
            catch (Exception exception)
            {
                if (view != null)
                {
                    view.ReportError(exception.ToString());
                }
            }

        }

        static public void GetAllFileNames(IProgressCallBack view, string folder, string pattern, Boolean recursive, List<FileInfo> fileNames)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            FileInfo[] fileInfos = null;

            try
            {
                fileInfos = dirInfo.GetFiles(pattern);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (view != null)
                    {
                        view.DoEvents();
                    }

                    fileNames.Add(fileInfo);
                }

                if (recursive)
                {
                    DirectoryInfo[] subfolders = dirInfo.GetDirectories();

                    foreach (DirectoryInfo directoryInfo in subfolders)
                    {
                        GetAllFileNames(view, directoryInfo.FullName, pattern, recursive, fileNames);
                    }
                }
            }
            catch (Exception exception)
            {
                if (view != null)
                {
                    view.ReportError(exception.ToString());
                }
            }

        }

        static public void GetAllFileNames(IProgressCallBack view, string folder, List<FileInfo> fileNames)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            FileInfo[] fileInfos = null;

            try
            {
                fileInfos = dirInfo.GetFiles();

                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (view != null)
                    {
                        view.DoEvents();
                    }

                    fileNames.Add(fileInfo);
                }

                DirectoryInfo[] subfolders = dirInfo.GetDirectories();

                foreach (DirectoryInfo directoryInfo in subfolders)
                {
                    GetAllFileNames(view, directoryInfo.FullName, fileNames);
                }
            }
            catch (Exception exception)
            {
                if (view != null)
                {
                    view.ReportError(exception.ToString());
                }
            }
        }

        static public void GetAllFolders(string folder, string pattern, Boolean recursive, List<string> folderList)
        {
            string[] folders = Directory.GetDirectories(folder);

            foreach (string subfolder in folders)
            {
                folderList.Add(subfolder);
            }

            if (recursive)
            {
                foreach (string subfolder in folders)
                {
                    string[] arSubfolder = subfolder.Split('\\');
                    if (arSubfolder.Length > 5)
                    {
                        break;
                    }
                    //DirectoryInfo dirInfo = new DirectoryInfo(subfolder);
                    //string newFolder = folder + "\\" + subfolder;
                    GetAllFolders(subfolder, pattern, recursive, folderList);
                }
            }
        }

        /// <summary>
        /// Create a timestamp that can be used in a file name.
        /// </summary>
        /// <returns>the current timestamp</returns>
        static public string GetTimeStamp()
        {
            DateTime now = DateTime.Now;

            string ts = string.Format("{0}{1}{2}-{3}{4}{5}.{6}",
                now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);

            return ts;
        }

        static public string GetTimeStampInUserReadableFormat()
        {
            DateTime now = DateTime.Now;

            string ts = string.Format("{0:0000}{1:00}{2:00}-{3:00}:{4:00}:{5:00}.{6:000}",
                now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);

            return ts;
        }

        public static bool TestExecuteProcessAndWait(string cmd, string options, int processTimeoutSeconds, string path, int expectedReturnCode)
        {
            bool success = false;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = cmd;
                psi.Arguments = options;
                psi.UseShellExecute = false;

                //
                // Add paths to Windows environment variable PATH if required
                //
                if (!string.IsNullOrEmpty(path))
                {
                    StringDictionary envVars = psi.EnvironmentVariables;

                    string envPath = envVars["PATH"];
                    envPath = envPath + ";" + path;
                    envVars["PATH"] = envPath;
                }

                Process process = Process.Start(psi);

                DateTime timeNow = DateTime.Now;
                DateTime timeStop = timeNow.AddSeconds(processTimeoutSeconds);

                //
                // Jetzt eine Weile warten und dabei nachsehen, ob x beendet wurde
                //
                while (timeNow < timeStop)
                {
                    success = process.HasExited;
                    if (success)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeNow = DateTime.Now;
                }

                if (success)
                {
                    int exitCode = process.ExitCode;

                    if ((exitCode != 0) && (exitCode != expectedReturnCode))
                    {
                        success = false;
                    }
                }
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public static bool ExecuteProcessAndWait(IProgressCallBack view, bool simulate, string cmd, string options, string strProgress, int processTimeoutSeconds, StringBuilder sbStdErrResult)
        {
            return ExecuteProcessAndWait(view, simulate, cmd, options, strProgress, processTimeoutSeconds, "", sbStdErrResult);
        }

        public static bool ExecuteProcessAndWait(IProgressCallBack view, bool simulate, string cmd, string options, string strProgress, int processTimeoutSeconds, string path, StringBuilder sbStdErrResult)
        {
            List<string> paths = null;

            if (!string.IsNullOrEmpty(path))
            {
                paths = new List<string>();
                paths.Add(path);
            }

            return ExecuteProcessAndWait(view, simulate, cmd, options, strProgress, processTimeoutSeconds, paths, sbStdErrResult);
        }

        public static void Progress(IProgressCallBack progressCallback, StringBuilder sbStdErrResult)
        {
            string text = sbStdErrResult.ToString();
            string[] arText = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (string line in arText)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    progressCallback.Progress(Tools.LogKeyError, line);
                }
            }
        }

        public static bool ExecuteProcessAndWaitToFile(IProgressCallBack view, bool simulate, string cmd, string options, string strProgress, int processTimeoutSeconds, List<string> pathVariables, string fileName)
        {
            bool success = true;

            string psiFileName = @"c:\windows\system32\cmd.exe";        
            string arguments = "/C " + cmd + " " + options;
            if (!string.IsNullOrEmpty(fileName))
            {
                arguments = arguments + " > " + fileName;
            }

            if (view != null)
            {
                view.Progress(LogKeyRun, psiFileName + " " + arguments);
            }

            if (!simulate)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.UseShellExecute = false;
                psi.FileName = psiFileName;
                psi.Arguments = arguments;

                //
                // Add paths to Windows environment variable PATH if required
                //
                if (pathVariables != null)
                {
                    StringDictionary envVars = psi.EnvironmentVariables;

                    string envPath = envVars["PATH"];

                    foreach (string path in pathVariables)
                    {
                        envPath = envPath + ";" + path;
                    }
                    envVars["PATH"] = envPath;
                }

                Process process = Process.Start(psi);

                DateTime timeNow = DateTime.Now;
                DateTime timeStop = timeNow.AddSeconds(processTimeoutSeconds);

                //
                // Jetzt eine Weile warten und dabei nachsehen, ob x beendet wurde
                //
                while (timeNow < timeStop)
                {
                    success = process.HasExited;
                    if (success)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeNow = DateTime.Now;
                }

                if (success)
                {
                    if (process.ExitCode != 0)
                    {
                        success = false;
                        if (view != null)
                        {
                            view.Progress(LogKeyError, "Failed");
                        }
                    }
                }
            }

            return success;
        }

        public static bool ExecuteProcessAndWaitToFileNoCmd(IProgressCallBack view, bool simulate, string cmd, string options, string strProgress, 
            int processTimeoutSeconds, List<string> pathVariables, string fileName)
        {
            bool success = true;

            string psiFileName = @"c:\windows\system32\cmd.exe";
            string arguments = "/C " + cmd + " " + options;
            if (!string.IsNullOrEmpty(fileName))
            {
                arguments = arguments + " > " + fileName;
            }

            if (view != null)
            {
                view.Progress(LogKeyRun, psiFileName + " " + arguments);
            }

            if (!simulate)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.UseShellExecute = false;
                psi.FileName = psiFileName;
                psi.Arguments = arguments;

                //
                // Add paths to Windows environment variable PATH if required
                //
                if (pathVariables != null)
                {
                    StringDictionary envVars = psi.EnvironmentVariables;

                    string envPath = envVars["PATH"];

                    foreach (string path in pathVariables)
                    {
                        envPath = envPath + ";" + path;
                    }
                    envVars["PATH"] = envPath;
                }

                Process process = Process.Start(psi);

                DateTime timeNow = DateTime.Now;
                DateTime timeStop = timeNow.AddSeconds(processTimeoutSeconds);

                //
                // Jetzt eine Weile warten und dabei nachsehen, ob x beendet wurde
                //
                while (timeNow < timeStop)
                {
                    success = process.HasExited;
                    if (success)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeNow = DateTime.Now;
                }

                if (success)
                {
                    if (process.ExitCode != 0)
                    {
                        success = false;
                        if (view != null)
                        {
                            view.Progress(LogKeyError, "Failed");
                        }
                    }
                }
            }

            return success;
        }

        public static bool ExecuteProcessAndWait(IProgressCallBack view, bool simulate, string cmd, string options, string strProgress, int processTimeoutSeconds, 
            List<string> pathVariables, StringBuilder sbStdErrResult)
        {
            bool success = true;

            if (view != null)
            {
                view.Progress(LogKeyRun, cmd + " " + options + " " + strProgress);
            }

            if (!simulate)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = cmd;
                psi.Arguments = options;
                psi.UseShellExecute = false;
                if (sbStdErrResult != null)
                {
                    psi.RedirectStandardError = true;
                }

                //
                // Add paths to Windows environment variable PATH if required
                //
                if (pathVariables != null)
                {
                    StringDictionary envVars = psi.EnvironmentVariables;

                    string envPath = envVars["PATH"];

                    foreach (string path in pathVariables)
                    {
                        envPath = envPath + ";" + path;
                    }
                    envVars["PATH"] = envPath;
                }


                Process process = Process.Start(psi);

                DateTime timeNow = DateTime.Now;
                DateTime timeStop = timeNow.AddSeconds(processTimeoutSeconds);

                //
                // Jetzt eine Weile warten und dabei nachsehen, ob x beendet wurde
                //
                while (timeNow < timeStop)
                {
                    success = process.HasExited;
                    if (success)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                    timeNow = DateTime.Now;
                }

                if (sbStdErrResult != null)
                {
                    //
                    // Read in all the text from the process with the StreamReader.
                    //
                    string stderrResult;
                    using (StreamReader reader = process.StandardError)
                    {
                        stderrResult = reader.ReadToEnd();
                        Console.Write(stderrResult);
                    }
                    sbStdErrResult.Append(stderrResult);
                }

                if (success)
                {
                    if (process.ExitCode != 0)
                    {
                        success = false;
                        if (sbStdErrResult != null)
                        {
                            Progress(view, sbStdErrResult);
                        }
                        else
                        {
                            if (view != null)
                            {
                                view.Progress(LogKeyError, "Failed");
                            }
                        }
                    }
                }

            }

            return success;
        }

        public static int HexToDecimal(string strHex)
        {
            if (strHex.ToLower().StartsWith("0x"))
            {
                strHex = strHex.Substring(2);
            }

            int base10 = Convert.ToInt32(strHex, 16);

            return base10;
        }

        /// <summary>
        /// Find a text in a text file. Does not work for binary files.
        /// </summary>
        /// <param name="fileName">The file to search</param>
        /// <param name="text">The text to searc for</param>
        /// <returns>true if the text was found in the file</returns>
        public static bool FindTextInFile(string fileName, string text)
        {
            bool success = false;

            StreamReader reader = new StreamReader(fileName);

            string contents = reader.ReadToEnd();

            if (contents.Contains(text))
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Find a text in all text files in a folder and return all filenames in which the text appears.
        /// </summary>
        /// <param name="folder">The fodler to search in</param>
        /// <param name="text">The text to search for</param>
        /// <param name="returnAllFileNames">return all file names in this folder or stop after the first</param>
        /// <param name="fileFilter">The filter specifies which files to search, for example: "*.c"</param>
        /// <param name="recurse">recurse into subfolders</param>
        /// <param name="maxLevel">if recurse=true, then recurse only up to this level, the first folder is 0</param>
        /// <returns>the list of file names</returns>
        public static List<string> FindTextInFolder(string folder, string text, bool returnAllFileNames, string fileFilter, bool recurse, int maxLevel)
        {
            return FindTextInFolder(folder, text, returnAllFileNames, fileFilter, recurse, maxLevel, 0);
        }

        /// <summary>
        /// Find a text in all text files in a folder and return all filenames in which the text appears.
        /// </summary>
        /// <param name="folder">The fodler to search in</param>
        /// <param name="text">The text to search for</param>
        /// <param name="returnAllFileNames">return all file names in this folder or stop after the first</param>
        /// <param name="fileFilter">The filter specifies which files to search, for example: "*.c"</param>
        /// <param name="recurse">recurse into subfolders</param>
        /// <param name="maxLevel">if recurse=true, then recurse only up to this level</param>
        /// <param name="level">if recurse=true, then this is the current level. The level of the original folder passed in is 0</param>
        /// <returns></returns>
        private static List<string> FindTextInFolder(string folder, string text, bool returnAllFileNames, string fileFilter, bool recurse, int maxLevel, int level)
        {
            List<string> fileNames = new List<string>();

            //
            // Find the text in all files in this folder
            //
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            FileInfo[] files = dirInfo.GetFiles(fileFilter);

            if (files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    if (FindTextInFile(file.FullName, text))
                    {
                        fileNames.Add(file.FullName);
                        if (!returnAllFileNames)
                        {
                            break;
                        }
                    }
                }
            }

            if (recurse)
            {
                if (level < maxLevel)
                {
                    //
                    // recurse into all subfolders
                    //
                    DirectoryInfo[] subFolders = dirInfo.GetDirectories();

                    foreach (DirectoryInfo subFolder in subFolders)
                    {
                        List<string> subFolderFileNames = FindTextInFolder(subFolder.FullName, text, returnAllFileNames, fileFilter, recurse, maxLevel, level + 1);
                        foreach (string entry in subFolderFileNames)
                        {
                            fileNames.Add(entry);
                        }
                    }
                }
            }

            return fileNames;
        }


        
        /*
         The XML file contains this, see Tools::public static string FindObjectForAddressInMapFile(string address, string mapFileName)
         
         <Sig>
         <VName>ABS_WHEEL_DATA_A[0].ABS_PHASE_WH_S.BIT6</VName>
         <SName>dummy0000</SName>
         <AAddress>0x40004DFE</AAddress>
         <RAddress>0x004BA</RAddress>
         <MName>CNV_FN4</MName>
         <STyp>flag (Bit1 ABS_WHEEL_DATA_A[0].ABS_PHASE_WH_S.BIT6 Byte0)</STyp>
         <LSB>1.0000000000</LSB>
         <Offset>0.000000</Offset>
         <Min>0.000000</Min>
         <Max>0.000000</Max>
         <Unit></Unit>
         <Comment></Comment>
         <BitMask>0x2</BitMask>
         <BitShift>1</BitShift>
      </Sig>

         */
        /// <summary>
        /// Read file AK300000396_ALL.xml, get all ASAPs (filter by 'filter') and search for the 
        /// &lt;AAddress&gt;0x40004DFE&lt;/AAddress&gt; in the map file to get the object file name.
        /// </summary>
        /// <param name="fullFileName">Full file name of a file such as AK300000396_ALL.xml</param>
        /// <param name="mapFileName">Full file name of a map file such as AK300000396.map</param>
        /// <param name="filter">A filter for the dummy such as 'dummy', if none is specified, then all entries are processed</param>
        public static void ReadAsap(string fullFileName, string mapFileName, string filter, bool displayProgress)
        {
            //
            // Reading the map file into memory does not speed up reading the file compared to reading the file from the hard disk at all.
            //
            FileStream fileStream = File.OpenRead(mapFileName);
            MemoryStream memStream = new MemoryStream();
            memStream.SetLength(fileStream.Length);
            fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);

            string mapFileContents = "";
            fileStream.Position = 0;

            using (StreamReader reader = new StreamReader(mapFileName))
            {
                mapFileContents = reader.ReadToEnd();
                mapFileContents = mapFileContents.ToLower();
            }

            Console.WriteLine("Sname (ASAP);Vname;AAddress;RAddress;MName;STyp;Comment;.obj file;Symbol in source code");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fullFileName);

            int total = xmlDocument.SelectNodes("a2l/SignalClasses/Sig").Count;
            if (displayProgress)
            {
                Console.Error.WriteLine("Total number of nodes to process: " + total);
            }

            XmlNodeList nodes = xmlDocument.SelectNodes("a2l/SignalClasses/Sig");

            int count = 0;
            foreach (XmlNode node in nodes)
            {
                XmlNode subnode;

                subnode = node.SelectSingleNode("SName");
                string strSname = subnode.InnerText;

                if ((string.IsNullOrEmpty(filter)) || (strSname.ToLower().Contains(filter)))
                {
                    subnode = node.SelectSingleNode("VName");
                    string strVname = subnode.InnerText;

                    subnode = node.SelectSingleNode("AAddress");
                    string strAAddress = subnode.InnerText;

                    subnode = node.SelectSingleNode("RAddress");
                    string strRAddress = subnode.InnerText;

                    subnode = node.SelectSingleNode("MName");
                    string strMName = subnode.InnerText;

                    subnode = node.SelectSingleNode("STyp");
                    string strSTyp = subnode.InnerText;

                    subnode = node.SelectSingleNode("Comment");
                    string strComment = subnode.InnerText;

                    string mapFileAddress = strAAddress.ToLower();

                    if (mapFileAddress.StartsWith("0x"))
                    {
                        mapFileAddress = mapFileAddress.Substring(2);
                    }

                    List<string> data = null;

                    //
                    // To speed things up, if the map file does not contain the address, we do not have to search for the object file.
                    //
                    if (mapFileContents.Contains(mapFileAddress))
                    {
                        data = Tools.FindObjectForAddressInMapFile(mapFileAddress, fileStream);
                    }

                    Console.Write(strSname + ";" + strVname + ";" + strAAddress + ";" + strRAddress + ";" + strMName + ";" + strSTyp + ";" + strComment);

                    if ((data != null) && (data.Count > 0))
                    {
                        Console.WriteLine(";" + data[0] + ";" + data[1]);
                    }
                    else
                    {
                        Console.WriteLine(";;");
                    }

                    count++;

                    if (displayProgress)
                    {
                        if (count % 100 == 0)
                        {
                            Console.Error.WriteLine(count + " / " + total);
                        }
                        else
                        {
                            Console.Error.Write(".");
                        }
                    }
                }
            }
        }

        public static string ShrinkToSingleSpaces(string text)
        {
            text = text.Trim();

            text = text.Replace('\t', ' ');
            text = text.Replace('\r', ' ');

            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            return text;
        }

        /*
            The map file contains this, see KitFileTool::public void ReadAsap(string fullFileName, string mapFileName)
         	
            .bss            40004df8	0000029c VERSION/LTO_Cache\ABS\MOT_OBJ\RAMABS_PRIO1.obj
	        ROUGH_ROAD_BITS_WH 40004df8	00000004
            ROUGH_ROAD_LOW_YRATE_DEV_DURATION 40004dfc	00000001
	        ABS_WHEEL_DATA_A 40004dfe	00000050
  
         */
        /// <summary>
        /// Find the address in the map file and return two strings: the object file and the symbol name
        /// </summary>
        /// <param name="address"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static List<string> FindObjectForAddressInMapFile(string address, FileStream fileStream)
        {
            List<string> data = new List<string>();

            string objectFile = "";
            string currentObjectFile = "";
            string symbolName = "";

            address = address.ToLower();

            fileStream.Position = 0;
            StreamReader reader = new StreamReader(fileStream);
           
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.ToLower().Contains(".obj"))
                {
                    currentObjectFile = line;
                }

                if (line.ToLower().Contains(address))
                {
                    objectFile = currentObjectFile;
                    symbolName = line;
                    break;
                }
            }

            //
            // process the line which contains the symbol from the source code
            //
            symbolName = Tools.ShrinkToSingleSpaces(symbolName);

            string[] arText = symbolName.Split(' ');
            if ((arText.Length == 3) && (arText[1] == address))
            {
                //
                // If we have a line such as this one with 3 parts and the middle one is the address, then use only the first part as the symbol
                //
	            // ABS_WHEEL_DATA_A 40004dfe	00000050
                //
                //
                //
                symbolName = arText[0];
            }

            //
            // process the line which contains the object file
            //
            if (!string.IsNullOrEmpty(objectFile))
            {
                //
                // remove multiple spaces by one
                //
                objectFile = Tools.ShrinkToSingleSpaces(objectFile);

                //
                //  .bss            40004df8	0000029c VERSION/LTO_Cache\ABS\MOT_OBJ\RAMABS_PRIO1.obj
                //
                //  get VERSION/LTO_Cache\ABS\MOT_OBJ\RAMABS_PRIO1.obj from the line
                //
                //
                arText = objectFile.Split(' ');
                if (arText[arText.Length - 1].Contains(".obj"))
                {
                    //
                    // if the retrieved part still contains .obj, use it, otherwise use the whole line
                    //
                    objectFile = arText[arText.Length - 1];
                }
            }

            data.Add(objectFile);
            data.Add(symbolName);

            return data;
        }

        public static bool PathContainsSpaces(string path)
        {
            bool success = false;

            if (path.Contains(" "))
            {
                success = true;
            }

            return success;
        }

        public static void StartWindowsExplorer(string path)
        {
            StartProcess("explorer.exe", path);
        }

        public static string UrlDecodeForNotesEmail(string text)
        {
            //
            // remove any characters which are not valid
            //
            string validCharacters = "1234567890qwertzuiopasdfghkjlyxcvbnmQWERTZIUOPASDFGHKJLYXCVBNM()[]{}<>:-_# ";

            for (int i = 0; i < text.Length; i++)
            {
                if (validCharacters.IndexOf(text[i]) == -1)
                {
                    text = text.Replace(text[i], ' ');
                }
            }

            System.Web.HttpUtility.UrlDecode(text);

            return text;
        }

        public static void WriteColumnsAsLists(ListView lv, string fileName, string seperator)
        {
            string line;
            StreamWriter writer = new StreamWriter(fileName);

            for (int j = 0; j < lv.Columns.Count; j++)
            {
                if (j > 0)
                {
                    writer.WriteLine("");
                }
                writer.WriteLine(lv.Columns[j].Text);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < lv.Items.Count; i++)
                {
                    ListViewItem lvi = lv.Items[i];

                    AddEmail(sb, lvi, j, seperator);
                }

                line = sb.ToString();
                writer.WriteLine(line);

            }

            writer.Close();
            writer = null;

            Process.Start(fileName);
        }

        public static void AddEmail(StringBuilder sb, ListViewItem lvi, int index, string separator)
        {
            string text = lvi.SubItems[index].Text;

            if (!string.IsNullOrEmpty(text))
            {
                if (!sb.ToString().Contains(text))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(separator);
                    }
                    sb.Append(text);
                }
            }
        }

        /// <summary>
        /// From a long text which contains a filename at the end, get that filename:
        /// Basically, from the end go back to the last backslash and return the text after the last backslash
        /// 
        /// ReleaseLevelDependencies.cvm -> ReleaseLevelDependencies.cvm
        /// EBS\MSW\PBCService\PBCService_plugin\PbcLib_CAS__MOT_5.9.2.1-b_4_LTO.zip -> PbcLib_CAS__MOT_5.9.2.1-b_4_LTO.zip
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static void ExtractTrailingFilename(string text, out string folder, out string fileName)
        {
            folder = "";
            fileName = text;

            int index = text.LastIndexOf("\\");
            if (index == -1)
            {
                index = text.LastIndexOf("/");
            }

            if (index != -1)
            {
                fileName = text.Substring(index + 1);
                folder = text.Substring(0, index);
            }
        }

        /// <summary>
        /// D:\casdev\sbxs\ffm-mks1\var\FC2D6\EBS\MSW\VhDynSig\VhDynSig_generic\SigVehSwa\Src\SigVehSwaSrv_ScdMain.c ->
        /// project: D:\casdev\sbxs\ffm-mks1\var\FC2D6\EBS\MSW\VhDynSig\VhDynSig_generic\SigVehSwa\project.pj
        /// and
        /// member: Src\SigVehSwaSrv_ScdMain.c
        /// </summary>
        /// <param name="directoryInfo">The file name to parse</param>
        /// <param name="project">Returns the full path to the project.pj</param>
        /// <param name="member">returns the file name relative from the project.pj</param>
        /// <returns></returns>
        public static bool GetParentPj(DirectoryInfo directoryInfo, FileInfo fileInfo, out string project, out string member)
        {
            bool success = false;

            project = "";
            member = "";

            //
            // From the end, find the first project.pj in the folder. The tail is the member
            //
            while (true)
            {
                project = directoryInfo.FullName + "\\project.pj";

                if (File.Exists(project))
                {
                    //
                    // a project.pj was found in this folder
                    //
                    success = true;

                    //
                    // From 
                    // D:\casdev\sbxs\ffm-mks1\var\FC2D6\EBS\MSW\VhDynSig\VhDynSig_generic\SigVehSwa\Src\SigVehSwaSrv_ScdMain.c
                    // remove the leading
                    // D:\casdev\sbxs\ffm-mks1\var\FC2D6\EBS\MSW\VhDynSig\VhDynSig_generic\SigVehSwa
                    //
                    member = fileInfo.FullName;
                    member = member.Remove(0, directoryInfo.FullName.Length);
                    while ((member.Length > 0) && (member[0] == '\\'))
                    {
                        member = member.Substring(1);
                    }

                    break;
                }

                //
                // stop if we are in the root folder d:\
                //
                if (directoryInfo.FullName == directoryInfo.Root.FullName)
                {
                    break;
                }

                //
                //
                // continue with the parent folder
                directoryInfo = directoryInfo.Parent;
            }

            return success;
        }

        public static string AsciiToHex(string ascii, string prefix, string separator)
        {
            char[] array = ascii.ToCharArray();
            string final = "";

            foreach (var i in array)
            {
                string hex = String.Format("{0:X}", Convert.ToInt32(i));

                if ((final.Length > 0) && (!string.IsNullOrEmpty(separator)))
                {
                    final = final + separator;
                }
                if (!string.IsNullOrEmpty(prefix))
                {
                    final = final + prefix;
                }
                final = final + hex;
            }

            final = final.TrimEnd();

            return final;
        }

        public static string AsciiToDecimal(string ascii, string prefix, string separator)
        {
            char[] array = ascii.ToCharArray();
            string final = "";

            foreach (var i in array)
            {
                string hex = String.Format("{0}", Convert.ToInt32(i));

                if ((final.Length > 0) && (!string.IsNullOrEmpty(separator)))
                {
                    final = final + separator;
                }
                if (!string.IsNullOrEmpty(prefix))
                {
                    final = final + prefix;
                }
                final = final + hex;
            }

            final = final.TrimEnd();

            return final;
        }
    }
}

