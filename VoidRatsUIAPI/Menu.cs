using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace VoidRatsUIAPI
{
    public class Menu
    {
        private string menuKey;
        private List<MenuEntry> entries = new List<MenuEntry>();

        public Menu(string Key)
        {
            menuKey = Key;
        }

        public string MenuKey { get { return menuKey; } }

        public void AddEntry(MenuEntry Entry)
        {
            lock (entries)
                entries.Add(Entry);
        }

        public void RemoveEntry(MenuEntry Entry)
        {
            lock (entries)
                entries.Remove(Entry);
        }

        public int Count
        {
            get
            {
                int retVal;
                lock (entries)
                    retVal = entries.Count;
                return retVal;
            }
        }

        public MenuEntry[] Entries
        {
            get
            {
                MenuEntry[] retVal;
                lock (entries)
                    retVal = entries.ToArray();
                return retVal;
            }
        }

        internal Grid BuildMenu()
        {
            ColumnDefinition col;
            RowDefinition row;
            Grid grid = new Grid();

            col = new ColumnDefinition();
            ///col.Width = System.Windows.GridLength.Auto;
            grid.ColumnDefinitions.Add(col);

            MenuEntry[] items;
            lock (entries)
                items = entries.ToArray();

            for(int i=0;i<items.Length;i++)
            {
                row = new RowDefinition();
                if (!items[i].IsBreak)
                    row.Height = System.Windows.GridLength.Auto;
                grid.RowDefinitions.Add(row);

                if (items[i].IsLabel)
                {
                    Label label = new Label();
                    label.Content = items[i].DisplayText ;
                    label.FontSize = 24;
                    label.FontWeight = System.Windows.FontWeights.Bold;
                    label.Margin = new System.Windows.Thickness(5);

                    grid.Children.Add(label);
                    Grid.SetRow(label, i);
                }
                else if (!items[i].IsBreak)
                {
                    Button button = new Button();
                    button.Content = items[i].DisplayText;
                    button.FontSize = 24;
                    button.FontWeight = System.Windows.FontWeights.Bold;
                    button.Margin = new System.Windows.Thickness(5);
                    button.Tag = items[i];
                    button.Click += new System.Windows.RoutedEventHandler(button_Click);

                    grid.Children.Add(button);
                    Grid.SetRow(button, i);
                }
            }

            return grid;
        }

        void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button && ((Button)sender).Tag is MenuEntry)
            {
                MenuEntry entry = (MenuEntry)((Button)sender).Tag;

                Extension.HandleAction(entry.ActionKey);
            }
        }
    }

    public class MenuEntry
    {
        private string actionKey,
            displayText;
        public string ActionKey { get { return actionKey; } }
        public string DisplayText { get { return displayText; } }
        public bool IsBreak { get { return string.IsNullOrEmpty(actionKey) || string.IsNullOrEmpty(displayText); } }
        public bool IsLabel { get { return string.IsNullOrEmpty(actionKey); } }

        public MenuEntry()
        {
            actionKey =
                displayText =
                string.Empty;
        }

        public MenuEntry(string ActionKey, string DisplayText)
        {
            actionKey = ActionKey;
            displayText = DisplayText;
        }
    }
}
