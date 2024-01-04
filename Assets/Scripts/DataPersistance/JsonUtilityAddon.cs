using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonUtilityAddon
{
    public static class JsonArray //https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
    {
        public static T[] ArrayFromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ArrayToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ArrayToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    public static class JsonDictionary //Mine
    {
        public static SerializableDictionary<Tkey, TValue> DictionaryToJson<Tkey, TValue>(Dictionary<Tkey, TValue> dictionary) 
        {
            SerializableDictionary<Tkey, TValue> serializedDictionary = new SerializableDictionary<Tkey, TValue>();

            serializedDictionary.keyArray = new Tkey[dictionary.Keys.Count];
            serializedDictionary.valueArray = new TValue[dictionary.Values.Count];                

            int index = 0;
            foreach (Tkey key in dictionary.Keys) 
            {
                serializedDictionary.keyArray[index] = key;
                index++;
                
                if (index > Mathf.Clamp(dictionary.Keys.Count-1, 0, dictionary.Keys.Count-1)) 
                {
                    index = 0;
                    break;
                }
            }
            foreach (TValue value in dictionary.Values)
            {
                serializedDictionary.valueArray[index] = value;
                index++;

                if (index > Mathf.Clamp(dictionary.Values.Count - 1, 0, dictionary.Values.Count - 1))
                {
                    index = 0;
                    break;
                }
            }

            return serializedDictionary;
        }

        public static Dictionary<Tkey, TValue> JsonToDictionary<Tkey, TValue>(SerializableDictionary<Tkey, TValue> serializedDictionary)
        {
            Dictionary<Tkey, TValue> dictionary = new Dictionary<Tkey, TValue>();


            int index = 0;
            foreach (Tkey key in serializedDictionary.keyArray)
            {
                dictionary.Add(serializedDictionary.keyArray[index], serializedDictionary.valueArray[index]);
                index++;

                if (index > Mathf.Clamp(dictionary.Keys.Count - 1, 0, dictionary.Keys.Count - 1))
                {
                    index = 0;
                    break;
                }
            }

            return dictionary;
        }

        [System.Serializable]
        public class SerializableDictionary<Tkey,TValue>
        {
            public Tkey[] keyArray;
            public TValue[] valueArray;
        }
    }
}
