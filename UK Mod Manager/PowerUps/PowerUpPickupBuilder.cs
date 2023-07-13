using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps {
	public class PowerUpPickupBuilder {
		internal static GameObject DualWieldPrefab { get; set; }

		public PowerUpInfo Info { get; }

		public PowerUpPickupBuilder(PowerUpInfo info) {
			Info = info;
		}

		public virtual PowerUpPickup CreatePickup(Vector3 position) {
			GameObject powerUpGO = UnityEngine.Object.Instantiate(DualWieldPrefab);
			UnityEngine.Object.Destroy(powerUpGO.GetComponent<DualWieldPickup>());

			powerUpGO.transform.position = position;

			Renderer renderer = powerUpGO.GetComponent<Renderer>();
			renderer.material.color = Info.Color;

			powerUpGO.AddComponent<PowerUpPickup>();
			PowerUpPickup pickup = powerUpGO.GetComponent<PowerUpPickup>();
			pickup.Info = Info;

			SpriteRenderer renderer2 = powerUpGO.transform.GetComponentInChildren<SpriteRenderer>();
			renderer2.sprite = Info.Icon;

			Light light = powerUpGO.transform.GetComponentInChildren<Light>();
			light.color = Info.Color;
			light.intensity = Info.LightIntensity;

			return pickup;
		}
	}
}
