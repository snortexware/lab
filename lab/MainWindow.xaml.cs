using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;

namespace lab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Carros> CarrosAtivos { get; set; } = new ObservableCollection<Carros>();
        public MainWindow()
        {



            InitializeComponent();
            EstacionamentoInterface interface1 = new EstacionamentoInterface();

            interface1.CreateDb();
            var repo = new EstacionamentoRepository();


            var ativosList = repo.GetAtivos();
            foreach (var item in ativosList)
            {
                CarrosAtivos.Add(item);
            }

            ativos.ItemsSource = CarrosAtivos;


        }



        private void saida(object sender, RoutedEventArgs e)
        {

            var selecionado = ativos.SelectedItem as Carros;

            DateTime date = DateTime.Now;
            string saida = date.ToString("dd/MM/yyyy HH:mm:ss");

            selecionado.Saida = saida;


            var x = selecionado.Entrada.ToString();

            DateTime.TryParseExact(x, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed);


            TimeSpan difference = date - parsed;

            int totalHoras = (int)difference.TotalHours;
            int totalMinutos = (int)difference.TotalMinutes;

            int tempoExcedente = totalMinutos / 60;



            string TotalFormatado = $"{(int)totalHoras:D2}:{totalMinutos:D2}";


            int minutosTolerancia = 10;


            int valorNovo = int.Parse(selecionado.PriceRow.Substring(3));



            if (tempoExcedente > minutosTolerancia)
            {

                valorNovo++;

            }

            if (totalMinutos < 30)
            {
                valorNovo /= 2;
            }

            string valorNovoFormatado = "R$ " + valorNovo.ToString();



            selecionado.TotalPrice = valorNovoFormatado;
            selecionado.Duracao = valorNovoFormatado.ToString();


            using (var cnn = new EstacionamentoInterface().SimplesDbConnection())
            {
                cnn.Open();

                cnn.Execute(@"UPDATE Estacionamento SET Saida = @saida_,Duracao = @duracao_, TotalPrice = @totalpricenew WHERE Placa = @placa_", new { saida_ = saida, placa_ = selecionado.Placa, totalpricenew = valorNovoFormatado, duracao_ = TotalFormatado });




            }

            ativos.Items.Refresh();


        }


        private void inputData(object sender, RoutedEventArgs e)
        {

            var cnn = new EstacionamentoInterface().SimplesDbConnection();

            cnn.Open();

            DateTime date = DateTime.Now;
            string entrada = date.ToString("dd/MM/yyyy HH:mm:ss");



            var value = cnn.Query<int>(@"SELECT Price FROM Prices WHERE Price >= 0");


            foreach (var item in value)
            {

                try
                {

                    int tempoConvertido = int.Parse(tempo.Text);

                    int valorAtual = tempoConvertido * item;

                    string valorAtualConvertido = "R$ " + valorAtual.ToString();

                    string valorConvertidoBanco = "R$ " + item.ToString();

                    string placaInput = plate.Text;

                    int tempoInputConvertido = int.Parse(tempo.Text);


                    CarrosAtivos.Add(new Carros
                    {
                        Placa = placaInput,
                        Entrada = entrada,
                        TempoEscolhido = tempoInputConvertido,
                        PriceRow = valorConvertidoBanco,
                        TotalPrice = valorAtualConvertido

                    });


                    cnn.Execute(@"INSERT INTO Estacionamento(Placa, Entrada, TempoEscolhido, PriceRow, TotalPrice) VALUES(@value, @Entrada1, @tempoescolhido, @pricerow, @totalprice);",
                    new { value = plate.Text, Entrada1 = entrada, tempoescolhido = tempoConvertido, pricerow = valorConvertidoBanco, totalprice = valorAtualConvertido });


                    plate.Text = string.Empty;
                    tempo.Text = string.Empty;


                    cnn.Close();


                }
                catch (Exception ex)
                {

                    MessageBox.Show("Ocorreu um erro ao processar os dados digitados nos campos");

                }





            }








        }

        private void TemplateTextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(plate.Text) ? Visibility.Visible : Visibility.Collapsed;
            PlaceHolderTempo.Visibility = string.IsNullOrEmpty(tempo.Text) ? Visibility.Visible : Visibility.Collapsed;

        }



        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ativos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}