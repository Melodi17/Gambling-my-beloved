﻿namespace Gambling_my_beloved.Models;

public class Company
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }

    public string CEO { get; set; } = GenerateCEOName();
    public string LogoUrl { get; set; }
    
    public decimal Controversy { get; set; }
    
    public virtual List<Stock> Stocks { get; set; }
    public List<Industry> Industries { get; set; }

    public override string ToString() => this.Name;

    public static readonly string[] FirstNames =
    {
        "Dorothea",
        "Jasmina",
        "Belvia",
        "Laurella",
        "Gail",
        "Gladys",
        "Jamima",
        "Rosene",
        "Coraline",
        "Nari",
        "Orsa",
        "Nellie",
        "Adriena",
        "Nada",
        "Aurore",
        "Beryle",
        "Valerye",
        "Kaycee",
        "Alverta",
        "Tracie",
        "Stormie",
        "Orelia",
        "Mariquilla",
        "Maribel",
        "Bobine",
        "Wilow",
        "Sheila",
        "Bettine",
        "Elaine",
        "Hildegarde",
        "Delphinia",
        "Lorna",
        "Dayle",
        "Bettye",
        "Kalila",
        "Grazia",
        "Kylila",
        "Leeann",
        "Rozella",
        "Adaline",
        "Maris",
        "Madonna",
        "Jemie",
        "Dionne",
        "Christiana",
        "Janaya",
        "Evangelia",
        "Carmel",
        "Nedda",
        "Fredia",
        "Eadith",
        "Genovera",
        "Madelin",
        "Gerti",
        "Reena",
        "Kiri",
        "Odella",
        "Lilli",
        "Madlin",
        "Carmina",
        "Roobbie",
        "Rani",
        "Indira",
        "Belita",
        "Lynnea",
        "Chelsy",
        "Celle",
        "Keriann",
        "Christy",
        "Cybill",
        "Prudy",
        "Chiquita",
        "Jeni",
        "Clari",
        "Nadine",
        "Meagan",
        "Aurelie",
        "Kizzee",
        "Gilberte",
        "Celisse",
        "Carmelina",
        "Adriana",
        "Charita",
        "Merle",
        "Lavina",
        "Nevsa",
        "Beverly",
        "Marillin",
        "Marty",
        "Cassaundra",
        "Dolley",
        "Garnet",
        "Tiffany",
        "Mira",
        "Storm",
        "Lurette",
        "Ilise",
        "Gabriel",
        "Daile",
        "Ginnie",
    };

    public static readonly string[] LastNames =
    {
        "Voletta",
        "Ulrick",
        "Armand",
        "Nertie",
        "Georgeanne",
        "Kinnon",
        "Friedland",
        "Adley",
        "Gyasi",
        "Jerrol",
        "Janella",
        "Market",
        "Sanalda",
        "Darin",
        "Patrich",
        "Guadalupe",
        "Dick",
        "Kalie",
        "Aubert",
        "Diarmit",
        "Descombes",
        "Hertz",
        "Millicent",
        "McDonald",
        "Lowrie",
        "Yovonnda",
        "Domel",
        "Noma",
        "Ormiston",
        "Vudimir",
        "Naiditch",
        "Timi",
        "Summers",
        "Stortz",
        "Hess",
        "Kimmie",
        "Quintessa",
        "Sloane",
        "Fenner",
        "Tice",
        "Byrann",
        "Fenny",
        "Colman",
        "Down",
        "Power",
        "Ralf",
        "Allrud",
        "Mannes",
        "Durkin",
        "Yager",
        "Ietta",
        "Rosenbaum",
        "Casabonne",
        "Jereme",
        "Rebak",
        "Vail",
        "Klarika",
        "Troy",
        "Blondelle",
        "Leonanie",
        "Dorelle",
        "Thevenot",
        "Nora",
        "Chancellor",
        "Sanchez",
        "Bunny",
        "Ludovika",
        "Jerrilyn",
        "Warder",
        "Hebrew",
        "Sebastian",
        "Triny",
        "Averil",
        "Coniah",
        "Ewer",
        "Lucine",
        "Maxama",
        "Pisarik",
        "Eustazio",
        "Arola",
        "Esmerelda",
        "Lattie",
        "Ilke",
        "Lorusso",
        "Belldas",
        "Tibold",
        "Hersh",
        "Nadabas",
        "Clance",
        "Jansson",
        "Jem",
        "Baler",
        "Werbel",
        "Osborn",
        "Becket",
        "Lilith",
        "Millian",
        "Phipps",
        "Astrix",
        "Mandych",
    };
    
    public static string GenerateCEOName()
    {
        return $"{FirstNames[Global.Random.Next(FirstNames.Length)]} {LastNames[Global.Random.Next(LastNames.Length)]}";
    }
}