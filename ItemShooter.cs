using UnityEngine;
using BS;

namespace ItemShooter
{
    // The item module will add a unity component to the item object. See unity monobehaviour for more information: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    // This component will apply a force on the player rigidbody to the direction of an item transform when the trigger is pressed (see custom reference in the item definition component of the item prefab)
    public class ItemShooter : MonoBehaviour
    {
        protected Item item;

        public ItemModuleShooter module;

        public Transform bulletSpawn;

        public AudioSource shotSFX = null;

        public ParticleSystem shotVFX = null;

        public bool hasShot = false;
        ItemData projectileOriginal;

        public bool isShootingAllowed;

        public bool nextShotReady = true;


        protected void Awake()
        {
            item = this.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleShooter>();

            isShootingAllowed = true;
            bulletSpawn = item.transform.Find(module.bulletSpawnName); 
            
            if(module.audioName != "None")
            {
                shotSFX = item.transform.Find(module.audioName).gameObject.GetComponent<AudioSource>();
            }

            if(module.particleSystemName != "None")
            {
                shotVFX = item.transform.Find(module.particleSystemName).gameObject.GetComponent<ParticleSystem>();
            }
            

            item.OnHeldActionEvent += OnTriggerPressed;
            projectileOriginal = Catalog.current.GetData<ItemData>(module.projectileID, true);
            
        }

        void OnTriggerPressed(Interactor interactor, Handle handle, Interactable.Action action)
        {

            if (action == Interactable.Action.UseStart && !hasShot && isShootingAllowed && nextShotReady)
            {
                
                if (!module.multipleShotsWithoutReleasingTrigger)
                {
                    hasShot = true;
                    Invoke("Shoot", module.shotDelay);
                }
                else
                {
                    InvokeRepeating("Shoot", module.shotDelay, module.delayBetweenShots);
                }
                
                


                nextShotReady = false;
                Invoke("Cooldown", module.delayBetweenShots);

                

            }


            if (action == Interactable.Action.UseStop)
            {

                hasShot = false;
                if (module.multipleShotsWithoutReleasingTrigger)
                {
                    CancelInvoke();
                }
                

            }

        }

        void Cooldown()
        {
            nextShotReady = true;

        }

        void Shoot()
        {
            Item projectile = projectileOriginal.Instantiate(null);
            projectile.gameObject.SetActive(true);


            projectile.transform.position = bulletSpawn.transform.position;
            projectile.transform.rotation = bulletSpawn.transform.rotation;

            if (shotSFX)
            {
                shotSFX.Play();
            }

            if (shotVFX)
            {
                shotVFX.Play();
            }


            projectile.Throw(1, Item.FlyDetection.CheckAngle);
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward.normalized * module.projectileSpeed, ForceMode.VelocityChange);
            
            if(module.timeToDespawn != 0)
            {
                Destroy(projectile.gameObject, module.timeToDespawn);
                Destroy(projectile, module.timeToDespawn);
            }

        }

    }
}