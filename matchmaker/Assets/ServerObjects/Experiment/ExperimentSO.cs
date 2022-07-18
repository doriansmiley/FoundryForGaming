using GPF.ServerObjects;
using System;
using System.Collections.Generic;

namespace ServerObjects
{
  [DataStorePath("sync.experiment")]
  [Syncable]
  [Register("experiment")]
  public class ExperimentSO : ServerObject
  {
    public Dictionary<string, string> experiments = new Dictionary<string, string>();

    public class ChangeExperiments : ServerObjectMessage
    {
      public Dictionary<string, string> experiments;
    }
    public void Handler(ChangeExperiments msg)
    {
      if (msg.experiments == null)
        experiments = new Dictionary<string, string>();
      else
        experiments = msg.experiments;
    }
  }
}
