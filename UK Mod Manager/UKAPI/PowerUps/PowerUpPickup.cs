using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps
{
    public class PowerUpPickup:MonoBehaviour
    {
        public PowerUpInfo Info { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                this.PickedUp();
            }
        }

        private void PickedUp()
        {
            Info.Activate(out _);
            Destroy(base.gameObject);
        }
    }
}
