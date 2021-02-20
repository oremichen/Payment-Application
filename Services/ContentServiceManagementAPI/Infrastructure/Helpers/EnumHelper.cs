using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ContentServiceManagementAPI.Infrastructure.Helpers
{

    public class EnumResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class EnumHelper
    {
        // Get All with Description Name of EnumResult Type
        public static List<EnumResult> GetEnumResults<T>()
        {
            var enumList = new List<EnumResult>();          
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                enumList.Add(new EnumResult()
                {                 
                    Id = (int)item,
                    Name = GetDescription<T>((T)item).ToString(),
                });
            }
            return enumList;
        }

        //Get Single Enum value with Description Name of EnumResult Type
        public static EnumResult GetEnumResultByEnumId<T>(int id)
        {
            var enumResult = new EnumResult();
            if (typeof(T).IsEnumDefined(id))
            {
                var response = (T)Enum.Parse(typeof(T), id.ToString());

                return new EnumResult() { Id = id , Name = GetDescription(response)};
            }
            return null;
        }
        // get an Enum value with description when an enum object is passed
        public static EnumResult GetEnumResultByEnumObject<T>(Enum value)
        {
            var enumResult = new EnumResult();
            int realValue = (int)Enum.Parse(value.GetType(), value.ToString()); ;

            if (typeof(T).IsEnumDefined(value))
            {
                return new EnumResult() { Id = realValue, Name = GetDescription(value) };
            }
            return null;
        }

        // Get the enum decription value by enum value 
        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }

        // Get the Description of the enum value by Generic T enum value
        public static string GetDescription<T>(T value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }
        public static T GetSingleValue<T>(int id)
        {
            if (typeof(T).IsEnumDefined(id))
            {
                var response = (T)Enum.Parse(typeof(T), id.ToString());

                return response;
            }
            return default(T);
        }


        // This returns REadony list of all Enum values based on the enum type if generic
        private static IReadOnlyList<T> GetValuesOfEnum<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
        private static int Getcount <T>()
        {
           return GetValuesOfEnum<T>().Count();
        }

        // Single value result of enum passed 
       
        internal static EnumResult GetEnumResultByEnumObject<T>(EnumResult contentPrivacyType)
        {
            throw new NotImplementedException();
        }
    }
}
