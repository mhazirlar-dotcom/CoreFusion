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
        if (_companyState.Companies.Count == 0)
        {
            await LoadDataAsync();
        }
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

    private async void RefreshButton_Click(object sender , RoutedEventArgs e)
    {
        await LoadDataAsync();
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

            _companyState.Remove(company.Id);

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