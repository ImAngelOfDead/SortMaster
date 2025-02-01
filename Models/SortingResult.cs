namespace SortMasterCLI.Models;

public class SortingResult {
    public string FilePath { get; set; }
    public string Action { get; set; }
    public string Destination { get; set; }
    public bool Success { get; set; }

    public string ErrorMessage { get; set; }
    
    public string RuleApplied { get; set; }
}