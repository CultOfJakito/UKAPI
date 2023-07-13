using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps {
	public static class PowerUpManager {
		private static Dictionary<string, PowerUpInfo> powerups = new Dictionary<string, PowerUpInfo>();

		/// <summary>
		/// Try's to get a PowerUpInfo from the powerups dictionary based on the id of the PowerUpInfo
		/// </summary>
		/// <param name="id">PowerUpInfo's ID</param>
		/// <returns>Requested PowerUpInfo</returns>
		public static PowerUpInfo GetPowerUp(string id) {
			powerups.TryGetValue(id, out PowerUpInfo res);
			return res;
		}
		
		/// <summary>
		/// Adds a PowerUpInfo to the powerups dictionary
		/// </summary>
		/// <param name="powerUp">A PowerUpInfo</param>
		public static void RegisterPowerUp(PowerUpInfo powerUp) {
			powerups[powerUp.ID] = powerUp;
		}

		/// <summary>
		/// Creates a PowerUpPickup (similar to the dual wield power up)using a PowerUpInfo and a Position
		/// </summary>
		/// <param name="info">PowerUpInfo for powerup</param>
		/// <param name="position">Vector3 representing position of powerup pickup</param>
		/// <returns></returns>
		public static GameObject CreatePowerUpPickup(PowerUpInfo info, Vector3 position) {
			return info.PickupBuilder.CreatePickup(position).gameObject;
		}

		/// <summary>
		/// Creates a PowerUpPickup (similar to the dual wield power up) using an id of a PowerUpInfo and a Position
		/// </summary>
		/// <param name="id">ID of PowerUpInfo</param>
		/// <param name="position">Vector3 representing position of powerup pickup</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static GameObject CreatePowerUpPickup(string id, Vector3 position) {
			PowerUpInfo info = GetPowerUp(id);
			if(info == null) {
				throw new InvalidOperationException("Cannot create powerup because ID was unknown.");
			}
			return CreatePowerUpPickup(info, position);
		}
	}
}
