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
using Newtonsoft.Json.Linq;
using RestReview.Models.data_consistency;
using RestReview.Models.data_consi_phone;
using System.Text.RegularExpressions;
using RestReview.Models.phone_data;
using RestReview.Models.FB;
using RestReview.Models.PhotoCountFB;
using RestReview.Models.ReviewClass;

namespace RestReview.Controllers
{
    public class HomeController : Controller
    {
        public static string API_KEY = WebConfigurationManager.AppSettings["yelp_api"];
        public static string FACEBOOK_ACCESS_TOKEN = WebConfigurationManager.AppSettings["facebook_access_token"];
        public static string yelpBusiness = WebConfigurationManager.AppSettings["YelpBusiness"];
        public static string dataconsistancygoogleapi = WebConfigurationManager.AppSettings["DataConsistancyGoogleApi"];
        public static string dataconsistancyBygoogleApi = WebConfigurationManager.AppSettings["DataConsistansyByWebSiteGoogleApi"];
        public static string facebookGraphApi = WebConfigurationManager.AppSettings["FaceBookGraphApi"];
        public static string faceBookPageSearch = WebConfigurationManager.AppSettings["FaceBookPageIdApi"];
        public static string GetAlbumByPage = WebConfigurationManager.AppSettings["AlbumByFaceBookPageIdApi"];

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

                if (!string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(CustomerId) && results.Location != null)
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
                        if (address1 == "" || address1 == null)
                        {
                            address1 = "Columbus";
                        }
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

                    if (businesses.Count < 3)
                    {
                        businesses = (from business in SearchList
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
                    }

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
                        b2.id = clsmatch.Id;
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
                    else if(closematchdata.Count == 3)
                    {
                        ViewBag.closeresult1 = closematchdata;
                        ViewBag.closeresult2 = closematchdata;
                        ViewBag.closeresult3 = closematchdata;
                    }
                    else
                    {
                        ViewBag.closeresult1 = obj;
                        ViewBag.closeresult2 = obj;
                        ViewBag.closeresult3 = obj;
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
                SearchList.Clear();
                return View();
            }
            catch (Exception ex)
            {
                SearchList.Clear();
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact(string id)
        {
            Analyzes analyze = new Analyzes();
            var website = "";
            var resturantName = "";
            var google_place_id = "";
            var fbID = "";
            long like_count = 0;
            long photo_count = 0;
            string Phone = "";
            string requestURL = "";

            //rating Data
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                if (id != null)
                {
                    ReviewExample res_review = new ReviewExample();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
                    client.BaseAddress = new Uri(yelpBusiness);
                    HttpResponseMessage response = client.GetAsync(id + "").Result;

                    if (response.IsSuccessStatusCode)
                    {                        
                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;
                        res_review = JsonConvert.DeserializeObject<ReviewExample>(result);

                        if (res_review.id != null)
                        {
                            resturantName = res_review.name;
                            Phone = res_review.phone;
                            decimal rate = (Convert.ToDecimal(res_review.rating) * 20) / 5;
                            decimal rate_count = (Convert.ToInt16(res_review.review_count) * 20) / 1738;
                            if (rate_count > 20)
                                rate_count = 20;
                            else if (rate_count == 0)
                                rate_count = 1;

                            analyze.RatingData = Convert.ToInt32(rate + rate_count);
                            analyze.RestaurantName = res_review.name;
                            analyze.ReviewCount = res_review.review_count;
                            analyze.ReviewRating = res_review.rating;
                        }
                    }
                    else
                    {                        
                        requestURL = response.RequestMessage.RequestUri.OriginalString;                        
                    }
                }
            }

            if (requestURL != "")
            {
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    if (id != null)
                    {
                        ReviewExample res_review = new ReviewExample();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
                        string uri = requestURL.Split(',')[0];
                        client.BaseAddress = new Uri(uri);
                        HttpResponseMessage response = client.GetAsync("").Result;

                        if (response.IsSuccessStatusCode)
                        {
                            response.EnsureSuccessStatusCode();
                            string result = response.Content.ReadAsStringAsync().Result;
                            res_review = JsonConvert.DeserializeObject<ReviewExample>(result);

                            if (res_review.id != null)
                            {
                                resturantName = res_review.name;
                                Phone = res_review.phone;
                                decimal rate = (Convert.ToDecimal(res_review.rating) * 20) / 5;
                                decimal rate_count = (Convert.ToInt16(res_review.review_count) * 20) / 1738;
                                if (rate_count > 20)
                                    rate_count = 20;
                                else if (rate_count == 0)
                                    rate_count = 1;

                                analyze.RatingData = Convert.ToInt32(rate + rate_count);
                                analyze.RestaurantName = res_review.name;
                                analyze.ReviewCount = res_review.review_count;
                                analyze.ReviewRating = res_review.rating;
                            }
                        }           
                    }
                }
            }

            //data consistency
            using (var client_consistency = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                if (Phone != null)
                {
                    //data cosistency code
                    string original_phone = new string(Phone.Where(char.IsDigit).ToArray());
                    client_consistency.BaseAddress = new Uri(dataconsistancygoogleapi);
                    HttpResponseMessage response_consistency = client_consistency.GetAsync("?input=%2B" + original_phone + "&inputtype=phonenumber&fields=place_id,name&key=AIzaSyBEnSTOkBqHX2LDW-yuzXD9Zi3lnQvbe9Y").Result;
                    response_consistency.EnsureSuccessStatusCode();
                    string result_consistency = response_consistency.Content.ReadAsStringAsync().Result;
                    var res_consistency = JsonConvert.DeserializeObject<TestExample>(result_consistency);

                    if (res_consistency.candidates.Count > 0)
                    {
                        var res1_nm = res_consistency.candidates[0].name;
                        google_place_id = res_consistency.candidates[0].place_id;
                        string[] actual_nm_array = resturantName.Split(' ');
                        string[] diff1 = res1_nm.Split(' ');
                        int length = actual_nm_array.Length;
                        int differentlength = diff1.Length;
                        int final_len = 0;
                        if (differentlength < length)
                        {
                            final_len = differentlength;
                        }
                        else
                        {
                            final_len = length;
                        }
                        
                        int count1 = 0;
                        for (int i = 0; i < final_len; i++)
                        {
                            if (string.Compare(actual_nm_array[i], diff1[i]) != 0)
                            {
                                count1 += 1;
                            }
                        }
                        if (count1 == 0 && (diff1.Length == actual_nm_array.Length))
                        {
                            analyze.DataDependecy = 10;      
                        }

                        if (analyze.DataDependecy == 0)
                        {
                            if (actual_nm_array.Length == diff1.Length)
                            {
                                int diff = 10 - count1;   
                                if (diff < 0)
                                    diff = 2;
                                analyze.DataDependecy = diff;        
                            }
                            else
                            {
                                Random randm = new Random();
                                analyze.DataDependecy = randm.Next(2, 7);
                            }
                        }
                    }
                }
            }

            //data consistency for website
            using (var client_consistency = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                //data cosistency with website from google
                client_consistency.BaseAddress = new Uri(dataconsistancyBygoogleApi);
                HttpResponseMessage response_consistency = client_consistency.GetAsync("?key=AIzaSyBEnSTOkBqHX2LDW-yuzXD9Zi3lnQvbe9Y&placeid=" + google_place_id + "&fields=name,rating,formatted_phone_number,website,international_phone_number").Result;
                response_consistency.EnsureSuccessStatusCode();
                string result_consistency = response_consistency.Content.ReadAsStringAsync().Result;
                var res_consistency = JsonConvert.DeserializeObject<Data_consi_Example>(result_consistency);

                if (res_consistency.result != null)
                {
                    if (res_consistency.result.website != "" && res_consistency.result.website != null)
                    {
                        var web = res_consistency.result.website.Split('/')[0]+"//"+ res_consistency.result.website.Split('/')[1]+ res_consistency.result.website.Split('/')[2];
                        website = web;
                        analyze.WebSite = 15;
                    }
                }
            }

            //facebook Like Count
            using (var client_consistency = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                if (website != "" && website != null)
                {
                    //facebook
                    client_consistency.BaseAddress = new Uri(facebookGraphApi);
                    HttpResponseMessage response_consistency = client_consistency.GetAsync("?ids=" + website + "").Result;
                    response_consistency.EnsureSuccessStatusCode();
                    string result_consistency = response_consistency.Content.ReadAsStringAsync().Result;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic item = serializer.Deserialize<dynamic>(result_consistency);
                    var like = item[website]["share"]["share_count"];

                    like_count = (Convert.ToInt64(like) * 20) / 2500;
                    if (like_count > 20)
                        like_count = 20;
                    else if (like_count < 0)
                        like_count = 0;

                    analyze.LikeCount = like_count;
                }
            }

            //facebook id generator
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                if (resturantName != "")
                {
                    var idRes = new FBidExample();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", FACEBOOK_ACCESS_TOKEN);
                    client.BaseAddress = new Uri(faceBookPageSearch);
                    HttpResponseMessage response = client.GetAsync("?q=" + resturantName + "").Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    idRes = JsonConvert.DeserializeObject<FBidExample>(result);
                    if (idRes.data.Count > 0)
                    {
                        fbID = idRes.data[0].id;
                    }
                }
            }

            //facebook photo count api
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                if (fbID != "")
                {
                    var idRes = new FBidExample();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", FACEBOOK_ACCESS_TOKEN);
                    client.BaseAddress = new Uri(GetAlbumByPage + fbID + "/albums");
                    HttpResponseMessage response = client.GetAsync("?fields=count,name").Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    idRes = JsonConvert.DeserializeObject<FBidExample>(result);

                    //photo count code
                    int? count = idRes.data.Count;
                    int? photos = 0;
                    for (int i = 0; i < count; i++)
                    {
                        photos += idRes.data[i].count;
                    }

                    photo_count = (Convert.ToInt64(photos) * 15) / 100;
                    if (photo_count > 15)
                        photo_count = 15;
                    else if (photo_count < 0)
                        photo_count = 0;
                    
                    analyze.PhotoCount = photo_count;
                }
                analyze.SocialMediaEngagement = analyze.LikeCount + analyze.PhotoCount;
            }
            analyze.MainProgress = analyze.RatingData + analyze.DataDependecy + analyze.WebSite + analyze.SocialMediaEngagement;
            //analyze.mainProg = analyze.MainProgress + "%";

            Session["RestaurantData"] = analyze;

            return View(analyze);
        }

        public ActionResult Analyze()
        {
            Analyzes analyzes = (Analyzes)Session["RestaurantData"];
            return View(analyzes);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllData(string xlocation)
        {

            SearchList.Clear();
            return await Task.FromResult(Json("hi", JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public async Task<JsonResult> AutoComplete(string prefix, string location)
        {
            client = new Client(API_KEY);
            var request = new Yelp.Api.Models.SearchRequest();
            request.Location = location;
            request.Term = prefix;
            request.MaxResults = 50;
            request.Categories = "Restaurants";

            searchResponse = await client.SearchBusinessesAllAsync(request);

            foreach (var b in searchResponse.Businesses)
            {
                if (!SearchList.Contains(b))
                    SearchList.Add(b);
            }

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
            var results = await client.GetBusinessAsync(ID);
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