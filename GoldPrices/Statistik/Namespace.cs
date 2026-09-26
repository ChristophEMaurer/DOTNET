using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CMaurer.Common
{
    public enum DatabaseType
    {
        MSSqlServer,
        MSAccess,
        MySql,
        OracleXE
    }

    [SerializableAttribute]
    public class MultipleRecordsException : System.Exception
    {
        public MultipleRecordsException()
        {
        }

        public MultipleRecordsException(string message)
            : base(message)
        {
        }

        public MultipleRecordsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MultipleRecordsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }


}
