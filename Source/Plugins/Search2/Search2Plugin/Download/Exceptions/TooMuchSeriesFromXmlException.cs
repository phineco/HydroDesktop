﻿using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Search.Download.Exceptions
{
    class TooMuchSeriesFromXmlException : Exception
    {
        private const string DEFAULT_MESSAGE = "Too much series in xml file. Expected: 1.";

        public TooMuchSeriesFromXmlException()
            : this(DEFAULT_MESSAGE)
        {

        }
        public TooMuchSeriesFromXmlException(Exception inner)
            : this(DEFAULT_MESSAGE, inner)
        {

        }
        public TooMuchSeriesFromXmlException(string message)
            : this(message, null)
        {

        }
        public TooMuchSeriesFromXmlException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected TooMuchSeriesFromXmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}