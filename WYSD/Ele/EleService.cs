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
using java.util.stream;

namespace WYSD.Ele
{
    public class EleService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 批量读取电能
        /// </summary>
        public void readActiveEnergyBatch() {
            try
            {
                string sqlQuery = "SELECT ELE_NUMBER,CID FROM wy_houseinfo a where a.FWSX<>0 ";
                DataTable dt = SqlHelper.ExexuteDataTalbe(sqlQuery);
                if (dt != null && dt.Rows.Count > 0)
                {  
                    // 设置sdk的底层http传输日志显示级别
                    LogSetter.setHttp2Debug();
                    // LogSetter.setHttp2Error();

                    // 设置sdk日志显示级别
                    LogSetter.setSdk2Debug();
                    // LogSetter.setSdk2Error();
                    //"http://192.168.1.10:8080/notify"

                    TQApi tqApi = new TQApi(
                            ConfigCom.authCode,
                            ConfigCom.nonce,
                            ConfigCom.EleIP + ConfigCom.ActiveEnergy,
                            SyncMode.enable);
                    java.util.List list = new java.util.ArrayList();
                    foreach (DataRow row in dt.Rows)
                    {
                        java.util.Map map = new java.util.HashMap();
                        map.put("opr_id", CommonUtil.generateOperateId());
                        map.put("address", row["ELE_NUMBER"]?.ToString());//map.put("address", "201908290001");
                        map.put("cid", row["CID"]?.ToString());//map.put("cid", "201908290001");
                        map.put("time_out", tqApi.getTimeOut());
                        map.put("must_online", true);
                        map.put("retry_time", tqApi.getRetryTimes());
                        map.put("type", ReadElecMeterType.PositiveActiveEnergy.getType());//正向总电 能 33
                        list.add(map);
                    }
                    TQResponse tqresponse = tqApi.readElecMeter(list);
                    string stats = tqresponse.getStatus();
                    if (stats == "SUCCESS")
                    {
                        string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //每天只留一条，先删除，在insert
                        string sqlDelete = " delete from wy_ele_Energy where datediff(day,CreateDate,'"+dateNow+"')=0 and Ustatus is not null";
                        int exctR = SqlHelper.ExcuteNonQuery(sqlDelete);
                        if (exctR > 0)
                        {
                            log.Info("readActiveEnergyBatch()Delete成功！");
                        }
                        else
                        {
                            log.Info("readActiveEnergyBatch()delete失败！");
                        }
                        java.util.List listRes = tqresponse.getResponseContent();
                        int total = 0;//如果超过998条要分批插入
                        int count = 0;//几次 998条insert
                        string sqlInsertHead = "insert into wy_ele_Energy(address,cid,opr_id,CreateDate,Cstatus,CMessage)values ";
                        string sqlInsert = sqlInsertHead;
                        for (int i = 0; i < listRes.size(); i++)
                        {
                            total = i;
                            java.util.HashMap mp = (java.util.HashMap)listRes.get(i);//string opr_id=mp.get("opr_id").ToString();
                            if (mp.get("status").ToString() == "SUCCESS")
                            {
                                sqlInsert += "(";
                                for (int k = 0; k < list.size(); k++)
                                {
                                    java.util.HashMap mpl = (java.util.HashMap)list.get(k);
                                    if (mpl.get("opr_id").ToString()== mp.get("opr_id").ToString()) {
                                        sqlInsert += "'"+ mpl.get("address").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("cid").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("opr_id").ToString() + "',";
                                        sqlInsert += "'" + dateNow + "',1,null),";
                                        total++;
                                        break;
                                    }
                                }
                            }
                            else {
                                sqlInsert += "(";
                                for (int k = 0; k < list.size(); k++)
                                {
                                    java.util.HashMap mpl = (java.util.HashMap)list.get(k);
                                    if (mpl.get("opr_id").ToString() == mp.get("opr_id").ToString())
                                    {
                                        sqlInsert += "'" + mpl.get("address").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("cid").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("opr_id").ToString() + "',";
                                        sqlInsert += "'" + dateNow + "',0,'"+ mp.get("error_msg").ToString() + "'),";
                                        total++;
                                        break;
                                    }
                                }
                            }
                            if (total==998) {//查过一千条insert 分批执行。
                                count++;
                                total = 0;
                                sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                                int exct= SqlHelper.ExcuteNonQuery(sqlInsert);
                                if (exct > 0)
                                {
                                    log.Info("readActiveEnergyBatch()insert第" + count + "几次成功！");
                                }
                                else {
                                    log.Info("readActiveEnergyBatch()insert第" + count + "几次时失败！");
                                }
                                sqlInsert = sqlInsertHead;
                            }
                        }
                        if (count==0||(count>0&&count*998< listRes.size())) {
                            sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                            int exct = SqlHelper.ExcuteNonQuery(sqlInsert);
                            if (exct > 0)
                            {
                                log.Info("readActiveEnergyBatch()insert第" + count + "几次成功！");
                            }
                            else
                            {
                                log.Info("readActiveEnergyBatch()insert第" + count + "几次时失败！");
                            }
                        }

                    }
                    else {
                        log.Error("readActiveEnergyBatch()=>tqresponse 返回不是success,是："+ stats+ ",errorMsg:"+tqresponse.getErrorMsg());
                    }
                    #region res说明
                    /*
                    {
       TQResponse {
           status = 'SUCCESS', errorMsg = 'null', responseContent = [{
               opr_id = 1 c05fa34 - fc65 - 41 b2 - 9 c96 - b35f5351db69,
               status = SUCCESS ,error_msg
           }], timestamp = 1585114838, sign = 'ed6c8381d3b448f505516a4f999ecc0f'
       }
   }
   [{
       "opr_id": "e3dfb115-2f07-46eb-a36f-0b6442bb1d1e",
       "resolve_time": "2020-03-27 17:14:02",
       "status": "SUCCESS",
       "data": [{
           "type": 3,
           "value": ["0.00", "0.00", "0.00", "0.00", "0.00"],
           "dsp": "总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh"
       }]
   }]
                    */
                    #endregion


                }
            }
            catch (Exception ex)
            {
                log.Error("批量读取电能接口readActiveEnergyBatch（）出错：" + ex.Message.ToString());
            }

        }
        /// <summary>
        /// 批量读取余额
        /// </summary>
        public void readRemainMoney()
        {
            try
            {
                string sqlQuery = "SELECT ELE_NUMBER,CID FROM wy_houseinfo a where a.FWSX<>0 ";
                DataTable dt = SqlHelper.ExexuteDataTalbe(sqlQuery);
                if (dt != null && dt.Rows.Count > 0)
                {
                    // 设置sdk的底层http传输日志显示级别
                    LogSetter.setHttp2Debug();
                    // LogSetter.setHttp2Error();

                    // 设置sdk日志显示级别
                    LogSetter.setSdk2Debug();
                    // LogSetter.setSdk2Error();
                    //"http://192.168.1.10:8080/notify"

                    TQApi tqApi = new TQApi(
                            ConfigCom.authCode,
                            ConfigCom.nonce,
                            ConfigCom.EleIP + ConfigCom.readRemainMoney,
                            SyncMode.enable);
                    java.util.List list = new java.util.ArrayList();
                    foreach (DataRow row in dt.Rows)
                    {
                        java.util.Map map = new java.util.HashMap();
                        map.put("opr_id", CommonUtil.generateOperateId());
                        map.put("address", row["ELE_NUMBER"]?.ToString());//map.put("address", "201908290001");
                        map.put("cid", row["CID"]?.ToString());//map.put("cid", "201908290001");
                        map.put("time_out", tqApi.getTimeOut());
                        map.put("must_online", true);
                        map.put("retry_time", tqApi.getRetryTimes());
                        map.put("type", ReadElecMeterType.RemainMoney.getType());//读取余额 22
                        list.add(map);
                    }
                    TQResponse tqresponse = tqApi.readElecMeter(list);
                    string stats = tqresponse.getStatus();
                    if (stats == "SUCCESS")
                    {
                        string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //每天只留一条，先删除，在insert
                        string sqlDelete = " delete from wy_ele_Balance where datediff(day,CreateDate,'" + dateNow + "')=0 and Ustatus is not null";
                        int exctR = SqlHelper.ExcuteNonQuery(sqlDelete);
                        if (exctR > 0)
                        {
                            log.Info("readRemainMoney()Delete成功！");
                        }
                        else
                        {
                            log.Info("readRemainMoney()delete失败！");
                        }
                        java.util.List listRes = tqresponse.getResponseContent();
                        int total = 0;//如果超过998条要分批插入
                        int count = 0;//几次 998条insert
                        string sqlInsertHead = "insert into wy_ele_Balance(address,cid,opr_id,CreateDate,Cstatus,CMessage)values ";
                        string sqlInsert = sqlInsertHead;
                        for (int i = 0; i < listRes.size(); i++)
                        {
                            total = i;
                            java.util.HashMap mp = (java.util.HashMap)listRes.get(i);//string opr_id=mp.get("opr_id").ToString();
                            if (mp.get("status").ToString() == "SUCCESS")
                            {
                                sqlInsert += "(";
                                for (int k = 0; k < list.size(); k++)
                                {
                                    java.util.HashMap mpl = (java.util.HashMap)list.get(k);
                                    if (mpl.get("opr_id").ToString() == mp.get("opr_id").ToString())
                                    {
                                        sqlInsert += "'" + mpl.get("address").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("cid").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("opr_id").ToString() + "',";
                                        sqlInsert += "'" + dateNow + "',1,null),";
                                        total++;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                sqlInsert += "(";
                                for (int k = 0; k < list.size(); k++)
                                {
                                    java.util.HashMap mpl = (java.util.HashMap)list.get(k);
                                    if (mpl.get("opr_id").ToString() == mp.get("opr_id").ToString())
                                    {
                                        sqlInsert += "'" + mpl.get("address").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("cid").ToString() + "',";
                                        sqlInsert += "'" + mpl.get("opr_id").ToString() + "',";
                                        sqlInsert += "'" + dateNow + "',0,'" + mp.get("error_msg").ToString() + "'),";
                                        total++;
                                        break;
                                    }
                                }
                            }
                            if (total == 998)
                            {//查过一千条insert 分批执行。
                                count++;
                                total = 0;
                                sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                                int exct = SqlHelper.ExcuteNonQuery(sqlInsert);
                                if (exct > 0)
                                {
                                    log.Info("readRemainMoney()insert第" + count + "几次成功！");
                                }
                                else
                                {
                                    log.Info("readRemainMoney()insert第" + count + "几次时失败！");
                                }
                                sqlInsert = sqlInsertHead;
                            }
                        }
                        if (count == 0 || (count > 0 && count * 998 < listRes.size()))
                        {
                            sqlInsert = sqlInsert.Substring(0, sqlInsert.Length - 1);
                            int exct = SqlHelper.ExcuteNonQuery(sqlInsert);
                            if (exct > 0)
                            {
                                log.Info("readRemainMoney()insert第" + count + "几次成功！");
                            }
                            else
                            {
                                log.Info("readRemainMoney()insert第" + count + "几次时失败！");
                            }
                        }

                    }
                    else
                    {
                        log.Error("readRemainMoney()=>tqresponse 返回不是success,是：" + stats + ",errorMsg:" + tqresponse.getErrorMsg());
                    }
                    #region res说明
                    /*
                    {
       TQResponse {
           status = 'SUCCESS', errorMsg = 'null', responseContent = [{
               opr_id = 1 c05fa34 - fc65 - 41 b2 - 9 c96 - b35f5351db69,
               status = SUCCESS ,error_msg
           }], timestamp = 1585114838, sign = 'ed6c8381d3b448f505516a4f999ecc0f'
       }
   }
   [{
       "opr_id": "e3dfb115-2f07-46eb-a36f-0b6442bb1d1e",
       "resolve_time": "2020-03-27 17:14:02",
       "status": "SUCCESS",
       "data": [{
           "type": 3,
           "value": ["0.00", "0.00", "0.00", "0.00", "0.00"],
           "dsp": "总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh"
       }]
   }]
                    */
                    #endregion


                }
            }
            catch (Exception ex)
            {
                log.Error("批量读取电能接口readActiveEnergyBatch（）出错：" + ex.Message.ToString());
            }

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
    /// 批量充值
    /// </summary>
    public void ElecMeterRechargeBatch()
    {

    }
}
}
