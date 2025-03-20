namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  public interface IPrefabFactory
  {
    TObject Create<TObject>(EntityType id);
  }
}