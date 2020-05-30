using FPOSDB.DTO;
using FPOSDB.Parameters;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
        public List<ItemPriceDTO> GetAllItemPrices()
        {
            SqlCommand command = null;
            SqlDataReader dataReader = null;
            SqlConnection connection = new SqlConnection(connectionString);
            List<ItemPriceDTO> items = new List<ItemPriceDTO>();
            try
            {
                connection.Open();

                string sql = Query.SelectAllItemPrices();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                ItemPriceDTO item;
                while (dataReader.Read())
                {
                    item = new ItemPriceDTO();
                    foreach(var prop in item.GetType().GetProperties())
                    {
                        string newValue = dataReader[prop.Name].ToString().ToUpper();
                            prop.SetValue(item, newValue, null);
                    }

                    items.Add(item);
                }
            
            }
            catch (Exception ex)
            {
                _log.Error("Retrieving all item prices from db to list failed", ex);
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


        public ImportResult UpdateItemPrices(List<ItemPriceDTO> itemsToUpdate)
        {
            List<ItemPriceDTO> newItems = new List<ItemPriceDTO>(itemsToUpdate);
            List<ItemPriceDTO> matchinItems = new List<ItemPriceDTO>();
            ImportResult result = new ImportResult();


            foreach (ItemPriceDTO item in newItems)
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

            foreach (ItemPriceDTO item in matchinItems)
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

        //public ImportResult UpdateItemPricess(List<ItemPriceDTO> itemsToUpdate)
        //{
        //    List<ItemPriceDTO> oldItems = GetAllItemPrices();
        //    List<ItemPriceDTO> newItems = new List<ItemPriceDTO>(itemsToUpdate);
        //    List<ItemPriceDTO> itemsImportFailed = new List<ItemPriceDTO>();
        //    List<ItemPriceDTO> itemsHandled = new List<ItemPriceDTO>();
        //    List<ItemPriceDTO> noPriceItems = newItems.Where(x => x.IsZeroPrice()).ToList();
        //    var matchingItems = oldItems.Select(x => x.ItemName).Intersect(newItems.Select(x => x.ItemName)).ToList();
        //    ImportResult result = new ImportResult();

        //    //delete all item prices associated with an item name with NO PRICING.
        //    foreach (ItemPriceDTO item in noPriceItems)
        //    {
        //        var existingItem = GetItemIdByItemName(item.ItemName);

        //        if (existingItem != null)
        //        {
        //            DeleteItemPricesByName(item.ItemName);
        //            result.SuccessItems.Add(item);
        //            //itemsHandled.Add(item);
        //        }
        //        else
        //        {
        //            result.IgnoredItems.Add(item);
        //        }
        //        newItems.Remove(item);
        //    }


        //    //insert in DB the new prices
        //    foreach (ItemPriceDTO item in newItems)
        //    {
        //        item.ItemID = GetItemIdByItemName(item.ItemName);
        //        int itemHandled = 0;

        //        //item exists in both DB's, proceed inserting item price
        //        if (!String.IsNullOrEmpty(item.ItemID))
        //        {
        //            DeleteItemPricesByName(item.ItemName);
        //            itemHandled = InsertItemPrice(item);
        //        }
        //        else
        //        {
        //            result.IgnoredItems.Add(item);
        //            itemsHandled.Add(item);
        //            itemHandled = 1;
        //        }

        //        if (itemHandled == 0)
        //        {
        //            var isAlreadyInList = itemsImportFailed.FirstOrDefault(x => x.ItemName == item.ItemName) != null ? true : false;
        //            var isAlreadyImported = itemsHandled.FirstOrDefault(x => x.ItemName == item.ItemName) != null ? true : false;

        //            if (!isAlreadyInList || !isAlreadyImported)
        //                itemsImportFailed.Add(item);
        //        }
        //        else
        //        {
        //            result.SuccessItems.Add(item);
        //        }
        //    }
        //    return itemsImportFailed;
        //}


        //public List<ItemPriceDTO> UpdateItemPricess(List<ItemPriceDTO> itemsToUpdate)
        //{
        //    List<ItemPriceDTO> oldItems = GetAllItemPrices();
        //    List<ItemPriceDTO> newItems = new List<ItemPriceDTO>(itemsToUpdate);
        //    List<ItemPriceDTO> itemsNotImported = new List<ItemPriceDTO>();
        //    List<ItemPriceDTO> itemsImported = new List<ItemPriceDTO>();
        //    List<ItemPriceDTO> noPriceItems = newItems.Where(x => x.IsZeroPrice()).ToList();

        //    //delete all item prices associated with an item name with NO PRICING.
        //    foreach (ItemPriceDTO item in noPriceItems)
        //    {
        //        DeleteItemPricesByName(item.ItemName);
        //        itemsImported.Add(item);
        //    }

        //    //find matches of item names between the old and new pricing
        //    var matchingItems = oldItems.Select(x => x.ItemName).Intersect(newItems.Select(x => x.ItemName)).ToList();

        //    //delete in DB any items that have price updates
        //    foreach (string itemName in matchingItems)
        //    {
        //        DeleteItemPricesByName(itemName);
        //    }

        //    //insert in DB the new prices
        //    foreach (ItemPriceDTO item in newItems)
        //    {
        //        item.ItemID = GetItemIdByItemName(item.ItemName).ItemId;
        //        int rowsUpdated = 0;

        //        //item exists in DB, proceed inserting item price
        //        if (!Object.Equals(item, null) && !String.IsNullOrEmpty(item.ItemID))
        //            rowsUpdated = InsertItemPrice(item);

        //        if (rowsUpdated == 0)
        //        {
        //            var isAlreadyInList = itemsNotImported.FirstOrDefault(x => x.ItemName == item.ItemName) != null ? true : false;
        //            var isAlreadyImported = itemsImported.FirstOrDefault(x => x.ItemName == item.ItemName) != null ? true : false;

        //            if (!isAlreadyInList || !isAlreadyImported)
        //                itemsNotImported.Add(item);
        //        }
        //    }
        //    return itemsNotImported;
        //}

        private string GetItemIdByItemName(string itemName)
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
            catch (Exception ex) {
                _log.Error("Updating an item price in the DB failed", ex);
            }
            finally { connection.Close(); }

            return rowsUpdated;
        }
        public int InsertItemPrice(ItemPriceDTO item)
        {
            if (String.IsNullOrEmpty(item.ItemID))
                throw new ArgumentNullException("ItemID for item \"" + item.ItemName + "\" is missing while inserting new item price");

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
            catch (Exception ex) {
                _log.Error("Inserting an item price into the DB failed", ex);
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
                string query = Query.DeleteItemPricesByItemName(itemName);
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
