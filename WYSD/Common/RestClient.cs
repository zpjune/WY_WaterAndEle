using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
{
   public class RestClient
    {
        private string BaseUri;
        public RestClient(string baseUri)
        {
            this.BaseUri = baseUri;
        }
        #region POST方式实现

        public string Post(string data, string uri)
        {
            return CommonHttpRequest(data, uri, "POST");
        }

        public string CommonHttpRequest(string data, string uri, string type)
        {
            //Web访问对象，构造请求的url地址
            string serviceUrl = string.Format("{0}/{1}", this.BaseUri, uri);

            //构造http请求的对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //转成网络流
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
            //设置
            myRequest.Method = type;
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
           // myRequest.MaximumAutomaticRedirections = ;
            //myRequest.AllowAutoRedirect = true;
            // 发送请求
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf,0 , buf.Length);
            newStream.Close();
            // 获得接口返回值
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string ReturnXml = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReturnXml;
        }
        #endregion

        #region GET方式实现
        public string Get(string uri)
        {
            //Web访问对象64
            string serviceUrl = string.Format("{0}/{1}", this.BaseUri, uri);

            //构造一个Web请求的对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            // 获得接口返回值68
            //获取web请求的响应的内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

            //通过响应流构造一个StreamReader
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
            string ReturnXml = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReturnXml;
        }
        #endregion
        #region ceshi 
        //调用代码demo
        //string serviceUrl = "http://localhost:44540/";
        //RestClient client = new RestClient(serviceUrl);
        //string data = @"{""UserName"":""1111"",""Age"":123,""Id"":133}";
        //string uriPost = "api/values/3";
        //string retPost = client.Put(data, uriPost);
        #endregion
    }
}
