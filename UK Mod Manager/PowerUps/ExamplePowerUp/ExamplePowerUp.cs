using System;

namespace UKAPI.PowerUps.ExamplePowerUp
{
    /// <summary>
    /// Example Speed Increase PowerUp
    /// </summary>
    public class ExamplePowerUp : TimedPowerUp
    {
        /// <summary>
        /// Define initial behaviour here, such as stat increases
        /// </summary>
        protected override void Start()
        {
            base.Start();

            NewMovement.Instance.walkSpeed *= 2;
        }

        /// <summary>
        /// Define code to run every second, such as a cooldown refresh
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
        }

        /// <summary>
        /// Define behaviour to remove any effects from initial behaviour, such as stat buffs.
        /// </summary>
        private void OnDestroy()
        {
            NewMovement.Instance.walkSpeed /= 2;
        }
    }
}