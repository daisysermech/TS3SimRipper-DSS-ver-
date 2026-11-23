using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace TS3SimRipper
{
    public class TONE
    {
        int version;
        int TGIoffset;
        int TGIsize;
        int shaderCount;
        Shader[] shaders;
        int toneRampIndex;
        int subSkinRampIndex;
        int ageGenderKeys;
        List<ageGenderSet> ageGenders;
        byte dominant;
        int TGIcount;
        List<TGI> TGIlist;

        public int Version
        {
            get { return this.version; }
        }

        public TGI ToneRampLink
        {
            get { return this.TGIlist[this.toneRampIndex]; }
        }

        public ageGenderSet[] ageGenderArray
        {
            get { return this.ageGenders.ToArray(); }
            set { this.ageGenders = value.ToList(); }
        }

        public SkinSet GetSkinSet(Species species, AgeGender age, AgeGender gender, PartTypeFlags partType)
        {
            foreach (ageGenderSet set in this.ageGenders)
            {
                if ((partType == set.CAStype) &&
                    ((((uint)species & set.AgeGenderSpecies) > 0) || (species == Species.Human && set.SimSpecies == 0)) && 
                    (((uint)age & set.AgeGenderSpecies) > 0) &&
                    (((uint)gender & set.AgeGenderSpecies) > 0))
                {
                    int[] indexes = set.SkinLinks;
                    TGI[] tgis = new TGI[indexes.Length];
                    for (int i = 0; i < indexes.Length; i++) tgis[i] = indexes[i] >= 0 ? this.TGIlist[indexes[i]] : null;
                    return new SkinSet(set.CAStype, tgis);
                }
            }
            return null;
        }

        public TGI[] TGIarray
        {
            get { return this.TGIlist.ToArray(); }
            set { this.TGIlist = value.ToList(); }
        }

        public int EdgeColorAdjustment
        {
            get { return EdgeColorAdjuster(41, this.shaders[2].EdgeColor[1]); }
        }

        public int SpecularPowerAdjustment
        {
            get { return SpecularPowerAdjuster(4.0f, this.shaders[2].SpecularPower); }
        }

        internal TONE(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            this.version = br.ReadInt32();
            this.TGIoffset = br.ReadInt32();
            this.TGIsize = br.ReadInt32();
            this.shaderCount = br.ReadInt32();
            this.shaders = new Shader[shaderCount];
            for (int i = 0; i < shaderCount; i++)
            {
                this.shaders[i] = new Shader(br);
            }
            this.toneRampIndex = br.ReadInt32();
            this.subSkinRampIndex = br.ReadInt32();
            this.ageGenderKeys = br.ReadInt32();
            this.ageGenders = new List<ageGenderSet>();
            for (int i = 0; i < ageGenderKeys; i++)
            {
                this.ageGenders.Add(new ageGenderSet(br, this.version));
            }
            this.dominant = br.ReadByte();
            this.TGIcount = br.ReadInt32();
            this.TGIlist = new List<TGI>();
            for (int i = 0; i < TGIcount; i++)
            {
                this.TGIlist.Add(new TGI(br));
            }
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(this.version);
            int agLen;
            if (version == 4)
            {
                agLen = 28;
            }
            else
            {
                agLen = 36;
            }
            this.TGIoffset = 8 + (this.shaderCount * 17) + 12 + (this.ageGenders.Count * agLen) + 1;
            bw.Write(this.TGIoffset);
            this.TGIsize = (this.TGIlist.Count * 16) + 4;
            bw.Write(this.TGIsize);
            bw.Write(this.shaderCount);
            for (int i = 0; i < shaderCount; i++)
            {
                this.shaders[i].Write(bw);
            }
            bw.Write(this.toneRampIndex);
            bw.Write(this.subSkinRampIndex);
            bw.Write(this.ageGenders.Count);
            for (int i = 0; i < this.ageGenders.Count; i++)
            {
                this.ageGenders[i].Write(bw, version);
            }
            bw.Write(this.dominant);
            bw.Write(this.TGIlist.Count);
            for (int i = 0; i < this.TGIlist.Count; i++)
            {
                this.TGIlist[i].Write(bw);
            }
        }

        //public void ShadersAdjust(int adjustSpecEdgeColor, int adjustSpecPower)
        //{
        //    this.shaders[0] = new Shader(new byte[4] { EdgeColorCalculator(35, adjustSpecEdgeColor),
        //        EdgeColorCalculator(34, adjustSpecEdgeColor), EdgeColorCalculator(35, adjustSpecEdgeColor), 255 },
        //        new byte[4] { 0, 0, 0, 255 }, SpecularPowerCalculator(4f, adjustSpecPower));
        //    this.shaders[1] = new Shader(new byte[4] { EdgeColorCalculator(41, adjustSpecEdgeColor),
        //        EdgeColorCalculator(36, adjustSpecEdgeColor), EdgeColorCalculator(41, adjustSpecEdgeColor), 255 },
        //        new byte[4] { 0, 0, 0, 255 }, SpecularPowerCalculator(4f, adjustSpecPower));
        //    this.shaders[2] = new Shader(new byte[4] { EdgeColorCalculator(46, adjustSpecEdgeColor),
        //        EdgeColorCalculator(41, adjustSpecEdgeColor), EdgeColorCalculator(46, adjustSpecEdgeColor), 255 },
        //        new byte[4] { 0, 0, 0, 255 }, SpecularPowerCalculator(4f, adjustSpecPower));
        //    this.shaders[3] = new Shader(new byte[4] { EdgeColorCalculator(42, adjustSpecEdgeColor),
        //        EdgeColorCalculator(42, adjustSpecEdgeColor), EdgeColorCalculator(46, adjustSpecEdgeColor), 255 },
        //        new byte[4] { 0, 0, 0, 255 }, SpecularPowerCalculator(4f, adjustSpecPower));
        //    this.shaders[4] = new Shader(new byte[4] { EdgeColorCalculator(38, adjustSpecEdgeColor),
        //        EdgeColorCalculator(44, adjustSpecEdgeColor), EdgeColorCalculator(51, adjustSpecEdgeColor), 255 },
        //        new byte[4] { 45, 49, 53, 255 }, SpecularPowerCalculator(4f, adjustSpecPower));
        //}

        internal static byte EdgeColorCalculator(int baseValue, int adjuster)
        {
            double a = 4.48, b = -9.4;
            if (baseValue == 34)
            { a = 3.76; b = -12.2; }
            if (baseValue == 35)
            { a = 3.72; b = -11.8; }
            if (baseValue == 36)
            { a = 3.68; b = -11.4; }
            if (baseValue == 38)
            { a = 3.6; b = -10.6; }
            if (baseValue == 41)
            { a = 3.48; b = -9.4; }
            if (baseValue == 42)
            { a = 3.44; b = -9.0; }
            if (baseValue == 44)
            { a = 3.36; b = -8.2; }
            if (baseValue == 46)
            { a = 3.28; b = -7.4; }
            if (baseValue == 51)
            { a = 3.08; b = -5.4; }
            return (byte)Math.Round((a * Math.Pow(adjuster, 2)) + (b * adjuster) + 1);
        }

        internal static float SpecularPowerCalculator(float baseValue, int adjuster)
        {
            return (float)((.17 * Math.Pow(adjuster, 2)) + (-.15 * adjuster) + .5);
        }

        internal static int EdgeColorAdjuster(int baseValue, int currentValue)
        {
            double a = 4.48, b = -9.4;
            if (baseValue == 34)
            { a = 3.76; b = -12.2; }
            if (baseValue == 35)
            { a = 3.72; b = -11.8; }
            if (baseValue == 36)
            { a = 3.68; b = -11.4; }
            if (baseValue == 38)
            { a = 3.6; b = -10.6; }
            if (baseValue == 41)
            { a = 3.48; b = -9.4; }
            if (baseValue == 42)
            { a = 3.44; b = -9.0; }
            if (baseValue == 44)
            { a = 3.36; b = -8.2; }
            if (baseValue == 46)
            { a = 3.28; b = -7.4; }
            if (baseValue == 51)
            { a = 3.08; b = -5.4; }
            double c = 1.0 - currentValue;
            int tmp = (int)Math.Round((-b + Math.Sqrt(Math.Pow(b, 2) - (4 * a * c))) / (2 * a));
            if (tmp < 0) tmp = 0;
            if (tmp > 10) tmp = 10;
            return tmp;
        }

        internal static int SpecularPowerAdjuster(float baseValue, float currentValue)
        {
            double c = 0.5 - currentValue;
            int tmp = (int)Math.Round((.15 + Math.Sqrt(.0225 - (.68 * c))) / .34);
            if (tmp < 0) tmp = 0;
            if (tmp > 10) tmp = 10;
            return tmp;
        }

        internal class Shader
        {
            byte age;
            byte gender_species;
            byte handedness;
            byte unknown;
            byte[] edgeColor;
            byte[] specularColor;
            float specularPower;
            byte genetic;

            public byte[] EdgeColor
            {
                get { return new byte[4] { edgeColor[0], edgeColor[1], edgeColor[2], edgeColor[3] }; }
                set { this.edgeColor = new byte[4] { value[0], value[1], value[2], value[3] }; }
            }

            public byte[] SpecularColor
            {
                get { return new byte[4] { specularColor[0], specularColor[1], specularColor[2], specularColor[3] }; }
                set { this.specularColor = new byte[4] { value[0], value[1], value[2], value[3] }; }
            }

            public float SpecularPower
            {
                get { return this.specularPower; }
                set { this.specularPower = value; }
            }

            internal Shader(BinaryReader br)
            {
                this.age = br.ReadByte();
                this.gender_species = br.ReadByte();
                this.handedness = br.ReadByte();
                this.unknown = br.ReadByte();
                this.edgeColor = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    this.edgeColor[i] = br.ReadByte();
                }
                this.specularColor = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    this.specularColor[i] = br.ReadByte();
                }
                this.specularPower = br.ReadSingle();
                this.genetic = br.ReadByte();
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(this.age);
                bw.Write(this.gender_species);
                bw.Write(this.handedness);
                bw.Write(this.unknown);
                for (int i = 0; i < 4; i++)
                {
                    bw.Write(this.edgeColor[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    bw.Write(this.specularColor[i]);
                }
                bw.Write(this.specularPower);
                bw.Write(this.genetic);
            }
        }

        public class ageGenderSet
        {
            //byte age;
            //byte gender_species;
            //byte handedness;
            //byte unknown;
            uint ageGenderSpecies;
            uint type;
            int specularIndex;
            int darkIndex;
            int lightIndex;
            int normalIndex;
            int overlayIndex;
            int cutnessIndex;
            int cleavageIndex;

            public AgeGender Age
            {
                get { return (AgeGender)(this.ageGenderSpecies & 0x00FF); }
            }

            public AgeGender Gender
            {
                get { return (AgeGender)(this.ageGenderSpecies & 0xF000); }
            }

            public Species SimSpecies
            {
                get { return (Species)(this.ageGenderSpecies & 0x0F00); }
            }

            public uint AgeGenderSpecies
            {
                get { return this.ageGenderSpecies; }
            }

            public PartTypeFlags CAStype
            {
                get { return (PartTypeFlags)this.type; }
                set { this.type = (uint)value; }
            }

            public int[] SkinLinks
            {
                get
                {
                    int[] tmp = new int[7];
                    tmp[0] = this.specularIndex;
                    tmp[1] = this.darkIndex;
                    tmp[2] = this.lightIndex;
                    tmp[3] = this.normalIndex;
                    tmp[4] = this.overlayIndex;
                    tmp[5] = this.cutnessIndex;
                    tmp[6] = this.cleavageIndex;
                    return tmp;
                }
            }

            internal ageGenderSet(BinaryReader br, int version)
            {
                //this.age = br.ReadByte();
                //this.gender_species = br.ReadByte();
                //this.handedness = br.ReadByte();
                //this.unknown = br.ReadByte();
                this.ageGenderSpecies = br.ReadUInt32();
                this.type = br.ReadUInt32();
                this.specularIndex = br.ReadInt32();
                this.darkIndex = br.ReadInt32();
                this.lightIndex = br.ReadInt32();
                this.normalIndex = br.ReadInt32();
                this.overlayIndex = br.ReadInt32();
                if (version > 4)
                {
                    this.cutnessIndex = br.ReadInt32();
                    this.cleavageIndex = br.ReadInt32();
                }
                else
                {
                    this.cutnessIndex = -1;
                    this.cleavageIndex = -1;
                }
            }

            internal void Write(BinaryWriter bw, int version)
            {
                //bw.Write(this.age);
                //bw.Write(this.gender_species);
                //bw.Write(this.handedness);
                //bw.Write(this.unknown);
                bw.Write(this.ageGenderSpecies);
                bw.Write(this.type);
                bw.Write(this.specularIndex);
                bw.Write(this.darkIndex);
                bw.Write(this.lightIndex);
                bw.Write(this.normalIndex);
                bw.Write(this.overlayIndex);
                if (version > 4)
                {
                    bw.Write(this.cutnessIndex);
                    bw.Write(this.cleavageIndex);
                }
            }

            public override string ToString()
            {
                return this.ageGenderSpecies.ToString("X8") +
                    ", " + this.type.ToString() + ": " + this.specularIndex.ToString() +
                    ", " + this.darkIndex.ToString() + ", " + this.lightIndex.ToString() + ", " + this.normalIndex.ToString() +
                    ", " + this.overlayIndex.ToString() + ", " + this.cutnessIndex.ToString() + ", " + this.cleavageIndex.ToString();
            }
        }

        public class SkinSet : IComparable<SkinSet>
        {
            public PartTypeFlags type;
            public TGI specularLink;
            public TGI darkLink;
            public TGI lightLink;
            public TGI normalLink;
            public TGI overlayLink;
            public TGI cutnessLink;
            public TGI cleavageLink;

            public TGI LightSkinLink { get { return this.lightLink; } }
            public TGI SpecularSkinLink { get { return this.specularLink; } }
            public TGI NormalsSkinLink { get { return this.normalLink; } }
            public TGI OverlaySkinLink { get { return this.overlayLink; } }
            /// <summary>
            /// Returns dark, cutness, and cleavage in that order
            /// </summary>
            public TGI[] SliderLinks
            {
                get { return new TGI[] { this.darkLink, this.cutnessLink, this.cleavageLink }; }
            }

            public SkinSet(PartTypeFlags partType, TGI[] links)
            {
                this.specularLink = links[0];
                this.darkLink = links[1];
                this.lightLink = links[2];
                this.normalLink = links[3];
                this.overlayLink = links[4];
                this.cutnessLink = links[5];
                this.cleavageLink = links[6];
            }

            public int CompareTo(SkinSet other)
            {
                return ((uint)this.type).CompareTo((uint)other.type);
            }
        }
    }
}
