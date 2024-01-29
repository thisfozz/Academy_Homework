using Command_;
using Npgsql;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel
{
    public class VegetablesAndFruitsViewModel : INotifyPropertyChanged
    {
        public ICommand ConnectCommandButton { get; } // Подключение к базе данных

        private DbConnection connection;
        private DbProviderFactory factory;

        private string selectedDatabase;
        public string SelectedDatabase
        {
            get => selectedDatabase;
            set
            {
                if (selectedDatabase != value)
                {
                    selectedDatabase = value;
                    OnPropertyChanged(nameof(SelectedDatabase));
                }
            }
        }

        private ObservableCollection<string> databaseList;
        public ObservableCollection<string> DatabaseList
        {
            get => databaseList;
            set
            {
                if (databaseList != value)
                {
                    databaseList = value;
                    OnPropertyChanged(nameof(DatabaseList));
                }
            }
        }

        public VegetablesAndFruitsViewModel(MainViewModel mainViewModel)
        {
            DatabaseList = new ObservableCollection<string>
            {
                "VegetablesFruit",
                "LocalDbVegetablesFruit"
            };

            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);

            ConnectCommandButton = new DelegateCommand(ConnectToDatabase, (_) => true);
        }


        private void ConnectToDatabase(object obj)
        {
            if (string.IsNullOrEmpty(selectedDatabase))
            {
                MessageBox.Show("Выберите провайдера и укажите строку подключения.");
                return;
            }

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[selectedDatabase];

            if (connectionStringSettings == null)
            {
                MessageBox.Show($"Не удалось найти строку подключения для провайдера {selectedDatabase}.");
                return;
            }

            factory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            connection = factory.CreateConnection();
            connection.ConnectionString = connectionStringSettings.ConnectionString;

            try
            {
                connection.Open();
                MessageBox.Show($"Успешное подключение к базе Vegetable And Fruits {connection.ConnectionString}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
