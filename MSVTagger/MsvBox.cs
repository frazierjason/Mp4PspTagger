// MSV UUID Box object.  Contains methods and properties related to a MSV UUID MP4 Box.
// v0.11 written by Jason Frazier.  This is freeware.
//
// "Memory Stick" is a registered trademark of Sony Corporation.

using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using BU = ByteUtilities.ByteUtil;

namespace MsvTagger
{
    /// <summary>
    /// Summary description for MsvBox.
    /// </summary>
    internal class MsvBox
    {
        internal MsvBox(MpegBox uuidBox)
        {
            // TODO:  Implement a real exception handler for this
            if (MpegBox.BoxUuidType.MsvTag != uuidBox.UuidType)
                throw new MsvDecoderException("Box supplied is not of type MsvTag.");
            this.msvTagBox = uuidBox;
            this.boxData = uuidBox.Data;
            this.ms = new MemoryStream(this.boxData);
            this.br = new BinaryReader(ms);
            this.bw = new BinaryWriter(ms);
            this.DataSegments = getDataSegments(ms, 0);
        }

        ~MsvBox()
        {
            br.Close();
            bw.Close();
            ms.Close();
        }

        private ArrayList getDataSegments(MemoryStream ms, long endPosition)
        {
            // read and validate UUID value
            // TODO:  Implement a real exception handler for this
            if ("55534D5421D24FCEBB88695CFAC9C740" != 
                BU.ByteArrayToStringHex(br.ReadBytes(16)))
                throw new MsvDecoderException("Box data supplied is not of type MsvTag.");
            // read box's internal data size
            this.internalSize = BU.ReverseToUInt32(br.ReadBytes(4));
            // validate next marker in header
            // TODO:  Implement a real exception handler for this
            if ("4D544454" != BU.ByteArrayToStringHex(br.ReadBytes(4)))
                throw new MsvDecoderException("Box data supplied is not of type MsvTag.");
            // read segment count
            this.segmentCount = BU.ReverseToUInt16(br.ReadBytes(2));
            // we should now be at the start of the first segment
            ArrayList result = new ArrayList();
            MsvTagDataSegment ds = null;
            while (getNextDataSegment(out ds, endPosition, null))
            {
                result.Add(ds);
            }
            // TODO: throw exception if we still have a null result
            //       or if arraylist item count != this.segmentCount
            return result;
        }

        private bool getNextDataSegment(out MsvTagDataSegment dataSegment, long endPosition, MpegBox parentBox)
        {
            // return empty object when we reach data endPosition.  If endPosition is set
            // to zero, return empty object if we reach end of data.
            if (this.br.BaseStream.Position == (0 == endPosition ? this.br.BaseStream.Length : endPosition))
            {
                dataSegment = null;
                return false;
            }
            
            // create new segment and note its offset in the box data
            MsvTagDataSegment ds = new MsvTagDataSegment();
            ds.offset = this.br.BaseStream.Position;
            // offset in file is equal to the start of the containing box, plus eight bytes for 
            // box's size and uuid markers, plus segment offset from start of box data
            ds.absoluteOffset = msvTagBox.AbsoluteOffset() + 8 + ds.offset;
            // now get the segment header info and data
            ds.size = BU.ReverseToUInt16(this.br.ReadBytes(2));  // size
            ds.type = BU.ReverseToUInt32(this.br.ReadBytes(4));
            ds.language = BU.ReverseToUInt16(this.br.ReadBytes(2));
            ds.unknownProperty = BU.ReverseToUInt16(this.br.ReadBytes(2));
            if (0 < ds.size - 10)  // if there is any data left after the header, get data
                ds.segmentData = this.br.ReadBytes(ds.size - 10); // data           

            // all done, verify br's pointer is where it should be

            // return finished segment
            dataSegment = ds;
            return true;

        }

        internal static byte[] GetSegmentData(SegmentType segmentType, ArrayList dataSegments)
        {
            int foundCount = 0;
            byte[] result = null;
            foreach (MsvTagDataSegment ds in dataSegments)
            {
                if ((uint)segmentType == ds.type)
                {
                    foundCount++;
                    result = ds.segmentData;
                }
            }
            if (1 == foundCount)
                return result;
                // TODO:  Implement a real exception handler for these
            else if (0 == foundCount)
                throw new MsvTagNotFoundException("No " + Enum.GetName(typeof(SegmentType), segmentType) + 
                    " segment found in current MsvBox");
            else
                throw new MsvDecoderException("Multiple (" + foundCount.ToString() + 
                    ") " + Enum.GetName(typeof(SegmentType), segmentType) + 
                    " segments found in current MsvBox");
        }
       
        internal static ArrayList GetBoxSegmentList(ArrayList dataSegments)
        {
            ArrayList result = new ArrayList();
            foreach (MsvTagDataSegment ds in dataSegments)
            {
                result.Add(ds.type);
            }
            return result;
        }

        internal static string GetSegmentUnicodeStringData(SegmentType segmentType, ArrayList dataSegments)
        {
            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding(true, false);
            byte[] data = GetSegmentData(segmentType, dataSegments);
            return encoding.GetString(data);
        }
        
        private void OverwriteData(byte[] data, long offset, long growOrShrinkSize)
        {
            if (0 == growOrShrinkSize)
            {
                // exact replacement
                this.bw.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
                this.bw.Write(data);
            }
        }

        internal ArrayList DataSegments = null;
        private MpegBox msvTagBox = null;
        private byte[] boxData = null;
        private uint internalSize = 0;
        private ushort segmentCount = 0;
        internal enum SegmentType
        {
            Title = 0x01,
            Date = 0x03,
            Encoder = 0x04,
            Mystery0A = 0x0A,
            Mystery0B = 0x0B
        }

        internal MemoryStream ms = null;
        internal BinaryReader br = null;
        internal BinaryWriter bw = null;
    }

    internal class MsvTagDataSegment
    {
        // TODO: change these to private where applicable
        public MsvTagDataSegment()
        {
        }
        internal long absoluteOffset = 0;
        public long offset = 0;
        public UInt16 size = 0;
        public uint type = 0;
        public UInt16 language = 0;
        public UInt16 unknownProperty = 0;
        public byte[] SegmentData
        {
            get { return segmentData; }
            set
            {
            }
        }
        public byte[] segmentData = null;
    }
}
