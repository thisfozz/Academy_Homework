using Command_;
using Npgsql;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel;

public class VegetablesAndFruitsViewModel : INotifyPropertyChanged
{
    public ICommand ConnectCommand { get; } // Подключение к базе данных
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
    public ICommand DeleteProductCommand { get; } // Удаление продукта

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

    public VegetablesAndFruitsViewModel()
    {
        DatabaseList = GetDatabaseNamesFromConfig();

        DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);

        ConnectCommand = new DelegateCommand(ConnectToDatabase, (_) => true);
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

        DeleteProductCommand = new DelegateCommand(DeleteProduct, (_) => true);
    }

    private ObservableCollection<string> GetDatabaseNamesFromConfig()
    {
        ObservableCollection<string> databaseNames = new ObservableCollection<string>();

        ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;

        foreach (ConnectionStringSettings connectionString in connectionStrings)
        {
            if (!String.IsNullOrEmpty(connectionString.Name) && connectionString.Name != "WarehouseConnectionString")
            {
                databaseNames.Add(connectionString.Name);
            }
        }

        return databaseNames;
    }

    private async void ConnectToDatabase(object obj)
    {
        await Task.Run(() =>
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

            try
            {
                factory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
                connection = factory.CreateConnection();
                connection.ConnectionString = connectionStringSettings.ConnectionString;

                connection.Open();
                MessageBox.Show($"Успешное подключение к базе Vegetable And Fruits {connection.ConnectionString}");
            }
            catch (DbException ex)
            {
                MessageBox.Show($"Не удалось создать подключение {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла ошибка при подключении {ex.Message}");
            }
        });
    }

    private static DbParameter CreateDbParameter<T>(DbCommand command, string parameterName, T value)
    {
        var dbParameter = command.CreateParameter();
        dbParameter.ParameterName = parameterName;
        dbParameter.Value = value;
        return dbParameter;
    }

    private async Task<string> GetUserInputAsync(string prompt, string title)
    {
        string userInput = await Task.Run(() =>
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, title, "");
        });

        return userInput;
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

    private async void ShowAllData(object obj)
    {
        await Task.Run(() =>
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

                Application.Current.Dispatcher.Invoke(() => ProduceDataTable = dataTable);
            });
        });
    }

    private async void ShowAllNames(object obj)
    {
        await Task.Run(() =>
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
        });
    }

    private async void ShowAllColors(object obj)
    {
        await Task.Run(() =>
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
        });
    }

    private async void ShowMaxCalories(object obj)
    {
        await Task.Run(() =>
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
        });
    }

    private async void ShowMinCalories(object obj)
    {
        await Task.Run(() => 
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
        });
    }

    private async void ShowAvgCalories(object obj)
    {

        await Task.Run(() =>
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
        });
    }

    private async void ShowAllVegetablesCount(object obj)
    {
        await Task.Run(() => 
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
        });
    }

    private async void ShowAllFruitsCount(object obj)
    {
        await Task.Run(() => 
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
        });
    }

    private async void ShowItemsByColor(object obj)
    {
        await Task.Run(() =>
        {
            ExecuteIfConnectionOpen(() =>
            {
                string color = GetUserInputAsync("Введите цвет:", "Цвет").Result;

                if (!string.IsNullOrEmpty(color))
                {
                    string query = "SELECT COUNT(*) AS ProduceCount FROM Produce WHERE color = @color";

                    using var command = factory.CreateCommand();
                    command.CommandText = query;
                    command.Connection = connection;

                    var parameter = CreateDbParameter(command, "@color", color);
                    command.Parameters.Add(parameter);

                    using var adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ProduceDataTable = dataTable;
                }
            });
        });
    }

    private async void ShowItemCountByColor(object obj)
    {
        await Task.Run(() => 
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT color, type, COUNT(*) AS ProduceCount FROM Produce GROUP BY color, type";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        });
    }

    private async void ShowItemsBelowCalories(object obj)
    {
        await Task.Run(() =>
        {
            ExecuteIfConnectionOpen(() =>
            {
                string caloriesInput = GetUserInputAsync("Введите максимум калорий: ", "Калории").Result;

                if (int.TryParse(caloriesInput, out int calories) && calories >= 0)
                {
                    string query = "SELECT name FROM Produce WHERE calories < @calories";

                    using var command = factory.CreateCommand();
                    command.CommandText = query;
                    command.Connection = connection;

                    var parameter = CreateDbParameter(command, "@calories", calories);
                    command.Parameters.Add(parameter);

                    using var adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ProduceDataTable = dataTable;
                }
                else
                {
                    MessageBox.Show("Введено неверное значение");
                }
            });
        });
    }

    private async void ShowItemsAboveCalories(object obj)
    {
        await Task.Run(() =>
        {
            ExecuteIfConnectionOpen(() =>
            {
                string caloriesInput = GetUserInputAsync("Введите минимум калорий: ", "Калории").Result;
                if (int.TryParse(caloriesInput, out int calories) && calories >= 0)
                {
                    string query = "SELECT name FROM Produce WHERE calories > @calories";

                    using var command = factory.CreateCommand();
                    command.CommandText = query;
                    command.Connection = connection;

                    var parameter = CreateDbParameter(command, "@calories", calories);
                    command.Parameters.Add(parameter);

                    using var adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ProduceDataTable = dataTable;
                }
                else
                {
                    MessageBox.Show("Введено неверное значение");
                }
            });
        });
    }

    private async void ShowItemsInCaloriesRange(object obj)
    {
        await Task.Run(() =>
        {
            ExecuteIfConnectionOpen(() =>
            {
                string caloriesBeginInput = GetUserInputAsync("От:", "Диапазон калорий").Result;
                string caloriesEndInput = GetUserInputAsync("До:", "Диапазон калорий").Result;

                if (int.TryParse(caloriesBeginInput, out int caloriesBegin) && int.TryParse(caloriesEndInput, out int caloriesEnd) && caloriesBegin >= 0 && caloriesEnd >= 0)
                {
                    string query = "SELECT * FROM Produce WHERE calories BETWEEN @minCalories AND @maxCalories";

                    using var command = factory.CreateCommand();
                    command.CommandText = query;
                    command.Connection = connection;

                    var parameterBegin = CreateDbParameter(command, "@minCalories", caloriesBegin);
                    var parameterEnd = CreateDbParameter(command, "@maxCalories", caloriesEnd);
                    command.Parameters.Add(parameterBegin);
                    command.Parameters.Add(parameterEnd);

                    using var adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ProduceDataTable = dataTable;
                }
                else
                {
                    MessageBox.Show("Введено неверное значение");
                }
            });
        });
    }

    private async void ShowItemsByColorYellowRed(object obj)
    {
        await Task.Run(() => 
        {
            ExecuteIfConnectionOpen(() =>
            {
                string query = "SELECT name FROM Produce WHERE color = 'Желтый' OR color = 'Красный'";
                using var command = factory.CreateCommand();

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = factory.CreateDataAdapter();

                adapter.SelectCommand = command;

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                ProduceDataTable = dataTable;
            });
        });
    }

    private async void DeleteProduct(object obj)
    {
        await Task.Run(() =>
        {
            ExecuteIfConnectionOpen(() =>
            {
                var dataRow = (DataRowView)obj;
                var rowID = dataRow.Row[0];

                string query = $"DELETE FROM Produce WHERE id = @id";

                using var command = factory.CreateCommand();
                command.Connection = connection;

                var parametr = CreateDbParameter(command, "@id", rowID);
                command.Parameters.Add(parametr);

                using var adapter = factory.CreateDataAdapter();
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.CommandText = query;
                adapter.DeleteCommand.ExecuteNonQuery();
            });
        });

        OnPropertyChanged(nameof(ProduceDataTable));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}