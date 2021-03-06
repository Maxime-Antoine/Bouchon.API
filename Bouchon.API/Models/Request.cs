﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bouchon.API.Models
{
    public class Request
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
        public ERequestStatus Status { get; set; }

        [Required]
        [StringLength(75, MinimumLength = 2, ErrorMessage = "Item name must be between 2 and 75 characters long")]
        public string ItemName { get; set; }

        [Required]
        [StringLength(350, MinimumLength = 2, ErrorMessage = "Item description must be between 2 and 350 characters long")]
        public string ItemDescription { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 75 characters long")]
        public string Category { get; set; }

        [DataType(DataType.Upload)]
        public byte[] Picture { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters long")]
        public string CityToBuy { get; set; }

        [Required]
        [StringLength(350, MinimumLength = 5, ErrorMessage = "Prefered dealing location must be between 5 and 350 characters long")]
        public string PreferedDealingLocation { get; set; }

        [Required]
        public string ProposedPrice { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public string Ccy { get; set; }
    }

    public enum ERequestStatus
    {
        Open,
        Accepted,
        Completing,
        Delivered,
        Closed
    }
}