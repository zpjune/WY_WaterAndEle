using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYSD
{
   public static class DateFormat
    {
        public static string DateToString(this DateTime dt) {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
