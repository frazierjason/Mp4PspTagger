using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using MpegAtoms;
using BU = ByteUtilities.ByteUtil;

namespace MpegFiles
{
	/// <summary>
	/// Summary description for MpegFile.
	/// </summary>
	public class MpegFile
    {        
        
        public MpegFile(string inputFile)
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

            // Fetch all atoms till we get to EOF
            this.AtomTree = GetAtoms(br, 0);   
        }

        public string PathToInputFile
        {
            get { return pathToInputFile; }
            set
            {
                pathToInputFile = value;
                if (! File.Exists(pathToInputFile))
                {
                    throw new ApplicationException("'" + pathToInputFile + "' does not exist!");
                }
            }
        }

        public string PathToOutputFile
        {
            get { return pathToOutputFile; }
            set
            {
                pathToOutputFile = value;
            }
        }

        private ArrayList GetAtoms(BinaryReader br, long endPosition)
        {
            ArrayList result = new ArrayList();
            MpegAtom ma = null;
            while (GetNextAtom(out ma, endPosition, null))
            {
                result.Add(ma);
            }
            // TODO: throw exception if we still have a null result
            return result;
        }

        private bool GetNextAtom(out MpegAtom mpegAtom, long endPosition, MpegAtom parentAtom)
        {
            // return empty object when we reach data endPosition.  If endPosition is set
            // to zero, return empty object if we reach EOF.
            if (this.br.BaseStream.Position == (0 == endPosition ? this.br.BaseStream.Length : endPosition))
            {
                mpegAtom = null;
                return false;
            }
            
            // create new atom, fill in the byte offset
            MpegAtom ma = new MpegAtom(null, null, MpegAtom.AtomType.Unknown, this.br.BaseStream.Position, 0);
            // now get the size and type
            ma.Size = BU.ReverseToUInt32(this.br.ReadBytes(4));  // size
            // type
            try
            {
                ma.Type = (MpegAtom.AtomType)BU.ReverseToInt32(this.br.ReadBytes(4));
            }
            catch
            {
                ma.Type = MpegAtom.AtomType.Unknown;
            }
            /*
            switch (BU.ReverseToInt32(this.br.ReadBytes(4)))  // type
            {
                case 0x6D6F6F76:    ma.Type = MpegAtom.AtomType.moov;     break;   // "moov"
                case 0x75756964:    ma.Type = MpegAtom.AtomType.uuid;     break;   // "uuid"
                default:            ma.Type = MpegAtom.AtomType.Unknown;  break;   // uninteresting to us
            }
            */

            // check the type to see if it has data or child atoms that we care about
            switch (ma.Type)
            {
                case MpegAtom.AtomType.moov:
                {
                    // now that we've picked up the moov atom, leave br's pointer
                    // at the start of the moov data and read in all its child atoms
                    ma.Children = GetAtoms(br, ma.Offset + ma.Size);
                    // TODO: verify br's pointer is now at end of moov data
                    break;
                }
                case MpegAtom.AtomType.trak:
                {
                    // now that we've picked up the moov atom, leave br's pointer
                    // at the start of the moov data and read in all its child atoms
                    ma.Children = GetAtoms(br, ma.Offset + ma.Size);
                    // TODO: verify br's pointer is now at end of moov data
                    break;
                }
                case MpegAtom.AtomType.uuid:  // may have data we want
                {
                    // note current br pointer location
                    long dataStartPos = this.br.BaseStream.Position;
                    // get the 16 byte UUID value from the start of data
                    ma.UuidType = GetUuidType(this.br.ReadBytes(16));
                    // return pointer to previous location
                    this.br.BaseStream.Position = dataStartPos;
                    // if this is a uuid we care about, copy its data into the atom 
                    if (!(MpegAtom.AtomUuidType.Unknown == ma.UuidType ||
                          MpegAtom.AtomUuidType.None == ma.UuidType))
                    {
                        // TODO: throw exception if ma.Size is bigger than an Int32 can hold
                        // read data into ma, then leave br's pointer at the end of this data/atom
                        ma.Data = br.ReadBytes((int)ma.Size - 8);  // atom size, less size/tag bytes
                    }
                    else
                    {
                        // don't care about this uuid, move br's pointer to the end of this atom
                        this.br.BaseStream.Seek(ma.Size - 8, SeekOrigin.Current);
                        break;
                    }
                    break;
                }
                default:  // unrecognized, don't read any data from this atom
                {
                    // just move br's pointer to the end of this atom
                    this.br.BaseStream.Seek(ma.Size - 8, SeekOrigin.Current);
                    break;
                }
            }

            // all done, verify br's pointer is where it should be

            // return finished atom
            mpegAtom = ma;
            return true;
        }

        private void SetupCleanState()
        {
            /*  Commented out until writing portion is working
            try
            {
                if (true) // opening new file for writing for now
                {
                    this.bw = new BinaryWriter(File.Open(this.PathToOutputFile,
                        FileMode.CreateNew,
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite));
                }
                else  // later on implement opening existing file for in-place modification
                {
                    this.bw = new BinaryWriter(File.OpenWrite(this.pathToInputFile));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Please provide a valid output filename", ex);
            }
            */
            try
            {
                /*
                //this.br = new BinaryReader(new FileStream(this.PathToInputFile,
                    FileMode.CreateNew,
                    FileAccess.Read,
                    FileShare.Read,
                    8192,
                    false));
                */
                this.br = new BinaryReader(File.OpenRead(this.pathToInputFile));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Please provide a valid input filename", ex);
            }
        }

        private void SetupDirtyState()
        {
            // TODO: throw exception to get calling code to confirm overwriting file
            // for now, just delete the old output file
            File.Delete(this.PathToOutputFile);
            // if the delete failed, prepend a char to filename to keep from overwriting old file
            while ((File.Exists(this.PathToOutputFile)))
                this.PathToOutputFile = "~" + this.PathToOutputFile;
            this.SetupCleanState();
        }

        public MpegAtom.AtomUuidType GetUuidType(byte[] uuidBytes)
        {
            string s = BU.ByteArrayToStringHex(uuidBytes);
            MpegAtom.AtomUuidType result = MpegAtom.AtomUuidType.Unknown;
            switch (s)
            {
                case "55534D5421D24FCEBB88695CFAC9C740":  // SonyPspTag
                {
                    result = MpegAtom.AtomUuidType.SonyPspTag;
                    break;
                }   
                case "C0EDBABEA7EBADF00DA7DEADBEEFCAFE":  // MyCustomTag, registered (see below)
                {
                    // NOTE: This is a registered UUID, do not use it for your
                    //       own purposes.  You can generate and register one for
                    //       free at:  http://www.itu.int/ITU-T/asn1/uuid.html
                    result = MpegAtom.AtomUuidType.MyCustomTag;
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
        private BinaryReader br = null;
        private BinaryWriter bw = null;
        public ArrayList AtomTree = null;

    }

}
