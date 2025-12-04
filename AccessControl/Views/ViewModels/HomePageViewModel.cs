using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using AccessControl.Infraestructure.Dto;
using System.Collections.ObjectModel;

namespace AccessControl.Views.ViewModels
{
    public class HomePageViewModel : ObservableObject
    {
        private readonly IAccessRecordUseCase _accessRecordService;

        private string _userName = string.Empty;
        private string _lastAccessTime = string.Empty;
        private int _establishmentId;

        private ObservableCollection<AccessRecordDisplayDto> _accessRecords = [];

        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords;
        private int _totalPages;
        private bool _isLoading;
        private bool _hasNextPage;
        private bool _hasPreviousPage;

        private DateTime _startDate;
        private DateTime _endDate;

        private List<UserAccessHistoryDto> _allRecords = [];


        public AsyncRelayCommand LoadDataCommand { get; }
        public AsyncRelayCommand NextPageCommand { get; }
        public AsyncRelayCommand PreviousPageCommand { get; }
        public AsyncRelayCommand RefreshDataCommand { get; }

        public HomePageViewModel(IAccessRecordUseCase accessRecordUseCase)
        {
            _accessRecordService = accessRecordUseCase;

            _endDate = DateTime.Now;
            _startDate = _endDate.AddDays(-30);

            LoadDataCommand = new AsyncRelayCommand(
                execute: async _ => await LoadHomePageDataAsync(),
                canExecute: _ => !IsLoading
            );

            NextPageCommand = new AsyncRelayCommand(
                execute: async _ => await GoToNextPageAsync(),
                canExecute: _ => HasNextPage && !IsLoading
            );

            PreviousPageCommand = new AsyncRelayCommand(
                execute: async _ => await GoToPreviousPageAsync(),
                canExecute: _ => HasPreviousPage && !IsLoading
            );

            RefreshDataCommand = new AsyncRelayCommand(
                execute: async _ => await RefreshDataAsync(),
                canExecute: _ => !IsLoading
            );
        }

        #region Properties

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string LastAccessTime
        {
            get => _lastAccessTime;
            set => SetProperty(ref _lastAccessTime, value);
        }

        public ObservableCollection<AccessRecordDisplayDto> AccessRecords
        {
            get => _accessRecords;
            set => SetProperty(ref _accessRecords, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    UpdatePaginationState();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public int TotalRecords
        {
            get => _totalRecords;
            set => SetProperty(ref _totalRecords, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    LoadDataCommand.RaiseCanExecuteChanged();
                    NextPageCommand.RaiseCanExecuteChanged();
                    PreviousPageCommand.RaiseCanExecuteChanged();
                    RefreshDataCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasNextPage
        {
            get => _hasNextPage;
            set
            {
                if (SetProperty(ref _hasNextPage, value))
                    NextPageCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasPreviousPage
        {
            get => _hasPreviousPage;
            set
            {
                if (SetProperty(ref _hasPreviousPage, value))
                    PreviousPageCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public string PaginationInfo => $"Página {CurrentPage} de {TotalPages} - Total: {TotalRecords} registros";

        #endregion

        #region Private Methods

        private void UpdatePaginationState()
        {
            HasNextPage = CurrentPage < TotalPages;
            HasPreviousPage = CurrentPage > 1;
            OnPropertyChanged(nameof(PaginationInfo));
        }

        private void LoadPageData()
        {
            var skip = (CurrentPage - 1) * PageSize;
            var pageRecords = _allRecords
                .Skip(skip)
                .Take(PageSize)
                .Select(record => new AccessRecordDisplayDto
                {
                    Fecha = record.EntryDateTime.ToString("dd/MM/yyyy"),
                    Hora = record.EntryDateTime.ToString("HH:mm:ss"),
                    NombreCompleto = record.FullName,
                    DocumentoIdentidad = record.IdentityDocument,
                    HoraSalida = record.ExitDateTime?.ToString("HH:mm:ss") ?? "En curso",
                    DuracionMinutos = record.DurationMinutes
                })
                .ToList();

            AccessRecords.Clear();
            foreach (var record in pageRecords)
            {
                AccessRecords.Add(record);
            }

            UpdatePaginationState();
        }

        private void CalculateLastAccessTime()
        {
            if (_allRecords.Count != 0)
            {
                var lastEntry = _allRecords
                    .OrderByDescending(r => r.EntryDateTime)
                    .FirstOrDefault();

                if (lastEntry != null)
                {
                    var timeSpan = DateTime.Now - lastEntry.EntryDateTime;

                    if (timeSpan.TotalMinutes < 60)
                    {
                        LastAccessTime = $"hace {(int)timeSpan.TotalMinutes} minutos";
                    }
                    else if (timeSpan.TotalHours < 24)
                    {
                        LastAccessTime = $"hace {(int)timeSpan.TotalHours} horas";
                    }
                    else
                    {
                        LastAccessTime = $"hace {(int)timeSpan.TotalDays} días";
                    }
                }
            }
        }

        #endregion

        #region Command Methods

        private async Task GoToNextPageAsync()
        {
            if (HasNextPage)
            {
                CurrentPage++;
                LoadPageData();
            }

            await Task.CompletedTask;
        }

        private async Task GoToPreviousPageAsync()
        {
            if (HasPreviousPage)
            {
                CurrentPage--;
                LoadPageData();
            }

            await Task.CompletedTask;
        }

        private async Task RefreshDataAsync()
        {
            CurrentPage = 1;
            await LoadHomePageDataAsync();
        }

        #endregion

        #region Public Methods

        public void Initialize(string userName, int establishmentId)
        {
            UserName = userName;
            _establishmentId = establishmentId;
        }

        public async Task LoadHomePageDataAsync()
        {
            try
            {
                IsLoading = true;

                var (historySuccess, historyMessage, historyData) =
                    await _accessRecordService.GetAccessHistoryAsync(
                        _establishmentId,
                        StartDate,
                        EndDate);

                if (historySuccess && historyData != null)
                {
                    _allRecords = [.. historyData];
                    TotalRecords = _allRecords.Count;
                    TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);

                    CurrentPage = 1;
                    LoadPageData();

                    CalculateLastAccessTime();
                }
                else
                {
                    AccessRecords.Clear();
                    TotalRecords = 0;
                    TotalPages = 0;
                    CurrentPage = 1;
                    UpdatePaginationState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading home page: {ex.Message}");
                AccessRecords.Clear();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ChangeDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            await LoadHomePageDataAsync();
        }

        #endregion
    }
}
