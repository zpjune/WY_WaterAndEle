using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
{
    /// <summary>
    /// 水表接口传送充值数据
    /// </summary>
  public  class WaterPayInfoModleL
    {
        /// <summary>
        /// 集成客户id
        /// </summary>
       public  string  ID { get; set; }
        public List<WaterPayInfoModle> PayList { get; set; }
    }
    public class WaterPayInfoModle {
        /// <summary>
        /// 每次数据标识
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 水表号
        /// </summary>
        public string MeterID { get; set; }
        /// <summary>
        /// 新增水量
        /// </summary>
        public string RechargeVolume { get; set; }
        /// <summary>
        /// 新增金额
        /// </summary>
        public string AddAmount { get; set; }
    }
}
