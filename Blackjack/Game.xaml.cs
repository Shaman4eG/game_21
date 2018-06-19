using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        private const string create_game = "create_game";
        private const string add_player_to_game = "add_player_to_game";
        private const string start_game = "start_game";
        private const string get_chosen_player_hand_id = "get_chosen_player_hand_id";
        private const string get_hand_cards_ids = "get_hand_cards_ids";
        private const string get_card_value = "get_card_value";
        private const string get_next_card = "get_next_card"; 
        private const string pass_turn = "pass_turn";
        private const string finish_game = "finish_game";

        private const int MaxCardsInHand = 5;
        private const int ValueAtWhichDealerStopsTakingCards = 16;

        private readonly int _loginedPlayerId;
        private int _playerHandId;
        private List<int> _playerCards;

        // WARNING!!! CHECK EXISTENS IN DB
        private const int _dealerId = 1;
        private int _dealerHandId;
        private List<int> _dealerCards;

        private int winnerId;

        public Game()
        {
            InitializeComponent();
        }

        public Game(int loginedPlayerId) : this()
        {
            _loginedPlayerId = loginedPlayerId;

            PrepareGame();
        }



        #region PrepareGameMethods

        private void PrepareGame()
        {
            try
            {
                CreateGame();
                AddDealerToGame();
                StartGame();

                _playerHandId = GetChosenPlayerHandId(_loginedPlayerId);
                _dealerHandId = GetChosenPlayerHandId(_dealerId);

                _playerCards = GetHandCardsIds(_playerHandId);
                _dealerCards = GetHandCardsIds(_dealerHandId);

                ShowHand(_loginedPlayerId, _playerCards);
                ShowHand(_dealerId, _dealerCards);

                UpdatePointsSum(_loginedPlayerId, _playerCards);
                UpdatePointsSum(_dealerId, _dealerCards);
            }
            catch
            {
                MainWindow.CleanGameInfo();
                ErrorText.Content = "Error calling DB proc.";
            }
        }

        private void CreateGame()
        {
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(create_game, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_host_id", _loginedPlayerId);
                cmd.Parameters["@_game_host_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_game_id", MySqlDbType.Int32);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                MainWindow.GameId = (int)cmd.Parameters["@_game_id"].Value;
            }
        }

        private void AddDealerToGame()
        {
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(add_player_to_game, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", MainWindow.GameId);
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
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(start_game, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", MainWindow.GameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_result_status", MySqlDbType.Int32);
                cmd.Parameters["@_result_status"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
            }
        }

        private int GetChosenPlayerHandId(int playerId)
        {
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(get_chosen_player_hand_id, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", MainWindow.GameId);
                cmd.Parameters.AddWithValue("@_player_id", playerId);

                cmd.Parameters.Add("@_id", MySqlDbType.Int32);
                cmd.Parameters["@_id"].Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteScalar();

                return Convert.ToInt32(cmd.Parameters["@_id"].Value);
            }
        }

        private List<int> GetHandCardsIds(int handId)
        {
            var handCardsIds = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
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

        #endregion



        #region VisualisationMethods

        private void ShowHand(int playerId, List<int> handCards)
        {
            if (playerId == _dealerId)
            {
                if (handCards.Count > 0) DealerCard1.Source = GetImageBitmap(handCards[0].ToString());
                if (handCards.Count > 1) DealerCard2.Source = GetImageBitmap(handCards[1].ToString());
                if (handCards.Count > 2) DealerCard3.Source = GetImageBitmap(handCards[2].ToString());
                if (handCards.Count > 3) DealerCard4.Source = GetImageBitmap(handCards[3].ToString());
                if (handCards.Count > 4) DealerCard5.Source = GetImageBitmap(handCards[4].ToString());
            }
            else
            {
                if (handCards.Count > 0) PlayerCard1.Source = GetImageBitmap(handCards[0].ToString());
                if (handCards.Count > 1) PlayerCard2.Source = GetImageBitmap(handCards[1].ToString());
                if (handCards.Count > 2) PlayerCard3.Source = GetImageBitmap(handCards[2].ToString());
                if (handCards.Count > 3) PlayerCard4.Source = GetImageBitmap(handCards[3].ToString());
                if (handCards.Count > 4) PlayerCard5.Source = GetImageBitmap(handCards[4].ToString());
            }
        }

        private BitmapImage GetImageBitmap(string imageName)
        {
            var stringPath = "/Images/" + imageName + ".png"; 
            Uri imageUri = new Uri(stringPath, UriKind.Relative);
            return new BitmapImage(imageUri);
        }

        private void UpdatePointsSum(int playerId, List<int> handCards)
        {
            var pointsSum = 0;
            for (int i = 0; i < handCards.Count; i++)
            {
                pointsSum += GetCardValue(handCards[i]);
            }

            if (playerId == _dealerId) DealerTotalPoints.Text = pointsSum.ToString();
            else PlayerTotalPoints.Text = pointsSum.ToString();
        }

        private int GetCardValue(int cardId)
        {
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(get_card_value, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_card_id", cardId);

                cmd.Parameters.Add("@value", MySqlDbType.Int32);
                cmd.Parameters["@value"].Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteScalar();

                return Convert.ToInt32(cmd.Parameters["@value"].Value);
            }
        }

        private void ShowResults()
        {
            MessageBoxResult result;
            if (winnerId == _loginedPlayerId)
            {
                result = MessageBox.Show(
                    "Congratulations! You won! \n\n" +
                    $"YOU: {PlayerTotalPoints.Text} \n" +
                    $"Dealer: {DealerTotalPoints.Text} \n\n" +
                    "Do you want to play again?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.None);
            }
            else
            {
                result = MessageBox.Show(
                    "Oh, you lost :( \n\n" +
                    $"YOU: {PlayerTotalPoints.Text} \n\n" +
                    $"Dealer: {DealerTotalPoints.Text} \n\n" +
                    "Do you want to play again?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.None);
            }

            if (result == MessageBoxResult.Yes)
            {
                var game = new Game(_loginedPlayerId);
                NavigationService.Navigate(game);
            }
            else
            {
                NavigationService.Navigate(new MainMenu());
            }
        }

        #endregion



        #region GameFlowMethods

        private void TakeCard_Click(object sender, RoutedEventArgs e)
        {
            var newCardId = GetNextCard(_playerHandId);
            _playerCards.Add(newCardId);

            ShowHand(_loginedPlayerId, _playerCards);

            UpdatePointsSum(_loginedPlayerId, _playerCards);

            if (Convert.ToInt32(PlayerTotalPoints.Text) > 21)
            {
                FinishGame();
            }

            if (_playerCards.Count == MaxCardsInHand)
            {
                TakeCardButton.IsEnabled = false;
            }
        }

        private int GetNextCard(int handId)
        {
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(get_next_card, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", MainWindow.GameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_hand_id", handId);
                cmd.Parameters["@_hand_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_result_status", MySqlDbType.Int32);
                cmd.Parameters["@_result_status"].Direction = ParameterDirection.Output;

                cmd.Parameters.AddWithValue("@_card_id", MySqlDbType.Int32);
                cmd.Parameters["@_card_id"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                return (int)cmd.Parameters["@_card_id"].Value;
            }
        }

        private void Pass_Click(object sender, RoutedEventArgs e)
        {
            TakeCardButton.IsEnabled = false;
            PassButton.IsEnabled = false;

            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(pass_turn, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", MainWindow.GameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_result_status", MySqlDbType.Int32);
                cmd.Parameters["@_result_status"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
            }

            StartDealerTurn();
        }

        private void StartDealerTurn()
        {
            var dealerPointsSum = Convert.ToInt32(DealerTotalPoints.Text);

            while (dealerPointsSum < ValueAtWhichDealerStopsTakingCards)
            {
                var newCardId = GetNextCard(_dealerHandId);
                _dealerCards.Add(newCardId);

                UpdatePointsSum(_dealerId, _dealerCards);

                dealerPointsSum = Convert.ToInt32(DealerTotalPoints.Text);
            }

            ShowHand(_dealerId, _dealerCards);

            FinishGame();
        }

        private void FinishGame()
        {
            using (MySqlConnection connection = new MySqlConnection(MainWindow.connectionString))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(finish_game, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@_game_id", MainWindow.GameId);
                cmd.Parameters["@_game_id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@_winner_id", MySqlDbType.Int32);
                cmd.Parameters["@_winner_id"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                winnerId = (int)cmd.Parameters["@_winner_id"].Value;
            }

            ShowResults();
        }

        #endregion



        #region NavigationMethods

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CleanGameInfo();
            NavigationService.Navigate(new Uri("MainMenu.xaml", UriKind.Relative));
        }

        #endregion
    }
}
