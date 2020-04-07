//using PHUEncode;
using PHUEncode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WYSD
{
    public class WaterService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        JavaScriptSerializer Serializer = new JavaScriptSerializer();
        private readonly string baseUri = ConfigCom.W_BaseUri;
       
        private static readonly object objUpload = new object();
        private static readonly object objUploadQuery = new object();
        /// <summary>
        /// 用水余量查询-批量查询
        /// </summary>
        public void GetWaterVolume()
        {
            try
            {

                ConfigManager cg = new ConfigManager();
                if (cg.W_ID==""||(cg.W_ID!=""&&!DateTimeCom.BoolDateTime(cg.W_TokenExpire,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))) {
                    cg.W_ID = GetToken();
                   // cg.W_TokenExpire = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                    string readUri = ConfigCom.W_ReadUri;
                    RestClient rc = new RestClient(baseUri);
                    string res = rc.Get(readUri + "/" + cg.W_ID);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        WaterAmountModleL model = Serializer.Deserialize<WaterAmountModleL>(res);
                        if (model.ID == cg.W_ID && model.MeterList != null && model.MeterList.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("insert into wy_w_amount(MeterID,UserID,UserName,UserAddress,MeterType, UserRegistTime, UserState, MeterReadingTime,MeterAccflow, MeterSurflow, MeterPayCount, ValveState,CreateDate)values ");
                            DateTime dt = DateTime.Now;
                            foreach (WaterAmountModle item in model.MeterList)
                            {
                                sb.Append("('");
                                sb.Append(item.MeterID?.ToString() + "','");
                                sb.Append(item.UserID?.ToString() + "','");
                                sb.Append(item.UserName?.ToString() + "','");
                                sb.Append(item.UserAddress?.ToString() + "','");
                                sb.Append(item.MeterType?.ToString() + "','");
                                sb.Append(item.UserRegistTime?.ToString() + "','");
                                sb.Append(item.UserState?.ToString() + "','");
                                sb.Append(item.MeterReadingTime?.ToString() + "',");
                                sb.Append(item.MeterAccflow?.ToString() + ",");
                                sb.Append(item.MeterSurflow?.ToString() + ",");
                                sb.Append(item.MeterPayCount?.ToString() + ",'");
                                sb.Append(item.ValveState?.ToString() + "','");
                                sb.Append(dt.ToString("yyyy-MM-dd HH:mm:ss") + "'),");
                            }
                            string sqlInsert = sb.ToString().Substring(0, sb.ToString().Length - 1);
                            string sqlDelete = "delete from wy_w_amount where datediff(day,CreateDate,'" + dt.ToString("yyyy-MM-dd") + "')=0 ";
                            if (SqlHelper.ExcuteNonQuery(sqlDelete) >= 0)
                            {
                                if (SqlHelper.ExcuteNonQuery(sqlInsert) > 0)
                                {
                                    //log记录
                                    log.Info("批量抄水表GetWaterVolume()执行成功！");
                                }
                                else
                                {
                                    log.Error("批量抄水表GetWaterVolume()执行失败！");
                                }
                            }
                        }
                    }
                    else
                    {
                        //log
                        log.Info("批量抄水表GetWaterVolume()http 返回数据为空！");
                    }
                
            }
            catch (Exception ex)
            {
                log.Error("批量抄水表GetWaterVolume()执行失败！" + ex.Message.ToString());
            }


        }
        /// <summary>
        /// 向水表传送充值数据
        /// </summary>

        public void GetWaterPay()
        {
            try
            {
                lock (objUpload)
                {
                    
                    string Uri = ConfigCom.W_UploadUri;
                    RestClient rc = new RestClient(baseUri);
                    string sqlSelect = "select GUID,MeterID,RechargeVolume,AddAmount,UnitPrice FROM wy_w_pay where CStatus<>1 or CStatus is null";
                    DataTable dtSelect = SqlHelper.ExexuteDataTalbe(sqlSelect);
                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                    {
                        List<WaterPayInfoModle> payList = TableToListCom.TableToList<WaterPayInfoModle>(dtSelect);
                        ConfigManager cg = new ConfigManager();
                        if (cg.W_ID == "" || (cg.W_ID != "" && !DateTimeCom.BoolDateTime(cg.W_TokenExpire, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))))
                        {
                            cg.W_ID = GetToken();
                            //cg.W_TokenExpire = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        WaterPayInfoModleL payInfo = new WaterPayInfoModleL() { ID = cg.W_ID, PayList = payList };
                        string questStr = Newtonsoft.Json.JsonConvert.SerializeObject(payInfo);
                        string res = rc.Post(questStr, Uri);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            WaterPayResponseModeL model = Serializer.Deserialize<WaterPayResponseModeL>(res);
                            if (model.Status.ToString().ToUpper() == "TRUE")
                            {
                                StringBuilder sbUpdate = new StringBuilder();
                                DateTime dt = DateTime.Now;
                                List<WaterPayResponseModel> TrueList = (List<WaterPayResponseModel>)model.Data.RceiveList.Where(n => n.Status.ToString().ToUpper() == "TRUE");
                                if (TrueList != null && TrueList.Count > 0)
                                {
                                    sbUpdate.Append("update wy_w_pay set CStatus=1,CUpdateDate='"+dt.ToString("yyyy-MM-dd HH:mm:ss")+"' where DUID IN(");
                                    foreach (WaterPayResponseModel item in TrueList)
                                    {
                                        sbUpdate.Append("'" + item.GUID + "',");
                                    }
                                    string sqlUpdate = sbUpdate.ToString().Substring(0, sbUpdate.ToString().Length - 1);
                                    if (SqlHelper.ExcuteNonQuery(sqlUpdate) > 0)
                                    {
                                        log.Info("update向水表传送充值数据GetWaterPay()返回成功数据,成功！");
                                    }
                                    else
                                    {
                                        log.Error("update向水表传送充值数据GetWaterPay()返回成功数据,失败！");
                                    }
                                }
                                sbUpdate.Length = 0;
                                List<WaterPayResponseModel> FalseList = (List<WaterPayResponseModel>)model.Data.RceiveList.Where(n => n.Status.ToString().ToUpper() != "TRUE");
                                if (FalseList != null && FalseList.Count > 0)
                                {

                                    foreach (WaterPayResponseModel item in FalseList)
                                    {
                                        sbUpdate.Append("update wy_w_pay set CStatus=0 ,CMessage='" + item.Message + "',CUpdateDate='" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'  where DUID =");
                                        sbUpdate.Append("'" + item.GUID + "' ;");
                                    }
                                    if (SqlHelper.ExcuteNonQuery(sbUpdate.ToString()) > 0)
                                    {
                                        log.Info("update向水表传送充值数据GetWaterPay()返回失败数据,成功！");
                                    }
                                    else
                                    {
                                        log.Error("update向水表传送充值数据GetWaterPay()返回失败数据,失败！");
                                    }
                                }
                                //sbUpdate.Append();
                            }
                            else
                            {
                                log.Error("向水表传送充值数据GetWaterPay()返回整体数据失败,失败!Status<>true！");
                            }
                        }
                        else
                        {
                            log.Info("向水表传送充值数据GetWaterPay()http 返回数据为空！");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("向水表传送充值数据GetWaterPay()执行失败！" + ex.Message.ToString());
            }
        }
        public void GetWaterPayState()
        {
            try
            {
                lock (objUploadQuery)
                {
                    string Uri = ConfigCom.W_UploadQueryUri;
                    RestClient rc = new RestClient(baseUri);
                    string sqlSelect = "select GUID,MeterID FROM wy_w_pay where CStatus=1 AND (PStatus<>1 or PStatus is null)";
                    DataTable dtSelect = SqlHelper.ExexuteDataTalbe(sqlSelect);
                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                    {
                        List<WaterPayResQuestModel> payList = TableToListCom.TableToList<WaterPayResQuestModel>(dtSelect);
                        ConfigManager cg = new ConfigManager();
                        if (cg.W_ID == "" || (cg.W_ID != "" && !DateTimeCom.BoolDateTime(cg.W_TokenExpire, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))))
                        {
                            cg.W_ID = GetToken();
                            //cg.W_TokenExpire = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        WaterPayResQuestModelL payInfo = new WaterPayResQuestModelL() { ID = cg.W_ID, ResultList = payList };
                        string questStr = Newtonsoft.Json.JsonConvert.SerializeObject(payInfo);
                        string res = rc.Post(questStr, Uri);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            WaterPayResponseModeL model = Serializer.Deserialize<WaterPayResponseModeL>(res);
                            if (model.Status.ToString().ToUpper() == "TRUE")
                            {
                                StringBuilder sbUpdate = new StringBuilder();
                                DateTime dt = DateTime.Now;
                                List<WaterPayResponseModel> TrueList = (List<WaterPayResponseModel>)model.Data.RceiveList.Where(n => n.Status.ToString().ToUpper() == "TRUE");
                                if (TrueList != null && TrueList.Count > 0)
                                {
                                    sbUpdate.Append("update wy_w_pay set PStatus=1,PUpdateDate='" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "' where DUID IN(");
                                    foreach (WaterPayResponseModel item in TrueList)
                                    {
                                        sbUpdate.Append("'" + item.GUID + "',");
                                    }
                                    string sqlUpdate = sbUpdate.ToString().Substring(0, sbUpdate.ToString().Length - 1);
                                    if (SqlHelper.ExcuteNonQuery(sqlUpdate) > 0)
                                    {
                                        log.Info("update查询充值数据是否充值成功GetWaterPayState()返回成功数据,成功！");
                                        GetWaterVolume();//如果有充值成功得，立即调用批量抄表接口 更新数据
                                    }
                                    else
                                    {
                                        log.Error("update查询充值数据是否充值成功GetWaterPayState()返回成功数据,失败！");
                                    }
                                }
                                sbUpdate.Length = 0;
                                List<WaterPayResponseModel> FalseList = (List<WaterPayResponseModel>)model.Data.RceiveList.Where(n => n.Status.ToString().ToUpper() != "TRUE");
                                if (FalseList != null && FalseList.Count > 0)
                                {

                                    foreach (WaterPayResponseModel item in FalseList)
                                    {
                                        sbUpdate.Append("update wy_w_pay set PStatus=0 ,PMessage='" + item.Message + "',PUpdateDate='" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "' where DUID =");
                                        sbUpdate.Append("'" + item.GUID + "' ;");
                                    }
                                    if (SqlHelper.ExcuteNonQuery(sbUpdate.ToString()) > 0)
                                    {
                                        log.Info("update查询充值数据是否充值成功GetWaterPayState()返回失败数据成功！");
                                    }
                                    else
                                    {
                                        log.Error("update查询充值数据是否充值成功GetWaterPayState()返回失败数据失败！");
                                    }
                                }
                            }
                            else
                            {
                                log.Info("查询充值数据是否充值成功GetWaterPayState()http 返回数据为空！");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("查询充值数据是否充值成功GetWaterPayState()执行失败！" + ex.Message.ToString());
            }
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>

        public string GetToken() {
            try
            {
                string userName = ConfigCom.W_UserName;
                string userPass = ConfigCom.W_UserPass;
                string uri = ConfigCom.W_TokenUri;
                 DataEncryption data = new DataEncryption();
                //加密方法      
                string encryUserName =  data.Encryption(userName);
                string encryUserPass =  data.Encryption(userPass);
                RestClient rc = new RestClient(baseUri);
                string res = rc.Get(uri + "/" + encryUserName + "/" + encryUserPass);
                //验证res
                if (string.IsNullOrWhiteSpace(res))
                {
                    ConfigCom.SetValue("W_ID", res);
                    ConfigCom.SetValue("W_TokenExpire", DateTime.Now.AddMinutes(Convert.ToDouble(ConfigCom.W_TokenExpireInterval)).ToString("yyyy-MM-dd HH:mm:ss"));
                    return res;
                }
                else {
                    log.Error("GetToken()返回值为空" );
                    return "";
                }
               
            }
            catch (Exception ex)
            {
                log.Error("GetToken()出错："+ex.Message.ToString());
                return "false";
            }
        }

        public void TongZhi() {
            try
            {
                string sql = @"select b.FWID,a.MeterAccflow ,c.MOBILE_PHONE,c.OPEN_ID ,1 JFLX,0 JFZT,0 SFTZ,GETDATE() YXQZ
                        from wy_w_amount a
                        join wy_houseinfo b on a.MeterID=b.WATER_NUMBER
                        join wy_shopinfo c on c.CZ_SHID=c.CZ_SHID
                        WHERE DATEDIFF(day, CreateDate,GETDATE() )=0 and 
                        a.MeterAccflow<= (select cast(CONF_VALUE as DECIMAL(18,2)) from ts_uidp_config  where CONF_CODE='Water_Reminder')
                        ";
                DataTable dt = SqlHelper.ExexuteDataTalbe(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("insert into wy_pay_record (RECORD_ID,JFLX,FWID,JFZT,SFTZ,JFCS,YXQZ,SURPLUSVALUE,CZ_SHID,OPEN_ID,CREATE_TIME)values");
                    string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendLine("(  ");
                        sb.Append("'" + Guid.NewGuid().ToString() + "',1,");
                        sb.Append("'" + row["FWID"]?.ToString() + "',0,1");
                        sb.Append("'" + dateNow + "',");
                        sb.Append("'" + row["FWID"]?.ToString() + "',");
                        sb.Append("'" + row["FWID"]?.ToString() + "',");
                        sb.Append("'" + dateNow + "'");
                        sb.AppendLine("),");
                    }
                    string sqlinsert = sb.ToString().Substring(0, sb.Length - 1);
                    if (SqlHelper.ExcuteNonQuery(sqlinsert) > 0)
                    {

                    }
                    else
                    {
                        log.Info("创建水费通知单失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("TongZhi()创建水费通知单失败："+ex.Message.ToString());
            }
           
        }
       
    }
}
