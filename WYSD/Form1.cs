using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using com.tqdq.sdk.api;
using com.tqdq.sdk.enums;
using com.tqdq.sdk.helper;
using com.tqdq.sdk.log;
using com.tqdq.sdk.parameters;
using java.util;
using java.util.stream;
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


                string aa = DateTime.Now.DateToString();
                WaterService w = new WaterService();
                w.GetToken();
                 //EleService sd = new EleService();
                // sd.ElecMeterRechargeBatch();
                // sd.readRemainMoney();
                //startTime();
                // string sql = "update wy_w_pay set MeterID=1 where GUID=1 ;update wy_w_pay set MeterID=2 where GUID=2 ;";
                // int a = SqlHelper.ExcuteNonQuery(sql);
                // string res = "{\"Status\": true,\"Message\": null,\"Data\": {\"ID\": \"123\",\"RceiveList\": [{\"GUID\": \"1239999 \",\"Status\": true,\"Message\": null},{\"GUID\": \"1239900 \",\"Status\":true,\"Message\": null}]}}";
                // JavaScriptSerializer Serializer = new JavaScriptSerializer();
                // WaterPayResponseModeL model = Serializer.Deserialize<WaterPayResponseModeL>(res);
               // LogSetter.setHttp2Debug();
               // TQApi tqApi = new TQApi(
               //            ConfigCom.authCode,
               //            ConfigCom.nonce,
               //            ConfigCom.EleIP + ConfigCom.readRemainMoney,
               //            SyncMode.enable);
               // java.util.List list = new java.util.ArrayList();

               // java.util.Map map = new java.util.HashMap();
               // map.put("opr_id", CommonUtil.generateOperateId());
               // map.put("address", "201908290001");
               // map.put("cid", "201908290001");
               // map.put("time_out", tqApi.getTimeOut());
               // map.put("must_online", true);
               // map.put("retry_time", tqApi.getRetryTimes());
               // map.put("type", ReadElecMeterType.RemainMoney.getType());//正向总电 能
               // ElecMeterAccount elecMeterAccount = new ElecMeterAccount("121534", "10", "");
               // map.put("params", elecMeterAccount.getAccount());
               // list.add(map);
               //TQResponse tqresponse = tqApi.elecMeterOpenAccount(list);//开户
               // TQResponse tqresponse = tqApi.elecMeterReset(list);//清零
               // TQResponse tqresponse = tqApi.elecMeterRecharge(list);//充值
               //TQResponse tqresponse = tqApi.readElecMeter(list);//余额
                this.button1.Enabled = false;
                this.button2.Enabled = true;

            }
            catch (Exception EX)
            {
               log.Error("button1_Click（）出错：" + EX.Message);
            }
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
                log.Info("startTime()=====start===========" );
            }
            catch (Exception ex)
            {
                timer.Enabled = false;
                log.Info("startTime()=====start===========" + ex.Message.ToString());
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
                log.Info("stopTime()=====end===========");
            }
            catch (Exception ex)
            {
                log.Error("自动下载关闭报错！" + ex.Message.ToString());
                return;
            }
        }
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string dtNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ConfigManager mg = new ConfigManager();
            string W_ReadLastDate = Convert.ToDateTime(mg.W_ReadLastDate).AddMinutes(Convert.ToDouble(mg.W_ReadInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            // string W_UploadLastDate = Convert.ToDateTime(mg.W_UploadLastDate).AddMinutes(Convert.ToDouble(mg.W_UploadInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            // string W_UploadQueryLastDate = Convert.ToDateTime(mg.W_UploadQueryLastDate).AddMinutes(Convert.ToDouble(mg.W_UploadQueryInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            string E_ActiveEnergyQueryLastDate = Convert.ToDateTime(mg.E_ActiveEnergyQueryLastDate).AddMinutes(Convert.ToDouble(mg.E_ActiveEnergyQueryInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            string E_readRemainMoneyLastDate = Convert.ToDateTime(mg.E_readRemainMoneyLastDate).AddMinutes(Convert.ToDouble(mg.E_readRemainMoneyInterval)).ToString("yyyy-MM-dd HH:mm:ss");
            string E_rechargeEleLastDate = Convert.ToDateTime(mg.E_rechargeEleLastDate).AddMinutes(Convert.ToDouble(mg.E_rechargeEleInterval)).ToString("yyyy-MM-dd HH:mm:ss");

            if (dtNow== W_ReadLastDate) {
                mg.SetValue("W_ReadLastDate", W_ReadLastDate);
                SetLableText();
                //执行批量抄表接口服务
                // WaterService ws = new WaterService();
                // ws.GetWaterVolume();

            }
            //if (dtNow == W_UploadLastDate)
            //{
            //    mg.SetValue("W_UploadLastDate", W_ReadLastDate);
            //    SetLableText();
            //    //执行上传充值水表数据
            //   //  WaterService ws = new WaterService();
            //     //ws.GetWaterPay();

            //}
            //if (dtNow == W_UploadQueryLastDate)
            //{
            //    mg.SetValue("W_UploadQueryLastDate", W_UploadQueryLastDate);
            //    SetLableText();
            //    //执行查询并更新 上传充值数据是否充值成功
            //    //WaterService ws = new WaterService();
            //    //ws.GetWaterPayState();

            //}
            if (dtNow == E_ActiveEnergyQueryLastDate)
            {
                mg.SetValue("E_ActiveEnergyQueryLastDate", E_ActiveEnergyQueryLastDate);
                SetLableText();
                //执行批量读取总电能
                EleService es = new EleService();
                es.ReadActiveEnergyBatch();
            }
            if (dtNow == E_readRemainMoneyLastDate)
            {
                mg.SetValue("E_readRemainMoneyLastDate", E_readRemainMoneyLastDate);
                SetLableText();
                //执行批量读取电余额
                EleService es = new EleService();
                es.ReadRemainMoneyBatch();
            }
            if (dtNow == E_rechargeEleLastDate)
            {
                mg.SetValue("E_rechargeEleLastDate", E_rechargeEleLastDate);
                SetLableText();
                //执行批量充值电
                EleService es = new EleService();
                es.ElecMeterRechargeBatch();
            }
        }
        #endregion
        private void SetLableText() {
            try
            {
                ConfigManager md = new ConfigManager();
                new Thread(() =>
                {
                    Action action = () =>
                    {
                        this.lb_w1.Text = md.W_ReadLastDate;
                        this.lb_w2.Text = md.W_UploadLastDate;
                        this.lb_w3.Text = md.W_UploadQueryLastDate;
                        this.lb_ele_enger.Text = md.E_ActiveEnergyQueryLastDate;
                        this.lb_ele_remain.Text = md.E_readRemainMoneyLastDate;
                        this.lb_ele_recharge.Text = md.E_rechargeEleLastDate;
                    };
                    Invoke(action);

                }).Start();
            }
            catch (Exception ex)
            {
                log.Error("SetLableText()报错："+ex.Message.ToString());
            }
        }
        private void SetAppConfigDate() {
            try
            {
                ConfigCom.SetValue("W_ReadLastDate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ConfigCom.SetValue("W_UploadLastDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ConfigCom.SetValue("W_UploadQueryLastDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ConfigCom.SetValue("E_ActiveEnergyQueryLastDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ConfigCom.SetValue("E_readRemainMoneyLastDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ConfigCom.SetValue("E_rechargeEleLastDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                log.Error("SetAppConfigDate()报错：" + ex.Message.ToString());
            }
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

        private void Form1_Load(object sender, EventArgs e)
        {
            SetAppConfigDate();
            SetLableText();
        }
    }
}
