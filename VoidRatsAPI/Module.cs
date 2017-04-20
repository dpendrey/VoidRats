using System;
using System.Collections.Generic;
using System.Text;

namespace VoidRatsAPI
{
    /// <summary>
    /// Represents a component module for use on a bot or a base
    /// </summary>
    public class Module : ServerData
    {
        private string name;
        private string desc;
        private ModuleAbility[] abilities;
        private double nominalCost;

        #region Load data
        internal void loadData(string Filename)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(Filename);

            string curLine = reader.ReadLine();
            bool serverDataRead = false;
            List<ModuleAbility> abilities = new List<ModuleAbility>();

            name = desc = string.Empty;
            nominalCost = 0;

            while (curLine != null)
            {
                if (base.loadData(reader, curLine, DataType.Ability))
                    serverDataRead = true;
                else if (curLine.ToUpper().StartsWith("NAME="))
                    name = curLine.Substring("NAME=".Length);
                else if (curLine.ToUpper().StartsWith("NOMINALCOST="))
                    nominalCost = double.Parse(curLine.Substring("NOMINALCOST=".Length));
                else if (curLine.ToUpper().StartsWith("ABILITY"))
                {
                    int abilityIndex = int.Parse(curLine.Substring("ABILITY".Length, 3));

                    while (abilities.Count <= abilityIndex)
                        abilities.Add(new ModuleAbility());

                    if (curLine.ToUpper().StartsWith("ABILITY" + abilityIndex.ToString("000") + "ID="))
                        abilities[abilityIndex].ability = int.Parse(curLine.Substring(("ABILITY" + abilityIndex.ToString("000") + "ID=").Length));
                    if (curLine.ToUpper().StartsWith("ABILITY" + abilityIndex.ToString("000") + "FIELD"))
                    {
                        int fieldIndex = int.Parse(curLine.Substring(("ABILITY" + abilityIndex.ToString("000") + "FIELD").Length, 3));

                        
                    }
                }
                else if (curLine.ToUpper() == "[DESCRIPTION]")
                    desc = reader.ReadToEnd();

                curLine = reader.ReadLine();
            }

            if (!serverDataRead)
                throw new NoServerDataException();
            if (name.Length == 0 || desc.Length == 0 || nominalCost <= 0)
                throw new InvalidModuleData();

            fieldNames = names.ToArray();
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Gets the name of this ability
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Gets the description of this ability
        /// </summary>
        public string Description { get { return desc; } }

        /// <summary>
        /// Gets the names of the fields of this ability
        /// </summary>
        public string[] FieldNames
        {
            get
            {
                string[] retVal = new string[fieldNames.Length];
                for (int i = 0; i < retVal.Length; i++)
                    retVal[i] = fieldNames[i];
                return retVal;
            }
        }

        /// <summary>
        /// Gets the filename of the image for this ability
        /// </summary>
        public string ImageFilename { get { return imgFilename; } }
        #endregion

        #region Exceptions
        /// <summary>
        /// Thrown when invalid data is found upon reading an ability data file
        /// </summary>
        public class InvalidAbilityData : Exception { }
        #endregion

        #region List
        private static Dictionary<int, Ability> list = new Dictionary<int, Ability>();

        public static Ability GetAbility(int ID)
        {
            Ability retVal;
            lock (list)
                retVal = list[ID];
            return retVal;
        }

        public static Ability[] ListAbilities()
        {
            List<Ability> retVal = new List<Ability>();
            lock (list)
                foreach (KeyValuePair<int, Ability> ability in list)
                    retVal.Add(ability.Value);
            return retVal.ToArray();
        }

        public static void LoadData()
        {
            List<Ability> abilities = new List<Ability>();

            // Load file for each ability in turn
            foreach (string file in System.IO.Directory.GetFiles(@"C:\ProgramData\VoidRats\Abilities\", @"*.voidData"))
            {
                Ability newAbility = new Ability();
                try
                {
                    newAbility.loadData(file);
                }
                catch (Exception) { newAbility = null; }
                if (newAbility != null)
                    abilities.Add(newAbility);
            }

            // Save new list of abilities
            lock (list)
            {
                list.Clear();
                foreach (Ability ability in abilities)
                    list.Add(ability.ID, ability);
            }
        }
        #endregion

        public class ModuleAbility
        {
            internal int ability;
            internal bool[] bitFields;
            internal int[] intFields;
            internal double[] doubleFields;
            internal string[] stringFields;

            public int Ability { get { return ability; } }
            public object[] Fields { get { return fields; } }
        }
    }
}
