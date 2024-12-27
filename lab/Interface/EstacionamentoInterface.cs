using System.Text;
using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Windows;

namespace lab
{
    public class EstacionamentoInterface
    {

        public static string DbFile { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "dados", "Estacionamento.db");

        public static SqliteConnection SimplesDbConnection()
        {

            string connection = $"Data Source={DbFile}";

            return new SqliteConnection(connection);
        }

        public void CreateDb()
        {


            if (!Directory.Exists(Path.GetDirectoryName(DbFile)))
            {

                Directory.CreateDirectory(Path.GetDirectoryName(DbFile));
            }


            if (!File.Exists(DbFile))
            {

                StreamWriter file = new StreamWriter(DbFile, true, Encoding.Default);

                file.Dispose();

                using (var cnn = SimplesDbConnection())
                {

                    try
                    {
                        cnn.Open();

                        cnn.Execute(@"CREATE TABLE IF NOT EXISTS Estacionamento( Placa TEXT NOT NULL, Entrada TEXT NOT NULL,
                        Saida TEXT DEFAULT 'Aguardando...' ,Duracao TEXT DEFAULT 'Aguardando...', Status VARCHAR(255),
                        TempoEscolhido INTEGER, PriceRow VARCHAR(255), TotalPrice VARCHAR(255)) ;");

                        cnn.Execute(@"CREATE TABLE IF NOT EXISTS Prices(Hora INT, Price INT, Validation_start TEXT NOT NULL,
                        Validation_end TEXT NOT NULL);");
                        cnn.Execute("INSERT INTO Prices(Price, Hora, Validation_start, Validation_end) VALUES (@Price, @Hora, @Validation_start, @Validation_end)",
                        new { Price = 2, Hora = 1, Validation_start = "2024-01-01", Validation_end = "2024-12-31" });


                        MessageBox.Show("Banco de dados criado com sucesso, pronto para uso");

                        cnn.Close();


                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);

                    }

                }

            }


        }

    }
}
