using Academy_Homework.ViewModel;
using System.Windows;

namespace Academy_Homework;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}