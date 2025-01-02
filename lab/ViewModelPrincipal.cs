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
        private ObservableCollection<Carros>? Filtrado
        {
            get;
            set;
        }
        public ObservableCollection<Carros> Original
        {
            get;
            set;
        } = new ObservableCollection<Carros>();

        private ObservableCollection<Carros>? _mostraRows;

        private Carros? _seleciona;

        private Visibility _placavisivel = Visibility.Visible;

        private Visibility _tempovisivel = Visibility.Visible;

        private Visibility _procurarvisivel = Visibility.Visible;

        public ICommand EntradaCommand
        {
            get;
        }
        public ICommand SaidaCommand
        {
            get;
        }
        public ICommand AtualizaCommand
        {
            get;
        }

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
                    OnPropertyChanged(nameof(PlacaTexto));
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
                    OnPropertyChanged(nameof(TempoTexto));
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
            catch
            {

                MessageBox.Show("Houve um erro ao processar a entrada");
            }

        }

        private void ProcessaEntrada()
        {

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

            MessageBox.Show("Carro adicionado com sucesso");

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
            IEnumerable<Carros> carroSelecionado = Original.Where(SelecionaCarros => SelecionaCarros.IsSelected == true);

            var dadosSaida = new EstacionamentoService();
            var repositorio = new EstacionamentoRepository();

            foreach (var carro in carroSelecionado)
            {



                dadosSaida.CalculaSaida(carro);

                var resultado = dadosSaida.CalculaSaida(carro);

                if (!resultado.sucesso)
                {

                    MessageBox.Show(resultado.mensagem);

                    return;

                }

                if (carro.Saida == "Aguardando...")
                {
                    carro.TotalPrice = dadosSaida.ValorNovoFormatadoSaida;
                    carro.Duracao = dadosSaida.DuracaoFinal;
                    carro.Saida = dadosSaida.Saida;

                    repositorio.AtualizaSaida(
                      dadosSaida.Saida, carro.Placa,
                      dadosSaida.ValorNovoFormatadoSaida,
                      dadosSaida.DuracaoFinal
                    );

                    OnPropertyChanged(nameof(MostraRows));
                }

            }
        }

        public async Task AtualizaInativos()
        {

            var respositorio = new EstacionamentoRepository();

            await respositorio.TransferirInativo(SelecionaCarros.Placa);

            OnPropertyChanged(nameof(Original));

            var inativo = Original.FirstOrDefault(carro => carro.Placa == SelecionaCarros.Placa);

            if (inativo is not null)
            {
                Original.Remove(inativo);
            }

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