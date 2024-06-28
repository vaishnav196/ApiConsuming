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

        //[HttpPost]
        //public IActionResult AddProduct(Product p)
        //{
        //    string url = "https://localhost:7000/api/Product/AddProduct/";
        //    var jsondata = JsonConvert.SerializeObject(p);
        //    StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
        //    HttpResponseMessage respone = client.PostAsync(url, content).Result;
        //    if (respone.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    return View();

        //}

        [HttpPost]
        public IActionResult AddProduct(Product p, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var imagePath = UploadFile(imageFile).Result;
                p.ImagePath = imagePath;
            }

            string url = "https://localhost:7000/api/Product/AddProduct/";
            var jsondata = JsonConvert.SerializeObject(p);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
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

        //[HttpPost]
        //public IActionResult EditProduct(Product p)
        //{
        //    string url = "https://localhost:7000/api/Product/UpdateProd/";
        //    var jsondata = JsonConvert.SerializeObject(p);
        //    StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
        //    HttpResponseMessage response = client.PutAsync(url, content).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        [HttpPost]
        public IActionResult EditProduct(Product p, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var imagePath = UploadFile(imageFile).Result;
                p.ImagePath = imagePath;

            }

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



        private async Task<string> UploadFile(IFormFile file)
        {
            var url = "https://localhost:7000/api/Product/upload";
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(result);
            return data.filePath;
        }

        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var url = $"https://localhost:7000/api/Product/download/{fileName}/";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return NotFound("File not found.");

            var fileStream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType.ToString();

            return File(fileStream, contentType, fileName);
        }




        [HttpPost]
        public IActionResult DeleteSelectedProducts(List<int> selectedIds)
        {
            if (selectedIds != null && selectedIds.Count > 0)
            {
                foreach (var id in selectedIds)
                {
                    string url = "https://localhost:7000/api/Product/DeleteMultipleProducts/";
                    HttpResponseMessage response = client.DeleteAsync(url).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        // Handle error
                        ViewBag.ErrorMessage = "Error deleting some products.";
                        break;
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }

}



