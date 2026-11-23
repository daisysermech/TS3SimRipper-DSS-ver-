using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TS3SimRipper
{
    public class SIME
    {
        ushort version;
        TGI[] links;        //links to SIMOs and other resources
        ushort unknown1;    //always 0?
        uint dataSize;
        ushort tableVersion;    //start of sim description, offsets are from here
        uint tableIndexOffset;    //offset from start of sim description to table index
        byte[] tableData;   //contains sim description data
        Dictionary<uint, int> tableIndex; //pairs of uints: field identifier / offset from start of table

        public TGI[] Links
        {
            get { return this.links; }
        }

        public string SimFirstName
        {
            get
            {
                int index;
                if (this.tableIndex.TryGetValue(0xFE481637, out index))
                {
                    int len = BitConverter.ToInt32(this.tableData, index);
                    return Encoding.Unicode.GetString(this.tableData, index + 4, len * 2);
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string SimLastName
        {
            get
            {
                int index;
                if (this.tableIndex.TryGetValue(0x655EAE83, out index))
                {
                    int len = BitConverter.ToInt32(this.tableData, index);
                    return Encoding.Unicode.GetString(this.tableData, index + 4, len * 2);
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public SIME(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            this.version = br.ReadUInt16();
            this.links = new TGI[br.ReadUInt16()];
            for (int i = 0; i < this.links.Length; i++) this.links[i] = new TGI(br, TGI.TGIsequence.IGT);
            this.unknown1 = br.ReadUInt16();
            this.dataSize = br.ReadUInt32();
            long descriptionStart = br.BaseStream.Position;
            this.tableVersion = br.ReadUInt16();
            this.tableIndexOffset = br.ReadUInt32();
            br.BaseStream.Position = descriptionStart;
            this.tableData = br.ReadBytes((int)tableIndexOffset);
            br.BaseStream.Position = descriptionStart + tableIndexOffset;
            ushort count = br.ReadUInt16();
            this.tableIndex = new Dictionary<uint, int>();
            for (int i = 0; i < count; i++)
            {
                uint hashVal = br.ReadUInt32();
                int index = br.ReadInt32();
                this.tableIndex.Add(hashVal, index);
            }

            //this.unknown2 = br.ReadUInt16();
            //this.unknown3 = new uint[2];
            //for (int i = 0; i < 2; i++) this.unknown3[i] = br.ReadUInt32();
            //this.unknown4 = br.ReadUInt16();
            //this.unknown5 = br.ReadUInt32();
            //this.unknown6 = br.ReadUInt16();
            //this.unknown7 = br.ReadSingle();
            //this.unknown8 = br.ReadUInt32();  //0x7FFFFFFF
            //this.unknown9 = new float[4];
            //for (int i = 0; i < 4; i++) this.unknown9[i] = br.ReadSingle();
            //this.unknown10 = br.ReadUInt32();   //always 1?
            //this.unknown11 = new uint[40];
            //for (int i = 0; i < 40; i++) this.unknown11[i] = br.ReadUInt32();
            //long mark = br.BaseStream.Position;
            //try
            //{
            //    this.unknown12 = new uint[4];
            //    for (int i = 0; i < this.unknown12.Length; i++) this.unknown12[i] = br.ReadUInt32();
            //    int len = br.ReadInt32();
            //    byte[] str = br.ReadBytes(len * 2);
            //    firstName = Encoding.Unicode.GetString(str);
            //    len = br.ReadInt32();
            //    str = br.ReadBytes(len * 2);
            //    lastName = Encoding.Unicode.GetString(str);
            //}
            //catch
            //{
            //    br.BaseStream.Position = mark;
            //    this.unknown12 = new uint[6];
            //    for (int i = 0; i < this.unknown12.Length; i++) this.unknown12[i] = br.ReadUInt32();
            //    int len = br.ReadInt32();
            //    byte[] str = br.ReadBytes(len * 2);
            //    firstName = Encoding.Unicode.GetString(str);
            //    len = br.ReadInt32();
            //    str = br.ReadBytes(len * 2);
            //    lastName = Encoding.Unicode.GetString(str);
            //}
            //if (!IsPrintable(firstName) || !IsPrintable(lastName))
            //{
            //    br.BaseStream.Position = mark;
            //    this.unknown12 = new uint[6];
            //    for (int i = 0; i < this.unknown12.Length; i++) this.unknown12[i] = br.ReadUInt32();
            //    int len = br.ReadInt32();
            //    byte[] str = br.ReadBytes(len * 2);
            //    firstName = Encoding.Unicode.GetString(str);
            //    len = br.ReadInt32();
            //    str = br.ReadBytes(len * 2);
            //    lastName = Encoding.Unicode.GetString(str);
            //}
        }

        internal bool IsPrintable(string s)
        {
            byte[] str = Encoding.ASCII.GetBytes(s);
            foreach (byte b in str) if (b < 32 || b > 126) return false;
            return true;
        }
    }
}
