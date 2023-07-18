using GameConsole;
using UnityEngine;

namespace UKAPI.Cheats.Example_Cheat
{
    [RegisterCheat]
    public class ExampleCheat : CheatBase
    {
        public override void OnCheatEnable()
        {
            Console.Instance.PrintLine("Hi");
        }

        public override void OnCheatDisable()
        {
            Console.Instance.PrintLine("Bye");
        }

        public override void Update()
        {
            Console.Instance.PrintLine("Why");
        }

        public override string Prefix => "ukapi";

        public override string ButtonEnabledOverride => null;

        public override string ButtonDisabledOverride => null;

        public override Sprite CheatIcon => null;

        public override bool DefaultState => false;

        public override StatePersistenceMode PersistenceMode => StatePersistenceMode.NotPersistent;

        public override string CheatCategory => "UKAPI";
    }
}