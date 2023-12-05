using JazzCashIntegeration.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace JazzCashIntegeration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation($"MerchantID: {_configuration["JazzCash:MerchantID"]}");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
      



        [HttpGet]
        public ActionResult Transaction()
        {
            return View();
        }

        

        [HttpPost]
        public async Task<ActionResult> Transaction(TransactionViewModel model)
        {
            try
            {
                if (model == null)
                {
                    TempData["ErrorMessage"] = "Model is null";
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (_configuration != null)
                {
                    var merchantId = _configuration["JazzCash:MerchantID"];
                    var password = _configuration["JazzCash:Password"];
                    var returnURL = _configuration["JazzCash:ReturnURL"];
                    var apiKey = _configuration["JazzCash:ApiKey"];

                    if (merchantId == null || password == null || returnURL == null || apiKey == null)
                    {
                        TempData["ErrorMessage"] = "One or more configuration values are null";
                        return View(model);
                    }

                    var pp_payload = new
                    {
                        pp_Version = "1.1",
                        pp_TxnType = "MWALLET",
                        pp_Language = "EN",
                        pp_MerchantID = merchantId,
                        pp_SubMerchantID = "",
                        pp_Password = password,
                        pp_BankID = "",
                        pp_ProductID = "",
                        pp_TxnRefNo = "0231205115329",
                        pp_Amount = model.Amount,
                        pp_TxnCurrency = "PKR",
                        pp_TxnDateTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        pp_BillReference = model.BillReference,
                        pp_Description = model.Description,
                        pp_TxnExpiryDateTime = "",
                        pp_ReturnURL = returnURL,
                        pp_SecureHash = "",
                        ppmpf_1 = model.DestinationNumber,
                        ppmpf_2 = "",
                        ppmpf_3 = "",
                        ppmpf_4 = "",
                        ppmpf_5 = ""
                    };

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("Api-Key", apiKey);

                        var response = await httpClient.PostAsJsonAsync("https://sandbox.jazzcash.com.pk/ApplicationAPI/API/Payment/DoTransaction", pp_payload);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = await response.Content.ReadFromJsonAsync<JazzCashResponse>();
                            if (response.StatusCode == HttpStatusCode.OK) {
                                TempData["pp_ResponseMessage"] = "Transaction Completed!";
                            
                            }
                            else
                            {
                                TempData["pp_ResponseMessage"] = responseData.pp_ResponseMessage;
                            }
                            TempData["pp_ResponseCode"] = responseData.pp_ResponseCode;
                            TempData["RRN"] = responseData.RRN;
                            TempData["DoShow"] = true;

                            return View("TransactionResponse",responseData );
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "JazzCash API request failed";
                            return View(model);
                        }
                    }
                }
                else
                {
                    return View(model);
                }
                
              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Transaction method.");
                throw;
            }
        }
        public IActionResult TransactionResponse()
        {
            return View();
        }


    }
}
