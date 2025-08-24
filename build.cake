using Newtonsoft.Json;
using System.Text.Json;

#addin nuget:?package=Cake.Unity&version=0.9.0
#addin nuget:?package=Cake.FileHelpers&version=7.0.0
#addin nuget:?package=Newtonsoft.Json&version=13.0.3

var target = Argument("target", "Build-Web");
var config = ReadConfig();

Task("Clean-Windows-Artifacts")
    .Does(() =>
{
    CleanDirectory($"./.artifacts/Windows");
});

Task("Build-Windows")
    .IsDependentOn("Clean-Windows-Artifacts")
    .Does(() =>
{
    UnityEditorInternal(config.EditorPath,
        new UnityEditorArguments
        {
            ExecuteMethod = "CollectiveMind.TicTac3D.Editor.Builder.BuildWindows",
            BuildTarget = BuildTarget.Win64,
            LogFile = "./.artifacts/Windows/unity.log"
        },
        new UnityEditorSettings
        {
            RealTimeLog = true,
        }
    );
});

Task("Clean-Web-Artifacts")
    .IsDependentOn("Build-Windows")
    .Does(() =>
{
    CleanDirectory($"./.artifacts/Web");
});

Task("Build-Web")
    .IsDependentOn("Clean-Web-Artifacts")
    .Does(() =>
{
    UnityEditorInternal(config.EditorPath,
        new UnityEditorArguments
        {
            ExecuteMethod = "CollectiveMind.TicTac3D.Editor.Builder.BuildWeb",
            BuildTarget = BuildTarget.WebGL,
            LogFile = "./.artifacts/Web/unity.log"
        },
        new UnityEditorSettings
        {
            RealTimeLog = true
        }
    );
});

void UnityEditorInternal(string path, UnityEditorArguments arguments, UnityEditorSettings settings)
{
    if(string.IsNullOrWhiteSpace(path))
        UnityEditor(arguments, settings);
    else
        UnityEditor(path, arguments, settings);
}


CakeConfig ReadConfig()
{
    JsonConvert.DefaultSettings = () => new JsonSerializerSettings 
    {
        Formatting = Formatting.Indented
    };

    string configPath = "./cakeConfig.json";
    if(FileExists(configPath))
    {
        var json = FileReadText(configPath);
        return JsonConvert.DeserializeObject<CakeConfig>(json);
    }
    else
    {
        var json = JsonConvert.SerializeObject(new CakeConfig());
        FileWriteText(configPath, json);
    }

    return new CakeConfig();
}

class CakeConfig
{
    public string EditorPath = "";
}

RunTarget(target);