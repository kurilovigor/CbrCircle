using CbrCircle.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace CbrCircle
{
    /// <summary>
    /// Сервис запроса курса валют на указанную дату с сайта ЦБРФ
    /// </summary>
    public class CbrDailyInfo : ICbrDailyInfo
    {
        Uri _uri;
        string _currency;
        /// <summary>
        /// Конструирует службу
        /// </summary>
        /// <param name="uri">URI SOAPслужбы ЦБРФ</param>
        /// <param name="currency">Буквенный код валюты, например "USD"</param>
        public CbrDailyInfo(Uri uri, string currency)
        {
            _uri = uri;
            _currency = currency;
        }
        /// <summary>
        /// Получает курс валюты на указанную дату
        /// </summary>
        /// <param name="dt">Дата, для получения курса валюты</param>
        /// <returns>Курс в рублях для 1 единицы</returns>
        public async Task<decimal?> GetCurs(DateTime dt)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(_uri);
            var channelFactory = new ChannelFactory<DailyInfoSoap>(binding, endpoint);
            var client = channelFactory.CreateChannel();
            var r = await client.GetCursOnDateAsync(dt);
            decimal vnom = 0;
            decimal? vcurs = null;
            r.Nodes.ForEach(n =>
            {
                var xCurs = n.XPathSelectElement($"ValuteData/ValuteCursOnDate[VchCode='{_currency}']");
                if (xCurs != null)
                {
                    vnom = Format.ToDecimal(xCurs.Element("Vnom").Value);
                    vcurs = Format.ToDecimal(xCurs.Element("Vcurs").Value);
                }
            });
            return vcurs / (vnom == 0 ? 1 : vnom);
        }
    }
}
