using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace httpMethodsApp
{
   public class SamplingPeriod
    {
       
       
        private List<Tuple<TimeSpan, TimeSpan>> samplingTimeAndPeriods;

        public SamplingPeriod()
        {

            samplingTimeAndPeriods = new List<Tuple<TimeSpan, TimeSpan>>();

        }

        public void removeAt(int index)
        {

            if (index > 0 && index < this.samplingTimeAndPeriods.Count)
            {
                this.samplingTimeAndPeriods.RemoveAt(index);
            }
        }

        public void addPeriodAtTime(TimeSpan period, TimeSpan atTime)
        {
            samplingTimeAndPeriods.Add(new Tuple<TimeSpan, TimeSpan>(period, atTime));
        }

       public void  setPeriodAtIndex(int index , TimeSpan period)
       {
           var data = this.samplingTimeAndPeriods[index];
           data = new Tuple<TimeSpan, TimeSpan>(period, data.Item2);
           this.samplingTimeAndPeriods[index] = data;
       }

       public void setAtTimeAtIndex(int index, TimeSpan atTime)
       {
           var data = this.samplingTimeAndPeriods[index];
           data = new Tuple<TimeSpan, TimeSpan>(data.Item1, atTime);
           this.samplingTimeAndPeriods[index] = data;
       }


       public Tuple<TimeSpan, TimeSpan>[] periodsAtTimes
       {
           get
           {
               return samplingTimeAndPeriods.ToArray();
           }
       }

       
       
    }
}
