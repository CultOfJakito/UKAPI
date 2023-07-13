using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {

        public PowerUpInfo Info { get; internal set; }
        
        protected virtual void Start() {
			MonoSingleton<CameraController>.Instance.CameraShake(0.35f);
		}
    }

}
