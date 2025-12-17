using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace CMaurer.Common
{
    public class IntegritySupport
    {
        private const string mks_server_srcsbx_path = "d:/mks/archives/src/";     // For ERTools projects
        private const string mks_server_src3sbx_path = "d:/mks/archives/src3/";   // for VC projects

        private string _hostname = "ffm-mks1";
        private string _port = "";
        private string _portSi = "7001";
        private string _portIm = "7002";

        public const int TYPE_SAFE = 0x01;
        public const int TYPE_TASK = 0x02;
        public const int TYPE_PFCR = 0x04;

        public IntegritySupport()
        {
        }

        public IntegritySupport(string hostname, string portSi, string portIm)
        {
            _hostname = hostname;
            _portSi = portSi;
            _portIm = portIm;
        }

        public IntegritySupport(string hostname, string port)
        {
            _hostname = hostname;
            _port = port;
        }

        public bool ViewProject(IProgressCallBack view, bool recurse, string project, string additionalOptions, string fileName)
        {
            /*
             HBA / 2009CW35_HBA.pj(1K2RAB) variant - subproject
             LINK / project.pj(1.4461.1.349.3.829.2.2918) shared - build - subproject 1.4461.1.349.3.829.2.2918
             LLD / LLD_MOT_SPACE.pj(1K2RAB) variant - subproject
             LVC / 2009CW35_LVC.pj(1K2RAB) variant - subproject
             MMI / 2009CW35_MMI.pj(1K2RAB) variant - subproject
             OSEK / MK60E_MOT_SPACE_OSEK.pj(1K2RAB) variant - subproject
            */

            bool success = false;

            string options = "viewproject --hostname=" + _hostname + " --port=" + _portSi;
            if (recurse)
            {
                options = options + " -R";
            }
            options = options + " " + additionalOptions + " --project=";

            options = options + project;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "si", options, "", 600000, null, fileName);

            return success;
        }

        public bool ConfigureSubprojectTB(IProgressCallBack view, bool simulate, string changePackage, string subprojectDevelopmentPath, string project, string subproject)
        {
            //
            // Using a sandbox:
            // si configuresubproject --hostname=ffm-mks1 --port=7001 --type=variant --subprojectDevelopmentPath=FC1DB --cpid=9380824:5 D:\casdev\sbxs\ffm-mks1\var\FC1DB\EBS\FSW\ADrvAs\ADrvAs_generic\project.pj
            //
            // Using a project:
            // si configuresubproject   --hostname=ffm-mks1 --port=7001 --type=variant --subprojectDevelopmentPath=FC1DA 
            //                          --cpid=9380824:5 --nocloseCP
            //                          --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ADrvAs ADrvAs_generic/project.pj
            //

            bool success = false;

            string options = "configuresubproject --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --type=variant --subprojectDevelopmentPath=" + subprojectDevelopmentPath + " --cpid=" + changePackage + " --nocloseCP";
            options = options + " --project=" + project;
            options = options + " " + subproject;

            success = Tools.ExecuteProcessAndWaitToFile(view, simulate, "si", options, "", 600000, null, null);

            return success;
        }

        public bool ConfigureSubprojectAll(IProgressCallBack view, bool simulate, string changePackage, string subprojectDevelopmentPath, string project, string subproject)
        {
            //
            // Using a sandbox:
            // si configuresubproject --hostname=ffm-mks1 --port=7001 --type=variant --subprojectDevelopmentPath=FC1DB --cpid=9380824:5 D:\casdev\sbxs\ffm-mks1\var\FC1DB\EBS\FSW\ADrvAs\ADrvAs_generic\project.pj
            //
            // Using a project:
            // si configuresubproject   --hostname=ffm-mks1 --port=7001 --type=variant --subprojectDevelopmentPath=FC1DA 
            //                          --cpid=9380824:5 --nocloseCP
            //                          --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ADrvAs ADrvAs_generic/project.pj
            //

            bool success = false;

            string options = "configuresubproject --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --type=variant --subprojectDevelopmentPath=" + subprojectDevelopmentPath + " --cpid=" + changePackage + " --nocloseCP";
            options = options + " --project=" + project;
            options = options + " " + subproject;

            success = Tools.ExecuteProcessAndWaitToFile(view, simulate, "si", options, "", 600000, null, null);

            return success;
        }


        public bool Drop(IProgressCallBack view, bool simulate, string project, string fileName, string changePackageId)
        {
            //
            // trunk:
            //
            // si drop --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC2/FORD_FC2.pj#EBS/FSW/ComAsw --changepackageid=9380824:5 --nocloseCP -f ComAsw_config\project.pj
            //
            // devpath:
            //
            // si drop --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw --changepackageid=9380824:5 --nocloseCP -f ComAsw_config\project.pj
            //

            bool success = false;

            string options = "drop --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --project=" + project;
            options = options + " --changepackageid=" + changePackageId;
            options = options + " --nocloseCP";
            options = options + " -f " + fileName;

            success = Tools.ExecuteProcessAndWait(view, simulate, "si", options, "", 600000, null);

            return success;
        }


        public bool ShareSubproject(IProgressCallBack view, bool simulate, string project, string sharedProject, string changePackageId, string file)
        {
            //
            // trunk:
            // si sharesubproject   --hostname=ffm-mks1 --port=7001 
            //                      --project=#p=d:/mks/archives/src3/FORD_FC2/FORD_FC2.pj#EBS/FSW/ComAsw
            //                      --sharedProject=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#EBS/FSW/ComAsw/ComAsw_config 
            //                      --changepackageid=9380824:5 --nocloseCP ComAsw_config\project.pj
            //
            // devpath:
            // si sharesubproject   --hostname=ffm-mks1 --port=7001 
            //                      --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw
            //                      --sharedProject=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DA#EBS/FSW/ComAsw/ComAsw_config 
            //                      --changepackageid=9380824:5 --nocloseCP ComAsw_config\project.pj
            //

            bool success = false;

            string options = "sharesubproject --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --project=" + project;
            options = options + " --sharedProject=" + sharedProject;
            options = options + " --changepackageid=" + changePackageId;
            options = options + " --nocloseCP " + file;

            success = Tools.ExecuteProcessAndWait(view, simulate, "si", options, "", 600000, null);

            return success;
        }

        public bool ViewCps(IProgressCallBack view, bool simulate, string fileName)
        {
            //
            // si viewcps --hostname=ffm-mks1 --port=7001 --fields=id,summary
            //

            bool success = false;

            string options = "viewcps --hostname=" + _hostname + " --port=" + _portSi + " --fields=id,summary";

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "si", options, "", 600000, null, fileName);

            return success;
        }

        public bool ViewCpsForId(IProgressCallBack view, bool simulate, string id, string fileName)
        {
            //
            // si viewcps --hostname=ffm-mks1 --port=7001 --fields=id,summary 20451979  
            //

            bool success = false;

            string options = "viewcps --hostname=" + _hostname + " --port=" + _portSi + " --fields=id,summary " + id;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "si", options, "", 600000, null, fileName);

            return success;
        }

        public string GetTokenValue(XmlNode node)
        {
            //XmlNode subnode = node.SelectSingleNode("
            return "";
        }

        public void ParseXmlForSharing(string fileName, List<MksWorkItem> workItems)
        {
            //
            // si drop            --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw --changepackageid=9380824:5 --nocloseCP -f ComAsw_config\project.pj
            // si sharesubproject --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw --sharedProject=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DA#EBS/FSW/ComAsw/ComAsw_config --changepackageid=9380824:5 --nocloseCP ComAsw_config\project.pj
            //

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(fileName);

            XmlNodeList nodes = xmlDocument.SelectNodes("Response/WorkItems/WorkItem");

            foreach (XmlNode node in nodes)
            {
                MksWorkItem mksWorkItem = new MksWorkItem();

                mksWorkItem._displayId = node.Attributes["displayId"].InnerText;
                mksWorkItem._id = node.Attributes["id"].InnerText;
                mksWorkItem._modelType = node.Attributes["modelType"].InnerText;
                mksWorkItem._parentID = node.Attributes["parentID"].InnerText;

                XmlNodeList subnodes = node.ChildNodes;

                foreach (XmlNode subnode in subnodes)
                {
                    string name = subnode.Attributes["name"].Value;
                    if (name == "name")
                    {
                        XmlNode nodeName = subnode.SelectSingleNode("Value/TokenValue");
                        mksWorkItem._name = nodeName.InnerText;
                    }
                    else if (name == "parent")
                    {
                        XmlNode nodeName = subnode.SelectSingleNode("Value/TokenValue");
                        mksWorkItem._parent = nodeName.InnerText;
                    }
                    else if (name == "type")
                    {
                        XmlNode nodeName = subnode.SelectSingleNode("Value/TokenValue");
                        mksWorkItem._type = nodeName.InnerText;
                    }
                    else if (name == "devpath")
                    {
                        XmlNode nodeName = subnode.SelectSingleNode("Item");
                        mksWorkItem._devpath = nodeName.Attributes["id"].Value;
                    }
                }

                workItems.Add(mksWorkItem);

            }
        }

        public void ParseTextForSharingTB(string fileName, string project, List<MksWorkItem> workItems)
        {
            //
            // si drop            --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw --changepackageid=9380824:5 --nocloseCP -f ComAsw_config\project.pj
            // si sharesubproject --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw --sharedProject=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DA#EBS/FSW/ComAsw/ComAsw_config --changepackageid=9380824:5 --nocloseCP ComAsw_config\project.pj
            //
            //  2009CW35_CONFIG.pj (1K3RAA) variant-subproject  
            //  ABS/2009CW35_ABS.pj (1K3RAA) variant-subproject  
            //

            StreamReader reader = new StreamReader(fileName);

            string line = "";

            line = reader.ReadLine();
            while (line != null && line.Length > 0)
            {
                line = line.Trim();

                string[] arLine = line.Split(' ');

                if (arLine.Length > 0)
                {
                    MksWorkItem mksWorkItem = new MksWorkItem();

                    mksWorkItem._displayId = arLine[0];
                    mksWorkItem._id = arLine[0];
                    mksWorkItem._modelType = arLine[0];
                    mksWorkItem._parentID = project;

                    workItems.Add(mksWorkItem);
                }

                line = reader.ReadLine();

            }
        }

        public void ParseTextForSharingVC(string fileName, List<MksWorkItem> workItems)
        {
            //
            // si drop            --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw --changepackageid=9380824:5 --nocloseCP -f ComAsw_config\project.pj
            // si sharesubproject --hostname=ffm-mks1 --port=7001 --project=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DB#EBS/FSW/ComAsw --sharedProject=#p=d:/mks/archives/src3/FORD_FC1/FORD_FC1.pj#d=FC1DA#EBS/FSW/ComAsw/ComAsw_config --changepackageid=9380824:5 --nocloseCP ComAsw_config\project.pj
            //

            StreamReader reader = new StreamReader(fileName);

            string line = "";

            line = reader.ReadLine();
            while (line != null && line.Length > 0)
            {
                line = line.Trim();

                string[] arLine = line.Split(' ');

                if (arLine.Length > 0)
                {
                    MksWorkItem mksWorkItem = new MksWorkItem();

                    mksWorkItem._displayId = arLine[0];
                    mksWorkItem._id = arLine[0];
                    mksWorkItem._modelType = arLine[0];
                    mksWorkItem._parentID = arLine[0];

                    workItems.Add(mksWorkItem);
                }

                line = reader.ReadLine();

            }
        }

        public void ParseViewCps(string fileName, List<MksChangePackage> changePackages)
        {
            StreamReader reader = new StreamReader(fileName);
            string line = null;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                string[] arLine = line.Split('\t');

                if (arLine.Length == 2)
                {
                    MksChangePackage changePackage = new MksChangePackage();
                    changePackage._id = arLine[0];
                    changePackage._summary = arLine[1];
                    changePackages.Add(changePackage);
                }
            }

            reader.Close();
        }



        #region ViewIssues
        public bool ViewIssues(IProgressCallBack view, string issueId, string fileName)
        {
            return ViewIssues(view, _hostname, _portIm, issueId, fileName);

        }
        public bool ViewIssues(IProgressCallBack view, string hostname, string port, string issueId, string fileName)
        {
            //
            // si viewissue --hostname=ffm-mks1 --port=7002 10036933
            //
            // PFCR 10036933
            //

            bool success = false;

            // im viewissue --hostname=ffm-mks1 --port=7002 14534809 

            string options = "--fields=ID,Type --query=" + issueId + " --hostname=" + hostname + " --port=" + port;
            //options = options + " " + issueId;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "im issues", options, "", 600000, null, fileName);

            return success;
        }

        public bool ParseIssuesText(IProgressCallBack view, string fileName, List<MksItem> mksItems)
        {
            /*
20659090    Post Freeze Change Request
20661279    SAFE
20666684    Post Freeze Change Request
20688414    SAFE
20771035    SAFE
20877645    Post Freeze Change Request
20884353    SAFE
20884356    Post Freeze Change Request
20884367    Post Freeze Change Request
20884368    Post Freeze Change Request
20980825    SAFE
20980826    SAFE
20980827    SAFE
*/
            mksItems.Clear();

            StreamReader reader = new StreamReader(fileName);
            string line = null;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                while (line.Contains("  "))
                {
                    line = line.Replace("  ", " ");
                }

                string[] arLine = line.Split(new string[] { " ", "\t" }, StringSplitOptions.None);

                if (arLine.Length > 1)
                {
                    MksItem mksItem = new MksItem();

                    mksItem._id = arLine[0];
                    if (line.Contains("SAFE"))
                    {
                        mksItem._type = IntegritySupport.TYPE_SAFE;
                    }
                    else if (line.Contains("Post Freeze Change Request"))
                    {
                        mksItem._type = IntegritySupport.TYPE_PFCR;
                    }
                    else if (line.Contains("TASK"))
                    {
                        mksItem._type = IntegritySupport.TYPE_TASK;
                    }

                    mksItems.Add(mksItem);
                }
            }

            reader.Close();
            reader = null;

            return true;
        }

        #endregion


        public bool ViewSubIssues(IProgressCallBack view, MksItem mksItem, string fileName)
        {
            //
            // im  viewissue --hostname=ffm-mks1 --port=7002 10036933
            //
            // PFCR 10036933
            //

            bool success = false;


            string options = "--hostname=" + _hostname + " --port=" + _portIm + " " + mksItem._id;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "im viewissue", options, "", 600000, null, fileName);

            return success;
        }

        public bool ParseSubIssuesText(IProgressCallBack view, string fileName, List<MksItem> subItems)
        {
            bool success = false;

            StreamReader reader = new StreamReader(fileName);
            string line = null;

            bool inForwardRelationship = false;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                while (line.Contains("  "))
                {
                    line = line.Replace("  ", " ");
                }

                if (line.StartsWith("Forward Relationships:"))
                {
                    inForwardRelationship = true;
                    continue;
                }

                if (line.StartsWith("Type:"))
                {
                    break;
                }

                if (inForwardRelationship)
                {
                    if (line.StartsWith("TASK ") || line.StartsWith("SAFE ") || line.StartsWith("Post Freeze Change Request "))
                    {
                        string[] arLine = line.Split(new string[] { " ", "\t" }, StringSplitOptions.None);

                        if (arLine.Length > 1)
                        {
                            MksItem mksItem = new MksItem();

                            string strId = "";

                            if (arLine[0] == "TASK")
                            {
                                strId = arLine[1].Replace(':', ' ');
                            }
                            else if (arLine[0] == "SAFE")
                            {
                                strId = arLine[1].Replace(':', ' ');
                            }
                            else if (arLine[0] == "Post")
                            {
                                strId = arLine[4].Replace(':', ' ');
                            }

                            while (strId.Contains(" "))
                            {
                                strId = strId.Replace(" ", "");
                            }

                            mksItem._id = strId;

                            if (line.Contains("SAFE"))
                            {
                                mksItem._type = IntegritySupport.TYPE_SAFE;
                            }
                            else if (line.Contains("Post Freeze Change Request"))
                            {
                                mksItem._type = IntegritySupport.TYPE_PFCR;
                            }
                            else if (line.Contains("TASK"))
                            {
                                mksItem._type = IntegritySupport.TYPE_TASK;
                            }

                            subItems.Add(mksItem);
                        }
                    }
                }
            }

            reader.Close();
            reader = null;

            success = true;

            return success;
        }

        #region ViewIssue
        public bool ViewIssue(IProgressCallBack view,string issueId, string fileName)
        {
            return ViewIssue(view, _hostname, _portIm, issueId, fileName);

        }

        public bool ViewIssueQuery(string hostname, string port, string queryName, string fileName)
        {
            //
            // mksapiviewer --xml --iplocal im issues --hostname=ffm-mks3 --port=7002 
            //      --fields="ID,Type,Summary,State,Assigned User,Project,Release Level,Release Index,Pre Freeze Version,Freeze Date,Freeze Version,Release Date,Release Version" 
            //      --showXHTML --query=MX_releases_open
            //

            bool success = false;

            string options = "--xml --iplocal im issues --hostname=" + hostname + " --port=" + port 
                + " --fields=\"ID,Type,Release Number,Release Use,Summary,State,Assigned User,Project,Release Level,Release Index,Pre Freeze Version,"
                + "Freeze Date,Freeze Version,Release Date,Release Version\" --query=" + queryName;

            success = Tools.ExecuteProcessAndWaitToFile(null, false, "mksapiviewer", options, "", 600000, null, fileName);

            XmlViewIssueCleanFileContents(null, fileName);

            return success;
        }

        public bool ViewIssue(IProgressCallBack view, string hostname, string port, string issueId, string fileName)
        {
            //
            // mksapiviewer --xml --iplocal im  viewissue --hostname=ffm-mks1 --port=7002 10036933
            //
            // PFCR 10036933
            //

            bool success = false;

            // TODO mksapiviewer --xml --iplocal im viewissue --hostname=ffm-mks1 --port=7002 14534809 

            string options = "--xml --iplocal im viewissue --hostname=" + hostname + " --port=" + port;
            options = options + " " + issueId;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "mksapiviewer", options, "", 600000, null, fileName);

            return success;
        }

        public static bool MemberInfo(IProgressCallBack view, string hostname, string port, string member, string fileName)
        {
            //
            // si memberinfo --hostname=ffm-mks1 --port=7001 --attributes --nolabels --norule --nolocate --
            // noacl--nochangepackage D:\casdev\C2B1_MLC70\Src\EBS\MSW\ActSrv\ActSrv_generic\Calibration\Src\valve_calibration_safe_barr.c
            //

            bool success = false;

            string options = "--attributes --nolabels --norule --nolocate --noacl --nochangepackage --hostname=" + hostname + " --port=" + port + " " + member;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "si memberinfo", options, "", 600000, null, fileName);

            return success;
        }

        public static bool GetExpertsFromMemberInfo(IProgressCallBack view, string fileName, List<string> experts)
        {
            //
            // Attributes:
            //    access = restricted
            //    experts = Reis; Reisan; Jaeger
            //    file_owner = Reis
            //

            experts.Clear();

            StreamReader reader = new StreamReader(fileName);
            string line = null;

            bool inAttributes = false;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Contains("Attributes:"))
                {
                    inAttributes = true;
                    continue;
                }

                if (inAttributes)
                {
                    string[] arLine = line.Split('=');

                    if (arLine.Length == 2)
                    {
                        //
                        //    experts = Reis; Reisan; Jaeger
                        // 
                        if (arLine[0] == "experts")
                        {
                            string[] arExperts = arLine[1].Split(';');
                            foreach (string expert in arExperts)
                            {
                                experts.Add(expert.Trim());
                            }
                            break;
                        }
                    }
                }
            }

            reader.Close();
            reader = null;

            return true;
        }

        public static void TestExperts()
        {
            const string fileName = "d:\\temp\\member-attributes.txt";

            MemberInfo(null, "ffm-mks1", "7001", @"D:\casdev\C2B1_MLC70\Src\EBS\MSW\ActSrv\ActSrv_generic\Calibration\Src\valve_calibration_safe_barr.c", fileName);
            List<string> experts = new List<string>();
            GetExpertsFromMemberInfo(null, fileName, experts);
        }

        /// <summary>
        /// Replace some characters by spaces which will produce XML error 
        /// Additional information: Ungültiges Zeichen in der angegebenen Codierung. Zeile 188, Position 1.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="fileName"></param>
        public void XmlViewIssueCleanFileContents(IProgressCallBack view, string fileName)
        {
            StreamReader reader = null;
            StreamWriter writer = null;

            string text;

            using (reader = new StreamReader(fileName))
            {
                text = reader.ReadToEnd();
                text = text.Replace('ÿ', ' ');

                //
                // remove 0x1f: it appears in PFCR 10648845
                //
                text = text.Replace('\x001f', ' ');


                text = text.Replace("<?xml version=\"1.1\" ?>", "");
            }
            if (reader != null)
            {
                reader.Close();
            }

            using (writer = new StreamWriter(fileName))
            {
                writer.Write(text);
            }
            if (writer != null)
            {
                writer.Close();
            }
        }

        public void ParseXmlViewIssueExtractCommonData(IProgressCallBack view, string fileName, List<MksSafeData> list)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.Load(fileName);

                XmlNodeList nodes = xmlDocument.SelectNodes("Response/WorkItems/WorkItem/Field");

                //
                // you can get many different XML files with "si viewissue"
                // if the xml has the same structure and we can use this function, then use this function
                //
                //bool isKnownType = false;
                bool isSafe = false;
                bool isTask = false;
                bool isReleaseRecommendation = false;
                bool isPfcr = false;
                bool isReleaseTask = false;

                MksSafeData safe = new MksSafeData();
                int breakCount = 2;

                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes["name"] != null)
                    {
                        string name = node.Attributes["name"].Value;

                        if (name == "Type")
                        {
                            XmlNode nodeItem = node.SelectSingleNode("Item");
                            string id = nodeItem.Attributes["id"].Value;
                            if (id == "SAFE")
                            {
                                isSafe = true;
                            }
                            else if (id == "TASK")
                            {
                                isTask = true;
                            }
                            else if (id == "Release Recommendation")
                            {
                                isReleaseRecommendation = true;
                            }
                            else if (id == "Post Freeze Change Request")
                            {
                                isPfcr = true;
                            }
                            else if (id == "Release Task")
                            {
                                isReleaseTask = true;
                                breakCount = 1;
                            }
                        }
                        else if (name == "Summary")
                        {
                            XmlNode nodeItem = node.SelectSingleNode("Value");
                            safe._topic = nodeItem.InnerText;
                        }
                        else if (name == "ID")
                        {
                            XmlNode nodeItem = node.SelectSingleNode("Value");
                            safe._id = nodeItem.InnerText;

                            if (isTask)
                            {
                                list.Add(safe);
                                break;
                            }
                        }
                        else if (name == "Assigned User")
                        {
                            XmlProjectSheetReadUser(node, safe._assignedUser);

                            breakCount--;
                            if (breakCount <= 0)
                            {
                                list.Add(safe);
                                break;
                            }
                        }
                        else if (name == "Assigned Tester")
                        {
                            XmlProjectSheetReadUser(node, safe._assignedTester);

                            breakCount--;
                            if (breakCount <= 0)
                            {
                                list.Add(safe);
                                break;
                            }
                        }
                        else if (name == "Function Expert")
                        {
                            XmlProjectSheetReadUser(node, safe._assignedUser);

                            list.Add(safe);
                            break;
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                view.ReportError(exception.Message);
            }
        }

        /// <summary>
        /// Find the file which occurs in "text" in the folder specified by the sandbox and create the correct command so that the 
        /// freeze and thaw commands ca be used.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sandbox"></param>
        /// <returns></returns>
        private string FormatPfcrFileRevisionText(IProgressCallBack view, string text, bool parseRevisionText, string sandbox, out string revision, List<FileInfo> fileNames)
        {
            bool parsingPossible = false;

            revision = "";

            //
            // "MSW/VhDynSig/VhDynSig_generic/SigVehSwa/Src/SigVehSwaSrv_ScdMain.c_1.28 --> 1.28.1.6"
            //
            // 0                                          1
            // "EBS\\FSW\\MMI\\MMI_generic\\DIS\\DIS.cidl 1.83->1.85"
            //

            text = Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);

            string line = ModifyRevisionLineAsNeeded(text);

            while (line.Contains("\t"))
            {
                line = line.Replace("\t", " ");
            }
            while (line.Contains(":"))
            {
                line = line.Replace(":", " ");
            }
            //
            // reduce all funny arrows such as "--->" "-->" to "->"
            //
            while (line.Contains("-->"))
            {
                line = line.Replace("-->", "->");
            }
            while (line.Contains(" ->"))
            {
                line = line.Replace(" ->", "->");
            }
            while (line.Contains("-> "))
            {
                line = line.Replace("-> ", "->");
            }

            //
            // reduce multiple spaces to one
            //
            while (line.Contains("  "))
            {
                line = line.Replace("  ", " ");
            }

            string[] arLine = line.Split(new string[] { "->" }, StringSplitOptions.None);
            if (arLine.Length == 2)
            {
                //
                // arLine
                // 0:   "DiagAsw.cvm 1.3"
                // 1:   "1.3.1.1"
                //
                MksChangePackageEntry entry = new MksChangePackageEntry();
                string[] arFile = arLine[0].Trim().Split(' ');
                //
                // arFile
                // 0:   "DiagAsw.cvm"
                // 1:   "1.3"
                //

                string sandboxFolder;
                string sandboxFilename;
                string revisionFolder;
                string revisionFileName = "" ;

                //
                // get the folder and file name from the text in the PFCR
                //
                if (arFile.Length == 2)
                {
                    //
                    // "StStMg.cvm 1.11.1.7.1.2->1.11.1.7.1.5"
                    //
                    Tools.ExtractTrailingFilename(arFile[0], out revisionFolder, out revisionFileName);
                }
                else if (arFile.Length == 3)
                {
                    //
                    // "FB3R6: StStMg.cvm 1.11.1.7.1.2->1.11.1.7.1.5"
                    //
                    Tools.ExtractTrailingFilename(arFile[1], out revisionFolder, out revisionFileName);
                }
                else
                {
                    //
                    // do what?
                    // 
                }

                //
                // Get the folder from the sandbox: "d:\\casdev\\sbxs\\ffm-mks1\\var\\FB3R6\\FORD_FB3.pj" -> 
                //      folder: "d:\\casdev\\sbxs\\ffm-mks1\\var\\FB3R6" 
                //      filename: "FORD_FB3.pj"
                //
                Tools.ExtractTrailingFilename(sandbox, out sandboxFolder, out sandboxFilename);

                Tools.GetAllFileNamesInSandbox(view, sandboxFolder, revisionFileName, fileNames);
                // Tools.GetAllFileNames(view, sandboxFolder, revisionFileName, true, fileNames);

                revision = arLine[1];
            }

            return "";
        }

        public void ParseXmlViewIssuePfcr(IProgressCallBack view, string fileName, bool parseRevisionText, string sandbox, List<MksChangePackageEntry> entries)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(fileName);

            XmlNodeList nodes = xmlDocument.SelectNodes("Response/WorkItems/WorkItem/Field");

            bool isPfcr = false;
            string revisions = null;

            List<FileInfo> fileNames = new List<FileInfo>();

            foreach (XmlNode node in nodes)
            {
                view.DoEvents();

                if (node.Attributes["name"] != null)
                {
                    string name = node.Attributes["name"].Value;

                    if (name == "Type")
                    {
                        XmlNode nodeItem = node.SelectSingleNode("Item");
                        string id = nodeItem.Attributes["id"].Value;
                        if (id == "Post Freeze Change Request")
                        {
                            isPfcr = true;
                        }
                    }

                    if (isPfcr && (name == "New Revisions"))
                    {
                        XmlNode nodeItem = node.SelectSingleNode("Value/TokenValue");
                        if (nodeItem != null)
                        {
                            revisions = nodeItem.InnerText;

                            string[] arRevisions = revisions.Split('\n');
                            foreach (string revisionRaw in arRevisions)
                            {
                                string newRevision;

                                fileNames.Clear();
                                FormatPfcrFileRevisionText(view, revisionRaw, parseRevisionText, sandbox, out newRevision, fileNames);

                                foreach (FileInfo fileInfo in fileNames)
                                {
                                    view.DoEvents();

                                    string folder = fileInfo.DirectoryName;
                                    //string fileName2 = fileInfo.FullName;
                                    DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
                                    string project = "";
                                    string member = "";
                                    bool success = Tools.GetParentPj(directoryInfo, fileInfo, out project, out member);

                                    MksChangePackageEntry entry = new MksChangePackageEntry();

                                    entry._member = member;
                                    entry._project = project;
                                    entry._revision = newRevision;

                                    entries.Add(entry);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string DateTime2String(DateTime ?dt)
        {
            string s = string.Format("{0:00}.{1:00}.{2:0000}", dt.Value.Day, dt.Value.Month, dt.Value.Year);

            return s;
        }

        public void ParseXmlViewIssuesQuery(IProgressCallBack view, string fileName)
        {
            string strId = "";
            string strType = "";
            string strReleaseUse = "";
            string strReleaseNumber = "";
            string strProject = "";
            string strState = "";
            string strSwpm = "";
            string strProjectId = "";
            string strReleaseLevel = "";
            string strReleaseIndex = "";
            string strPreFreezeDate = "";
            string strPreFreezeVersion = "";
            string strFreezeDate = "";
            string strFreezeVersion = "";
            string strReleaseDate = "";
            string strReleaseVersion = "";

            DateTime dtPreFreezeDate = new DateTime();
            DateTime dtFreezeDate = new DateTime();
            DateTime dtReleaseDate = new DateTime();

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(fileName);

            XmlNodeList nodes = xmlDocument.SelectNodes("Response/WorkItems/WorkItem");

            StreamWriter htmlWriter = new StreamWriter(fileName + ".html");

            htmlWriter.WriteLine("<html>");
            htmlWriter.WriteLine("<table border='1'>");

            string[] h = { "#ID", "Type", "Use", "No", "Project", "State", "SWPM", "ID", "MLC", "PFDate","PFVersion", "FDate", "FVersion", "RDate", "RVersion" };

            StringBuilder sbData = new StringBuilder();
            foreach (string s in h)
            {
                htmlWriter.Write("<th>" + s + "</th>");
                if (sbData.Length > 0)
                {
                    sbData.Append("|");
                }
                sbData.Append(s);
            }
            Console.WriteLine(sbData.ToString());

            foreach (XmlNode node in nodes)
            {
                if (view != null)
                {
                    view.DoEvents();
                }

                htmlWriter.WriteLine("<tr>");

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Attributes["name"] != null)
                    {
                        string name = childNode.Attributes["name"].Value;

                        if (name == "ID")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strId = nodeItem.InnerText;
                        }
                        else if (name == "Summary")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strProject = nodeItem.InnerText;
                        }
                        else if (name == "Release Number")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strReleaseNumber = nodeItem.InnerText;
                        }
                        else if (name == "Release Use")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strReleaseUse = nodeItem.InnerText;
                        }
                        else if (name == "Release Level")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strReleaseLevel = nodeItem.InnerText;
                        }
                        else if (name == "Release Index")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strReleaseIndex = nodeItem.InnerText;
                        }
                        else if (name == "Pre Freeze Version")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strPreFreezeVersion = nodeItem.InnerText;
                        }
                        else if (name == "Freeze Version")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strFreezeVersion = nodeItem.InnerText;
                        }
                        else if (name == "Release Version")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strReleaseVersion = nodeItem.InnerText;
                        }

                        else if ((name == "Release Date"))
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            string s1 = nodeItem.InnerText;
                            string s2 = s1;

                            if (DateTime.TryParse(s1, out dtReleaseDate))
                            {
                                s2 = DateTime2String(dtReleaseDate);
                            }
                            else
                            {
                            }
                            strReleaseDate = s2;
                        }
                        else if ((name == "Freeze Date"))
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Value");
                            strFreezeDate = nodeItem.InnerText;
                            strPreFreezeDate = strFreezeDate;

                            if (DateTime.TryParse(strFreezeDate, out dtFreezeDate))
                            {
                                TimeSpan ts2Weeks = new TimeSpan(14, 0, 0, 0);

                                dtPreFreezeDate = dtFreezeDate.Subtract(ts2Weeks);

                                strFreezeDate = DateTime2String(dtFreezeDate);
                                strPreFreezeDate = DateTime2String(dtPreFreezeDate);
                            }
                            else
                            {
                            }
                        }
                        else if (name == "Project")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Item");
                            strProjectId = nodeItem.Attributes["id"].Value;
                        }
                        else if (name == "Type")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Item");
                            strType = nodeItem.Attributes["id"].Value;
                        }
                        else if (name == "State")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Item");
                            strState = nodeItem.Attributes["id"].Value;
                        }
                        else if (name == "Assigned User")
                        {
                            XmlNode nodeItem = childNode.SelectSingleNode("Item/Field/Value");
                            strSwpm = nodeItem.InnerText;
                        }
                    }
                }
                DateTime dtToday = DateTime.Today;

                //string[] h = { "#ID", "Type", "Use", "No", "Project", "State", "SWPM", "ID", "MLC", "PFDate", "PFVersion", "FDate", "FVersion", "RDate", "RVersion" };

                sbData.Clear();
                sbData.Append(strId);
                sbData.Append("|");
                sbData.Append(strType);
                sbData.Append("|");
                sbData.Append(strReleaseUse);
                sbData.Append("|");
                sbData.Append(strReleaseNumber);
                sbData.Append("|");
                sbData.Append(strProject);
                sbData.Append("|");
                sbData.Append(strState);
                sbData.Append("|");
                sbData.Append(strSwpm);
                sbData.Append("|");
                sbData.Append(strProjectId);
                sbData.Append("|");
                sbData.Append(strReleaseLevel + "/" + strReleaseIndex);
                sbData.Append("|");
                sbData.Append(strPreFreezeDate);
                sbData.Append("|");
                sbData.Append(strPreFreezeVersion);
                sbData.Append("|");
                sbData.Append(strFreezeDate);
                sbData.Append("|");
                sbData.Append(strFreezeVersion);
                sbData.Append("|");
                sbData.Append(strReleaseDate);
                sbData.Append("|");
                sbData.Append(strReleaseVersion);

                Console.WriteLine(sbData.ToString());

                //string[] h = { "#ID", "Type", "Use", "No", "Project", "State", "SWPM", "ID", "MLC", "PFDate", "PFVersion", "FDate", "FVersion", "RDate", "RVersion" };

                htmlWriter.Write("<td>" + strId + "</td>"
                    + "<td>" + strType + "</td>"
                    + "<td>" + strReleaseUse + "</td>"
                    + "<td>" + strReleaseNumber + "</td>"
                    + "<td>" + strProject + "</td>"
                    + "<td>" + strState + "</td>"
                    + "<td>" + strSwpm + "</td>"
                    + "<td>" + strProjectId + "</td>"
                    + "<td>" + strReleaseLevel + "/" + strReleaseIndex + "</td>");

                if ((dtPreFreezeDate < dtToday) && (string.IsNullOrEmpty(strPreFreezeVersion) && (!strReleaseUse.Contains("CPF"))))
                {
                    htmlWriter.Write(""
                    + "<td style='background-color:#ff0000;'>" + strPreFreezeDate + "</td>"
                    + "<td style='background-color:#ff0000;'>" + strPreFreezeVersion + "</td>");

                }
                else
                {
                    htmlWriter.Write(""
                    + "<td>" + strPreFreezeDate + "</td>"
                    + "<td>" + strPreFreezeVersion + "</td>");
                }

                if ((dtFreezeDate < dtToday) && (string.IsNullOrEmpty(strFreezeVersion)))
                {
                    htmlWriter.Write(""
                    + "<td style='background-color:#ff0000;'>" + strFreezeDate + "</td>"
                    + "<td style='background-color:#ff0000;'>" + strFreezeVersion + "</td>");

                }
                else
                {
                    htmlWriter.Write(""
                    + "<td>" + strFreezeDate + "</td>"
                    + "<td>" + strFreezeVersion + "</td>");
                }

                if ((dtReleaseDate < dtToday) && (string.IsNullOrEmpty(strReleaseVersion)))
                {
                    htmlWriter.Write(""
                    + "<td style='background-color:#ff0000;'>" + strReleaseDate + "</td>"
                    + "<td style='background-color:#ff0000;'>" + strReleaseVersion + "</td>");

                }
                else
                {
                    htmlWriter.Write(""
                    + "<td>" + strReleaseDate + "</td>"
                    + "<td>" + strReleaseVersion + "</td>");
                }

                htmlWriter.WriteLine("</tr>");
            }
            htmlWriter.WriteLine("</table>");
            htmlWriter.WriteLine("</html>");

            htmlWriter.Close();
            htmlWriter = null;
        }

        private string ModifyRevisionLineAsNeeded(string text)
        {
            //
            // "DiagAsw.cvm     1.3 -> 1.3.1.1"
            // "MSW/VhDynSig/VhDynSig_generic/SigVehSwa/Src/SigVehSwaSrv_ScdMain.c_1.28 --> 1.28.1.6"
            //

            //
            // go backward from the right and replace the last "_" by a space " "
            // "MSW/VhDynSig/VhDynSig_generic/SigVehSwa/Src/SigVehSwaSrv_ScdMain.c_1.28 --> 1.28.1.6"
            // ->
            // "MSW/VhDynSig/VhDynSig_generic/SigVehSwa/Src/SigVehSwaSrv_ScdMain.c 1.28 --> 1.28.1.6"
            //
            // "DiagAsw.cvm\t1.3 -> 1.3.1.1" 
            // ->
            // "DiagAsw.cvm 1.3 -> 1.3.1.1" 
            //

            for (int index = text.Length - 1; index > 0; index--)
            {
                if ("1234567890".IndexOf(text[index]) != -1)
                {
                    if (index > 0) 
                    {
                        if ((text[index - 1] == '_'))
                        {
                            text = text.ReplaceAt(index - 1, ' ');
                            break;
                        }
                    }
                }
            }

            return text;

        }
        #endregion

        #region ViewProjectSheet
        public bool ViewProjectSheet(IProgressCallBack view, string id, string fileName)
        {
            //
            // im viewissue --hostname=ffm-mks3 --port=7002 --xmlapi 1187025 
            //

            bool success = false;

            string options = "--xml --iplocal im viewissue --hostname=" + _hostname + " --port=" + _portIm;
            options = options + " " + id;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "mksapiviewer", options, "", 600000, null, fileName);

            return success;
        }

        public void ParseXmlViewProjectSheet(string fileName, List<MksProjectSheetLine> entries)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(fileName);

            XmlNodeList nodes = xmlDocument.SelectNodes("Response/WorkItems/WorkItem/Field");

            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];

                string text = node.Attributes["name"].Value;

                //
                // <Field name="MMI Resp.">
                //
                string tag = "Resp.";
                if (text.EndsWith(tag) && (text.Length > tag.Length))
                {

                    //
                    // read the next 4 items
                    //
                    MksProjectSheetLine line = new MksProjectSheetLine();

                    line._functionModel = text;

                    XmlProjectSheetReadUser(node, line._data[0]);

                    i++;
                    node = nodes[i];
                    XmlProjectSheetReadUser(node, line._data[1]);

                    i++;
                    node = nodes[i];
                    XmlProjectSheetReadUser(node, line._data[2]);

                    i++;
                    node = nodes[i];
                    XmlProjectSheetReadUser(node, line._data[3]);

                    entries.Add(line);
                }
            }
        }

        public void XmlProjectSheetReadUser(XmlNode node, MksUserData data)
        {
            bool success = true;
            /*
<Field name="MMI Resp.">
<Item id="SufanaL" modelType="im.User" displayId="SufanaL">
<Field name="fullname">
<Value dataType="string">
<TokenValue>Sufana, Lucian</TokenValue>
</Value>
</Field>
<Field name="Email">
<Value dataType="string">
<TokenValue>lucian.sufana@continental-corporation.com</TokenValue>
</Value>
</Field>
</Item>
</Field>
 
 */
            XmlNode nodeItem = node.SelectSingleNode("Item");

            int count = 0;

            string id = "";
            string fullName = "";
            string email = "";


            try
            {
                id = nodeItem.Attributes["id"].Value;
                count++;

                string displayId = nodeItem.Attributes["displayId"].Value;

                XmlNodeList subnodes = nodeItem.ChildNodes;

                /*
      <Field name="fullname">
        <Value dataType="string">
          <TokenValue>Sufana, Lucian</TokenValue>
        </Value>
      </Field>
      <Field name="Email">
        <Value dataType="string">
          <TokenValue>lucian.sufana@continental-corporation.com</TokenValue>
        </Value>
      </Field>
                 */


                /*
                  <Field name="fullname">
                    <Value dataType="string">
                      <TokenValue>Sufana, Lucian</TokenValue>
                    </Value>
                  </Field>
                 */
                XmlNode subnode = subnodes[0];
                XmlNode subsubnode = subnode.SelectSingleNode("Value");
                fullName = subsubnode.InnerText;

                subnode = subnodes[1];
                subsubnode = subnode.SelectSingleNode("Value");
                email = subsubnode.InnerText;

            }
            catch (Exception)
            {
                success = false;
            }

            if (success)
            {
                data._mksId = id;
                data._humanReadableName = fullName;
                data._email = email;
            }

        }

        #endregion


        #region ViewCp
        public bool ViewCpText(IProgressCallBack view, string changePackageId, string fileName)
        {
            //
            // si viewcp --hostname=ffm-mks1 --port=7001 9380824:1
            //

            bool success = false;

            string options = "viewcp --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " " + changePackageId;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "si", options, "", 600000, null, fileName);

            return success;
        }

        public bool ViewCpXml(IProgressCallBack view, string changePackageId, string fileName)
        {
            //
            // si viewcp --hostname=ffm-mks1 --port=7001 9380824:1
            //

            bool success = false;

            string options = "--xml --iplocal si viewcp --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " " + changePackageId;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "mksapiviewer", options, "", 600000, null, fileName);

            return success;
        }

        public void ParseXmlViewCp(string fileName, List<MksChangePackageEntry> entries)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(fileName);

            XmlNodeList nodes = xmlDocument.SelectNodes("Response/WorkItems/WorkItem/Field/List/Item");

            foreach (XmlNode node in nodes)
            {
                MksChangePackageEntry entry = new MksChangePackageEntry();

                string id = node.Attributes["id"].Value;
                string[] arId = id.Split(':');

                XmlNodeList subnodes = node.ChildNodes;

                int count = 0;

                foreach (XmlNode subnode in subnodes)
                {

                    try
                    {
                        string name = subnode.Attributes["name"].Value;
                        if (name == "project")
                        {
                            XmlNode nodeName = subnode.SelectSingleNode("Item");
                            entry._project = nodeName.Attributes["id"].Value;
                            count++;
                        }
                        else if (name == "member")
                        {
                            XmlNode nodeName = subnode.SelectSingleNode("Item");
                            entry._member = nodeName.Attributes["id"].Value;
                            count++;
                        }
                        else if (name == "revision")
                        {
                            XmlNode nodeName = subnode.SelectSingleNode("Item");
                            entry._revision = nodeName.Attributes["id"].Value;
                            count++;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (count == 3)
                {
                    entries.Add(entry);
                }

            }
        }

//        public void ParseTextViewCp(string fileName, List<MksChangePackageEntry> entries)
        public void ParseTextViewCp(string fileName, MksChangePackage cp)
        {
            List<MksChangePackageEntry> entries = cp._changePackageEntries;

            //
            // 19148165:1	DBC Update to version 20.20.142 [D2UX MY21]
            //
            // 0                   1                    2                            3                                 4
            // Update Archive      vec_gmd2uxmoc.c 1.21.1.39.2.1   d:/ mks / archives / src / BUS / 2009CW35_BUS.pj G99BAB
            // Update              vec_gmd2uxmoc.c 1.21.1.40      d:/ mks / archives / src / BUS / 2009CW35_BUS.pj G99BAB

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    while (line.Contains("Update Archive"))
                    {
                        line = line.Replace("Update Archive", "UpdateArchive");
                    }
                    while (line.Contains("Update Revision"))
                    {
                        line = line.Replace("Update Revision", "UpdateRevision");
                    }
                    while (line.Contains("\t"))
                    {
                        line = line.Replace("\t", " ");
                    }

                    while (line.Contains("  "))
                    {
                        line = line.Replace("  ", " ");
                    }

                    string[] arLine = line.Split(new string[] { " " }, StringSplitOptions.None);
                    if (arLine.Length == 5)
                    {
                        MksChangePackageEntry cpEntry = new MksChangePackageEntry();

                        cpEntry._devpath = arLine[4];
                        cpEntry._project = arLine[3];
                        cpEntry._member = arLine[1];
                        cpEntry._revision = arLine[2];
                        entries.Add(cpEntry);
                    }
                }
            }
        }        
        #endregion

        #region Freeze - Thaw
        /*
        D:\cmaurer\MKS\share>si viewcp --hostname=ffm-mks1 --port=7001 --fields=Member,Revision,Project,Location,Variant 9718361:2 
        9718361:2	CoDeg: review and fix degradation for Variant codable Roll Rate Sensor - Var_get_rrs_active()
        Szupper, Tibor (SzupperT)	Jun 30, 2016 1:47:50 PM	Open 	Development 
        Propagated: 
        Propagated By: 
        VhDynSig.cvm	1.5.1.1	d:/mks/archives/src3/FORD_FC1/EBS/MSW/VhDynSig/VhDynSig_config/project.pj	d:/mks/archives/src3/FORD_FC1/EBS/MSW/VhDynSig/VhDynSig_config/VhDynSig.cvm	
        FsFallbackMonitor.cidl	4.93.2.2.1.1	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackMonitor/project.pj	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackMonitor/rcs/FsFallbackMonitor.cidl	
        Src/mon_degr_ayc_sens.c	4.37.1.2.2.1	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackMonitor/project.pj	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackMonitor/Src/rcs/mon_degr_ayc_sens.c	
        FsFallbackSensor.cidl	4.60.1.4.1.1	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackSensor/project.pj	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackSensor/rcs/FsFallbackSensor.cidl	
        Src/sens_degr.c	4.38.2.1	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackSensor/project.pj	d:/mks/archives/src3/Cluster_Pool_Generic/MSW/CoDeg_generic/FsFallbackSensor/Src/rcs/sens_degr.c	
        */

        public bool Thaw(IProgressCallBack view, bool simulate, string project, string devpath, string member, string fileName)
        {
            bool success = false;

            //
            // si thaw --hostname=ffm-mks1 --port=7001 --devpath=FC1DB --project=d:/mks/archives/src3/FORD_FC1/EBS/MSW/VhDynSig/VhDynSig_config/project.pj d:/mks/archives/src3/FORD_FC1/EBS/MSW/VhDynSig/VhDynSig_config/VhDynSig.cvm
            //

            string options = "thaw --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --project=" + project;
            if (!string.IsNullOrEmpty(devpath))
            {
                options = options + " --devpath=" + devpath;
            }
            options = options + " " + member;

            success = Tools.ExecuteProcessAndWaitToFile(view, simulate, "si", options, "", 600000, null, fileName);

            return success;
        }

        public bool ThawInSandbox(IProgressCallBack view, bool simulate, string sandbox, string member)
        {
            bool success = false;

            //
            // si thaw --hostname=ffm-mks1 --port=7001 --sandbox=D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\project.pj D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\DIS.cidl
            //

            string options = "thaw --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --sandbox=" + sandbox;
            options = options + " " + member;

            success = Tools.ExecuteProcessAndWaitToFile(view, simulate, "si", options, "", 600000, null, null);

            return success;
        }


        public bool Freeze(IProgressCallBack view, bool simulate, string project, string devpath, string member, string fileName)
        {
            bool success = false;

            //
            // si thaw --hostname=ffm-mks1 --port=7001 --devpath=FC1DB --project=d:/mks/archives/src3/FORD_FC1/EBS/MSW/VhDynSig/VhDynSig_config/project.pj d:/mks/archives/src3/FORD_FC1/EBS/MSW/VhDynSig/VhDynSig_config/VhDynSig.cvm
            //

            string options = "freeze --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --project=" + project;
            if (!string.IsNullOrEmpty(devpath))
            {
                options = options + " --devpath=" + devpath;
            }
            options = options + " " + member;

            success = Tools.ExecuteProcessAndWaitToFile(view, simulate, "si", options, "", 600000, null, fileName);

            return success;
        }

        public bool FreezeInSandbox(IProgressCallBack view, bool simulate, string sandbox, string member)
        {
            bool success = false;

            //
            // si freeze --hostname=ffm-mks1 --port=7001 --sandbox=D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\project.pj D:\casdev\sbxs\ffm-mks1\var\FC1R6\EBS\FSW\MMI\MMI_generic\DIS\DIS.cidl
            //

            string options = "freeze --hostname=" + _hostname + " --port=" + _portSi;
            options = options + " --sandbox=" + sandbox;
            options = options + " " + member;

            success = Tools.ExecuteProcessAndWaitToFile(view, simulate, "si", options, "", 600000, null, null);

            return success;
        }


#endregion

        public bool Checkout(IProgressCallBack view, bool simulate, string sandbox, string fileName, string revision)
        {
            bool success = true;

            //
            // co = check out
            //
            string cmd = "co " + "--hostname=" + _hostname + " --port=" + _portSi;
            cmd = cmd + " --batch --nomerge --nolock --yes " + " --sandbox='" + sandbox + "' --revision=" + revision + " " + fileName;

            success = Tools.ExecuteProcessAndWaitToFile(view, simulate, "si", cmd, "", 600000, null, null);

            return success;
        }

        #region Sandboxes
        public bool Sandboxes(IProgressCallBack view, string fileName)
        {
            //
            // si viewissue --hostname=ffm-mks1 --port=7002 10036933
            //
            // PFCR 10036933
            //

            bool success = false;

            string options = "sandboxes --hostname=" + _hostname + " --port=" + _portSi;

            success = Tools.ExecuteProcessAndWaitToFile(view, false, "si", options, "", 600000, null, fileName);

            return success;
        }

        public void ParseTxtSandboxes(string fileName, List<MksSandbox> entries)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    if ((line.IndexOf(mks_server_srcsbx_path) != -1) || (line.IndexOf(mks_server_src3sbx_path) != -1))
                    {
                        MksSandbox item = new MksSandbox();

                        string[] arLine = line.Split(new string[] { "->" }, StringSplitOptions.None);
                        if (arLine.Length == 2)
                        {
                            item._sandbox = arLine[0].Trim();
                            item._project = arLine[1].Trim();
                            entries.Add(item);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
