CREATE DATABASE Game_21;
USE Game_21;

set @@auto_increment_increment = 1;

CREATE TABLE Players(
	`id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`name` VARCHAR(50) NOT NULL,
	`wins` INT NOT NULL DEFAULT 0,
	`amount_of_games` INT NOT NULL DEFAULT 0
);

CREATE INDEX player_wins_index ON Players(wins);

CREATE TABLE Game(
	`id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`hands_turn` INT,
	`dealer_first_card_id` INT,
	`is_started` TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE Hands(
	`id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`player_id` INT NOT NULL,
	`next_hand_id` INT,
	`game_id` INT NOT NULL,
	`is_dealer` TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE Cards(
	`id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`value` INT NOT NULL,
	`suit` VARCHAR(1) NOT NULL
);

CREATE TABLE Hand_Cards(
    `hand_id` INT NOT NULL,
    `card_id` INT NOT NULL
);

CREATE TABLE Deck(
    `game_id` INT NOT NULL,
    `card_id` INT NOT NULL
);

CREATE TABLE Constant(
    `max_cards_in_hand`  INT NOT NULL DEFAULT 5,
    `max_players_in_game` INT NOT NULL DEFAULT 7,
    `min_players_in_game` INT NOT NULL DEFAULT 2
);



ALTER TABLE Hands ADD CONSTRAINT FK_Hands_Game FOREIGN KEY(game_id) REFERENCES Game(id) ON DELETE CASCADE;
ALTER TABLE Hands ADD CONSTRAINT FK_Hands_Hands FOREIGN KEY(next_hand_id) REFERENCES Hands(id);
ALTER TABLE Hands ADD CONSTRAINT FK_Hands_Players FOREIGN KEY(player_id) REFERENCES Players(id);
ALTER TABLE Hand_Cards ADD CONSTRAINT FK_HandCards_Hands FOREIGN KEY(hand_id) REFERENCES Hands(id) ON DELETE CASCADE;
ALTER TABLE Hand_Cards ADD CONSTRAINT FK_HandCards_Cards FOREIGN KEY(card_id) REFERENCES Cards(id);
ALTER TABLE Deck ADD CONSTRAINT FK_Deck_Game FOREIGN KEY(game_id) REFERENCES Game(id) ON DELETE CASCADE;
ALTER TABLE Deck ADD CONSTRAINT FK_Deck_Cards FOREIGN KEY(card_id) REFERENCES Cards(id);
ALTER TABLE Game ADD CONSTRAINT FK_Game_Cards FOREIGN KEY(dealer_first_card_id) REFERENCES Cards(id);
ALTER TABLE Game ADD CONSTRAINT FK_Game_Hands FOREIGN KEY(hands_turn) REFERENCES Hands(id);

INSERT INTO cards(`value`, `suit`) VALUES 
('2', 's'), ('3', 's'), ('4', 's'), ('6', 's'), ('7', 's'), ('8', 's'), ('9', 's'), ('10', 's'), ('11', 's'),
('2', 'c'), ('3', 'c'), ('4', 'c'), ('6', 'c'), ('7', 'c'), ('8', 'c'), ('9', 'c'), ('10', 'c'), ('11', 'c'),
('2', 'h'), ('3', 'h'), ('4', 'h'), ('6', 'h'), ('7', 'h'), ('8', 'h'), ('9', 'h'), ('10', 'h'), ('11', 'h'),
('2', 'd'), ('3', 'd'), ('4', 'd'), ('6', 'd'), ('7', 'd'), ('8', 'd'), ('9', 'd'), ('10', 'd'), ('11', 'd');

INSERT INTO Constant VALUES ();

INSERT INTO players(`Name`) VALUES
('Arkadiy'), ('Michael'), ('Konstantin'), ('George'), ('Elena'), ('Julia'), ('Federico');


Select *
From players

Select *
From cards


CALL `game_21`.`register_new_player`('VASYAPUPKIN', @result_status);
SELECT @result_status;

CALL `game_21`.`login`('VASYAPUPKsIN', @result_status);
SELECT @result_status;




















