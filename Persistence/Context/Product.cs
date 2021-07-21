using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    [Table("Product")]
    [Index(nameof(Modified), Name = "IX_Product_Modified")]
    [Index(nameof(ModifiedBy), Name = "IX_Post_ModifiedBy")]
    public class Product
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(2000)]
        public string Brand { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expiration { get; set; }
        public bool IsInStock { get; set; }
        public int Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }

    }
}