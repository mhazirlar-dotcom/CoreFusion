using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using System.Windows;
using Wpf.Companies.Models;

namespace Wpf.Companies;

public partial class CompanyAddressForm : Window, ITransientService
{
    #region Fields

    private readonly IMasterApiClient _masterApiClient;
    private readonly CompanyAddressItem? _address;
    private List<MasterDefinitionResponse> _addressTypes = [];
    private List<CityResponse> _cities = [];
    private List<DistrictResponse> _districts = [];
    private bool _isLoading;

    #endregion Fields

    #region Constructors

    public CompanyAddressForm(IMasterApiClient masterApiClient)
    {
        ArgumentNullException.ThrowIfNull(masterApiClient);

        _masterApiClient = masterApiClient;

        InitializeComponent();

        Loaded += CompanyAddressForm_Loaded;
    }

    public CompanyAddressForm(IMasterApiClient masterApiClient , CompanyAddressItem address) : this(masterApiClient)
    {
        ArgumentNullException.ThrowIfNull(address);

        _address = address;
    }

    #endregion Constructors

    #region Properties

    public CompanyAddressItem? Result { get; private set; }

    #endregion Properties

    #region Events

    private async void CompanyAddressForm_Loaded(object sender , RoutedEventArgs e)
    {
        await LoadDataAsync();
    }

    private async void CityComboBox_EditValueChanged(object sender , DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
    {
        if (_isLoading)
        {
            return;
        }

        if (CityComboBox.EditValue is not CityResponse city)
        {
            DistrictComboBox.ItemsSource = null;
            _districts = [];
            return;
        }

        try
        {
            await LoadDistrictsAsync(city.Id);
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(
                exception.Message ,
                "İlçe Bilgileri" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private void OkButton_Click(object sender , RoutedEventArgs e)
    {
        if (AddressTypeComboBox.EditValue is not MasterDefinitionResponse addressType)
        {
            System.Windows.MessageBox.Show(
                "Lütfen adres tipini seçin." ,
                "Adres" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        if (CityComboBox.EditValue is not CityResponse city)
        {
            System.Windows.MessageBox.Show(
                "Lütfen il seçin." ,
                "Adres" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        if (DistrictComboBox.EditValue is not DistrictResponse district)
        {
            System.Windows.MessageBox.Show(
                "Lütfen ilçe seçin." ,
                "Adres" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        CompanyAddressItem address = _address ?? new CompanyAddressItem
        {
            Id = Guid.NewGuid()
        };

        address.AddressTypeId = addressType.Id;
        address.CityId = city.Id;
        address.DistrictId = district.Id;
        address.AddressType = addressType.Name;
        address.City = city.Name;
        address.District = district.Name;
        address.Address = AddressTextEdit.Text;
        address.PostalCode = PostalCodeTextEdit.Text;
        address.IsDefault = DefaultCheckEdit.IsChecked == true;

        Result = address;
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
        _isLoading = true;

        try
        {
            _addressTypes = await _masterApiClient.GetAddressTypesAsync();
            _cities = await _masterApiClient.GetCitiesAsync();

            AddressTypeComboBox.ItemsSource = _addressTypes;
            CityComboBox.ItemsSource = _cities;

            AddressTypeComboBox.DisplayMember = nameof(MasterDefinitionResponse.Name);
            CityComboBox.DisplayMember = nameof(CityResponse.Name);

            if (_address is null)
            {
                return;
            }

            AddressTypeComboBox.EditValue = _address.AddressTypeId;
            CityComboBox.EditValue = _address.CityId;

            await LoadDistrictsAsync(_address.CityId);

            DistrictComboBox.EditValue = _address.DistrictId;

            AddressTextEdit.EditValue = _address.Address;
            PostalCodeTextEdit.EditValue = _address.PostalCode;
            DefaultCheckEdit.IsChecked = _address.IsDefault;
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(
                exception.Message ,
                "Adres Bilgileri" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task LoadDistrictsAsync(Guid cityId)
    {
        _districts = await _masterApiClient.GetDistrictsAsync(cityId);

        DistrictComboBox.ItemsSource = _districts;
        DistrictComboBox.DisplayMember = nameof(DistrictResponse.Name);
        DistrictComboBox.EditValue = null;
    }

    #endregion Methods
}