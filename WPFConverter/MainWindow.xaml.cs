using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WPFConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }

        /// <summary>
        /// this method will bind currency to the combobox
        /// </summary>
        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");

            // add data to the datatable
            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("EUR", 1);
            dtCurrency.Rows.Add("USD", 1.06);
            dtCurrency.Rows.Add("CHF", 0.95);
            dtCurrency.Rows.Add("GBP", 0.87);

            // assign datatable to the item source
            CmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            CmbFromCurrency.DisplayMemberPath = "Text";
            CmbFromCurrency.SelectedValuePath = "Value";
            CmbFromCurrency.SelectedIndex = 0;

            CmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            CmbToCurrency.DisplayMemberPath = "Text";
            CmbToCurrency.SelectedValuePath = "Value";
            CmbToCurrency.SelectedIndex = 0;
        }

        private void ButtonConvert_Click(object sender, RoutedEventArgs e)
        {
            // variable that will store converted currency
            var convertedValue = 0.0;

            // check if the ammount textbox is null or empty
            if (string.IsNullOrEmpty(TxtCurrency.Text))
            {
                // if is null or empty display message
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                // set focus
                TxtCurrency.Focus();
                return;
            }
            // if the user did not select either currency or the default value is selected
            else if (string.IsNullOrEmpty(CmbFromCurrency.SelectedValue.ToString()) || CmbFromCurrency.SelectedIndex == 0)
            {
                // show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                // set focus
                CmbFromCurrency.Focus();
                return;
            }
            // if the user did not select either currency or the default value is selected
            else if (string.IsNullOrEmpty(CmbToCurrency.SelectedValue.ToString()) || CmbToCurrency.SelectedIndex == 0)
            {
                // show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                // set focus
                CmbToCurrency.Focus();
                return;
            }

            // if combobox are the same don't need to convert just show
            if (CmbFromCurrency.Text == CmbToCurrency.Text)
            {
                convertedValue = double.Parse(TxtCurrency.Text);

                // show the value to the label, and the N2 places 2 decimal cases after the dot
                LblCurrency.Content = $"{CmbToCurrency.Text} {convertedValue.ToString("N2")}";
            }
            else
            {
                // convert the insert value to the correct currency
                convertedValue = ((double.Parse(CmbToCurrency.SelectedValue.ToString()) * double.Parse(TxtCurrency.Text)) / double.Parse(CmbFromCurrency.SelectedValue.ToString()));

                // show the value to the label, and the N2 places 2 decimal cases after the dot
                LblCurrency.Content = $"{CmbToCurrency.Text} {convertedValue.ToString("N2")}";
            }

        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        /// <summary>
        /// will validate that the input is only numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// clear all the fields
        /// </summary>
        private void ClearControls()
        {
            TxtCurrency.Text = string.Empty;
            LblCurrency.Content = string.Empty;
            if (CmbFromCurrency.Items.Count > 0) { CmbFromCurrency.SelectedIndex = 0; }
            if (CmbToCurrency.Items.Count > 0) { CmbToCurrency.SelectedIndex = 0; }
            TxtCurrency.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButonSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
