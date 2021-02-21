using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;


namespace FPOSPriceUpdater.UserControls
{
    /// <summary>
    /// Interaction logic for FileEntry.xaml
    /// </summary>
    public partial class FileEntry
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FileEntry), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FileEntry), new PropertyMetadata(null));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public string Description
        {
            get { return GetValue(DescriptionProperty) as string; }
            set { SetValue(DescriptionProperty, value); }
        }

        public FileEntry()
        {
            InitializeComponent();
        }

        private void BrowseFolder(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = "c:\\";
                dlg.Filter = "CSV (*.csv)|*.csv";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                try
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Text = dlg.FileName;
                        BindingExpression be = GetBindingExpression(TextProperty);
                        if (be != null)
                            be.UpdateSource();
                    }
                } catch (Exception) { }

            }
        }
    }
}
