namespace AdBlock.Classes;

public class AdBlockFilter
{
    public bool IsAdBlockResource(string url)
    {
        return url.Contains("adserver.com/banner");
    }
}