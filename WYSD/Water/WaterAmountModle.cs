using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
{
    /// <summary>
    /// 响应数据，批量获取水表数据
    /// </summary>
    public  class WaterAmountModleL{
        public string ID { get; set; }
        public List<WaterAmountModle> MeterList { get; set; }
}
   public class WaterAmountModle
    {
        /// <summary>
        /// 水表编号
        /// </summary>
        public string MeterID { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 用户名称（商铺名称）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string UserAddress { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public string MeterType { get; set; }
        /// <summary>
        /// 开户时间
        /// </summary>
        public string UserRegistTime { get; set; }
        /// <summary>
        /// 用户状态
        /// </summary>
        public string UserState { get; set; }
        /// <summary>
        /// 抄表时间
        /// </summary>
        public string MeterReadingTime { get; set; }
        /// <summary>
        /// 水表读数（剩余水量）
        /// </summary>
        public string MeterAccflow { get; set; }
        /// <summary>
        /// 水表累计用水量
        /// </summary>
        public string MeterSurflow { get; set; }
        /// <summary>
        /// 充值次数
        /// </summary>
        public string MeterPayCount { get; set; }
        /// <summary>
        /// 阀门状态
        /// </summary>
        public string ValveState { get; set; }
    }
        
}

