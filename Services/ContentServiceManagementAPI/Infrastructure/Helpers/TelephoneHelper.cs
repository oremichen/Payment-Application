
namespace ContentServiceManagementAPI.Infrastructure.Helpers
{
    public static class TelephoneHelper
    {
        private static string PhoneNumberFormated = "";

        public static string FormatPhone(string phone)
        {
            string firstBlock = "";
            string secondBlock = "";
            string lastBlock = "";
            string extra = "";

            if (phone.Length <= 6)
            {
                firstBlock = phone.Substring(0, 2);
                secondBlock = phone.Substring(2, 2);
                lastBlock = phone.Substring(3);

                PhoneNumberFormated = $"{firstBlock}-{lastBlock}";
                return PhoneNumberFormated;
            }

            else if (phone.Length > 6 && phone.Length  <= 9)
            {
                firstBlock = phone.Substring(0, 3);
                secondBlock = phone.Substring(3, 3);
                lastBlock = phone.Substring(6);

                PhoneNumberFormated = $"{firstBlock}-{secondBlock}-{lastBlock}";
                return PhoneNumberFormated;
            }

            else if(phone.Length > 9 && phone.Length <= 11)
            {
                firstBlock = phone.Substring(0, 3);
                secondBlock = phone.Substring(3, 4);
                lastBlock = phone.Substring(7, phone.Length -7);

                PhoneNumberFormated = $"{firstBlock}-{secondBlock}-{lastBlock}";
                return PhoneNumberFormated;
            }

            firstBlock = phone.Substring(0, 3);
            secondBlock = phone.Substring(3, 4);
            lastBlock = phone.Substring(6,4);
            extra = phone.Substring(7);

            PhoneNumberFormated = $"{firstBlock}-{secondBlock}-{lastBlock}-{extra}";
            return PhoneNumberFormated;
        }

        private static string FormatInternationalwithContryCode(string phone)
        {
            return PhoneNumberFormated;
        }     

    }
}
