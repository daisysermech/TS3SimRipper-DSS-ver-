/* TS4 SimRipper, a tool for creating custom content for The Sims 4,
   Copyright (C) 2014  C. Marinetti

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
   The author may be contacted at modthesims.info, username cmarNYC. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TS3SimRipper
{
    public class BGEO
    {
        private char[] magic;
        private int version;
        private int sect1count, sect1lodcount, sect2count, sect3count;
        private int sect1headerSize, sect1lodSize;
        private int sect1offset, sect2offset, sect3offset;
        private Section1[] sect1;
        private Section2[] sect2;
        private Section3[] sect3;

        internal float weight;
        internal string name = "unknown";

        public List<VertexData> GetDeltas(int entry, int lod)
        {
            List<VertexData> vertDeltas = new List<VertexData>();
            int sect2count = this.section2count(entry, lod);
            int mySect2Start = this.getSection2StartIndex(entry, lod);
            int mySect2StartVertID = this.getLODstartVertexID(entry, lod);
            int runningOffset = this.getLODinitialOffset(entry, lod);
            int mySect3Start = this.getSection3StartIndex(entry, lod);
            for (int i = 0; i < sect2count; i++)
            {
                BGEO.Section2 sect2 = this.getSection2(mySect2Start + i);
                runningOffset += sect2.dataOffset;
                Vector3 pos = new Vector3(), norm = new Vector3();
                int tmpIndex = runningOffset;
                if (sect2.hasPositionData)
                {
                    pos = new Vector3(this.getSection3(tmpIndex + mySect3Start));
                    tmpIndex++;
                }
                if (sect2.hasNormalsData)
                {
                    norm = new Vector3(this.getSection3(tmpIndex + mySect3Start));
                }
                VertexData vert = new VertexData(mySect2StartVertID + i, pos, norm);
                vertDeltas.Add(vert);
            }
            return vertDeltas;
        }

        public int GetSection1EntryIndex(Species species, AgeGender age, AgeGender gender)
        {
            for (int i = 0; i < this.sect1count; i++)
            {
                if (((this.sect1[i].AgeGenderSpecies & (uint)species) > 0) &&
                    ((this.sect1[i].AgeGenderSpecies & (uint)age) > 0) &&
                    ((this.sect1[i].AgeGenderSpecies & (uint)gender) > 0))
                    return i;
            }
            return -1;
        }

        public int section1count
        {
            get
            {
                return sect1count;
            }
        }
        public int section1LODcount
        {
            get
            {
                return sect1lodcount;
            }
        }

        public int section2count()
        {
            return sect2count;
        }
        public int section2count(int section1entryIndex)
        {
            Section1 sect1 = getSection1(section1entryIndex);
            int tmp = 0;
            for (int i = 0; i < this.sect1lodcount; i++)
            {
                int[] LODdata = sect1.LODdata(i);
                tmp += LODdata[1];
            }
            return tmp;
        }
        public int section2count(int section1entryIndex, int lod)
        {
            Section1 sect1 = getSection1(section1entryIndex);
            int[] LODdata = sect1.LODdata(lod);
            return LODdata[1];
        }
        public int section3count()
        {
            return sect3count;
        }
        public int section3count(int section1entryIndex)
        {
            Section1 sect1 = getSection1(section1entryIndex);
            int tmp = 0;
            for (int i = 0; i < this.sect1lodcount; i++)
            {
                int[] LODdata = sect1.LODdata(i);
                tmp += LODdata[2];
            }
            return tmp;
        }
        public int section3count(int section1entryIndex, int lod)
        {
            Section1 sect1 = getSection1(section1entryIndex);
            int[] LODdata = sect1.LODdata(lod);
            return LODdata[2];
        }
        public Section1 getSection1(int section1entryIndex)
        {
            return sect1[section1entryIndex];
        }
        public Section2 getSection2(int section2index)
        {
            return sect2[section2index];
        }
        public float[] getSection3(int section3index)
        {
            return sect3[section3index].DeltaValues;
        }

        public int getSection2StartIndex(int section1entryIndex, int lod)
        {
            int tmp = 0;
            for (int i = 0; i < section1entryIndex; i++)
            {
                for (int j = 0; j < this.section1LODcount; j++)
                {
                    int[] lodDatatmp = sect1[i].LODdata(j);
                    tmp += lodDatatmp[1];
                }
            }
            for (int j = 0; j < lod; j++)
            {
                int[] lodDatatmp = sect1[section1entryIndex].LODdata(j);
                tmp += lodDatatmp[1];
            }
            return tmp;
        }
        public int getSection3StartIndex(int section1entryIndex, int lod)
        {
            int tmp = 0;
            for (int i = 0; i < section1entryIndex; i++)
            {
                for (int j = 0; j < this.section1LODcount; j++)
                {
                    int[] lodDatatmp = sect1[i].LODdata(j);
                    tmp += lodDatatmp[2];
                }
            }
            for (int j = 0; j < lod; j++)
            {
                int[] lodDatatmp = sect1[section1entryIndex].LODdata(j);
                tmp += lodDatatmp[2];
            }
            return tmp;
        }
        public int getLODstartVertexID(int section1entryIndex, int lod)
        {
            int[] lodDatatmp = sect1[section1entryIndex].LODdata(lod);
            return lodDatatmp[0];
        }
        public int getLODinitialOffset(int section1entryIndex, int lod)
        {
            //Offsets are calculated for all lods in the morphs as one set of numbers, with the total number of vertices
            //for each lod added at the end of the lod. Get the initial offset relative to the beginning of the current
            //lod, before the first section 2 offset. 
            if (section1entryIndex == 0 & lod == 0) return 0;
            int offset = 0;
            for (int e = 0; e < section1entryIndex; e++)
            {                                   //add offsets for all vertices in previous entries
                int eStart = this.getSection2StartIndex(e, 0);
                int eCount = this.section2count(e);
                for (int j = eStart; j < eStart + eCount; j++)
                {
                    Section2 tmp = this.getSection2(j);
                    offset += tmp.dataOffset;
                }
            }
            for (int i = 0; i < lod; i++)      //add offsets for all vertices in previous lods in current entry
            {
                int lodStart = this.getSection2StartIndex(section1entryIndex, i);
                int lodCount = this.section2count(section1entryIndex, i);
                for (int j = lodStart; j < lodStart + lodCount; j++)
                {
                    Section2 tmp = this.getSection2(j);
                    offset += tmp.dataOffset;
                }
            }
            return offset;
        }

        public BGEO() { }

        public BGEO(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            this.magic = br.ReadChars(4);
            if (new string(this.magic) != "BGEO")
            {
                throw new BlendException("Not a valid BGEO file.");
            }
            this.version = br.ReadInt32();
            if (version != 768)
            {
                throw new BlendException("Not a recognized BGEO version.");
            }
            this.sect1count = br.ReadInt32();
            this.sect1lodcount = br.ReadInt32();
            this.sect2count = br.ReadInt32();
            this.sect3count = br.ReadInt32();
            this.sect1headerSize = br.ReadInt32();
            this.sect1lodSize = br.ReadInt32();
            this.sect1offset = br.ReadInt32();
            this.sect2offset = br.ReadInt32();
            this.sect3offset = br.ReadInt32();
            this.sect1 = new Section1[sect1count];
            for (int i = 0; i < sect1count; i++)
            {
                this.sect1[i] = new Section1(br, sect1lodcount);
            }
            this.sect2 = new Section2[sect2count];
            for (int i = 0; i < sect2count; i++)
            {
                this.sect2[i] = new Section2(br);
            }
            this.sect3 = new Section3[sect3count];
            for (int i = 0; i < sect3count; i++)
            {
                this.sect3[i] = new Section3(br);
            }
            int sect2index = 0;
            for (int i = 0; i < sect1count; i++)
            {
                sect2index = this.sect1[i].FixSection3Count(this.sect2, sect1lodcount, sect2index);
            }
        }

        public void WriteFile(BinaryWriter bw)
        {
            bw.Write(this.magic);
            bw.Write(this.version);
            bw.Write(this.sect1count);
            bw.Write(this.sect1lodcount);
            bw.Write(this.sect2count);
            bw.Write(this.sect3count);
            bw.Write(this.sect1headerSize);
            bw.Write(this.sect1lodSize);
            bw.Write(this.sect1offset);
            bw.Write(this.sect2offset);
            bw.Write(this.sect3offset);
            for (int i = 0; i < this.sect1count; i++)
            {
                this.sect1[i].Write(bw, this.sect1lodcount);
            }
            for (int i = 0; i < this.sect2count; i++)
            {
                this.sect2[i].Write(bw);
            }
            for (int i = 0; i < this.sect3count; i++)
            {
                this.sect3[i].Write(bw);
            }
        }

        public class Section1
        {
            uint agegenderSpecies, region;
            int[] firstVertID, numVertIDs, entryCount, original_entryCount;

            internal Section1() { }

            internal Section1(BinaryReader br, int sect1lodcount)
            {
                this.agegenderSpecies = br.ReadUInt32();
                this.region = br.ReadUInt32();
                this.firstVertID = new int[sect1lodcount];
                this.numVertIDs = new int[sect1lodcount];
                this.entryCount = new int[sect1lodcount];
                this.original_entryCount = new int[sect1lodcount];
                for (int i = 0; i < sect1lodcount; i++)
                {
                    this.firstVertID[i] = br.ReadInt32();
                    this.numVertIDs[i] = br.ReadInt32();
                    this.entryCount[i] = br.ReadInt32();
                    this.original_entryCount[i] = this.entryCount[i];
                }
            }
            public Section1(int age, int gender, int species, int region, int[] firstVertID, int[] numVertIDs, int[] entryCount)
            {
                this.agegenderSpecies = (uint)(age + (gender << 12) + (species << 8)) + (1 << 16);
                this.region = (uint)region;
                this.firstVertID = firstVertID;
                this.numVertIDs = numVertIDs;
                this.entryCount = entryCount;
                this.original_entryCount = entryCount;
                if (firstVertID.Length != 4 | numVertIDs.Length != 4 | entryCount.Length != 4)
                    throw new BlendException("Section 1 constructor: LOD information arrays must have four elements.");
            }
            internal void Write(BinaryWriter bw, int sect1lodcount)
            {
                bw.Write(this.agegenderSpecies);
                bw.Write(this.region);
                for (int i = 0; i < sect1lodcount; i++)
                {
                    bw.Write(this.firstVertID[i]);
                    bw.Write(this.numVertIDs[i]);
                    bw.Write(this.entryCount[i]);
                }
            }
            internal int FixSection3Count(Section2[] sect2, int sect1lodcount, int sect2index)
            {
                int ind = sect2index;
                for (int i = 0; i < sect1lodcount; i++)
                {
                    int fixedCount = 0;
                    for (int j = ind; j < ind + this.numVertIDs[i]; j++)
                    {
                        if (sect2[j].hasPositionData) fixedCount++;
                        if (sect2[j].hasNormalsData) fixedCount++;
                    }
                    this.entryCount[i] = Math.Max(fixedCount, this.original_entryCount[i]);
                    ind += this.numVertIDs[i];
                }
                return ind;
            }
            
            public uint AgeGenderSpecies
            {
                get
                {
                    uint tmp = this.agegenderSpecies;
                    if ((tmp & 0x0F00) == 0) tmp = tmp | (uint)Species.Human;
                    return tmp;
                }
            }
            public uint CASregion
            {
                get
                {
                    return this.region;
                }
            }
            public override string ToString()
            {
                return ((Species)this.agegenderSpecies).ToString() + ((AgeGender)this.agegenderSpecies).ToString() + Environment.NewLine +
                    "LOD 0: First vertex " + this.firstVertID[0].ToString() + ", Number vertices " + this.numVertIDs[0].ToString() + ", Number entries " + this.entryCount[0] + Environment.NewLine +
                    "LOD 1: First vertex " + this.firstVertID[1].ToString() + ", Number vertices " + this.numVertIDs[1].ToString() + ", Number entries " + this.entryCount[1] + Environment.NewLine +
                    "LOD 2: First vertex " + this.firstVertID[2].ToString() + ", Number vertices " + this.numVertIDs[2].ToString() + ", Number entries " + this.entryCount[2] + Environment.NewLine +
                    "LOD 3: First vertex " + this.firstVertID[3].ToString() + ", Number vertices " + this.numVertIDs[3].ToString() + ", Number entries " + this.entryCount[3];
            }
            public string LODdatatoString(int LOD)
            {
                return this.firstVertID[LOD].ToString() + ", " + this.numVertIDs[LOD].ToString() + ", " + this.entryCount[LOD].ToString();
            }
            public int[] LODdata(int LOD)
            {
                return new int[] { this.firstVertID[LOD], this.numVertIDs[LOD], this.entryCount[LOD], this.original_entryCount[LOD] };
            }
        }
        public class Section2
        {
            bool hasPosition, hasNormals;
            int offset;
            internal Section2(BinaryReader br)
            {
                short tmp = br.ReadInt16();
                this.hasPosition = (tmp & 1) == 1;
                this.hasNormals = (tmp & 2) == 2;
                this.offset = tmp >> 2;
            }
            public Section2()
            {
                this.hasPosition = false;
                this.hasNormals = false;
                this.offset = 0;
            }
            public Section2(bool PositionFlag, bool NormalsFlag, int ListOffset)
            {
                this.hasPosition = PositionFlag;
                this.hasNormals = NormalsFlag;
                this.offset = ListOffset;
            }
            internal void Write(BinaryWriter bw)
            {
                short tmp = (short)(this.offset << 2);
                if (this.hasPosition) tmp += 1;
                if (this.hasNormals) tmp += 2;
                bw.Write(tmp);
            }
            public override string ToString()
            {
                return "Position: " + hasPosition.ToString() + ", Normals: " + hasNormals.ToString() + ", Offset: " + offset.ToString();
            }
            public bool hasPositionData
            {
                get
                {
                    return this.hasPosition;
                }
            }
            public bool hasNormalsData
            {
                get
                {
                    return this.hasNormals;
                }
            }
            public int dataOffset
            {
                get
                {
                    return offset;
                }
            }
        }

        internal class Section3
        {
            float x, y, z;
            internal Section3(BinaryReader br)
            {
                UInt16 xtmp = br.ReadUInt16();
                UInt16 ytmp = br.ReadUInt16();
                UInt16 ztmp = br.ReadUInt16();
                this.x = convertIn(xtmp);
                this.y = convertIn(ytmp);
                this.z = convertIn(ztmp);
            }
            public Section3(float Xvalue, float Yvalue, float Zvalue)
            {
                this.x = Xvalue;
                this.y = Yvalue;
                this.z = Zvalue;
            }
            internal void Write(BinaryWriter bw)
            {
                UInt16 xtmp = convertOut(this.x);
                UInt16 ytmp = convertOut(this.y);
                UInt16 ztmp = convertOut(this.z);
                bw.Write(xtmp);
                bw.Write(ytmp);
                bw.Write(ztmp);
            }
            internal float convertIn(UInt16 encode)
            {
                int tmp = encode;
                int tmp2 = tmp ^ 0x8000;            //flip sign bit
                int tmp3 = tmp2 << 16;
                int tmp4 = tmp3 >> 16;
                float tmpf = tmp4 / 2000f;
                return tmpf;
            }
            internal UInt16 convertOut(float decode)
            {
                int tmp = Convert.ToInt32(decode * 2000f);
                int tmp2 = tmp ^ 0x8000;            //flip sign bit
                byte[] tmp3 = BitConverter.GetBytes(tmp2);
                UInt16 tmp4 = BitConverter.ToUInt16(tmp3, 0);
                return tmp4;
            }
            public override string ToString()
            {
                return "X: " + x.ToString() + ", Y: " + y.ToString() + ", Z: " + z.ToString();
            }
            public float[] DeltaValues
            {
                get
                {
                    return new float[] { this.x, this.y, this.z };
                }
            }
        }

        public class VertexData
        {
            internal int vertexID;
            internal Vector3 position;
            internal Vector3 normals;
            internal VertexData(int ID, Vector3 position, Vector3 normals)
            {
                this.vertexID = ID;
                this.position = new Vector3(position);
                this.normals = new Vector3(normals);
            }
        }

        [global::System.Serializable]
        public class BlendException : ApplicationException
        {
            public BlendException() { }
            public BlendException(string message) : base(message) { }
            public BlendException(string message, Exception inner) : base(message, inner) { }
            protected BlendException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
