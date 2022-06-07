using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSDB.DTO
{
    public class Item : BaseModel<Item>
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }

        public override string DisplayName => ItemName;
        public override string PrimaryKey => ItemId;

        public override string ToString()
        {
            return "Item Name: " + DisplayName;
        }
    }
}