using Academy_Homework.ViewModel;
using System.Windows;

namespace Academy_Homework.View;

public partial class CountriesWindow : Window
{
    public CountriesWindow(CountriesViewModel countriesViewModel)
    {
        InitializeComponent();
        DataContext = countriesViewModel;
    }
}
