using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FangJiaFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "server=192.168.10.68;uid=root;pwd=root;database=ivrdb_test_pf;CharSet=utf8;";
            var url = "https://shanghai.anjuke.com/market/";//上海
            //var url = "https://zhengzhou.anjuke.com/market/";//郑州

            var web = new HtmlWeb();
            var doc = web.Load(url);
            //var parentNode = doc.DocumentNode.SelectNodes("//div[@class='area']");
            //var currQu = doc.DocumentNode.SelectSingleNode("//div[@class='area']/div[@class='bigArea']/a[@class='curr']");
            List<JiaGe> list = new List<JiaGe>();
            var allBigArea = doc.DocumentNode.SelectNodes("//div[@class='area']/div[@class='bigArea']/a");
            for (int i = 1; i < allBigArea.Count; i++)
            {
                Console.WriteLine($"当前区域：{ allBigArea[i].InnerText}");
                var allUrl = allBigArea[i].Attributes["href"].Value;
                var allName = allBigArea[i].InnerText;
                list.Add(new JiaGe { BigArea = allName, BigAreaUrl = allUrl });
            }
            List<JiaGe> childList = new List<JiaGe>();
            foreach(var item in list)
            {
                var smallDoc = web.Load(item.BigAreaUrl);
                var allArea = smallDoc.DocumentNode.SelectNodes("//div[@class='area']/div[@class='smallArea']/a");
                System.Threading.Thread.Sleep(500);
                //获取所有区域
                for(int i=1;i<allArea.Count;i++)
                {
                    childList.Add(new JiaGe { BigArea = item.BigArea, Area = allArea[i].InnerText, AreaUrl = allArea[i].Attributes["href"].Value });
                    //item.Area = allArea[i].InnerText;
                    //item.AreaUrl = allArea[i].Attributes["href"].Value;
                }
                //foreach (var area in allArea)
                //{
                //    item.Area = area.InnerText;
                //    item.AreaUrl = area.Attributes["href"].Value;
                //    //list.Add(new JiaGe { Area = area.InnerText, AreaUrl = item.Attributes["href"].Value });
                //    //Console.WriteLine($"子区域：{ item.InnerText}-链接：{item.Attributes["href"].Value }");
                //}
            }

            ////获取所有区域价格信息
            foreach (var item in childList)
            {
                var docItem = web.Load(item.AreaUrl);
                var trendR = docItem.DocumentNode.SelectNodes("//div[@class='trendR']");
                System.Threading.Thread.Sleep(1000);
                if (trendR == null)
                    continue;
                foreach (var trend in trendR)
                {
                    try
                    {
                        var jiaGeTrend = trend.SelectSingleNode("./h2[@class='highLight']").ChildNodes[1].Attributes[0].Value;
                        var jiaGeValue = trend.SelectSingleNode("./h2[@class='highLight']").ChildNodes[1].InnerText;
                        item.JunJia = jiaGeValue;
                        item.JunJiaTrend = jiaGeTrend;

                        var huanBiTrend = trend.SelectSingleNode("./h2[3]/i[1]").Attributes[0].Value;
                        var huanBiJiaGe = trend.SelectSingleNode("./h2[3]/i[1]").InnerText;
                        item.HuanBi = huanBiJiaGe.Replace("%","").Replace("↓", "").Replace("↑", "");
                        item.HuanBiTrend = huanBiTrend;

                        var tongBiTrend = trend.SelectSingleNode("./h2[3]/i[2]").Attributes[0].Value;
                        var tongBiJiaGe = trend.SelectSingleNode("./h2[3]/i[2]").InnerText;
                        item.TongBi = tongBiJiaGe.Replace("%", "").Replace("↓", "").Replace("↑", ""); 
                        item.TongBiTrend = tongBiTrend;
                        Console.WriteLine($"大区：{item.BigArea}\t子区：{item.Area}\t均价：{item.JunJia}\t同比：{item.TongBiTrend}\t 幅度:{item.TongBi}\t环比：{item.HuanBiTrend}\t幅度:{item.HuanBi}");
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            foreach (var item in childList)
            {
                Console.WriteLine($"大区：{item.BigArea}\t子区：{item.Area}\t均价：{item.JunJia}\t同比：{item.TongBiTrend}\t 幅度:{item.TongBi}\t环比：{item.HuanBiTrend}\t幅度:{item.HuanBi}");
            }
            //Console.WriteLine($"当前区域：{ currQu.InnerText}");
            //var allArea = doc.DocumentNode.SelectNodes("//div[@class='area']/div[@class='smallArea']/a");
            // List<JiaGe> list = new List<JiaGe>();
            ////获取所有区域
            //foreach (var item in allArea)
            //{
            //    list.Add(new JiaGe { Area = item.InnerText,AreaUrl = item.Attributes["href"].Value });
            //    Console.WriteLine($"子区域：{ item.InnerText}-链接：{item.Attributes["href"].Value }");
            //}
            ////获取所有区域价格信息
            //foreach (var item in list)
            //{
            //    var docItem = web.Load(item.AreaUrl);
            //    var trendR = docItem.DocumentNode.SelectNodes("//div[@class='trendR']");
            //    System.Threading.Thread.Sleep(1000);
            //    if (trendR == null)
            //        continue;

            //    foreach (var trend in trendR)
            //    {

            //        var jiaGeTrend = trend.SelectSingleNode("./h2[@class='highLight']").ChildNodes[1].Attributes[0].Value;
            //        var jiaGeValue = trend.SelectSingleNode("./h2[@class='highLight']").ChildNodes[1].InnerText;
            //        item.JunJia = jiaGeValue;
            //        item.JunJiaTrend = jiaGeTrend;

            //        var huanBiTrend = trend.SelectSingleNode("./h2[3]/i[1]").Attributes[0].Value;
            //        var huanBiJiaGe = trend.SelectSingleNode("./h2[3]/i[1]").InnerText;
            //        item.HuanBi = huanBiJiaGe;
            //        item.HuanBiTrend = huanBiTrend;

            //        var tongBiTrend = trend.SelectSingleNode("./h2[3]/i[2]").Attributes[0].Value;
            //        var tongBiJiaGe = trend.SelectSingleNode("./h2[3]/i[2]").InnerText;
            //        item.TongBi = tongBiJiaGe;
            //        item.TongBiTrend = tongBiTrend;

            //    }
            //}
            //foreach (var item in list)
            //{
            //    Console.WriteLine($"区域：{item.Area}\t均价：{item.JunJia}\t同比：{item.TongBiTrend}\t 幅度:{item.TongBi}\t环比：{item.HuanBiTrend}\t幅度:{item.HuanBi}");    

            //}
            //// string sql = "insert into ShangHai (Area,AreaValue,TongBiValue,TongBiTrend,HuanBiValue,HuanBiTrend) values('{0}','{1}','{2}','{3}','{4}','{5}');";
            string sql = "insert into ShangHai (BigArea,Area,AreaValue,TongBiValue,TongBiTrend,HuanBiValue,HuanBiTrend) values('{6}','{0}','{1}','{2}','{3}','{4}','{5}');";
            StringBuilder sb = new StringBuilder();
            using (var conn = new MySqlConnection(connStr))
            {
                foreach (var item in childList)
                {
                    sb.AppendLine(string.Format(sql, item.Area, item.JunJia, item.TongBi, item.TongBiTrend, item.HuanBi, item.HuanBiTrend,item.BigArea));
                }
                using (var comm = new MySqlCommand(sb.ToString(), conn))
                {
                    conn.Open();
                    var res = comm.ExecuteNonQuery();
                }
            }

            Console.ReadKey();


        }
        public class JiaGe
        {
            /// <summary>
            /// 区域
            /// </summary>
            public string BigArea { get; set; }
            /// <summary>
            /// 区域URL
            /// </summary>
            public string BigAreaUrl { get; set; }
            /// <summary>
            /// 区域价格
            /// </summary>
            public string BigAreaValue { get; set; }

            /// <summary>
            /// 区域名称
            /// </summary>
            public string Area { get; set; }
            /// <summary>
            /// 区域Url
            /// </summary>
            public string AreaUrl { get; set; }
            /// <summary>
            /// 均价
            /// </summary>
            public string JunJia { get; set; }
            /// <summary>
            /// 趋势，up上升，down下降
            /// </summary>
            public string JunJiaTrend { get; set; }
            /// <summary>
            /// 环比
            /// </summary>
            public string HuanBi { get; set; }
            /// <summary>
            /// 趋势，up上升，down下降
            /// </summary>
            public string HuanBiTrend { get; set; }
            /// <summary>
            /// 同比
            /// </summary>
            public string TongBi { get; set; }
            /// <summary>
            ///趋势，up上升，down下降
            /// </summary>
            public string TongBiTrend { get; set; }
        }
    }
}
