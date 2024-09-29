using Microsoft.Extensions.Localization;

namespace Localization.Resources;

public interface ISharedResource
{
    string GetTranslation(string key);


    string CheckTheInputValues { get; }
    string Exception { get; }
}

public class SharedResource : ISharedResource
{
    private readonly IStringLocalizer _localizer;

    public SharedResource(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public string GetTranslation(string key) => _localizer[key];
    public string CheckTheInputValues => GetTranslation("Check_The_Input_Values");
    public string Exception => GetTranslation("Exception");
}
