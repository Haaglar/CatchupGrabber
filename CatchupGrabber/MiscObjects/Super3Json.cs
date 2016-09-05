using System.Collections.Generic;

//Used with Super3
namespace CatchupGrabber.MiscObjects.Super3
{

    public class Estat
    {
        public bool actiu { get; set; }
        public object text { get; set; }
        public int id { get; set; }
    }

    public class Durada
    {
        public string text { get; set; }
        public int milisegons { get; set; }
    }

    public class DataEmissio
    {
        public string text { get; set; }
        public string utc { get; set; }
    }

    public class DataCaducitat
    {
        public string text { get; set; }
        public string utc { get; set; }
    }

    public class Tematica
    {
        public string text { get; set; }
        public int id { get; set; }
    }

    public class CodiEtic
    {
        public string text { get; set; }
        public string id { get; set; }
    }

    public class Informacio
    {
        public Estat estat { get; set; }
        public int id { get; set; }
        public int op { get; set; }
        public int programa_id { get; set; }
        public int capitol { get; set; }
        public string titol { get; set; }
        public string titol_complet { get; set; }
        public string slug { get; set; }
        public string programa { get; set; }
        public string tipus_contingut { get; set; }
        public string descripcio { get; set; }
        public string categoria_bb_ppal { get; set; }
        public Durada durada { get; set; }
        public DataEmissio data_emissio { get; set; }
        public DataCaducitat data_caducitat { get; set; }
        public Tematica tematica { get; set; }
        public CodiEtic codi_etic { get; set; }
        public string logo { get; set; }
        public object domini { get; set; }
    }

    public class Subtitols
    {
        public string text { get; set; }
        public string iso { get; set; }
        public string url { get; set; }
        public string format { get; set; }
    }

    public class Media
    {
        public string geo { get; set; }
        public string format { get; set; }
        public string url { get; set; }
    }

    public class Imatges
    {
        public int amplada { get; set; }
        public int alcada { get; set; }
        public string url { get; set; }
    }

    public class Prerolls
    {
        public int instances { get; set; }
    }

    public class Midrolls
    {
        public int instances { get; set; }
    }

    public class Postrolls
    {
        public int instances { get; set; }
    }

    public class Data
    {
        public Prerolls prerolls { get; set; }
        public Midrolls midrolls { get; set; }
        public Postrolls postrolls { get; set; }
    }

    public class Adrules
    {
        public int duration_min { get; set; }
        public int duration_max { get; set; }
        public Data data { get; set; }
    }

    public class Config
    {
        public int sas_siteid { get; set; }
        public int sas_pageid { get; set; }
        public string sas_pagename { get; set; }
        public string sas_format_linears { get; set; }
    }

    public class Sas
    {
        public object options { get; set; }
        public Adrules adrules { get; set; }
        public Config config { get; set; }
    }

    public class Publicitat
    {
        public Sas sas { get; set; }
    }

    public class Parametres
    {
        public string eVar10 { get; set; }
        public string prop10 { get; set; }
        public string eVar11 { get; set; }
        public string prop11 { get; set; }
        public string eVar12 { get; set; }
        public string prop12 { get; set; }
        public string eVar13 { get; set; }
        public string prop13 { get; set; }
        public int eVar14 { get; set; }
        public int prop14 { get; set; }
        public int eVar15 { get; set; }
        public int prop15 { get; set; }
        public int eVar16 { get; set; }
        public int prop16 { get; set; }
        public string eVar17 { get; set; }
        public string prop17 { get; set; }
        public string eVar18 { get; set; }
        public string prop18 { get; set; }
        public string eVar19 { get; set; }
        public string prop19 { get; set; }
        public string eVar20 { get; set; }
        public string prop20 { get; set; }
        public string eVar21 { get; set; }
        public string prop21 { get; set; }
        public string eVar22 { get; set; }
        public string prop22 { get; set; }
        public string eVar23 { get; set; }
        public string prop23 { get; set; }
        public string eVar24 { get; set; }
        public string prop24 { get; set; }
        public string eVar25 { get; set; }
        public string prop25 { get; set; }
        public string eVar26 { get; set; }
        public string prop26 { get; set; }
        public string eVar27 { get; set; }
        public string prop27 { get; set; }
        public string eVar28 { get; set; }
        public string prop28 { get; set; }
        public string eVar29 { get; set; }
        public string prop29 { get; set; }
        public int eVar30 { get; set; }
        public int prop30 { get; set; }
        public int eVar31 { get; set; }
        public int prop31 { get; set; }
        public string eVar32 { get; set; }
        public string prop32 { get; set; }
        public string eVar33 { get; set; }
        public string prop33 { get; set; }
        public string eVar34 { get; set; }
        public string prop34 { get; set; }
        public string eVar35 { get; set; }
        public string prop35 { get; set; }
        public string eVar36 { get; set; }
        public string prop36 { get; set; }
        public int eVar37 { get; set; }
        public int prop37 { get; set; }
        public int eVar38 { get; set; }
        public int prop38 { get; set; }
        public string eVar39 { get; set; }
        public string prop39 { get; set; }
        public string eVar54 { get; set; }
        public string prop54 { get; set; }
        public string eVar58 { get; set; }
        public string prop58 { get; set; }
        public string eVar59 { get; set; }
        public string prop59 { get; set; }
        public string eVar60 { get; set; }
        public string prop60 { get; set; }
        public string eVar62 { get; set; }
        public string prop62 { get; set; }
    }

    public class Sitecatalyst
    {
        public string compte { get; set; }
        public string nom { get; set; }
        public int durada { get; set; }
        public string reproductor { get; set; }
        public int directe { get; set; }
        public string tipus { get; set; }
        public Parametres parametres { get; set; }
    }

    public class Parametres2
    {
        public int c1 { get; set; }
        public int c2 { get; set; }
        public string c3 { get; set; }
        public string c4 { get; set; }
        public string c5 { get; set; }
    }

    public class Comscore
    {
        public string url { get; set; }
        public Parametres2 parametres { get; set; }
    }

    public class Content
    {
        public int id_dty { get; set; }
        public string variant { get; set; }
    }

    public class Parametres3
    {
        public string pid { get; set; }
        public bool directe { get; set; }
        public string tipus { get; set; }
        public Content content { get; set; }
    }

    public class Matr
    {
        public Parametres3 parametres { get; set; }
    }

    public class Audiencies
    {
        public Sitecatalyst sitecatalyst { get; set; }
        public Comscore comscore { get; set; }
        public string rating { get; set; }
        public Matr matr { get; set; }
    }

    public class Super3Json
    {
        public Informacio informacio { get; set; }
        public Subtitols subtitols { get; set; }
        public Media media { get; set; }
        public Imatges imatges { get; set; }
        public Publicitat publicitat { get; set; }
        public Audiencies audiencies { get; set; }
    }

}
