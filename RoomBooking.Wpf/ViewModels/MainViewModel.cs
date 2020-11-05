using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<Booking> _bookings;
        private List<Room> _rooms;
        private Room _selectedRoom;
        private Booking _selectedBooking;

        public Booking SelectedBooking 
        {
            get => _selectedBooking; 
            set
            {
                _selectedBooking = value;
                OnPropertyChanged();
            }
        }

        public Room SelectedRoom 
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
            }
        }

        public List<Room> Rooms 
        {
            get => _rooms;
            set
            {
                _rooms = value;
            }
        }

        public ObservableCollection<Booking> Bookings 
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged();
            }
        }

        private ICommand _cmdEditCustomer;
        public ICommand CmdEditCustomer 
        { 
            get
            {
                if(_cmdEditCustomer == null)
                {
                    _cmdEditCustomer = new RelayCommand(
                        execute: async _ =>
                        {
                            var editCustomerViewModel = await EditCustomerViewModel.CreateAsync(Controller, SelectedBooking.CustomerId);
                            Controller.ShowWindow(editCustomerViewModel, true);
                            await LoadDataAsync();
                        },
                        canExecute: _ => SelectedBooking != null);
                };

                return _cmdEditCustomer;
            }
            set { _cmdEditCustomer = value; }
        }

        public MainViewModel(IWindowController windowController) : base(windowController)
        {
        }

        private async Task LoadDataAsync()
        {
            using (IUnitOfWork uow = new UnitOfWork())
            {
                var rooms = await uow.Rooms.GetAllWithBookingsAsync();
                _rooms = new List<Room>(rooms);
                SelectedRoom = Rooms.FirstOrDefault();

                var bookings = SelectedRoom.Bookings;

                _bookings = new ObservableCollection<Booking>(bookings);
            }
        }

        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
