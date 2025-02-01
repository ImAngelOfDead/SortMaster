namespace SortMasterCLI.Models;

public class Rule {

    public List<string> Extensions { get; set; }


    public string Destination { get; set; }


    public long? MinFileSize { get; set; }


    public long? MaxFileSize { get; set; }


    public DateTime? CreatedAfter { get; set; }


    public DateTime? CreatedBefore { get; set; }


    public string NamePattern { get; set; }
}