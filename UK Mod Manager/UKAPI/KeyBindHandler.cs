using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UKAPI.Internal
{
    internal static class KeyBindHandler
    {
        internal static Dictionary<string, UKKeyBind> moddedKeyBinds = new Dictionary<string, UKKeyBind>();
        internal static KeyBindEnabledEvent OnKeyBindEnabled = new KeyBindEnabledEvent();

        internal static void LoadKeyBinds() // Copilot basically wrote this code
        {
            if (SaveFileHandler.EnsureModData("UMM", "KeyBinds"))
            {
                string[] keyBinds = SaveFileHandler.RetrieveModData("KeyBinds", "UMM").Split(';');
                foreach (string keyBind in keyBinds)
                {
                    string[] keyBindData = keyBind.Split(':');
                    if (keyBindData.Length == 2)
                    {
                        Plugin.logger.LogMessage("Loading keybind" + keyBindData[0] + " : " + keyBindData[1]);
                        UKKeyBind bind = new UKKeyBind(new InputAction(keyBindData[0], InputActionType.Value, null, null, null, null), keyBindData[0], (KeyCode)Enum.Parse(typeof(KeyCode), keyBindData[1]));
                        moddedKeyBinds.Add(keyBindData[0], bind);
                    }
                }
            }
            else
            {
                SaveFileHandler.SetModData("UMM", "KeyBinds", "");
            }
        }

        internal static UKKeyBind GetKeyBind(string key, KeyCode defaultBind)
        {
            if (moddedKeyBinds.ContainsKey(key))
                return moddedKeyBinds[key];
            UKKeyBind bind = new UKKeyBind(new InputAction(key, InputActionType.Value, null, null, null, null), key, defaultBind);
            moddedKeyBinds.Add(key, bind);
            return bind;
        }

        internal static void DumpKeyBinds()
        {
            string keyBinds = "";
            foreach (KeyValuePair<string, UKKeyBind> keyBind in moddedKeyBinds)
            {
                Plugin.logger.LogInfo("Adding keybind " + keyBind.Key + ":" + keyBind.Value.keyBind + ";");
                keyBinds += keyBind.Key + ":" + keyBind.Value.keyBind + ";"; // This is fine because all keyBinds should only have one action
            }
            SaveFileHandler.SetModData("UMM", "KeyBinds", keyBinds);
        }

        internal static IEnumerator SetKeyBindRoutine(GameObject currentKey, string keyName) // I copy and pasted this function completely, credit to whoever wrote ControlsOptions.OnGUI
        {
            Color32 normalColor = new Color32(20, 20, 20, 255);
            yield return null;
            while (true) // This is bad practice, I don't care :P
            {
                Event current = Event.current;
                if (current == null)
                {
                    yield return null;
                    continue;
                }
                KeyCode keyCode = KeyCode.None;
                if (current.keyCode == KeyCode.Escape)
                {
                    currentKey.GetComponent<Image>().color = normalColor;
                    currentKey = null;
                    OptionsManager.Instance.dontUnpause = false;
                }
                else if (current.isKey || current.isMouse || current.button > 2 || current.shift)
                {
                    if (current.isKey)
                    {
                        keyCode = current.keyCode;
                    }
                    else if (Input.GetKey(KeyCode.LeftShift))
                    {
                        keyCode = KeyCode.LeftShift;
                    }
                    else if (Input.GetKey(KeyCode.RightShift))
                    {
                        keyCode = KeyCode.RightShift;
                    }
                    else
                    {
                        if (current.button > 6)
                        {
                            currentKey.GetComponent<Image>().color = normalColor;
                            OptionsManager.Instance.dontUnpause = false;
                            yield break;
                        }
                        keyCode = KeyCode.Mouse0 + current.button;
                    }
                }
                else if (Input.GetKey(KeyCode.Mouse3) || Input.GetKey(KeyCode.Mouse4) || Input.GetKey(KeyCode.Mouse5) || Input.GetKey(KeyCode.Mouse6))
                {
                    if (Input.GetKey(KeyCode.Mouse4))
                    {
                        keyCode = KeyCode.Mouse4;
                    }
                    else if (Input.GetKey(KeyCode.Mouse5))
                    {
                        keyCode = KeyCode.Mouse5;
                    }
                    else if (Input.GetKey(KeyCode.Mouse6))
                    {
                        keyCode = KeyCode.Mouse6;
                    }
                }
                else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    keyCode = KeyCode.LeftShift;
                    if (Input.GetKey(KeyCode.RightShift))
                    {
                        keyCode = KeyCode.RightShift;
                    }
                }
                else
                {
                    yield return null;
                    continue;
                }
                if (keyCode == KeyCode.None)
                {
                    yield return null;
                    continue;
                }
                //InputManager.Instance.Inputs[this.currentKey.name] = keyCode;
                currentKey.GetComponentInChildren<Text>().text = ControlsOptions.GetKeyName(keyCode);
                //MonoSingleton<PrefsManager>.Instance.SetInt("keyBinding." + this.currentKey.name, (int)keyCode);
                //InputManager.Instance.UpdateBindings();
                moddedKeyBinds[keyName].ChangeKeyBind(keyCode);
                currentKey.GetComponent<Image>().color = normalColor;
                OptionsManager.Instance.dontUnpause = false;
                yield break;
            }
        }

        internal class KeyBindEnabledEvent : UnityEvent<UKKeyBind> { }
    }
}
