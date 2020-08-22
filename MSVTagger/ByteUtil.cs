// Byte Utilities class.  Contains helper methods for manipulating bytes and byte arrays.
// v0.11 written by Jason Frazier.  This is freeware.

using System;

namespace ByteUtilities
{
	/// <summary>
	/// Summary description for ByteUtil.
	/// </summary>
	public class ByteUtil
	{
		public ByteUtil()
		{
			//
			// TODO: Add constructor logic here
			//
		}        
        
        public static byte[] ReverseToBytes(UInt32 u)
        {
            byte[] temp = BitConverter.GetBytes(u);
            Array.Reverse(temp);
            return temp;
        }
  
        public static byte[] ReverseToBytes(UInt16 u)
        {
            byte[] temp = BitConverter.GetBytes(u);
            Array.Reverse(temp);
            return temp;
        }

        public static int ReverseToInt32(byte[] byteArray)
        {
            byte[] temp = byteArray;  // create copy so we don't modify original
            // TODO: throw exception if b.Length > 4
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }

        public static byte[] ReverseBytes(byte[] byteArray)
        {
            byte[] result = byteArray;  // create copy so we don't modify original
            Array.Reverse(result);
            return result;
        }

        public static bool AreByteArraysEqual(byte[] bytesA, byte[] bytesB)
        {
            bool result = false;
            if (bytesA.Length == bytesB.Length)
            {
                int i=0;
                while ((i < bytesA.Length) && (bytesA[i] == bytesB[i]))
                {
                    i += 1;
                }
                if (i == bytesA.Length)
                {
                    result = true;
                }
            }
            return result;
        }
        public static uint ReverseToUInt32(byte[] byteArray)
        {
            byte[] temp = byteArray;  // create copy so we don't modify original
            // TODO: throw exception if b.Length > 4
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp, 0);
        }
        
        public static ushort ReverseToUInt16(byte[] byteArray)
        {
            byte[] temp = byteArray;  // create copy so we don't modify original
            // TODO: throw exception if b.Length > 2
            Array.Reverse(temp);
            return BitConverter.ToUInt16(temp, 0);
        }

        public static uint FourCharsToUInt32(string fourChars)
        {
            if (4 != fourChars.Length)
                throw new MsvTagger.MsvArithmeticException("Four characters were not provided for conversion to UInt32.");
            return BitConverter.ToUInt32(System.Text.ASCIIEncoding.ASCII.GetBytes(fourChars), 0);
        }

        public static string ByteArrayToStringHex(byte[] bytes)
        {
            string s = null;
            foreach (byte b in bytes)
                s += b.ToString("X2").ToUpper();
            return s;
        }
	}
}
