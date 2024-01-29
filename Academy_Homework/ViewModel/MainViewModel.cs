using Academy_Homework.View;
using Command_;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel;

public class MainViewModel
{
    public ICommand WarehouseHmwOpenCommand { get; }
    public ICommand VegetablesAndFruitsCommand { get; }
    public ICommand OpenCountriesCommand { get; }

    public MainViewModel()
    {
        WarehouseHmwOpenCommand = new DelegateCommand(OpenWarehousWindow, (_) => true);
        VegetablesAndFruitsCommand = new DelegateCommand(OpenVegetablesAndFruitsWindow, (_) => true);
        OpenCountriesCommand = new DelegateCommand(OpenOpenCountrieWindow, (_) => true);
    }

    private void OpenWarehousWindow(object parameter)
    {
        var warehouseViewModel = new WarehouseViewModel();

        var warehouseWindow = new WarehouseWindow(warehouseViewModel);

        Application.Current.MainWindow.Close();

        Application.Current.MainWindow = warehouseWindow;
        Application.Current.MainWindow.Show();
    }

    private void OpenVegetablesAndFruitsWindow(object obj)
    {
        var vegetablesAndFruitsViewModel = new VegetablesAndFruitsViewModel();

        var vegetablesAndFruitsWindow = new VegetablesAndFruitsWindow(vegetablesAndFruitsViewModel);

        Application.Current.MainWindow.Close();

        Application.Current.MainWindow = vegetablesAndFruitsWindow;
        Application.Current.MainWindow.Show();
    }

    private void OpenOpenCountrieWindow(object obj)
    {
        var countriesViewModel = new CountriesViewModel();

        var countriesWindow = new CountriesWindow(countriesViewModel);

        Application.Current.MainWindow.Close();
        Application.Current.MainWindow = countriesWindow;
        Application.Current.MainWindow.Show();
    }
}