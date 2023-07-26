using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UKAPI
{
    public static class Style
    {
        private static readonly Dictionary<string, string> toAdd = new();

        public static void AddStyleSource(string identifier, string readableName)
        {
            if (toAdd.Where((k) => k.Key == identifier).Count() == 0)
            {
                toAdd.Add(identifier, readableName);
            }
        }

        public static void AddStyleSource(string identifier, string readableName, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGB(color);
            readableName = $"<color=#{hex}>{readableName}</color>";

            if (toAdd.Where((k) => k.Key == identifier).Count() == 0)
            {
                toAdd.Add(identifier, readableName);
            }
        }

        public static void AddStyleSource(string identifier, string readableName, string hexColor)
        {
            if (!hexColor.Contains("#"))
            {
                hexColor = hexColor.Insert(0, "#");
            }

            if (!ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                throw new Exception("Hex colour is incorrect!");
            }

            readableName = $"<color={hexColor}>{readableName}</color>";

            if (toAdd.Where((k) => k.Key == identifier).Count() == 0)
            {
                toAdd.Add(identifier, readableName);
            }
        }

        public static void AddStyleSourceWithFormatting(string identifier, string readableNameWithFormatting)
        {
            if (toAdd.Where((k) => k.Key == identifier).Count() == 0)
            {
                toAdd.Add(identifier, readableNameWithFormatting);
            }
        }

        private static void LoadAll()
        {
            foreach (KeyValuePair<string, string> kvp in toAdd)
            {
                StyleHUD.Instance.idNameDict.Add(kvp.Key, kvp.Value);
            }

            Debug.Log("Loaded all queued style sources.");
        }

        [HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.Start)), HarmonyPostfix]
        public static void StyleHUDOnEnable()
        {
            LoadAll();
        }
    }
}
