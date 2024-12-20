using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace lab
{
    internal class EstacionamentoRepository : Carros
    {



        public ObservableCollection<Carros> GetAtivos()
        {

            var observableList = new ObservableCollection<Carros>();


            var dbCnn = new EstacionamentoInterface().SimplesDbConnection();

            dbCnn.Open();

            var values = dbCnn.Query<Carros>(@"SELECT * FROM Estacionamento");



            foreach (var item in values)
            {

                observableList.Add(item);

            }

            return observableList;


        }



    }
}
