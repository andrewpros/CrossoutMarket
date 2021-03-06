﻿using System.Collections.Generic;
using System.Linq;
using Crossout.Web.Models;
using Crossout.Web.Models.Charts;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Data
{
    public class ChartDataModule : NancyModule
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);
        public ChartDataModule()
        {
            // would capture routes to /products/list sent as a GET request
            Get["/data/item/{name}/{id:int}"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                string name = x.name;

                // Harcoded Limit of 40k data points for now
                // we will aggregate data soon, we should never have so much data hopefully
                string query = "(SELECT market.id,market.sellprice,market.buyprice,market.selloffers,market.buyorders,market.datetime FROM market where market.itemnumber = @id ORDER BY market.Datetime desc LIMIT 40000) ORDER BY id ASC;";
                var p = new Parameter { Identifier = "@id", Value = x.id };
                var parmeter = new List<Parameter>();
                parmeter.Add(p);

                var ds = sql.SelectDataSet(query, parmeter);
                ChartDataModel model = new ChartDataModel();
                model.Name = name;
                if (ds != null && ds.Count > 0)
                {
                    foreach (var row in ds)
                    {
                        ChartItem item = ChartItem.CreateForChart(row);
                        model.Items.Add(item);
                    }
                }

                return Response.AsJson(model);
            };
        }
    }
}
