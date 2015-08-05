using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
    public class Ranking : RawDataWrapper<kcsapi_ranking>, IIdentifiable
    {
        public int Id
        {
            get { return this.RawData.api_no; }
        }

        public int Rate
        {
            get { return this.RawData.api_rate; }
        }

        public string NickName
        {
            get { return this.RawData.api_nickname; }
        }

        public string Comment
        {
            get { return this.RawData.api_comment; }
        }

        public int Rank
        {
            get { return this.RawData.api_rank; }
        }

        public int Level
        {
            get { return this.RawData.api_level; }
        }

        public int Experience
        {
            get { return this.RawData.api_experience; }
        }

        public int Flag
        {
            get { return this.RawData.api_flag; }
        }

        public Ranking(kcsapi_ranking rawData) : base(rawData) { }

        public override string ToString()
        {
            return string.Format("NickName = {0}", this.NickName);
        }
    }
}
