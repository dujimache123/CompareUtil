using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compare
{
    class UserDefault
    {
        public static Dictionary<string, object> defaultMap = new Dictionary<string, object>();
        public static void Init()
        {
            var fileName = "./UserDefault.txt";
            var fs = new FileStream(fileName, FileMode.OpenOrCreate);
            var sr = new StreamReader(fs);
            var text = sr.ReadToEnd();
            fs.Close();
            sr.Close();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            if (jsonObj != null)
            {
                defaultMap = jsonObj;
            }
        }

        public static void Save()
        {
            var jsonStr = JsonConvert.SerializeObject(defaultMap);
            var fileName = "./UserDefault.txt";
            var fs = new FileStream(fileName, FileMode.OpenOrCreate);
            var sw = new StreamWriter(fs);
            sw.Write(jsonStr);
            sw.Close();
            fs.Close();
        }

        public static void SetValue(string key, object value)
        {
            defaultMap[key] = value;
            Save();
        }

        public static object GetValue(string key, object defaultValue)
        {
            if (defaultMap.ContainsKey(key))
            {
                return defaultMap[key];
            }
            return defaultValue;
        }
    }
}
