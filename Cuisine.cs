namespace BurritoBarn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cuisine")]
    public partial class Cuisine
    {
        public Cuisine()
        {
            Restaurants = new HashSet<Restaurant>();
        }

        public long id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
