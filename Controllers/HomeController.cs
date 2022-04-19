using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class HomeController : Controller
    {
        OnlineStoreContext db = new OnlineStoreContext();
        Dictionary<string, string> admin = new Dictionary<string, string>();

        public ActionResult ProductList()
        {
            return View(db.Products);
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            db.Products.Add(product);
            db.SaveChanges();
            return Redirect("/Home/ProductList");
        }

        [HttpGet]
        public ActionResult AddBasket(int id)
        {
            Product product = db.Products.Find(id);
            return View(product);
        }

        [HttpPost]
        public ActionResult AddBasket(Product product)
        {
            int numberProductToStock = db.Products.Find(product.Id).Number;    // получаем количество товара на складе

            if (product.Number > numberProductToStock)
            {
                ModelState.AddModelError(nameof(product.Number), "Нельзя заказать товара больше, чем есть в наличии!");
            }
            if (ModelState.IsValid)
            {
                if (Session["Order"] != null)
                    Session["Order"] += product.Id + ";" + product.Price + ";" + product.Number + ";";
                else
                    Session["Order"] = product.Id + ";" + product.Price + ";" + product.Number + ";";
                return Redirect("/Home/ProductList");
            }
            return View(product);
        }

        
        public ActionResult IndexBasket()
        {
            if (Session["Order"] != null)
            {
                int price = 0;
                string ListOrders = Session["Order"].ToString();
                string[] Orders = ListOrders.Split(';');
                List<Product> list = new List<Product>();   // список заказа
                for (int i = 0; i < Orders.Length - 1; i += 3)
                {
                    Product newProduct = new Product();
                    newProduct.Id = Int32.Parse(Orders[i]);
                    newProduct.Title = db.Products.Find(newProduct.Id).Title;
                    newProduct.Description = "";
                    newProduct.Price = Int32.Parse(Orders[i + 1]);
                    newProduct.Number = Int32.Parse(Orders[i + 2]);
                    list.Add(newProduct);
                    price += newProduct.Price * newProduct.Number;

                    // изменили количество оставшегося товара
                    Product EditCountProduct =  db.Products.Find(newProduct.Id);
                    EditCountProduct.Number -= newProduct.Number;
                    db.Entry(EditCountProduct).State = EntityState.Modified;
                    db.SaveChanges();
                }
                Session["Price"] = price;
                return View(list);
            }
            return Redirect("/Home/ProductList");
        }

        [HttpGet]
        public ActionResult Purchase()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Purchase(Customer customer)
        {
            Customer saveCustomer =  db.Customers.FirstOrDefault(p => p.SNP == customer.SNP && p.Email == customer.Email);
            if (saveCustomer == null)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                saveCustomer = db.Customers.FirstOrDefault(p => p.SNP == customer.SNP && p.Email == customer.Email);
            }
            Order order = new Order();
            order.Customer_Id = saveCustomer.Id;
            order.Price = (int)Session["Price"];
            db.Orders.Add(order);
            db.SaveChanges();
            Session["Price"] = null;
            Session["Order"] = null;
            return Redirect("/Home/ThankYouForPurchase");
        }

        
        public ActionResult ThankYouForPurchase()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAdmin(string login, string password)
        {
            admin.Add("admin", "admin");
            string value = "";
            if (admin.TryGetValue(login.ToLower(), out value) && value == password)
            {
                return Redirect("~/Home/ProductListAdmin");
            }
            else
            {
                ModelState.AddModelError(nameof(login), "Неверно имя пользователя или пароль!");
                return View();
            }
        }

        public ActionResult ProductListAdmin()
        {
            return View(db.Products);
        }

        public ActionResult AllOrders()
        {
            return View(db.Orders);
        }

        [HttpGet]
        public ActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Product product = db.Products.Find(id);
            if (product != null)
            {
                return View(product);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ProductListAdmin");
        }











        public ActionResult Index()
        {
            return View();
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
    }
}