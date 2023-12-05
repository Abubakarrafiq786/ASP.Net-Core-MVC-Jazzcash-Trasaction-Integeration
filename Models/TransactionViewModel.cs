using System.ComponentModel.DataAnnotations;

namespace JazzCashIntegeration.Models
{
    public class TransactionViewModel
    {
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Destination Number is required")]
        public string DestinationNumber { get; set; }

        public string BillReference { get; set; } = "billref"; 

        public string Description { get; set; } = "Description of transaction"; 
    }
}
