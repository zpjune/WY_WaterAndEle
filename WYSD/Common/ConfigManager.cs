using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
{
    public class ConfigManager
    {
        public ConfigManager()
        {
            W_ID = ConfigurationManager.AppSettings["W_ID"].ToString();
            W_TokenExpire = ConfigurationManager.AppSettings["W_TokenExpire"].ToString();
            W_ReadInterval = ConfigurationManager.AppSettings["W_ReadInterval"].ToString();
            W_ReadLastDate = ConfigurationManager.AppSettings["W_ReadLastDate"].ToString();
            W_UploadInterval = ConfigurationManager.AppSettings["W_UploadInterval"].ToString();
            W_UploadLastDate = ConfigurationManager.AppSettings["W_UploadLastDate"].ToString();
            W_UploadQueryInterval = ConfigurationManager.AppSettings["W_UploadQueryInterval"].ToString();
            W_UploadQueryLastDate = ConfigurationManager.AppSettings["W_UploadQueryLastDate"].ToString();

        }
        #region 水接口定时任务时间 间隔 配置
        /// <summary>
        /// 水接口token 也是集成客户id
        /// </summary>
        public string W_ID { get; set; }
        /// <summary>
        /// 过期时间 yyyy-MM-dd HH:mm:ss
         /// </summary>
        public string W_TokenExpire {get;set;}
        /// <summary>
        /// 批量抄表 时间间隔 一分钟为单位
        /// </summary>
        public string W_ReadInterval { get; set; }
        /// <summary>
        /// 上次抄表执行时间
        /// </summary>
        public string W_ReadLastDate { get; set; }
        /// <summary>
        /// 上传数据时间间隔
        /// </summary>
        public string W_UploadInterval { get; set; }
        /// <summary>
        /// 上次上传充值数据时间
        /// </summary>
        public string W_UploadLastDate { get; set; }
        /// <summary>
        /// 查询是否充值成功 执行时间间隔
        /// </summary>
        public string W_UploadQueryInterval { get; set; }
        /// <summary>
        /// 上次执行查询充值是否真正充值成功 时间
        /// </summary>
        public string W_UploadQueryLastDate { get; set; }
        

        #endregion
        public  void SetValue(string AppKey, string AppValue)
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
