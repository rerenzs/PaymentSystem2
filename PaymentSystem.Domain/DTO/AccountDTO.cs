using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PaymentSystem.Domain.DTO
{
    public class AccountDTO
    {
        public long ID { get; set; }
        [Required]
        public int AccountNumber { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Balance { get; set; }
        public IEnumerable<PaymentDTO> Payments { get; set; }
    }
}
