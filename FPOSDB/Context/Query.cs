using FPOSDB.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace FPOSDB.Context
{
    public static class Query
    {

        /// <summary>
        /// Updates the provided employee id to backoffice password to: Global_11$
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public static string UpdateBackOfficePassword(int employeeId)
        {

            string query = "UPDATE Employee " +
                            "SET Salt = 0x7CF0A88C3BABBB8D5FF1D4E3CC56D2097962EF0036A34E40, " +
                            "BackOfficePassword1 = 0x040A953962B1572B71A62420FCA3998E13EF97E0DC0A06C61C7347939258320C, " +
                            "BackOfficePassword2 = 0xB8D3CD7BEBE24A2C45EB8121695E58F22AE7B3106D96A5171A51B74B80965880, " +
                            "BackOfficePassword3 = 0x1E91CD03CBAF45508291DF1747E16C122FCEB979D852E047F555B74DD71D4B0C, " +
                            "BackOfficePassword4 = 0xF23CEA1525FD6DC0441B45934446D007CAEBE83F38E143EDA19E4890CA4EE64E, " +
                            "BackOfficeHash1 = 0x2304DE06120C0D8B49C1794B3D18886A5ECCA55BD8ED8046DB16473481F08832, " +
                            "BackOfficeHash2 = 0x8176298C89566694ECDA3D07C1B72F4BFCCBB6CF4141CC361EEA459F06B34D43, " +
                            "BackOfficeHash3 = 0x871D378468A4B410C84F5B60AE7AB0C59394DFC1D3928C52154FB34702738C03, " +
                            "BackOfficeHash4 = 0x66461ED54F5AC1D0963A8EBCEDE95F7AA57178D3D1F5616361B549BBA6F74EC4 " +
                            $"where EmpID = {employeeId}";
            return query;
        }

        /// <summary>
        /// Retrieve all items and prices
        /// </summary>
        /// <returns></returns>
        public static string SelectAllItemPrices()
        {
            //return "select * from ItemPrice " +
            //        "INNER JOIN Item on ItemPrice.ItemID = Item.ItemID";
            return "select * from Item FULL JOIN ItemPrice on Item.ItemId = ItemPrice.ItemID";
        }

        /// <summary>
        /// updates an items price when the ItemPrice already exists
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string UpdateExistingItemPrice(ItemPriceDTO newItem, ItemPriceDTO existingItem)
        {
            if ((newItem.ScheduleIndex != existingItem.ScheduleIndex) ||
                (newItem.ItemName.ToLower() != existingItem.ItemName.ToLower()))
                throw new ArgumentNullException(nameof(UpdateExistingItemPrice), "Item name and scheduled index must match!");

            var props = newItem.GetType().GetProperties();
            string query = "UPDATE ItemPrice SET ";
            foreach (var prop in props)
            {
                if (prop.Name != nameof(newItem.ItemName) && prop.Name != nameof(newItem.ItemID))
                    query += $"{prop.Name} = {prop.GetValue(newItem, null)}, ";
            }
            query = query.Substring(0, query.Length - 2) + " "; //remove comma at end of query
            query +=
                $"Where {nameof(existingItem.ItemID)} = '{existingItem.ItemID}' AND " +
                $"{nameof(existingItem.ScheduleIndex)} = {existingItem.ScheduleIndex}";

            return query;
        }


        public static string InsertItemPrice(ItemPriceDTO item)
        {

            var props = item.GetType().GetProperties();
            string query = "Insert INTO ItemPrice (";
            //query column headers
            foreach (var prop in props)
            {   
                  if (prop.Name != nameof(item.ItemName))
                        query += $"{prop.Name}, ";
            }
            query = query.Substring(0, query.Length - 2); //remove comma at end of query
            query += ") VALUES (";
            //query values
            foreach (var prop in props)
            {
                if (prop.Name == nameof(item.ItemID))
                    query += $"'{prop.GetValue(item, null)}', ";
                else if (prop.Name != nameof(item.ItemName))
                    query += $"{prop.GetValue(item, null)}, ";
            }
            query = query.Substring(0, query.Length - 2); //remove comma at end of query
            query += ")";

            return query;
        }

        /// <summary>
        /// Deletes all item price records associated with an item Name
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static string DeleteItemPricesByItemName(string itemName)
        {
            string query = "delete from ItemPrice " +
                "where ItemPrice.ItemID = " +
                "(select DISTINCT Item.ItemID from Item " +
                "FULL JOIN ItemPrice on Item.ItemId = ItemPrice.ItemID " +
                $"where Item.ItemName = '{itemName}')";
            return query;
        }

        /// <summary>
        /// Query that retrieves an Item Price given an item name and schedule index
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="scheduleIndex"></param>
        /// <returns></returns>
        public static string GetItemPrice(string itemName, string scheduleIndex)
        {
            return  "select * from ItemPrice " +
                    "FULL JOIN Item on ItemPrice.ItemID = Item.ItemID " +
                    $"Where Item.ItemName = '{itemName}' AND ItemPrice.ScheduleIndex = {scheduleIndex}";
        }

        public static string GetItemByName(string itemName)
        {
            return "select ItemID,ItemName from Item " +
                    $"Where ItemName = '{itemName}'";
        }
    }
}
