using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        System.Timers.Timer timer = new System.Timers.Timer();
        public Form1()
        {
            InitializeComponent();
        }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
 
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.label1.Text = "任务正在执行中。。。";
                // startTime();
                // string sql = "update wy_w_pay set MeterID=1 where GUID=1 ;update wy_w_pay set MeterID=2 where GUID=2 ;";
                // int a = SqlHelper.ExcuteNonQuery(sql);
                // string res = "{\"Status\": true,\"Message\": null,\"Data\": {\"ID\": \"123\",\"RceiveList\": [{\"GUID\": \"1239999 \",\"Status\": true,\"Message\": null},{\"GUID\": \"1239900 \",\"Status\":true,\"Message\": null}]}}";
                // JavaScriptSerializer Serializer = new JavaScriptSerializer();
                // WaterPayResponseModeL model = Serializer.Deserialize<WaterPayResponseModeL>(res);
                this.button1.Enabled = false;
                this.button2.Enabled = true;

            }
            catch (Exception EX)
            {
               log.Error("button1_Click（）出错：" + EX.Message);
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
            stopTime();
            this.button1.Enabled = true;
            this.button2.Enabled = false;
        }
        #region 定时任务

       
        private void startTime()
        {
            try
            {
                //设置执行的时间间隔
                timer.Interval = 1000;

                //到达时间的时候执行事件
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

                //是否是一直执行
                timer.AutoReset = true;

                //是否执行
                timer.Enabled = true;
                log.Error("startTime()=====start===========" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception)
            {
                timer.Enabled = false;
                MessageBox.Show("启动失败！");
                return;
            }
        }

        /// <summary>
        /// 结束执行定时
        /// </summary>
        private void stopTime()
        {
            log.Info("stopTime(),关闭定时任务。");
            try
            {
                timer.Stop();
                timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
                timer.Enabled = false;
                this.label1.Text = "点击开始任务按钮，任务即将开始。。。";
                log.Info("stopTime()=====end===========" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                log.Error("自动下载关闭失败！" + ex.Message.ToString());
                return;
            }
        }
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string dtNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ConfigManager mg = new ConfigManager();
            string W_ReadLastDate = Convert.ToDateTime(mg.W_ReadLastDate).AddMinutes(Convert.ToInt32(mg.W_ReadInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            string W_UploadLastDate = Convert.ToDateTime(mg.W_UploadLastDate).AddMinutes(Convert.ToInt32(mg.W_UploadInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            string W_UploadQueryLastDate = Convert.ToDateTime(mg.W_UploadQueryLastDate).AddMinutes(Convert.ToInt32(mg.W_UploadQueryInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            if (dtNow== W_ReadLastDate) {
                //执行批量抄表接口服务
               // WaterService ws = new WaterService();
               // ws.GetWaterVolume();
                mg.SetValue("W_ReadLastDate", W_ReadLastDate);
                SetLableText();
            }
            if (dtNow == W_UploadLastDate)
            {
                //执行上传充值水表数据
               //  WaterService ws = new WaterService();
                 //ws.GetWaterPay();
                mg.SetValue("W_UploadLastDate", W_ReadLastDate);
                SetLableText();
            }
            if (dtNow == W_UploadQueryLastDate)
            {
                //执行查询并更新 上传充值数据是否充值成功
                //WaterService ws = new WaterService();
                //ws.GetWaterPayState();
                mg.SetValue("W_UploadQueryLastDate", W_UploadQueryLastDate);
                SetLableText();
            }
        }
        #endregion
        private void SetLableText() {
            ConfigManager md = new ConfigManager();
            this.lb_w1.Text = md.W_ReadLastDate;
            this.lb_w2.Text = md.W_UploadLastDate;
            this.lb_w3.Text = md.W_UploadQueryLastDate;
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false; //不显示在系统任务栏 
                notifyIcon1.Visible = true; //托盘图标可见 
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
