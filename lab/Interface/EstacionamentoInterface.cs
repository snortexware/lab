using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Windows;

namespace lab
{
    internal class EstacionamentoInterface
    {


        public string DbFile { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "dados", "Estacionamento.db");

        public SqliteConnection SimplesDbConnection()
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

                        cnn.Execute(@"CREATE TABLE IF NOT EXISTS Estacionamento(Placa TEXT NOT NULL, Entrada TEXT NOT NULL, Saida TEXT,Duracao TEXT, TempoEscolhido INTEGER, PriceRow VARCHAR(255), TotalPrice VARCHAR(255)) ;");
                        cnn.Execute(@"CREATE TABLE IF NOT EXISTS Prices(Hora INT, Price INT, Validation_start TEXT NOT NULL, Validation_end TEXT NOT NULL);");
                        cnn.Execute("INSERT INTO Prices(Price, Hora, Validation_start, Validation_end) VALUES (@Price, @Hora, @Validation_start, @Validation_end)",
                        new { Price = 2, Hora = 1, Validation_start = "01/01/2024", Validation_end = "31/12/2024" });


                        MessageBox.Show("Db criada com sucesso");

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
