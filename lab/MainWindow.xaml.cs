using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
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



            string formatado = $"{(int)difference.TotalHours:D2}:{difference.Minutes:D2}";


            selecionado.Duracao = formatado;







            using (var cnn = new EstacionamentoInterface().SimplesDbConnection())


            {



                cnn.Execute(@"UPDATE Estacionamento SET Saida = @saida_ WHERE Placa = @placa_", new { saida_ = saida, placa_ = selecionado.Placa.ToString() });










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

                int b = int.Parse(tempo.Text);

                int value1;

                value1 = b * item;

                MessageBox.Show(item.ToString());
                MessageBox.Show(b.ToString());
                MessageBox.Show(value1.ToString());

                MessageBox.Show(value1.ToString());



                CarrosAtivos.Add(new Carros
                {
                    Placa = plate.Text,
                    Entrada = entrada,
                    Time = int.Parse(tempo.Text),
                    Price = "R$ " + item.ToString(),
                    FinalPrice = "R$ " + value1.ToString()

                });

                plate.Text = string.Empty;


                cnn.Execute(@"INSERT INTO Estacionamento(Placa, Entrada) VALUES(@value, @Entrada1);",
                new { value = plate.Text, Entrada1 = entrada });


            }








        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ativos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}