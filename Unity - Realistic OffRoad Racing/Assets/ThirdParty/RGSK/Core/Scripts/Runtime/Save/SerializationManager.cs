using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace RGSK
{
    public static class SerializationManager
    {
        public static bool Save(object data, string path, bool encrypt = false)
        {
            try
            {
                var setting = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                var json = JsonConvert.SerializeObject(data, setting);
                File.WriteAllText(path, !encrypt ? json : EncryptDecrypt(json));
                
#if UNITY_WEBGL && !UNITY_EDITOR
                //Force sync when running on WebGL to ensure save persists
                PlayerPrefs.SetString("webgl_sync", string.Empty);
#endif
            }
            catch (Exception e)
            {
                Logger.Log($"Save Error: {e.Message}");
                return false;
            }

            return true;
        }

        public static object Load(string path, Type type, bool decrypt = false)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                var json = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject(!decrypt ? json : EncryptDecrypt(json), type);
                return data;
            }
            catch (Exception e)
            {
                Logger.Log($"Load Error: {e.Message}");

                if (e.Message.Contains("Unexpected character"))
                {
                    Logger.Log($"Failed to decrypt load data! Retrying with decryption set to: {!decrypt}.");
                    return Load(path, type, !decrypt);
                }

                return null;
            }
        }

        static string EncryptDecrypt(string data)
        {
            var key = "data012345";
            var result = "";

            for (int i = 0; i < data.Length; i++)
            {
                result += (char)(data[i] ^ key[i % key.Length]);
            }

            return result;
        }
    }
}