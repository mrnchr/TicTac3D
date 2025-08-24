namespace CollectiveMind.TicTac3D.Runtime.AssetManagement
{
  public interface IPrefabFactory
  {
    TObject Create<TObject>(EntityType id);
  }
}