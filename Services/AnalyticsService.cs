namespace SortMasterCLI.Services;

public class AnalyticsService {
    private readonly Dictionary<string, int> _data = new();

    public void Increment(string category) {
        if (_data.ContainsKey(category))
            _data[category]++;
        else
            _data[category] = 1;
    }

    public Dictionary<string, int> GetData() {
        return _data;
    }
}