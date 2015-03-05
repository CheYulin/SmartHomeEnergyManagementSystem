﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//json解析
using System.Runtime.Serialization.Json;
using System.IO;

namespace SHEMS.Codes
{
    class Utilities
    {
        public static string toJsonData(object item)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());
            string result = String.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, item);
                ms.Position = 0;
                using (StreamReader reader = new StreamReader(ms))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
    }
}