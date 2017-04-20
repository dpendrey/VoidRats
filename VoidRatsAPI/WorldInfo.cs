using System;
using System.Collections.Generic;
using System.Text;

namespace VoidRatsAPI
{
    /// <summary>
    /// A piece of world building information (often in the form of a story) held in the game
    /// </summary>
    public class WorldInfo : ServerData
    {
        private string name;
        private string desc;
        private int parent;

        #region Load data
        internal void loadData(string Filename)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(Filename);

            string curLine = reader.ReadLine();
            bool serverDataRead = false;

            name = desc  = string.Empty;

            while (curLine != null)
            {
                if (base.loadData(reader, curLine, DataType.WorldInfo))
                    serverDataRead = true;
                else if (curLine.ToUpper().StartsWith("NAME="))
                    name = curLine.Substring("NAME=".Length);
                else if (curLine.ToUpper().StartsWith("PARENT="))
                    parent = int.Parse(curLine.Substring("PARENT=".Length));
                else if (curLine.ToUpper() == "[DESCRIPTION]")
                    desc = reader.ReadToEnd();

                curLine = reader.ReadLine();
            }

            if (!serverDataRead)
                throw new NoServerDataException();
            if ( name.Length == 0 || desc.Length == 0 )
                throw new InvalidWorldInfoData();
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Gets the name of this world info
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Gets the description of this world info
        /// </summary>
        public string Description { get { return desc; } }

        /// <summary>
        /// Gets the parent of this world info
        /// </summary>
        public int Parent{ get { return parent; } }
        #endregion

        #region Exceptions
        /// <summary>
        /// Thrown when invalid data is found upon reading an world info data file
        /// </summary>
        public class InvalidWorldInfoData : Exception { }
        #endregion

        #region List
        private static Dictionary<int, WorldInfo> list = new Dictionary<int, WorldInfo>();

        public static WorldInfo GetWorldInfo(int ID)
        {
            WorldInfo retVal;
            lock (list)
                retVal = list[ID];
            return retVal;
        }

        public static WorldInfo[] ListWorldInfos()
        {
            List<WorldInfo> retVal = new List<WorldInfo>();
            lock (list)
                foreach (KeyValuePair<int, WorldInfo> worldInfo in list)
                    retVal.Add(worldInfo.Value);
            return retVal.ToArray();
        }

        public static WorldInfo[] ListChildren(int Parent)
        {
            List<WorldInfo> retVal = new List<WorldInfo>();
            lock (list)
                foreach (KeyValuePair<int, WorldInfo> worldInfo in list)
                    if (worldInfo.Value.Parent == Parent)
                        retVal.Add(worldInfo.Value);
            return retVal.ToArray();
        }

        public static void LoadData()
        {
            List<WorldInfo> worldInfos = new List<WorldInfo>();

            // Load file for each world info in turn
            foreach (string file in System.IO.Directory.GetFiles(@"C:\ProgramData\VoidRats\WorldInfo\", @"*.voidData"))
            {
                WorldInfo newWorldInfo = new WorldInfo();
                try
                {
                    newWorldInfo.loadData(file);
                }
                catch (Exception) { newWorldInfo = null; }
                if (newWorldInfo != null)
                    worldInfos.Add(newWorldInfo);
            }

            // Save new list of abilities
            lock (list)
            {
                list.Clear();
                foreach (WorldInfo worldInfo in worldInfos)
                    list.Add(worldInfo.ID, worldInfo);
            }
        }
        #endregion
    }
}
