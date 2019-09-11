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
        public ParticleSystem triggerPressedVFX = null;

        public AudioSource triggerPressedSFX = null;

        public bool hasShot = false;

        public ObjectHolder gunHolder = null;

        public Animation animation = null;

        ItemData projectileOriginal;

        public bool isShootingAllowed;

        public bool nextShotReady = true;

        public bool magInPlace = true;


        protected void Awake()
        {
            item = this.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleShooter>();

            isShootingAllowed = true;
            bulletSpawn = item.transform.Find(module.bulletSpawnName); 
            
            if(module.shootSFX != "None")
            {
                shotSFX = item.transform.Find(module.shootSFX).gameObject.GetComponent<AudioSource>();
            }

            if (module.triggerPressSFX != "None")
            {
                triggerPressedSFX = item.transform.Find(module.triggerPressSFX).gameObject.GetComponent<AudioSource>();
            }
            

            if (module.shootVFX != "None")
            {
                shotVFX = item.transform.Find(module.shootVFX).gameObject.GetComponent<ParticleSystem>();
            }

            if (module.triggerPressVFX != "None")
            {
                triggerPressedVFX = item.transform.Find(module.shootVFX).gameObject.GetComponent<ParticleSystem>();
            }

            if (item.GetComponentInChildren<ObjectHolder>())
            {
                gunHolder = item.GetComponentInChildren<ObjectHolder>();
            }

            if (item.GetComponentInChildren<Animation>())
            {
                animation = item.GetComponentInChildren<Animation>();
            }


            item.OnHeldActionEvent += OnTriggerPressed;
            item.OnUngrabEvent += OnGunUngrab;

            projectileOriginal = Catalog.current.GetData<ItemData>(module.projectileID, true);
            
            
        }

        void OnGunUngrab(Handle handle, Interactor interactor, bool throwing)
        {
            if(handle == item.mainHandleLeft || handle == item.mainHandleRight)
            {
                CancelInvoke("Shoot");
            }
        }

        void OnTriggerPressed(Interactor interactor, Handle handle, Interactable.Action action)
        {
            Debug.Log("OnTriggerPressed starts");
            if (handle == item.mainHandleLeft || handle == item.mainHandleRight)
            {
                Debug.Log("Handle checked");
                if (gunHolder)
                {
                    if(gunHolder.holdObjects.Count > 0)
                    {
                        if (gunHolder.holdObjects[0])
                        {
                            if (gunHolder.holdObjects[0].definition.itemId == module.magID)
                            {
                                magInPlace = true;
                            }
                            else
                            {
                                magInPlace = false;
                            }
                        }
                        else
                        {
                            magInPlace = false;
                        }
                    }
                    else
                    {
                        magInPlace = false;
                    }
                   

                }

                Debug.Log("Mag checked");

                if (magInPlace)                    
                {
                    if ((action == Interactable.Action.UseStart && !module.shootWithAltUse) || (action == Interactable.Action.AlternateUseStart && module.shootWithAltUse))
                    {
                        Debug.Log("Button checked");
                        if (!hasShot && isShootingAllowed && nextShotReady)
                        {
                            Debug.Log("Bools checked");
                            if (!module.multipleShotsWithoutReleasingTrigger)
                            {
                                hasShot = true;
                                if (triggerPressedSFX)
                                {
                                    triggerPressedSFX.Play();
                                }

                                if (triggerPressedVFX)
                                {
                                    triggerPressedVFX.Play();
                                }
                                
                                Invoke("Shoot", module.shotDelay);
                            }
                            else
                            {
                                InvokeRepeating("Shoot", module.shotDelay, module.delayBetweenShots);
                            }
                            Debug.Log("Shoot called");
                            nextShotReady = false;
                            Invoke("Cooldown", module.delayBetweenShots);
                        }
                    }
                }

                

                


                if ((action == Interactable.Action.UseStop && !module.shootWithAltUse) || (action == Interactable.Action.AlternateUseStop && module.shootWithAltUse))
                {

                    hasShot = false;
                    if (module.multipleShotsWithoutReleasingTrigger)
                    {
                        CancelInvoke("Shoot");
                    }


                }
            }

            Debug.Log("OnTriggerPressed ends");

        }

        void Cooldown()
        {
            nextShotReady = true;

        }

        void Shoot()
        {
            Item projectile = projectileOriginal.Instantiate(null);
            projectile.gameObject.SetActive(true);

            if (animation)
            {
                animation.Play();
            }

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


            projectile.Throw(1, Item.FlyDetection.Forced);
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward.normalized * module.projectileSpeed, ForceMode.VelocityChange);
            
            if(module.timeToDespawn != 0)
            {
                Destroy(projectile.gameObject, module.timeToDespawn);
                Destroy(projectile, module.timeToDespawn);
            }

        }

    }
}