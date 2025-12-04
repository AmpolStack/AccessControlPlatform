using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using AccessControl.Infraestructure.Dto;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AccessControl.Views.ViewModels
{
    public class ReportsPageViewModel : ObservableObject
    {
        private readonly IAccessRecordUseCase _accessRecordService;
        private readonly IEstablishmentUseCase _establishmentService;

        private int _establishmentId;
        private string _establishmentName = string.Empty;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _isLoading;
        private string _searchText = string.Empty;
        private ObservableCollection<UserAccessHistoryDto> _records = new();
        private ObservableCollection<UserAccessHistoryDto> _allRecords = new();
        private int _totalRecords;

        public AsyncRelayCommand LoadDataCommand { get; }
        public AsyncRelayCommand GenerateReportCommand { get; }
        public RelayCommand ClearSearchCommand { get; }

        public ReportsPageViewModel(
            IAccessRecordUseCase accessRecordService,
            IEstablishmentUseCase establishmentService)
        {
            _accessRecordService = accessRecordService;
            _establishmentService = establishmentService;

            // Fechas por defecto: último mes
            _endDate = DateTime.Now;
            _startDate = _endDate.AddDays(-30);

            LoadDataCommand = new AsyncRelayCommand(
                execute: async _ => await LoadReportDataAsync(),
                canExecute: _ => !IsLoading
            );

            GenerateReportCommand = new AsyncRelayCommand(
                execute: async _ => await GenerateReportAsync(),
                canExecute: _ => !IsLoading && Records.Any()
            );

            ClearSearchCommand = new RelayCommand(
                execute: _ => SearchText = string.Empty
            );
        }

        #region Properties

        public string EstablishmentName
        {
            get => _establishmentName;
            set => SetProperty(ref _establishmentName, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                    OnPropertyChanged(nameof(DateRangeText));
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                    OnPropertyChanged(nameof(DateRangeText));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    LoadDataCommand.RaiseCanExecuteChanged();
                    GenerateReportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<UserAccessHistoryDto> Records
        {
            get => _records;
            set
            {
                if (SetProperty(ref _records, value))
                {
                    OnPropertyChanged(nameof(RecordsCountText));
                    OnPropertyChanged(nameof(FilteredCountText));
                    GenerateReportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public int TotalRecords
        {
            get => _totalRecords;
            set
            {
                if (SetProperty(ref _totalRecords, value))
                {
                    OnPropertyChanged(nameof(RecordsCountText));
                    OnPropertyChanged(nameof(FilteredCountText));
                }
            }
        }

        public string DateRangeText => $"Período: {StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";

        public string RecordsCountText => $"Total de registros: {TotalRecords}";

        public string FilteredCountText => string.IsNullOrWhiteSpace(SearchText) 
            ? string.Empty 
            : $"Mostrando: {Records.Count} de {TotalRecords}";

        #endregion

        #region Public Methods

        public void Initialize(int establishmentId, string establishmentName)
        {
            _establishmentId = establishmentId;
            EstablishmentName = establishmentName;
        }

        #endregion

        #region Private Methods

        private async Task LoadReportDataAsync()
        {
            try
            {
                IsLoading = true;

                var (success, message, data) = await _accessRecordService.GetAccessHistoryAsync(
                    _establishmentId,
                    StartDate,
                    EndDate
                );

                if (success && data != null)
                {
                    _allRecords = new ObservableCollection<UserAccessHistoryDto>(data.ToList());
                    TotalRecords = _allRecords.Count;
                    ApplyFilter();
                }
                else
                {
                    _allRecords.Clear();
                    Records.Clear();
                    TotalRecords = 0;
                    System.Diagnostics.Debug.WriteLine($"Error al cargar datos: {message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos del reporte: {ex.Message}");
                _allRecords.Clear();
                Records.Clear();
                TotalRecords = 0;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Sin filtro, mostrar todos los registros
                Records = new ObservableCollection<UserAccessHistoryDto>(_allRecords);
            }
            else
            {
                // Filtrar por nombre (no distingue mayúsculas/minúsculas)
                var filtered = _allRecords.Where(r => 
                    r.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    r.IdentityDocument.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                Records = new ObservableCollection<UserAccessHistoryDto>(filtered);
            }
        }

        private async Task GenerateReportAsync()
        {
            try
            {
                IsLoading = true;

                // TODO: Implementar generación de reporte RDLC
                System.Diagnostics.Debug.WriteLine($"Generando reporte para {EstablishmentName}");
                System.Diagnostics.Debug.WriteLine($"Registros: {TotalRecords}");
                
                await Task.Delay(1000); // Simular generación

                System.Diagnostics.Debug.WriteLine("Reporte generado correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al generar reporte: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
