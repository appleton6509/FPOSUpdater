using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSDB.DTO
{
    public class ItemPriceDTO 
    {
        public string ItemName { get; set; }
        public string ItemID { get; set; }
        public string ScheduleIndex { get; set; }
        public string DefaultPrice { get; set; }
        public string Level1Price { get; set; }
        public string Level2Price { get; set; }
        public string Level3Price { get; set; }
        public string Level4Price { get; set; }
        public string Level5Price { get; set; }
        public string Level6Price { get; set; }
        public string Level7Price { get; set; }
        public string Level8Price { get; set; }
        public string Level9Price { get; set; }
        public string Level10Price { get; set; }
        public string Level11Price { get; set; }
        public string Level12Price { get; set; }
        public string Level13Price { get; set; }
        public string Level14Price { get; set; }
        public string Level15Price { get; set; }
        public string Level16Price { get; set; }
        public string Level17Price { get; set; }
        public string Level18Price { get; set; }
        public string Level19Price { get; set; }
        public string Level20Price { get; set; }
        public string Level21Price { get; set; }
        public string Level22Price { get; set; }
        public string Level23Price { get; set; }
        public string Level24Price { get; set; }
        public string Level25Price { get; set; }
        public string Level26Price { get; set; }
        public string Level27Price { get; set; }
        public string Level28Price { get; set; }
        public string Level29Price { get; set; }

        public override string ToString()
        {
            return "Item Name: " + this.ItemName;
        }
        public bool IsZeroPrice()
        {
            var props = this.GetType().GetProperties();
            bool isPriceZero = true;
            foreach (var prop in props)
            {
                string value = (string)prop.GetValue(this, null);
                if (prop.Name.Contains("Price") && !String.IsNullOrEmpty(value) && (Int32.Parse(value) > 0))
                {
                    isPriceZero = false;
                    break;
                }
            }
            return isPriceZero;
        }


        public bool AreEqual(ItemPriceDTO item)
        {
            bool areEqual = true;
            var props = this.GetType().GetProperties();
   
            foreach (var prop in props)
            {
                var thisPropValue = (int)prop.GetValue(this, null);
                var otherPropValue = (int)prop.GetValue(item, null);
                if (thisPropValue != otherPropValue)
                {
                    areEqual = false;
                    break;
                }
            }

            return areEqual;
        }
    }
}
