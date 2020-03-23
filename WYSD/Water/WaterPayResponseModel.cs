using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
{
    /// <summary>
    /// 1上传充值数据到水表接口是否上传成功 和 2查询上传的充值数据是否实际充值成功 两个反回数据类型一样  通用model
    /// </summary>
    public class WaterPayResponseModeL
    {
        /// <summary>
        /// 传送数据请求是否成功 true false
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 成功或者失败信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回每条数据状态，是否上传成功
        /// </summary>
        public Data Data { get; set; }
    }
    public class Data {
        /// <summary>
        /// 客户集成id
        /// </summary>
       public string ID { get; set; }
        public List<WaterPayResponseModel> RceiveList { get; set; }
    }
    public class WaterPayResponseModel
    {
        /// <summary>
        /// 唯一表示
        /// </summary>
         public string GUID  { get;set;}
        /// <summary>
        /// 状态 true 成功 false 失败
        /// </summary>
         public string Status { get;set;}
        /// <summary>
        /// 成功或者失败信息
        /// </summary>
         public string Message {get;set;}

    }

}
