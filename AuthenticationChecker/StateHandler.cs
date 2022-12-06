using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using AuthenticationChecker.Controllers;
using log4net;

namespace AuthenticationChecker
{
  public class AuthenticationState
  {
    public string ReturnUrl { get; internal set; }
    public string UserId { get; internal set; }
    public string FrontEnd { get; internal set; }

    public override string ToString()
    {
      return $"ReturnUrl='{ReturnUrl}', UserId='{UserId}', Frontend='{FrontEnd}'";
    }
  }
  
  interface IStateHandler
  {
    void Add(string key, AuthenticationState state);
    AuthenticationState GetAndRemove(string key);
  }
  public class StateHandler : IStateHandler
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(StateHandler));
    private static Dictionary<string, AuthenticationState> s_state = new Dictionary<string, AuthenticationState>();
    public void Add(string key, AuthenticationState state)
    {
      s_log.Info($"Adding state with key {key}. State is {state}.");
      s_state.Add(key, state);
    }

    public AuthenticationState GetAndRemove(string key)
    {
      if (s_state.TryGetValue(key, out var state))
      {
        {
          if (s_state.Remove(key))
          {
            s_log.Info($"State with key {key} removed.");
            return state;
          }
        }
      }
      
      return null;
    }
  }
}