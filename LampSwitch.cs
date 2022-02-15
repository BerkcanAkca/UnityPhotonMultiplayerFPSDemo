using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampSwitch : MonoBehaviour, IInteractable
{
    Light _light;
    public bool lightsOn;
    [SerializeField] bool isThisLightOnAtStart = true;
   

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        lightsOn = isThisLightOnAtStart;
        
    }

    void Switch()
    {
        lightsOn = !lightsOn;

        if (lightsOn && _light.enabled == false)
            _light.enabled = true;
        else if (!lightsOn && _light.enabled == true)
            _light.enabled = false;
    }
    
    public void Interact()
    {
        Switch();
    }
}
