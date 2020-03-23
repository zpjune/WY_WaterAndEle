using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using com.tqdq.sdk.api;
using com.tqdq.sdk.enums;
using com.tqdq.sdk.helper;
using com.tqdq.sdk.log;
using com.tqdq.sdk.parameters;
using log4net;
using WYSD.Ele;

namespace WYSD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                string sql = "update wy_w_pay set MeterID=1 where GUID=1 ;update wy_w_pay set MeterID=2 where GUID=2 ;";
                int a = SqlHelper.ExcuteNonQuery(sql);
               // string res = "{\"Status\": true,\"Message\": null,\"Data\": {\"ID\": \"123\",\"RceiveList\": [{\"GUID\": \"1239999 \",\"Status\": true,\"Message\": null},{\"GUID\": \"1239900 \",\"Status\":true,\"Message\": null}]}}";
               // JavaScriptSerializer Serializer = new JavaScriptSerializer();
               // WaterPayResponseModeL model = Serializer.Deserialize<WaterPayResponseModeL>(res);

            }
            catch (Exception EX)
            {
               // LHSM.Logs.Log.Logger.Error("定时DoTimer（）出错："+EX.Message);
            }
          


           // EleService s = new EleService();
          //  s.readRemainMoney();
            //// 设置sdk的底层http传输日志显示级别
            //LogSetter.setHttp2Debug();
            //// LogSetter.setHttp2Error();

            //// 设置sdk日志显示级别
            //LogSetter.setSdk2Debug();
            //// LogSetter.setSdk2Error();

            //TQApi tqApi = new TQApi(
            //    "28ee95766b5ce43f931710d03e2c5028",
            //    "1UxoNXlwHQOg4GxlS20y8vDEcVu",
            //    "http://192.168.1.10:8080/notify",
            //    SyncMode.enable);

            //GeneralOperate generalOperate = new GeneralOperate(tqApi);

            //// 添加采集器
            //generalOperate.addController("111");
            //// 批量读取电表的正向有功总电能
            //generalOperate.readTotalPositiveActiveEnergyBatch(getIdPairs());

            //// 构造Lora水表辅助对象
            //LoraWaterMeter meter = new LoraWaterMeter("20190813000004", "100000000032", tqApi);

            //// 读水表水价
            //meter.readPrice();
        }

        private static java.util.List getIdPairs()
        {
            java.util.List list = new java.util.LinkedList();
            list.add(new MeterIdPair("202003160000", "202003160000"));
            list.add(new MeterIdPair("123", "3423"));
            list.add(new MeterIdPair("1122", "3223"));

            return list;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            log.Debug("调试信息");
            log.Info("调试信息log.Debug(");
        }
    }
}
