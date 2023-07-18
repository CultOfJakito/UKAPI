using UnityEngine;

namespace UKAPI.Cheats
{
    public abstract class CheatBase : ICheat
    {
        public void Enable()
        {
            active = true;
            OnCheatEnable();
        }

        public abstract void OnCheatEnable();

        public virtual void Disable()
        {
            active = false;
            OnCheatDisable();
        }
        
        public abstract void OnCheatDisable();

        
        public abstract void Update();

    
        protected virtual string TypeNameForNameGeneration
        {
            get
            {
                string name = GetType().Name;
                if (name.EndsWith("Cheat"))
                {
                    name = name.Substring(0, name.Length - "Command".Length);
                }

                return name;
            }
        }

        private bool active;

        public string LongName => TypeNameForNameGeneration;
        
        public abstract string Prefix { get; }

        public string Identifier => $"{Prefix}.{LongName.ToLower().Replace(' ', '.')}";

        public abstract string ButtonEnabledOverride { get; }

        public abstract string ButtonDisabledOverride { get; }

        public abstract Sprite CheatIcon { get; }

        public string Icon => CheatIcon.name;

        public bool IsActive => active;

        public abstract bool DefaultState { get; }

        public abstract StatePersistenceMode PersistenceMode { get; }
        
        public abstract string CheatCategory { get; }
    }
}