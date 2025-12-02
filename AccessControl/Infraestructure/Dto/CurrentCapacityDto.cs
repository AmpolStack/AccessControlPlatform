namespace AccessControl.Infraestructure.Dto
{
    public class CurrentCapacityDto
    {
        public int CurrentCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public string EstablishmentName { get; set; } = string.Empty;
    }
}
