using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koi.Common
{
    public class UParticleUtils
    {
        public static void SetupLoop(ParticleSystem particle, bool loop)
        {
            var main = particle.main;
            main.loop = loop;
        }

        public static void SetupParticleColor(ParticleSystem particle, Color color)
        {
            var main = particle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
        }

        public static void SetupParticleMat(ParticleSystem particle, Material mat)
        {
            particle.GetComponent<ParticleSystemRenderer>().material = mat;
        }
    }
}
