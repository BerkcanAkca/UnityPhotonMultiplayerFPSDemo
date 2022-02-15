using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShooter : MonoBehaviourPunCallbacks
{
    Camera cam;
    [SerializeField] GameObject bulletImpact;
    //[SerializeField] float timeBetweenShots = 0.1f;
    private float shotCounter;
    [SerializeField] float maxHeat = 10f, /*heatPershot = 1f,*/ coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overheated;
    public Gun[] gunsArray;
    private int currentGunIndex = 0;
    [SerializeField] float muzzleDisplayTime;
    private float muzzleCounter;
    public GameObject playerHitImpact;
    public int maxHealth = 100;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        UIController.instance.weaponTempSlider.maxValue = maxHeat;


        //SwitchGun();
        photonView.RPC("SetGun", RpcTarget.All, currentGunIndex);

        currentHealth = maxHealth;
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (gunsArray[currentGunIndex].muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;
                if (muzzleCounter <= 0)
                {
                    muzzleCounter = 0;
                    gunsArray[currentGunIndex].muzzleFlash.SetActive(false);
                }
            }

            //Shooting and Overheat
            if (!overheated)
            {
                if (Input.GetMouseButtonDown(0))
                    Shoot();

                if (Input.GetMouseButton(0) && gunsArray[currentGunIndex].isAutomatic)
                {
                    shotCounter -= Time.deltaTime;

                    if (shotCounter <= 0)
                        Shoot();

                }

                heatCounter -= coolRate * Time.deltaTime;
            }
            else
            {
                heatCounter -= overheatCoolRate * Time.deltaTime;
                if (heatCounter <= 0)
                {

                    overheated = false;
                    UIController.instance.overheatedMessage.gameObject.SetActive(false);
                }
            }

            if (heatCounter <= 0)
            {
                heatCounter = 0;
            }
            UIController.instance.weaponTempSlider.value = heatCounter;
            
            //Gun Selection
            GunsSelection();

            
        }
            
    }

    [PunRPC]
    public void DealDamage(string damager, int damageAmount, int actor)
    {
        TakeDamage(damager, damageAmount, actor);
    }
    public void TakeDamage(string damager, int damageAmount, int actor)
    {
        if (photonView.IsMine)
        {
            //Debug.Log(photonView.Owner.NickName + "has been hit by " + damager);
            currentHealth -= damageAmount;

            if(currentHealth <= 0)
            {
                currentHealth = 0;
                PlayerSpawner.instance.Die(damager);
                MatchManager.instance.UpdateStatsSend(actor, 0, 1); //kill stat send
            }
            UIController.instance.healthSlider.value = currentHealth;

        }
        
    }
    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, 
                    photonView.Owner.NickName, gunsArray[currentGunIndex].shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {


                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bulletImpactObject, 5f);
            }
        }

        shotCounter = gunsArray[currentGunIndex].timeBetweenShots;

        heatCounter += gunsArray[currentGunIndex].heatPerShot;
        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overheated = true;
            UIController.instance.overheatedMessage.gameObject.SetActive(true);
        }
        gunsArray[currentGunIndex].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
    }

    void GunsSelection()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            currentGunIndex++;

            if (currentGunIndex >= gunsArray.Length)
            {
                currentGunIndex = 0;
            }
            //SwitchGun();
            photonView.RPC("SetGun", RpcTarget.All, currentGunIndex);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            currentGunIndex--;

            if (currentGunIndex < 0)
            {
                currentGunIndex = gunsArray.Length - 1;
            }
            //SwitchGun();
            photonView.RPC("SetGun", RpcTarget.All, currentGunIndex);
        }

        //Switch with Top Numbers
        for (int i = 0; i < gunsArray.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                currentGunIndex = i;
                //SwitchGun();
                photonView.RPC("SetGun", RpcTarget.All, currentGunIndex);
            }
        }




    }

    void SwitchGun()
    {
        foreach(Gun gun in gunsArray )
        {
            gun.gameObject.SetActive(false);
        }

        gunsArray[currentGunIndex].gameObject.SetActive(true);
        gunsArray[currentGunIndex].muzzleFlash.SetActive(false);



    }

    [PunRPC]
    public void SetGun(int gunToSwitchTo)
    {
        if (gunToSwitchTo < gunsArray.Length)
        {
            currentGunIndex = gunToSwitchTo;
            SwitchGun();
        }
    }
}
