using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using System.Windows;
using Wpf.State;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Wpf.Companies;

public partial class CompanyListForm : UserControl, ITransientService
{
    #region Fields

    private readonly ICompanyApiClient _companyApiClient;
    private readonly CompanyState _companyState;

    #endregion Fields

    #region Events

    public event EventHandler? NewCompanyRequested;
    public event EventHandler<CompanyModel>? EditCompanyRequested;
    public event EventHandler<CompanyModel>? PeriodsRequested;
    public event EventHandler<CompanyModel>? PartnersRequested;
    public event EventHandler<CompanyModel>? AddressesRequested;
    public event EventHandler<CompanyModel>? ContactsRequested;

    #endregion Events

    #region Constructors

    public CompanyListForm(ICompanyApiClient companyApiClient , CompanyState companyState)
    {
        ArgumentNullException.ThrowIfNull(companyApiClient);
        ArgumentNullException.ThrowIfNull(companyState);

        _companyApiClient = companyApiClient;
        _companyState = companyState;

        InitializeComponent();

        CompaniesGridControl.ItemsSource = _companyState.Companies;

        Loaded += CompanyListForm_Loaded;
    }

    #endregion Constructors

    #region Events

    private async void CompanyListForm_Loaded(object sender , RoutedEventArgs e)
    {
        await LoadDataAsync();
    }

    private void NewButton_Click(object sender , RoutedEventArgs e)
    {
        NewCompanyRequested?.Invoke(this , EventArgs.Empty);
    }

    private void EditButton_Click(object sender , RoutedEventArgs e)
    {
        if (CompaniesGridControl.SelectedItem is not CompanyModel selectedCompany)
        {
            MessageBox.Show(
                "Lütfen düzenlemek istediğiniz firmayı seçin." ,
                "Firma" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        EditCompanyRequested?.Invoke(this , selectedCompany);
    }

    private void PeriodsButton_Click(object sender , RoutedEventArgs e)
    {
        if (CompaniesGridControl.SelectedItem is not CompanyModel selectedCompany)
        {
            MessageBox.Show(
                "Lütfen dönemlerini görüntülemek istediğiniz firmayı seçin." ,
                "Firma Dönemleri" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        PeriodsRequested?.Invoke(this , selectedCompany);
    }

    private void PartnersButton_Click(object sender , RoutedEventArgs e)
    {
        if (CompaniesGridControl.SelectedItem is not CompanyModel selectedCompany)
        {
            MessageBox.Show(
                "Lütfen ortaklarını görüntülemek istediğiniz firmayı seçin." ,
                "Firma Ortakları" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        PartnersRequested?.Invoke(this , selectedCompany);
    }

    private void AddressesButton_Click(object sender , RoutedEventArgs e)
    {
        if (CompaniesGridControl.SelectedItem is not CompanyModel selectedCompany)
        {
            MessageBox.Show(
                "Lütfen adreslerini görüntülemek istediğiniz firmayı seçin." ,
                "Firma Adresleri" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        AddressesRequested?.Invoke(this , selectedCompany);
    }

    private void ContactsButton_Click(object sender , RoutedEventArgs e)
    {
        if (CompaniesGridControl.SelectedItem is not CompanyModel selectedCompany)
        {
            MessageBox.Show(
                "Lütfen iletişim bilgilerini görüntülemek istediğiniz firmayı seçin." ,
                "Firma İletişim Bilgileri" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        ContactsRequested?.Invoke(this , selectedCompany);
    }

    private async void DeleteButton_Click(object sender , RoutedEventArgs e)
    {
        if (CompaniesGridControl.SelectedItem is not CompanyModel selectedCompany)
        {
            MessageBox.Show(
                "Lütfen silmek istediğiniz firmayı seçin." ,
                "Firma" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        MessageBoxResult result = MessageBox.Show(
            $"'{selectedCompany.Name}' firması silinecek. Devam etmek istiyor musunuz?" ,
            "Firma Sil" ,
            MessageBoxButton.YesNo ,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        await DeleteCompanyAsync(selectedCompany);
    }

    #endregion Events

    #region Methods

    private async Task LoadDataAsync()
    {
        try
        {
            IDataResult<List<CompanyModel>> result = await _companyApiClient.GetAllAsync();

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message ,
                    "Firma Listesi" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            _companyState.SetCompanies(result.Data);
            CompaniesGridControl.RefreshData();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Firma Listesi" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private async Task DeleteCompanyAsync(CompanyModel company)
    {
        try
        {
            IResult result = await _companyApiClient.DeleteAsync(company.Id);

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message ,
                    "Firma Sil" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            await LoadDataAsync();

            MessageBox.Show(
                result.Message ,
                "Firma Sil" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Firma Sil" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    #endregion Methods
}