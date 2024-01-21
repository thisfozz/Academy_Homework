using Command_;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel
{
    public class WarehouseViewModel : INotifyPropertyChanged
    {
        private readonly string connectionString;
        private NpgsqlConnection connection;

        private DataSet dataSet;
        private SqlCommandBuilder command;
        private DataTable dataTable;

        public ICommand ConnectCommandButton { get; } // Подключение к базе данных
        public ICommand AddProductCommand { get; } // Добавление товара
        public ICommand AddProductTypes { get; } // Добавление типа продукта

        public WarehouseViewModel()
        {
            connectionString = ConfigurationManager.ConnectionStrings["WarehouseConnectionString"].ConnectionString;
            connection = new NpgsqlConnection(connectionString);

            ConnectCommandButton = new DelegateCommand(ConnectToDatabase, (_) => true);
            AddProductCommand = new DelegateCommand(AddProduct, (_) => true);
            AddProductTypes = new DelegateCommand(AddProductType, (_) => true);
        }

        // Временное решение с передачей ID. (может получать через SELECT id FROM Product ORDER BY id DESC LIMIT 1; +1 сделав поле readonly)
        private int _productID;
        public int ProductID
        {
            get { return _productID; }
            set
            {
                if (_productID != value)
                {
                    _productID = value;
                    OnPropertyChanged(nameof(ProductID));
                }
            }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                if (_productName != value)
                {
                    _productName = value;
                    OnPropertyChanged(nameof(ProductName));
                }
            }
        }

        private int _productTypeID;
        public int ProductTypeID
        {
            get { return _productTypeID; }
            set
            {
                if (_productTypeID != value)
                {
                    _productTypeID = value;
                    OnPropertyChanged(nameof(ProductTypeID));
                }
            }
        }

        private int _productQuantity;
        public int ProductQuantity
        {
            get { return _productQuantity; }
            set
            {
                if (_productQuantity != value)
                {
                    _productQuantity = value;
                    OnPropertyChanged(nameof(ProductQuantity));
                }
            }
        }

        private decimal _productCostPrice;
        public decimal ProductCostPrice
        {
            get { return _productCostPrice; }
            set
            {
                if (_productCostPrice != value)
                {
                    _productCostPrice = value;
                    OnPropertyChanged(nameof(ProductCostPrice));
                }
            }
        }

        private string _productType;
        public string ProductType
        {
            get { return _productType; }
            set
            {
                if (_productType != value)
                {
                    _productType = value;
                    OnPropertyChanged(nameof(ProductType));
                }
            }
        }

        private void ConnectToDatabase(object obj)
        {

            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    connection = new NpgsqlConnection(connectionString);
                    connection.Open();

                    MessageBox.Show("Успешное подключение к базе Warehouse");
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Уже существует активное подключение.");
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

        int getLastIndex(string sqlQuery)
        {
            using var command = new NpgsqlCommand(sqlQuery, connection);
            using var reader = command.ExecuteReader();

            int lastIndex = -1;

            if (reader.Read())
            {
                lastIndex = Convert.ToInt32(reader["id"]);
            }
            lastIndex += 1;

            return lastIndex;
        }

        private bool IsProductTypeValid(int productTypeId)
        {
            int maxProductTypeId = getLastIndex("SELECT id FROM ProductTypes ORDER BY id DESC");
            return productTypeId >= 1 && productTypeId <= maxProductTypeId;
        }

        private void AddProduct(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                NpgsqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    int lastIndex = getLastIndex("SELECT id FROM Products ORDER BY id DESC");

                    if (IsProductTypeValid(ProductTypeID))
                    {
                        var commandText = "INSERT INTO Products VALUES(@id, @name, @type_id, @quantity, @cost_price)";

                        using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

                        command.Parameters.AddWithValue("@id", lastIndex);
                        command.Parameters.AddWithValue("@name", ProductName);
                        command.Parameters.AddWithValue("@type_id", ProductTypeID);
                        command.Parameters.AddWithValue("@quantity", ProductQuantity);
                        command.Parameters.AddWithValue("@cost_price", ProductCostPrice);

                        command.ExecuteNonQuery();
                        transaction.Commit();

                        MessageBox.Show("Данные успешно добавлены");
                    }
                    else
                    {
                        MessageBox.Show("Веденный номер товара не существует");
                    }
                } 
                catch(Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"При добавлении данных возникла ошибка - {ex.Message}");
                }
                finally
                {
                    transaction.Dispose();
                }
            });
        }

        private void AddProductType(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                NpgsqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    var commandText = "INSERT INTO ProductTypes VALUES(@id, @type_name)";
                    int lastIndex = getLastIndex("SELECT id FROM ProductTypes ORDER BY id DESC");

                    using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

                    command.Parameters.AddWithValue("@id", lastIndex);
                    command.Parameters.AddWithValue("@type_name", ProductType);

                    command.ExecuteNonQuery();
                    transaction.Commit();

                    MessageBox.Show("Данные успешно добавлены");
                } 
                catch(Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"При добавлении данных возникла ошибка - {ex.Message}");
                }
                finally
                {
                    transaction.Dispose();
                }
            });
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
