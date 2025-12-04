namespace AccessControl.Infraestructure.Dto
{
    public class AccessRecordDisplayDto
    {
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string DocumentoIdentidad { get; set; } = string.Empty;
        public string HoraSalida { get; set; } = string.Empty;
        public int DuracionMinutos { get; set; }
    }
}
