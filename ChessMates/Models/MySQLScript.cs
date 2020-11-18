using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMates.Models
{
    public class MySQLScript
    {
        public MySQLScript(){}

        /*  CREATE database CHESSMATESDB    */
        public static void CreateDatabase(MySqlConnection connection, string connString)
        {
            string db = "";
            if (connString == "ChessMatesDB")
                db = "chessmatesdb";
            else if (connString == "RemoteMySQLDB")
                db = "1eepySKaVU";

            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            connection.Open();
            command.CommandText = "drop database if exists " + db;
            command.ExecuteNonQuery();
            command.CommandText = "create database if not exists " + db + " default character set utf8 collate utf8_bin";
            command.ExecuteNonQuery();
            command.CommandText = "use " + db;
            command.ExecuteNonQuery();
            connection.Close();
        }

        /*  CREATE table PLAYERS    */
        public static void CreateTablePlayers(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            connection.Open();
            command.CommandText = @"drop table if exists players";
            command.ExecuteNonQuery();

            command.CommandText = @"create table if not exists players(id_player int primary key not null auto_increment,
                                        firstname text not null, lastname text, fiderank int,
                                        birthyear int, country text)";
            command.ExecuteNonQuery();
            connection.Close();
        }

        /*  INSERT data into table PLAYERS  */
        public static void InsertIntoPlayers(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            connection.Open();
            List<string> FirstNames = new List<string>{ "Marija", "Luka", "Adel", "Bogdana", "Bojan",
                    "Grozdana", "Boris", "Yosif", "Gizi", "Erzsebet", "Dmitar", "Kira", "Jani", "Janko", "Yuliana"};
            List<string> LastNames = new List<string>(){ "Tomic", "Antov", "Fejes", "Sokolov", "Bachvarov",
                    "Molnar", "Kovac", "Stankovic", "Tar", "Antic", "Sarkozi", "Vukovic", "Radic", "Grbic", "Novak"};
            List<int> FideRanks = new List<int>() { 1987, 1659, 2001, 1981, 2309, 2180, 1657, 1999, 1909, 1879, 1776, 2111, 1950, 1645, 2430 };
            List<int> BirthYears = new List<int>() { 1978, 1999, 1986, 1992, 1996, 2002, 2005, 1975, 1969, 1988, 1997, 2003, 1956, 1971, 1949 };
            List<string> Countries = new List<string>() { "Serbia", "Bulgaria", "Hungary", "Croatia", "B&H" };
            connection.Close();

            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = "insert into players (firstname, lastname, fiderank, birthyear, country)" +
                "values (@firstname, @lastname, @fiderank, @birthyear, @country)";
                connection.Open();
                command.Parameters.AddWithValue("@firstname", FirstNames[rand.Next(0, 14)]);
                command.Parameters.AddWithValue("@lastname", LastNames[rand.Next(0, 14)]);
                command.Parameters.AddWithValue("@fiderank", FideRanks[rand.Next(0, 14)]);
                command.Parameters.AddWithValue("@birthyear", BirthYears[rand.Next(0, 14)]);
                command.Parameters.AddWithValue("@country", Countries[rand.Next(0, 4)]);
                command.Prepare();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /*  CREATE table PAIRS  */
        public static void CreateTablePairs(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            connection.Open();
            command.CommandText = "drop table if exists pairs";
            command.ExecuteNonQuery();

            command.CommandText = @"create table if not exists pairs(id int primary key not null auto_increment,
                                        round int not null, player1 int not null, player2 int not null,
                                        foreign key fk_player1(player1) references players(id_player)
                                        on update cascade on delete restrict, foreign key fk_player2(player2)
                                        references players(id_player) on update cascade on delete restrict)";
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void TruncateTablePairs(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            connection.Open();
            command.CommandText = "truncate table pairs";
            command.ExecuteNonQuery();
            connection.Close();
        }
        public static void TruncateTablePlayers(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            connection.Open();
            //command.CommandText = "truncate table players";
            command.CommandText = "delete from players; alter table players auto_increment = 1;";
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void TruncateTables(MySqlConnection connection)
        {
            MySQLScript.TruncateTablePairs(connection);
            MySQLScript.TruncateTablePlayers(connection);
        }

        public static void Script(string connString)
        {
            string cs = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(cs))
            {
                
                CreateDatabase(connection, connString);
                CreateTablePlayers(connection);
                InsertIntoPlayers(connection);
                CreateTablePairs(connection);
            }
        }
    }
}
