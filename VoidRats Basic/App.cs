using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;

namespace VoidRats_Basic
{
    /// <summary>
    /// 
    /// </summary>
    class App : Application
    {
        /// <summary>
        /// 
        /// </summary>
        [STAThread()]
        static void Main()
        {
            Splasher.Splash = new SplashScreen();
            Splasher.ShowSplash();


            MessageListener.Instance.ReceiveMessage("Contacting server for updates...");
            Thread.Sleep(500);
            MessageListener.Instance.ReceiveMessage("Loading resources");
            VoidRatsAPI.Resource.LoadData();
            MessageListener.Instance.ReceiveMessage("Loading abilities");
            VoidRatsAPI.Ability.LoadData();
            MessageListener.Instance.ReceiveMessage("Loading world info");
            VoidRatsAPI.WorldInfo.LoadData();

            MessageListener.Instance.ReceiveMessage("Loading extensions");
            
            ///new App();
            VoidRatsUIAPI.Extension.LoadExtensions();
        }
        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            /*
            StartupUri = new System.Uri("MainWindow.xaml", UriKind.Relative);

            ResourceDictionary myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri("BlackCrystal.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);

            Run();
            */
        }
    }
}