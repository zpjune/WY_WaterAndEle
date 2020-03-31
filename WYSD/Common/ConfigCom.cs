using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
{
    public static class ConfigCom
    {

        #region 电接口配置属性

        /// <summary>
        /// 电接口 授权码
        /// </summary>
        public static string authCode { get; set; }
        /// <summary>
        /// 电接口 授权码
        /// </summary>
        public static string nonce { get; set; }
        /// <summary>
        /// 回调本系统接口ip+端口
        /// </summary>
        public static string EleIP { get; set; }
        /// <summary>
        /// 电接口 读取余额api 接收地址
        /// </summary>
        public static string readRemainMoney { get; set; }
        /// <summary>
        /// 电接口 读取总电能 api 接收地址
        /// </summary>
        public static string ActiveEnergy { get; set; }
        #endregion
        #region 水接口参数配置
        /// <summary>
        /// 获取水token  账号
        /// </summary>
        public static string W_UserName { get; set; }
        /// <summary>
        /// 获取水token  密码
        /// </summary>
        public static string W_UserPass { get; set; }
        /// <summary>
        /// 获取水接口token地址
        /// </summary>
        public static string W_TokenUri { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>

        public static string W_TokenExpireInterval { get; set; }
        /// <summary>
        /// 水接口ip地址 http://ip+端口
        /// </summary>
        public static string W_BaseUri { get; set; }
        /// <summary>
        /// 读取水余量回调接口地址get
        /// </summary>
        public static string W_ReadUri { get; set; }
        /// <summary>
        /// 水充值-上传数据地址post
        /// </summary>
        public static string W_UploadUri { get; set; }
        /// <summary>
        /// 水充值-查询结果地址post
        /// </summary>
        public static string W_UploadQueryUri { get; set; }
        #endregion

        static ConfigCom() {
            #region 电接口参数
            authCode = ConfigurationManager.AppSettings["authCode"].ToString();
            nonce = ConfigurationManager.AppSettings["nonce"].ToString();
            EleIP = ConfigurationManager.AppSettings["EleIP"].ToString();
            readRemainMoney = ConfigurationManager.AppSettings["readRemainMoney"].ToString();
            ActiveEnergy = ConfigurationManager.AppSettings["ActiveEnergy"].ToString();
            #endregion
            #region 水接口参数
           
            W_BaseUri = ConfigurationManager.AppSettings["W_BaseUri"].ToString();
            W_ReadUri = ConfigurationManager.AppSettings["W_ReadUri"].ToString();
            W_UploadUri = ConfigurationManager.AppSettings["W_UploadUri"].ToString();
            W_UploadQueryUri = ConfigurationManager.AppSettings["W_UploadQueryUri"].ToString();
            W_UserName = ConfigurationManager.AppSettings["W_UserName"].ToString();
            W_UserPass = ConfigurationManager.AppSettings["W_UserPass"].ToString();
            W_TokenUri= ConfigurationManager.AppSettings["W_TokenUri"].ToString();
            #endregion
        }
        public static void SetValue(string AppKey, string AppValue)
        {
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根据Key读取<add>元素的Value
            // string name = config.AppSettings.Settings[AppKey].Value;
            //写入<add>元素的Value
            config.AppSettings.Settings[AppKey].Value = AppValue;
            //增加<add>元素            
            //config.AppSettings.Settings.Add("url", "http://www.fx163.net");
            //删除<add>元素
            //  config.AppSettings.Settings.Remove("name");
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
