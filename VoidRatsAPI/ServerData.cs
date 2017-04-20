using System;
using System.Collections.Generic;
using System.Text;

namespace VoidRatsAPI
{
    /// <summary>
    /// A piece of data held on the server to be kept in sync locally
    /// </summary>
    public abstract class ServerData
    {
        private int type = 0, id = 0;
        private DateTime lastRefreshed = DateTime.MinValue;

        /// <summary>
        /// Lists the data types available
        /// </summary>
        public enum DataType : int
        {
            Ability = 1,
            Resource = 2,
            WorldInfo = 1000
        }

        #region Load data
        #region Overloads
        /// <summary>
        /// Read in the data for this piece of server data from a local data file, and ensures the correct data type
        /// </summary>
        /// <param name="Reader">StreamReader to continue reading data from</param>
        /// <param name="CurrentLine">Current line of data to read</param>
        /// <param name="DataType">Required data type for this piece of server data</param>
        /// <returns>True if we read data</returns>
        protected bool loadData(System.IO.StreamReader Reader, string CurrentLine, DataType DataType)
        {
            if (!loadData(Reader, CurrentLine))
                return false;
            else
            {
                if (type != (int)DataType)
                    throw new InvalidServerDataException();
                return true;
            }
        }

        /// <summary>
        /// Read in the data for this piece of server data from a local data file, and ensures the correct data type
        /// </summary>
        /// <param name="Reader">StreamReader to continue reading data from</param>
        /// <param name="CurrentLine">Current line of data to read</param>
        /// <param name="DataType">Required data type for this piece of server data</param>
        /// <returns>True if we read data</returns>
        protected bool loadData(System.IO.StreamReader Reader, string CurrentLine, int DataType)
        {
            if (!loadData(Reader, CurrentLine))
                return false;
            else
            {
                if (type != DataType)
                    throw new InvalidServerDataException();
                return true;
            }
        }
        #endregion

        /// <summary>
        /// Read in the data for this piece of server data from a local data file
        /// </summary>
        /// <param name="Reader">StreamReader to continue reading data from</param>
        /// <param name="CurrentLine">Current line of data to read</param>
        /// <returns>True if we read data</returns>
        protected bool loadData(System.IO.StreamReader Reader, string CurrentLine)
        {
            if (CurrentLine.ToUpper() == "[HEADER]")
            {
                type = 0;
                id = 0;
                lastRefreshed = DateTime.MinValue;

                CurrentLine = Reader.ReadLine();
                while (CurrentLine != null && CurrentLine.ToUpper() != "[ENDHEADER]")
                {
                    if (CurrentLine.ToUpper().StartsWith("TYPE="))
                    {
                        if (!int.TryParse(CurrentLine.Substring("TYPE=".Length), out type))
                            throw new InvalidServerDataException();
                    }
                    else if (CurrentLine.ToUpper().StartsWith("ID="))
                    {
                        if (!int.TryParse(CurrentLine.Substring("ID=".Length), out id))
                            throw new InvalidServerDataException();
                    }
                    else if (CurrentLine.ToUpper().StartsWith("LASTREFRESHED="))
                    {
                        long ticks = 0;
                        if (long.TryParse(CurrentLine.Substring("LASTREFRESHED=".Length), out ticks))
                            lastRefreshed = new DateTime(ticks);
                        else
                            throw new InvalidServerDataException();
                    }
                    else if (CurrentLine.Trim().Length > 0)
                    {
                        throw new InvalidServerDataException();
                    }

                    CurrentLine = Reader.ReadLine();
                }

                if (type == 0 || id == 0 )
                    throw new InvalidServerDataException();

                return true;
            }
            else
                return false;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Gets the data type of this piece of data
        /// </summary>
        public int Type { get { return type; } }
        /// <summary>
        /// Gets the unique ID of this piece of data
        /// </summary>
        public int ID { get { return id; } }
        /// <summary>
        /// Gets the last time (UTC) this piece of data was refreshed from the server
        /// </summary>
        public DateTime LastRefreshed { get { return lastRefreshed; } }
        #endregion

        #region Exceptions
        /// <summary>
        /// Thrown when no server data is found in what should be a server data file
        /// </summary>
        public class NoServerDataException : Exception { }
        /// <summary>
        /// Thrown when there is invalid server data in a data file
        /// </summary>
        public class InvalidServerDataException : Exception { }
        #endregion
    }
}
