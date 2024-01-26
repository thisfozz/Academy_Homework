using Academy_Homework.ViewModel;
using System.Windows;

namespace Academy_Homework.View;
public partial class VegetablesAndFruitsWindow : Window
{
    public VegetablesAndFruitsWindow(VegetablesAndFruitsViewModel vegetablesAndFruitsViewModel)
    {
        InitializeComponent();
        DataContext = vegetablesAndFruitsViewModel;
    }
}
