using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Data;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PdfSharp.Drawing;
using System.Windows.Documents;
using Paragraph = System.Windows.Documents.Paragraph;
using ListItem = System.Windows.Documents.ListItem;
using System.Windows.Controls;
using System.Xaml;
using System.Xml;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using Table = Microsoft.Office.Interop.Word.Table;

namespace ChessMates.Models
{
    /*  Berger / Round Robin sistem
        igra svako sa svakim tacno jednom   */

    /*  Berger / Round Robin system
        each player plays with each player once */
    public class BergerPairingAlgorithm : IPairingAlgorithm
    {
        //  funkcija za dodavanje parova u bazu
        //  function for adding pairs to the database
        public void AddPairsToDB(int player1, int player2, int round, MySqlCommand command, MySqlConnection connection)
        {
            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = "insert into pairs (round, player1, player2)" +
                "values (@round, @player1, @player2)";
            connection.Open();
            command.Parameters.AddWithValue("@round", round);

            /*  igraci sa istorodnim (parnim/neparnim) id-jevima:
                    -> bele figure ima igrac sa vecim id-jem
                igraci sa raznorodnim id-jevima:
                    -> bele figure ima igrac sa manjim id-jem   */

            /*  players with the homogeneous (even/odd) IDs:
                    -> white is the one with higher ID
                players with heterogeneous IDs:
                    -> white is the one with lower ID   */
            if ((IsHomogeneous(player1, player2) && player1 > player2)
                || !IsHomogeneous(player1, player2) && player1 < player2)
            {
                Console.WriteLine("{0} vs {1}", player1, player2);
                command.Parameters.AddWithValue("@player1", player1);
                command.Parameters.AddWithValue("@player2", player2);
            }
            else
            {
                Console.WriteLine("{0} vs {1}", player2, player1);
                command.Parameters.AddWithValue("@player1", player2);
                command.Parameters.AddWithValue("@player2", player1);
            }
            command.Prepare();
            command.ExecuteNonQuery();
            connection.Close();
        }

        //  funkcija za ispitivanje istorodnosti id-jeva igraca
        //  function for checking homogeneity of players IDs
        public bool IsHomogeneous(int player1, int player2)
        {
            return Math.Abs(player1 - player2) % 2 == 0;
        }

        //  funkcija za kreiranje Word reporta sacinjenog od parova kola na turniru
        //  function for creating Word report made of tournament pairings
        public void DocReport(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            connection.Open();
            command.CommandText = "select pairs.round, p1.firstname, p1.lastname, p2.firstname, p2.lastname " +
                "from pairs " +
                "left join players p1 on (p1.id_player = pairs.player1) " +
                "left join players p2 on (p2.id_player = pairs.player2);";
           
            command.ExecuteNonQuery();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = command;
            adapter.Fill(dataSet);
            connection.Close();

            string round = null;
            string player1 = null;
            string player2 = null;

            try
            {
                Microsoft.Office.Interop.Word.Application winword = new Microsoft.Office.Interop.Word.Application();
                winword.ShowAnimation = false;
                winword.Visible = false;
                object missing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word.Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            
                // header
                foreach (Microsoft.Office.Interop.Word.Section section in document.Sections)
                {
                    Microsoft.Office.Interop.Word.Range headerRange = section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage);
                    headerRange.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = Microsoft.Office.Interop.Word.WdColorIndex.wdBlue;
                    headerRange.Font.Size = 10;
                    headerRange.Text = "ChessMates Pairing System";
                }
                // heading
                Microsoft.Office.Interop.Word.Paragraph heading = document.Content.Paragraphs.Add(ref missing);
                object styleHeading2 = "Heading 1";
                heading.Range.set_Style(ref styleHeading2);
                heading.Range.Text = "Tournament pairings";
                heading.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                heading.Range.InsertParagraphAfter();


                // table
                int numRows = dataSet.Tables[0].Rows.Count;
                int numCols = 3;
                Table dataTable = document.Tables.Add(heading.Range, numRows, numCols, ref missing, ref missing);
                int rowIdx = -1;
                dataTable.Borders.Enable = 1;
                foreach (Row row in dataTable.Rows)
                {
                    row.Alignment = WdRowAlignment.wdAlignRowCenter;
                    foreach (Cell cell in row.Cells)
                    {
                        if (cell.RowIndex == 1)
                        {
                            cell.Range.Font.Bold = 1;
                            if(cell.ColumnIndex == 1)
                            {
                                cell.Range.Text = "Round";
                            }
                            else if (cell.ColumnIndex == 2)
                            {
                                cell.Range.Text = "White player";
                            }
                            else if (cell.ColumnIndex == 3)
                            {
                                cell.Range.Text = "Black player";
                            }
                            cell.Range.Font.Bold = 1;
                            cell.Range.Font.Name = "verdana";
                            cell.Range.Font.Size = 10;                      
                            cell.Shading.BackgroundPatternColor = WdColor.wdColorLightBlue;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        else
                        {
                            round = dataSet.Tables[0].Rows[rowIdx].ItemArray[0].ToString();
                            player1 = dataSet.Tables[0].Rows[rowIdx].ItemArray[1].ToString()
                                + " " + dataSet.Tables[0].Rows[rowIdx].ItemArray[2].ToString();
                            player2 = dataSet.Tables[0].Rows[rowIdx].ItemArray[3].ToString()
                                + " " + dataSet.Tables[0].Rows[rowIdx].ItemArray[4].ToString();

                            if (cell.ColumnIndex == 1)
                            {
                                cell.Range.Text = round;
                            }
                            else if (cell.ColumnIndex == 2)
                            {
                                cell.Range.Text = player1;
                            }
                            else if (cell.ColumnIndex == 3)
                            {
                                cell.Range.Text = player2;
                            }
                        }
                    }
                    rowIdx++;
                }

                /*  dodatak: korisnik sam bira gde zeli da sacuva report    */
                /*  appendix: user has the privilege to choose where should report be saved */
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
                saveFileDlg.FileName = "DocReport";
                saveFileDlg.Title = "Save report as";
                saveFileDlg.DefaultExt = ".docx";
                saveFileDlg.Filter = "Word Documents (.docx)|*.docx";
                string filename = "";
                string dataDir = "";
                Nullable<bool> result = saveFileDlg.ShowDialog();
                if (result == true)
                {
                    filename = saveFileDlg.FileName;
                    int idx = filename.LastIndexOf("\\");

                    if (idx != -1)
                    {
                        dataDir = filename.Substring(0, idx);
                    }
                }

                //string dataDir = Path.GetFullPath("../../PDF/");

                object fullFilename = filename;
                document.SaveAs2(ref fullFilename);
                document.Close(ref missing, ref missing, ref missing);
                document = null;
                winword.Quit(ref missing, ref missing, ref missing);
                winword = null;

                ConvertDocToPdf(dataDir, filename);

                MessageBox.Show("Report created successfully !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Report could not be created");
            }
        }

        //  funkcija za konvertovanje .doc(x) fajla u .pdf fajl
        //  function for converting .doc(x) into .pdf file
        public void ConvertDocToPdf(string dataDir, string filename)
        {
            //string dataDir = Path.GetFullPath("../../PDF/");
            
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = wordApp.Documents.Open(filename);

            wordDocument.ExportAsFixedFormat(dataDir + "/TournamentPairs.PDF", Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);

            wordDocument.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges,
                               Microsoft.Office.Interop.Word.WdOriginalFormat.wdOriginalDocumentFormat, false);

            wordApp.Quit();

            /*  dodatak: report se automatski otvara nakon kreiranja    */
            /*  appendix: report automatically opens after its creation */
 
            Process.Start(dataDir + "/TournamentPairs.PDF");
        }


        // glavna funkcija algoritma za parovanje
        // koristi pomocne funkcije IsHomogenous(...) i AddPairsToDB(...)
        public void Pair(string connString)
        {
            string cs = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(cs))
            {
                
                MySQLScript.CreateTablePairs(connection);   //  da se ne bi dodavali parovi na vec postojece
                                                            //  pairs are not appended to an existing ones

                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                connection.Open();
                string query = "SELECT * FROM players";
                command = new MySqlCommand(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                List<int> players = new List<int>();
                while (reader.Read())
                {
                    int player = reader.GetInt32(0);
                    players.Add(player);
                }
                reader.Close();

                /*  ukoliko je neparan broj igraca
                    dodaje se 1 virtuelni igrac    */

                /*  if there is odd number of players
                    a virtual player is being added */
                if (players.Count % 2 != 0)
                {
                    players.Add(players.Count+1);
                    command = new MySqlCommand();
                    command.Connection = connection;
                    command.CommandText = "insert into players (firstname)" +
                        "values (@firstname)";
                    command.Parameters.AddWithValue("@firstname", "Bye");
                    command.Prepare();
                    command.ExecuteNonQuery();
                }
                connection.Close();

                int numPlayers = players.Count();
                int numRounds = (numPlayers - 1);
                int halfPlayers = numPlayers / 2;

                List<int> contestants = new List<int>();

                contestants.AddRange(players.Skip(halfPlayers).Take(halfPlayers));
                contestants.AddRange(players.Skip(1).Take(halfPlayers - 1).ToArray().Reverse());

                int numContestants = contestants.Count;

                for (int round = 0; round < numRounds; round++)
                {
                    Console.WriteLine("\nRound {0}", (round + 1));
                    int contestantIdx = round % numContestants;
                    AddPairsToDB(contestants[contestantIdx], players[0], (round+1), command, connection);
                    
                    for (int idx = 1; idx < halfPlayers; idx++)
                    {
                        int first = (round + idx) % numContestants;
                        int second = (round + numContestants - idx) % numContestants;
                        AddPairsToDB(contestants[first], contestants[second], (round+1), command, connection);
                    }
                }

                DocReport(connection);
                connection.Close();
            }
        }
    }
}
