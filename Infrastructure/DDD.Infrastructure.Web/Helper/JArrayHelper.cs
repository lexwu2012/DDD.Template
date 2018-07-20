using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Web.Helper
{
    public static class JArrayHelper
    {
        public static string JsonToHtmlJArray(string json, string titleKeyValue)
        {

            StringBuilder mailBody = new StringBuilder();

            string mailTitle = "";
            int rowIndex = 0;

            #region 标题字典
            string[] TitleList = titleKeyValue.Split('|');
            Dictionary<string, string> dictTitle = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string title in TitleList)
            {
                string[] keyValueArr = title.Split('=');
                if (keyValueArr.Length != 2) continue;

                dictTitle[keyValueArr[0]] = keyValueArr[1];
            }
            #endregion 标题字典

            try
            {
                #region 遍历json

                JArray tList = JArray.Parse(json);

                foreach (var t in tList)
                {
                    mailBody.Append("<tr>");

                    JObject o = JObject.Parse(t.ToString());
                    IEnumerable<JProperty> properties = o.Properties();
                    foreach (JProperty item in properties)
                    {
                        string name = item.Name;
                        string value = item.Value.ToString();
                        if (!dictTitle.Keys.Contains(name))
                        {
                            continue;
                        }

                        if (rowIndex == 0)
                        {
                            string title = name;
                            if (dictTitle.Keys.Contains(name))
                            {
                                title = dictTitle[name];
                            }
                            mailTitle += string.Format(@"<td>{0}</td>", title);
                        }

                        mailBody.Append(string.Format(@"<td>{0}</td>", value));

                    }

                    mailBody.Append("</tr>");
                    rowIndex++;

                }

                mailBody.Append("</table>");

                mailBody.Insert(0, "</tr>");
                mailBody.Insert(0, mailTitle);
                mailBody.Insert(0, "<tr style=\"background-color:#2cb778;\">");
                mailBody.Insert(0, "<table  style=\"text-align:center;border-collapse: collapse;\" border=\"1\" cellpadding=\"6\">");

                #endregion 遍历json

            }
            catch (Exception ex)
            {
                return "JsonToHtml error=" + ex.Message + " StackTrace=" + ex.StackTrace;
            }

            return mailBody.ToString();

        }
    }
}
