using System;

namespace UKAPI.PowerUps.ExamplePowerUp
{
    public class ExamplePowerUp : TimedPowerUp
    {
        protected override void Start()
        {
            base.Start();

            NewMovement.Instance.walkSpeed *= 2;
        }

        protected override void Update()
        {
            base.Update();
            
        }

        private void OnDestroy()
        {
            NewMovement.Instance.walkSpeed /= 2;
        }
    }
}