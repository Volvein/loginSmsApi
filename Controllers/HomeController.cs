using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using System.Web.Security;
namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        TestLoginEntities db = new TestLoginEntities();
        // GET: Home
        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginDM aa)
        {
            using (var context = new TestLoginEntities())
            {
                var isEmailAlreadyExists = db.Logins.Any(x => x.EmailID == aa.EmailID && x.Password == aa.Password);
                var isNumberAlreadyExists = db.Logins.Any(x => x.Phone == aa.EmailID && x.Password == aa.Password);
                if (isEmailAlreadyExists)      
                {
                    FormsAuthentication.SetAuthCookie(aa.EmailID, false);
                    return RedirectToAction("Home", "Admin");
                }
                else if (isNumberAlreadyExists)
                {
                    TempData["Phone"] = aa.EmailID;                    
                    string otp = otpnum();
                    string mobileNo = aa.EmailID;
                    string SMSContents = otp + " is your One-Time Password, valid for 10 minutes only, Please do not share your OTP with anyone.";
                    string smsResult = SendSMS(mobileNo, SMSContents);
                    TempData["logOTP"] = otp;
                    TempData.Keep();
                    return RedirectToAction("LoginVerify", "Home");
                }
                return View();
            }
            
        }
        public ActionResult LoginVerify()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginVerify(LoginDM we)
        {
            string otp1 = (string)TempData["logOTP"];
            if (we.LoginOtp != null)
            {
                if (otp1==we.LoginOtp)
                {
                    FormsAuthentication.SetAuthCookie(we.LoginOtp, false);
                    return RedirectToAction("Home","Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid OTP");
                }
            }
            return View();
        }

        public ActionResult Signup()
        {
            try
            {
                List<State> statelist = db.States.ToList();
                ViewBag.state = new SelectList(statelist, "ID", "States");
            }catch(Exception) { }
                return View();
        }
        [HttpPost]
        public ActionResult Signup(LoginDM emp)
        {
            var isEmailAlreadyExists = db.Logins.Any(x => x.EmailID == emp.EmailID);
            if (isEmailAlreadyExists)
            {
                ModelState.AddModelError("EmailID", "User with this email already exists");
                return View(emp);
            }
            else
            {
                if (emp.EmailID != null && emp.Username != null && emp.Password != null && emp.ConformPassword != null)
                {

                    TempData["Name"] = emp.Username;
                    TempData["EmailID"] = emp.EmailID;
                    if (emp.Password == emp.ConformPassword)
                    {
                        TempData["Password"] = emp.Password;
                        return RedirectToAction("SignupNext", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Password");
                    }
                    TempData.Keep();
                }
                else 
                { 
                    ModelState.AddModelError("", "Invalid User");
                }
            }
            return View();
        }
        //Next Signup Page
        public ActionResult SignupNext()
        {
            try
            {
                List<State> statelist = db.States.ToList();
                ViewBag.state = new SelectList(statelist, "ID", "States");
            }
            catch (Exception) { }
                return View();
        }
        [HttpPost]
        public ActionResult SignupNext(LoginDM emp)
        {
            try
            {               
                    if (emp.Pincode != null && emp.City != null && emp.State != null && emp.Phone != null)
                    {
                        var isNumberAlreadyExists = db.Logins.Any(x => x.Phone == emp.Phone);
                        if (isNumberAlreadyExists)
                        {
                            ModelState.AddModelError("Phone", "User with this Number already exists");
                            return View(emp);
                        }
                        else
                        {
                            TempData["Phone"] = emp.Phone;
                            TempData["State"] = emp.State;
                            TempData["City"] = emp.City;
                            TempData["Pincode"] = emp.Pincode;
                            string otp = otpnum();
                            string mobileNo = emp.Phone;
                            string SMSContents = otp + " is your One-Time Password, valid for 10 minutes only, Please do not share your OTP with anyone.";
                            string smsResult = SendSMS(mobileNo, SMSContents);
                            TempData["OTP"] = otp;
                            TempData.Keep();
                            return RedirectToAction("OtpVarify", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid User Details");
                    }
                }
            catch (Exception)
            {
            } 
            return View();
        }
        public ActionResult OtpVarify()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OtpVarify(LoginDM yo)
        {
            string name = (string)TempData["Name"];
            string email = (string)TempData["EmailID"];
            string phone = (string)TempData["Phone"];
            string state = (string)TempData["State"];
            string password = (string)TempData["Password"];
            string city = (string)TempData["City"];
            string pincode = (string)TempData["Pincode"];
            string otp = (string)TempData["OTP"];
            if (yo.Otp != null)
            {
                if (yo.Otp == otp)
                {
                    Login obj = new Login
                    {
                        Username = name,
                        EmailID = email,
                        Phone = phone,
                        State = state,
                        City = city,
                        Pincode=pincode,
                        Password = password

                    };
                    db.Logins.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("SignupStatus", "Home");
                }               
            }
            else
            {
                ModelState.AddModelError("", "Invalid OTP");
            }
            return View();
        }
        public static string otpnum()
        {
            Random r = new Random();
            string OTP = r.Next(1000, 9999).ToString();
            string strrandom = OTP;
            return strrandom;
        }
        public static string SendSMS(string MblNo, string Msg)
        {
            string MainUrl = "http://sms.ezytm.com/api/SMS?";
            string strMobileno = MblNo;
            string URL = MainUrl + "UserID=3030&TokenID=43191&MobileNo=" + strMobileno + "&TextM=" + HttpUtility.UrlEncode(Msg) + "dude";
            string strResponce = Get(URL);
            string msg;
            if (strResponce.Equals("Fail"))
            {
                msg = "Fail";
            }
            else
            {
                msg = strResponce;
            }
            return msg;
        }
        public static string Get(string url)
        {
            string response = string.Empty;
            try
            {
                HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse())
                {
                    StreamReader sr = new StreamReader(httpres.GetResponseStream());
                    response = sr.ReadToEnd();
                    sr.Close();
                }

            }
            catch (Exception ex)
            {
                response = "CONN_ERROR-" + ex.Message;
                string comment = ", url=" + url;
            }
            return response;
        }
        public ActionResult SignupStatus()
        {
            ViewBag.name = TempData["Name"];
            ViewBag.email = TempData["EmailID"];
            return View();
        }
        [HttpPost]
        public ActionResult SignupStatus(LoginDM aa)
        {
            ViewBag.name = TempData["Name"];
            ViewBag.email = TempData["EmailID"];
            return RedirectToAction("Login", "Home");
        }
        
    }
}