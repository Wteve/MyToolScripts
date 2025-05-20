using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTool
{

    public class MySaveData<T>
    {

        public string key;
        public T defaultValue;

        public MySaveData(string key, T defaultValue = default)
        {
            this.key = key;
            this.defaultValue = defaultValue;

            if (typeof(T) == typeof(int))
            {
                m_value = (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
            }
            else if (typeof(T) == typeof(float))
            {
                m_value = (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
            }
            else if (typeof(T) == typeof(string))
            {
                m_value = (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
            }
            else if (typeof(T) == typeof(bool))
            {
                m_value = (T)(object)(PlayerPrefs.GetInt(key, (bool)(object)defaultValue ? 1 : 0) == 1);
            }
            else
            {
                Debug.LogError($"不支持的类型: {typeof(T)}");
            }
        }

        public T m_value;
        public T Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
                if (typeof(T) == typeof(int))
                {
                    PlayerPrefs.SetInt(key, (int)(object)value);
                }
                else if (typeof(T) == typeof(float))
                {
                    PlayerPrefs.SetFloat(key, (float)(object)value);
                }
                else if (typeof(T) == typeof(string))
                {
                    PlayerPrefs.SetString(key, (string)(object)value);
                }
                else if (typeof(T) == typeof(bool))
                {
                    PlayerPrefs.SetInt(key, (bool)(object)value ? 1 : 0);
                }
                else
                {
                    Debug.LogError($"不支持的类型: {typeof(T)}");
                    return;
                }
                // PlayerPrefs.Save();
            }
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(key);
            // PlayerPrefs.Save();
        }

        public bool HasValue()
        {
            return PlayerPrefs.HasKey(key);
        }
    }

}