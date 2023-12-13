-- phpMyAdmin SQL Dump
-- version 5.1.3
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Creato il: Feb 19, 2023 alle 14:38
-- Versione del server: 10.4.24-MariaDB
-- Versione PHP: 7.4.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

-- Dump della struttura di tabella mezz.give_away_blocks
CREATE TABLE IF NOT EXISTS `give_away_blocks` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `user_id` int(10) NOT NULL,
  `added` double NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `user_id` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=latin1;


-- Dump dei dati di tabella mezz.server_settings 

INSERT INTO `server_settings` (`variable`, `value`, `description`) VALUES ('give_away.block.minutes', '2', '');
INSERT INTO `server_settings` (`variable`, `value`, `description`) VALUES ('give_away.give.credits', '10', '');
INSERT INTO `server_settings` (`variable`, `value`, `description`) VALUES ('give_away.give.diamonds', '1', '');
INSERT INTO `server_settings` (`variable`, `value`, `description`) VALUES ('give_away.give.duckets', '0', '');
INSERT INTO `server_settings` (`variable`, `value`, `description`) VALUES ('give_away.give.item', '1', '');



-- Dump della struttura di tabella mezz.system_commands

INSERT INTO `system_commands` (`id`, `input`, `minrank`, `description_fr`, `description_en`, `description_br`, `description_es`) VALUES (279, 'cleargiveawayblocks', 19, '', '', '', '');
INSERT INTO `system_commands` (`id`, `input`, `minrank`, `description_fr`, `description_en`, `description_br`, `description_es`) VALUES (278, 'giveaway', 1, '', '', '', '');
INSERT INTO `system_commands` (`id`, `input`, `minrank`, `description_fr`, `description_en`, `description_br`, `description_es`) VALUES (277, 'setz', 1, '', '', '', '');
INSERT INTO `system_commands` (`id`, `input`, `minrank`, `description_fr`, `description_en`, `description_br`, `description_es`) VALUES (276, 'w', 4, '', '', '', 'Se uasa para hablar con los miembros del  equipo staff por susurros\n');

-- Dump della struttura di tabella mezz.fuserights

INSERT INTO `fuserights` (`id`, `rank`, `fuse`) VALUES (51, 19, 'giveaway_manager_command');

-- Fix imtem_moodlight

CREATE TABLE IF NOT EXISTS `item_moodlight` (
    `item_id` INTEGER UNSIGNED NOT NULL,
    `enabled` BOOLEAN NOT NULL DEFAULT false,
    `current_preset` INTEGER UNSIGNED NOT NULL,
    `preset_one` VARCHAR(200) NOT NULL,
    `preset_two` VARCHAR(200) NOT NULL,
    `preset_three` VARCHAR(200) NOT NULL,

    UNIQUE INDEX `item_id`(`item_id`),
    PRIMARY KEY (`item_id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Dump della struttura del database mezz
CREATE DATABASE IF NOT EXISTS `mezz` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;
USE `mezz`;

-- Dump della struttura di tabella mezz.catalog_items

ALTER TABLE `catalog_items` ADD `offer_id` INT NOT NULL DEFAULT 1 AFTER `badge`;


-- Dump dei dati di tabella mezz.catalog_items

UPDATE catalog_items SET offer_id =item_id WHERE offer_id;

