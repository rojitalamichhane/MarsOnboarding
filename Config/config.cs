namespace qa_dotnet_cucumber.Config
{
    public class TestSettings
{
    public BrowserSettings Browser { get; set; } = new BrowserSettings();
    public ReportSettings Report { get; set; } = new ReportSettings();
    public EnvironmentSettings Environment { get; set; } = new EnvironmentSettings();
}


    public class BrowserSettings
    {
        public string? Type { get; set; }
        public bool Headless { get; set; }
        public int TimeoutSeconds { get; set; }
    }

    public class ReportSettings
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class EnvironmentSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

}