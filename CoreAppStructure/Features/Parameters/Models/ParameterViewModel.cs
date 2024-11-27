namespace CoreAppStructure.Features.Parameters.Models
{
    public class ParameterViewModel
    {
        public string  ParaScope           { get; set; }
        public string  ParaName            { get; set; }
        public string? ParaShortValue      { get; set; }
        public string? ParaLobValue        { get; set; }
        public string? ParaDesc            { get; set; }
        public string  ParaType            { get; set; }
        public string? UserAccessibleFlag  { get; set; } // Y or N
        public string? AdminAccessibleFlag { get; set; } // Y or N
        public string? SystemId            { get; set; }
    }
}
