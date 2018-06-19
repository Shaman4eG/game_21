using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly string connectionString = "server=localhost;user=root;database=game_21;port=3306;password=root";

        private const string clean_game_info = "clean_game_info";

        public static int GameId = 0;

        public MainWindow()
        {
            InitializeComponent();

            NavigationFrame.Navigate(new MainMenu());
        }

        private void OnWindowCloseAction(object sender, CancelEventArgs e)
        {
            CleanGameInfo();
        }

        public static void CleanGameInfo()
        {
            // Game was not created;
            if (GameId == 0) return;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(clean_game_info, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", GameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
