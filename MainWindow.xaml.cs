using Microsoft.Win32;
using System.Windows;

namespace MaltedWPF
{
    public partial class MainWindow : Window
    {
        public string selectedFilePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                selectedFilePath = dialog.FileName;
                selectedFileTxt.Content = selectedFilePath;
            } else
            {
                selectedFilePath = null;
                selectedFileTxt.Content = "";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool result = Malted.EncryptFile(selectedFilePath, passwordField.Text);
            ResultTxt.Text = $"encrypted: {result}";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            bool result = Malted.DecryptFile(selectedFilePath, passwordField.Text);
            ResultTxt.Text = $"decrypted: {result}";
        }
    }
}
