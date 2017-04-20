using System;
using System.Collections.Generic;
using System.Text;

namespace VoidRatsAPI
{
    /// <summary>
    /// Represents a resource in the game system
    /// </summary>
    public class Resource : ServerData
    {
        private string name;
        private string desc;
        private double density;
        private double nominalPrice;
        private string imgFilename;
        private double systemAvailability;
        private double systemUsage;

        #region Load data
        internal void loadData(string Filename)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(Filename);

            string curLine = reader.ReadLine();
            bool serverDataRead = false;

            name = desc = imgFilename = string.Empty;
            density = nominalPrice = systemAvailability = systemUsage = 0;

            while (curLine != null)
            {
                if (base.loadData(reader, curLine, DataType.Resource))
                    serverDataRead = true;
                else if (curLine.ToUpper().StartsWith("NAME="))
                    name = curLine.Substring("NAME=".Length);
                else if (curLine.ToUpper() == "[DESCRIPTION]")
                    desc = reader.ReadToEnd();
                else if (curLine.ToUpper().StartsWith("DENSITY="))
                {
                    if (!double.TryParse(curLine.Substring("DENSITY=".Length), out density))
                        throw new InvalidResourceData();
                }
                else if (curLine.ToUpper().StartsWith("NOMPRICE="))
                {
                    if (!double.TryParse(curLine.Substring("NOMPRICE=".Length), out nominalPrice))
                        throw new InvalidResourceData();
                }
                else if (curLine.ToUpper().StartsWith("SYSTEMAVAILABILITY="))
                {
                    if (!double.TryParse(curLine.Substring("SYSTEMAVAILABILITY=".Length), out systemAvailability))
                        throw new InvalidResourceData();
                }
                else if (curLine.ToUpper().StartsWith("SYSTEMUSAGE="))
                {
                    if (!double.TryParse(curLine.Substring("SYSTEMUSAGE=".Length), out systemUsage))
                        throw new InvalidResourceData();
                }
                else if (curLine.ToUpper().StartsWith("IMGFILENAME="))
                    imgFilename = curLine.Substring("IMGFILENAME=".Length);

                curLine = reader.ReadLine();
            }

            if (!serverDataRead)
                throw new NoServerDataException();
            if (name.Length == 0 || desc.Length == 0 || imgFilename.Length == 0 || nominalPrice == 0 || density == 0 || systemAvailability == 0 || systemUsage == 0)
                throw new InvalidResourceData();
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Gets the name of this resource
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Gets the description of this resource
        /// </summary>
        public string Description { get { return desc; } }

        /// <summary>
        /// Gets the density of this resource (k.g./m^3)
        /// </summary>
        public double Density { get { return density; } }

        /// <summary>
        /// Gets the amount available within the system
        /// </summary>
        public double SystemAvailability { get { return systemAvailability; } }

        /// <summary>
        /// Gets the amount used within the system
        /// </summary>
        public double SystemUsage { get { return systemUsage; } }

        /// <summary>
        /// Gets the nominal price (watts/k.g.) of this resource
        /// </summary>
        public double NominalPrice { get { return nominalPrice; } }

        /// <summary>
        /// Gets the filename of the image for this resource
        /// </summary>
        public string ImageFilename { get { return imgFilename; } }
        #endregion

        #region Exceptions
        /// <summary>
        /// Thrown when invalid data is found upon reading an resource data file
        /// </summary>
        public class InvalidResourceData : Exception { }
        #endregion

        #region List
        private static Dictionary<int, Resource> list = new Dictionary<int, Resource>();

        public static Resource GetResource(int ID)
        {
            Resource retVal;
            lock (list)
                retVal = list[ID];
            return retVal;
        }

        public static Resource[] ListResources()
        {
            List<Resource> retVal = new List<Resource>();
            lock (list)
                foreach (KeyValuePair<int, Resource> resource in list)
                    retVal.Add(resource.Value);
            return retVal.ToArray();
        }

        public static void LoadData()
        {
            List<Resource> resources = new List<Resource>();

            // Load file for each resource in turn
            foreach (string file in System.IO.Directory.GetFiles(@"C:\ProgramData\VoidRats\Resources\", @"*.voidData"))
            {
                Resource newResource = new Resource();
                try
                {
                    newResource.loadData(file);
                }
                catch (Exception) { newResource = null; }
                if (newResource != null)
                    resources.Add(newResource);
            }

            // Save new list of resources
            lock (list)
            {
                list.Clear();
                foreach (Resource resource in resources)
                    list.Add(resource.ID, resource);
            }
        }
        #endregion
    }
}
