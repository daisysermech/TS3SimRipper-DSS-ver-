using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace TS3SimRipper
{
    public class CASP       //Sims 3 CASP resource
    {
        uint version = 18;
        Preset[] presets;
        string partname = "";
        float sortPriority;
        byte unknown2;
        ClothingType clothing;
        PartTypeFlags dataType;
        AgeGender ageGender;
        ClothingCategoryFlags clothingCategory;
        byte casPart1Index;
        byte casPart2Index;
        byte blendInfoFatIndex;
        byte blendInfoFitIndex;
        byte blendInfoThinIndex;
        byte blendInfoSpecialIndex;
        byte blendInfoTSMElderIndex;
        byte blendInfoTSMTeenIndex;
        uint overlayPriority;
        byte[] vpxyIndexes;
        LODInfoEntry[] lodInfo;
        byte[] diffuse1Indexes;
        byte[] specular1Indexes;
        byte[] diffuse2Indexes;
        byte[] specular2Indexes;
        byte[] bondIndexes;
        string unknown4 = "";
        string outfit_material = "";
        ulong medieval_profession;
        byte[] medieval_unknown;
        TGI[] tgiList;

        public OutfitPreset outfitPreset;

        public string PartName { get { return this.partname; } }

        public ClothingType TypeOfClothing { get { return this.clothing; } }

        public ClothingCategoryFlags OutfitType { get { return this.clothingCategory; } }

        public AgeGender SimAgeGender { get { return this.ageGender; } }

        public PartTypeFlags PartType { get { return this.dataType; } }

        public TGI[] VPXYLinks
        {
            get
            {
                TGI[] tmp = new TGI[this.vpxyIndexes.Length];
                for (int i = 0; i < this.vpxyIndexes.Length; i++) tmp[i] = new TGI(this.tgiList[this.vpxyIndexes[i]]);
                return tmp;
            }
        }

        public TGI[] LinkList
        {
            get
            {
                TGI[] tmp = new TGI[this.tgiList.Length];
                for (int i = 0; i < this.tgiList.Length; i++) tmp[i] = new TGI(this.tgiList[i]);
                return tmp;
            }
        }

        public TGI DiffuseLink { get { return this.tgiList[this.diffuse1Indexes[0]]; } }
        public TGI SpecularLink { get { return this.tgiList[this.specular1Indexes[0]]; } }

        /// <summary>
        /// Returns a TGI array of links to the fat, fit, thin, and special blends, in that order.
        /// </summary>
        public TGI[] BlendLinks
        {
            get
            {
                return new TGI[] { new TGI(this.tgiList[this.blendInfoFatIndex]), new TGI(this.tgiList[this.blendInfoFitIndex]),
                                   new TGI(this.tgiList[this.blendInfoThinIndex]), new TGI(this.tgiList[this.blendInfoSpecialIndex]) };
            }
        }

        public CASP(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            version = br.ReadUInt32();
            long tgiPosn = br.ReadInt32() + 8;
            uint presetCount = br.ReadUInt32();
            presets = new Preset[presetCount];
            for (int i = 0; i < presetCount; i++) presets[i] = new Preset(br);
            BinaryReader bEnc = new BinaryReader(br.BaseStream, Encoding.BigEndianUnicode);
            partname = bEnc.ReadString();
            sortPriority = br.ReadSingle();
            unknown2 = br.ReadByte();
            clothing = (ClothingType)br.ReadUInt32();
            dataType = (PartTypeFlags)br.ReadUInt32();
            ageGender = (AgeGender)br.ReadUInt32();
            clothingCategory = (ClothingCategoryFlags)br.ReadUInt32();
            casPart1Index = br.ReadByte();
            casPart2Index = br.ReadByte();
            blendInfoFatIndex = br.ReadByte();
            blendInfoFitIndex = br.ReadByte();
            blendInfoThinIndex = br.ReadByte();
            blendInfoSpecialIndex = br.ReadByte();
            if (this.version >= 22U)
            {
                this.blendInfoTSMElderIndex = br.ReadByte();
                this.blendInfoTSMTeenIndex = br.ReadByte();
            }
            overlayPriority = br.ReadUInt32();
            byte vpxyCount = br.ReadByte();
            vpxyIndexes = new byte[vpxyCount];
            for (int i = 0; i < vpxyCount; i++) vpxyIndexes[i] = br.ReadByte();
            byte lodCount = br.ReadByte();
            lodInfo = new LODInfoEntry[lodCount];
            for (int i = 0; i < lodCount; i++) lodInfo[i] = new LODInfoEntry(br);
            byte diffuse1Count = br.ReadByte();
            diffuse1Indexes = new byte[diffuse1Count];
            for (int i = 0; i < diffuse1Count; i++) diffuse1Indexes[i] = br.ReadByte();
            byte specular1Count = br.ReadByte();
            specular1Indexes = new byte[specular1Count];
            for (int i = 0; i < specular1Count; i++) specular1Indexes[i] = br.ReadByte();
            byte diffuse2Count = br.ReadByte();
            diffuse2Indexes = new byte[diffuse2Count];
            for (int i = 0; i < diffuse2Count; i++) diffuse2Indexes[i] = br.ReadByte();
            byte specular2Count = br.ReadByte();
            specular2Indexes = new byte[specular2Count];
            for (int i = 0; i < specular2Count; i++) specular2Indexes[i] = br.ReadByte();
            byte bondCount = br.ReadByte();
            bondIndexes = new byte[bondCount];
            for (int i = 0; i < bondCount; i++) bondIndexes[i] = br.ReadByte();
            unknown4 = bEnc.ReadString();
            if (this.version >= 22U)
            {
                this.outfit_material = bEnc.ReadString();
                this.medieval_profession = br.ReadUInt64();
                if (this.version >= 23U) this.medieval_unknown = br.ReadBytes(4);
            }

            byte count = br.ReadByte();
            tgiList = new TGI[count];
            for (int i = 0; i < count; i++) tgiList[i] = new TGI(br, TGI.TGIsequence.IGT);
        }

        public class Preset
        {
            string xml;
            uint unknown1;

            public Preset(BinaryReader br)
            {
                xml = System.Text.Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2));
                unknown1 = br.ReadUInt32();
            }
        }

        public class LODInfoEntry
        {
            byte level;
            PartTypeFlags destTexture;
            LODAsset[] lodAssets;

            public LODInfoEntry(BinaryReader br)
            {
                level = br.ReadByte();
                destTexture = (PartTypeFlags)br.ReadUInt32();
                byte assetCount = br.ReadByte();
                lodAssets = new LODAsset[assetCount];
                for (int i = 0; i < assetCount; i++) lodAssets[i] = new LODAsset(br);
            }
        }

        public class LODAsset
        {
            uint sorting;
            uint specLevel;
            uint castShadow;

            public LODAsset(BinaryReader br)
            {
                sorting = br.ReadUInt32();
                specLevel = br.ReadUInt32();
                castShadow = br.ReadUInt32();
            }
        }

        public class OutfitPreset
        {
            string xml;

            public OutfitPreset(string s)
            {
                xml = s;
            }

            public string XML { get { return this.xml; } }

            public PartTypeFlags? GetPartType()
            {
                if (xml == null || xml.Length == 0) return null;
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string str = reader.GetAttribute("key");
                            if (str != null && String.Compare(str, "partType") == 0)
                            {
                                string tmp = reader.GetAttribute("value");
                                return (PartTypeFlags)Enum.Parse(typeof(PartTypeFlags), tmp);
                            }
                        }
                    }
                }
                return null;
            }

            public Complate? GetComplate()
            {
                if (xml == null || xml.Length == 0) return null;
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (String.Compare(reader.Name, "complate") == 0)
                            {
                                string str = reader.GetAttribute("name");
                                if (str != null)
                                {
                                    try
                                    {
                                        Complate c = (Complate)Enum.Parse(typeof(Complate), str);
                                        return c;
                                    }
                                    catch
                                    {
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
                return null;
            }
            //REVERT THE CHANGES AND MAKE NEW METHOD

            public TGI GetLink(string valueName)
            {
                if (xml == null || xml.Length == 0)
                {
                    return null;
                }
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string str = reader.GetAttribute("key");
                            if (str != null && String.Compare(str, valueName) == 0)
                            {
                                string link = reader.GetAttribute("value");
                                link = link.Replace("key:", "");
                                TGI tgi = new TGI(link);
                                return tgi;
                            }
                        }
                    }
                }
                return null;
            }
            public bool PatternEnabled(string pattern)
            {
                bool enabled = false;
                if (xml == null || xml.Length == 0)
                {
                    return false;
                }
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return false;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string str = reader.GetAttribute("key");
                            if (str != null && String.Compare(str, pattern + " Enabled") == 0)
                            {
                                enabled = reader.GetAttribute("value").Contains("true");
                            }
                            else if (str != null && String.Compare(str, pattern) == 0)
                            {
                                string tmp = reader.GetAttribute("value");
                            }
                        }
                    }
                }
                return enabled;
            }

            public bool IfKeyTrue(string key)
            {
                bool enabled = false;
                if (xml == null || xml.Length == 0)
                {
                    return false;
                }
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return false;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string str = reader.GetAttribute("key");
                            if (str != null && String.Compare(str, key) == 0)
                            {
                                enabled = reader.GetAttribute("value").Contains("true");
                            }
                            else if (str != null && String.Compare(str, key) == 0)
                            {
                                string tmp = reader.GetAttribute("value");
                            }
                        }
                    }
                }
                return enabled;
            }
            private TGI ResolveAssetPathToTGI(string assetPath)
            {
                string name; ulong instance = 0;
                if (assetPath.Count(c => c == ':') == 2)
                    instance = Convert.ToUInt64(assetPath.Split(':').Last(),16);
                // "($assetRoot)\InGame\Complates\Materials\Geometric\CirclesEP8Sml_2.tga"
                else
                {
                    name = assetPath.Split('\\').Last();
                    instance = FNVhash.FNV64(name);
                }  // use the existing (uint,uint,ulong) ctor — skip TGI(string) entirely

                return new TGI(0x00B2D882, 0x00000000, instance);
            }
            public Form1.PatternInfo GetPatternInfo(string pattern) //TODO fix it so background and etc file textures are calculated fnvs from just names
            { //aka crop the path only to name without extention, get fnv64 and replace the info with that
                if (xml == null || xml.Length == 0)
                {
                    return null;
                }
                string patternName = "";
                TGI rgb = null, background = null;
                TGI[] channels = null;
                bool[] channelEnabled = new bool[] { false, false, false };
                float[] color = null;
                float[][] colors = null;
                float[] hsvBaseBg = null;
                float[][] hsvBase = null;
                float[] hsvShiftBg = null;
                float[][] hsvShift = null;
                float[] hsvBg = null;
                float[][] hsv = null;
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (String.Compare(reader.Name, "pattern") == 0)
                            {
                                patternName = reader.GetAttribute("name");
                                string pvar = reader.GetAttribute("variable");
                                if (String.Compare(pvar, pattern) == 0)
                                {
                                    using (var innerReader = reader.ReadSubtree())
                                    {
                                        while (innerReader.Read())
                                        {
                                            if (innerReader.NodeType == XmlNodeType.Element)
                                            {
                                                string istr = innerReader.GetAttribute("key");
                                                if (istr != null && String.Compare(istr, "rgbmask") == 0)
                                                {
                                                    string link = innerReader.GetAttribute("value");
                                                    link = link.Replace("key:", "");
                                                    if (!link.Contains("0x"))
                                                        rgb = ResolveAssetPathToTGI(link);
                                                    else
                                                        rgb = new TGI(link);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Background Image") == 0)
                                                {
                                                    string link = innerReader.GetAttribute("value");
                                                    link = link.Replace("key:", "");
                                                    if (!link.Contains("0x"))
                                                        background = ResolveAssetPathToTGI(link);
                                                    else
                                                        background = new TGI(link);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Channel 1") == 0)
                                                {
                                                    if (channels == null) channels = new TGI[3];
                                                    string link = innerReader.GetAttribute("value");
                                                    link = link.Replace("key:", "");
                                                    if (!link.Contains("0x"))
                                                        channels[0] = ResolveAssetPathToTGI(link);
                                                    else
                                                        channels[0] = new TGI(link);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Channel 2") == 0)
                                                {
                                                    if (channels == null) channels = new TGI[3];
                                                    string link = innerReader.GetAttribute("value");
                                                    link = link.Replace("key:", "");
                                                    if (!link.Contains("0x"))
                                                        channels[1] = ResolveAssetPathToTGI(link);
                                                    else
                                                        channels[1] = new TGI(link);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Channel 3") == 0)
                                                {
                                                    if (channels == null) channels = new TGI[3];
                                                    string link = innerReader.GetAttribute("value");
                                                    link = link.Replace("key:", "");
                                                    if (!link.Contains("0x"))
                                                        channels[2] = ResolveAssetPathToTGI(link);
                                                    else
                                                        channels[2] = new TGI(link);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Channel 1 Enabled") == 0)
                                                {
                                                    if (channelEnabled == null) channelEnabled = new bool[3];
                                                    string enable = innerReader.GetAttribute("value");
                                                    channelEnabled[0] = (String.Compare(enable, "true") == 0);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Channel 2 Enabled") == 0)
                                                {
                                                    if (channelEnabled == null) channelEnabled = new bool[3];
                                                    string enable = innerReader.GetAttribute("value");
                                                    channelEnabled[1] = (String.Compare(enable, "true") == 0);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Channel 3 Enabled") == 0)
                                                {
                                                    if (channelEnabled == null) channelEnabled = new bool[3];
                                                    string enable = innerReader.GetAttribute("value");
                                                    channelEnabled[2] = (String.Compare(enable, "true") == 0);
                                                    continue;
                                                }

                                                else if (istr != null && String.Compare(istr, "Color") == 0)
                                                {
                                                    color = GetQuadrupleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Color 0") == 0)
                                                {
                                                    if (colors == null) colors = new float[5][];
                                                    colors[0] = GetQuadrupleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Color 1") == 0)
                                                {
                                                    if (colors == null) colors = new float[5][];
                                                    colors[1] = GetQuadrupleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Color 2") == 0)
                                                {
                                                    if (colors == null) colors = new float[5][];
                                                    colors[2] = GetQuadrupleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Color 3") == 0)
                                                {
                                                    if (colors == null) colors = new float[5][];
                                                    colors[3] = GetQuadrupleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Color 4") == 0)
                                                {
                                                    if (colors == null) colors = new float[5][];
                                                    colors[4] = GetQuadrupleFloat(innerReader);
                                                    continue;
                                                }

                                                else if (istr != null && String.Compare(istr, "Base H Bg") == 0)
                                                {
                                                    if (hsvBaseBg == null) hsvBaseBg = new float[3];
                                                    hsvBaseBg[0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base S Bg") == 0)
                                                {
                                                    if (hsvBaseBg == null) hsvBaseBg = new float[3];
                                                    hsvBaseBg[1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base V Bg") == 0)
                                                {
                                                    if (hsvBaseBg == null) hsvBaseBg = new float[3];
                                                    hsvBaseBg[2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base H 1") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[0] == null) hsvBase[0] = new float[3];
                                                    hsvBase[0][0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base S 1") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[0] == null) hsvBase[0] = new float[3];
                                                    hsvBase[0][1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base V 1") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[0] == null) hsvBase[0] = new float[3];
                                                    hsvBase[0][2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base H 2") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[1] == null) hsvBase[1] = new float[3];
                                                    hsvBase[1][0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base S 2") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[1] == null) hsvBase[1] = new float[3];
                                                    hsvBase[1][1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base V 2") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[1] == null) hsvBase[1] = new float[3];
                                                    hsvBase[1][2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base H 3") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[2] == null) hsvBase[2] = new float[3];
                                                    hsvBase[2][0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base S 3") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[2] == null) hsvBase[2] = new float[3];
                                                    hsvBase[2][1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "Base V 3") == 0)
                                                {
                                                    if (hsvBase == null) hsvBase = new float[3][];
                                                    if (hsvBase[2] == null) hsvBase[2] = new float[3];
                                                    hsvBase[2][2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }

                                                else if (istr != null && String.Compare(istr, "H Bg") == 0)
                                                {
                                                    if (hsvBg == null) hsvBg = new float[3];
                                                    hsvBg[0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "S Bg") == 0)
                                                {
                                                    if (hsvBg == null) hsvBg = new float[3];
                                                    hsvBg[1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "V Bg") == 0)
                                                {
                                                    if (hsvBg == null) hsvBg = new float[3];
                                                    hsvBg[2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "H 1") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[0] == null) hsv[0] = new float[3];
                                                    hsv[0][0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "S 1") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[0] == null) hsv[0] = new float[3];
                                                    hsv[0][1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "V 1") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[0] == null) hsv[0] = new float[3];
                                                    hsv[0][2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "H 2") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[1] == null) hsv[1] = new float[3];
                                                    hsv[1][0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "S 2") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[1] == null) hsv[1] = new float[3];
                                                    hsv[1][1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "V 2") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[1] == null) hsv[1] = new float[3];
                                                    hsv[1][2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "H 3") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[2] == null) hsv[2] = new float[3];
                                                    hsv[2][0] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "S 3") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[2] == null) hsv[2] = new float[3];
                                                    hsv[2][1] = GetSingleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "V 3") == 0)
                                                {
                                                    if (hsv == null) hsv = new float[3][];
                                                    if (hsv[2] == null) hsv[2] = new float[3];
                                                    hsv[2][2] = GetSingleFloat(innerReader);
                                                    continue;
                                                }

                                                else if (istr != null && String.Compare(istr, "HSVShift Bg") == 0)
                                                {
                                                    hsvShiftBg = GetTripleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "HSVShift 1") == 0)
                                                {
                                                    if (hsvShift == null) hsvShift = new float[3][];
                                                    hsvShift[0] = GetTripleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "HSVShift 2") == 0)
                                                {
                                                    if (hsvShift == null) hsvShift = new float[3][];
                                                    hsvShift[1] = GetTripleFloat(innerReader);
                                                    continue;
                                                }
                                                else if (istr != null && String.Compare(istr, "HSVShift 3") == 0)
                                                {
                                                    if (hsvShift == null) hsvShift = new float[3][];
                                                    hsvShift[2] = GetTripleFloat(innerReader);
                                                    continue;
                                                }

                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    return new Form1.PatternInfo(patternName, rgb, color, colors, background, channels, channelEnabled, hsvBaseBg, hsvBase, hsvBg, hsv, hsvShiftBg, hsvShift);
                                }
                            }
                        }
                    }
                }
                return null;
            }

            private float GetSingleFloat(XmlReader innerReader)
            {
                string c = innerReader.GetAttribute("value");
                try
                {
                    float tmp = float.Parse(c, CultureInfo.InvariantCulture);
                    return tmp;
                }
                catch
                {
                    return 0;
                }
            }

            private float[] GetTripleFloat(XmlReader innerReader)
            {
                string c = innerReader.GetAttribute("value");
                try
                {
                    string[] colorstr = c.Split(',');
                    if (colorstr.Length != 3) return new float[] { 0, 0, 0 };
                    float[] tmp = new float[3];
                    for (int i = 0; i < 3; i++)
                    {
                        tmp[i] = float.Parse(colorstr[i], CultureInfo.InvariantCulture);
                    }
                    return tmp;
                }
                catch
                {
                    return new float[] { 0, 0, 0 };
                }
            }

            private float[] GetQuadrupleFloat(XmlReader innerReader)
            {
                string c = innerReader.GetAttribute("value");
                try
                {
                    string[] colorstr = c.Split(',');
                    if (colorstr.Length != 4) return new float[] { 0, 0, 0, 0 };
                    float[] tmp = new float[4];
                    for (int i = 0; i < 4; i++)
                    {
                        tmp[i] = float.Parse(colorstr[i], CultureInfo.InvariantCulture);
                    }
                    return tmp;
                }
                catch
                {
                    return new float[] { 0, 0, 0, 0 };
                }
            }

            public float[] GetColor(string colorname)
            {
                if (xml == null || xml.Length == 0) return null;
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string str = reader.GetAttribute("key");
                            if (str != null && String.Compare(str, colorname) == 0)
                            {
                                string tmp = reader.GetAttribute("value");
                                try
                                {
                                    string[] colorstr = tmp.Split(',');
                                    if (colorstr.Length != 4) return null;
                                    float[] colors = new float[4];
                                    for (int i = 0; i < 4; i++)
                                    {
                                        colors[i] = float.Parse(colorstr[i], CultureInfo.InvariantCulture);
                                    }
                                    return colors;
                                }
                                catch
                                {
                                    return null;
                                }
                            }
                        }

                    }
                }
                return null;
            }

            public float GetOpacity()
            {
                if (xml == null || xml.Length == 0) return 1f;
                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    if (reader == null) return 1f;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string str = reader.GetAttribute("key");
                            if (str != null && String.Compare(str, "Opacity") == 0)
                            {
                                string tmp = reader.GetAttribute("value");
                                try
                                {
                                    float color;
                                    color = float.Parse(tmp, CultureInfo.InvariantCulture);
                                    return color;
                                }
                                catch
                                {
                                    return 1f;
                                }
                            }
                        }

                    }
                }
                return 1f;
            }
        }
    }
}
