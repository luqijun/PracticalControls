using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Common.Helpers
{
    public class CommonHelper
    {
        private static CommonHelper _instance;

        public static CommonHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommonHelper();
                return _instance;
            }
        }

        public string NumberToSystem26(int n)
        {
            string s = string.Empty;
            n++;
            while (n > 0)
            {
                int m = n % 26;
                if (m == 0) m = 26;
                s = (char)(m + 64) + s;
                n = (n - m) / 26;
            }

            return s;
        }

        public int System26ToNumber(string s)
        {
            int r = 0;
            for (int i = 0; i < s.Length; i++)
            {
                r = r * 26 + s[i] - 'A' + 1;
            }
            return r;
        }
    }
}
