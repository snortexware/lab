using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using lab.Interface;


namespace lab
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Placa { get; set; } = string.Empty;
        public string? Tempo { get; set; } = string.Empty;


        private ObservableCollection<Carros>? _mostraRows;

        private ObservableCollection<Carros> MostraRows
        {
            get => _mostraRows;
            set
            {
                _mostraRows = value;
                OnPropertyChanged(nameof(MostraRows));
                ativos.Items.Refresh();
            }
        }

        private ObservableCollection<Carros> Original { get; set; } = new ObservableCollection<Carros>();

        private ObservableCollection<Carros>? Filtrado { get; set; }



        public MainWindow()
        {

            InitializeComponent();

            ProcessaData();

        }



        public void ProcessaData()
        {


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

        private void Entrada(object sender, RoutedEventArgs e)
        {

            try
            {

                ProcessaEntrada();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "O seguinte erro ao processar a data aconteceu:");

            }

        }

        private void ProcessaEntrada()
        {

            var dadosEntrada = new EstacionamentoService();

            var repositorio = new EstacionamentoRepository();



            var resultado = dadosEntrada.CalculaEntrada(Tempo);


            if (!resultado.sucesso)
            {

                MessageBox.Show(resultado.mensagem);

                return;

            }


            Original.Add(new Carros
            {
                Placa = Placa,
                Entrada = dadosEntrada.EntradaData,
                Saida = "Aguardando...",
                Duracao = "Aguardando...",
                TempoEscolhido = dadosEntrada.TempoEntradaConvertido,
                PriceRow = dadosEntrada.ValorConvertidoBanco,
                TotalPrice = dadosEntrada.ValorAtual

            });

            repositorio.AtualizaEntrada(Placa,
                dadosEntrada.EntradaData,
                dadosEntrada.TempoEntradaConvertido,
                dadosEntrada.ValorConvertidoBanco,
                dadosEntrada.ValorAtual);


            Placa = string.Empty;
            Tempo = string.Empty;


            MostraRows = Original;

            OnPropertyChanged(nameof(MostraRows));
            OnPropertyChanged(nameof(Placa));
            OnPropertyChanged(nameof(Tempo));

            ativos.Items.Refresh();





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

            var dadosSaida = new EstacionamentoService();
            var repositorio = new EstacionamentoRepository();

            dadosSaida.CalculaSaida(selecionado);

            selecionado.TotalPrice = dadosSaida.ValorNovoFormatadoSaida;
            selecionado.Duracao = dadosSaida.DuracaoFinal;
            selecionado.Saida = dadosSaida.Saida;

            repositorio.AtualizaSaida(dadosSaida.Saida, selecionado.Placa, dadosSaida.ValorNovoFormatadoSaida, dadosSaida.DuracaoFinal);

            OnPropertyChanged(nameof(MostraRows));
            ativos.Items.Refresh();
        }


        private void TextoVisivel(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(Placa) ? Visibility.Visible : Visibility.Collapsed;
            PlaceHolderTempo.Visibility = string.IsNullOrEmpty(Tempo) ? Visibility.Visible : Visibility.Collapsed;
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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}