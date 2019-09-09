using BS;

namespace ItemShooter
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleShooter : ItemModule
    {
        public string projectileID = "Arrow1";
        public string bulletSpawnName = "ParryPoint";

        public bool shootWithAltUse = false;

        public string shootSFX = "None";
        public string triggerPressSFX = "None";

        public string shootVFX = "None";
        public string triggerPressVFX = "None";

        public float projectileSpeed = 40f;
        public float timeToDespawn = 1f;

        public float shotDelay = 0.1f;
        public float delayBetweenShots = 1f;
        public bool multipleShotsWithoutReleasingTrigger = false;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemShooter>();
        }
    }
}
