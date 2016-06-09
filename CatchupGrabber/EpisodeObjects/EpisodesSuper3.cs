using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatchupGrabber.EpisodeObjects.Super3
{
    //Temp fix class
    public class Canal
    {
        public string id { get; set; }
        public string desc { get; set; }
        public bool main { get; set; }
    }

    public class Canals
    {
        public Canal canal { get; set; }
    }

    public class Bband
    {
        public string id { get; set; }
        public string desc { get; set; }
        public bool main { get; set; }
    }

    public class Idioma
    {
        public string id { get; set; }
        public string desc { get; set; }
        public bool main { get; set; }
    }

    public class Geolocalitzacio
    {
        public string id { get; set; }
        public string desc { get; set; }
        public bool main { get; set; }
    }

    public class Programa
    {
        public int id { get; set; }
        public string tipologia { get; set; }
        public string desc { get; set; }
        public string nom { get; set; }
    }

    public class Programes
    {
        public List<Programa> programa { get; set; }
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

    public class Puidioma
    {
        public string id { get; set; }
        public string desc { get; set; }
        public bool main { get; set; }
    }

    public class CodiEtic
    {
        public string id { get; set; }
        public string desc { get; set; }
        public bool main { get; set; }
    }

    public class Target
    {
        public string id { get; set; }
        public string desc { get; set; }
        public bool main { get; set; }
    }

    public class Targets
    {
        public Target target { get; set; }
    }

    public class Item
    {
        public Canals canals { get; set; }
        public string entradeta { get; set; }
        public string data_caducitat { get; set; }
        public int versio { get; set; }
        public string durada { get; set; }
        public string data_publicacio { get; set; }
        public Bband bband { get; set; }
        public Idioma idioma { get; set; }
        public string domini { get; set; }
        public string tipus_contingut { get; set; }
        public string avantitol { get; set; }
        public Geolocalitzacio geolocalitzacio { get; set; }
        public Programes programes { get; set; }
        public Imatges imatges { get; set; }
        public int id { get; set; }
        public string programa { get; set; }
        public Puidioma puidioma { get; set; }
        public int produccio { get; set; }
        public string data_modificacio { get; set; }
        public string titol { get; set; }
        public string data_emissio { get; set; }
        public CodiEtic codi_etic { get; set; }
        public int capitol { get; set; }
        public Targets targets { get; set; }
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

    public class Paginacio
    {
        public int total_items { get; set; }
        public int items_pagina { get; set; }
        public int pagina_actual { get; set; }
        public int total_pagines { get; set; }
    }

    public class Resposta
    {
        public Items items { get; set; }
        public string status { get; set; }
        public Paginacio paginacio { get; set; }
    }

    public class EpisodesSuper3
    {
        public Resposta resposta { get; set; }
    }
}
