using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonUtilityAddon
{
    public static class JsonArray //https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
    {
        public static T[] ArrayFromJson<T>(string json)
        {
            SerializableArray<T> wrapper = JsonUtility.FromJson<SerializableArray<T>>(json);
            return wrapper.elements;
        }

        public static string ArrayToJson<T>(T[] array)
        {
            SerializableArray<T> wrapper = new SerializableArray<T>();
            wrapper.elements = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ArrayToJson<T>(T[] array, bool prettyPrint)
        {
            SerializableArray<T> wrapper = new SerializableArray<T>();
            wrapper.elements = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }
        
        [System.Serializable]
        public class SerializableArray<T>
        {
            public T[] elements;
        }
    }

    public static class JsonList // Edited the one above slightly.
    {
        public static List<T> ListFromJson<T>(string json)
        {
            SerializableList<T> wrapper = JsonUtility.FromJson<SerializableList<T>>(json);
            return wrapper.content;
        }

        public static string ListToJson<T>(List<T> list)
        {
            SerializableList<T> wrapper = new SerializableList<T>();
            wrapper.content = list;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ListToJson<T>(List<T> list, bool prettyPrint)
        {
            SerializableList<T> wrapper = new SerializableList<T>();
            wrapper.content = list;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        public class SerializableList<T>
        {
            public List<T> content;
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
                
                if (index > Mathf.Clamp(dictionary.Keys.Count-1, 0, dictionary.Keys.Count-1)) //So that it doesn't loop but just iterates over every element once.
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

    public static class JsonVector 
    {
        [System.Serializable]
        public class SerializableVector3
        {
            public float x;
            public float y;
            public float z;
        }
    }
}
