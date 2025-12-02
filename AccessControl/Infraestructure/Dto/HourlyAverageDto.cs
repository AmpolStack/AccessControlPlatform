namespace AccessControl.Infraestructure.Dto
{
    public class HourlyAverageDto
    {
        public int Hour { get; set; }
        public int Entries { get; set; }
        public decimal AverageEntriesPerDay { get; set; }
    }
}
