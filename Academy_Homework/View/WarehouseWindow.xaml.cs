using Academy_Homework.ViewModel;
using System.Windows;

namespace Academy_Homework.View;

public partial class WarehouseWindow : Window
{
    public WarehouseWindow(WarehouseViewModel warehouseViewModel)
    {
        InitializeComponent();
        DataContext = warehouseViewModel;

        Height = 900;
        Width = 900;
    }
}
