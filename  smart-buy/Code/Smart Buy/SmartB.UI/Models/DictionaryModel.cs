using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Models
{
    public class DictionaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }

        public static DictionaryModel MapToDictionaryEntity(DictionaryModel dictionary)
        {
            DictionaryModel model = new DictionaryModel();
            model.Id = dictionary.Id;
            model.Name = dictionary.Name;
            model.ProductId = dictionary.ProductId;
            return model;
        }

    }
}