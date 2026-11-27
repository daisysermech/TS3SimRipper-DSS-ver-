using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using s3pi.Package;
using s3pi.Interfaces;
using s3pi.ImageResource;
using Ookii.Dialogs;

namespace TS3SimRipper
{
    public partial class Form1 : Form
    {
        string version = "TS3 SimRipper v0.6.1";
        Game currentGame;
        static string GEOMfilter = "GEOM mesh files (*.simgeom, *.geom)|*.simgeom;*.geom|All files (*.*)|*.*";
        static string OBJfilter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
        static string MS3Dfilter = "Milkshape MS3D files (*.ms3d)|*.ms3d|All files (*.*)|*.*";
        static string DAEfilter = "Collada .dae files (*.dae)|*.dae|All files (*.*)|*.*";
        static string Imagefilter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
        SimDescription[] simsArray;

        string[] physiqueNamesHuman;
        string[] physiqueNamesAnimal;
        TONE currentTONE;

        bool isPregnant;
        float pregnancyProgress;

        string startupErrors = "";
        string simDesc = "";
        string errorList = "";
        string presetList = "";

        List<Package> currentSavePacks;
        List<string> currentSavePackNames;
        bool useDCCache_n_SS;

        public Form1()
        {
            InitializeComponent();
            useDCCache_n_SS = Properties.Settings.Default.useDCCache_n_SS;
            currentGame = (Game)Properties.Settings.Default.GameMode;
            GameMode_comboBox.SelectedIndexChanged -= GameMode_comboBox_SelectedIndexChanged;
            GameMode_comboBox.SelectedIndex = Properties.Settings.Default.GameMode;
            GameMode_comboBox.SelectedIndexChanged += GameMode_comboBox_SelectedIndexChanged;
            this.Text = version;
            physiqueNamesHuman = new string[] { "Body_Heavy", "Body_Fit", "Body_Lean", "Body_Bony", "Body_Pregnant", "Hips_Wide", "Hips_Narrow", "Waist_Wide", "Waist_Narrow" };
            physiqueNamesAnimal = new string[] { "headModifier_Body_Fat", "headModifier_Body_Fit", "headModifier_Body_Skinny", "headModifier_Body_Bony",
                "headModifier_Body_Pregnant", "headModifier_Hips_Wide", "headModifier_Hips_Narrow", "headModifier_Waist_Wide", "headModifier_Waist_Narrow" };

            while (!DetectFilePaths())
            {
                DialogResult res = MessageBox.Show("Can't find game and/or user files. Do you want to set them manually?", "Files not found", MessageBoxButtons.RetryCancel);
                if (res == DialogResult.Cancel) break;
                Form f = new PathsPrompt(currentGame);
                f.ShowDialog();
            }

            if (SetupGamePacks())
            {
                startupErrors = "";
            }

            for (int i = 0; i < SimFilter_checkedListBox.Items.Count; i++)
            {
                SimFilter_checkedListBox.SetItemChecked(i, true);
            }
            SimsSort_comboBox.SelectedIndex = 0;
        }

        private void SaveGameFolder_button_Click(object sender, EventArgs e)
        {
            string savesFolder = null;
            if (Properties.Settings.Default.LastSaveGamePath != null && Properties.Settings.Default.LastSaveGamePath.Length > 0)
            {
                savesFolder = Properties.Settings.Default.LastSaveGamePath;
                savesFolder = savesFolder.Replace(@"\\", @"\");
            }
            else if (Properties.Settings.Default.TS3SavesPath != null && Properties.Settings.Default.TS3SavesPath.Length > 0)
            {
                savesFolder = Properties.Settings.Default.TS3SavesPath;
                savesFolder = savesFolder.Replace(@"\\", @"\");
            }
            VistaFolderBrowserDialog folder = new VistaFolderBrowserDialog();
            folder.ShowNewFolderButton = false;
            folder.Description = "Select saved game folder or saved sims folder";
            folder.UseDescriptionForTitle = true;
            folder.SelectedPath = savesFolder;
            folder.ShowDialog();
            SaveGameFolder.Text = folder.SelectedPath;
            if (SaveGameFolder.Text.Length == 0) return;
            Properties.Settings.Default.LastSaveGamePath = folder.SelectedPath;
            Properties.Settings.Default.Save();

            List<string> savePacks = new List<string>(Directory.GetFiles(folder.SelectedPath, "*.package"));
            savePacks.AddRange(Directory.GetFiles(folder.SelectedPath, "*.sim"));
            Package[] packs = new Package[savePacks.Count];
            List<SimDescription> simExports = new List<SimDescription>();
            for (int i = 0; i < savePacks.Count; i++)
            {
                packs[i] = (Package)Package.OpenPackage(0, savePacks[i], false);
                Predicate<IResourceIndexEntry> idel = r => r.ResourceType == (uint)ResourceTypes.SimExport;
                List<IResourceIndexEntry> iries = packs[i].FindAll(idel);
                foreach (IResourceIndexEntry irie in iries)
                {
                    Stream s = packs[i].GetResource(irie);
                    BinaryReader br = new BinaryReader(s);
                    SIME sim = new SIME(br);
                    SimDescription simdesc = new SimDescription(sim.SimFirstName, sim.SimLastName, packs[i], savePacks[i], sim.Links);
                    simExports.Add(simdesc);
                }
            }
            for (int i = 0; i < simExports.Count; i++)
            {
                List<SIMO> simoList = new List<SIMO>();
                List<Package> packList = new List<Package>();
                List<string> nameList = new List<string>();
                foreach (TGI tgi in simExports[i].simLinks)
                {
                    if (tgi.Type == (uint)ResourceTypes.SimOutfit)
                    {
                        for (int p = 0; p < savePacks.Count; p++)
                        {
                            Predicate<IResourceIndexEntry> idel = r => r.ResourceType == tgi.Type && r.ResourceGroup == tgi.Group && r.Instance == tgi.Instance;
                            IResourceIndexEntry irie = packs[p].Find(idel);
                            if (irie != null)
                            {
                                Stream s = packs[p].GetResource(irie);
                                BinaryReader br = new BinaryReader(s);
                                SIMO simo = new SIMO(br);
                                simoList.Add(simo);
                                if (!packList.Contains(packs[p])) { packList.Add(packs[p]); nameList.Add(savePacks[p]); }
                            }
                        }
                    }
                }
                simExports[i].AddOutfits(simoList, packList, nameList);
            }
            simsArray = simExports.ToArray();

            ListSims();
        }

        public void ListSims()
        {
            if (simsArray == null) return;
            List<SimDescription> simsList = new List<SimDescription>();
            for (int i = 0; i < simsArray.Length; i++)
            {
                if (((simsArray[i].species <= Species.Human && SimFilter_checkedListBox.GetItemChecked(0)) ||
                     (simsArray[i].species > Species.Human && SimFilter_checkedListBox.GetItemChecked(1))) &
                    ((simsArray[i].gender == AgeGender.Male && SimFilter_checkedListBox.GetItemChecked(2)) ||
                     (simsArray[i].gender == AgeGender.Female && SimFilter_checkedListBox.GetItemChecked(3))) &
                    ((simsArray[i].age == AgeGender.Toddler && SimFilter_checkedListBox.GetItemChecked(4)) ||
                     (simsArray[i].age == AgeGender.Child && SimFilter_checkedListBox.GetItemChecked(5)) ||
                     (simsArray[i].age == AgeGender.Teen && SimFilter_checkedListBox.GetItemChecked(6)) ||
                     (simsArray[i].age == AgeGender.YoungAdult && SimFilter_checkedListBox.GetItemChecked(7)) ||
                     (simsArray[i].age == AgeGender.Adult && SimFilter_checkedListBox.GetItemChecked(8)) ||
                     (simsArray[i].age == AgeGender.Elder && SimFilter_checkedListBox.GetItemChecked(9))))
                    simsList.Add(new SimDescription(simsArray[i]));
            }
            if (SimsSort_comboBox.SelectedIndex == 0) simsList.Sort((x, y) => x.sortNameLast.CompareTo(y.sortNameLast));
            else simsList.Sort((x, y) => x.sortNameFirst.CompareTo(y.sortNameFirst));
            sims_listBox.Items.Clear();
            for (int i = 0; i < simsList.Count; i++)
            {
                sims_listBox.Items.Add(simsList[i]);
            }
        }

        internal string GetFilename(string title, string filter, string defaultFolder)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = filter;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Title = title;
            if (defaultFolder != null && defaultFolder.Length > 0 && Directory.Exists(defaultFolder)) openFileDialog1.InitialDirectory = defaultFolder;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal class OutfitInfo
        {
            internal CASP[] casps;
            internal string[] packages;
            internal float[] physique;
            internal TONE skintone;
            internal float skinColorSlider;
            internal float cutnessSlider;
            internal float cleavageSlider;
            internal string blendNames;
            internal BGEO[] bgeos;
            internal BOND[] bonds;
            internal OutfitInfo(CASP[] casps, string[] packageNames, float[] physique, TONE skintone, float skinColor, float cutness,
                float cleavage, string blendNames, BGEO[] bgeoArray, BOND[] bondArray)
            {
                this.casps = casps;
                this.packages = packageNames;
                this.physique = physique;
                this.skintone = skintone;
                this.skinColorSlider = skinColor;
                this.cutnessSlider = cutness;
                this.cleavageSlider = cleavage;
                this.blendNames = blendNames;
                this.bgeos = bgeoArray;
                this.bonds = bondArray;
            }
        }

        internal class SimDescription
        {
            internal string simFirstName;
            internal string simLastName;
            internal List<Package> savePacks;
            internal List<string> savePackNames;
            internal TGI[] simLinks;
            internal SIMO[] simOutfits;
            internal Species species;
            internal AgeGender age;
            internal AgeGender gender;
            internal SimDescription(string firstName, string lastName, Package pack, string packName, TGI[] links)
            {
                this.simFirstName = firstName;
                this.simLastName = lastName;
                this.savePacks = new List<Package>();
                this.savePacks.Add(pack);
                this.savePackNames = new List<string>();
                this.savePackNames.Add(packName);
                this.simLinks = links;
            }
            internal SimDescription(SimDescription other)
            {
                this.simFirstName = other.simFirstName;
                this.simLastName = other.simLastName;
                this.savePacks = other.savePacks;
                this.savePackNames = other.savePackNames;
                this.simLinks = other.simLinks;
                this.simOutfits = other.simOutfits;
                this.species = other.species;
                this.age = other.age;
                this.gender = other.gender;
            }
            internal void AddOutfits(List<SIMO> outfits, List<Package> packs, List<string> packNames)
            {
                this.simOutfits = outfits.ToArray();
                if (this.simOutfits[0] != null)
                {
                    this.species = this.simOutfits[0].SimSpecies == Species.MedievalHuman ? Species.Human : this.simOutfits[0].SimSpecies;
                    this.age = this.simOutfits[0].SimAge;
                    this.gender = this.simOutfits[0].SimGender;
                }
                for (int i = 0; i < packs.Count; i++)
                {
                    if (!this.savePacks.Contains(packs[i])) { this.savePacks.Add(packs[i]); this.savePackNames.Add(packNames[i]); }
                }
            }
            public string sortNameLast
            {
                get { return simLastName + simFirstName; }
            }
            public string sortNameFirst
            {
                get { return simFirstName + simLastName; }
            }
            public override string ToString()
            {
                return simFirstName + " " + simLastName;
            }
        }
        SimDescription sim;
        private void sims_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sim = (SimDescription)sims_listBox.SelectedItem;
            currentName = sim.simFirstName + " " + sim.simLastName;
            currentSpecies = sim.species;
            if (sim.species == 0) currentSpecies = Species.Human;
            currentAge = sim.age;
            currentGender = sim.gender;
            //currentSize = currentSpecies == Species.Human ? humanTextureSize : petTextureSize;
            errorList = "";

            Pregnancy_trackBar.Value = 0;
            Pregnancy_trackBar.Enabled = false;

            currentSavePacks = sim.savePacks;

            int outfitcount = sim.simOutfits.Length;
            Outfits_comboBox.Items.Clear();
            for (int o = 0; o < outfitcount; o++)
                Outfits_comboBox.Items.Add("Outfit " + (o + 1).ToString());
            Outfits_comboBox.Enabled = outfitcount > 1;
        }

        private void Outfits_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sim == null || Outfits_comboBox.SelectedIndex < 0) return;
            DisplaySim(sim, Outfits_comboBox.SelectedIndex);
        }
        private void DisplaySim(SimDescription sim, int OutfixIndex, bool resetView = true)
        {
            bool debug = false;
            Working_label.Visible = true;
            Working_label.Refresh();
            morphPreview1.Stop_Mesh();

            if (debug) errorList += "DisplaySim start" + Environment.NewLine;

            simDesc = "Name: " + currentName + Environment.NewLine + "Species: " + currentSpecies.ToString() + Environment.NewLine +
                "Age: " + currentAge.ToString() + Environment.NewLine + "Gender: " + currentGender.ToString() + Environment.NewLine + Environment.NewLine;
            SIMO[] outfits = sim.simOutfits;

            currentOutfits = new List<OutfitInfo>();
            List<TGI> caspsNotFound = new List<TGI>();
            //for (int o = 0; o < outfits.Length; o++)
            SIMO outfit = outfits[OutfixIndex];
            List<CASP> casps = new List<CASP>();
            List<string> packNames = new List<string>();
            SIMO.CASPEntry[] caspEntries = outfit.CASPEntries;
            for (int i = 0; i < caspEntries.Length; i++)
            {
                TGI tgi = outfit.TGILinks[caspEntries[i].CaspLinkIndex];
                if (caspsNotFound.Contains(tgi)) continue;
                string packname;
                CASP casp = FetchGameCASP(tgi, out packname, caspEntries[i].TypeOfClothing, ref errorList);
                if (casp == null) { caspsNotFound.Add(tgi); continue; }
                casp.outfitPreset = new CASP.OutfitPreset(outfit.PresetXMLs[i].XML);
                casps.Add(casp);
                packNames.Add(packname);
            }
            string packname2;
            TONE skin = FetchGameTONE(outfit.TGILinks[outfit.skinToneIndex], out packname2, ref errorList);
            SIMO.BlendEntry[] blends = outfit.BlendEntries;
            List<BGEO> bgeoList = new List<BGEO>();
            List<BOND> bondList = new List<BOND>();
            string blendDesc = "";
            //  List<BOND> bigMorphs = new List<BOND>();
            foreach (SIMO.BlendEntry blend in blends)
            {
                TGI tgi = outfit.TGILinks[blend.BlendIndex];
                if (tgi.Type != (uint)ResourceTypes.FaceBlend) continue;
                string packname;
                BBLN bbln = FetchGameBlend(tgi, out packname, ref errorList);
                if (bbln == null) continue;
                //bool isTeen = (string.Compare(bbln.BlendName, "bodyTeen") == 0);
                //if (!isTeen) 
                //if (bbln.BlendName.IndexOf("HorseSizeSlider") < 0) continue;
                blendDesc += bbln.BlendName + " :";
                BGEO bgeo = null;
                if (bbln.BGEOlink != null)
                {
                    TGI bgeoTGI = bbln.BGEOlink;
                    bgeo = FetchGameBGEO(bgeoTGI, ref errorList);
                }
                bool gotBGEO = false;
                bool gotBOND = false;
                foreach (BBLN.Entry entry in bbln.Entries)
                {
                    foreach (BBLN.MorphEntry geom in entry.geomMorphs)
                    {
                        if ((!gotBGEO) &&
                            (((uint)currentSpecies & geom.ageGenderSpecies) > 0 || (currentSpecies == Species.Human) & (geom.ageGenderSpecies & 0x0F00) == 0) &&
                            (((uint)currentAge & geom.ageGenderSpecies) > 0) && (((uint)currentGender & geom.ageGenderSpecies) > 0))
                        {
                            if (bgeo != null)
                            {
                                bgeo.weight = blend.Weight * geom.amount;
                                // if (bgeo.weight < 0) bgeo.weight = 0;
                                bgeo.name = bbln.BlendName;
                                bgeoList.Add(bgeo);
                                blendDesc += " BGEO:" + bgeo.weight.ToString();
                            }
                            gotBGEO = true;
                            break;
                        }
                    }
                    foreach (BBLN.MorphEntry bone in entry.boneMorphs)
                    {
                        if ((!gotBOND) &&
                            (((uint)currentSpecies & bone.ageGenderSpecies) > 0 || (currentSpecies == Species.Human) & (bone.ageGenderSpecies & 0x0F00) == 0) &&
                            (((uint)currentAge & bone.ageGenderSpecies) > 0) && (((uint)currentGender & bone.ageGenderSpecies) > 0))
                        {
                            TGI vpxyTGI = bbln.TGIList[bone.tgiIndex];
                            string packagename;
                            VPXY vpxy = FetchGameVPXY(vpxyTGI, out packagename, ref errorList);
                            if (vpxy == null) continue;
                            foreach (TGI t in vpxy.AllLinks)
                            {
                                BOND bond = FetchGameBOND(t, ref errorList);
                                if (bond != null)
                                {
                                    bond.weight = blend.Weight * bone.amount;
                                    // if (bond.weight < 0) bond.weight = 0;
                                    bond.name = bbln.BlendName;
                                    //if (bond.IsSizeMorph())
                                    //{
                                    //    bigMorphs.Add(bond);
                                    //}
                                    //else
                                    //{
                                    bondList.Add(bond);
                                    //}
                                    blendDesc += " BOND:" + bond.weight.ToString() + " ";
                                }
                            }
                            gotBOND = true;
                            break;
                        }
                    }
                }
                blendDesc += Environment.NewLine;
            }
            currentOutfits.Add(new OutfitInfo(casps.ToArray(), packNames.ToArray(), outfit.SimPhysique, skin, outfit.skinColorSlider,
                outfit.definitionSlider, outfit.breastSlider, blendDesc, bgeoList.ToArray(), bondList.ToArray()));
            //Outfits_comboBox.Items.Add("Outfit " + (o + 1).ToString());


            /*
            Outfits_comboBox.SelectedIndexChanged -= Outfits_comboBox_SelectedIndexChanged;
            outfitIndex = 0;
            Outfits_comboBox.SelectedIndex = outfitIndex;
            Outfits_comboBox.SelectedIndexChanged += Outfits_comboBox_SelectedIndexChanged;
            Outfits_comboBox.Enabled = outfits.Length > 1
            */

            if (debug) errorList += "DisplaySim loaded outfits" + Environment.NewLine;

            string morphInfo = "";


            morphPreview1.Stop_Mesh();
            GetCurrentModel();
            morphPreview1.Start_Mesh(GetModels(), GetTextures(), GetSpeculars(), true);
            Working_label.Visible = false;
        }

        internal GEOM[] GetModels()
        {
            List<GEOM> models = new List<GEOM>();
            models.Add(currentBodyModel);
            models.Add(currentFaceModel);
            models.Add(currentScalpModel);
            models.Add(currentGlassModel);
            models.Add(currentHairModel);
            models.AddRange(currentAccessories);
            return models.ToArray();
        }

        internal Image[] GetTextures()
        {
            List<Image> textures = new List<Image>();
            textures.Add(currentBodyTexture);
            textures.Add(currentFaceTexture);
            textures.Add(currentScalpTexture);
            textures.Add(currentGlassTexture);
            textures.Add(currentHairTexture);
            textures.AddRange(currentAccessoryTexture);
            return textures.ToArray();
        }

        internal Image[] GetSpeculars()
        {
            List<Image> specs = new List<Image>();
            specs.Add(currentBodySpecular);
            specs.Add(currentFaceSpecular);
            specs.Add(currentScalpSpecular);
            specs.Add(currentGlassSpecular);
            specs.Add(currentHairSpecular);
            specs.AddRange(currentAccessorySpecular);
            return specs.ToArray();
        }

        internal Package OpenPackage(string packagePath, bool readwrite)
        {
            try
            {
                Package package = (Package)Package.OpenPackage(0, packagePath, readwrite);
                return package;
            }
            catch
            {
                MessageBox.Show("Unable to read valid package data from " + packagePath);
                return null;
            }
        }

        internal static string WriteGEOM(string title, GEOM geom, string defaultFilename)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = GEOMfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "simgeom";
            saveFileDialog1.OverwritePrompt = true;
            if (defaultFilename != null && String.Compare(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            BinaryWriter bw = new BinaryWriter(myStream);
                            geom.WriteFile(bw);
                        }
                        myStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    myStream.Close();
                }
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal static string WriteMS3D(string title, MS3D ms3d, string defaultFilename)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = MS3Dfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            if (defaultFilename != null && String.Compare(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            saveFileDialog1.DefaultExt = "ms3d";
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            BinaryWriter bw = new BinaryWriter(myStream);
                            ms3d.Write(bw);
                        }
                        myStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    myStream.Close();
                }
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal static string WriteOBJFile(string title, OBJ myOBJ, string defaultFilename)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = OBJfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "obj";
            saveFileDialog1.OverwritePrompt = true;
            if (defaultFilename != null && String.Compare(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            StreamWriter sw = new StreamWriter(myStream);
                            myOBJ.Write(sw);
                        }
                        myStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    myStream.Close();
                }
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal string WriteDAEFile(string title, DAE dae, bool flipYZ, string defaultFilename)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = DAEfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "dae";
            saveFileDialog1.OverwritePrompt = true;
            if (defaultFilename != null && String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                float boneDivider = ((10f - (float)BoneSize_numericUpDown.Value) / 4f) * 100f;
                dae.Write(saveFileDialog1.FileName, flipYZ, boneDivider);
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal string WriteImage(string title, Image image, string defaultFilename)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = Imagefilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "png";
            saveFileDialog1.OverwritePrompt = true;
            if (defaultFilename != null && String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image.Save(saveFileDialog1.FileName);
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        public RIG GetTS3Rig(Species species, AgeGender age, ref string info)
        {
            String rigName = GetRigPrefix(species, age, AgeGender.Unisex) + "Rig";
            TGI rigTGI = new TGI((uint)ResourceTypes.Rig, 0u, FNVhash.FNV64(rigName));
            string packname;
            RIG rig = FetchGameRig(rigTGI, out packname, ref errorList);
            info += "Rig: " + rigName + " (" + rigTGI.ToString() + " Package: " + packname + Environment.NewLine;
            return rig;
        }

        public static double[] ArrayToDouble(float[] array)
        {
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }
            return result;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(version + Environment.NewLine + "by cmar" + Environment.NewLine + "Available free from ModTheSims.info" +
                Environment.NewLine + "Fork and fixes by Daisy Sermech" + Environment.NewLine + "Available at GitHub");
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new PathsPrompt(currentGame);
            f.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveOBJ_button_Click(object sender, EventArgs e)
        {
            SaveModelMorph(MeshFormat.OBJ);
        }

        private void SaveDAE_button_Click(object sender, EventArgs e)
        {
            SaveModelMorph(MeshFormat.DAE);
        }

        private void SaveMS3D_button_Click(object sender, EventArgs e)
        {
            SaveModelMorph(MeshFormat.MS3D);
        }

        private void SaveGEOM_button_Click(object sender, EventArgs e)
        {
            SaveModelMorph(MeshFormat.GEOM);
        }

        private void SaveTextures_button_Click(object sender, EventArgs e)
        {
            SaveModelMorph(MeshFormat.None);
        }

        private void SimFilter_checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SimFilter_checkedListBox.ItemCheck -= SimFilter_checkedListBox_ItemCheck;
            SimFilter_checkedListBox.SetItemCheckState(e.Index, e.NewValue);
            SimFilter_checkedListBox.ItemCheck += SimFilter_checkedListBox_ItemCheck;

            ListSims();
        }

        private void SimInfo_Click(object sender, EventArgs e)
        {
            ListingForm fulllist = new ListingForm((string)SimInfo_button.Tag, "Sim Information");
            fulllist.Show();
        }

        private void SimError_Click(object sender, EventArgs e)
        {
            ListingForm fulllist = new ListingForm(startupErrors + Environment.NewLine + errorList, "Error List");
            fulllist.Show();
        }

        private void ShowPreset_button_Click(object sender, EventArgs e)
        {
            ListingForm fulllist = new ListingForm(presetList, "Outfit Presets");
            fulllist.Show();
        }

        private void Occults_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TS4SaveGame.SimData sim = ((SimListing)sims_listBox.SelectedItem).sim;
            //DisplaySim(sim, (SimOccult)Occults_comboBox.SelectedItem);
        }

        private void Pregnancy_trackBar_Scroll(object sender, EventArgs e)
        {
            if (BaseModel == null) return;
            pregnancyProgress = Pregnancy_trackBar.Value / 10f;
            //   CurrentModel = LoadDMapMorph(BaseModel, pregnantModifier[0], pregnantModifier[1]);
            //   GlassModel = LoadDMapMorph(BaseGlass, pregnantModifier[0], pregnantModifier[1]);
            morphPreview1.Start_Mesh(GetModels(), GetTextures(), GetSpeculars(), true);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (startupErrors != null && startupErrors.Length > 0) MessageBox.Show(startupErrors);
        }

        private void GameMode_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentGame = (Game)GameMode_comboBox.SelectedIndex;
            Properties.Settings.Default.GameMode = GameMode_comboBox.SelectedIndex;
            while (!DetectFilePaths())
            {
                DialogResult res = MessageBox.Show("Can't find game and/or user files. Do you want to set them manually?", "Files not found", MessageBoxButtons.RetryCancel);
                if (res == DialogResult.Cancel) break;
                Form f = new PathsPrompt(currentGame);
                f.ShowDialog();
            }
            if (SetupGamePacks())
            {
                startupErrors = "";
            }
        }

        private void SimsSort_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListSims();
        }
    }
}