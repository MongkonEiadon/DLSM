using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class UploadPictureRequest
    {
        public string Key { get; set; }
        public string StID { get; set; }
        public string WH_ID { get; set; }
        public string cardEIN { get; set; }

        public string Status { get; set; }
        public string ImageSize { get; set; }
        public string PartSize { get; set; }
        public string PartPosition { get; set; }
        public string PartData { get; set; }

        public string person_image { get; set; }
    }
}