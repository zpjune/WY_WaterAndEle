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
        /// 电接口 授权码
        /// </summary>
        public static string readRemainMoney { get; set; }
        /// <summary>
        /// 电接口 授权码
        /// </summary>
        public static string readRemainEle { get; set; }
        #endregion
        #region 水接口参数配置
        /// <summary>
        /// 集成客户编号 id(http://IP:port/api/Account/GetWaterVolume/{id})
        /// </summary>
        public static string W_ID { get; set; }
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
            readRemainEle = ConfigurationManager.AppSettings["readRemainEle"].ToString();
            #endregion
            #region 水接口参数
            W_ID = ConfigurationManager.AppSettings["W_ID"].ToString();
            W_BaseUri = ConfigurationManager.AppSettings["W_BaseUri"].ToString();
            W_ReadUri = ConfigurationManager.AppSettings["W_ReadUri"].ToString();
            W_UploadUri = ConfigurationManager.AppSettings["W_UploadUri"].ToString();
            W_UploadQueryUri = ConfigurationManager.AppSettings["W_UploadQueryUri"].ToString();
            #endregion
        }
    }
}
