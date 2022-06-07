using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSDB.DTO
{
    public class Button : BaseModel<Button>
    {
        public string ButtonId { get; set; }
        public string ButtonName { get; set; }
        public string Text { get; set; }

        public override string DisplayName => ButtonName;
        public override string PrimaryKey => ButtonId;

        public override string ToString()
        {
            return "Button Name: " + DisplayName;
        }
    }
}
