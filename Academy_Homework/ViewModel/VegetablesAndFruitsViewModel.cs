using Command_;
using Npgsql;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel
{
    public class VegetablesAndFruitsViewModel : INotifyPropertyChanged
    {
        public ICommand ConnectCommandButton { get; } // Подключение к базе данных
        public ICommand ShowAllDataCommand { get; } // Отображение всей информации из таблицы с овощами и фруктами
        public ICommand ShowAllNamesCommand { get; } // Отображение всех названий овощей и фруктов
        public ICommand ShowAllColorsCommand { get; } // Отображение всех цветов
        public ICommand ShowMaxCaloriesCommand { get; } // ■ Показать максимальную калорийность
        public ICommand ShowMinCaloriesCommand { get; } // Показать минимальную калорийность
        public ICommand ShowAvgCaloriesCommand { get; } // Показать среднюю калорийность
        public ICommand ShowAllVegetablesCountCommand { get; } // Показать количество овощей
        public ICommand ShowAllFruitsCountCommand { get; } // Показать количество фруктов
        public ICommand ShowItemsByColorCommand { get; } //  Показать количество овощей и фруктов заданного цвета
        public ICommand ShowItemCountByColorCommand { get; } // Показать количество овощей фруктов каждого цвета
        public ICommand ShowItemsBelowCaloriesCommand { get; } // Показать овощи и фрукты с калорийностью ниже указанной
        public ICommand ShowItemsAboveCaloriesCommand { get; } //  Показать овощи и фрукты с калорийностью выше указанной
        public ICommand ShowItemsInCaloriesRangeCommand { get; } // Показать овощи и фрукты с калорийностью в указанном диапазоне
        public ICommand ShowItemsByColorYellowRedCommand { get; } // Показать все овощи и фрукты, у которых цвет желтый или красный

        private DbConnection connection;
        private DbProviderFactory factory;
        private string connectionString;

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

        private DataTable produceDataTable;
        public DataTable ProduceDataTable
        {
            get => produceDataTable;
            set
            {
                produceDataTable = value;
                OnPropertyChanged(nameof(ProduceDataTable));
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
            ShowAllDataCommand = new DelegateCommand(ShowAllData, (_) => true);
            ShowAllNamesCommand = new DelegateCommand(ShowAllNames, (_) => true);
            ShowAllColorsCommand = new DelegateCommand(ShowAllColors, (_) => true);
            ShowMaxCaloriesCommand = new DelegateCommand(ShowMaxCalories, (_) => true);
            ShowMinCaloriesCommand = new DelegateCommand(ShowMinCalories, (_) => true);
            ShowAvgCaloriesCommand = new DelegateCommand(ShowAvgCalories, (_) => true);

            ShowAllVegetablesCountCommand = new DelegateCommand(ShowAllVegetablesCount, (_) => true);
            ShowAllFruitsCountCommand = new DelegateCommand(ShowAllFruitsCount, (_) => true);
            ShowItemsByColorCommand = new DelegateCommand(ShowItemsByColor, (_) => true);
            ShowItemCountByColorCommand = new DelegateCommand(ShowItemCountByColor, (_) => true);
            ShowItemsBelowCaloriesCommand = new DelegateCommand(ShowItemsBelowCalories, (_) => true);
            ShowItemsAboveCaloriesCommand = new DelegateCommand(ShowItemsAboveCalories, (_) => true);
            ShowItemsInCaloriesRangeCommand = new DelegateCommand(ShowItemsInCaloriesRange, (_) => true);
            ShowItemsByColorYellowRedCommand = new DelegateCommand(ShowItemsByColorYellowRed, (_) => true);
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
            connectionString = connectionStringSettings.ConnectionString;

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

        private void ExecuteIfConnectionOpen(Action action)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                action.Invoke();
            }
            else
            {
                MessageBox.Show("Нет подключения к базе данных");
            }
        }

        private void ShowAllData(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT * FROM Produce";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowAllNames(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT name FROM Produce";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowAllColors(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT color FROM Produce";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowMaxCalories(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT MAX(calories) AS MaxCalories FROM Produce";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowMinCalories(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT MIN(calories) AS MinCalories FROM Produce";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowAvgCalories(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT AVG(calories) AS AvgCalories FROM Produce";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowAllVegetablesCount(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT COUNT(*) AS VegetablesCount FROM Produce WHERE type = 'Овощ'";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowAllFruitsCount(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT COUNT(*) AS VegetablesCount FROM Produce WHERE type = 'Фрукт'";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        }

        private void ShowItemsByColor(object obj)
        {
            throw new NotImplementedException();
        }

        private void ShowItemCountByColor(object obj)
        {
            throw new NotImplementedException();
        }

        private void ShowItemsBelowCalories(object obj)
        {
            throw new NotImplementedException();
        }

        private void ShowItemsAboveCalories(object obj)
        {
            throw new NotImplementedException();
        }

        private void ShowItemsInCaloriesRange(object obj)
        {
            throw new NotImplementedException();
        }

        private void ShowItemsByColorYellowRed(object obj)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
