using System;
using System.Collections.Generic;
using System.Reflection;

namespace ContentManagementServiceAPI.Utilities
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        public Enumeration() { }

        public Enumeration(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object other)
        {
            return Id.CompareTo(((Enumeration)other).Id);
        }

        public override bool Equals(object obj)
        {
            var othervalue = obj as Enumeration;
            if (othervalue == null)
                return false;
            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(othervalue.Id);

            return typeMatches && valueMatches;
        }
        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetTypeInfo().GetFields(BindingFlags.Public |
                                                      BindingFlags.Static |
                                                      BindingFlags.DeclaredOnly);
            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }
      

    }
}
