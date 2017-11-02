using System.Runtime.Serialization;

namespace Loansv2.Models.Chart
{
    [DataContract]
    public class DataPoint
    {
        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public decimal? X;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public decimal? Y;

        //Explicitly setting the name to be used while serializing to JSON. 
        [DataMember(Name = "label")]
        public string Label;


        #region Constructors
        public DataPoint()
        {
        }

        public DataPoint(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public DataPoint(string label, decimal y)
        {
            Y = y;
            Label = label;
        }
        #endregion
    }
}