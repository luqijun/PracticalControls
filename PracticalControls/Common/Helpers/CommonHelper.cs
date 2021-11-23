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

        #region 数字与大写字母互转 用于Excel列名称

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

        #endregion

        #region 进制间相互转换转换

        /// <summary>
        /// 进制转换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        public string ConvertGenericBinary(string input, byte
             fromType, byte toType)
        {
            string output = input;
            switch (fromType)
            {
                case 2:
                    output = ConvertGenericBinaryFromBinary(input, toType);
                    break;
                case 8:
                    output = ConvertGenericBinaryFromOctal(input, toType);
                    break;
                case 10:
                    output = ConvertGenericBinaryFromDecimal(input, toType);
                    break;
                case 16:
                    output = ConvertGenericBinaryFromHexadecimal(input, toType);
                    break;
                default:
                    break;
            }
            return output;
        }

        /// <summary>
        /// 从二进制转换成其他进制
        /// </summary>
        /// <param name="input"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        private string ConvertGenericBinaryFromBinary(string input, byte toType)
        {
            switch (toType)
            {
                case 8:
                    //先转换成十进制然后转八进制
                    input = Convert.ToString(Convert.ToInt32(input, 2), 8);
                    break;
                case 10:
                    input = Convert.ToInt32(input, 2).ToString();
                    break;
                case 16:
                    input = Convert.ToString(Convert.ToInt32(input, 2), 16);
                    break;
                default:
                    break;
            }
            return input;
        }


        /// <summary>
        /// 从八进制转换成其他进制
        /// </summary>
        /// <param name="input"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        private string ConvertGenericBinaryFromOctal(string input, byte toType)
        {
            switch (toType)
            {
                case 2:
                    input = Convert.ToString(Convert.ToInt32(input, 8), 2);
                    break;
                case 10:
                    input = Convert.ToInt32(input, 8).ToString();
                    break;
                case 16:
                    input = Convert.ToString(Convert.ToInt32(input, 8), 16);
                    break;
                default:
                    break;
            }
            return input;
        }

        /// <summary>
        /// 从十进制转换成其他进制
        /// </summary>
        /// <param name="input"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        private string ConvertGenericBinaryFromDecimal(string input, int toType)
        {
            string output = "";
            int intInput = Convert.ToInt32(input);
            switch (toType)
            {
                case 2:
                    output = Convert.ToString(intInput, 2);
                    break;
                case 8:
                    output = Convert.ToString(intInput, 8);
                    break;
                case 16:
                    output = Convert.ToString(intInput, 16);
                    break;
                default:
                    output = input;
                    break;
            }
            return output;
        }

        /// <summary>
        /// 从十六进制转换成其他进制
        /// </summary>
        /// <param name="input"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        private string ConvertGenericBinaryFromHexadecimal(string input, int toType)
        {
            switch (toType)
            {
                case 2:
                    input = Convert.ToString(Convert.ToInt32(input, 16), 2);
                    break;
                case 8:
                    input = Convert.ToString(Convert.ToInt32(input, 16), 8);
                    break;
                case 10:
                    input = Convert.ToInt32(input, 16).ToString();
                    break;
                default:
                    break;
            }
            return input;
        }


        /// <summary>
        /// 二进制之间的加法
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public string AddBetweenBinary(string x, string y)
        {
            int intSum = Convert.ToInt32(x, 2) + Convert.ToInt32(y, 2);
            return Convert.ToString(intSum, 2);
        }
        #endregion

        #region 左右填充

        public string LeftFill(string input, char c, int totallen)
        {
            if (input.Length >= totallen)
                return input;

            string left = new string(c, totallen - input.Length);
            return left + input;
        }

        public string RightFill(string input, char c, int totallen)
        {
            if (input.Length >= totallen)
                return input;

            string left = new string(c, totallen - input.Length);
            return input + left;
        }

        #endregion
    }
}
