using System;
using System.Collections.Generic;        
using System.Diagnostics;

namespace LogPerformance{
  public static class MyCode{
  
    delegate void ToEvaluate(string format);
        
    static string LogAverageInterval(string format, decimal evaluationTime, ToEvaluate toEvaluate)
    {
      int counter = 0;
      Stopwatch sw = Stopwatch.StartNew();
      while(sw.ElapsedMilliseconds < evaluationTime)
      {
        sw.Start();
        toEvaluate(format);
        sw.Stop();
        counter++;
      }
      sw.Stop();
      decimal v = (evaluationTime * 1000000);
      return String.Format("average time: {1}ns | {3}ms", format.PadRight(13),Math.Round( v / counter, 1).ToString().PadRight(8), v, Math.Round(evaluationTime/counter, 8));
    }
    
    void SimpleDateTime(string format)
    {
      string test = DateTime.Now.ToString(format);
    }
    void LogDateTime(string format)
    {
      string test = DateTime.Now.ToString(format);
      QuickLog.WriteInfo(test);
    }
    void LogDateTimeMS(string format)
    {
      string test = DateTime.Now.ToString(format);
      QuickLog.WriteMSInfo(test);
    }
    void LogDateTimeMicro(string format)
    {
      string test = DateTime.Now.ToString(format);
      QuickLog.WriteMicroInfo(test);
    }
    
    static void Run(){
      Dictionary<string, ToEvaluate> toEvaluates = new Dictionary<string, ToEvaluate>()
      {
        {"SimpleDateTime", new ToEvaluate(SimpleDateTime) },
        {"LogDateTime", new ToEvaluate(LogDateTime) },
        {"LogDateTimeMS", new ToEvaluate(LogDateTimeMS) },
        {"LogDateTimeMicro", new ToEvaluate(LogDateTimeMicro) }
      };
      List<string> dtFormats = new List<string>()
      {
        "fff",
        "fffff",
        "fffffff",
        "HH:mm:ss:f",
        "HH:mm:ss:ffff"
      };
      
      Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
      foreach (KeyValuePair<string, ToEvaluate> kvp in toEvaluates)
      {
        dtFormats.ForEach(dtf =>
        {
          string fullTestName = String.Format("function: {0} | dateTimeFormat: {1}", kvp.Key.PadRight(16), dtf);
          string result = LogAverageInterval(dtf, 2000, kvp.Value);
          keyValuePairs.Add(fullTestName, result);                    
        });
      }
      foreach(KeyValuePair<string, string> kvp in keyValuePairs)
      {
        string label = "Test: " + kvp.Key;
        string result = "Result: " + kvp.Value;
        QuickLog.WriteInfo("{0} {1}", label.PadRight(65), result);
      }
    }    
  }
}
