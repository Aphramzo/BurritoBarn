namespace BurritoBarn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Rating")]
    public partial class Rating
    {
        public long id { get; set; }

        public long employeeId { get; set; }

        public long restaurantId { get; set; }

        [Column("rating")]
        public int rating1 { get; set; }

        [StringLength(2000)]
        public string comment { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Restaurant Restaurant { get; set; }
    }
}
