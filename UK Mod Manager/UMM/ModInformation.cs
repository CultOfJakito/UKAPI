using System;
using BepInEx;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UKAPI.Internal;
using System.Reflection;
using System.Linq;

namespace UKAPI.UMM
{
    public class ModInformation : IComparable<ModInformation>
    {
        public ModType modType { get; }
        public Type mod { get; }
        public string GUID { get; }
        public string modName { get; private set; }
        public string modDescription { get; private set; }
        public Texture2D previewIcon { get; internal set; }
        public Version modVersion { get; private set; }
        public bool supportsUnloading { get; }
        public bool loadOnStart { get; internal set; }
        public bool loaded { get; internal set; }
        public List<Dependency> dependencies { get; private set; }
        public DirectoryInfo fileDirectory;
        internal MethodInfo unloadMethod { get; private set; } = null;

        public ModInformation(Type mod, ModType modType, DirectoryInfo fileDirectory)
        {
            Plugin.logger.LogInfo("Creating mod info " + fileDirectory.FullName + " " + mod.Name);
            this.modType = modType;
            this.mod = mod;

            if (modType == ModType.BepInPlugin)
            {
                BepInPlugin metaData = UltraModManager.GetBepinMetaData(mod);
                GUID = metaData.GUID;
                dependencies = UltraModManager.GetBepinDependencies(mod);
                if (!GetMetadataFromFile(fileDirectory.FullName))
                {
                    modName = metaData.Name;
                    modVersion = metaData.Version;
                    modDescription = "NO DESCRIPTION FOUND";
                }
                unloadMethod = (from x in mod.GetMethods() where x.Name == "OnUnload" && x.GetParameters().Count() == 0 select x).FirstOrDefault();
                if (unloadMethod != null)
                {
                    supportsUnloading = true;
                    Debug.Log("Found unload method.");
                }
            }
            else if (modType == ModType.UKMod)
            {
                UKPlugin metaData = UltraModManager.GetUKMetaData(mod);
                GUID = metaData.GUID;
                dependencies = UltraModManager.GetUKModDependencies(mod);
                foreach (Dependency dependency in UltraModManager.GetBepinDependencies(mod))
                    dependencies.Add(dependency);
                if (!metaData.usingManifest || !GetMetadataFromFile(fileDirectory.FullName))
                {
                    modName = metaData.name;
                    modDescription = metaData.description;
                    if (metaData.version != null)
                        modVersion = Version.Parse(metaData.version);
                }
                supportsUnloading = metaData.unloadingSupported;
            }
        }

        private bool GetMetadataFromFile(string fileDirectory)
        {
            FileInfo file = new FileInfo(fileDirectory + "/manifest.json");
            if (!file.Exists)
                return false;
            // Read json from file and convert it to string -> string dictionary : thanks copilot :D
            try
            {
                ManifestStruct manifest = JsonConvert.DeserializeObject<ManifestStruct>(File.ReadAllText(file.FullName));
                this.modName = manifest.name;
                this.modDescription = manifest.description;
                if (manifest.version_number != null)
                    this.modVersion = Version.Parse(manifest.version_number);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Clicked()
        {
            if (!loaded)
                LoadThisMod();
            else
                UnLoadThisMod();
        }

        public int CompareTo(ModInformation other)
        {
            return String.Compare(modName, other.modName);
        }

        public bool LoadThisMod()
        {
            if (!loaded)
            {
                UltraModManager.LoadMod(this);
            }
            return loaded;
        }

        public void UnLoadThisMod()
        {
            if (loaded && supportsUnloading)
            {
                UltraModManager.UnloadMod(this);
            }
        }

        public enum ModType
        {
            UKMod,
            BepInPlugin
        }

        public class Dependency
        {
            public string GUID;
            public Version MinimumVersion;
        }

        private struct ManifestStruct
        {
            public string name;
            public string description;
            public string version_number;
        }
    }
}
