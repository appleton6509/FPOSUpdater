using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;


namespace FPOSUpdater.UserControls
{
    /// <summary>
    /// Interaction logic for FolderEntry.xaml
    /// </summary>
    public partial class FolderEntry
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FolderEntry), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FolderEntry), new PropertyMetadata(null));

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

        public FolderEntry()
        {
            InitializeComponent();
        }

        private void BrowseFolder(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = Description;
                dlg.SelectedPath = Text;
                dlg.ShowNewFolderButton = true;
                try
                {
                    DialogResult result = dlg.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Text = dlg.SelectedPath;
                        BindingExpression be = GetBindingExpression(TextProperty);
                        if (be != null)
                            be.UpdateSource();
                    }
                } catch (Exception) { }
            }
        }
    }
}
