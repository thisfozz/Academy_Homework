using Academy_Homework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Academy_Homework.View
{
    public partial class VegetablesAndFruitsWindow : Window
    {
        public VegetablesAndFruitsWindow(VegetablesAndFruitsViewModel vegetablesAndFruitsViewModel)
        {
            InitializeComponent();
            DataContext = vegetablesAndFruitsViewModel;


            Height = 900;
            Width = 900;
        }
    }
}
