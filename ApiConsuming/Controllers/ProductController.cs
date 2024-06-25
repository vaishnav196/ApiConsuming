using ApiConsuming.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Security;
using System.Text;

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



     
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product p)
        {
            string url = "https://localhost:7000/api/Product/AddProduct/";
            var jsondata = JsonConvert.SerializeObject(p);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage respone = client.PostAsync(url, content).Result;
            if (respone.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();

        }


       

        public IActionResult DeleteProduct(int id)
        {
            string url = "https://localhost:7000/api/Product/DeleteProd/";
            HttpResponseMessage response = client.DeleteAsync(url+id).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult EditProduct(int id)
        {
            Product prod = new Product();
            string url = "https://localhost:7000/api/Product/GetProdById/";
            HttpResponseMessage response = client.GetAsync(url + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string jsondata = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<Product>(jsondata);
                if (obj != null)
                {
                    prod = obj;
                }
            }
            return View(prod);
        }

        [HttpPost]
        public IActionResult EditProduct(Product p)
        {
            string url = "https://localhost:7000/api/Product/UpdateProd/";
            var jsondata = JsonConvert.SerializeObject(p);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PutAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

