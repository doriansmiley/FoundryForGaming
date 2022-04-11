using System;
using System.Collections.Generic;
using GPF.ServerObjects;
using GPF.Content;
using System.Reflection;
using System.IO;
using System.Text;

namespace GPF.Build
{
    internal class ClassScanner
    {
        static void Main(string[] args)
        {
            SearchAssemblies();
        }

        static void SearchAssemblies()
        {
            var totalClassList = new List<Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                totalClassList.AddRange(GetTypeList(a));
            }

            StringBuilder sb = new StringBuilder();
            foreach (var t in totalClassList)
                sb.AppendLine(t.FullName);

            File.WriteAllText("classes.txt", sb.ToString());
        }

        static List<Type> GetTypeList(Assembly a)
        {
            var result = new List<Type>();
            foreach (Type t in a.GetTypes())
            {
                if (Attribute.GetCustomAttribute(t, typeof(ServerContentAttribute)) != null)
                {
                    result.Add(t);
                    continue;
                }
                if (t.IsAbstract || t.IsInterface)
                    continue;
                if (t.IsSubclassOf(typeof(ServerObject)) || t.IsSubclassOf(typeof(ServerObjectMessage)))
                {
                    result.Add(t);
                }
            }
            return result;
        }
    }
}