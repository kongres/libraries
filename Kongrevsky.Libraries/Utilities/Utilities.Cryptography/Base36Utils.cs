namespace Utilities.Cryptography
{
    #region << Using >>

    using System;
    using System.Collections.Generic;

    #endregion

    public static class Base36Utils
    {
        #region Constants

        private const int BASE36_LENGTH_BLOC_SIZE_36 = 6;

        private const int BASE36_BLOC_SIZE_36 = 13; //Encode36(ulong.MaxValue).Length;

        #endregion

        #region Public methods

        /// <summary>
        /// Convert byte[] to Base36String
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteArrayToBase36String(byte[] bytes)
        {
            var result = Encode36((ulong)bytes.Length).PadLeft(BASE36_LENGTH_BLOC_SIZE_36, '0');

            if (bytes.Length <= 0)
                return result;

            var bytesList = SplitBytes(bytes, 8);
            if (bytesList[bytesList.Count - 1].Length < 8)
            {
                var newLastArray = new byte[8];
                bytesList[bytesList.Count - 1].CopyTo(newLastArray, 0);
                bytesList[bytesList.Count - 1] = newLastArray;
            }

            foreach (var byteArray in bytesList)
            {
                //for (int i = 0; i < byteArray.Length; i++) value = value * 256 + byteArray[i];
                var value = BitConverter.ToUInt64(byteArray, 0);
                result = result + Encode36(value).PadLeft(BASE36_BLOC_SIZE_36, '0');
            }

            return result;
        }

        /// <summary>
        /// Convert Base36String to byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Base36StringToByteArray(string input)
        {
            var result = new byte[0];

            if (input.Length < BASE36_LENGTH_BLOC_SIZE_36)
                return result;

            var arrayLength = (int)Decode36(input.Substring(0, BASE36_LENGTH_BLOC_SIZE_36));
            var data = input.Remove(0, BASE36_LENGTH_BLOC_SIZE_36);
            var bytesList = new List<byte[]>();
            foreach (var value36 in new List<string>(SplitStringByLength(data, BASE36_BLOC_SIZE_36)))
            {
                var byteArray = BitConverter.GetBytes(Decode36(value36));
                bytesList.Add(byteArray);
            }

            result = JoinBytes(bytesList);
            Array.Resize(ref result, arrayLength);

            return result;
        }

        #endregion

        #region Private methods

        private static string _CharList36 = string.Empty;

        private static string CharList36
        {
            get
            {
                if (_CharList36.Length >= 36)
                    return _CharList36;

                var array = new char[36];
                for (var i = 0; i < 10; i++) array[i] = (char)(i + 48);
                for (var i = 0; i < 26; i++) array[i + 10] = (char)(i + 97);
                _CharList36 = new string(array);

                return _CharList36;
            }
        }

        private static List<string> SplitStringByLength(string str, int chunkSize)
        {
            var list = new List<string>();
            int i;
            for (i = 0; i < str.Length / chunkSize; i++)
                list.Add(str.Substring(i * chunkSize, chunkSize));

            i = i * chunkSize;
            if (i < str.Length - 1)
                list.Add(str.Substring(i, str.Length - i));

            return list;
        }

        private static string Encode36(ulong input)
        {
            var clistarr = CharList36.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(clistarr[input % 36]);
                input /= 36;
            }

            return new string(result.ToArray()).ToUpper();
        }

        private static ulong Decode36(string input)
        {
            var reversed = ReverseString(input.ToLower());
            ulong result = 0;
            var pos = 0;
            foreach (var c in reversed)
            {
                result += (ulong)CharList36.IndexOf(c) * (ulong)Math.Pow(36, pos);
                pos++;
            }

            return result;
        }

        private static string ReverseString(string text)
        {
            var cArray = text.ToCharArray();
            for (var i = 0; i < cArray.Length / 2; i++)
            {
                var c = cArray[i];
                cArray[i] = cArray[cArray.Length - 1 - i];
                cArray[cArray.Length - 1 - i] = c;
            }

            return new string(cArray);
        }

        private static byte[] StringToBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static List<byte[]> SplitBytes(byte[] bytes, int length)
        {
            var result = new List<byte[]>();

            var position = 0;
            while (bytes.Length - position > length)
            {
                var temp = new byte[length];
                for (var i = 0; i < temp.Length; i++) temp[i] = bytes[i + position];
                position += length;
                result.Add(temp);
            }

            if (position < bytes.Length)
            {
                var temp = new byte[bytes.Length - position];
                for (var i = 0; i + position < bytes.Length; i++) temp[i] = bytes[i + position];
                result.Add(temp);
            }

            return result;
        }

        private static string BytesToString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private static byte[] JoinBytes(List<byte[]> listBytes)
        {
            var totalLength = 0;
            foreach (var bytes in listBytes) totalLength += bytes.Length;
            var result = new byte[totalLength];
            var position = 0;
            foreach (var bytes in listBytes)
                foreach (var t in bytes)
                {
                    result[position] = t;
                    position++;
                }

            return result;
        }

        #endregion
    }
}