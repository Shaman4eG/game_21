using System;
using System.Collections.Generic;
using System.Data;
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
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        private const string register_new_player = "register_new_player";
        private const string login = "login";

        public MainMenu()
        {
            InitializeComponent();
        }

        private enum ResultStatus
        {
            Success,
            Error
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            ResultStatus registrationResult;
            int playerId;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
                {
                    connection.Open();

                    MySqlCommand cmd = new MySqlCommand(register_new_player, connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@_name", UserNameInput.Text);
                    cmd.Parameters["@_name"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("@_result_status", MySqlDbType.Int32);
                    cmd.Parameters["@_result_status"].Direction = ParameterDirection.Output;

                    cmd.Parameters.AddWithValue("@_player_id", MySqlDbType.Int32);
                    cmd.Parameters["@_player_id"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    registrationResult = (ResultStatus) cmd.Parameters["@_result_status"].Value;
                    playerId = (int) cmd.Parameters["@_player_id"].Value;
                }
            }
            catch
            {
                ErrorText.Text = "Login's length should be from 3 to 50 characters.";
                return;
            }

            if (registrationResult == ResultStatus.Success)
            {
                var game = new Game(playerId);
                NavigationService.Navigate(game);
            }
            else
            {
                ErrorText.Text = "Player with given name already exists.";
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ResultStatus loginResult;
            int playerId;
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(login, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_name", UserNameInput.Text);
                cmd.Parameters["@_name"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_result_status", MySqlDbType.Int32);
                cmd.Parameters["@_result_status"].Direction = ParameterDirection.Output;

                cmd.Parameters.AddWithValue("@_player_id", MySqlDbType.Int32);
                cmd.Parameters["@_player_id"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                loginResult = (ResultStatus)cmd.Parameters["@_result_status"].Value;
                playerId = (int)cmd.Parameters["@_player_id"].Value;
            }

            if (loginResult == ResultStatus.Success)
            {
                var game = new Game(playerId);
                NavigationService.Navigate(game);
            }
            else
            {
                ErrorText.Text = "Player with given name does not exist.";
            }
        }
    }
}
