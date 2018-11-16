using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web;
using System.Web.Mvc;
using Yelp.Api;
using Yelp.Api.Models;

namespace RestReview.Controllers
{
    public class HomeController : Controller
    {
        public static string API_KEY = WebConfigurationManager.AppSettings["yelp_api"];
        Client client = new Client(API_KEY);
        public static SearchResponse searchResponse;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Review(string CustomerName, string CustomerId)
        {
            try
            {
            
            if (!string.IsNullOrEmpty(CustomerName))
            {
                client = new Client(API_KEY);
                var results = await client.GetBusinessAsync(CustomerId);//, request.Latitude, request.Longitude);
                                                                        //results.Location.DisplayAddress[0].ToString();
                ViewBag.data = results;
                ViewBag.data1 = results;
                ViewBag.data2 = results;

                var businesses = (from business in searchResponse.Businesses
                                  where (business.Id != CustomerId)
                                  select new
                                  {
                                      name = business.Name,
                                      id = business.Id,
                                      rate = business.Rating,
                                      ratecount = business.ReviewCount,
                                      ImageUrl = business.ImageUrl,
                                      Phone = business.DisplayPhone,
                                      Location = business.Location.DisplayAddress.FirstOrDefault() + " " + business.Location.City+ ", " + business.Location.Country
                                      // }).Distinct().Take(3);
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
                    obj.Add(b1);
                }
                //ViewData["dt"] = businesses;
                ViewBag.businessList = obj;
                ViewBag.Message = "CustomerName: " + CustomerName + " CustomerId: " + CustomerId;

                return View();
            }
            return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetAllData(string xlocation)
        {
            client = new Client(API_KEY);

            var request = new Yelp.Api.Models.SearchRequest();
            request.Location = xlocation;
            request.Term = "";
            request.MaxResults = 40;

            searchResponse = await client.SearchBusinessesAllAsync(request);

            return await Task.FromResult(Json("hi", JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public async Task<JsonResult> AutoComplete(string prefix)
        {
            var customers = (from customer in searchResponse.Businesses
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
        }
    }
}