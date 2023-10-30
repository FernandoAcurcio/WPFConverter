using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
        private SqlConnection _connection = new SqlConnection();
        private SqlCommand _command = new SqlCommand();
        private SqlDataAdapter _adapter = new SqlDataAdapter();

        private int _currencyId = 0;
        private double _fromAmount = 0;
        private double _toAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
            GetData();
        }

        /// <summary>
        /// this method create a connection to the database
        /// </summary>
        public void MyConnection()
        {
            var connection = ConfigurationManager.ConnectionStrings["ConnectionCurrencyConverterDb"].ConnectionString;
            _connection = new SqlConnection(connection);
            _connection.Open(); // open connection
        }

        /// <summary>
        /// this method will bind currency to the combobox
        /// </summary>
        private void BindCurrency()
        {
            MyConnection();
            DataTable dt = new DataTable();
            // fetch data from database
            _command = new SqlCommand("Select Id, CurrencyName from Currency", _connection);
            _command.CommandType = CommandType.Text;
            _adapter = new SqlDataAdapter(_command); // create addapter
            _adapter.Fill(dt); // fill datatable with data, the is retrieved from the above command

            DataRow firstRow = dt.NewRow(); // create a new row for data table
            firstRow["Id"] = 0;
            firstRow["CurrencyName"] = "-- SELECT --"; // create and add "-- select --" as the first value, the other values should be add automaticly

            dt.Rows.InsertAt(firstRow, 0); // insert the firstrow in the position 0

            // check if dt is not null
            if (dt != null && dt.Rows.Count > 0)
            {
                // assign data to combobox
                CmbFromCurrency.ItemsSource = dt.DefaultView;
                CmbToCurrency.ItemsSource = dt.DefaultView;
            }
            _connection.Close(); // closes connection

            CmbFromCurrency.DisplayMemberPath = "CurrencyName";
            CmbFromCurrency.SelectedValuePath = "Id";
            CmbFromCurrency.SelectedIndex = 0;

            CmbToCurrency.DisplayMemberPath = "CurrencyName";
            CmbToCurrency.SelectedValuePath = "Id";
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
            Regex regex = new Regex("[^0-9,\\.]+");
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

        private void TbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // check if both fields have data, trimming data prevents that space is being added
                if (string.IsNullOrEmpty(TxtAmount.Text) || TxtAmount.Text.Trim() == "")
                {
                    // if is null or empty display message
                    MessageBox.Show("Please Enter Amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    // set focus
                    TxtAmount.Focus();
                    return;
                }
                else if (string.IsNullOrEmpty(TxtCurrencyName.Text) || TxtCurrencyName.Text.Trim() == "")
                {
                    // if is null or empty display message
                    MessageBox.Show("Please Enter Currency Name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    // set focus
                    TxtAmount.Focus();
                    return;
                }
                else
                {
                    // if there is information in both fields I can insert data into the database
                    if (_currencyId > 0)
                    {
                        UpdateData();
                    }
                    else
                    {
                        AddData();
                    }
                }
                ClearMaster();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        private void AddData()
        {
            if (MessageBox.Show("Are you sure you want to save?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) // if the result is equals to yes do the code bellow
            {
                MyConnection();
                _command = new SqlCommand("Insert Into Currency(Amount, CurrencyName) Values(@Amount, @CurrencyName)", _connection); // update query
                _command.CommandType = CommandType.Text;
                _command.Parameters.AddWithValue("@Amount", TxtAmount.Text);
                _command.Parameters.AddWithValue("@CurrencyName", TxtCurrencyName.Text);
                _command.ExecuteNonQuery();
                _connection.Close();

                MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateData()
        {
            if (MessageBox.Show("Are you sure you want to update?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) // if the result is equals to yes do the code bellow
            {
                MyConnection();
                _command = new SqlCommand("UPDATE  Currency SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", _connection); // update query
                _command.CommandType = CommandType.Text;
                _command.Parameters.AddWithValue("@Id", _currencyId);
                _command.Parameters.AddWithValue("@Amount", TxtAmount.Text);
                _command.Parameters.AddWithValue("@CurrencyName", TxtCurrencyName.Text);
                _command.ExecuteNonQuery();
                _connection.Close();

                MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// clear all the input
        /// </summary>
        private void ClearMaster()
        {
            try
            {
                TxtAmount.Text = string.Empty;
                TxtCurrencyName.Text = string.Empty;
                ButtonSave.Content = "Save";
                GetData();
                _currencyId = 0;
                BindCurrency();
                TxtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// fill gridview
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void GetData()
        {
            MyConnection();

            DataTable dt = new DataTable();

            _command = new SqlCommand("SELECT * FROM Currency", _connection);
            _command.CommandType = CommandType.Text;
            _adapter = new SqlDataAdapter(_command);
            _adapter.Fill(dt);

            //dt is not null and rows count greater than 0
            if (dt != null && dt.Rows.Count > 0)
            {
                //Assign DataTable data to dgvCurrency using ItemSource property.   
                DgvCurrency.ItemsSource = dt.DefaultView;
            }
            else
            {
                DgvCurrency.ItemsSource = null;
            }
            _connection.Close();
        }

        /// <summary>
        /// when user hit cancel just clear all the fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = (DataGrid)sender; // takes the data from the datagrid that was selected
                DataRowView selectedRow = dataGrid.CurrentItem as DataRowView; // gets the specific object

                // if selected row is not null
                if (selectedRow != null)
                {
                    if (DgvCurrency.Items.Count > 0) // if there are elements inside the datagrid do the bellow
                    {
                        if (dataGrid.SelectedCells.Count > 0)
                        {
                            _currencyId = Int32.Parse(selectedRow["Id"].ToString()); // gets the id of the selected row

                            // check what grid cell column was selected
                            if (dataGrid.SelectedCells[0].Column.DisplayIndex == 0) // edit 
                            {
                                TxtAmount.Text = selectedRow["Amount"].ToString();
                                TxtCurrencyName.Text = selectedRow["CurrencyName"].ToString();
                                ButtonSave.Content = "Update"; // change text on save button to update
                            }
                            if (dataGrid.SelectedCells[0].Column.DisplayIndex == 1) // delete
                            {
                                if (MessageBox.Show("Are you sure you want to delete?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    MyConnection();
                                    _command = new SqlCommand("DELETE FROM  Currency WHERE Id = @Id", _connection); // update query
                                    _command.CommandType = CommandType.Text;
                                    _command.Parameters.AddWithValue("@Id", _currencyId); // delete data of given id
                                    _command.ExecuteNonQuery();
                                    _connection.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
