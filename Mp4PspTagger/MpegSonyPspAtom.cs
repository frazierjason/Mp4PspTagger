using System;
using System.IO;
using System.Collections;
using MpegFiles;
using MpegAtoms;
using System.Text;
using System.Text.RegularExpressions;
using BU = ByteUtilities.ByteUtil;

namespace MpegFiles
{
    /// <summary>
    /// Summary description for MpegSonyPspAtom.
    /// </summary>
    public class MpegSonyPspAtom
    {
        public MpegSonyPspAtom(MpegAtom uuidAtom)
        {
            // TODO:  Implement a real exception handler for this
            if (MpegAtom.AtomUuidType.SonyPspTag != uuidAtom.UuidType)
                throw new ApplicationException("Atom supplied is not of type SonyPspTag.");
            this.pspTagAtom = uuidAtom;
            this.atomData = uuidAtom.Data;
            this.ms = new MemoryStream(this.atomData);
            this.br = new BinaryReader(ms);
            this.DataSegments = getDataSegments(ms, 0);
        }

        private ArrayList getDataSegments(MemoryStream ms, long endPosition)
        {
            // read and validate UUID value
            // TODO:  Implement a real exception handler for this
            if ("55534D5421D24FCEBB88695CFAC9C740" != 
                BU.ByteArrayToStringHex(br.ReadBytes(16)))
                throw new ApplicationException("Atom data supplied is not of type SonyPspTag.");
            // read atom's internal data size
            this.internalSize = BU.ReverseToUInt32(br.ReadBytes(4));
            // validate next marker in header
            // TODO:  Implement a real exception handler for this
            if ("4D544454" != BU.ByteArrayToStringHex(br.ReadBytes(4)))
                throw new ApplicationException("Atom data supplied is not of type SonyPspTag.");
            // read segment count
            this.segmentCount = BU.ReverseToUInt16(br.ReadBytes(2));
            // we should now be at the start of the first segment
            ArrayList result = new ArrayList();
            SonyPspTagDataSegment ds = null;
            while (getNextDataSegment(out ds, endPosition, null))
            {
                result.Add(ds);
            }
            // TODO: throw exception if we still have a null result
            //       or if arraylist item count != this.segmentCount
            return result;
        }

        private bool getNextDataSegment(out SonyPspTagDataSegment dataSegment, long endPosition, MpegAtom parentAtom)
        {
            // return empty object when we reach data endPosition.  If endPosition is set
            // to zero, return empty object if we reach end of data.
            if (this.br.BaseStream.Position == (0 == endPosition ? this.br.BaseStream.Length : endPosition))
            {
                dataSegment = null;
                return false;
            }
            
            // create new segment and note its offset in the atom data
            SonyPspTagDataSegment ds = new SonyPspTagDataSegment();
            ds.offset = this.br.BaseStream.Position;
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

        public static byte[] GetSegmentData(SegmentType segmentType, ArrayList dataSegments)
        {
            int foundCount = 0;
            byte[] result = null;
            foreach (SonyPspTagDataSegment ds in dataSegments)
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
                throw new ApplicationException("No " + Enum.GetName(typeof(SegmentType), segmentType) + 
                    " segment found in current MpegSonyPspAtom");
            else
                throw new ApplicationException("Multiple (" + foundCount.ToString() + 
                    ") " + Enum.GetName(typeof(SegmentType), segmentType) + 
                    " segments found in current MpegSonyPspAtom");
        }
        
        public static string GetSegmentList(ArrayList dataSegments)
        {
            string result = "";
            foreach (SonyPspTagDataSegment ds in dataSegments)
            {
                result+= ((int)ds.type).ToString("X2") + " ";
            }
            return result;
        }

        public static string GetSegmentUnicodeStringData(MpegSonyPspAtom sonyPspTag, SegmentType segmentType, ArrayList dataSegments)
        {
            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding(true, false);
            byte[] data = GetSegmentData(segmentType, dataSegments);
            return encoding.GetString(data);
        }

        public ArrayList DataSegments = null;
        private MpegAtom pspTagAtom = null;
        private byte[] atomData = null;
        private uint internalSize = 0;
        private ushort segmentCount = 0;
        public enum SegmentType
        {
            Title = 0x01,
            Date = 0x03,
            Encoder = 0x04,
            Mystery0A = 0x0A,
            Mystery0B = 0x0B
        }

        private MemoryStream ms = null;
        private BinaryReader br = null;
    }

    public class SonyPspTagDataSegment
    {
        public SonyPspTagDataSegment()
        {
        }
        public long offset = 0;
        public UInt16 size = 0;
        public uint type = 0;
        public UInt16 language = 0;
        public UInt16 unknownProperty = 0;
        public byte[] segmentData = null;
    }
}
