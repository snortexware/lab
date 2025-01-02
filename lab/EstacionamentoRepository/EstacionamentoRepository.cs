using System.Collections.ObjectModel;
using System.Windows;
using Dapper;
using Microsoft.Data.Sqlite;

namespace lab
{
    public class EstacionamentoRepository
    {

        public SqliteConnection cnn;

        public EstacionamentoRepository()
        {
            cnn = IEstacionamentoInterface.SimplesDbConnection();
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

        public async Task AtualizaEntrada(string placa, string entradaData, int tempoescolhido, string valorAtual, string totalprice)
        {

            cnn.Open();

            await cnn.ExecuteAsync(@"INSERT INTO Estacionamento(Placa, Entrada, TempoEscolhido, PriceRow,
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

        public async Task AtualizaSaida(string saida, string placa, string valorNovoFormatado, string duracao)
        {

            cnn.Open();

            await cnn.ExecuteAsync(@"UPDATE Estacionamento SET Saida = @saida_,Duracao = @duracao_, TotalPrice = @totalpricenew,
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
        public async Task TransferirInativo(string placa)
        {
            cnn.Open();


            var carroAtivo = await cnn.QueryAsync(@"SELECT * FROM Estacionamento WHERE Placa = @_placa", new
            {
                _placa = placa
            });

            string transferAtivo = @"
            INSERT INTO Inativos (Placa, Entrada, Saida, Duracao, Status, TempoEscolhido, PriceRow, TotalPrice) 
            VALUES (@Placa, @Entrada, @Saida, @Duracao, @Status, @TempoEscolhido, @PriceRow, @TotalPrice)";

            foreach (var car in carroAtivo)
            {
                var parameters = new
                {
                    car.Placa,
                    car.Entrada,
                    car.Saida,
                    car.Duracao,
                    car.Status,
                    car.TempoEscolhido,
                    car.PriceRow,
                    car.TotalPrice
                };

                await cnn.ExecuteAsync(transferAtivo, parameters);
            }

            await cnn.ExecuteAsync(@"DELETE FROM Estacionamento WHERE Placa = @_placa", new
            {
                _placa = placa
            });


            cnn.Close();

        }



    }
}
