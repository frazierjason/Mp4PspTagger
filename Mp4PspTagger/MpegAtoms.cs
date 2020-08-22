using System;
using System.IO;
using System.Collections;
using BU = ByteUtilities.ByteUtil;

namespace MpegAtoms
{
    /// <summary>
    /// Summary description for MpegAtom.
    /// </summary>
    public class MpegAtom
    {
        public MpegAtom(MpegAtom parentAtom,
            ArrayList childAtoms,
            AtomType atomType,
            long atomOffset,
            uint atomSize)
        {
            Parent = parentAtom;
            Children = childAtoms;
            Type = atomType;
            Offset = atomOffset;
            Size = atomSize;
        }

        public virtual long Offset 
        {
            get { return this.offset; }
            set { this.offset = value; }
        }
        
        public virtual uint Size 
        {
            get { return this.size; }
            set { this.size = value; }
        }

        public virtual AtomType Type 
        {
            get { return this.type; }
            set { this.type = value; }
        }
        
        public virtual AtomUuidType UuidType 
        {
            get { return this.uuidType; }
            set { this.uuidType = value; }
        }

        public virtual byte[] Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public virtual ArrayList Children
        {
            get { return this.children; }
            set { this.children = value; }
        }

        public virtual MpegAtom Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        public virtual bool HasParent() 
        {
            return null == this.parent ? false : true;
        }

        public virtual bool HasChildren() 
        {
            return null == this.children ? false : true;
        }

        public virtual long AbsoluteOffset() 
        {
            if (!HasParent())
            {
                // this MpegAtom is top level, so its offset is already absolute
                return this.offset;
            }
            else
            {
                // this MpegAtom's offset is relative to its parent's offset.
                return parent.offset + this.offset;
            }
        }

        private MpegAtom parent = null;
        private ArrayList children = null;
        private AtomType type = AtomType.Unknown;
        private AtomUuidType uuidType = AtomUuidType.None;
        private long offset = 0;
        private uint size = 0;
        private byte[] data = null;

        public enum AtomType : uint
        {
            edts = 0x65647473,
            ftyp = 0x66747970,
            hdlr = 0x68646C72,
            iods = 0x696F6473,
            mdat = 0x6D646174,
            mdia = 0x6D646961,
            moov = 0x6D6F6F76,
            mvhd = 0x6D766864,
            stsc = 0x73747363,
            tkhd = 0x746B6864,
            trak = 0x7472616B,
            tref = 0x74726566,
            Unknown = 0xFFFFFFFF,
            uuid = 0x75756964
        }

        public enum AtomUuidType
        {
            None,
            SonyPspTag,
            MyCustomTag,
            Unknown
        }

    }

}
