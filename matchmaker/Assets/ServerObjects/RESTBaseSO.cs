using GPF.ServerObjects;
using System.Collections.Generic;
using System;

namespace ServerObjects.Test
{
    public abstract class RESTBaseSO : ServerObject
    {
        protected static Dictionary<string, string> GetParamsFromQueryString(string queryString)
        {
            var myParams = new Dictionary<string, string>();
            var pairParts = queryString.Substring(1).Split('&');
            foreach (var pair in pairParts)
            {
                try
                {
                    var parts = pair.Split('=');
                    myParams[parts[0]] = parts[1];
                }
                catch (Exception)
                {

                }
            }

            return myParams;
        }

    }
}