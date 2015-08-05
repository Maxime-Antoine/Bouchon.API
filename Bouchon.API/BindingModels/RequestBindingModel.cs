using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bouchon.API.BindingModels
{
    public class RequestBindingModel
    {
        [Required]
        [StringLength(75, MinimumLength=2, ErrorMessage="Item name must be between 2 and 75 characters long")]
        public string ItemName { get; set; }

        [Required]
        [StringLength(350, MinimumLength=2, ErrorMessage="Item description must be between 2 and 350 characters long")]
        public string ItemDescription { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 75 characters long")]
        public string Category { get; set; }

        [DataType(DataType.Upload)]
        public byte[] Picture { get; set; }

        [DataType(DataType.Url)]
        [MaxLength(250)]
        public string Url { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters long")]
        public string CityToBuy { get; set; }

        [Required]
        [StringLength(350, MinimumLength=5, ErrorMessage = "Prefered dealing location must be between 5 and 350 characters long")]
        public string PreferedDealingLocation { get; set; }

        [Required]
        public string ProposedPrice { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public string Ccy { get; set; }
    }
}