using FPOSDB.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;

namespace FPOSDB.Context
{
    public class DBService
    {
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
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public ObservableCollection<string> GetOpenTables()
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader dataReader = null;

            string sql = "SELECT * FROM Sale WHERE isSuspend=1";
            connection = new SqlConnection(connectionString);
            ObservableCollection<string> tables = new ObservableCollection<string>();
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                    tables.Add(dataReader["CheckNumber"].ToString());

            }
            catch (Exception)
            {
                //Do nothing
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }

            return tables;
        }

        public int DeleteSale(int checkNumber)
        {
            int rowsUpdated = 0;

            string sql = "DELETE FROM Sale WHERE CheckNumber=@checknumber";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@checknumber", checkNumber.ToString());
            try
            {
                connection.Open();
                try
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally { cmd.Dispose(); }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return rowsUpdated;
        }

        public int DeleteItemSold(int checkNumber)
        {
            int rowsUpdated = 0;

            string sql = "DELETE FROM ItemSold WHERE CheckNumber=@checknumber";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@checknumber", checkNumber.ToString());
            try
            {
                connection.Open();
                try
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally { cmd.Dispose(); }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return rowsUpdated;
        }

        public int ResetEmpPassword(int employeeId)
        {
            int rowsUpdated = 0;

            //query to update EMPLOYEE PASSWORD to: Global_11$
            string query = Query.UpdateBackOfficePassword(employeeId);

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                try
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally { cmd.Dispose(); }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return rowsUpdated;
        }

        public ObservableCollection<string> GetEmployees()
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader dataReader = null;

            string sql = "SELECT EmpID FROM Employee";
            connection = new SqlConnection(connectionString);
            ObservableCollection<string> employees = new ObservableCollection<string>();
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                    employees.Add(dataReader["EmpID"].ToString());

            }
            catch (Exception)
            {
                //Do nothing
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }
            return employees;
        }

        /// <summary>
        /// Get All item prices from the DB
        /// </summary>
        /// <returns></returns>
        public List<ItemPriceDTO> GetAllItemPrices()
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader dataReader = null;

            string sql = Query.SelectAllItemPrices();

            connection = new SqlConnection(connectionString);
            List<ItemPriceDTO> items = new List<ItemPriceDTO>();
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                ItemPriceDTO item;
                while (dataReader.Read())
                {
                    item = new ItemPriceDTO();
                    foreach(var prop in item.GetType().GetProperties())
                    {
                            var newValue = dataReader[prop.Name].ToString();
                            prop.SetValue(item, newValue, null);
                    }
                    items.Add(item);
                }
            }
            catch (Exception)
            {
                //Do nothing
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }

            return items;
        }

        public List<ItemPriceDTO> UpdateItemPrices(List<ItemPriceDTO> items)
        {
            List<ItemPriceDTO> oldItems = GetAllItemPrices();
            List<ItemPriceDTO> newItems = new List<ItemPriceDTO>(items);
            List<ItemPriceDTO> noPriceItems = newItems.Where(x => x.IsZeroPrice()).ToList();

            //delete all item prices associated with an item name with NO PRICING.
            foreach (ItemPriceDTO item in noPriceItems)
            {
                DeleteItemPricesByName(item.ItemName);
                newItems.Remove(item);
                oldItems.RemoveAll(x => x.ItemName.ToUpper() == item.ItemName.ToUpper());
            }

            //find matches of item names between the old and new pricing
            var matchingItems = oldItems.Select(x => x.ItemName).Intersect(newItems.Select(x => x.ItemName));

            //delete in DB any items that have price updates
            foreach(string itemName in matchingItems)
            {
                DeleteItemPricesByName(itemName);
            }

            //insert in DB the new prices
            foreach(ItemPriceDTO item in newItems)
            {
                item.ItemID = GetItemByName(item.ItemName).ItemId;
                InsertItemPrice(item);
            }

            return null;
 
        }

        private ItemDTO GetItemByName(string itemName)
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader dataReader = null;

            string sql = Query.GetItemByName(itemName);
            connection = new SqlConnection(connectionString);
            ItemDTO item = new ItemDTO();
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
            catch (Exception)
            {
                //Do nothing
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }
            return item;
        }

        public int UpdateItemPrice(ItemPriceDTO newItem, ItemPriceDTO existingItem)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            int rowsUpdated = 0;
            try
            {
                connection.Open();
                string query = Query.UpdateExistingItemPrice(newItem, existingItem);
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception) { /*Do nothing*/ }
            finally { connection.Close(); }

            return rowsUpdated;
        }

        public int InsertItemPrice(ItemPriceDTO item)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            int rowsUpdated = 0;
            try
            {
                connection.Open();
                string query = Query.InsertItemPrice(item);
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception) { /*Do nothing*/ }
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
                string query = Query.DeleteItemPricesByItemName(itemName);
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    rowsDeleted = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception) { /*Do Nothing*/}
            finally { connection.Close(); }

            return rowsDeleted;
        }

        /// <summary>
        /// Gets all item prices for a single item given a schedule index and itemname
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ItemPriceDTO GetItemPricesByNameAndIndex(ItemPriceDTO item)
        {
            if (String.IsNullOrEmpty(item.ItemName) || String.IsNullOrEmpty(item.ScheduleIndex))
                throw new ArgumentNullException(nameof(GetItemPricesByNameAndIndex), "Item name and scheduled index must have values");

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = null;
            SqlDataReader dataReader = null;
            ItemPriceDTO returnItem = null;

            string sql = Query.GetItemPrice(item.ItemName, item.ScheduleIndex);

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    returnItem = new ItemPriceDTO();
                    foreach (var prop in item.GetType().GetProperties())
                    {
                        var newValue = dataReader[prop.Name].ToString();
                        prop.SetValue(item, newValue, null);
                    }
                }
            }
            catch (Exception) {/*do nothing*/}
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (command != null)
                    command.Dispose();
                connection.Close();
            }
            return returnItem;
        }

    }
}
