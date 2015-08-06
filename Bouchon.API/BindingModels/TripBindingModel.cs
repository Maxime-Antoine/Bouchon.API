using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bouchon.API.BindingModels
{
    public class CreateTripBindingModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage="City must be between 2 and 50 characters long")]
        public string FromCity { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage="City must be between 2 and 50 characters long")]
        public string ToCity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
    }
}