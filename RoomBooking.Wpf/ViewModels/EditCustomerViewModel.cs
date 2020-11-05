using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Core.Validations;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class EditCustomerViewModel : BaseViewModel
    {
        private Customer _customer;
        private string _firstNameOnOpen;
        private string _lastNameOnOpen;
        private string _ibanOnOpen;
        private string _firstName;
        private string _lastName;
        private string _iban;

        public string Iban 
        {
            get => _iban;
            set
            {
                _iban = value;
                OnPropertyChanged();
                Validate();
            }
        }

        [Required]
        [MinLength(2, ErrorMessage = "Mindetslänge des Vornamens ist 2 Zeichen!")]
        public string FirstName 
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        [Required]
        [MinLength(2, ErrorMessage = "Mindetslänge des Nachnames ist 2 Zeichen!")]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public Customer Customer 
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }

        private ICommand _cmdSaveChanges;
        public ICommand CmdSaveChanges 
        { 
            get
            {
                if(_cmdSaveChanges == null)
                {
                    _cmdSaveChanges = new RelayCommand(
                        execute: async _ => 
                        {
                            await SaveChangesAsync();
                        },
                        canExecute: _ => IsValid && string.IsNullOrEmpty(DbError));
                }

                return _cmdSaveChanges;
            }
            set { _cmdSaveChanges = value; }
        }

        private ICommand _cmdUndo;
        public ICommand CmdUndo
        {
            get
            {
                if(_cmdUndo == null)
                {
                    _cmdUndo = new RelayCommand(
                        execute: _ =>
                        {
                            FirstName = _firstNameOnOpen;
                            LastName = _lastNameOnOpen;
                            Iban = _ibanOnOpen;
                        },
                        canExecute: _ => true);
                }
                return _cmdUndo;
            }
        }

        private async Task SaveChangesAsync()
        {
            using IUnitOfWork unitOfWork = new UnitOfWork();

            try
            {
                Customer.FirstName = FirstName;
                Customer.LastName = LastName;
                Customer.Iban = Iban;

                unitOfWork.Customers.Update(Customer); //Nötig weil Entity nicht getrackt wurde
                await unitOfWork.SaveAsync();

                Controller.CloseWindow(this);
            }
            catch(ValidationException ex)
            {
                DbError = ex.ValidationResult.ToString();
            }
        }

        public EditCustomerViewModel(IWindowController windowController) : base(windowController)
        {

        }

        public async static Task<EditCustomerViewModel> CreateAsync(IWindowController windowController, int customerId)
        {
            var viewModel = new EditCustomerViewModel(windowController);
            await viewModel.LoadDataAsync(customerId);
            return viewModel;
        }

        private async Task LoadDataAsync(int customerId)
        {
            using (IUnitOfWork uow = new UnitOfWork())
            {
                _customer = await uow.Customers.GetByIdAsync(customerId);
                FirstName = _customer?.FirstName;
                _firstNameOnOpen = FirstName;
                LastName = _customer?.LastName;
                _lastNameOnOpen = LastName;
                Iban = _customer?.Iban;
                _ibanOnOpen = Iban;
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!IbanChecker.CheckIban(Iban))
            {
                yield return new ValidationResult("Der eingegebene Iban ist ungültig", new string[] { nameof(Iban) });
            }
        }
    }
}
