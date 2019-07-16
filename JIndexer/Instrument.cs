using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIndexer
{
    class Instrument
    {

        private string name;
        private string file;
        private int stars;
        private string tags;
        private long size;
        private int loadingFails;


        public Instrument(string name, string file, int stars, string tags, long size, int loadingFails)
        {
            this.name = name;
            this.file = file;
            this.stars = stars;
            this.tags = tags;
            this.size = size;
            this.loadingFails = loadingFails;
        }

        public string GetName()
        {
            return name;
        }

        public int GetLoadingFails()
        {
            return loadingFails;
        }

        public string GetFile( )
        {
            return file;
        }

        public int GetStars()
        {
            return stars;
        }

        public long GetSize()
        {
            return size;
        }

        public string GetTags()
        {
            return tags;
        }

    }
}
