DROP DATABASE IF EXISTS `chessmatesdb`;
CREATE DATABASE IF NOT EXISTS `chessmatesdb`
	DEFAULT CHARACTER SET utf8 COLLATE utf8_bin;
USE `chessmatesdb`;

DROP TABLE IF EXISTS `pairs`;
DROP TABLE IF EXISTS `players`;

CREATE TABLE IF NOT EXISTS `players` (
	`id_player` INT NOT NULL AUTO_INCREMENT ,
	`firstname` VARCHAR(30) NOT NULL ,
	`lastname` VARCHAR(30) ,
	`fiderank` INT ,
	`birthyear` INT ,
	`country` VARCHAR(30) ,
	PRIMARY KEY (`id_player`)
)  ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

CREATE TABLE IF NOT EXISTS `pairs` (
	`id` INT NOT NULL AUTO_INCREMENT ,
	`round` INT NOT NULL ,
	`player1` INT NOT NULL ,
	`player2` INT NOT NULL ,
	PRIMARY KEY (`id`) ,
	FOREIGN KEY fk_player1(player1)
		REFERENCES players(id_player)
		ON UPDATE CASCADE
		ON DELETE RESTRICT ,
	FOREIGN KEY fk_player2(player2)
		REFERENCES players(id_player)
		ON UPDATE CASCADE
		ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

INSERT INTO `players` (`firstname`, `lastname`, `fiderank`, `birthyear`, `country`)
    VALUES ("Luka", "Petrovic", 1768, 1998, "Serbia"),
    ("Kristina", "Tomcic", 1945, 1989, "Serbia"),
    ("Maja", "Lukic", 1711, 2000, "Serbia"),
    ("Alexander", "Szekeres", 1888, 1986, "Hungary"),
    ("Evelin", "Magyar", 1690, 2002, "Hungary"),
    ("Zvonko", "Arihmedovic", 1665, 1977, "BiH"),
	("Ivana", "Jurisa", 0, 1991, "Croatia"),
    ("Angel", "Mateev", 2054, 1994, "Bulgaria"),
	("Nikolay", "Maryanov", 2001, 1985, "Bulgaria"),
	("Yosif", "Martinov", 1876, 1987, "Bulgaria");