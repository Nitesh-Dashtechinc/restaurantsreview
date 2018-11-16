using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Yelp.Api;
using Yelp.Api.Models;

namespace RestReview.Controllers
{
    public class RestController : Controller
    {
        public static string API_KEY = WebConfigurationManager.AppSettings["yelp_api"];
        Client client = new Client(API_KEY);
        public static SearchResponse searchResponse;

        public ActionResult Index()
        {
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

        [HttpPost]
        public async Task<ActionResult> restaurantsreviews(string CustomerName, string CustomerId)
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
                                  }).Distinct().ToList();

                List<biz_data> obj = new List<biz_data>();
                foreach (var item in businesses)
                {
                    biz_data b1 = new biz_data();
                    b1.id = item.id;
                    b1.name = item.name;
                    b1.rate = item.rate;
                    b1.ratecount = item.ratecount;
                    obj.Add(b1);
                }
                //ViewData["dt"] = businesses;
                ViewBag.businessList = obj;
                ViewBag.Message = "CustomerName: " + CustomerName + " CustomerId: " + CustomerId;

                return View();
            }
            return RedirectToAction("Index","Home");
        }
        public class biz_data
        {
            public string id { get; set; }
            public string name { get; set; }
            public float rate { get; set; }
            public int ratecount { get; set; }
        }

        [HttpPost]
        public ActionResult RestaurantMarketGrade()
        {
            return View();
        }

    }
}