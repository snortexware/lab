﻿using System.Globalization;

namespace lab.Interface
{


    //###################################
    //###      Logica de negocio      ###
    //###################################


    public class EstacionamentoService : MainWindow
    {
        public string? EntradaData { get; set; }
        public int TempoEntradaConvertido { get; set; }

        public string? ValorAtual { get; set; }

        public string? ValorConvertidoBanco { get; set; }


        public (bool sucesso, string mensagem) CalculaEntrada(string tempo)

        {
            DateTime entradaAtual = DateTime.Now;

            int valorDb = new EstacionamentoRepository().GetPriceAtivo();

            if (valorDb == 0)
            {

                return (false, "Houve um erro ao buscar valor do banco de dados. Talvez o valor de tabela não esteja atualizado?");

            }
            bool verifacaoPrincipal = int.TryParse(tempo, out int tempoEntradaConvertido) || tempoEntradaConvertido == 0;

            if (!verifacaoPrincipal)
            {
                return (false, "Digite um tempo válido. Exemplo: '1'");

            }
            else
            {

                TempoEntradaConvertido = tempoEntradaConvertido;

                int valorAtual = TempoEntradaConvertido * valorDb;

                ValorAtual = valorAtual.ToString("C", new CultureInfo("pt-BR"));

                ValorConvertidoBanco = valorDb.ToString("C", new CultureInfo("pt-BR"));

                EntradaData = entradaAtual.ToString("dd/MM/yyyy HH:mm:ss");

                return (true, "Todos os calculos foram um sucesso");
            }
        }


        public string? Entrada { get; set; }
        public string? Saida { get; set; }

        public string? ValorNovoFormatadoSaida { get; set; }

        public string? DuracaoFinal { get; set; }



        public (bool sucesso, string mensagem) CalculaSaida(Carros selecionado)
        {

            if (selecionado is null && selecionado.Saida != "Aguardando...")
            {
                return (false, "Selecione um carro ativo ou valido antes de continuar");
            }


            DateTime dataAtual = DateTime.Now;

            Saida = dataAtual.ToString("dd/MM/yyyy HH:mm:ss");

            var dataEntradaStr = selecionado.Entrada;

            if (!DateTime.TryParseExact(dataEntradaStr, "dd/MM/yyyy HH:mm:ss",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataEntrada))
            {
                return (false, "Erro ao converter data ao processar saida");

            };

            TimeSpan DataSubtraida = dataAtual - dataEntrada;

            int totalHoras = (int)DataSubtraida.TotalHours;

            int totalMinutos = (int)DataSubtraida.TotalMinutes;

            int minutosFormatados = totalMinutos % 60;

            DuracaoFinal = $"{totalHoras:D2}:{minutosFormatados:D2}";

            string valorNaoEspecial = selecionado.PriceRow.Replace("0", "").Replace(",", "");

            int valorNovoSaida = int.Parse(valorNaoEspecial.Substring(3));

            int valorFinal = valorNovoSaida * totalHoras;

            int minutosTolerancia = 10;

            if (minutosFormatados > minutosTolerancia)
            {

                valorFinal++;

            }

            if (totalMinutos < 30)
            {
                valorNovoSaida /= 2;

            }

            ValorNovoFormatadoSaida = totalMinutos < 30
            ? valorNovoSaida.ToString("C", new CultureInfo("pt-BR"))
            : valorFinal.ToString("C", new CultureInfo("pt-BR"));

            return (true, "Todos os calculos finalizados com sucesso, continuando...");

        }


    }





}
