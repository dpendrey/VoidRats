using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace VoidRatsUIAPI
{
    public abstract class Extension
    {
        public abstract int Priority { get; }
        public abstract string Name { get; }
        protected abstract bool handleAction(string Action);
        protected abstract bool prepareInfo(string Key, System.Windows.Documents.FlowDocument Document);
        protected abstract void loadExtension();

        private static List<Menu> menuList = new List<Menu>();
        public static Menu[] GetMenus()
        {
            Menu[] retVal;
            lock (menuList)
                retVal = menuList.ToArray();
            return retVal;
        }
        public static Menu GetMenu(int Index)
        {
            Menu retVal;
            lock (menuList)
                retVal = menuList[Index];
            return retVal;
        }
        public static Menu GetMenu(string Key)
        {
            Menu retVal = null;
            lock (menuList)
                for (int i = 0; i < menuList.Count; i++)
                    if (menuList[i].MenuKey == Key)
                    {
                        retVal = menuList[i];
                        break;
                    }

            if (retVal == null)
                throw new IndexOutOfRangeException();

            return retVal;
        }

        public static void MergeMenus(IEnumerable<Menu> Menus)
        {
            foreach (Menu menu in Menus)
            {
                bool isFound = false;
                lock (menuList)
                    foreach (Menu curMenu in menuList)
                        if (curMenu.MenuKey == menu.MenuKey)
                        {
                            foreach (MenuEntry entry in menu.Entries)
                            {
                                isFound = false;
                                foreach (MenuEntry curEntry in curMenu.Entries)
                                {
                                    if (curEntry.ActionKey == menu.MenuKey)
                                    {
                                        isFound = true;
                                        break;
                                    }
                                }
                                if (!isFound)
                                    curMenu.AddEntry(entry);
                            }
                            isFound = true;
                            break;
                        }
                if (!isFound)
                    lock (menuList)
                        menuList.Add(menu);
            }
        }

        private static SortedList<int, Extension> extensionList = new SortedList<int, Extension>();

        public static void LoadExtensions()
        {
            Extension ext = new BasicUI();
            extensionList.Add(ext.Priority, ext);

            foreach (KeyValuePair<int, Extension> extension in extensionList)
                extension.Value.loadExtension();

            Application app = new Application();

            ///app.StartupUri = new System.Uri("VoidRatsUI.xaml", UriKind.Relative);
            app.StartupUri = new System.Uri("pack://application:,,,/VoidRatsUIAPI;component/VoidRatsUI.xaml", UriKind.Absolute);


            ResourceDictionary myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri("BlackCrystal.xaml", UriKind.Relative);
            app.Resources.MergedDictionaries.Add(myResourceDictionary);

            app.Run();
        }

        public static void PrepareInfo(string Key, System.Windows.Documents.FlowDocument Document)
        {
            foreach (KeyValuePair<int, Extension> extension in extensionList)
                if (extension.Value.prepareInfo(Key, Document))
                    return;
        }

        public static void HandleAction(string Action)
        {
            foreach (KeyValuePair<int, Extension> extension in extensionList)
                if (extension.Value.handleAction(Action))
                    return;
        }
        public static void HandleAction(object sender, RequestNavigateEventArgs e)
        {
            HandleAction(e.Uri.OriginalString);
        }

        public class BasicUI : Extension
        {
            public override int Priority
            {
                get { return 0; }
            }
            public override string Name
            {
                get { return "Basic UI"; }
            }
            protected override bool handleAction(string Action)
            {
                if (Action.StartsWith("data://"))
                {
                    if (VoidRatsUI.currentWindow != null)
                        VoidRatsUI.currentWindow.showInfo(Action);
                }
                else if (Action.StartsWith("menu://"))
                {
                    if (VoidRatsUI.currentWindow != null)
                    {
                        Menu newMenu = null;
                        try { newMenu = GetMenu(Action.Substring("menu://".Length)); }
                        catch (Exception) { newMenu = GetMenu("MAINMENU"); }
                        VoidRatsUI.currentWindow.setMenu(newMenu);
                    }
                }
                else if (Action.StartsWith("action://"))
                {
                    if (Action == "action://EXIT")
                        if (VoidRatsUI.currentWindow != null)
                            VoidRatsUI.currentWindow.Close();
                }

                return true;
            }
            protected override void loadExtension()
            {
                List<Menu> menus = new List<Menu>();
                Menu menu;

                #region Main menu
                menu = new Menu("MAINMENU");
                menu.AddEntry(new MenuEntry("menu://VIEWBASES", "Bases"));
                menu.AddEntry(new MenuEntry("menu://VIEWBOTS", "Bots"));
                menu.AddEntry(new MenuEntry("menu://VIEWMARKET", "Market"));
                menu.AddEntry(new MenuEntry("menu://VIEWCOMPANIES", "Companies"));
                menu.AddEntry(new MenuEntry("menu://VIEWASTEROIDS", "Asteroids"));

                menu.AddEntry(new MenuEntry());

                menu.AddEntry(new MenuEntry("menu://WORLDINFO", "World Info"));
                menu.AddEntry(new MenuEntry("menu://EXITGAME", "Exit"));

                menus.Add(menu);
                #endregion

                #region World info menu
                menu = new Menu("WORLDINFO");
                menu.AddEntry(new MenuEntry("data://WORLDINFO/", "World Info"));
                menu.AddEntry(new MenuEntry("data://ABILITIES/", "Abilities"));
                menu.AddEntry(new MenuEntry("data://ITEMS/", "Items"));
                menu.AddEntry(new MenuEntry("data://PLANS/", "Plans"));
                menu.AddEntry(new MenuEntry("data://RECIPIES/", "Recipies"));
                menu.AddEntry(new MenuEntry("data://RESOURCES/", "Resources"));

                menu.AddEntry(new MenuEntry());

                menu.AddEntry(new MenuEntry("menu://MAINMENU", "Back"));

                menus.Add(menu);
                #endregion

                #region Exit game menu
                menu = new Menu("EXITGAME");

                menu.AddEntry(new MenuEntry(null, "Exit Game?"));

                menu.AddEntry(new MenuEntry());

                menu.AddEntry(new MenuEntry("action://EXIT", "Exit"));
                menu.AddEntry(new MenuEntry("menu://MAINMENU", "Cancel"));

                menus.Add(menu);
                #endregion

                Extension.MergeMenus(menus);
            }
            protected override bool prepareInfo(string Key, System.Windows.Documents.FlowDocument Document)
            {
                // Clear the old text
                Document.Blocks.Clear();

                Run run;
                Paragraph paragraph;

                #region Abilities list
                if (Key == "data://ABILITIES/")
                {
                    run = new Run("Abilities");
                    run.FontSize = 24;
                    run.FontWeight = FontWeights.Bold;
                    Document.Blocks.Add(new Paragraph(run));

                    run = new Run("\"Abilities\" are what we call the things different items can do. Select an ability from the list below to read more about it.");
                    run.FontSize = 16;
                    Document.Blocks.Add(new Paragraph(run));

                    paragraph = new Paragraph();
                    foreach (VoidRatsAPI.Ability ability in VoidRatsAPI.Ability.ListAbilities())
                    {
                        run = new Run(ability.Name + "\r\n");
                        run.FontSize = 16;
                        Hyperlink textLink = new Hyperlink(run);
                        textLink.NavigateUri = new Uri("data://ABILITY/" + ability.ID, UriKind.Absolute);
                        textLink.RequestNavigate +=
                          new System.Windows.Navigation.RequestNavigateEventHandler(HandleAction);

                        paragraph.Inlines.Add(textLink);
                    }
                    ///run = new Run(builder.ToString());
                    ///run.FontSize = 16;
                    ///Document.Blocks.Add(new Paragraph(run));
                    Document.Blocks.Add(paragraph);
                    return true;
                }
                #endregion
                #region Ability details
                if (Key.StartsWith("data://ABILITY/"))
                {
                    VoidRatsAPI.Ability ability;
                    try { ability = VoidRatsAPI.Ability.GetAbility(int.Parse(Key.Substring("data://ABILITY/".Length))); }
                    catch (Exception) { ability = null; }

                    if (ability == null)
                    {
                        run = new Run("Ability");
                        run.FontSize = 24;
                        run.FontWeight = FontWeights.Bold;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run("Ability not found.");
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));
                    }
                    else
                    {
                        run = new Run("Ability: " + ability.Name);
                        run.FontSize = 24;
                        run.FontWeight = FontWeights.Bold;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run("Image: Not yet supported.");
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));

                        paragraph = new Paragraph();
                        for (int i = 0; i < ability.FieldNames.Length; i++)
                        {
                            run = new Run(ability.FieldNames[i] + "\r\n");
                            run.FontSize = 16;
                            paragraph.Inlines.Add(run);
                        }
                        Document.Blocks.Add(paragraph);

                        run = new Run(ability.Description);
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));
                    }
                    return true;
                }
                #endregion
                #region Resources list
                if (Key == "data://RESOURCES/")
                {
                    run = new Run("Resources");
                    run.FontSize = 24;
                    run.FontWeight = FontWeights.Bold;
                    Document.Blocks.Add(new Paragraph(run));

                    run = new Run("\"Resources\" are what you will need to build or buy anything. The most useful resource and most abundant, energy, is the defacto currency. Select an resource from the list below to read more about it.");
                    run.FontSize = 16;
                    Document.Blocks.Add(new Paragraph(run));

                    paragraph = new Paragraph();
                    foreach (VoidRatsAPI.Resource resource in VoidRatsAPI.Resource.ListResources())
                    {
                        run = new Run(resource.Name + "\r\n");
                        run.FontSize = 16;
                        Hyperlink textLink = new Hyperlink(run);
                        textLink.NavigateUri = new Uri("data://RESOURCE/" + resource.ID, UriKind.Absolute);
                        textLink.RequestNavigate +=
                          new System.Windows.Navigation.RequestNavigateEventHandler(HandleAction);

                        paragraph.Inlines.Add(textLink);
                    }
                    ///run = new Run(builder.ToString());
                    ///run.FontSize = 16;
                    ///Document.Blocks.Add(new Paragraph(run));
                    Document.Blocks.Add(paragraph);
                    return true;
                }
                #endregion
                #region Resource details
                if (Key.StartsWith("data://RESOURCE/"))
                {
                    VoidRatsAPI.Resource resource;
                    try { resource = VoidRatsAPI.Resource.GetResource(int.Parse(Key.Substring("data://RESOURCE/".Length))); }
                    catch (Exception) { resource = null; }

                    if (resource == null)
                    {
                        run = new Run("Resource");
                        run.FontSize = 24;
                        run.FontWeight = FontWeights.Bold;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run("Resource not found.");
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));
                    }
                    else
                    {
                        run = new Run("Resource: " + resource.Name);
                        run.FontSize = 24;
                        run.FontWeight = FontWeights.Bold;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run("Image: Not yet supported.");
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run("Density: " + resource.Density.ToString("0.0") + " k.g./m^3\r\n" +
                            "Nominal Price: " + resource.NominalPrice.ToString("0") + " watts/k.g. (a theoretical price for this resources, if the stock market stopped being run by people and all trade suddenly became mathematically fair and perfect overnight)\r\n" +
            "System Availability: " + resource.SystemAvailability.ToString("0") + "% (a relative value to express how common the resource is within the stellar system)\r\n" +
            "System Usage: " + resource.SystemUsage.ToString("0") + "% (a relative value to express how common use of this resource is within the stellar system)");
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run(resource.Description);
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));
                    }
                    return true;
                }
                #endregion

                #region World info list
                if (Key == "data://WORLDINFO/")
                {
                    run = new Run("World Info");
                    run.FontSize = 24;
                    run.FontWeight = FontWeights.Bold;
                    Document.Blocks.Add(new Paragraph(run));

                    run = new Run("Here you can learn how to play the game, and read up on some of the official background storyline.");
                    run.FontSize = 16;
                    Document.Blocks.Add(new Paragraph(run));

                    paragraph = new Paragraph();
                    foreach (VoidRatsAPI.WorldInfo worldInfo in VoidRatsAPI.WorldInfo.ListChildren(0))
                    {
                        run = new Run(worldInfo.Name + "\r\n");
                        run.FontSize = 16;
                        Hyperlink textLink = new Hyperlink(run);
                        textLink.NavigateUri = new Uri("data://WORLDINFO/" + worldInfo.ID, UriKind.Absolute);
                        textLink.RequestNavigate +=
                          new System.Windows.Navigation.RequestNavigateEventHandler(HandleAction);

                        paragraph.Inlines.Add(textLink);
                    }
                    Document.Blocks.Add(paragraph);
                    return true;
                }
                #endregion
                #region World info details
                if (Key.StartsWith("data://WORLDINFO/"))
                {
                    VoidRatsAPI.WorldInfo worldInfo;
                    try { worldInfo = VoidRatsAPI.WorldInfo.GetWorldInfo(int.Parse(Key.Substring("data://WORLDINFO/".Length))); }
                    catch (Exception) { worldInfo = null; }

                    if (worldInfo == null)
                    {
                        run = new Run("World Info");
                        run.FontSize = 24;
                        run.FontWeight = FontWeights.Bold;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run("World information not found.");
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));
                    }
                    else
                    {
                        run = new Run("World Info: " + worldInfo.Name);
                        run.FontSize = 24;
                        run.FontWeight = FontWeights.Bold;
                        Document.Blocks.Add(new Paragraph(run));

                        run = new Run(worldInfo.Description);
                        run.FontSize = 16;
                        Document.Blocks.Add(new Paragraph(run));

                        List<VoidRatsAPI.WorldInfo> seeAlso = new List<VoidRatsAPI.WorldInfo>();
                        if (worldInfo.Parent > 0)
                            seeAlso.Add(VoidRatsAPI.WorldInfo.GetWorldInfo(worldInfo.Parent));
                        else
                            seeAlso.AddRange(VoidRatsAPI.WorldInfo.ListChildren(0));
                        seeAlso.AddRange(VoidRatsAPI.WorldInfo.ListChildren(worldInfo.ID));

                        if (seeAlso.Count > 0)
                        {
                            paragraph = new Paragraph();
                            run = new Run("See Also:\r\n");
                            run.FontSize = 20;
                            run.FontWeight = FontWeights.Bold;
                            paragraph.Inlines.Add(run);

                            foreach (VoidRatsAPI.WorldInfo also in seeAlso)
                            {
                                run = new Run(also.Name + "\r\n");
                                run.FontSize = 16;
                                Hyperlink textLink = new Hyperlink(run);
                                textLink.NavigateUri = new Uri("data://WORLDINFO/" + also.ID, UriKind.Absolute);
                                textLink.RequestNavigate +=
                                  new System.Windows.Navigation.RequestNavigateEventHandler(HandleAction);

                                paragraph.Inlines.Add(textLink);
                            }
                            Document.Blocks.Add(paragraph);
                        }
                    }
                    return true;
                }
                #endregion



                Document.Blocks.Add(new Paragraph(new Run("Could not find the topic in question.")));
                Document.Blocks.Add(new Paragraph(new Run(Key)));

                return true;
            }
        }
    }
}
