using FPOSDB.DTO;
using FPOSDB.Parameters;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SqlKata;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FPOSDB.Attributes;
using FPOSDB.Extensions;

namespace FPOSDB.Context
{
    public class DBService
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string connectionString { get; set; }

        public DBService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool IsConnectionValid()
        {
            bool result = true;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                _log.Error("Database connection failed", ex);
                result = false;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Get All item prices from the DB
        /// </summary>
        /// <returns></returns>
        public List<ItemPrice> GetAllItemPrices()
        {
            try
            {
                return GetAll<ItemPrice>(QueryBuilder.SelectAllItemPrices());
            }
            catch (Exception ex)
            {
                _log.Error("Retrieving all item prices from db to list failed", ex);
                return new List<ItemPrice>();
            }
        }

        /// <summary>
        /// Get All buttons from db
        /// </summary>
        /// <returns></returns>
        public List<Button> GetAllButtons()
        {
            try
            {
                return GetAll<Button>(QueryBuilder.SelectAllButtons());
            }
            catch (Exception ex)
            {
                _log.Error("Retrieving all buttons from db to list failed", ex);
                return new List<Button>();
            }
        }

        public List<T> GetAll<T>(string query) where T : new()
        {
            SqlCommand command = null;
            SqlDataReader dataReader = null;
            SqlConnection connection = new SqlConnection(connectionString);
            var dbObjects = new List<T>();
            try
            {
                connection.Open();
                command = new SqlCommand(query, connection);
                dataReader = command.ExecuteReader();
                var dbObject = new T();
                var properties = dbObject.GetType().GetProperties();
                while (dataReader.Read())
                {
                    dbObject = new T();
                    foreach (var prop in properties)
                    {
                        var serializable = !Attribute.IsDefined(prop, typeof(NotSerializable));
                        if (serializable)
                        {
                            var newValue = dataReader[prop.Name].ToString();
                            prop.SetValue(dbObject, newValue, null);
                        }
                    }
                    dbObjects.Add(dbObject);
                }
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }
            return dbObjects;
        }

        public IImportResult<ItemPrice> UpdateItemPrices(List<ItemPrice> itemsToUpdate)
        {
            List<ItemPrice> newItems = new List<ItemPrice>(itemsToUpdate);
            List<ItemPrice> matchinItems = new List<ItemPrice>();
            var result = new ImportResult<ItemPrice>();


            foreach (ItemPrice item in newItems)
            {
                item.ItemID = GetItemIdByItemName(item.ItemName);

                if (String.IsNullOrEmpty(item.ItemID))
                    result.Ignored.Add(item);
                else
                {
                    matchinItems.Add(item);
                    DeleteItemPricesByName(item.ItemName);
                }
            }

            foreach (ItemPrice item in matchinItems)
            {
                if (item.IsZeroPrice())
                    result.Imported.Add(item);
                else
                {
                    var rowsInserted = InsertItemPrice(item);
                    if (rowsInserted > 0)
                        result.Imported.Add(item);
                    else
                        result.Failed.Add(item);
                }
            }

            return result;
        }

        public IImportResult<Button> UpdateButtonText(List<Button> buttons)
        {
            var newButtons = new List<Button>(buttons);
            var matchinItems = new List<Button>();
            var result = new ImportResult<Button>();

            foreach (Button newButton in newButtons)
            {
                var oldButton = GetButtonByButtonName(newButton.ButtonName);
                newButton.ButtonId = oldButton.ButtonId;

                //old button id wasnt found or button text is unchanged
                if (String.IsNullOrEmpty(oldButton.ButtonId) || 
                    oldButton?.Text.Equals(newButton.Text.ReplaceEscapedCharactors(), StringComparison.OrdinalIgnoreCase) != false)
                    result.Ignored.Add(newButton);
                else
                    matchinItems.Add(newButton);
            }

            foreach (Button button in matchinItems)
            {
                var rowUpdated = UpdateButtonText(button);
                if (rowUpdated > 0)
                    result.Imported.Add(button);
                else
                    result.Failed.Add(button);
            }

            return result;
        }

        private string GetItemIdByItemName(string name)
        {
            SqlCommand command = null;
            SqlDataReader dataReader = null;
            var sql = QueryBuilder.GetItemByName(name);
            var connection = new SqlConnection(connectionString);
            var item = new Item();
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                
                while (dataReader.Read())
                {
                    item.ItemId = dataReader[nameof(item.ItemId)].ToString();
                    item.ItemName = dataReader[nameof(item.ItemName)].ToString();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Retrieve an item price BY NAME in the DB failed", ex);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }
            return item.ItemId;
        }

        private Button GetButtonByButtonName(string buttonText)
        {
            SqlCommand command = null;
            SqlDataReader dataReader = null;
            var sql = QueryBuilder.GetButtonByName(buttonText);
            var connection = new SqlConnection(connectionString);
            var button = new Button();
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    button.ButtonId = dataReader[nameof(button.ButtonId)].ToString();
                    button.Text = dataReader[nameof(button.Text)].ToString();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Retrieve an item price BY NAME in the DB failed", ex);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }
            return button;
        }


        public int InsertItemPrice(ItemPrice item)
        {
            if (String.IsNullOrEmpty(item.ItemID))
                throw new ArgumentNullException("ItemID for item \"" + item.ItemName + "\" is missing while inserting new item price");

            SqlConnection connection = new SqlConnection(connectionString);
            int rowsUpdated = 0;
            string query;
            try
            {
                connection.Open();
                query = QueryBuilder.InsertItemPrice(item);
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {
                _log.Error("Inserting an item price into the DB failed", ex);
            }
            finally { connection.Close(); }

            return rowsUpdated;
        }

        public int UpdateButtonText(Button button)
        {
            if (String.IsNullOrEmpty(button.ButtonId))
                throw new ArgumentNullException("ButtonId for button \"" + button.ButtonId + "\" is missing while updated new button text");

            SqlConnection connection = new SqlConnection(connectionString);
            int rowsUpdated = 0;
            try
            {
                connection.Open();
                var sqlResult = QueryBuilder.UpdateButtonText(button);
                using (SqlCommand cmd = new SqlCommand(sqlResult.Sql,connection))
                {
                    foreach(var sql in sqlResult.NamedBindings)
                    {
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = sql.Key, Value = sql.Value.ToString().ReplaceEscapedCharactors() });
                    }
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Updating button text in DB failed", ex);
            }
            finally { connection.Close(); }

            return rowsUpdated;
        }

        /// <summary>
        /// Delete all item prices associated with an item name
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        private int DeleteItemPricesByName(string itemName)
        {
            int rowsDeleted = 0;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                string query = QueryBuilder.DeleteItemPricesByItemName(itemName);
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    rowsDeleted = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {
                _log.Error("Deleting an item price in the DB failed", ex);
            }
            finally { connection.Close(); }

            return rowsDeleted;
        }
    }
}
