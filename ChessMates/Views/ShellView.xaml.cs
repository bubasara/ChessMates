using ChessMates.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessMates.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        static string connString = "ChessMatesDB";  // default, changeable
        public ShellView()
        {
            InitializeComponent();
            MySQLScript.Script(connString);
        }

        /*      Buttons     */
        /*      Button: ImportPlayers     */
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear.Background = new SolidColorBrush(Color.FromRgb(114, 189, 171));
            string cs = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(cs))
            {
                MySQLScript.TruncateTables(connection);
            }
        }

        /*      Button: AddNewPlayer     */
        private void AddNewPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddNewPlayerPopup.IsOpen = true;
            FideRankTextBox.Text = ""; BirthYearTextBox.Text = "";
        }

        /*      Button: Pair     */
        private void Pair_Click(object sender, RoutedEventArgs e)
        {
            //connString = "ChessMatesDB";
            (new BergerPairingAlgorithm()).Pair(connString);
        }

        /*      Button: Submit     */
        private void SubmitPlayer_Click(object sender, RoutedEventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(cs))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = "insert into players (firstname, lastname, fiderank, birthyear, country)" +
                    "values (@firstname, @lastname, @fiderank, @birthyear, @country)";
                command.Parameters.AddWithValue("@firstname", FirstNameTextBox.Text);
                command.Parameters.AddWithValue("@lastname", LastNameTextBox.Text);
                if (FideRankTextBox.Text.IsEmpty())
                {
                    FideRankTextBox.Text = "0";
                    command.Parameters.AddWithValue("@fiderank", FideRankTextBox.Text);
                }
                else
                {
                    command.Parameters.AddWithValue("@fiderank", FideRankTextBox.Text);
                }
                if (BirthYearTextBox.Text.IsEmpty()) {
                    BirthYearTextBox.Text = "0";
                    command.Parameters.AddWithValue("@birthyear", BirthYearTextBox.Text);
                }
                else
                {
                    command.Parameters.AddWithValue("@birthyear", BirthYearTextBox.Text);
                }
                command.Parameters.AddWithValue("@country", CountryTextBox.Text);
                command.Prepare();
                command.ExecuteNonQuery();
                connection.Close();
            }
            FirstNameTextBox.Text = ""; LastNameTextBox.Text = ""; FideRankTextBox.Text = ""; BirthYearTextBox.Text = ""; CountryTextBox.Text = "";
            AddNewPlayerPopup.IsOpen = false;
        }

        /*      Button: Cancel     */
        private void CancelPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddNewPlayerPopup.IsOpen = false;
        }

        /*      Radio button: RemoteDB     */
        private void RadioButtonRemoteDB_Checked(object sender, RoutedEventArgs e)
        {
            // connects to remotemysql server and uses remote database
            connString = "RemoteMySQLDB";
            MySQLScript.Script(connString);
        }

        /*      Radio button: LocalDB     */
        private void RadioButtonLocalDB_Checked_1(object sender, RoutedEventArgs e)
        {
            // uses local database
            connString = "ChessMatesDB";
            MySQLScript.Script(connString);
        }
    }
}
