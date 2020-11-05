using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomBooking.Core.Entities
{
    public class Customer : EntityObject
    {
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Iban { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName}";

        public ICollection<Booking> Bookings { get; set; }

        public Customer()
        {
            Bookings = new List<Booking>();
        }
    }
}
