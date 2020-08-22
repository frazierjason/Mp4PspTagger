// MPEG file object.  Contains methods and properties related to accessing MPEG files.
// v0.11 written by Jason Frazier.  This is freeware.
//
// "Memory Stick" is a registered trademark of Sony Corporation.

#define DEBUG
using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using BU = ByteUtilities.ByteUtil;

namespace MsvTagger
{
	/// <summary>
	/// Summary description for MpegFile.
	/// </summary>
	internal class MpegFile : IDisposable
    {        
        
        internal MpegFile(string inputFile)
        {
            this.PathToInputFile = inputFile;
            // Existing file state
            this.PathToOutputFile = inputFile + ".out";
            if (File.Exists(this.PathToOutputFile))
            {
                this.SetupDirtyState();
            }
            else
            {
                this.SetupCleanState();
            }

            // Fetch all boxes till we get to EOF
            this.BoxTree = GetBoxes(br, 0, null);
            MpegBox temp = (MpegBox)this.BoxTree[this.BoxTree.Count - 1];
            if (temp.Type != MpegBox.BoxType.moov)
            {
                temp.Parent = null;
            }

        }

        ~MpegFile()
        {
            br.Close();
            bw.Close();
            fs.Close();
        }



        internal string PathToInputFile
        {
            get { return pathToInputFile; }
            set
            {
                pathToInputFile = value;
                if (! File.Exists(pathToInputFile))
                {
                    throw new MsvFileStreamException("The file '" + pathToInputFile + "' does not exist!");
                }
            }
        }

        internal string PathToOutputFile
        {
            get { return pathToOutputFile; }
            set
            {
                pathToOutputFile = value;
            }
        }

        private ArrayList GetBoxes(BinaryReader br, long endPosition, MpegBox parentBox)
        {
            ArrayList result = new ArrayList();
            MpegBox ma = null;
            while (GetNextBox(out ma, endPosition, parentBox))
            {
                result.Add(ma);
            }
            // TODO: throw exception if we still have a null result
            return result;
        }

        private bool GetNextBox(out MpegBox mpegBox, long endPosition, MpegBox parentBox)
        {
            // return empty object when we reach data endPosition.  If endPosition is set
            // to zero, return empty object if we reach EOF.
            if (this.br.BaseStream.Position == (0 == endPosition ? this.br.BaseStream.Length : endPosition))
            {
                mpegBox = null;
                return false;
            }
            
            // create new box, fill in the byte offset
            MpegBox ma = new MpegBox(parentBox, null, MpegBox.BoxType.Unknown, this.br.BaseStream.Position, 0);
            // now get the size and type
            ma.Size = BU.ReverseToUInt32(this.br.ReadBytes(4));  // size
            // special size values aren't handled yet
            if (ma.Size < 2)
                throw new MsvDecoderException("Special MPEG box size values are not yet supported.");
            // type
            try
            {
                ma.Type = (MpegBox.BoxType)BU.ReverseToInt32(this.br.ReadBytes(4));
            }
            catch
            {
                ma.Type = MpegBox.BoxType.Unknown;
            }

            // check the type to see if it has data or child boxes that we care about
            switch (ma.Type)
            {
                case MpegBox.BoxType.moov:
                {
                    // now that we've picked up the moov box, leave br's pointer
                    // at the start of the moov data and read in all its child boxes
                    ma.Children = GetBoxes(br, ma.Offset + ma.Size, ma);
                    break;
                }
                case MpegBox.BoxType.trak:
                {
                    // read in all its child boxes
                    ma.Children = GetBoxes(br, ma.Offset + ma.Size, ma);
                    break;
                }
                case MpegBox.BoxType.mdia:
                {
                    // read in all its child boxes
                    ma.Children = GetBoxes(br, ma.Offset + ma.Size, ma);
                    break;
                }
                case MpegBox.BoxType.minf:
                {
                    // read in all its child boxes
                    ma.Children = GetBoxes(br, ma.Offset + ma.Size, ma);
                    break;
                }
                case MpegBox.BoxType.stbl:
                {
                    // read in all its child boxes
                    ma.Children = GetBoxes(br, ma.Offset + ma.Size, ma);
                    break;
                }
                case MpegBox.BoxType.uuid:  // may have data we want
                {
                    // note current br pointer location
                    long dataStartPos = this.br.BaseStream.Position;
                    // get the 16 byte UUID value from the start of data
                    ma.UuidType = GetUuidType(this.br.ReadBytes(16));
                    // return pointer to previous location
                    this.br.BaseStream.Position = dataStartPos;
                    // if this is a uuid we care about, copy its data into the box 
                    if (!(MpegBox.BoxUuidType.Unknown == ma.UuidType ||
                          MpegBox.BoxUuidType.None == ma.UuidType))
                    {
                        // TODO: throw exception if ma.Size is bigger than an Int32 can hold
                        // read data into ma, then leave br's pointer at the end of this data/box
                        ma.Data = br.ReadBytes((int)ma.Size - 8);  // box size, less size/tag bytes
                    }
                    else
                    {
                        // don't care about this uuid, move br's pointer to the end of this box
                        this.br.BaseStream.Seek(ma.Size - 8, SeekOrigin.Current);
                        break;
                    }
                    break;
                }
                case MpegBox.BoxType.stco:  // contains list of mdat data chunk offsets
                {
                    // read data into ma, then leave br's pointer at the end of this data/box
                    ma.Data = br.ReadBytes((int)ma.Size - 8);  // box size, less size/tag bytes
                    break;
                }
                default:  // unrecognized, don't read any data from this box
                {
                    // just move br's pointer to the end of this box
                    this.br.BaseStream.Seek(ma.Size - 8, SeekOrigin.Current);
                    break;
                }
            }

            // all done, verify br's pointer is where it should be

            // return finished box
            mpegBox = ma;
            return true;
        }

        internal void SetTagUnicodeStringData(MsvBox.SegmentType type, string stringData, int lengthDelta)
        {
            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding(true, false);
            byte[] dataBytes = encoding.GetBytes(stringData + '\x00');
            this.SetTagData(type, dataBytes, lengthDelta);
        }

        internal void SetTagData(MsvBox.SegmentType type, byte[] bytes, int lengthDelta)
        {
            // verify data is not too big to fit in an MSV data segment
            if (bytes.Length > 0xFFFF)
                throw new MsvArgumentException("Size of data exceeds MSV tag capacity.");

            // verify there is only one tag to write to throughout the entire file
            ArrayList al = this.GetMsvSegmentList();
            if (al.IndexOf((uint)type) == al.LastIndexOf((uint)type) && 
                al.Contains((uint)type))
            {
                SetTagData(this.BoxTree, type, bytes, lengthDelta, false);
                return;
            }
            throw new MsvDecoderException("Number of segment matches found was not equal to one.");
        }

        private void SetTagData(ArrayList mpegBoxes, MsvBox.SegmentType type, byte[] bytes, 
                                    int lengthDelta, bool haveProcessedMdat)
        {
            foreach (MpegBox mb in mpegBoxes)
            {
                if (MpegBox.BoxType.mdat == mb.Type)
                {
                    haveProcessedMdat = true;
                }
                else if (MpegBox.BoxType.moov == mb.Type || MpegBox.BoxType.trak == mb.Type || 
                    MpegBox.BoxType.mdia == mb.Type || MpegBox.BoxType.minf == mb.Type || 
                    MpegBox.BoxType.stbl == mb.Type)
                {
                    SetTagData(mb.Children, type, bytes, lengthDelta, haveProcessedMdat);
                }
                else if (MpegBox.BoxType.stco == mb.Type)
                {
                    // if we've not yet encountered the mdat box, then it must come after the moov box
                    // and so we'll need to adjust all the chunk offsets to match their new locations
                    // if we are resizing the tag's length
                    if (!haveProcessedMdat && lengthDelta != 0)
                    {
                        // go to the start of the actual stco data
                        long dataStartPosition = mb.AbsoluteOffset() + 8;
                        this.br.BaseStream.Seek(dataStartPosition, System.IO.SeekOrigin.Begin);
                        if (0x00 != br.ReadByte())
                        {
                            throw new MsvEncoderException("Detected chunk offset box version is not supported.");
                        }
                        if (!BU.AreByteArraysEqual(new byte[]{0x00, 0x00, 0x00}, br.ReadBytes(3)))
                        {
                            throw new MsvEncoderException("Detected chunk offset box flags are not supported.");
                        }
                        UInt32 chunkCount = BU.ReverseToUInt32(this.br.ReadBytes(4));
                        UInt32 tempChunkOffset = 0;
                        for (int i = 0; i < chunkCount; i++)
                        {
                            // get next chunk offset
                            tempChunkOffset = BU.ReverseToUInt32(this.br.ReadBytes(4));
                            // back pointer up, then rewrite chunk offset to reflect lengthDelta
                            this.br.BaseStream.Seek(-4, System.IO.SeekOrigin.Current);
                            this.bw.Write(BU.ReverseToBytes((UInt32)(tempChunkOffset + lengthDelta)));
                        }
                    }

                }
                else if (MpegBox.BoxUuidType.MsvTag == mb.UuidType)
                {
                    MsvBox pb = new MsvBox(mb);
                    foreach (MsvTagDataSegment ds in pb.DataSegments)
                    {
                        if ((uint)type == ds.type)
                        {
                            // go to the start of the actual data within ds
                            // this assumes that the structure preceding the data is always
                            // consistent between all segments having write support
                            long dataStartPosition = ds.absoluteOffset + 10;
                            this.br.BaseStream.Seek(dataStartPosition, System.IO.SeekOrigin.Begin);
                            // verify that we know what data we're about to overwrite
                            byte[] compare = this.br.ReadBytes(ds.SegmentData.Length);
                            if (!BU.AreByteArraysEqual(compare, ds.SegmentData))
                            {
                                throw new MsvEncoderException(
                                    "The filestream position was invalid while writing tag data.");
                            }

                            if (bytes.Length != ds.SegmentData.Length)
                            {
                                // we have to grow or shrink the file and adjust its size markers
                                long moveByteCount = this.bw.BaseStream.Length - this.bw.BaseStream.Position;
                                int moveBlockSize = 131072;  // move in 128kb blocks
                                int partialBlockSize = (int)(moveByteCount % moveBlockSize);  // remaining partial block
                                // verify that the supplied lengthDelta value is correct
                                // the length delta is a negative number when shrinking a tag
                                if (lengthDelta != bytes.Length - ds.SegmentData.Length)
                                {
                                    throw new MsvEncoderException(
                                        "lengthDelta supplied did not match old tag length found.");
                                }
                                byte[] buffer;  // buffer for moving blocks

                                if (lengthDelta > 0)  // new tag is too big, so grow the file
                                {   
                                    // grow the file the needed number of bytes before moving data out
                                    this.br.BaseStream.SetLength(this.bw.BaseStream.Length + lengthDelta);
                                
                                    // need to shift subsequent data further down the file
                                    if (moveByteCount >= moveBlockSize)
                                    {
                                        // we have at least one full block to move
                                        for (int i = 0; i < moveByteCount / moveBlockSize; i++)
                                        {
                                            // move pointer to beginning of next block to be moved
                                            // blocks are moved in reverse, from EOF backward
                                            this.br.BaseStream.Seek((moveBlockSize * (i + 1) + lengthDelta) * -1,
                                                System.IO.SeekOrigin.End);
                                            // read in a block
                                            buffer = br.ReadBytes(moveBlockSize);
                                            // move pointer backwards to start of the block, minus the grow size
                                            this.br.BaseStream.Seek((moveBlockSize - lengthDelta) * -1,
                                                System.IO.SeekOrigin.Current);
                                            this.bw.Write(buffer);
                                            buffer = null;                                            
                                        }
                                    }
                                    // move any remaining partial block
                                    if (0 != partialBlockSize)
                                    {
                                        // move pointer to start of partial block
                                        this.br.BaseStream.Seek(dataStartPosition + ds.segmentData.Length,
                                            System.IO.SeekOrigin.Begin);
                                        // read in the partial block
                                        buffer = br.ReadBytes(partialBlockSize);
                                        // move pointer backwards to start of partial block, minus the grow size
                                        this.br.BaseStream.Seek((partialBlockSize - lengthDelta) * -1,
                                            System.IO.SeekOrigin.Current);
                                        this.bw.Write(buffer);
                                        buffer = null;
                                    }
                                }
                                else if (lengthDelta < 0)  // new tag is too small, so shrink the file
                                {
                                    
                                    // need to shift subsequent data further up the file
                                    // move pointer to beginning of first block
                                    this.br.BaseStream.Seek(dataStartPosition + ds.segmentData.Length,
                                        System.IO.SeekOrigin.Begin);

                                    if (moveByteCount >= moveBlockSize)
                                    {
                                        // we have at least one full block to move
                                        for (int i = 0; i < moveByteCount / moveBlockSize; i++)
                                        {
                                            // read in a block
                                            buffer = br.ReadBytes(moveBlockSize);
                                            // move pointer backwards to start of the block, plus the shrink size
                                            // remember that lengthDelta is a negative number for shrink operations
                                            this.br.BaseStream.Seek((moveBlockSize - lengthDelta) * -1,
                                                System.IO.SeekOrigin.Current);
                                            this.bw.Write(buffer);
                                            buffer = null;
                                            // block is moved, so advance pointer to the beginning of the
                                            // next block
                                            this.br.BaseStream.Seek(lengthDelta * -1,
                                                System.IO.SeekOrigin.Current);
                                        }
                                    }
                                    // move any remaining partial block
                                    if (0 != partialBlockSize)
                                    {
                                        // pointer should already be at start of partial block
                                        // read in the partial block
                                        buffer = br.ReadBytes(partialBlockSize);
                                        // move pointer backwards to start of the block, plus the shrink size
                                        // remember that lengthDelta is a negative number for shrink operations
                                        this.br.BaseStream.Seek((partialBlockSize - lengthDelta) * -1,
                                            System.IO.SeekOrigin.Current);
                                        this.bw.Write(buffer);
                                        buffer = null;
                                    }   
                                    // shrink the file the needed number of bytes after moving data in
                                    this.br.BaseStream.SetLength(this.bw.BaseStream.Length + lengthDelta);
                                }

                                // now fix up all the size markers
                                UInt32 temp32 = 0;
                                UInt16 temp16 = 0;
                                // this tag's size marker
                                this.br.BaseStream.Seek(ds.absoluteOffset, System.IO.SeekOrigin.Begin);
                                temp16 = BU.ReverseToUInt16(this.br.ReadBytes(2)); // tag size
                                this.br.BaseStream.Seek(-2, System.IO.SeekOrigin.Current);
                                if (temp16 != ds.size)
                                {
                                    throw new MsvEncoderException(
                                        "The filestream position was invalid while writing tag size.");
                                }
                                this.bw.Write(BU.ReverseToBytes((UInt16)(temp16 + lengthDelta)));
                                // the rest of the size markers we must first read and then change
                                // incase for some reason we're changing them multiple times

                                // both of this MSV box's external and internal size markers 
                                this.br.BaseStream.Seek(mb.AbsoluteOffset(), System.IO.SeekOrigin.Begin);
                                temp32 = BU.ReverseToUInt32(this.br.ReadBytes(4)); // UUID box size
                                this.br.BaseStream.Seek(-4, System.IO.SeekOrigin.Current);
                                this.bw.Write(BU.ReverseToBytes((UInt32)(temp32 + lengthDelta)));
                                this.br.BaseStream.Seek(20, System.IO.SeekOrigin.Current);
                                temp32 = BU.ReverseToUInt32(this.br.ReadBytes(4)); // MSV internalSize
                                this.br.BaseStream.Seek(-4, System.IO.SeekOrigin.Current);
                                this.bw.Write(BU.ReverseToBytes((UInt32)(temp32 + lengthDelta)));

                                // size markers of this box's parent and all the parent's parents
                                MpegBox tempBox = mb.Parent;
                                while (tempBox != null)
                                {
                                    this.br.BaseStream.Seek(tempBox.AbsoluteOffset(),
                                        System.IO.SeekOrigin.Begin);
                                    temp32 = BU.ReverseToUInt32(this.br.ReadBytes(4)); // tempBox size
                                    this.br.BaseStream.Seek(-4, System.IO.SeekOrigin.Current);
                                    this.bw.Write(BU.ReverseToBytes((UInt32)(temp32 + lengthDelta)));

                                    tempBox = tempBox.Parent;
                                }
                            }

                            // set reader back to start of tag data we wish to overwrite
                            this.br.BaseStream.Seek(dataStartPosition, System.IO.SeekOrigin.Begin);
                            // now write the new data over the old data
                            this.bw.Write(bytes);
                            this.bw.BaseStream.Seek(dataStartPosition, System.IO.SeekOrigin.Begin);
                            // verify it worked
                            byte[] verify = this.br.ReadBytes(bytes.Length);
                            if (!BU.AreByteArraysEqual(verify, bytes))
                            {
                                throw new MsvEncoderException("The data write failed verification.");
                            }
                            // make sure all the data changes get flushed to disk
                            this.bw.Flush();                                
                        }
                    }
                }
            }
            return;
        }
        
        internal ArrayList GetMsvSegmentList()
        {
            ArrayList result = new ArrayList();
            GetMsvSegmentList(this.BoxTree, ref result);
            return result;
        }

        private void GetMsvSegmentList(ArrayList mpegBoxes, ref ArrayList msvSegmentList)
        {
            foreach (MpegBox mb in mpegBoxes)
            {
                if (MpegBox.BoxType.moov == mb.Type || MpegBox.BoxType.trak == mb.Type)
                {
                    GetMsvSegmentList(mb.Children, ref msvSegmentList);
                }
                else if (MpegBox.BoxUuidType.MsvTag == mb.UuidType)
                {
                    MsvBox pb = new MsvBox(mb);
                    foreach (uint u in MsvBox.GetBoxSegmentList(pb.DataSegments))
                    {
                        msvSegmentList.Add(u);
                    }
                }
            }
        }

        private void SetupCleanState()
        {
            this.fs = new FileStream(this.pathToInputFile, System.IO.FileMode.Open);
            this.bw = new BinaryWriter(fs);
            this.br = new BinaryReader(fs);
        }

        private void SetupDirtyState()
        {
            this.SetupCleanState();
        }

        internal MpegBox.BoxUuidType GetUuidType(byte[] uuidBytes)
        {
            string s = BU.ByteArrayToStringHex(uuidBytes);
            MpegBox.BoxUuidType result = MpegBox.BoxUuidType.Unknown;
            switch (s)
            {
                case "55534D5421D24FCEBB88695CFAC9C740":  // MsvTag
                {
                    result = MpegBox.BoxUuidType.MsvTag;
                    break;
                }   
                case "C0EDBABEA7EBADF00DA7DEADBEEFCAFE":  // MyCustomTag, registered (see below)
                {
                    // NOTE: This is a registered UUID, do not use it for your
                    //       own purposes.  You can generate and register one for
                    //       free at:  http://www.itu.int/ITU-T/asn1/uuid.html
                    result = MpegBox.BoxUuidType.MyCustomTag;
                    break;
                }
                default:
                {
                    // leave the return initialized as Unknown
                    break;
                }
            } 
            return result;
        }

        private string pathToInputFile;
        private string pathToOutputFile;
        private FileStream fs = null;
        private BinaryReader br = null;
        private BinaryWriter bw = null;
        internal ArrayList BoxTree = null;
        #region IDisposable Members

        public void Dispose()
        {
            br.Close();
            bw.Close();
            fs.Close();
        }

        #endregion
    }

}
