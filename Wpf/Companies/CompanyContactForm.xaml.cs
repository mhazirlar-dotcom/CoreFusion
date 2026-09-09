using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using System.Windows;
using Wpf.Companies.Models;
using MessageBox = System.Windows.MessageBox;

namespace Wpf.Companies;

public partial class CompanyContactForm : Window,ITransientService
{
    #region Fields

    private readonly IMasterApiClient _masterApiClient;
    private readonly CompanyContactItem? _editItem;
    private List<MasterDefinitionResponse> _contactTypes = [];

    #endregion Fields

    #region Properties

    public CompanyContactItem? Result { get; private set; }

    #endregion Properties

    #region Constructors

    public CompanyContactForm(IMasterApiClient masterApiClient)
    {
        ArgumentNullException.ThrowIfNull(masterApiClient);

        _masterApiClient = masterApiClient;

        InitializeComponent();

        Loaded += CompanyContactForm_Loaded;
    }

    public CompanyContactForm(IMasterApiClient masterApiClient , CompanyContactItem editItem) : this(masterApiClient)
    {
        ArgumentNullException.ThrowIfNull(editItem);

        _editItem = editItem;
    }

    #endregion Constructors

    #region Events

    private async void CompanyContactForm_Loaded(object sender , RoutedEventArgs e)
    {
        await LoadDataAsync();

        if (_editItem is not null)
        {
            LoadEditData();
        }
    }

    private void SaveButton_Click(object sender , RoutedEventArgs e)
    {
        MasterDefinitionResponse? selectedContactType = ContactTypeComboBox.SelectedItem as MasterDefinitionResponse;

        if (selectedContactType is null)
        {
            MessageBox.Show(
                "Lütfen iletişim tipi seçin." ,
                "İletişim" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        if (string.IsNullOrWhiteSpace(FirstNameTextEdit.Text))
        {
            MessageBox.Show(
                "Lütfen ad girin." ,
                "İletişim" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            FirstNameTextEdit.Focus();

            return;
        }

        CompanyContactItem contact = _editItem ?? new CompanyContactItem
        {
            Id = Guid.NewGuid()
        };

        contact.ContactTypeId = selectedContactType.Id;
        contact.ContactType = selectedContactType.Name;
        contact.FirstName = FirstNameTextEdit.Text.Trim();
        contact.LastName = LastNameTextEdit.Text.Trim();
        contact.Title = TitleTextEdit.Text.Trim();
        contact.Phone = PhoneTextEdit.Text.Trim();
        contact.MobilePhone = MobilePhoneTextEdit.Text.Trim();
        contact.Email = EmailTextEdit.Text.Trim();
        contact.IsAuthorized = IsAuthorizedCheckBox.IsChecked == true;
        contact.IsDefault = IsDefaultCheckBox.IsChecked == true;

        Result = contact;

        DialogResult = true;
    }

    private void CancelButton_Click(object sender , RoutedEventArgs e)
    {
        DialogResult = false;
    }

    #endregion Events

    #region Methods

    private async Task LoadDataAsync()
    {
        try
        {
            _contactTypes = await _masterApiClient.GetContactTypesAsync();

            ContactTypeComboBox.ItemsSource = _contactTypes;
            ContactTypeComboBox.DisplayMember = nameof(MasterDefinitionResponse.Name);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "İletişim Bilgileri" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private void LoadEditData()
    {
        if (_editItem is null)
        {
            return;
        }

        ContactTypeComboBox.SelectedItem = _contactTypes.FirstOrDefault(contactType => contactType.Id == _editItem.ContactTypeId);
        FirstNameTextEdit.Text = _editItem.FirstName;
        LastNameTextEdit.Text = _editItem.LastName;
        TitleTextEdit.Text = _editItem.Title;
        PhoneTextEdit.Text = _editItem.Phone;
        MobilePhoneTextEdit.Text = _editItem.MobilePhone;
        EmailTextEdit.Text = _editItem.Email;
        IsAuthorizedCheckBox.IsChecked = _editItem.IsAuthorized;
        IsDefaultCheckBox.IsChecked = _editItem.IsDefault;
    }

    #endregion Methods
}