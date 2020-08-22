// MPEG box object.  Contains methods and properties related to MPEG boxes.
// v0.11 written by Jason Frazier.  This is freeware.
//
// "Memory Stick" is a registered trademark of Sony Corporation.

using System;
using System.IO;
using System.Collections;
using BU = ByteUtilities.ByteUtil;

namespace MsvTagger
{
    /// <summary>
    /// Summary description for MpegBox.
    /// </summary>
    internal class MpegBox
    {
        internal MpegBox(MpegBox parentBox,
            ArrayList childBoxes,
            BoxType boxType,
            long boxOffset,
            uint boxSize)
        {
            Parent = parentBox;
            Children = childBoxes;
            Type = boxType;
            Offset = boxOffset;
            Size = boxSize;
        }

        internal virtual long Offset 
        {
            get { return this.offset; }
            set { this.offset = value; }
        }
        
        internal virtual uint Size 
        {
            get { return this.size; }
            set { this.size = value; }
        }

        internal virtual BoxType Type 
        {
            get { return this.type; }
            set { this.type = value; }
        }
        
        internal virtual BoxUuidType UuidType 
        {
            get { return this.uuidType; }
            set { this.uuidType = value; }
        }

        internal virtual byte[] Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        internal virtual ArrayList Children
        {
            get { return this.children; }
            set { this.children = value; }
        }

        internal virtual MpegBox Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        internal virtual bool HasParent() 
        {
            return null == this.parent ? false : true;
        }

        internal virtual bool HasChildren() 
        {
            return null == this.children ? false : true;
        }

        internal virtual long AbsoluteOffset() 
        {
            if (!HasParent())
            {
                // this MpegBox is top level, so its offset is already absolute
                return this.offset;
            }
            else
            {
                /*
                // this MpegBox's offset is relative to its parent's offset.
                return parent.offset + this.offset;
                */
                return this.offset;
            }
        }

        private MpegBox parent = null;
        private ArrayList children = null;
        private BoxType type = BoxType.Unknown;
        private BoxUuidType uuidType = BoxUuidType.None;
        private long offset = 0;
        private uint size = 0;
        private byte[] data = null;

        internal enum BoxType : uint
        {
            dinf = 0x64696E66,
            dref = 0x64726566,
            edts = 0x65647473,
            ftyp = 0x66747970,
            hdlr = 0x68646C72,
            iods = 0x696F6473,
            mdat = 0x6D646174,
            mdhd = 0x6D646864,
            mdia = 0x6D646961,
            minf = 0x6D696E66,
            moov = 0x6D6F6F76,
            mvhd = 0x6D766864,
            smhd = 0x736D6864,
            stbl = 0x7374626C,
            stco = 0x7374636F,
            stsc = 0x73747363,
            stsd = 0x73747364,
            stss = 0x73747373,
            stsz = 0x7374737A,
            stts = 0x73747473,
            tkhd = 0x746B6864,
            trak = 0x7472616B,
            tref = 0x74726566,
            Unknown = 0xFFFFFFFF,
            uuid = 0x75756964,
            vmhd = 0x766D6864
        }

        internal enum BoxUuidType
        {
            None,
            MsvTag,
            MyCustomTag,
            Unknown
        }

    }

}
