using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UKAPI.PowerUps {
	public abstract class TimedPowerUp : PowerUp {
		internal static bool isDualWieldActive = false;

		protected PowerUpMeter meter;

		protected override void Start() {
			base.Start();

			meter = PowerUpMeter.Instance;

			if(meter == null) {
				return;
			}

			meter.latestMaxJuice = Info.DurationSeconds;
			meter.juice = Info.DurationSeconds;
			meter.powerUpColor = Info.Color.ChangeHSV(0f, -0.1f, 0f);
		}

		protected virtual void Update() {

			if (meter == null)
			{
				meter = PowerUpMeter.Instance;
			}

			if(meter == null) {
				return;
			}

			if(meter.juice <= 0f) {
				Info.Deactivate();
			}
		}
		
	}
	internal static class ColorExtensions {
		public static Color ChangeHSV(this Color color, float h, float s, float v) {
			Color.RGBToHSV(color, out float oldH, out float oldS, out float oldV);
			return Color.HSVToRGB(oldH + h, oldS + s, oldV + v);
		}
	}
}
