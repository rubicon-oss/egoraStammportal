using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AuthenticationChecker.Properties;
using log4net;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationChecker.Controllers
{
  
  public class AuthenticateController : Controller
  {

    private static readonly ILog s_log = LogManager.GetLogger(typeof(AuthenticateController));
    private static readonly OpenIdConnectConfiguration Configuration;
    private static readonly IdpInfo Info;

    static AuthenticateController()
    {
      var configString = System.IO.File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, Settings.Default.IdpInfoFile));
      Info = JsonSerializer.Deserialize<IdpInfo>(configString);
      if (Info == null)
        throw new ConfigurationErrorsException($"IdpInfo {Settings.Default.IdpInfoFile} not valid.");

      s_log.Info($"retrieve information from {Info.DiscoveryEndpoint}");
      var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(Info.DiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
      Configuration = configManager.GetConfigurationAsync().Result;
      s_log.Info($"AuthorizationEndpoint is '{Configuration.AuthorizationEndpoint}'");
      s_log.Info($"TokenEndpoint is '{Configuration.TokenEndpoint}'");
    }
    public ActionResult Start(string returnUrl, string userId)
    {
      ViewBag.ReturnUrl = returnUrl;
      ViewBag.UserId = userId;
      return View();
    }

    public ActionResult End()
    {
      ViewBag.CheckResult = "OK";
      Response.Headers.Add("X-Egora-Authentication-UserId", "OK");
      return View();
    }

    public ActionResult Authorize(string returnUrl, string userId, string frontend)
    {
      if (string.IsNullOrEmpty(userId))
        throw new ApplicationException($"Parameter {nameof(userId)} is missing.");

      var stateHandler = new StateHandler();
      var stateKey = Guid.NewGuid().ToString();
      stateHandler.Add(stateKey, new AuthenticationState() { ReturnUrl = returnUrl, UserId = userId, FrontEnd = frontend });

      if (Settings.Default.DummyMode)
      {
        var dummyParameters = new Dictionary<string, string>()
        {
          {"code", "dummy" },
          {"state", stateKey}
        };
        Response.Redirect($"{GetActionUrl(nameof(Receive), frontend)}?{GetQueryString(dummyParameters)}");
        return null;
      }

      var parameters = new Dictionary<string, string>()
      {
        {"response_type", "code" },
        {"client_id", Info.ClientId },
        {"redirect_uri", GetActionUrl(nameof(Receive), frontend)},
        {"scope", "openid profile email"},
        {"state", stateKey}
      };
      var query = GetQueryString(parameters);
      string idpUrl = Configuration.AuthorizationEndpoint + $"?{query}";
      s_log.Debug($"redirecting to {idpUrl}");
      Response.Redirect(idpUrl);
      return null;
    }

    private static string GetQueryString(Dictionary<string, string> parameters)
    {
      return string.Join("&", parameters.Select(p => $"{UrlEncoder.Default.Encode(p.Key)}={UrlEncoder.Default.Encode(p.Value)}"));
    }

    public ActionResult Receive(string code, string state)
    {
      ViewBag.Code = code;
      ViewBag.State = state;
      var stateHandler = new StateHandler();
      var data = stateHandler.GetAndRemove(state);
      if (data == null)
      {
        s_log.Error($"unknown state '{state}'.");
        ViewBag.Error = $"unknown state '{state}'.";
        return View();
      }
      if (Settings.Default.DummyMode)
      {
        if (string.IsNullOrEmpty(data.ReturnUrl))
        {
          ViewBag.Title = "Dummy Login";
          ViewBag.AccountName = data.UserId;
          return View();
        }
        SetHeader(data.UserId, "Dummy");
        Response.Redirect(data.ReturnUrl, true);
      }

      var client = new HttpClient();

      var content = new FormUrlEncodedContent(
        new Dictionary<string, string>()
        {
          { "grant_type", "authorization_code" },
          { "code", code },
          { "redirect_uri", GetActionUrl(nameof(Receive), data.FrontEnd) },
          { "client_id", Info.ClientId },
          { "client_secret", Info.ClientSecret },
        });
      s_log.Info($"exchange data with code='{code}'");
      var response = client.PostAsync(Configuration.TokenEndpoint, content).Result;
      if (response == null || response.Content == null)
      {
        throw new ApplicationException($"Idp Response or Idp Response Content is null.");
      }
      
      var responseContent = response.Content.ReadAsStringAsync().Result;
      
      s_log.Debug($"exchange data received:'{responseContent}'");
      var json = JsonDocument.Parse(responseContent);
      var idToken = json.RootElement.GetProperty("id_token").ToString();
      var tokenHandler = new JwtSecurityTokenHandler();

      var signingKeys = Configuration.SigningKeys;

      var validationParameters = new TokenValidationParameters
      {
        RequireExpirationTime = true,
        RequireSignedTokens = true,
        ValidateIssuer = true,
        ValidIssuer = Info.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeys = signingKeys,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2),
        ValidateAudience = true,
        ValidAudience = Info.ClientId,
      };

      SecurityToken validatedToken;
      try
      {
        var principal = tokenHandler.ValidateToken(idToken, validationParameters, out validatedToken);
      }
      catch (Exception e)
      {
        s_log.Error("Error validating token.",e);
        throw;
      }

      var jwt = new JwtSecurityToken(idToken);

      ViewBag.Identity = string.Join(", ", jwt.Claims.Select(c => $"{c.Type}={c.Value}"));
      string firstName = jwt.Claims.First(c => c.Type == "given_name").Value;
      string lastName = jwt.Claims.First(c => c.Type == "family_name").Value;
      var accountName =new ADHelper().FindUser(firstName, lastName, data.UserId)?.Properties["userPrincipalName"].Value;
      ViewBag.AccountName = accountName;
      ViewBag.ReturnUrl = data.ReturnUrl;
      SetHeader(accountName?.ToString(), validatedToken?.ToString());

      if (!string.IsNullOrEmpty(data.ReturnUrl))
        Response.Redirect(data.ReturnUrl, true);

      return View();
    }

    private void SetHeader(string userId, string data)
    {
      Response.Headers.Add("X-Egora-Authentication-UserId", userId);
      Response.Headers.Add("X-Egora-Authentication-UserData", data);
    }

    private string GetActionUrl(string action, string frontend)
    {
      var actionPath = Url.Action(action);

      if (string.IsNullOrEmpty(frontend))
        frontend = Request.Url.GetLeftPart(UriPartial.Authority);
      
      if (frontend.EndsWith("/"))
        frontend = frontend.TrimEnd('/');

      return frontend + actionPath;
    }
  }
}