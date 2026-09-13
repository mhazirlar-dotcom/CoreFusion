using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Wpf.Companies;

public partial class CompanyPeriodForm : UserControl, ITransientService
{
    #region Fields

    private readonly ICompanyPeriodApiClient _companyPeriodApiClient;
    private readonly ObservableCollection<CompanyPeriodModel> _periods = [];
    private Guid? _companyId;
    private Guid? _editingPeriodId;
    private string _companyName = string.Empty;

    #endregion Fields

    #region Events

    public event EventHandler? CompanyListRequested;
    public event EventHandler? PeriodsChanged;

    #endregion Events

    #region Constructors

    public CompanyPeriodForm(ICompanyPeriodApiClient companyPeriodApiClient)
    {
        ArgumentNullException.ThrowIfNull(companyPeriodApiClient);

        _companyPeriodApiClient = companyPeriodApiClient;

        InitializeComponent();

        PeriodsGridControl.ItemsSource = _periods;
    }

    #endregion Constructors

    #region Events

    private void NewButton_Click(object sender , RoutedEventArgs e)
    {
        if (_companyId is null)
        {
            MessageBox.Show(
                "Firma seçilmedi." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        _editingPeriodId = null;

        DateTime today = DateTime.Today;

        DateTime yearEnd = new(
            today.Year,
            12,
            31);

        StartDateEdit.EditValue = today;
        EndDateEdit.EditValue = yearEnd;

        EditPanel.Visibility = Visibility.Visible;
    }

    private void EditButton_Click(object sender , RoutedEventArgs e)
    {
        if (PeriodsGridControl.CurrentItem is not CompanyPeriodModel selectedPeriod)
        {
            MessageBox.Show(
                "Lütfen düzenlemek istediğiniz dönemi seçin." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        _editingPeriodId = selectedPeriod.Id;

        StartDateEdit.EditValue =
            selectedPeriod.StartDate.ToDateTime(TimeOnly.MinValue);

        EndDateEdit.EditValue =
            selectedPeriod.EndDate.ToDateTime(TimeOnly.MinValue);

        EditPanel.Visibility = Visibility.Visible;
    }

    private async void DeleteButton_Click(object sender , RoutedEventArgs e)
    {
        if (_companyId is null)
        {
            MessageBox.Show(
                "Firma seçilmedi." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        if (PeriodsGridControl.CurrentItem is not CompanyPeriodModel selectedPeriod)
        {
            MessageBox.Show(
                "Lütfen silmek istediğiniz dönemi seçin." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);

            return;
        }

        MessageBoxResult result = MessageBox.Show(
            $"{_companyName}\n\n" +
            $"{selectedPeriod.StartDate:dd.MM.yyyy} - {selectedPeriod.EndDate:dd.MM.yyyy} dönemi silinecek.\n\n" +
            "Devam etmek istiyor musunuz?",
            "Dönem Sil",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            IResult deleteResult = await _companyPeriodApiClient.DeleteAsync(
                _companyId.Value,
                selectedPeriod.Id);

            if (!deleteResult.Success)
            {
                MessageBox.Show(
                    deleteResult.Message ,
                    "Dönem Sil" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            await LoadPeriodsAsync();

            PeriodsChanged?.Invoke(this , EventArgs.Empty);

            MessageBox.Show(
                deleteResult.Message ,
                "Dönem Sil" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Dönem Sil" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private async void SaveButton_Click(object sender , RoutedEventArgs e)
    {
        if (_companyId is null)
        {
            MessageBox.Show(
                "Firma seçilmedi." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        if (StartDateEdit.EditValue is not DateTime startDate ||
            EndDateEdit.EditValue is not DateTime endDate)
        {
            MessageBox.Show(
                "Başlangıç ve bitiş tarihlerini girin." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        DateOnly startDateOnly = DateOnly.FromDateTime(startDate);
        DateOnly endDateOnly = DateOnly.FromDateTime(endDate);
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        if (startDateOnly > endDateOnly)
        {
            MessageBox.Show(
                "Başlangıç tarihi, bitiş tarihinden büyük olamaz." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        if (_editingPeriodId is null &&
            startDateOnly.Year > today.Year)
        {
            MessageBox.Show(
                $"Yeni dönem yalnızca {today.Year} yılı veya daha önceki yıllar için oluşturulabilir.\n\n" +
                $"{startDateOnly:dd.MM.yyyy} tarihi ile yeni dönem oluşturulamaz." ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            if (_editingPeriodId is null)
            {
                CreateCompanyPeriodRequest request = new(
                    _companyId.Value,
                    startDateOnly,
                    endDateOnly);

                IDataResult<Guid> result =
                    await _companyPeriodApiClient.CreateAsync(request);

                if (!result.Success)
                {
                    MessageBox.Show(
                        result.Message ,
                        "Dönem Kaydet" ,
                        MessageBoxButton.OK ,
                        MessageBoxImage.Warning);

                    return;
                }

                HideEditPanel();

                await LoadPeriodsAsync();

                PeriodsChanged?.Invoke(this , EventArgs.Empty);

                MessageBox.Show(
                    result.Message ,
                    "Dönem Kaydet" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Information);

                return;
            }

            UpdateCompanyPeriodRequest updateRequest = new(
                _editingPeriodId.Value,
                startDateOnly,
                endDateOnly,
                true);

            IResult updateResult =
                await _companyPeriodApiClient.UpdateAsync(
                    _companyId.Value,
                    updateRequest);

            if (!updateResult.Success)
            {
                MessageBox.Show(
                    updateResult.Message ,
                    "Dönem Güncelle" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            HideEditPanel();

            await LoadPeriodsAsync();

            PeriodsChanged?.Invoke(this , EventArgs.Empty);

            MessageBox.Show(
                updateResult.Message ,
                "Dönem Güncelle" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Dönem" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender , RoutedEventArgs e)
    {
        HideEditPanel();
    }

    private void CompanyListButton_Click(object sender , RoutedEventArgs e)
    {
        CompanyListRequested?.Invoke(this , EventArgs.Empty);
    }

    #endregion Events

    #region Methods

    public async Task LoadCompanyAsync(Guid companyId , string companyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(companyName);

        _companyId = companyId;
        _companyName = companyName;
        _editingPeriodId = null;

        CompanyNameTextBlock.Text = $"{_companyName} - Firma Dönemleri";

        HideEditPanel();

        await LoadPeriodsAsync();
    }

    private async Task LoadPeriodsAsync()
    {
        if (_companyId is null)
        {
            return;
        }

        try
        {
            IDataResult<List<CompanyPeriodModel>> result =
                await _companyPeriodApiClient.GetByCompanyIdAsync(
                    _companyId.Value);

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message ,
                    "Dönemler" ,
                    MessageBoxButton.OK ,
                    MessageBoxImage.Warning);

                return;
            }

            _periods.Clear();

            foreach (CompanyPeriodModel period in result.Data.OrderBy(item => item.StartDate))
            {
                _periods.Add(period);
            }

            PeriodsGridControl.RefreshData();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Dönemler" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private void HideEditPanel()
    {
        _editingPeriodId = null;
        EditPanel.Visibility = Visibility.Collapsed;
    }

    #endregion Methods
}