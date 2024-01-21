using Command_;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
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
        public ICommand DeleteProductTypeCommand { get; } // Удаление типа продукта из базы данных
        public ICommand DeleteSupplierCommand { get; } // Удаление поставщика из базы данных

        public ICommand ShowSupplierWithMaxItemsCommand { get; } // Поставщик, макс. товары
        public ICommand ShowSupplierWithMinItemsCommand { get; } // Поставщик, мин. товары"
        public ICommand ShowTypeWithMaxItemsCommand { get; } // Тип, макс. товары
        public ICommand ShowTypeWithMinItemsCommand { get; } // Тип, мин. товары
        public ICommand ShowOlderThanNDaysCommand { get; } // Поставка, старше N дней

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

            ShowSupplierWithMaxItemsCommand = new DelegateCommand(ShowSupplierWithMaxItems, (_) => true);
            ShowSupplierWithMinItemsCommand = new DelegateCommand(ShowSupplierWithMinItems, (_) => true);
            ShowTypeWithMaxItemsCommand = new DelegateCommand(ShowTypeWithMaxItems, (_) => true);
            ShowTypeWithMinItemsCommand = new DelegateCommand(ShowTypeWithMinItems, (_) => true);
            ShowOlderThanNDaysCommand = new DelegateCommand(ShowOlderThanNDays, (_) => true);
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

        private bool IsProductTypeValid(int productTypeId, string sqlQuery)
        {
            int maxProductTypeId = getLastIndex(sqlQuery);
            if (productTypeId <= maxProductTypeId || productTypeId > maxProductTypeId) return true;

            return false;
        }


        // Вставка новых товаров/типов товаров/поставщиков
        private void AddProduct(object obj)
        {
            ExecuteIfConnectionOpen(() =>
            {
                NpgsqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    if (IsProductTypeValid(ProductTypeID, "SELECT id FROM ProductTypes ORDER BY id DESC"))
                    {
                        var commandText = "INSERT INTO Products(name, type_id, quantity, cost_price) VALUES(@name, @type_id, @quantity, @cost_price)";

                        using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

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
                    var commandText = "INSERT INTO ProductTypes(type_name) VALUES(@type_name)";

                    using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

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
                    var commandText = "INSERT INTO Suppliers(supplier_name) VALUES(@supplier_name)";

                    using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

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
            string query = $"SELECT * FROM {tableName}";
            var adapter = new NpgsqlDataAdapter(query, connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet, tableName);

            ProductsTypeDataView = dataSet.Tables[tableName]?.DefaultView;
        }
        private void UpdateSuppliersInformation(object obj)
        {
            string tableName = "Suppliers";
            string query = $"SELECT * FROM {tableName}";
            var adapter = new NpgsqlDataAdapter(query, connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet, tableName);

            SuppliersDataView = dataSet.Tables[tableName]?.DefaultView;
        }

        private void DeleteProduct(object obj)
        {
            var dataRow = (DataRowView)obj;
            var rowID = dataRow.Row[0];

            string tableName = "Products";
            string query = $"DELETE FROM {tableName} WHERE id = {rowID}";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            ExecuteIfConnectionOpen(() =>
            {
                command.ExecuteNonQuery();
            });

            ProductsDataView?.Table.Rows.Remove(dataRow.Row);

            OnPropertyChanged(nameof(ProductsDataView));
        }
        private void DeleteProductType(object obj)
        {
            var dataRow = (DataRowView)obj;
            var rowID = dataRow.Row[0];

            string tableName = "ProductTypes";
            string query = $"DELETE CASCADE FROM {tableName} WHERE id = {rowID}";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            ExecuteIfConnectionOpen(() =>
            {
                command.ExecuteNonQuery();
            });

            ProductsTypeDataView?.Table.Rows.Remove(dataRow.Row);

            OnPropertyChanged(nameof(ProductsTypeDataView));
        }
        private void DeleteSupplier(object obj)
        {
            var dataRow = (DataRowView)obj;
            var rowID = dataRow.Row[0];

            string tableName = "Suppliers";
            string query = $"DELETE FROM {tableName} WHERE id = {rowID}";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            ExecuteIfConnectionOpen(() =>
            {
                command.ExecuteNonQuery();
            });

            SuppliersDataView?.Table.Rows.Remove(dataRow.Row);

            OnPropertyChanged(nameof(SuppliersDataView));
        }


        // Выборка данных
        private void ShowSupplierWithMaxItems(object obj)
        {
            string query = "SELECT s.supplier_name AS supplier_name, SUM(p.quantity) AS total_quantity " +
                           "FROM Suppliers s " +
                           "JOIN Products_Suppliers ps ON s.id = ps.supplier_id " +
                           "JOIN Products p ON ps.product_id = p.id " +
                           "GROUP BY s.supplier_name " +
                           "ORDER BY total_quantity DESC " +
                           "LIMIT 1;";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);

            ExecuteIfConnectionOpen(() =>
            {
                using NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string supplierName = reader["supplier_name"].ToString();
                    int totalQuantity = Convert.ToInt32(reader["total_quantity"]);
                    MessageBox.Show($"Поставщик с наибольшим количеством товаров: {supplierName}\nОбщее количество товаров: {totalQuantity}");
                }
                else
                {
                    MessageBox.Show("Нет данных о поставщиках.");
                }
            });
        }

        private void ShowSupplierWithMinItems(object obj)
        {
            string query = "SELECT s.supplier_name AS supplier_name, SUM(p.quantity) AS total_quantity " +
                           "FROM Suppliers s " +
                           "JOIN Products_Suppliers ps ON s.id = ps.supplier_id " +
                           "JOIN Products p ON ps.product_id = p.id " +
                           "GROUP BY s.supplier_name " +
                           "ORDER BY total_quantity ASC " +
                           "LIMIT 1;";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);

            ExecuteIfConnectionOpen(() =>
            {
                using NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string supplierName = reader["supplier_name"].ToString();
                    int totalQuantity = Convert.ToInt32(reader["total_quantity"]);
                    MessageBox.Show($"Поставщик с минимальным количеством товаров: {supplierName}\nОбщее количество товаров: {totalQuantity}");
                }
                else
                {
                    MessageBox.Show("Нет данных о поставщиках.");
                }
            });
        }

        private void ShowTypeWithMaxItems(object obj)
        {
            string query = "SELECT pt.type_name AS type_name, SUM(p.quantity) AS total_quantity " +
                            "FROM ProductTypes pt " +
                            "JOIN Products p ON pt.id = p.type_id " +
                            "GROUP BY pt.type_name " +
                            "ORDER BY total_quantity DESC " +
                            "LIMIT 1;";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);

            ExecuteIfConnectionOpen(() =>
            {
                using NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string typeName = reader["type_name"].ToString();
                    int totalQuantity = Convert.ToInt32(reader["total_quantity"]);
                    MessageBox.Show($"Тип товаров с наибольшим количеством: {typeName}\nОбщее количество товаров: {totalQuantity}");
                }
                else
                {
                    MessageBox.Show("Нет данных о типах товаров.");
                }
            });
        }

        private void ShowTypeWithMinItems(object obj)
        {
            string query = "SELECT pt.type_name AS type_name, SUM(p.quantity) AS total_quantity " +
                           "FROM ProductTypes pt " +
                           "JOIN Products p ON pt.id = p.type_id " +
                           "GROUP BY pt.type_name " +
                           "ORDER BY total_quantity ASC " +
                           "LIMIT 1;";

            using NpgsqlCommand command = new NpgsqlCommand(query, connection);

            ExecuteIfConnectionOpen(() =>
            {
                using NpgsqlDataReader reader = command.ExecuteReader();


                if (reader.Read())
                {
                    string typeName = reader["type_name"].ToString();
                    int totalQuantity = Convert.ToInt32(reader["total_quantity"]);
                    MessageBox.Show($"Тип товаров с наименьшим количеством: {typeName}\nОбщее количество товаров: {totalQuantity}");
                }
                else
                {
                    MessageBox.Show("Нет данных о типах товаров.");
                }
            });
        }

        private void ShowOlderThanNDays(object obj)
        {
            string userInput = Microsoft.VisualBasic.Interaction.InputBox("Введите количество дней:", "Ввод количества дней", "");

            if (int.TryParse(userInput, out int days))
            {
                string query = $"SELECT p.name AS product_name, s.supplier_name AS supplier_name, ps.price, s.amount, s.supply_date " +
                   $"FROM Supplies s " +
                   $"JOIN Products_Suppliers ps ON s.product_id = ps.product_id AND s.supplier_id = ps.supplier_id " +
                   $"JOIN Products p ON s.product_id = p.id " +
                   $"WHERE s.supply_date <= NOW() - INTERVAL '{days} days';";

                using NpgsqlCommand command = new NpgsqlCommand(query, connection);

                ExecuteIfConnectionOpen(() =>
                {
                    using NpgsqlDataReader reader = command.ExecuteReader();

                    // допишу с утра
                });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
