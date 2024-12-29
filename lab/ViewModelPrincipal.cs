using System.Collections.ObjectModel;
using System.ComponentModel;
using lab.Interface;
using System.Windows.Input;
using System.Windows;

namespace lab
{
    public class ViewModelPrincipal : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<Carros>? Filtrado { get; set; }
        public ObservableCollection<Carros> Original { get; set; } = new ObservableCollection<Carros>();

        private ObservableCollection<Carros>? _mostraRows;

        private Carros? _seleciona;

        private Visibility _placavisivel = Visibility.Visible;

        private Visibility _tempovisivel = Visibility.Visible;

        private Visibility _procurarvisivel = Visibility.Visible;

        public ICommand EntradaCommand { get; }
        public ICommand SaidaCommand { get; }
        public ICommand FiltroSearchCommand { get; }
        public ICommand AtualizarTextoVisivelCommand { get; }
        public ICommand ProcessaDataBind { get; }

        private bool _isselected;

        public bool IsSelectAll
        {
            get => _isselected;

            set
            {
                if (_isselected != value)
                {

                    _isselected = value;

                    OnPropertyChanged(nameof(IsSelectAll));

                    foreach (var carro in MostraRows)
                    {

                        carro.IsSelected = value;

                    }


                }
            }
        }


        private string _placaTexto;

        private string _tempotexto;

        private string _procurartexto;

        public string PlacaTexto
        {
            get => _placaTexto;
            set
            {
                if (_placaTexto != value)
                {
                    _placaTexto = value;
                    OnPropertyChanged(PlacaTexto);
                    AtualizarTextoVisivel();
                }
            }
        }

        public string TempoTexto
        {
            get => _tempotexto;

            set
            {
                if (_tempotexto != value)
                {
                    _tempotexto = value;
                    AtualizarTextoVisivel();
                    OnPropertyChanged(TempoTexto);
                }
            }
        }


        public string ProcurarTexto
        {
            get => _procurartexto;
            set
            {
                if (_procurartexto != value)
                {
                    _procurartexto = value;
                    AtualizarTextoVisivel();
                    OnPropertyChanged(ProcurarTexto);
                }
            }
        }



        public ObservableCollection<Carros> MostraRows
        {
            get => _mostraRows;
            set
            {
                _mostraRows = value;
                OnPropertyChanged(nameof(MostraRows));
            }
        }



        public Carros SelecionaCarros
        {
            get => _seleciona;
            set
            {

                _seleciona = value;

            }
        }


        public Visibility PlacaVisivel
        {
            get => _placavisivel;
            set
            {
                if (_placavisivel != value)
                {
                    _placavisivel = value;
                    OnPropertyChanged(nameof(PlacaVisivel));
                }
            }
        }

        public Visibility TempoVisivel
        {
            get => _tempovisivel;
            set
            {

                if (_tempovisivel != value)
                {

                    _tempovisivel = value;
                    OnPropertyChanged(nameof(TempoVisivel));
                    AtualizarTextoVisivel();
                }


            }
        }

        public Visibility ProcurarVisivel
        {
            get => _procurarvisivel;
            set
            {
                if (_procurarvisivel != value)
                {

                    _procurarvisivel = value;
                    OnPropertyChanged(nameof(ProcurarVisivel));
                    AtualizarTextoVisivel();
                }

            }
        }


        public ViewModelPrincipal()
        {

            EntradaCommand = new RelayCommand(Entrada1);
            SaidaCommand = new RelayCommand(Saida1);


            ProcessaData(null);





        }



        public void ProcessaData(object parameter)
        {



            IEstacionamentoInterface.CreateDb();

            var repo = new EstacionamentoRepository();

            var ativosList = repo.GetAtivos();

            foreach (var item in ativosList)
            {
                Original.Add(item);
            }

            MostraRows = Original;

        }

        public void Entrada1(object parameter)
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
            MessageBox.Show(TempoTexto);

            var dadosEntrada = new EstacionamentoService();

            var repositorio = new EstacionamentoRepository();

            dadosEntrada.CalculaEntrada(TempoTexto);

            var resultado = dadosEntrada.CalculaEntrada(TempoTexto);

            if (!resultado.sucesso)
            {

                MessageBox.Show(resultado.mensagem);

                return;

            }

            Original.Add(new Carros
            {
                Placa = PlacaTexto,
                Entrada = dadosEntrada.EntradaData,
                Saida = "Aguardando...",
                Duracao = "Aguardando...",
                TempoEscolhido = dadosEntrada.TempoEntradaConvertido,
                PriceRow = dadosEntrada.ValorConvertidoBanco,
                TotalPrice = dadosEntrada.ValorAtual

            });

            repositorio.AtualizaEntrada(PlacaTexto,
                dadosEntrada.EntradaData,
                dadosEntrada.TempoEntradaConvertido,
                dadosEntrada.ValorConvertidoBanco,
                dadosEntrada.ValorAtual);


            PlacaTexto = string.Empty;
            TempoTexto = string.Empty;


            MostraRows = Original;

            OnPropertyChanged(nameof(MostraRows));
            OnPropertyChanged(nameof(PlacaTexto));
            OnPropertyChanged(nameof(TempoTexto));


        }

        public void Saida1(object sender)
        {

            if (SelecionaCarros is not null)
            {
                ProcessaSaida();
                OnPropertyChanged(nameof(MostraRows));

            }
            else
            {
                MessageBox.Show("Houve um erro ao processar a saida do veiculo");

            }


        }

        private void ProcessaSaida()
        {


            var dadosSaida = new EstacionamentoService();
            var repositorio = new EstacionamentoRepository();

            dadosSaida.CalculaSaida(SelecionaCarros);

            SelecionaCarros.TotalPrice = dadosSaida.ValorNovoFormatadoSaida;
            SelecionaCarros.Duracao = dadosSaida.DuracaoFinal;
            SelecionaCarros.Saida = dadosSaida.Saida;




            repositorio.AtualizaSaida(dadosSaida.Saida, SelecionaCarros.Placa, dadosSaida.ValorNovoFormatadoSaida, dadosSaida.DuracaoFinal);
            OnPropertyChanged(nameof(MostraRows));

        }


        public void AtualizarTextoVisivel()
        {
            PlacaVisivel = string.IsNullOrEmpty(PlacaTexto) ? Visibility.Visible : Visibility.Collapsed;
            TempoVisivel = string.IsNullOrEmpty(TempoTexto) ? Visibility.Visible : Visibility.Collapsed;
            ProcurarVisivel = string.IsNullOrEmpty(ProcurarTexto) ? Visibility.Visible : Visibility.Collapsed;
        }


        public void FiltroSearch()
        {

            if (string.IsNullOrEmpty(ProcurarTexto))
            {

                MostraRows = new ObservableCollection<Carros>(Original);
            }
            else
            {

                var filtrados = Original.Where(carro => carro.Placa.Contains(ProcurarTexto, StringComparison.OrdinalIgnoreCase)).ToList();

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










