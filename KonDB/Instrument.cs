﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonDB
{
    class Instrument
    {

        private string name;
        private string file;
        private int stars;
        private string tags;
        private long size;
        private int loadingFails;
        private bool isNkm;


        public Instrument(string name, string file, int stars, string tags, long size, int loadingFails)
        {
            this.name = name;
            this.file = file;
            this.stars = stars;
            this.tags = tags;
            this.size = size;
            this.loadingFails = loadingFails;
            this.isNkm = file.ToLower().EndsWith("nkm");
        }

        public string GetName()
        {
            return name;
        }

        public bool isNkmFile()
        {
            return isNkm;
        }


        public bool GetLoadingFails()
        {
            return loadingFails == 1;
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
