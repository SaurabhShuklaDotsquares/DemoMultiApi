using EC.API.ViewModels;
using EC.Service.Shippings;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using ToDo.WebApi.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using EC.Core.LIBS;
using EC.API.ViewModels.SiteKey;
using EC.Core.Enums;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using EC.Core;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Collections;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using MathNet.Numerics;
using EC.Service.Currency_data;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettingManagerController : BaseAPIController
    {
        #region Constructor
        private ISettingService _settingService;
        private ICurrenciesdataService _currenciesdataService;
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
        public SettingManagerController(ISettingService settingService, ICurrenciesdataService currenciesdataService)
        {
            this._settingService = settingService;
            _currenciesdataService = currenciesdataService;
        }
        #endregion

        #region Get setting List

        [Route("/users/user-config")]
        [HttpGet]
        public IActionResult GetSettingList()
        {
            try
            {
                string Message = string.Empty;
                SettingManagerViewModel model = new SettingManagerViewModel();
                string[] manager = Enum.GetNames(typeof(SettingManager));
                var settingList = _settingService.GetSettingListByManager(manager);

                if (settingList != null && settingList.Any())
                {
                    // General List
                    var generalList = settingList.Where(x => x.Manager == SettingManager.general.ToDescription());
                    if (generalList != null && generalList.Any())
                    {
                        foreach (var general in generalList)
                        {
                            model.general.Add(general.Slug, general.ConfigValue);
                        }
                    }

                    // Theme Image
                    var themeImage = settingList.Where(x => x.Manager == SettingManager.theme_images.ToDescription());
                    if (themeImage != null && themeImage.Any())
                    {
                        foreach (var themeimage in themeImage)
                        {
                            model.theme_images.Add(themeimage.Slug, SiteKey.ImagePath + "/Uploads/" + themeimage.ConfigValue);
                        }
                    }

                    // Social List
                    var social = settingList.Where(x => x.Manager == SettingManager.social.ToDescription());
                    var socialDictionary = new Dictionary<string, string>();
                    if (social != null && social.Any())
                    {
                        foreach (var socialItem in social)
                        {
                            model.social.Add(socialItem.Slug, socialItem.ConfigValue);
                        }
                    }
                    Message = "Settings fetch successfully.";
                }

                // Get Currency List
                var currency = _currenciesdataService.GetCurrencyDataList();
                if (currency != null && currency.Any())
                {
                    foreach (var item in currency)
                    {
                        CurrencyViewModel currencyView = new CurrencyViewModel();
                        currencyView.id = Convert.ToInt32(item.Id);
                        currencyView.currency_id = item.CurrencyId;
                        if (item.Currency != null)
                        {
                            CurrencyModel currencyModel = new CurrencyModel();
                            currencyModel.id = item.Currency.Id;
                            currencyModel.iso = item.Currency.Iso;
                            currencyModel.name = item.Currency.Name;
                            currencyModel.symbol = item.Currency.Symbol;
                            currencyModel.symbol_native = item.Currency.SymbolNative;
                            currencyView.currency = currencyModel;
                        }
                        model.currency.Add(currencyView);
                    }
                }

                if (!settingList.Any() && !currency.Any())
                {
                    Message = "Record Not Found.";
                }

                return Ok(new { error = false, Data = model, message = Message, code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

    }
}
