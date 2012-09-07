using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using DataAccess;
using ExpNewOrd.Common;

namespace ExpNewOrd.Entity
{
    [DelimitedRecord("|")]
    public partial class NewTotalOrderEntitycs
    {
        private static string Sql_NewTotalOrd
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select                                                                            \n");
                sb.Append("OrderNo ID                                                                        \n");
                sb.Append(",m.CustomerSID CustomerSID                                                        \n");
                sb.Append(",trd.DateCreated CreateTime                                                       \n");
                sb.Append(",trd.TotalPoints NeedPoint                                                        \n");
                sb.Append(",trd.CounterNo StoreNo                                                            \n");
                sb.Append(",trd.OrderStatus                                                                  \n");
                sb.Append(",trd.SourcesData CreateSource                                                     \n");
                sb.Append("from                                                                              \n");
                sb.Append("tab_order4total trd                                                               \n");
                sb.Append("join                                                                              \n");
                sb.Append("Tab_UserCommunity m                                                               \n");
                sb.Append("on                                                                                \n");
                sb.Append("m.Idx=trd.UserIdx_Fx                                                              \n");
                sb.Append("where                                                                             \n");
                sb.Append("DateCreated between                                                               \n");
                sb.Append("cast(convert(nvarchar(20),dateadd(day,-1,getdate()),102)+' 00:00:00' as datetime) \n");
                sb.Append("and                                                                               \n");
                sb.Append("getdate()                                                                         \n");
                sb.Append("and                                                                               \n");
                sb.Append("SourcesData='web'                                                                 \n");
                sb.Append("and                                                                               \n");
                sb.Append("Len(OrderNo)>0                                                                    \n");
                return sb.ToString();
            }
        }

        /// <summary>
        /// 总订单编号
        /// </summary>
        public string ID;

        /// <summary>
        /// 会员POS号
        /// </summary>
        public string CustomerSID;

        /// <summary>
        /// 订单生成时间
        /// </summary>
        [FieldConverter(ConverterKind.Date, "yyyy-MM--dd HH:mm:ss.fff")]
        public DateTime CreateTime;

        /// <summary>
        /// 总积分
        /// </summary>
        public int NeedPoint;

        /// <summary>
        /// 柜台编号
        /// </summary>
        public string StoreNo;

        /// <summary>
        /// 订单状态
        /// 0 订单未处理
        /// 1 订单全部成功
        /// 2 订单全部失败
        /// 3 订单部分成功
        /// </summary>
        public string OrderStatus;

        /// <summary>
        /// Web 网站
        /// Internal 电话中心
        /// </summary>
        public string CreateSource;

        public static void ExportCSV()
        {
            try
            {
                var dt = SqlHelper.ExecuteDataTable(AppCommon.Connstring, System.Data.CommandType.Text, Sql_NewTotalOrd);
                if (null == dt || dt.Rows.Count < 1) return;
                var ets = ConvertHelper.OTConverter.ConvertTableToObjectByField<NewTotalOrderEntitycs>(dt);
                if (null == ets || ets.Count < 1) return;
                FileHelperEngine engine = new FileHelperEngine(typeof(NewTotalOrderEntitycs));
                engine.WriteFile("newTotalOrder.csv", ets);
                //AppCommon.Log.Info(string.Format("{0} output newTotalOrder.csv",
                //    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                NLog.LogManager.GetCurrentClassLogger().Trace(string.Format("{0} output newTotalOrder.csv",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().DebugException(string.Empty,ex);
            }
        }
    }
}
