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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RLGL
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InheritanceBehavior = InheritanceBehavior.SkipToThemeNext;
            InitializeComponent();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WatchOutCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckboxHandle(sender as CheckBox);
        }

        void CheckboxHandle(CheckBox c)
        {
            bool check = c.IsChecked.Value;
            Console.WriteLine(check.ToString());
        }
    }
}
