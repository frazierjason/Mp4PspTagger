// MsvException classes.  Contains exception classes used by MSVTagger.
// v0.11 written by Jason Frazier.  This is freeware.
//
// "Memory Stick" is a registered trademark of Sony Corporation.

using System;

namespace MsvTagger
{
    /// <summary>
    /// Summary description for MsvException.
    /// </summary>
    public class MsvException : ApplicationException
    {
        public MsvException() : base()
        {}

        public MsvException(string message) : base(message)
        {}

        public MsvException(string message, Exception innerException) : base(message, innerException)
        {}
    }

    /// <summary>
    /// Summary description for MsvArgumentException.
    /// </summary>
    public class MsvArgumentException : ArgumentException
    {
        public MsvArgumentException() : base()
        {}

        public MsvArgumentException(string message) : base(message)
        {}

        public MsvArgumentException(string message, Exception innerException) : base(message, innerException)
        {}                            
    }

    /// <summary>
    /// Summary description for MsvTagNotFoundException.
    /// </summary>
    public class MsvTagNotFoundException : ArgumentException
    {
        public MsvTagNotFoundException() : base()
        {}

        public MsvTagNotFoundException(string message) : base(message)
        {}

        public MsvTagNotFoundException(string message, Exception innerException) : base(message, innerException)
        {}                            
    }

    /// <summary>
    /// Summary description for MsvDecoderException.
    /// </summary>
    public class MsvDecoderException : ArgumentException
    {
        public MsvDecoderException() : base()
    {}

        public MsvDecoderException(string message) : base(message)
    {}

        public MsvDecoderException(string message, Exception innerException) : base(message, innerException)
    {}                            
    }

    /// <summary>
    /// Summary description for MsvEncoderException.
    /// </summary>
    public class MsvEncoderException : ArgumentException
    {
        public MsvEncoderException() : base()
        {}

        public MsvEncoderException(string message) : base(message)
        {}

        public MsvEncoderException(string message, Exception innerException) : base(message, innerException)
        {}                            
    }

    /// <summary>
    /// Summary description for MsvArithmeticException.
    /// </summary>
    public class MsvArithmeticException : ArgumentException
    {
        public MsvArithmeticException() : base()
        {}

        public MsvArithmeticException(string message) : base(message)
        {}

        public MsvArithmeticException(string message, Exception innerException) : base(message, innerException)
        {}                            
    }

    /// <summary>
    /// Summary description for MsvFileStreamException.
    /// </summary>
    public class MsvFileStreamException : ArgumentException
    {
        public MsvFileStreamException() : base()
        {}

        public MsvFileStreamException(string message) : base(message)
        {}

        public MsvFileStreamException(string message, Exception innerException) : base(message, innerException)
        {}                            
    }

}
