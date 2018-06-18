using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        private const string connectionString = "server=localhost;user=root;database=game_21;port=3306;password=root";
        private const string create_game = "create_game";
        private const string add_player_to_game = "add_player_to_game";
        private const string start_game = "start_game";
        private const string get_chosen_player_hand_id = "get_chosen_player_hand_id";
        private const string get_hand_cards_ids = "get_hand_cards_ids";

        private readonly int _loginedPlayerId;
        private readonly int _playerHandId;
        private List<int> _playersCards;

        private const int _dealerId = 11;
        private readonly int _dealerHandId;

        private int _gameId;

        public Game()
        {
            InitializeComponent();
        }

        public Game(int loginedPlayerId) : this()
        {
            _loginedPlayerId = loginedPlayerId;

            CreateGame();
            AddDealerToGame();
            StartGame();
            _playerHandId = GetChosenPlayerHandId(_loginedPlayerId);
            _dealerHandId = GetChosenPlayerHandId(_dealerId);
            GetHandCardsIds(_playerHandId);
            GetHandCardsIds(_dealerHandId);
        }

        private void CreateGame()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(create_game, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_host_id", _loginedPlayerId);
                cmd.Parameters["@_game_host_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_game_id", MySqlDbType.Int32);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                _gameId = (int)cmd.Parameters["@_game_id"].Value;
            }
        }

        private void AddDealerToGame()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(add_player_to_game, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", _gameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_player_id", _dealerId);
                cmd.Parameters["@_player_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_result_status", MySqlDbType.Int32);
                cmd.Parameters["@_result_status"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
            }
        }

        private void StartGame()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(start_game, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", _gameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_result_status", MySqlDbType.Int32);
                cmd.Parameters["@_result_status"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
            }
        }

        private int GetChosenPlayerHandId(int playerId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(get_chosen_player_hand_id, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", _gameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_player_id", playerId);
                cmd.Parameters["@_player_id"].Direction = ParameterDirection.Input;

                return (int)cmd.ExecuteScalar();
            }
        }

        private List<int> GetHandCardsIds(int handId)
        {
            var handCardsIds = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(get_hand_cards_ids, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_hand_id", handId);
                cmd.Parameters["@_hand_id"].Direction = ParameterDirection.Input;

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    handCardsIds.Add((int)reader["card_id"]);
                }
            }

            return handCardsIds;
        }

        private void TakeCard_Click(object sender, RoutedEventArgs e)
        {
            // GetHand
            // CheckTakenCardsAmount
            // GetTopCardFromDeck (ИСПОЛЬЗОВАТЬ ТРИГГЕР ДЛЯ АВТОУДАЛЕНИЯ КАРТЫ) (ПЕРЕДАВАТЬ HAND_ID И ПРОВЕРЯТЬ, ЧТО КОЛОДА НЕ ПЕРЕПОЛНЕНА. 
            // ДОБАВЛЯТЬ КАРТУ В РУКУ В ЭТОЙ ЖЕ ПРОЦЕДУРЕ)
            // GetHand
            // ShowCards
            // UpdateSumText
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("MainMenu.xaml", UriKind.Relative));
        }
    }
}
