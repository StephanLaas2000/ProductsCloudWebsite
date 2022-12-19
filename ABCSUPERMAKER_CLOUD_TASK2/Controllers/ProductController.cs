using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABCSUPERMAKER_CLOUD_TASK2.Models;
using ABCSUPERMAKER_CLOUD_TASK2.TableHandler;
using ABCSUPERMAKER_CLOUD_TASK2.BlobHandler;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ABCSUPERMAKER_CLOUD_TASK2.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index(string id)
        {
            //for our edit
            if (!string.IsNullOrEmpty(id))
            {
                //set the name of the table
                TableManager TableManagerObj = new TableManager("Product");
                //retrieve the Product to be updated
                List<Product> ProductListObj = TableManagerObj.RetrieveEntity<Product>("RowKey eq '" + id + "'");

                Product ProductObj = ProductListObj.FirstOrDefault();
                return View(ProductObj);
            }
            return View(new Product());
        }
        // GET: Home
        [HttpPost]
        public ActionResult Index(string id, HttpPostedFileBase uploadFile, FormCollection formData)
        {
            Product ProductObj = new Product();
            ProductObj.productDescription = formData["productDescription"] == "" ? null : formData["productDescription"];
            ProductObj.productName = formData["productName"] == "" ? null : formData["productName"];
            double productPrice;
            if (double.TryParse(formData["productPrice"], out productPrice))
            {
                ProductObj.productPrice = double.Parse(formData["productPrice"] == "" ? null : formData["productPrice"]);
            }
            else
            {
                return View(new Product());
            }
            foreach (string file in Request.Files)
            {
                uploadFile = Request.Files[file];
            }
            //blob container creation
            BlobManager BlobManagerObj = new BlobManager("pictures");
            string FileAbsoluteUri = BlobManagerObj.UploadFile(uploadFile);
            ProductObj.FilePath = FileAbsoluteUri.ToString();
            //Insert statement
            if (string.IsNullOrEmpty(id))
            {
                ProductObj.PartitionKey = "Product";
                ProductObj.RowKey = Guid.NewGuid().ToString();
                TableManager TableManagerObj = new TableManager("Product");
                TableManagerObj.InsertEntity<Product>(ProductObj, true);
            }
            else
            {
                ProductObj.PartitionKey = "Product";
                ProductObj.RowKey = id;
                TableManager TableManagerObj = new TableManager("Product");
                TableManagerObj.InsertEntity<Product>(ProductObj, false);
            }
            return RedirectToAction("Get");
        }
        //get Products
        public ActionResult Get()
        {
            TableManager TableManagerObj = new TableManager("Product");
            List<Product> ProductListObj = TableManagerObj.RetrieveEntity<Product>(null);
            return View(ProductListObj);
        }
        //Delete Product
        public ActionResult Delete(string id)
        {
            //return the Product to be deleted
            TableManager TableManagerObj = new TableManager("Product");
            List<Product> ProductListObj = TableManagerObj.RetrieveEntity<Product>("RowKey eq'" + id + "'");

            Product ProductObj = ProductListObj.FirstOrDefault();
            //delete the Product
            TableManagerObj.DeleteEntity<Product>(ProductObj);
            return RedirectToAction("Get");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Website Created by Stephan Laas.";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        //public async Task<IActionResult> Upload(List<IFormFile> files)
        //{
        //    long size = files.Sum(f => f.Length);
        //    var filePaths = new List<string>();

        //    foreach (var formFile in files)
        //    {

        //        if (formFile.Length > 0)
        //        {
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), formFile.FileName);
        //            filePaths.Add(filePath);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }
        //        }
        //    }
        //    return Ok(new { count = files.Count, size, filePaths }); ;
        //}

    }
}