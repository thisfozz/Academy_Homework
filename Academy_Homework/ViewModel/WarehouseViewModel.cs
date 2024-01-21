using Command_;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace Academy_Homework.ViewModel
{
    public class WarehouseViewModel : INotifyPropertyChanged
    {
        private readonly string connectionString;
        private NpgsqlConnection connection;

        public ICommand ConnectCommandButton { get; } // Подключение к базе данных
        public ICommand AddProductCommand { get; } // Добавление товара
        public ICommand AddProductTypesCommand { get; } // Добавление типа продукта
        public ICommand AddSuppliersCommand { get; } // Добавление типа продукта
        public ICommand UpdateProductInformationCommand { get; } // Обновление списка продуктов из базы данных
        public ICommand UpdateProductTypesInformationCommand { get; } // Обновление списка типов из базы данных
        public ICommand UpdateSuppliersInformationCommand { get; } // Обновление списка поставщиков из базы данных
        public ICommand DeleteProductCommand { get; } // Удаление продукта из базы данных
        public ICommand DeleteProductTypeCommand { get; } // Удаление продукта из базы данных
        public ICommand DeleteSupplierCommand { get; } // Удаление продукта из базы данных

        public WarehouseViewModel(MainViewModel mainViewModel)
        {
            connectionString = ConfigurationManager.ConnectionStrings["WarehouseConnectionString"].ConnectionString;
            connection = new NpgsqlConnection(connectionString);

            ConnectCommandButton = new DelegateCommand(ConnectToDatabase, (_) => true);
            AddProductCommand = new DelegateCommand(AddProduct, (_) => true);
            AddProductTypesCommand = new DelegateCommand(AddProductType, (_) => true);
            AddSuppliersCommand = new DelegateCommand(AddSuppliers, (_) => true);

            UpdateProductInformationCommand = new DelegateCommand(UpdateProductInformation, (_) => true);
            UpdateProductTypesInformationCommand = new DelegateCommand(UpdateProductTypesInformation, (_) => true);
            UpdateSuppliersInformationCommand = new DelegateCommand(UpdateSuppliersInformation, (_) => true);

            DeleteProductCommand = new DelegateCommand(DeleteProduct, (_) => true);
            DeleteProductTypeCommand = new DelegateCommand(DeleteProductType, (_) => true);
            DeleteSupplierCommand = new DelegateCommand(DeleteSupplier, (_) => true);
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

        private string _suppliersName;
        public string SuppliersName
        {
            get { return _suppliersName; }
            set
            {
                if (_suppliersName != value)
                {
                    _suppliersName = value;
                    OnPropertyChanged(nameof(SuppliersName));
                }
            }
        }

        private DataView _productsDataView;
        public DataView ProductsDataView
        {
            get { return _productsDataView; }
            set
            {
                _productsDataView = value;
                OnPropertyChanged(nameof(ProductsDataView));
            }
        }

        private DataView _productsTypeDataView;
        public DataView ProductsTypeDataView
        {
            get { return _productsTypeDataView; }
            set
            {
                _productsTypeDataView = value;
                OnPropertyChanged(nameof(ProductsTypeDataView));
            }
        }

        private DataView _suppliersDataView;
        public DataView SuppliersDataView
        {
            get { return _suppliersDataView; }
            set
            {
                _suppliersDataView = value;
                OnPropertyChanged(nameof(SuppliersDataView));
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

        // Вставка новых товаров/типов товаров/поставщиков
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
                        var commandText = "INSERT INTO Products((name, type_id, quantity, cost_price) VALUES(@id, @name, @type_id, @quantity, @cost_price)";

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


        private void AddSuppliers(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                NpgsqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    var commandText = "INSERT INTO Suppliers VALUES(@id, @supplier_name)";
                    int lastIndex = getLastIndex("SELECT id FROM Suppliers ORDER BY id DESC");

                    using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

                    command.Parameters.AddWithValue("@id", lastIndex);
                    command.Parameters.AddWithValue("@supplier_name", SuppliersName);

                    command.ExecuteNonQuery();
                    transaction.Commit();

                    MessageBox.Show("Данные успешно добавлены");
                }
                catch (Exception ex)
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

        // Обновление информации о товарах/типов товаров/поставщиках

        private void UpdateProductInformation(object obj)
        {
            string tableName = "Products";
            string query = "SELECT * FROM Products";
            var adapter = new NpgsqlDataAdapter(query, connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet, tableName);

            ProductsDataView = dataSet.Tables[tableName]?.DefaultView;
        }

        private void UpdateProductTypesInformation(object obj)
        {
            string tableName = "ProductTypes";
            string query = "SELECT * FROM ProductTypes";
            var adapter = new NpgsqlDataAdapter(query, connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet, tableName);

            ProductsTypeDataView = dataSet.Tables[tableName]?.DefaultView;
        }

        private void UpdateSuppliersInformation(object obj)
        {
            string tableName = "Suppliers";
            string query = "SELECT * FROM Suppliers";
            var adapter = new NpgsqlDataAdapter(query, connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet, tableName);

            SuppliersDataView = dataSet.Tables[tableName]?.DefaultView;
        }

        private void DeleteProduct(object obj)
        {
            //int id = obj.
        }


        private void DeleteProductType(object obj)
        {
            //
        }

        private void DeleteSupplier(object obj)
        {
            //
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
