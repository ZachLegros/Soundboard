using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for InputProfileName.xaml
    /// </summary>
    public partial class InputProfileName : Window
    {
        bool canceled = false;

        public InputProfileName()
        {
            InitializeComponent();
        }

        public string ShowDialogAndGetText()
        {
            ShowDialog();
            if (!canceled)
                return TxtBoxProfileName.Text;
            else
                return null;
        }

        public void BtnOK_Click(object sender, RoutedEventArgs e) 
        {
            if (TxtBoxProfileName.Text == "" || TxtBoxProfileName.Text == null)
                MessageBox.Show("Invalid input", "Error");
            else
            {
                Close();
            }
        }

        public void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            canceled = true;
            Close();
        }
    }
}
