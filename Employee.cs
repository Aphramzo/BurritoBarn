namespace BurritoBarn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Employee")]
    public partial class Employee
    {
        public Employee()
        {
            Ratings = new HashSet<Rating>();
            Supplies = new HashSet<Supply>();
        }

        public long id { get; set; }

        [StringLength(50)]
        public string firstName { get; set; }

        [StringLength(50)]
        public string lastName { get; set; }

        public string password { get; set; }

        [Required]
        [StringLength(250)]
        public string emailAddress { get; set; }

        [StringLength(50)]
        public string phoneNumber { get; set; }

        public bool? isActive { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Supply> Supplies { get; set; }
    }
}
