using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace AuthenticationChecker
{
  public class AuthenticationState
  {
    public string ReturnUrl { get; internal set; }
    public string UserId { get; internal set; }
    public string FrontEnd { get; internal set; }
  }
  
  interface IStateHandler
  {
    void Add(string key, AuthenticationState state);
    AuthenticationState GetAndRemove(string key);
  }
  public class StateHandler : IStateHandler
  {
    private static Dictionary<string, AuthenticationState> s_state = new Dictionary<string, AuthenticationState>();
    public void Add(string key, AuthenticationState state)
    {
      s_state.Add(key, state);
    }

    public AuthenticationState GetAndRemove(string key)
    {
      if (s_state.TryGetValue(key, out var state))
      {
        {
          if (s_state.Remove(key))
            return state;
        }
      }
      
      return null;
    }
  }
}