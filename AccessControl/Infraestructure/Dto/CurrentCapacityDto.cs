namespace AccessControl.Infraestructure.Dto
{
    /// <summary>
    /// DTO for Current Capacity data returned by sp_GetCurrentCapacity
    /// </summary>
    public class CurrentCapacityDto
    {
        public int CurrentCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public string EstablishmentName { get; set; } = string.Empty;
    }
}
