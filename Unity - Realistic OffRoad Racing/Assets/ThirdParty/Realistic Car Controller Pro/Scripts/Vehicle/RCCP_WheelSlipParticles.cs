using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCCP_WheelSlipParticles : RCCP_Component {

    private ParticleSystem wheelParticles;
    public ParticleSystem WheelParticles {

        get {

            if (!wheelParticles)
                wheelParticles = GetComponent<ParticleSystem>();

            if (!wheelParticles) {

                Debug.LogError("Particles couldn't found on this gameobject named " + gameObject.name + "! In order to use the wheel particles on your wheels, ensure that your prefab has proper particle system.");
                return null;

            }

            return wheelParticles;

        }

    }

    [Range(0f, 100f)] public float minEmissionRate = 2.5f;
    [Range(0f, 100f)] public float maxEmissionRate = 25f;

    public void Emit(bool state) {

        ParticleSystem.EmissionModule em = WheelParticles.emission;
        em.enabled = state;

        SetEmissionRate(.5f);

    }

    public void Emit(bool state, float volume) {

        ParticleSystem.EmissionModule em = WheelParticles.emission;
        em.enabled = state;

        SetEmissionRate(volume);

    }

    public void SetEmissionRate(float volume) {

        // Get the emission module from the ParticleSystem
        var emission = WheelParticles.emission;

        // Modify the emission rate over time
        emission.rateOverTime = Mathf.Lerp(minEmissionRate, maxEmissionRate, volume);

    }

}
