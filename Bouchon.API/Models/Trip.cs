using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bouchon.API.Models
{
    public class Trip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters long")]
        public string FromCity { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters long")]
        public string ToCity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
    }
}