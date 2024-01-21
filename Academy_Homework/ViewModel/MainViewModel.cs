using Academy_Homework.View;
using Command_;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel
{
    public class MainViewModel
    {
        public ICommand WarehouseHmwOpenButton { get; }

        public MainViewModel()
        {
            WarehouseHmwOpenButton = new DelegateCommand(OpenWarehousWindow, (_) => true);
        }

        private void OpenWarehousWindow(object parameter)
        {
            var warehouseViewModel = new WarehouseViewModel();
            WarehouseWindow warehouseWindow = new WarehouseWindow();
            warehouseWindow.DataContext = warehouseViewModel;

            Application.Current.MainWindow.Close();

            Application.Current.MainWindow = warehouseWindow;
            Application.Current.MainWindow.Show();
        }
    }
}
