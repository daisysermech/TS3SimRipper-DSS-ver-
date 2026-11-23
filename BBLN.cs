using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TS3SimRipper
{
    public class BBLN
    {
        int version;
        int TGI_offset, TGI_size;
        string partName;
        int unknown;
        TGI blendGeometry;
        Entry[] entries;
        TGI[] tgiList;

        public string BlendName { get { return this.partName; } }

        public TGI BGEOlink { get { return this.blendGeometry; } }

        public Entry[] Entries { get { return this.entries; } }

        public TGI[] TGIList { get { return this.tgiList; } }

        public BBLN(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            version = br.ReadInt32();
            TGI_offset = br.ReadInt32();
            TGI_size = br.ReadInt32();
            int tmp = br.ReadByte();
            byte[] tmpb = br.ReadBytes(tmp);
            partName = Encoding.BigEndianUnicode.GetString(tmpb);
            unknown = br.ReadInt32();
            if (version == 8)
            {
                blendGeometry = new TGI(br);
            }
            int entryCount = br.ReadInt32();
            entries = new Entry[entryCount];
            for (int i = 0; i < entryCount; i++)
            {
                entries[i] = new Entry(br);
            }
            int tgiCount = br.ReadInt32();
            tgiList = new TGI[tgiCount];
            for (int i = 0; i < tgiCount; i++)
            {
                tgiList[i] = new TGI(br);
            }
        }

        public BBLN(int version, string partName, TGI blendGeometryTGI)
        {
            this.version = version;
            this.partName = partName;
            this.unknown = 2;
            if (version == 8)
            {
                blendGeometry = new TGI(blendGeometryTGI);
            }
            this.entries = new Entry[1];
            this.entries[0] = new Entry(CASregions.Body,
                                        new MorphEntry[] { new MorphEntry(77951, 1f, 0) },
                                        new MorphEntry[0]);
            if (version == 8)
            {
                this.tgiList = new TGI[] { new TGI(0, 0, 0) };
            }
            else
            {
                this.tgiList = new TGI[] { new TGI(blendGeometryTGI.Type, blendGeometryTGI.Group, blendGeometryTGI.Instance) };
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(version);
            byte[] tmpb = Encoding.BigEndianUnicode.GetBytes(partName);
            this.TGI_offset = 13 + tmpb.Length;
            if (version == 8) this.TGI_offset += 16;
            for (int i = 0; i < entries.Length; i++)
            {
                this.TGI_offset += entries[i].Length;
            }
            bw.Write(TGI_offset);
            this.TGI_size = 4 + (tgiList.Length * 16);
            if (version == 8) this.TGI_size += 8;
            bw.Write(TGI_size);
            bw.Write((byte)tmpb.Length);
            bw.Write(tmpb);
            bw.Write(unknown);
            if (version == 8)
            {
                blendGeometry.Write(bw);
            }
            bw.Write(entries.Length);
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i].Write(bw);
            }
            bw.Write(tgiList.Length);
            for (int i = 0; i < tgiList.Length; i++)
            {
                tgiList[i].Write(bw);
            }
        }

        public class Entry
        {
            CASregions region;
            public MorphEntry[] geomMorphs;
            public MorphEntry[] boneMorphs;

            internal int Length
            {
                get { return 12 + (geomMorphs.Length * 12) + (boneMorphs.Length * 12); }
            }

            internal Entry(BinaryReader br)
            {
                region = (CASregions)br.ReadUInt32();
                int geomCount = br.ReadInt32();
                this.geomMorphs = new MorphEntry[geomCount];
                for (int i = 0; i < geomCount; i++)
                {
                    this.geomMorphs[i] = new MorphEntry(br);
                }
                int boneCount = br.ReadInt32();
                this.boneMorphs = new MorphEntry[boneCount];
                for (int i = 0; i < boneCount; i++)
                {
                    this.boneMorphs[i] = new MorphEntry(br);
                }
            }

            internal Entry(CASregions region, MorphEntry[] geomMorphs, MorphEntry[] boneMorphs)
            {
                this.region = region;
                this.geomMorphs = geomMorphs;
                this.boneMorphs = boneMorphs;
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write((uint)region);
                bw.Write(this.geomMorphs.Length);
                for (int i = 0; i < this.geomMorphs.Length; i++)
                {
                    this.geomMorphs[i].Write(bw);
                }
                bw.Write(this.boneMorphs.Length);
                for (int i = 0; i < this.boneMorphs.Length; i++)
                {
                    this.boneMorphs[i].Write(bw);
                }
            }
        }

        public class MorphEntry
        {
            public uint ageGenderSpecies;
            public float amount;
            public int tgiIndex;

            internal MorphEntry(BinaryReader br)
            {
                ageGenderSpecies = br.ReadUInt32();
                amount = br.ReadSingle();
                tgiIndex = br.ReadInt32();
            }

            internal MorphEntry(uint ageGenderFlags, float amount, int tgiIndex)
            {
                this.ageGenderSpecies = ageGenderFlags;
                this.amount = amount;
                this.tgiIndex = tgiIndex;
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(ageGenderSpecies);
                bw.Write(amount);
                bw.Write(tgiIndex);
            }
        }
    }
}
