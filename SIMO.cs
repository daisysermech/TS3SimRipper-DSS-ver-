using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TS3SimRipper
{
    public class SIMO
    {
        uint version = 0x0015;

        //-- version >= 0x08 attributes
        XMLEntry[] xmlEntries;//version >= 0x10
        uint[] unknown3;//version >= 0x09 && version < 0x10
        int unknown1;//version >= 0x10
        int unknown2;//version >= 0x10
        public float heavySlider;
        public float muscleSlider;
        public float thinSlider;
        uint unknown6;//version >= 0x09
        AgeGender age;
        AgeGender gender;
        Species species;
        uint handedness;//version >= 0x09
        public short skinToneIndex;//byte if version < 0x15
        public byte hairToneIndex;//version == 0x08
        public float skinColorSlider;
        public float definitionSlider;//version >= 0x11
        public float breastSlider;//version >= 0x12
        UInt32 hairBaseColour;//version >= 0x0E
        UInt32 hairHaloHighColour;//version >= 0x0E
        UInt32 hairHaloLowColour;//version >= 0x0E
        float numCurls;//version >= 0x13
        float curlPixelRadius;//version >= 0x13
        TGI furMap;//version >= 0x14
        uint unknown11;//version == 0x08
        byte unknown12;//version >= 0x09, version <= 0x0D
        CASPEntry[] caspEntries;
        byte zero;
        BlendEntry[] faceEntries;
        uint unknown13;//version == 0x08
        TGI[] tgiList;

        //-- version < 0x08 attributes
        uint unknown1v7;
        uint unknown2v7;
        uint unknown3v7;
        AgeGender agev7;
        AgeGender genderv7;
        uint unknown4v7;
        TGI skinTonev7;
        TGI hairTonev7;
        uint unknown5v7;
        uint unknown6v7;
        CASPV7Entry[] caspEntriesv7;
        ushort unknown7v7;

        public XMLEntry[] PresetXMLs { get { return this.xmlEntries; } }
        public Species SimSpecies { get { return this.species; } }
        public AgeGender SimAge { get { return this.age; } }
        public AgeGender SimGender { get { return this.gender; } }
        /// <summary>
        /// Returns fat, fit, and thin values
        /// </summary>
        public float[] SimPhysique { get { return new float[] { this.heavySlider, this.muscleSlider, this.thinSlider }; } }
        public CASPEntry[] CASPEntries { get { return this.caspEntries; } }
        public BlendEntry[] BlendEntries { get { return this.faceEntries; } }
        public TGI[] TGILinks { get { return this.tgiList; } }

        public SIMO(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            version = br.ReadUInt32();

            if (version >= 0x08)
            {
                long tgiPosn = br.ReadInt32() + br.BaseStream.Position;

                if (version >= 0x09)
                {
                    if (version >= 0x10)
                    {
                        uint xmlCount = br.ReadUInt32();
                        xmlEntries = new XMLEntry[xmlCount];
                        for (int i = 0; i < xmlCount; i++) xmlEntries[i] = new XMLEntry(br);
                    }
                    else
                    {
                        unknown3 = new uint[br.ReadUInt32()];
                        for (int i = 0; i > unknown3.Length; i++) unknown3[i] = br.ReadUInt32();
                    }
                }

                if (version >= 0x0E)
                {
                    unknown1 = br.ReadInt32();
                    unknown2 = br.ReadInt32();
                }

                heavySlider = br.ReadSingle();
                muscleSlider = br.ReadSingle();
                thinSlider = br.ReadSingle();

                if (version >= 0x09)
                {
                    unknown6 = br.ReadUInt32();
                }

                age = (AgeGender)br.ReadUInt32();
                gender = (AgeGender)br.ReadUInt32();
                species = (Species)br.ReadUInt32();

                if (version >= 0x09)
                    handedness = br.ReadUInt32();

                if (version >= 0x15)
                    skinToneIndex = br.ReadInt16();
                else
                    skinToneIndex = br.ReadByte();

                if (version == 0x08)
                    hairToneIndex = br.ReadByte();

                skinColorSlider = br.ReadSingle();

                if (version >= 0x09)
                {
                    if (version >= 0x000E)
                    {
                        if (version >= 0x0011)
                        {
                            definitionSlider = br.ReadSingle();
                            if (version >= 0x0012)
                            {
                                breastSlider = br.ReadSingle();
                            }
                        }

                        hairBaseColour = br.ReadUInt32();
                        hairHaloHighColour = br.ReadUInt32();
                        hairHaloLowColour = br.ReadUInt32();

                        if (version >= 0x0013)
                        {
                            numCurls = br.ReadSingle();
                            curlPixelRadius = br.ReadSingle();
                            if (version >= 0x0014)
                            {
                                furMap = new TGI(br);
                            }
                        }
                    }
                    else
                    {
                        unknown12 = br.ReadByte();//?hairToneIndex
                    }
                }
                else
                {
                    unknown11 = br.ReadUInt32();
                }

                byte caspCount = br.ReadByte();
                caspEntries = new CASPEntry[caspCount];
                for (int i = 0; i < caspCount; i++) caspEntries[i] = new CASPEntry(br, version);

                zero = br.ReadByte();

                byte faceCount = br.ReadByte();
                faceEntries = new BlendEntry[faceCount];
                for (int i = 0; i < faceCount; i++) faceEntries[i] = new BlendEntry(br, version);

                if (version < 0x0A)
                    unknown13 = br.ReadUInt32();

                if (version >= 0x15)
                {
                    short count = br.ReadInt16();
                    tgiList = new TGI[count];
                    for (int i = 0; i < count; i++) tgiList[i] = new TGI(br, TGI.TGIsequence.IGT);
                }
                else
                {
                    byte count = br.ReadByte();
                    tgiList = new TGI[count];
                    for (int i = 0; i < count; i++) tgiList[i] = new TGI(br, TGI.TGIsequence.IGT);
                }
            }

            else
            {
                    // Ancient versions
                unknown1v7 = br.ReadUInt32();
                unknown2v7 = br.ReadUInt32();
                unknown3v7 = br.ReadUInt32();

                agev7 = (AgeGender)br.ReadUInt32();
                genderv7 = (AgeGender)br.ReadUInt32();

                unknown4v7 = br.ReadUInt32();

                skinTonev7 = new TGI(br, TGI.TGIsequence.IGT);
                hairTonev7 = new TGI(br, TGI.TGIsequence.IGT);

                unknown5v7 = br.ReadUInt32();
                unknown6v7 = br.ReadUInt32();

                byte caspCount = br.ReadByte();
                caspEntriesv7 = new CASPV7Entry[caspCount];
                for (int i = 0; i < caspCount; i++) caspEntriesv7[i] = new CASPV7Entry(br, version);

                unknown7v7 = br.ReadUInt16();
            }
        }

        public class XMLEntry
        {
            byte unknown1;//version>=0x0B
            string xml;

            public string XML { get { return this.xml; } }

            public XMLEntry(BinaryReader br)
            {
                unknown1 = br.ReadByte();
                xml = System.Text.Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2));
            }
        }

        public class IndexPair
        {
            short txtc1index;//byte if version < 0x15
            short txtc2index;//byte if version < 0x15

            public IndexPair(BinaryReader br, uint parentVersion)
            {
                if (parentVersion >= 0x00000015)
                {
                    txtc1index = br.ReadInt16();
                    txtc2index = br.ReadInt16();
                }
                else
                {
                    txtc1index = br.ReadByte();
                    txtc2index = br.ReadByte();
                }
            }
        }

        public class CASPEntry
        {
            short casPartIndex;//byte if version < 0x15
            ClothingType clothing;//version >= 0x0E
            IndexPair[] txtcIndexes;

            public short CaspLinkIndex { get { return this.casPartIndex; } }
            public ClothingType TypeOfClothing { get { return this.clothing;  } }

            public CASPEntry(BinaryReader br, uint parentVersion)
            {
                if (parentVersion >= 0x15)
                {
                    casPartIndex = br.ReadInt16();
                }
                else
                {
                    casPartIndex = br.ReadByte();
                }
                if (parentVersion >= 0x0E) clothing = (ClothingType)br.ReadUInt32();
                byte indexCount = br.ReadByte();
                txtcIndexes = new IndexPair[indexCount];
                for (int i = 0; i < indexCount; i++) txtcIndexes[i] = new IndexPair(br, parentVersion);
            }
        }

        public class TGIPair
        {
            TGI txtc1TGI;
            TGI txtc2TGI;

            public TGIPair(BinaryReader br)
            {
                txtc1TGI = new TGI(br);
                txtc2TGI = new TGI(br);
            }
        }

        public class CASPV7Entry
        {
            TGI casPart;
            TGIPair[] txtcTGIs;
            ClothingType clothing;

            public CASPV7Entry(BinaryReader br, uint parentVersion)
            {
                casPart = new TGI(br);
                byte tgiCount = br.ReadByte();
                txtcTGIs = new TGIPair[tgiCount];
                for (int i = 0; i < tgiCount; i++) txtcTGIs[i] = new TGIPair(br);
                clothing = (ClothingType)br.ReadUInt32();
            }
        }

        public class BlendEntry
        {
            short faceIndex;//byte if version < 0x15
            float weight;

            public short BlendIndex { get { return this.faceIndex; } }
            public float Weight { get { return this.weight; } }

            public BlendEntry(BinaryReader br, uint parentVersion)
            {
                if (parentVersion >= 0x00000015)
                {
                    faceIndex = br.ReadInt16();
                }
                else
                {
                    faceIndex = br.ReadByte();
                }
                weight = br.ReadSingle();
            }
        }
    }
}

