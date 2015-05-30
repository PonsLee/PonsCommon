using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil
{
    public class DateTimeHelper
    {
        /// <summary>
        /// 注意所有规则以周日为一周第一天
        /// </summary>
        /// <param name="weekIndex"></param>
        /// <returns></returns>
        public static DateTime[] GetTimeByWeekIndex(int weekIndex)
        {
            return GetTimeByWeekIndex(DateTime.Now.Year, weekIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekIndex"></param>
        /// <returns></returns>
        public static DateTime[] GetTimeByWeekIndex(int year, int weekIndex)
        {
            try
            {
                if (weekIndex < 1)
                {
                    throw new Exception("请输入大于0的整数");
                }

                int allDays = (weekIndex - 1) * 7;
                //确定当年第一天
                var firstDate = new DateTime(year, 1, 1);
                int firstDayOfWeek = (int)firstDate.DayOfWeek;
                //周日为第一天
                firstDayOfWeek = firstDayOfWeek == 0 ? 1 : firstDayOfWeek + 1;
                //周开始日
                int startAddDays = allDays + (1 - firstDayOfWeek);
                DateTime weekRangeStart = firstDate.AddDays(startAddDays);
                //周结束日
                int endAddDays = allDays + (7 - firstDayOfWeek);
                DateTime weekRangeEnd = firstDate.AddDays(endAddDays);

                if (weekRangeStart.Year > year || (weekRangeStart.Year == year && weekRangeEnd.Year > year))
                {
                    throw new Exception("今年没有第" + weekIndex + "周。");
                }
                return new DateTime[2] { weekRangeStart, weekRangeEnd };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int GetMaxWeekOfYear(int year)
        {
            try
            {
                DateTime tempDate = new DateTime(year, 12, 31);
                int tempDayOfWeek = (int)tempDate.DayOfWeek;
                //周日为第一天
                tempDayOfWeek = tempDayOfWeek == 0 ? 1 : tempDayOfWeek + 1;
                if (tempDayOfWeek != 7)
                {
                    tempDate = tempDate.Date.AddDays(-tempDayOfWeek);
                }
                return GetWeekIndex(tempDate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns></returns>
        public static int GetWeekIndex(DateTime dTime)
        {
            try
            {
                //确定此时间在一年中的位置
                var dayOfYear = dTime.DayOfYear;
                var tempDayOfWeek = (int)dTime.DayOfWeek;
                //周日为第一天
                tempDayOfWeek = tempDayOfWeek == 0 ? 1 : tempDayOfWeek + 1;

                //当年第一天
                var tempDate = new DateTime(dTime.Year, 1, 1);
                var tempDayFirstWeek = (int)tempDate.DayOfWeek;
                tempDayFirstWeek = tempDayFirstWeek == 0 ? 1 : tempDayFirstWeek + 1;

                //当前周的范围
                DateTime retStartDay = dTime.AddDays(-tempDayOfWeek + 1);
                DateTime retEndDay = dTime.AddDays(7 - tempDayOfWeek);
                //确定当前是第几周
                int weekIndex = (int)Math.Ceiling(((double)dayOfYear + (tempDayFirstWeek == 7 ? 0 : (tempDayFirstWeek - 1))) / 7);

                if (retStartDay.Year < retEndDay.Year)
                {
                    weekIndex = 1;
                }

                return weekIndex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static int GetWeek(DateTime oneDay)
        {
            DateTime firstDay = new DateTime(oneDay.Year, 1, 1);

            int firstDayNum = (int)(firstDay.DayOfWeek) == 0 ? 7 : (int)(firstDay.DayOfWeek);

            int result = (int)Math.Ceiling((firstDayNum - 1 + oneDay.DayOfYear) / (float)7);

            return result;
        } 
    }
}
