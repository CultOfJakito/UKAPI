using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps {
	public class PowerUpInfo {
		
		/// <summary>
		/// ID for the PowerUpInfo
		/// </summary>
		public string ID { get; set; } = Guid.NewGuid().ToString();
		
		/// <summary>
		/// Color of the PowerUpPickup and the light source in a PowerUpPickup
		/// </summary>
		public Color Color { get; set; } = Color.white;
		
		/// <summary>
		/// Icon of the PowerUp, appears in the center of the PowerUp
		/// </summary>
		public Sprite Icon { get; set; } = null;
		
		/// <summary>
		/// Brightness of the light source in a PowerUpPickUp
		/// </summary>
		public float LightIntensity { get; set; } = 10f;
		
		/// <summary>
		/// PowerUpPickupBuilder, Only change for custom changes to a power up pickup.
		/// </summary>
		public PowerUpPickupBuilder PickupBuilder { get; set; }
		
		/// <summary>
		/// Script which defines PowerUp Behaviour (Inherits from PowerUp or TimedPowerUp)
		/// </summary>
		public Type BehaviourType { get; set; }
		
		/// <summary>
		/// Duration of the PowerUp in Seconds
		/// </summary>
		public float DurationSeconds { get; set; } = 30f;
		
		/// <summary>
		/// Whether the PowerUp works in Clash Mode or not (Gives 3 Extra Hits if True)
		/// </summary>
		public bool FPSOnly { get; set; } = true;
		
		private static GameObject _holderObject;

		private static GameObject HolderObject {
			get {
				if(_holderObject == null) {
					_holderObject = new GameObject("Powerup Holder");
				}
				return _holderObject;
			}
		}

		public virtual bool Activate(out PowerUp component) {
			if(PlayerTracker.Instance.playerType == PlayerType.Platformer && FPSOnly) {
				MonoSingleton<CameraController>.Instance.CameraShake(0.35f);
				PlatformerMovement.Instance.AddExtraHit(3);
				component = null;
				return false;
			}

			GameObject obj = HolderObject;
			component = obj.GetComponent(BehaviourType) as PowerUp;
			if(component != null) {
				return false;
			}
			component = obj.AddComponent(BehaviourType) as PowerUp ?? throw new Exception("The behaviour type is not a PowerUp!");
			component.Info = this;
			return true;
		}
		public virtual void Deactivate() {
			GameObject obj = HolderObject;
			Component component = obj.GetComponent(BehaviourType);
			if(component == null) {
				return;
			}
			UnityEngine.Object.Destroy(component);
		}

		public PowerUpInfo() {
			PickupBuilder = new PowerUpPickupBuilder(this);
		}
	}
}
