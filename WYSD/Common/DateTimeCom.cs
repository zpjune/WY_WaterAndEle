using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
 public   class DateTimeCom
    {
        /// <summary>
        /// 比较日期大小
        /// DateTime dt3=DateTime.Now ; 
        ///string strdt3 = dt3.ToString("yyyy-MM-dd HH:mm:ss");
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool BoolDateTime(string date1, string date2)
        {
            if (date1.CompareTo(date2) <= 0)//CompareTo:0-两者相等 
                return false;
            //Response.Write("<script>alert(\"strdt3<strdt4\");</script>");
            else
                return true;
            // Response.Write("<script>alert(\"strdt3>strdt4\");</script>");
        }
    }
}
