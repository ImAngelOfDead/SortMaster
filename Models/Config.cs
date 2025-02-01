namespace SortMasterCLI.Models;

public class Config {
  
    public string SourceDirectory { get; set; }


    public bool DryRun { get; set; } = true;


    public bool Recursive { get; set; } = false;


    public bool Interactive { get; set; } = false;


    public List<Rule> Rules { get; set; }


    public string LogFilePath { get; set; } = "SortMaster.log";


    public string ProfileName { get; set; } = "default";
}