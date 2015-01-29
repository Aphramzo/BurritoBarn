namespace BurritoBarn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Restaurant")]
    public partial class Restaurant
    {
        public Restaurant()
        {
            Ratings = new HashSet<Rating>();
            Supplies = new HashSet<Supply>();
        }

        public long id { get; set; }

        public long cuisineId { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public virtual Cuisine Cuisine { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Supply> Supplies { get; set; }
    }
}
