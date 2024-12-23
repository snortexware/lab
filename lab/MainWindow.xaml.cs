using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dapper;


namespace lab
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<Carros> _mostraRows;

        public ObservableCollection<Carros> MostraRows
        {
            get => _mostraRows;
            set
            {
                _mostraRows = value;
                OnPropertyChanged(nameof(MostraRows));
                ativos.Items.Refresh();
            }
        }

        public ObservableCollection<Carros> Original { get; set; } = new ObservableCollection<Carros>();

        public ObservableCollection<Carros> Filtrado { get; set; }

        public string status;


        public MainWindow()
        {



            InitializeComponent();
            EstacionamentoInterface interface1 = new();

            interface1.CreateDb();

            var repo = new EstacionamentoRepository();

            var ativosList = repo.GetAtivos();
            foreach (var item in ativosList)
            {
                Original.Add(item);
            }

            MostraRows = Original;

            DataContext = this;

        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Saida(object sender, RoutedEventArgs e)
        {

            var selecionado = ativos.SelectedItem as Carros;

            if (selecionado is not null)
            {
                ProcessaSaida();
            }
            else
            {
                MessageBox.Show("Houve um erro ao processar a saida do veiculo");

            }


        }


        private void ProcessaSaida()
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

            int minutosFormatados = totalMinutos % 60;

            string TotalFormatado = $"{totalHoras:D2}:{minutosFormatados:D2}";


            int minutosTolerancia = 10;

            string valorNaoEspecial = selecionado.PriceRow.Replace("0", "").Replace(",", "");

            int valorNovo = int.Parse(valorNaoEspecial.Substring(3));



            if (minutosFormatados > minutosTolerancia)
            {

                valorNovo++;

            }

            if (totalMinutos < 30)
            {
                valorNovo /= 2;
            }


            string valorNovoFormatado = valorNovo.ToString("C", CultureInfo.CurrentCulture);

            selecionado.TotalPrice = valorNovoFormatado;
            selecionado.Duracao = TotalFormatado;


            using (var cnn = new EstacionamentoInterface().SimplesDbConnection())
            {
                cnn.Open();

                status = "Finalizado";

                cnn.Execute(@"UPDATE Estacionamento SET Saida = @saida_,Duracao = @duracao_, TotalPrice = @totalpricenew, Status = @_status WHERE Placa = @placa_", new { saida_ = saida, placa_ = selecionado.Placa, totalpricenew = valorNovoFormatado, duracao_ = TotalFormatado, _status = status });


            }


            OnPropertyChanged(nameof(MostraRows));
            ativos.Items.Refresh();
        }








        private void Entrada(object sender, RoutedEventArgs e)
        {

            try
            {
                ProcessaEntrada();
            }
            catch (Exception ex)
            {


                throw new ArgumentException("Ouve um erro ao processar a entrada", ex.Message);

            }


        }


        private void ProcessaEntrada()
        {


            var cnn = new EstacionamentoInterface().SimplesDbConnection();

            cnn.Open();

            int valorDb = cnn.Query<int>(@"SELECT Price FROM Prices WHERE DATE(Validation_Start) <= DATE('now') AND DATE(Validation_end) >= DATE('now')").SingleOrDefault();

            if (valorDb == 0)
            {

                MessageBox.Show("Não foi possivel Buscar valores no banco de dados");

            }
            else
            {

                DateTime dataHoje = DateTime.Now;

                string entrada = dataHoje.ToString("dd/MM/yyyy HH:mm:ss");

                int tempoEntradaConvertido;


                if (!int.TryParse(tempo.Text, out tempoEntradaConvertido))
                {
                    MessageBox.Show("Digite um tempo valido. Exemplo: '1'");
                    return;
                }


                int valorAtual = tempoEntradaConvertido * valorDb;

                string valorAtualConvertido = valorAtual.ToString("C", CultureInfo.CurrentCulture);

                string valorConvertidoBanco = valorDb.ToString("C", CultureInfo.CurrentCulture);

                string placaInput = plate.Text;

                int tempoInputConvertido = int.Parse(tempo.Text);



                status = "Aguardando...";

                Original.Add(new Carros
                {
                    Placa = placaInput,
                    Entrada = entrada,
                    Duracao = status,
                    Saida = status,
                    TempoEscolhido = tempoInputConvertido,
                    PriceRow = valorConvertidoBanco,
                    TotalPrice = valorAtualConvertido

                });


                cnn.Execute(@"INSERT INTO Estacionamento(Placa, Entrada, TempoEscolhido, PriceRow, TotalPrice, Status) VALUES(@value, @Entrada1, @tempoescolhido, @pricerow, @totalprice, @_status);",
                new { value = plate.Text, Entrada1 = entrada, tempoescolhido = tempoInputConvertido, pricerow = valorConvertidoBanco, totalprice = valorAtualConvertido, _status = status });


                plate.Text = string.Empty;
                tempo.Text = string.Empty;


                cnn.Close();

                MostraRows = Original;

                OnPropertyChanged(nameof(MostraRows));
                ativos.Items.Refresh();

            }



        }


        private void TextoVisivel(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(plate.Text) ? Visibility.Visible : Visibility.Collapsed;
            PlaceHolderTempo.Visibility = string.IsNullOrEmpty(tempo.Text) ? Visibility.Visible : Visibility.Collapsed;
            procurar.Visibility = string.IsNullOrEmpty(txtSearch.Text) ? Visibility.Visible : Visibility.Collapsed;

        }


        public void FiltroSearch(object sender, KeyEventArgs e)
        {

            if (string.IsNullOrEmpty(txtSearch.Text))
            {

                MostraRows = new ObservableCollection<Carros>(Original);

            }
            else
            {

                var filtrados = Original.Where(carro => carro.Placa.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)).ToList();


                Filtrado = new ObservableCollection<Carros>(filtrados);

                MostraRows = Filtrado;

                OnPropertyChanged(nameof(MostraRows));
            }


        }

    }
}