using BS;

namespace ItemShooter
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleShooter : ItemModule
    {
        public string projectileID = "Arrow1";
        public string bulletSpawnName = "ParryPoint";
        public string audioName = "None";
        public string particleSystemName = "None";

        public float projectileSpeed = 40f;
        public float timeToDespawn = 1f;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemShooter>();
        }
    }
}
