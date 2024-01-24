using Academy_Homework.View;
using Command_;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel
{
    public class MainViewModel
    {
        public ICommand WarehouseHmwOpenCommand { get; }
        public ICommand VegetablesAndFruitsCommand { get; }

        public MainViewModel()
        {
            WarehouseHmwOpenCommand = new DelegateCommand(OpenWarehousWindow, (_) => true);
            VegetablesAndFruitsCommand = new DelegateCommand(OpenVegetablesAndFruitsWindow, (_) => true);
        }

        private void OpenWarehousWindow(object parameter)
        {
            var warehouseViewModel = new WarehouseViewModel(this);

            var warehouseWindow = new WarehouseWindow(warehouseViewModel);

            Application.Current.MainWindow.Close();

            Application.Current.MainWindow = warehouseWindow;
            Application.Current.MainWindow.Show();
        }

        private void OpenVegetablesAndFruitsWindow(object obj)
        {
            var vegetablesAndFruitsViewModel = new VegetablesAndFruitsViewModel(this);

            var vegetablesAndFruitsWindow = new VegetablesAndFruitsWindow();

            Application.Current.MainWindow.Close();

            Application.Current.MainWindow = vegetablesAndFruitsWindow;
            Application.Current.MainWindow.Show();
        }
    }
}