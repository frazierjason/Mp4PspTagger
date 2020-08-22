// MsvTagger object.  Contains methods and properties for accessing
// tags in a Memory Stick Video MP4 file.
// v0.11 written by Jason Frazier.  This is freeware.
//
// "Memory Stick" is a registered trademark of Sony Corporation.

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace MsvTagger
{
	/// <summary>
	/// Summary description for MsvTagger.
	/// </summary>
	public class MsvTags : IDisposable
	{
        /// <summary>
        /// Instance of Memory Stick Video Tagger.
        /// </summary>
        /// <param name="inputFile">Path to input .MP4 file.</param>
		public MsvTags(string inputFile)
		{
            mpegFile = new MpegFile(inputFile);

            foreach (MpegBox ma in mpegFile.BoxTree)
            {
                getMsvTags(ma, ref msvBoxes);
            }
		}

        private void getMsvTags(MpegBox mpegBox, ref ArrayList msvBoxes)
        {
            if (mpegBox.UuidType == MpegBox.BoxUuidType.MsvTag)
            {
                msvBoxes.Add(mpegBox);
            }
            if (mpegBox.HasChildren())
            {
                foreach (MpegBox ma in mpegBox.Children)
                {
                    getMsvTags(ma, ref msvBoxes);
                }
            }
        }

        /// <summary>
        /// Gets Memory Stick Video title
        /// </summary>
        /// <returns>Returns MSV title string</returns>
        public string GetTagTitle()
        {
            return this.GetTagStringData(MsvBox.SegmentType.Title);
        }
        
        /// <summary>
        /// Gets Memory Stick Video Date
        /// </summary>
        /// <returns>Returns MSV date string</returns>
        public string GetTagDate()
        {
            return this.GetTagStringData(MsvBox.SegmentType.Date);
        }
        
        /// <summary>
        /// Gets Memory Stick Video encoder used to make file
        /// </summary>
        /// <returns>Returns MSV encoder string</returns>
        public string GetTagEncoder()
        {
            return this.GetTagStringData(MsvBox.SegmentType.Encoder);
        }

        public void SetTagTitle(string s)
        {
            SetTagStringData(MsvBox.SegmentType.Title, s);
        }
        
        public void SetTagDate(string s)
        {
            SetTagStringData(MsvBox.SegmentType.Date, s);
        }
        
        public void SetTagEncoder(string s)
        {
            SetTagStringData(MsvBox.SegmentType.Encoder, s);
        }

        internal void SetTagStringData(MsvBox.SegmentType type, string s)
        {
            // first get the old tag in order to verify its presence and length
            string oldTag = this.GetTagStringData(type);
            // lengthDelta is size difference in double-bytes between s and null-stripped oldTag
            int lengthDelta = (s.Length - (oldTag.Length - 1)) * 2;
            mpegFile.SetTagUnicodeStringData(type, s, lengthDelta);
            return;
        }

        private string GetTagStringData(MsvBox.SegmentType type)
        {
            string result = "";
            int foundCount = 0;
            foreach (MpegBox ma in msvBoxes)
            {
                MsvBox pa = new MsvBox(ma);
                foundCount++;
                try
                {
                    result = MsvBox.GetSegmentUnicodeStringData(type, pa.DataSegments);
                }
                catch (MsvTagNotFoundException)
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
                 throw new MsvDecoderException("Multiple MSV " + 
                    Enum.GetName(typeof(MsvBox.SegmentType), type) + " tags found.");
            }
            else
            {
                throw new MsvTagNotFoundException("No MSV " + 
                    Enum.GetName(typeof(MsvBox.SegmentType), type) + " tag found.");
            }
        }

        /// <summary>
        /// Gets Memory Stick Video data segment list
        /// </summary>
        /// <returns>Returns a list of the MSV data segments found in the MP4 file</returns>
        public string GetTagSegmentListString()
        {
            string result = "";
            foreach (uint u in mpegFile.GetMsvSegmentList())
            {
                result+= u.ToString("X2") + " ";
            }
            return result;
        }

        private ArrayList msvBoxes = new ArrayList();
        private MpegFile mpegFile = null;
        #region IDisposable Members

        public void Dispose()
        {
            mpegFile.Dispose();
        }

        #endregion
    }
}
