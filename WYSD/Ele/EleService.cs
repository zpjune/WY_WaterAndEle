using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tqdq.sdk.api;
using com.tqdq.sdk.enums;
using com.tqdq.sdk.helper;
using com.tqdq.sdk.log;
using com.tqdq.sdk.parameters;


namespace WYSD.Ele
{
    public class EleService
    {
        /// <summary>
        /// 读取余额金额
        /// </summary>
        public void readRemainMoney() {

           // DataTable dt = SqlHelper.ExexuteDataTalbe("select top 10 * from ts_uidp_org");
            // 设置sdk的底层http传输日志显示级别
            LogSetter.setHttp2Debug();
            // LogSetter.setHttp2Error();

            // 设置sdk日志显示级别
            LogSetter.setSdk2Debug();
            // LogSetter.setSdk2Error();
            //"http://192.168.1.10:8080/notify",
            TQApi tqApi = new TQApi(
                ConfigCom.authCode,
                ConfigCom.nonce,
                ConfigCom.EleIP + ConfigCom.readRemainMoney,
                SyncMode.enable);

            GeneralOperate generalOperate = new GeneralOperate(tqApi);

            // 添加采集器
            generalOperate.addController("111");
            // 批量读取电表的正向有功总电能
            TQResponse tqResponse2= generalOperate.readTotalPositiveActiveEnergyBatch(getIdPairs());

           // TriphaseMultipricePrepaidStepMeter meter = new TriphaseMultipricePrepaidStepMeter("000000000003", "12345678901", tqApi);
           // TQResponse tqResponse = meter.readRemainMoney();

        }
        private static java.util.List getIdPairs()
        {
            java.util.List list = new java.util.LinkedList();
            
            
            list.add(new MeterIdPair("202003160000", "202003160000"));
            list.add(new MeterIdPair("123", "3423"));
            list.add(new MeterIdPair("1122", "3223"));

            return list;
        }
        /// <summary>
        /// 读取当前电能 度数
        /// </summary>
        public void readRemainEle ()
        {

        }
    }
}
