using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ybinterface_auto
{
    /// <summary>
    /// Encrypt 的摘要说明。
    /// </summary>
    public class DEncrypt
    {
        #region 使用 给定密钥字符串 加密/解密string
        /// <summary>
        /// 使用给定密钥字符串加密string
        /// </summary>
        /// <param name="original">原始文字</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码方案</param>
        /// <returns>密文</returns>
        public static string Encrypt(string original, string key)
        {
            try
            {
                byte[] buff = Encoding.Default.GetBytes(original);
                byte[] kb = Encoding.Default.GetBytes(key);
                return Convert.ToBase64String(Encrypt(buff, kb));
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 使用给定密钥字符串解密string
        /// </summary>
        /// <param name="original">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string Decrypt(string original, string key)
        {
            try
            {
                return Decrypt(original, key, Encoding.Default);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 使用给定密钥字符串解密string,返回指定编码方式明文
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码方案</param>
        /// <returns>明文</returns>
        public static string Decrypt(string encrypted, string key, Encoding encoding)
        {
            try
            {
                byte[] buff = Convert.FromBase64String(encrypted);
                byte[] kb = Encoding.Default.GetBytes(key);
                return encoding.GetString(Decrypt(buff, kb));
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
        #endregion

        /// <summary>
        /// 生成MD5摘要
        /// </summary>
        /// <param name="original">数据源</param>
        /// <returns>摘要</returns>
        public static byte[] MakeMD5(byte[] original)
        {
            try
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                byte[] keyhash = hashmd5.ComputeHash(original);
                hashmd5 = null;
                return keyhash;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 使用给定密钥加密
        /// </summary>
        /// <param name="original">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public static byte[] Encrypt(byte[] original, byte[] key)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = MakeMD5(key);
                des.Mode = CipherMode.ECB;
                return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 使用给定密钥解密数据
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = MakeMD5(key);
                des.Mode = CipherMode.ECB;
                return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
    }
}
