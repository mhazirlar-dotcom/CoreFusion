using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Companies.Models;
using Wpf.State;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Wpf.Companies;

public partial class CompanyForm : UserControl, ITransientService
{
    #region Fields

    private readonly IMasterApiClient _masterApiClient;
    private readonly ICompanyApiClient _companyApiClient;
    private readonly CompanyState _companyState;
    private readonly ObservableCollection<CompanyAddressItem> _addresses = [];
    private readonly ObservableCollection<CompanyContactItem> _contacts = [];
    private List<MasterDefinitionResponse> _taxOffices = [];
    private Guid? _companyId;

    #endregion Fields

    #region Constructors

    public CompanyForm(IMasterApiClient masterApiClient , ICompanyApiClient companyApiClient , CompanyState companyState)
    {
        ArgumentNullException.ThrowIfNull(masterApiClient);
        ArgumentNullException.ThrowIfNull(companyApiClient);
        ArgumentNullException.ThrowIfNull(companyState);

        _masterApiClient = masterApiClient;
        _companyApiClient = companyApiClient;
        _companyState = companyState;

        InitializeComponent();

        AddressesGridControl.ItemsSource = _addresses;
        ContactsGridControl.ItemsSource = _contacts;

        Loaded += CompanyForm_Loaded;
    }

    #endregion Constructors

    #region Events

    private async void CompanyForm_Loaded(object sender , RoutedEventArgs e)
    {
        if (_taxOffices.Count == 0)
        {
            await LoadDataAsync();
        }
    }

    private void NewAddressButton_Click(object sender , RoutedEventArgs e)
    {
        CompanyAddressForm form = new(_masterApiClient)
        {
            Owner = Window.GetWindow(this)
        };

        if (form.ShowDialog() != true || form.Result is null)
        {
            return;
        }

        if (form.Result.IsDefault)
        {
            ClearDefaultAddress();
        }

        _addresses.Add(form.Result);
    }

    private void EditAddressButton_Click(object sender , RoutedEventArgs e)
    {
        if (AddressesGridControl.SelectedItem is not CompanyAddressItem selectedAddress)
        {
            MessageBox.Show(
                "Lütfen düzenlemek istediğiniz adresi seçin." ,
                "Adres" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        CompanyAddressForm form = new(_masterApiClient , selectedAddress)
        {
            Owner = Window.GetWindow(this)
        };

        if (form.ShowDialog() != true || form.Result is null)
        {
            return;
        }

        if (form.Result.IsDefault)
        {
            ClearDefaultAddress(form.Result.Id);
        }

        AddressesGridControl.RefreshData();
    }

    private void DeleteAddressButton_Click(object sender , RoutedEventArgs e)
    {
        if (AddressesGridControl.SelectedItem is not CompanyAddressItem selectedAddress)
        {
            MessageBox.Show(
                "Lütfen silmek istediğiniz adresi seçin." ,
                "Adres" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        MessageBoxResult result = MessageBox.Show(
            "Seçili adres silinecek. Devam etmek istiyor musunuz?" ,
            "Adres Sil" ,
            MessageBoxButton.YesNo ,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        _addresses.Remove(selectedAddress);
    }

    private void NewContactButton_Click(object sender , RoutedEventArgs e)
    {
        CompanyContactForm form = new(_masterApiClient)
        {
            Owner = Window.GetWindow(this)
        };

        if (form.ShowDialog() != true || form.Result is null)
        {
            return;
        }

        if (form.Result.IsDefault)
        {
            ClearDefaultContact();
        }

        _contacts.Add(form.Result);
    }

    private void EditContactButton_Click(object sender , RoutedEventArgs e)
    {
        if (ContactsGridControl.SelectedItem is not CompanyContactItem selectedContact)
        {
            MessageBox.Show(
                "Lütfen düzenlemek istediğiniz iletişimi seçin." ,
                "İletişim" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        CompanyContactForm form = new(_masterApiClient , selectedContact)
        {
            Owner = Window.GetWindow(this)
        };

        if (form.ShowDialog() != true || form.Result is null)
        {
            return;
        }

        if (form.Result.IsDefault)
        {
            ClearDefaultContact(form.Result.Id);
        }

        ContactsGridControl.RefreshData();
    }

    private void DeleteContactButton_Click(object sender , RoutedEventArgs e)
    {
        if (ContactsGridControl.SelectedItem is not CompanyContactItem selectedContact)
        {
            MessageBox.Show(
                "Lütfen silmek istediğiniz iletişimi seçin." ,
                "İletişim" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        MessageBoxResult result = MessageBox.Show(
            "Seçili iletişim silinecek. Devam etmek istiyor musunuz?" ,
            "İletişim Sil" ,
            MessageBoxButton.YesNo ,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        _contacts.Remove(selectedContact);
    }

    private async void SaveButton_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            if (TaxOfficeComboBox.SelectedItem is not MasterDefinitionResponse selectedTaxOffice)
            {
                MessageBox.Show(
                    "Lütfen vergi dairesi seçin." ,
                    "Firma" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            Guid taxOfficeId = selectedTaxOffice.Id;

            if (_companyId is not null)
            {
                UpdateCompanyRequest request = new(
                    _companyId.Value,
                    CompanyNameTextEdit.Text,
                    ShortNameTextEdit.Text,
                    TaxNumberTextEdit.Text,
                    taxOfficeId,
                    TradeRegistryNumberTextEdit.Text,
                    MersisNumberTextEdit.Text,
                    WebsiteTextEdit.Text);

                IResult result = await _companyApiClient.UpdateAsync(request);

                if (!result.Success)
                {
                    MessageBox.Show(
                        result.Message ,
                        "Firma Güncelle" ,
                        MessageBoxButton.OK ,
                        MessageBoxImage.Warning);

                    return;
                }

                IDataResult<CompanyModel> companyResult = await _companyApiClient.GetByIdAsync(_companyId.Value);

                if (!companyResult.Success)
                {
                    MessageBox.Show(
                        companyResult.Message ,
                        "Firma Güncelle" ,
                        MessageBoxButton.OK ,
                        MessageBoxImage.Warning);

                    return;
                }

                _companyState.Update(companyResult.Data);

                MessageBox.Show(
                    result.Message ,
                    "Firma Güncelle" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Information);

                return;
            }

            CreateCompanyRequest createRequest = new(
                CompanyNameTextEdit.Text,
                ShortNameTextEdit.Text,
                TaxNumberTextEdit.Text,
                taxOfficeId,
                TradeRegistryNumberTextEdit.Text,
                MersisNumberTextEdit.Text,
                WebsiteTextEdit.Text,
                [.. _addresses.Select(address => new CreateCompanyAddressRequest(
                    address.CityId,
                    address.DistrictId,
                    address.AddressTypeId,
                    address.Address,
                    address.PostalCode,
                    address.IsDefault))],
                [.. _contacts.Select(contact => new CreateCompanyContactRequest(
                    contact.ContactTypeId,
                    contact.FirstName,
                    contact.LastName,
                    contact.Title,
                    contact.Phone,
                    contact.MobilePhone,
                    contact.Email,
                    contact.IsAuthorized,
                    contact.IsDefault))]);

            IDataResult<Guid> createResult = await _companyApiClient.CreateAsync(createRequest);

            if (!createResult.Success)
            {
                MessageBox.Show(
                    createResult.Message ,
                    "Firma" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            IDataResult<CompanyModel> createdCompanyResult = await _companyApiClient.GetByIdAsync(createResult.Data);

            if (!createdCompanyResult.Success)
            {
                MessageBox.Show(
                    createdCompanyResult.Message ,
                    "Firma" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            _companyState.Add(createdCompanyResult.Data);

            MessageBox.Show(
                createResult.Message ,
                "Firma Kaydet" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Firma Kaydet" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    #endregion Events

    #region Methods

    public async Task LoadCompanyAsync(Guid companyId)
    {
        try
        {
            if (_taxOffices.Count == 0)
            {
                await LoadDataAsync();
            }

            IDataResult<CompanyModel> result = await _companyApiClient.GetByIdAsync(companyId);

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message ,
                    "Firma" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            CompanyModel company = result.Data;

            _companyId = company.Id;
            CompanyNameTextEdit.Text = company.Name;
            ShortNameTextEdit.Text = company.ShortName;
            TaxNumberTextEdit.Text = company.TaxNumber;
            TradeRegistryNumberTextEdit.Text = company.TradeRegistryNumber;
            MersisNumberTextEdit.Text = company.MersisNumber;
            WebsiteTextEdit.Text = company.Website;

            MasterDefinitionResponse? taxOffice = _taxOffices.FirstOrDefault(
                item => item.Id == company.TaxOfficeId);

            TaxOfficeComboBox.SelectedItem = taxOffice;
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Firma" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    public void PrepareForNew()
    {
        _companyId = null;

        CompanyNameTextEdit.Clear();
        ShortNameTextEdit.Clear();
        TaxNumberTextEdit.Clear();
        TradeRegistryNumberTextEdit.Clear();
        MersisNumberTextEdit.Clear();
        WebsiteTextEdit.Clear();

        TaxOfficeComboBox.SelectedItem = null;

        _addresses.Clear();
        _contacts.Clear();

        AddressesGridControl.RefreshData();
        ContactsGridControl.RefreshData();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _taxOffices = await _masterApiClient.GetTaxOfficesAsync();

            TaxOfficeComboBox.ItemsSource = _taxOffices;
            TaxOfficeComboBox.DisplayMember = nameof(MasterDefinitionResponse.Name);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Firma Bilgileri" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private void ClearDefaultAddress(Guid? exceptId = null)
    {
        foreach (CompanyAddressItem address in _addresses)
        {
            if (exceptId is not null && address.Id == exceptId.Value)
            {
                continue;
            }

            address.IsDefault = false;
        }

        AddressesGridControl.RefreshData();
    }

    private void ClearDefaultContact(Guid? exceptId = null)
    {
        foreach (CompanyContactItem contact in _contacts)
        {
            if (exceptId is not null && contact.Id == exceptId.Value)
            {
                continue;
            }

            contact.IsDefault = false;
        }

        ContactsGridControl.RefreshData();
    }

    #endregion Methods
}