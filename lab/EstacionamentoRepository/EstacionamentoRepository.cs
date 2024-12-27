using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using ControlzEx.Standard;
using Dapper;
using Microsoft.Data.Sqlite;

namespace lab
{
    public class EstacionamentoRepository : EstacionamentoInterface
    {

        public SqliteConnection cnn;

        public EstacionamentoRepository()
        {
            cnn = SimplesDbConnection();
        }

        public ObservableCollection<Carros> GetAtivos()
        {

            var observableList = new ObservableCollection<Carros>();

            cnn.Open();

            var values = cnn.Query<Carros>(@"SELECT * FROM Estacionamento");

            foreach (var item in values)
            {

                observableList.Add(item);

            }
            cnn.Close();

            return observableList;

        }

        public int GetPriceAtivo()
        {

            cnn.Open();

            int valorDb = cnn.Query<int>(@"SELECT Price FROM Prices
            WHERE DATE(Validation_Start) <= DATE('now') AND DATE(Validation_end) >= DATE('now')").SingleOrDefault();

            cnn.Close();

            return valorDb;


        }

        public void AtualizaEntrada(string placa, string entradaData, int tempoescolhido, string valorAtual, string totalprice)
        {

            cnn.Open();

            cnn.Execute(@"INSERT INTO Estacionamento(Placa, Entrada, TempoEscolhido, PriceRow,
            TotalPrice, Status) VALUES(@_placa, @_entrada, @_tempoEscolhido, @_pricerow, @_totalprice, @_status);",
            new
            {
                _placa = placa,
                _entrada = entradaData,
                _tempoEscolhido = tempoescolhido,
                _pricerow = valorAtual,
                _totalprice = totalprice,
                _status = "aguardando"
            });

            cnn.Close();

        }

        public void AtualizaSaida(string saida, string placa, string valorNovoFormatado, string duracao)
        {

            cnn.Open();

            cnn.Execute(@"UPDATE Estacionamento SET Saida = @saida_,Duracao = @duracao_, TotalPrice = @totalpricenew,
            Status = @_status WHERE Placa = @placa_",
            new
            {
                saida_ = saida,
                placa_ = placa,
                totalpricenew = valorNovoFormatado,
                duracao_ = duracao,
                _status = "finalizado"
            });

            cnn.Close();

        }



    }
}
