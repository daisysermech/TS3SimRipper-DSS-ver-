using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using s3pi.Package;
using s3pi.Interfaces;
using s3pi.ImageResource;

namespace TS3SimRipper
{
    public partial class Form1 : Form
    {
        Package[] gamePackages = new Package[0];
        string[] gamePackageNames = new string[0];
        bool[] isBasegamePackage = new bool[0];
        //List<Package> currentCCPackages = new List<Package>();
        //List<string> currentCCPackNames = new List<string>();
        Package TroubleshootPackage = (Package)Package.NewPackage(0);

        private bool DetectFilePaths()
        {
            if (currentGame == Game.SimsMedieval) return DetectFilePathsTSM();
            else return DetectFilePathsTS3();
        }

        private bool DetectFilePathsTSM()
        {
            try
            {
                if (Properties.Settings.Default.TSMPath == null || Properties.Settings.Default.TSMPath.Count < 1)
                {
                    RegistryKey gameRoot = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Electronic Arts\\The Sims Medieval");
                    if (gameRoot == null)
                    {
                        gameRoot = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Sims Medieval(Steam)");
                    }
                    if (gameRoot != null)
                    { 
                        System.Collections.Specialized.StringCollection gamePaths = new System.Collections.Specialized.StringCollection();
                        gamePaths.Add((string)gameRoot.GetValue("Install Dir"));
                        Properties.Settings.Default.TSMPath = gamePaths;
                        Properties.Settings.Default.Save();
                    }
                }

                if (String.Compare(Properties.Settings.Default.TSMModsPath, " ") <= 0)
                {
                    string tmp = Properties.Settings.Default.TSMPath != null && Properties.Settings.Default.TSMPath.Count > 0 ? 
                        Properties.Settings.Default.TSMPath[0] + "\\Mods" : "";
                    if (Directory.Exists(tmp))
                    {
                        Properties.Settings.Default.TSMModsPath = tmp; Properties.Settings.Default.Save();
                    }
                }

                if (String.Compare(Properties.Settings.Default.TSMSavesPath, " ") <= 0)
                {
                    string tmp = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\The Sims Medieval\\SavedSims";
                    if (!Directory.Exists(tmp))
                    {
                        string[] tmp2 = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\", "*Sims Medieval*", SearchOption.AllDirectories);
                        if (tmp2.Length == 1)
                        {
                            // tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            tmp = tmp2[0] + Path.DirectorySeparatorChar + "SavedSims";
                        }
                        else if (tmp2.Length > 1)
                        {
                            if (tmp2[0].Length < tmp2[1].Length)
                                tmp = tmp2[0] + Path.DirectorySeparatorChar + "SavedSims";
                            //tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            else
                                tmp = tmp2[1] + Path.DirectorySeparatorChar + "SavedSims";
                            //tmp = Path.GetDirectoryName(tmp2[1]) + Path.DirectorySeparatorChar;
                        }
                    }
                    if (tmp != null) Properties.Settings.Default.TSMSavesPath = tmp; Properties.Settings.Default.Save();
                }

                if (Properties.Settings.Default.TSMPath == null) return false;
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (string s in Properties.Settings.Default.TSMPath)
            {
                if (!Directory.Exists(s)) return false;
            }
            return true;
        }

        private bool DetectFilePathsTS3()
        {
            try
            {
                if (Properties.Settings.Default.TS3Path == null || Properties.Settings.Default.TS3Path.Count < 1)
                {
                    RegistryKey gameRoot = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Sims");
                    if (gameRoot != null)
                    {
                        string regNode = "Sims";
                        string[] gamePacks = gameRoot.GetSubKeyNames();
                        if (gamePacks.Length == 0)
                        {
                            gameRoot = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Sims(Steam)");
                            if (gameRoot != null)
                            {
                                regNode = "Sims(Steam)";
                                gamePacks = gameRoot.GetSubKeyNames();
                            }
                        }
                        System.Collections.Specialized.StringCollection gamePaths = new System.Collections.Specialized.StringCollection();
                        for (int i = 0; i < gamePacks.Length; i++)
                        {
                            gamePaths.Add((string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\" + regNode + "\\" + gamePacks[i], "Install Dir", null));
                        }
                        Properties.Settings.Default.TS3Path = gamePaths;
                        Properties.Settings.Default.Save();
                    }
                }

                if (String.Compare(Properties.Settings.Default.TS3ModsPath, " ") <= 0)
                {
                    string tmp = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\The Sims 3\\Mods";
                    if (!Directory.Exists(tmp))
                    {
                        string[] tmp2 = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\", "*Sims 3*", SearchOption.AllDirectories);
                        if (tmp2.Length == 1)
                        {
                            // tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            tmp = tmp2[0] + Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar;
                        }
                        else if (tmp2.Length > 1)
                        {
                            if (tmp2[0].Length < tmp2[1].Length)
                                tmp = tmp2[0] + Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar;
                            //tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            else
                                tmp = tmp2[1] + Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar;
                            //tmp = Path.GetDirectoryName(tmp2[1]) + Path.DirectorySeparatorChar;
                        }
                    }
                    if (tmp != null) Properties.Settings.Default.TS3ModsPath = tmp; Properties.Settings.Default.Save();
                }

                if (String.Compare(Properties.Settings.Default.TS3SavesPath, " ") <= 0)
                {
                    string tmp = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\The Sims 3\\Saves";
                    if (!Directory.Exists(tmp))
                    {
                        string[] tmp2 = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\", "*Sims 3*", SearchOption.AllDirectories);
                        if (tmp2.Length == 1)
                        {
                            // tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            tmp = tmp2[0] + Path.DirectorySeparatorChar + "Saves";
                        }
                        else if (tmp2.Length > 1)
                        {
                            if (tmp2[0].Length < tmp2[1].Length)
                                tmp = tmp2[0] + Path.DirectorySeparatorChar + "Saves";
                            //tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            else
                                tmp = tmp2[1] + Path.DirectorySeparatorChar + "Saves";
                            //tmp = Path.GetDirectoryName(tmp2[1]) + Path.DirectorySeparatorChar;
                        }
                    }
                    if (tmp != null) Properties.Settings.Default.TS3SavesPath = tmp; Properties.Settings.Default.Save();
                }

                if (Properties.Settings.Default.TS3Path == null || Properties.Settings.Default.TS3ModsPath == null) return false;
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (string s in Properties.Settings.Default.TS3Path)
            {
                if (!Directory.Exists(s)) return false;
            }
            return Directory.Exists(Properties.Settings.Default.TS3ModsPath);
        }
        private string[] GetUserFolderFiles(string folderName, string pattern)
        {
            if (currentGame == Game.SimsMedieval) return new string[0];
            try
            {
                List<string> roots = new List<string>();
                string saves = Properties.Settings.Default.TS3SavesPath;
                if (!String.IsNullOrWhiteSpace(saves))
                    roots.Add(Path.Combine(Path.GetDirectoryName(saves.TrimEnd('\\', '/')), folderName));
                string mods = Properties.Settings.Default.TS3ModsPath;
                if (!String.IsNullOrWhiteSpace(mods))
                    roots.Add(Path.Combine(Path.GetDirectoryName(mods.TrimEnd('\\', '/')), folderName));
                roots.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "Electronic Arts", "The Sims 3", folderName));
                foreach (string dir in roots)
                {
                    if (dir != null && Directory.Exists(dir))
                        return Directory.GetFiles(dir, pattern, SearchOption.AllDirectories);
                }
            }
            catch { }
            return new string[0];
        }

        private string[] GetDCCacheFiles() { return GetUserFolderFiles("DCCache", "*.dbc"); }
        private string[] GetSavedSimFiles() { return GetUserFolderFiles("SavedSims", "*.sim"); }

        private bool SetupGamePacks()
        {
            System.Collections.Specialized.StringCollection TSPathStrings = currentGame == Game.SimsMedieval ? 
                Properties.Settings.Default.TSMPath : Properties.Settings.Default.TS3Path;
            if (TSPathStrings == null) TSPathStrings = new System.Collections.Specialized.StringCollection();
            List<Package> gamePacks = new List<Package>();
            List<string> packPaths = new List<string>();
            List<bool> isBasegame = new List<bool>();
            foreach (string path in TSPathStrings)
            {
                try
                {
                    List<string> pathsPatch = new List<string>(Directory.GetFiles(path, "ContentPatch.package", SearchOption.AllDirectories));
                    List<string> pathsBuild = new List<string>(Directory.GetFiles(path, "*Build*.package", SearchOption.AllDirectories));
                    pathsBuild.Sort();
                    packPaths.AddRange(pathsPatch);
                    packPaths.AddRange(pathsBuild);
                }
                catch (DirectoryNotFoundException e)
                {
                    MessageBox.Show("Your game packages path '" + path + "' is invalid! Please go to File / Change Settings and correct it or make it blank to reset, then restart."
                        + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                    return false;
                }
                catch (IOException e)
                {
                    MessageBox.Show("Your game packages path '" + path + "' is invalid or a network error has occurred! Please go to File / Change Settings and correct it or make it blank to reset, then restart."
                        + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                    return false;
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show("Your game packages path '" + path + "' is not specified correctly! Please go to File / Change Settings and correct it or make it blank to reset, then restart."
                        + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                    return false;
                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show("You do not have the required permissions to access the game packages folder '" + path + "'! Please restart with admin privileges."
                        + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                    return false;
                }
            }

            if (packPaths.Count == 0)
            {
                MessageBox.Show("Can't find game packages! Please go to File / Change Settings and correct the game packages path or make it blank to reset, then restart.");
                return false;
            }
            try
            {
                string err = "";
                for (int i = 0; i < packPaths.Count; i++)
                {
                    try
                    {
                        Package p = OpenPackage(packPaths[i], false);
                        if (p == null)
                        {
                            err += packPaths[i] + Environment.NewLine;
                            packPaths[i] = null;
                            continue;
                        }
                        gamePacks.Add(p);
                        isBasegame.Add(packPaths[i].Contains("The Sims 3\\GameData"));
                        //MessageBox.Show(packPaths[i] + Environment.NewLine + isBasegame[isBasegame.Count - 1]);
                    }
                    catch
                    {
                        err += packPaths[i] + Environment.NewLine;
                        packPaths[i] = null;
                    }
                }
                if (err.Length > 0) MessageBox.Show("Unable to open the following game packages:" + Environment.NewLine + err);

                string[] localCC;
                //string[] localDL;
                string[] dccFiles = new string[] { };
                if (useDCCache_n_SS)
                {
                    List<string> extraFiles = new List<string>(GetDCCacheFiles());
                    extraFiles.AddRange(GetSavedSimFiles());
                    dccFiles = extraFiles.ToArray();
                }
                try
                {
                    string[] modFiles = Directory.GetFiles(currentGame == Game.SimsMedieval ? Properties.Settings.Default.TSMModsPath : Properties.Settings.Default.TS3ModsPath,
                        "*.package", SearchOption.AllDirectories);
                    localCC = new string[modFiles.Length + dccFiles.Length];
                    modFiles.CopyTo(localCC, 0);
                    dccFiles.CopyTo(localCC, modFiles.Length);
                }
                catch
                {
                    MessageBox.Show("Either the path to your user Sims Mods folder is incorrect or you have no custom content." +
                        Environment.NewLine + "If the path is incorrect, go to File / Change Settings and correct it or make it blank to reset, then restart.");
                    gamePackages = gamePacks.ToArray();
                    gamePackageNames = packPaths.ToArray();
                    isBasegamePackage = isBasegame.ToArray();
                    return true;
                }
                List<string> ccPaths = new List<string>(localCC);
                ccPaths.Sort();
                List<Package> ccPacks = new List<Package>();
                List<bool> isEA = new List<bool>();

                err = "";
                for (int i = 0; i < ccPaths.Count; i++)
                {
                    try
                    {
                        Package p = OpenPackage(ccPaths[i], false);
                        if (p == null)
                        {
                            err += ccPaths[i] + Environment.NewLine;
                            ccPaths[i] = null;
                            continue;
                        }
                        ccPacks.Add(p);
                        isEA.Add(false);
                    }
                    catch
                    {
                        err += ccPaths[i] + Environment.NewLine;
                        ccPaths[i] = null;
                        continue;
                    }
                }
                if (err.Length > 0) MessageBox.Show("Unable to open the following mod packages:" + Environment.NewLine + err);


                ccPacks.AddRange(gamePacks);
                ccPaths.AddRange(packPaths);
                isEA.AddRange(isBasegame);
                ccPaths.RemoveAll(item => item == null);
                gamePackages = ccPacks.ToArray();
                gamePackageNames = ccPaths.ToArray();
                isBasegamePackage = isEA.ToArray();
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("You do not have the required permissions to open the game packages! Please restart with admin privileges."
                    + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            return true;
        }

        private void SaveStream(IResourceIndexEntry irie, BinaryReader br, Package pack)
        {
            Stream s = new MemoryStream();
            br.BaseStream.CopyTo(s);
            s.Position = 0;
            pack.AddResource(irie, s, true);
        }
        private void SaveStream(TGI tgi, Stream r, Package pack)
        {
            Stream s = new MemoryStream();
            r.CopyTo(s);
            s.Position = 0;
            TGIBlock tgiblock = new TGIBlock(1, null, tgi.Type, tgi.Group, tgi.Instance);
            pack.AddResource(tgiblock, s, true);
        }
        private void SaveStream(IResourceIndexEntry irie, Stream r, Package pack)
        {
            Stream s = new MemoryStream();
            r.CopyTo(s);
            s.Position = 0;
            pack.AddResource(irie, s, true);
        }

        private BGEO FetchGameBGEO(TGI tgi, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        { 
                            BGEO bgeo = new BGEO(br);
                            return bgeo;
                        }
                        catch
                        {
                            errorMsg += "Can't read BGEO " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find BGEO " + tgi.ToString() + Environment.NewLine;
            return null;
        }

        private BOND FetchGameBOND(TGI tgi, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        {
                            BOND bond = new BOND(br);
                            return bond;
                        }
                        catch
                        {
                            errorMsg += "Can't read BOND " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find BoneDelta " + tgi.ToString() + Environment.NewLine;
            return null;
        }

        private CASP FetchGameCASP(TGI tgi, out string packageName, ClothingType partType, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) { packageName = ""; return null; }
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        {
                            CASP casp = new CASP(br);
                            packageName = gamePackageNames[i];
                            return casp;
                        }
                        catch
                        {
                            errorMsg += "Can't read " + partType.ToString() + " CASP " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            packageName = gamePackageNames[i];
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find " + partType.ToString() + " CASP " + tgi.ToString() + Environment.NewLine;
            packageName = "";
            return null;
        }
        private VPXY FetchGameVPXY(TGI tgi, out string packageName, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) { packageName = ""; return null; }
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        {
                            VPXY vpxy = new VPXY(br);
                            packageName = gamePackageNames[i];
                            return vpxy;
                        }
                        catch
                        {
                            errorMsg += "Can't read VPXY " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            packageName = gamePackageNames[i];
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find VPXY " + tgi.ToString() + Environment.NewLine;
            packageName = "";
            return null;
        }
        private BBLN FetchGameBlend(TGI tgi, out string packageName, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) { packageName = ""; return null; }
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        {
                            BBLN blend = new BBLN(br);
                            packageName = gamePackageNames[i];
                            return blend;
                        }
                        catch
                        {
                            errorMsg += "Can't read Blend " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            packageName = gamePackageNames[i];
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find Blend " + tgi.ToString() + Environment.NewLine;
            packageName = "";
            return null;
        }
        private GEOM FetchGameGEOM(TGI tgi, string packname, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        {
                            GEOM geom = new GEOM(br);
                           // geom.StandardizeFormat();
                            return geom;
                        }
                        catch
                        {
                            errorMsg += "Can't read GEOM " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find GEOM mesh " + tgi.ToString() + ", Linked from package: " + packname + Environment.NewLine;
            return null;
        }

        private DdsFile FetchGameDDS(TGI tgi, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (Stream s = p.GetResource(irie))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, s, TroubleshootPackage); }
                        try
                        {
                            DdsFile dds = new DdsFile();
                            s.Position = 0;
                            dds.Load(s, false);
                            return dds;
                        }
                        catch
                        {
                            errorMsg += "Can't read DDS " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find DDS texture " + tgi.ToString() + Environment.NewLine;
            return null;
        }
        private Bitmap FetchGameImageFromDDS(TGI tgi, ref string errorMsg)
        {
            return FetchGameImageFromDDS(tgi, ref errorMsg, null);
        }
        private Bitmap FetchGameImageFromDDS(TGI tgi, ref string errorMsg, bool standardSize)
        {
            return FetchGameImageFromDDS(tgi, ref errorMsg, standardSize ? new int[] { 1024, 1024 } : null);
        }
        private Bitmap FetchGameImageFromDDS(TGI tgi, ref string errorMsg, int width, int height)
        {
            return FetchGameImageFromDDS(tgi, ref errorMsg, new int[] { width, height });
        }
        private Bitmap FetchGameImageFromDDS(TGI tgi, ref string errorMsg, int[] dimensions)
        {
            if (tgi == null || tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (Stream s = p.GetResource(irie))
                    {
                        using (DdsFile dds = new DdsFile())
                        {
                            if (!isBasegamePackage[i]) { SaveStream(irie, s, TroubleshootPackage); }
                            try
                            {
                                s.Position = 0;
                                dds.Load(s, false);
                                //if (irie.Instance == 0x3c4db572c11bc077)
                                //{
                                //    int idummy = 0;
                                //}
                                if (dimensions != null)
                                {
                                    Bitmap texture = new Bitmap(StreamCopy(dds.Image), dimensions[0], dimensions[1]);
                                    return texture;
                                }
                                else
                                {
                                    Bitmap texture = StreamCopy(dds.Image);
                                    return texture;
                                }
                            }
                            catch
                            {
                                errorMsg += "Can't read DDS image " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                                return null;
                            }
                        }
                    }
                }
            }
            errorMsg += "Can't find DDS texture " + tgi.ToString() + Environment.NewLine;
            return null;
        }

        //overload for multiple packages, used for faster fetching images from CC packages
        private Bitmap FetchGameImageFromDDS(TGI tgi, ref string errorMsg, string[] packagenames, int[] dimensions=null)
        {
            Package[] gP;
            List<Package> gP0=new List<Package>();
            for (int i = 0;i<packagenames.Length;i++)
            {
                gP0.Add((Package)Package.OpenPackage(0,packagenames[i]));
            }
            gP = gP0.ToArray();

            if (tgi == null || tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gP.Length; i++)
            {
                Package p = gP[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (Stream s = p.GetResource(irie))
                    {
                        using (DdsFile dds = new DdsFile())
                        {
                            if (!isBasegamePackage[i]) { SaveStream(irie, s, TroubleshootPackage); }
                            try
                            {
                                s.Position = 0;
                                dds.Load(s, false);
                                //if (irie.Instance == 0x3c4db572c11bc077)
                                //{
                                //    int idummy = 0;
                                //}
                                if (dimensions != null)
                                {
                                    Bitmap texture = new Bitmap(StreamCopy(dds.Image), dimensions[0], dimensions[1]);
                                    return texture;
                                }
                                else
                                {
                                    Bitmap texture = StreamCopy(dds.Image);
                                    return texture;
                                }
                            }
                            catch
                            {
                                errorMsg += "Can't read DDS image " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                                return null;
                            }
                        }
                    }
                }
            }
            errorMsg += "Can't find DDS texture " + tgi.ToString() + Environment.NewLine;
            return null;
        }
        private Bitmap StreamCopy(Bitmap other)
        {
            Bitmap tmp;
            MemoryStream ms = new MemoryStream();
            other.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            tmp = (Bitmap)Image.FromStream(ms, false);
            
            return tmp;
        }
        private uint[] FetchImageArrayFromDDS(TGI tgi, ref string errorMsg, int width, int height)
        {
            return FetchImageArrayFromDDS(tgi, ref errorMsg, new int[] { width, height });
        }
        private uint[] FetchImageArrayFromDDS(TGI tgi, ref string errorMsg, int[] dimensions)
        {
            if (tgi == null || tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (Stream s = p.GetResource(irie))
                    {
                        using (DdsFile dds = new DdsFile())
                        {
                            if (!isBasegamePackage[i]) { SaveStream(irie, s, TroubleshootPackage); }
                            try
                            {
                                s.Position = 0;
                                dds.Load(s, false);
                                if (dimensions != null && (dimensions[0] != dds.Size.Width || dimensions[1] != dds.Size.Height))
                                {
                                    // DdsFile dds2 = dds.Resize(new Size(dimensions[0], dimensions[1]));
                                    // return dds2.Image.ToARGBData();
                                    return dds.Resize(new Size(dimensions[0], dimensions[1])).Image.ToARGBData();
                                }
                                else
                                {
                                    return dds.Image.ToARGBData();
                                }
                            }
                            catch
                            {
                                errorMsg += "Can't read DDS image " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                                return null;
                            }
                        }
                    }
                }
            }
            errorMsg += "Can't find DDS texture " + tgi.ToString() + Environment.NewLine;
            return null;
        }
        private TONE FetchGameTONE(TGI tgi, out string packageName, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) { packageName = ""; return null; }
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        {
                            TONE tone = new TONE(br);
                            packageName = gamePackageNames[i];
                            return tone;
                        }
                        catch
                        {
                            errorMsg += "Can't read TONE " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            packageName = gamePackageNames[i];
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find skin TONE " + tgi.ToString() + Environment.NewLine;
            packageName = "";
            return null;
        }
        private RIG FetchGameRig(TGI tgi, out string packageName, ref string errorMsg)
        {
            if (tgi == null || tgi.Instance == 0ul) { packageName = ""; return null; }
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.ResourceGroup == tgi.Group & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePackages.Length; i++)
            {
                Package p = gamePackages[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        if (!isBasegamePackage[i]) { SaveStream(irie, br, TroubleshootPackage); }
                        try
                        {
                            RIG rig = new RIG(br);
                            packageName = gamePackageNames[i];
                            return rig;
                        }
                        catch
                        {
                            errorMsg += "Can't read Rig " + tgi.ToString() + ", Package: " + gamePackageNames[i] + Environment.NewLine;
                            packageName = gamePackageNames[i];
                            return null;
                        }
                    }
                }
            }
            errorMsg += "Can't find Rig " + tgi.ToString() + Environment.NewLine;
            packageName = "";
            return null;
        }
    }
}
