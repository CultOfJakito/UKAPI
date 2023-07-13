using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps {
	public static class PowerUpManager {
		private static Dictionary<string, PowerUpInfo> powerups = new Dictionary<string, PowerUpInfo>();

		public static PowerUpInfo GetPowerUp(string id) {
			powerups.TryGetValue(id, out PowerUpInfo res);
			return res;
		}
		public static void RegisterPowerUp(PowerUpInfo powerUp) {
			powerups[powerUp.ID] = powerUp;
		}

		public static GameObject CreatePowerUpPickup(PowerUpInfo info, Vector3 position) {
			return info.PickupBuilder.CreatePickup(position).gameObject;
		}

		public static GameObject CreatePowerUpPickup(string id, Vector3 position) {
			PowerUpInfo info = GetPowerUp(id);
			if(info == null) {
				throw new InvalidOperationException("Cannot create powerup because ID was unknown.");
			}
			return CreatePowerUpPickup(info, position);
		}
	}
}
