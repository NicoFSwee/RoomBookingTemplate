using RoomBooking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace RoomBooking.ImportConsole
{
    public static class ImportController
    {
        /// <summary>
        /// Liest die Buchungen mit ihren Räumen und Kunden aus der
        /// csv-Datei ein.
        /// </summary>
        /// <returns></returns>
        /// 

        const int LASTNAME_IDX = 0;
        const int FIRSTNAME_IDX = 1;
        const int IBAN_IDX = 2;
        const int ROOMNR_IDX = 3;
        const int FROM_IDX = 4;
        const int TO_IDX = 5;

        public static async Task<IEnumerable<Booking>> ReadBookingsFromCsvAsync()
        {
            string[][] matrix = await MyFile.ReadStringMatrixFromCsvAsync("bookings.csv", true);

            var customers = matrix.GroupBy(c => c[IBAN_IDX])
                                    .Select(c => new Customer()
                                    {
                                        Iban = c.Key,
                                        FirstName = c.Select(_ => _[FIRSTNAME_IDX]).First(),
                                        LastName = c.Select(_ => _[LASTNAME_IDX]).First(),
                                        Bookings = new List<Booking>()
                                    }).ToDictionary(c => c.Iban);

            var rooms = matrix.GroupBy(r => r[ROOMNR_IDX])
                                .Select(r => new Room
                                {
                                    RoomNumber = r.Key,
                                    Bookings = new List<Booking>()
                                }).ToDictionary(r => r.RoomNumber);

            var bookings = matrix.Select(b => new Booking
            {
                Customer = customers[b[IBAN_IDX]],
                Room = rooms[b[ROOMNR_IDX]],
                From = b[FROM_IDX],
                To = b[TO_IDX]
            }).ToArray();

            return bookings;
        }

    }
}
