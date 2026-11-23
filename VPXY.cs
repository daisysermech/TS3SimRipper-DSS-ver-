using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TS3SimRipper
{
    public class VPXY
    {
        int rcolVersion, rcolCount, index3, extCount, intCount;
        TGI[] extITG, intITG;
        int chunkPos, chunkSize;
        char[] magic;
        int version;
        int tgiOffset;
        int tgiSize;
        byte count;
        Entry[] entries;
        byte boxType;
        float[] boundingBox;
        uint unknown;
        byte flag;
        int ftptIndex;
        int tgiCount;
        TGI[] tgiList;

        public Entry[] Type0Entries
        {
            get
            {
                if (entries == null) return null;
                List<Entry> tmp = new List<Entry>();
                foreach (Entry e in this.entries)
                {
                    if (e.type == EntryType.MeshEntry) tmp.Add(e);
                }
                return tmp.ToArray();
            }
        }

        public Entry[] Type1Entries
        {
            get
            {
                if (entries == null) return null;
                List<Entry> tmp = new List<Entry>();
                foreach (Entry e in this.entries)
                {
                    if (e.type == EntryType.BoneEntry) tmp.Add(e);
                }
                return tmp.ToArray();
            }
        }

        public TGI[] MeshLinks(int lod)
        {
            List<TGI> tmp = new List<TGI>();
            foreach (Entry e in this.entries)
            {
                if (e.type == EntryType.MeshEntry && e.lod == lod)
                {
                    foreach (int i in e.tgiRefs)
                    {
                        tmp.Add(this.tgiList[i]);
                    }
                }
            }
            return tmp.ToArray();
        }

        public TGI[] BondLinks
        {
            get
            {
                List<TGI> tmp = new List<TGI>();
                foreach (Entry e in this.entries)
                {
                    if (e.type == EntryType.BoneEntry)
                    {
                        foreach (int i in e.tgiRefs)
                        {
                            tmp.Add(this.tgiList[i]);
                        }
                    }
                }
                return tmp.ToArray();
            }
        }

        public TGI[] AllLinks
        {
            get
            {
                List<TGI> tmp = new List<TGI>();
                foreach (Entry e in this.entries)
                {

                    foreach (int i in e.tgiRefs)
                    {
                        tmp.Add(this.tgiList[i]);
                    }

                }
                return tmp.ToArray();
            }
        }

        public TGI[] tgiArray
        {
            get
            {
                return this.tgiList;
            }
            set
            {
                this.tgiList = new TGI[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    this.tgiList[i] = new TGI(value[i].Type, value[i].Group, value[i].Instance);
                }
            }
        }

        public VPXY(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            rcolVersion = br.ReadInt32();
            rcolCount = br.ReadInt32();
            index3 = br.ReadInt32();
            extCount = br.ReadInt32();
            intCount = br.ReadInt32();
            if (intCount > 0) intITG = new TGI[intCount];
            for (int i = 0; i < intCount; i++)
            {
                intITG[i] = new TGI(br, TGI.TGIsequence.ITG);
            }
            if (extCount > 0) extITG = new TGI[extCount];
            for (int i = 0; i < extCount; i++)
            {
                extITG[i] = new TGI(br, TGI.TGIsequence.ITG);
            }
            chunkPos = br.ReadInt32();
            chunkSize = br.ReadInt32();
            magic = new char[4];
            magic = br.ReadChars(4);
            version = br.ReadInt32();
            tgiOffset = br.ReadInt32();
            tgiSize = br.ReadInt32();
            count = br.ReadByte();
            entries = new Entry[count];
            for (int i = 0; i < count; i++)
            {
                entries[i] = new Entry(br);
            }
            boxType = br.ReadByte();
            boundingBox = new float[6];
            for (int i = 0; i < 6; i++)
            {
                boundingBox[i] = br.ReadSingle();
            }
            unknown = br.ReadUInt32();
            if (version <= 4)
            {
                flag = br.ReadByte();
                if (flag == 1)
                {
                    ftptIndex = br.ReadInt32();
                }
            }
            if (tgiSize > 0)
            {
                tgiCount = br.ReadInt32();
                tgiList = new TGI[tgiCount];
                for (int i = 0; i < tgiCount; i++)
                {
                    tgiList[i] = new TGI(br);
                }
            }
            else
            {
                tgiCount = 0;
                tgiList = new TGI[0];
            }
        }

        public class Entry
        {
            internal EntryType type;
            internal byte lod;
            internal byte refCount;
            internal int[] tgiRefs;

            public EntryType Type
            {
                get { return type; }
                set { this.type = value; }
            }

            public int Lod
            {
                get
                {
                    if (this.type == EntryType.MeshEntry & tgiRefs.Length > 0) return (int)lod;
                    else return -1;
                }
                set
                {
                    if (this.type == EntryType.MeshEntry & tgiRefs.Length > 0) this.lod = (byte)value;
                }
            }

            public int Size
            {
                get
                {
                    if (this.type == EntryType.MeshEntry)
                    {
                        return 3 + (this.refCount * 4);
                    }
                    else
                    {
                        return 5;
                    }
                }
            }

            public int[] IndexArray
            {
                get { return tgiRefs; }
            }

            internal Entry(BinaryReader br)
            {
                type = (EntryType)br.ReadByte();
                if (type == EntryType.MeshEntry)
                {
                    lod = br.ReadByte();
                    refCount = br.ReadByte();
                    tgiRefs = new int[refCount];
                    for (int i = 0; i < refCount; i++)
                    {
                        tgiRefs[i] = br.ReadInt32();
                    }
                }
                else if (type == EntryType.BoneEntry)
                {
                    tgiRefs = new int[1];
                    tgiRefs[0] = br.ReadInt32();
                }
            }

            public Entry(int TGIindex)
            {
                this.type = EntryType.BoneEntry;
                this.tgiRefs = new int[] { TGIindex };
            }

            public Entry(byte LOD, int[] TGIindexArray)
            {
                this.type = EntryType.MeshEntry;
                this.lod = LOD;
                this.tgiRefs = new int[TGIindexArray.Length];
                this.refCount = (byte)TGIindexArray.Length;
                for (int i = 0; i < TGIindexArray.Length; i++)
                {
                    this.tgiRefs[i] = TGIindexArray[i];
                }
            }

            public Entry(int LOD, int[] TGIindexArray) : this((byte)LOD, TGIindexArray) { }

            public Entry(int LOD, int TGIindex) : this((byte)LOD, new int[] { TGIindex }) { }

            internal void Write(BinaryWriter bw)
            {
                bw.Write((byte)type);
                if (type == EntryType.MeshEntry)
                {
                    bw.Write(lod);
                    bw.Write(refCount);
                    for (int i = 0; i < refCount; i++)
                    {
                        bw.Write(tgiRefs[i]);
                    }
                }
                else if (type == EntryType.BoneEntry)
                {
                    bw.Write(tgiRefs[0]);
                }
            }
        }

        public enum EntryType : byte
        {
            MeshEntry = 0,
            BoneEntry = 1
        }
    }
}
