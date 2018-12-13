using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web;
using System.Web.Mvc;
using Yelp.Api;
using Yelp.Api.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using RestReview.Models;

namespace RestReview.Controllers
{
    public class HomeController : Controller
    {
        public static string API_KEY = WebConfigurationManager.AppSettings["yelp_api"];
        Client client = new Client(API_KEY);
        public static SearchResponse searchResponse;
        static List<BusinessResponse> SearchList = new List<BusinessResponse>();


        public ActionResult Index()
        {
            ViewBag.businessList = null;
            ViewBag.closeresult1 = null;
            ViewBag.closeresult2 = null;
            ViewBag.closeresult3 = null;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Review(string CustomerName, string CustomerId)
        {
            ViewBag.businessList = null;
            ViewBag.closeresult1 = null;
            ViewBag.closeresult2 = null;
            ViewBag.closeresult3 = null;
            try
            {
                client = new Client(API_KEY);
                var results = await client.GetBusinessAsync(CustomerId);

                List<BusinessMatch> closematches = new List<BusinessMatch>();

                if (!string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(CustomerId))
                {
                    string name = results.Name;
                    string address1 = results.Location.Address1;
                    string city = results.Location.City;
                    string state = results.Location.State;
                    string country = results.Location.Country;

                    using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
                        client.BaseAddress = new Uri("https://api.yelp.com/v3/businesses/matches/");
                        HttpResponseMessage response = client.GetAsync("lookup?name=" + name + "&address1=" + address1 + "&city=" + city + "&state=" + state + "&country=" + country + "").Result;
                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;
                        var res = JsonConvert.DeserializeObject<Example>(result);
                        var count = res.businesses.Count();
                        for (int i = 0; i < count; i++)
                        {
                            BusinessMatch business = new BusinessMatch();
                            business.id = res.businesses[i].id;
                            closematches.Add(business);
                        }
                    }


                    var businesses = (from business in SearchList
                                      where (business.Id != CustomerId)
                                      select new
                                      {
                                          name = business.Name,
                                          id = business.Id,
                                          rate = business.Rating,
                                          ratecount = business.ReviewCount,
                                          ImageUrl = business.ImageUrl,
                                          Phone = business.DisplayPhone,
                                          Location = business.Location.DisplayAddress.FirstOrDefault() + " " + business.Location.City + ", " + business.Location.Country,
                                          Url = business.Url
                                      }).Distinct().ToList();

                    List<biz_data> obj = new List<biz_data>();
                    foreach (var item in businesses)
                    {
                        biz_data b1 = new biz_data();
                        b1.id = item.id;
                        b1.name = item.name;
                        b1.rate = item.rate;
                        b1.ratecount = item.ratecount;
                        b1.imageurl = item.ImageUrl;
                        b1.Location = item.Location;
                        b1.Phone = item.Phone;
                        b1.Url = item.Url;
                        obj.Add(b1);
                    }

                    List<CloseMatchData> closematchdata = new List<CloseMatchData>();
                    foreach (var item in closematches)
                    {
                        CloseMatchData b2 = new CloseMatchData();
                        var clsmatch = await client.GetBusinessAsync(item.id);
                        b2.id = clsmatch.Id;  //added by dipa
                        b2.name = clsmatch.Name;
                        b2.rate = clsmatch.Rating;
                        b2.ratecount = clsmatch.ReviewCount;
                        b2.imageurl = clsmatch.ImageUrl;
                        b2.Location = clsmatch.Location.Address1 + " " + clsmatch.Location.City + " " + clsmatch.Location.State + " " + clsmatch.Location.Country;
                        b2.Phone = clsmatch.Phone;
                        b2.Url = clsmatch.Url;
                        closematchdata.Add(b2);
                    }

                    if (closematchdata.Count == 1)
                    {
                        ViewBag.closeresult1 = closematchdata;
                        ViewBag.closeresult2 = obj;
                        ViewBag.closeresult3 = obj;
                    }
                    else if (closematchdata.Count == 2)
                    {
                        ViewBag.closeresult1 = closematchdata;
                        ViewBag.closeresult2 = closematchdata;
                        ViewBag.closeresult3 = obj;
                    }
                    else
                    {
                        ViewBag.closeresult1 = closematchdata;
                        ViewBag.closeresult2 = closematchdata;
                        ViewBag.closeresult3 = closematchdata;
                    }

                    ViewBag.businessList = obj;
                    ViewBag.Message = "CustomerName: " + CustomerName + " CustomerId: " + CustomerId;
                }
                else
                {
                    var businesses = (from business in SearchList
                                      where (business.Id != CustomerId)
                                      select new
                                      {
                                          name = business.Name,
                                          id = business.Id,
                                          rate = business.Rating,
                                          ratecount = business.ReviewCount,
                                          ImageUrl = business.ImageUrl,
                                          Phone = business.DisplayPhone,
                                          Location = business.Location.DisplayAddress.FirstOrDefault() + " " + business.Location.City + ", " + business.Location.Country,
                                          Url = business.Url
                                      }).Distinct().ToList();

                    List<biz_data> obj = new List<biz_data>();
                    foreach (var item in businesses)
                    {
                        biz_data b1 = new biz_data();
                        b1.id = item.id;
                        b1.name = item.name;
                        b1.rate = item.rate;
                        b1.ratecount = item.ratecount;
                        b1.imageurl = item.ImageUrl;
                        b1.Location = item.Location;
                        b1.Phone = item.Phone;
                        b1.Url = item.Url;
                        obj.Add(b1);
                    }
                    ViewBag.closeresult1 = obj;
                    ViewBag.closeresult2 = obj;
                    ViewBag.closeresult3 = obj;
                    ViewBag.businessList = obj;
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact(float rateing, string id)
        {
            ViewBag.Message = "Your contact page.";
            //rating bar code
            float rate = (rateing * 30) / 5;
            ViewBag.ratingData = rate;
            return View();
        }

        public ActionResult Analyze()
        {
            ViewBag.Message = "Your Analyze page.";
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllData(string xlocation)
        {

            SearchList.Clear();
            //client = new Client(API_KEY);
            //var request = new Yelp.Api.Models.SearchRequest();
            //request.Location = xlocation;
            //request.Term = "pl";
            //request.MaxResults = 50;

            //int count = 1;


            //while (count < 1000)
            //{
            //    request.ResultsOffset = count;
            //    searchResponse = await client.SearchBusinessesAllAsync(request);

            //    foreach (var b in searchResponse.Businesses)
            //    {
            //        if (!SearchList.Contains(b))
            //            SearchList.Add(b);
            //    }
            //    count += 50;

            //}

            //using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            //{
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
            //    client.BaseAddress = new Uri("https://api.yelp.com/v3/openql/businesses/search");
            //    var location = "North Kingstown";
            //    HttpResponseMessage response = client.GetAsync("lookup?term=oak&location=" + location + "").Result;
            //    response.EnsureSuccessStatusCode();
            //    string result = response.Content.ReadAsStringAsync().Result;
            //    var res = JsonConvert.DeserializeObject<Example>(result);
            //    var count = res.businesses.Count();

            //}

            return await Task.FromResult(Json("hi", JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public async Task<JsonResult> AutoComplete(string prefix, string location)
        {

            //  SearchList.Clear();
            client = new Client(API_KEY);
            var request = new Yelp.Api.Models.SearchRequest();
            request.Location = location;
            request.Term = prefix;
            request.MaxResults = 50;

            //int count = 0;
            //request.ResultsOffset = count;
            searchResponse = await client.SearchBusinessesAllAsync(request);

            foreach (var b in searchResponse.Businesses)
            {
                if (!SearchList.Contains(b))
                    SearchList.Add(b);
            }

            //   var customers = (from customer in searchResponse.Businesses
            var customers = (from customer in SearchList
                             where ((customer.Name.ToLower()).Contains(prefix.ToLower()))
                             select new
                             {
                                 label = customer.Name,
                                 val = customer.Id
                             }).Distinct().ToList();

            return await Task.FromResult(Json(customers, JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public async Task<JsonResult> GetBusiness(string ID)
        {
            var results = await client.GetBusinessAsync(ID);//, request.Latitude, request.Longitude);
            return await Task.FromResult(Json(results, JsonRequestBehavior.AllowGet));
        }

        public class biz_data
        {
            public string id { get; set; }
            public string name { get; set; }
            public float rate { get; set; }
            public int ratecount { get; set; }
            public string imageurl { get; set; }
            public string Phone { get; set; }
            public string Location { get; set; }
            public string Url { get; set; }
        }

        public class CloseMatchData
        {
            public string id { get; set; }
            public string name { get; set; }
            public float rate { get; set; }
            public int ratecount { get; set; }
            public string imageurl { get; set; }
            public string Phone { get; set; }
            public string Location { get; set; }
            public string Url { get; set; }
        }
    }
}