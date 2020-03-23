using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD.Water
{
    /// <summary>
    /// 查询上传导水接口的充值数据是否正在充值成功 
    /// </summary>
   public class WaterPayResQuestModelL
    {
        /// <summary>
        /// 集成客户id
        /// </summary>
        public string ID { get; set; }
        public List<WaterPayResQuestModel> ResultList { get; set; }
    }
    public class WaterPayResQuestModel {
        /// <summary>
        /// 每次数据标识
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 水表号
        /// </summary>
        public string MeterID { get; set; }
    }
}
