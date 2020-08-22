using System;
using System.Collections;
using MpegFiles;
using MpegAtoms;
using System.Text;
using System.Text.RegularExpressions;

namespace Mp4PspTagger
{
	/// <summary>
	/// Summary description for Mp4PspTagger.
	/// </summary>
	public class Mp4PspTagger
	{
		public Mp4PspTagger(string inputFile)
		{
            mpegFile = new MpegFile(inputFile);

            foreach (MpegAtom ma in mpegFile.AtomTree)
            {
                getPspTags(ma, ref mp4PspAtoms);
            }
		}

        private void getPspTags(MpegAtom mpegAtom, ref ArrayList pspAtoms)
        {
            if (mpegAtom.UuidType == MpegAtom.AtomUuidType.SonyPspTag)
            {
                pspAtoms.Add(mpegAtom);
            }
            if (mpegAtom.HasChildren())
            {
                foreach (MpegAtom ma in mpegAtom.Children)
                {
                    getPspTags(ma, ref pspAtoms);
                }
            }
        }

        public string GetTagTitle()
        {
            return this.GetTagStringData(MpegSonyPspAtom.SegmentType.Title);
        }

        public string GetTagDate()
        {
            return this.GetTagStringData(MpegSonyPspAtom.SegmentType.Date);
        }

        
        public string GetTagEncoder()
        {
            return this.GetTagStringData(MpegSonyPspAtom.SegmentType.Encoder);
        }

        private string GetTagStringData(MpegSonyPspAtom.SegmentType type)
        {
            string result = "";
            int foundCount = 0;
            foreach (MpegAtom ma in mp4PspAtoms)
            {
                MpegSonyPspAtom pa = new MpegSonyPspAtom(ma);
                foundCount++;
                try
                {
                    result = MpegSonyPspAtom.GetSegmentUnicodeStringData(pa, 
                        type, pa.DataSegments);
                }
                catch
                {
                    foundCount--;
                }
            }
            if (foundCount == 1)
            {
                return result;
            }
            else if (foundCount > 1)
            {
                 throw new ApplicationException("Multiple PSP " + 
                    Enum.GetName(typeof(MpegSonyPspAtom.SegmentType), type) + " tags found.");
                //System.Windows.Forms.MessageBox.Show("Multiple PSP " + 
                //    Enum.GetName(typeof(MpegSonyPspAtom.SegmentType), type) + " tags found.");
                // return "<MULTIPLE " + Enum.GetName(typeof(MpegSonyPspAtom.SegmentType), type).ToUpper() + " FOUND>";
            }
            else
            {
                throw new ApplicationException("No PSP " + 
                    Enum.GetName(typeof(MpegSonyPspAtom.SegmentType), type) + " tag found.");
                // System.Windows.Forms.MessageBox.Show("No PSP " + 
                //     Enum.GetName(typeof(MpegSonyPspAtom.SegmentType), type) + " tag found.");
                // return "<" + Enum.GetName(typeof(MpegSonyPspAtom.SegmentType), type).ToUpper() + " NOT FOUND>";
            }
        }

        
        public string GetTagDataSegmentList()
        {
            string result = "";
            foreach (MpegAtom ma in mp4PspAtoms)
            {
                MpegSonyPspAtom pa = new MpegSonyPspAtom(ma);
                try
                {
                    result += MpegSonyPspAtom.GetSegmentList(pa.DataSegments) + " ";
                }
                catch
                {}
            }
            return result;
        }

        private ArrayList mp4PspAtoms = new ArrayList();
        private MpegFile mpegFile = null;
	}
}
