using System.IO;
using CollectiveMind.TicTac3D.Runtime;
using UnityEditor;
using UnityEditor.Build.Profile;

namespace CollectiveMind.TicTac3D.Editor
{
  public class Builder
  {
    [MenuItem(MIC.BUILD_MENU + "📦 Build All")]
    public static void BuildAll()
    {
      BuildWeb();
      BuildWindows();
    }

    [MenuItem(MIC.BUILD_MENU + "🌐 Web Build")]
    public static void BuildWeb()
    {
      string projectName = PlayerSettings.productName;
      var path = $"../.artifacts/Web/{projectName}";
      if (Directory.Exists(path))
        Directory.Delete(path, true);

      BuildProfile profile = AssetFinder.FindAsset<BuildProfile>(nameof(BuildProfile), "Host.Web");
      BuildPipeline.BuildPlayer(new BuildPlayerWithProfileOptions
      {
        buildProfile = profile,
        locationPathName = path
      });
    }

    [MenuItem(MIC.BUILD_MENU + "🖥 Windows Build")]
    public static void BuildWindows()
    {
      string projectName = PlayerSettings.productName;
      var path = $"../.artifacts/Windows/{projectName}";
      if (Directory.Exists(path))
        Directory.Delete(path, true);

      BuildProfile profile = AssetFinder.FindAsset<BuildProfile>(nameof(BuildProfile), "Host.Win");
      BuildPipeline.BuildPlayer(new BuildPlayerWithProfileOptions
      {
        buildProfile = profile,
        locationPathName = $"{path}/{projectName}.exe"
      });
    }
  }
}