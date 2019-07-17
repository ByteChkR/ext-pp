using System;
using ext_pp_base;

namespace ext_pp_base
{
    public static class EnumParser
    {


        public static int Parse(Type enu, string input)
        {
            if (input.IsAllDigits())
            {
                return int.Parse(input);
            }

            if (!enu.IsEnum) return 0;

            int ret = -1;

            string[] ands = input.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var and in ands)
            {
                string[] ors = and.Split("|", StringSplitOptions.RemoveEmptyEntries);
                int r = -1;
                foreach (var or in ors)
                {
                    string enumStr = or.Trim();
                    if (r == -1) r = (int)Enum.Parse(enu, enumStr);
                    else r |= (int)Enum.Parse(enu, enumStr);
                }

                if (ret == -1) ret = r;
                else ret &= r;
            }

            return ret;
        }



    }
}