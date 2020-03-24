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
        private string baseUri = ConfigCom.W_BaseUri;
       
        private static object objUpload = new object();
        private static object objUploadQuery = new object();
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
                    cg.W_TokenExpire = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
                    string sqlSelect = "select GUID,MeterID,RechargeVolume,AddAmount,UnitPrice FROM wy_w_pay where CStatus<>1";
                    DataTable dtSelect = SqlHelper.ExexuteDataTalbe(sqlSelect);
                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                    {
                        List<WaterPayInfoModle> payList = TableToListCom.TableToList<WaterPayInfoModle>(dtSelect);
                        ConfigManager cg = new ConfigManager();
                        if (cg.W_ID == "" || (cg.W_ID != "" && !DateTimeCom.BoolDateTime(cg.W_TokenExpire, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))))
                        {
                            cg.W_ID = GetToken();
                            cg.W_TokenExpire = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
                    string sqlSelect = "select GUID,MeterID FROM wy_w_pay where CStatus=1 AND PStatus<>1 ";
                    DataTable dtSelect = SqlHelper.ExexuteDataTalbe(sqlSelect);
                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                    {
                        List<WaterPayResQuestModel> payList = TableToListCom.TableToList<WaterPayResQuestModel>(dtSelect);
                        ConfigManager cg = new ConfigManager();
                        if (cg.W_ID == "" || (cg.W_ID != "" && !DateTimeCom.BoolDateTime(cg.W_TokenExpire, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))))
                        {
                            cg.W_ID = GetToken();
                            cg.W_TokenExpire = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
                string encryUserName = "";//加密
                string encryUserPass = "";//加密
                RestClient rc = new RestClient(baseUri);
                string res = rc.Get(uri + "/" + encryUserName + "/" + encryUserPass);
                //验证res


                ConfigManager mg = new ConfigManager();
                mg.SetValue("W_ID", res);
                mg.SetValue("W_TokenExpire", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                return res;
            }
            catch (Exception ex)
            {
                log.Error("GetToken()出错："+ex.Message.ToString());
                return "false";
            }
        }
       
    }
}
