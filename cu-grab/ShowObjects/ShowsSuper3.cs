using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab.Shows.Super3
{
    public class CodiEtic
    {
        public string id { get; set; }
        public string desc { get; set; }
    }

    public class Bband
    {
        public string id { get; set; }
        public string desc { get; set; }
    }

    public class Idioma
    {
        public string id { get; set; }
        public string desc { get; set; }
    }

    public class Imatge
    {
        public string text { get; set; }
        public string mida { get; set; }
    }

    public class Imatges
    {
        public List<Imatge> imatge { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string data_publicacio { get; set; }
        public string data_modificacio { get; set; }
        public string titol { get; set; }
        public string url { get; set; }
        public CodiEtic codi_etic { get; set; }
        public Bband bband { get; set; }
        public Idioma idioma { get; set; }
        public Imatges imatges { get; set; }
        public override string ToString()
        {
            return titol;
        }
    }

    public class Items
    {
        public int num { get; set; }
        public List<Item> item { get; set; }
    }

    public class Resposta
    {
        public Items items { get; set; }
        public string status { get; set; }
    }

    public class ShowsSuper3
    {
        public Resposta resposta { get; set; }
    }
}
