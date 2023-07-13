using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps {
	public class PowerUpInfo {
		public string ID { get; set; } = Guid.NewGuid().ToString();
		public Color Color { get; set; } = Color.white;
		public Sprite Icon { get; set; } = null;
		public float LightIntensity { get; set; } = 10f;
		public PowerUpPickupBuilder PickupBuilder { get; set; }
		public Type BehaviourType { get; set; }
		public float DurationSeconds { get; set; } = 30f;
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
