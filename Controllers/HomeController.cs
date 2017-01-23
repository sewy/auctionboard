using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using auctionboard.Factory;
using auctionboard.Models;
using Microsoft.AspNetCore.Http;

namespace auctionboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserFactory userFactory;
        public HomeController()
        {
            userFactory = new UserFactory();
        }


        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.errors = ModelState.Values;
            ViewBag.loginerrors = "";
            return View("Index");
        }
        [HttpPost]
        [Route("/register")]
        public IActionResult register(User newuser)
        {
            if(ModelState.IsValid)
            {
                userFactory.Add(newuser);
                return login(newuser.username, newuser.password);
            }
            ViewBag.errors = ModelState.Values;
            return Index();
        }

        [HttpPost]
        [RouteAttribute("/login")]
        public IActionResult login(string username, string password)
        {
            if(userFactory.FindByUsername(username) != null)
            {
                User check = userFactory.FindByUsername(username);

                if(check.password == password)
                {
                    HttpContext.Session.SetInt32("userid", check.id);
                    return RedirectToAction("auctions");
                }
            }
            ViewBag.errors = ModelState.Values;
            ViewBag.loginerrors = "Incorrect Email or Password.";
            return View("Index");
        }

        [HttpGet]
        [Route("/auctions")]
        public IActionResult auctions()
        {
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            User user = userFactory.FindByID((int)HttpContext.Session.GetInt32("userid"));
            ViewBag.auctions = userFactory.FindAllAuctions();
            ViewBag.user = user;
            foreach(var auction in ViewBag.auctions)
            {
                double hours = Math.Truncate((auction.end_date - DateTime.Now).TotalHours);
                if(hours > 24)
                {
                    string remain = Math.Truncate((hours / 24)).ToString() + " Days";
                    auction.converted = remain;
                }
                else{
                    string remain = hours.ToString() + " Hours";
                    auction.converted = remain;
                }
            }
            return View("auctions");
        }

        [HttpGet]
        [Route("/newauction")]
        public IActionResult newauction()
        {
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.errors = ModelState.Values;
            return View("newauction");
        }

        [HttpPost]
        [Route("/createauction")]
        public IActionResult createauction(Auction newauction)
        {
            if(ModelState.IsValid)
            {
                newauction.user_id = (int)HttpContext.Session.GetInt32("userid");
                newauction.bidder_id = (int)HttpContext.Session.GetInt32("userid");
                userFactory.AddAuction(newauction);
                return RedirectToAction("auctions");
            }
            ViewBag.errors = ModelState.Values;
            return View("newauction");
        }

        [HttpGet]
        [Route("/auction/{id}")]
        public IActionResult auctionpage(int id)
        {   
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.auction = userFactory.FindAuction(id);
            ViewBag.bidder = userFactory.FindBidder(id);
            double hours = Math.Truncate((ViewBag.auction.end_date - DateTime.Now).TotalHours);
            if(hours > 24)
            {
                string remain = Math.Truncate((hours / 24)).ToString() + " Days";
                ViewBag.auction.converted = remain;
            }
            else{
                string remain = hours.ToString() + " Hours";
                ViewBag.auction.converted = remain;
            }
            return View("auction");
        }

        [HttpPost]
        [Route("/bid/{id}")]
        public IActionResult bid(int id, decimal bid)
        {
            ViewBag.auction = userFactory.FindAuction(id);
            ViewBag.bidder = userFactory.FindBidder(id);
            ViewBag.money = userFactory.FindMoney((int)HttpContext.Session.GetInt32("userid"));
            if(bid > (decimal)ViewBag.auction.bid && ViewBag.money.money >= bid)
            {
                if(ViewBag.auction.user_id != (int)HttpContext.Session.GetInt32("userid"))
                {
                    if(ViewBag.auction.user_id != ViewBag.auction.bidder_id)
                    {
                        userFactory.RefundUser(ViewBag.bidder.bidder_id, (decimal)ViewBag.auction.bid);
                    }
                    userFactory.ChargeUser((int)HttpContext.Session.GetInt32("userid"), bid);
                    userFactory.UpdateAuction((int)HttpContext.Session.GetInt32("userid"), id ,bid);
                }

            }
            return RedirectToAction("auctionpage", new{id=id});
        }

        [HttpPost]
        [Route("/delete/{id}")]
        public IActionResult delete(int id)
        {        
            ViewBag.auction = userFactory.FindAuction(id);
            ViewBag.bidder = userFactory.FindBidder(id);
            if(ViewBag.auction.user_id != ViewBag.auction.bidder_id)
                    {
                        userFactory.RefundUser(ViewBag.bidder.bidder_id, (decimal)ViewBag.auction.bid);
                    }
            userFactory.DeleteAuction(id);
            return RedirectToAction("auctions");
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult logoff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
