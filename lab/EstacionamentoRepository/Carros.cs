using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace lab
{
    public class Carros : INotifyPropertyChanged
    {

        private bool _isselect;
        private string _placa;
        private string _entrada;
        private string _saida;
        private string _duracao;
        private int _tempoEscolhido;
        private string _priceRow;
        private string _totalPrice;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSelected
        {
            get => _isselect;
            set
            {
                if (_isselect != value)
                {
                    _isselect = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Placa
        {
            get => _placa;
            set
            {
                if (_placa != value)
                {
                    _placa = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Entrada
        {
            get => _entrada;
            set
            {
                if (_entrada != value)
                {
                    _entrada = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Saida
        {
            get => _saida;
            set
            {
                if (_saida != value)
                {
                    _saida = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Duracao
        {
            get => _duracao;
            set
            {
                if (_duracao != value)
                {
                    _duracao = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TempoEscolhido
        {
            get => _tempoEscolhido;
            set
            {
                if (_tempoEscolhido != value)
                {
                    _tempoEscolhido = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PriceRow
        {
            get => _priceRow;
            set
            {
                if (_priceRow != value)
                {
                    _priceRow = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TotalPrice
        {
            get => _totalPrice;
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged();
                }
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string Propriedade = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Propriedade));
        }
    }
}
