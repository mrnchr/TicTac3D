namespace CollectiveMind.TicTac3D.Runtime.Shared
{
  /// <summary>
  /// Create asset constants
  /// </summary>
  public static class CAC
  {
    public const string PROJECT_MENU = "TicTac3D/";
    public const string CONFIG_MENU = PROJECT_MENU + "Configs/";

    public static class Names
    {
      public const string CONFIG_PROVIDER_MENU = CONFIG_MENU + "ConfigProvider";
      public const string CONFIG_PROVIDER_FILE = "ConfigProvider";
      
      public const string ROTATION_CONFIG_MENU = CONFIG_MENU + "Rotation";
      public const string ROTATION_CONFIG_FILE = "RotationConfig";
      
      public const string CELL_CONFIG_MENU = CONFIG_MENU + "Cell";
      public const string CELL_CONFIG_FILE = "CellConfig";
      
      public const string PREFAB_PROVIDER_MENU = CONFIG_MENU + "PrefabProvider";
      public const string PREFAB_PROVIDER_FILE = "PrefabProvider";
      
      public const string GAME_CONFIG_MENU = CONFIG_MENU + "Game";
      public const string GAME_CONFIG_FILE = "GameConfig";
      
      public const string SETTINGS_CONFIG_MENU = CONFIG_MENU + "Settings";
      public const string SETTINGS_CONFIG_FILE = "SettingsConfig";
      
      public const string SHAPE_CONFIG_MENU = CONFIG_MENU + "Shape";
      public const string SHAPE_CONFIG_FILE = "ShapeConfig";
    }
  }
}