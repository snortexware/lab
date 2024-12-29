using System;
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
                    OnPropertyChanged(nameof(IsSelected));
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
                    OnPropertyChanged(nameof(Placa));
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
                    OnPropertyChanged(nameof(Entrada));
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
                    OnPropertyChanged(nameof(Saida));
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
                    OnPropertyChanged(nameof(Duracao));
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
                    OnPropertyChanged(nameof(TempoEscolhido));
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
                    OnPropertyChanged(nameof(PriceRow));
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
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
