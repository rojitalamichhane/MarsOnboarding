namespace qa_dotnet_cucumber.Config
{
    public class TestSettings
    {
        public BrowserSettings? Browser { get; set; }
        public ReportSettings? Report { get; set; }
        public EnvironmentSettings? Environment { get; set; }
    }

    public class BrowserSettings
    {
        public string Type { get; set; } = string.Empty;
        public bool Headless { get; set; } = false;
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class ReportSettings
    {
        public string Path { get; set; } = string.Empty;
        public string ? Title { get; set; }
    }

    public class EnvironmentSettings
    {
        public string ?BaseUrl { get; set; }
    }
}