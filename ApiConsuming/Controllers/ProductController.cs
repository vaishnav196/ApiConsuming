using ApiConsuming.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Security;

namespace ApiConsuming.Controllers
{
    public class ProductController : Controller
    {   HttpClient client; 
        public ProductController()
            //this is for Handling SSL ceertificate errors
        {       HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback=(sender,cert,chain,SslPolicyErrors) => { return true; };
            client = new HttpClient(handler);  
        }

        string url;
        public IActionResult Index()
        {
            string url = "https://localhost:7000/GetAllProduct/";
            List<Product> products = new List<Product>();   
            HttpResponseMessage response=client.GetAsync(url).Result;
            if(response.IsSuccessStatusCode)
            {
                string jsondata=response.Content.ReadAsStringAsync().Result;
                var obj=JsonConvert.DeserializeObject<List<Product>>(jsondata);
                if(obj != null)
                {
                    products = obj;
                    //products.Add(obj);
                   
                }
            }
            return View(products);
        }
    }
}
