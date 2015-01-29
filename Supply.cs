namespace BurritoBarn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Supply")]
    public partial class Supply
    {
        public long id { get; set; }

        public long employeeId { get; set; }

        public long restaurantId { get; set; }

        public DateTime suppliedDate { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Restaurant Restaurant { get; set; }
    }
}
