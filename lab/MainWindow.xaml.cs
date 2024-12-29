using System.Windows;
using System.Windows.Input;


namespace lab
{

    public partial class MainWindow : Window
    {


        public MainWindow()
        {

            InitializeComponent();

            this.DataContext = new ViewModelPrincipal();

        }


        private void PesquisaTexto(object sender, KeyEventArgs e)
        {

            var viewModel = (ViewModelPrincipal)DataContext;

            viewModel.FiltroSearch();

        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {

            var viewModel = (ViewModelPrincipal)DataContext;

            if (viewModel.IsSelectAll)
            {

                viewModel.IsSelectAll = false;


            }
            else
            {
                viewModel.IsSelectAll = true;

            }



        }




    }
}