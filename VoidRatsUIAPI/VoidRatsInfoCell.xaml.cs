using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VoidRatsUIAPI
{
    /// <summary>
    /// Interaction logic for VoidRatsInfoCell.xaml
    /// </summary>
    public partial class VoidRatsInfoCell : UserControl
    {
        public VoidRatsInfoCell()
        {
            InitializeComponent();

            richText.Document.Blocks.Clear();
            richText.Document.Blocks.Add(new Paragraph(new Run("Text")));
        }

        public void ShowInfo(string Topic)
        {
            Extension.PrepareInfo(Topic, richText.Document);
        }

        public void link_Clicked(object sender, RequestNavigateEventArgs e)
        {
            ShowInfo(e.Uri.OriginalString);
        }
    }
}
