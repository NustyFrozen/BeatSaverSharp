using System;
using BeatSaverSharp;
using System.Collections.Generic;

namespace BeatSaverSharp_test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BeatSaverAPI API = new BeatSaverAPI("Test", "1.0.0");
                Console.WriteLine("Testing getMapAsync(string): ");
                Beatmap map = API.getMapbyKeyAsync("1add6").Result;
                printMap(map);
                Console.WriteLine("Testing search(string,int): ");
                map = API.getMapbyHashAsync("61856079A53AB096C2943A4CA32638AAB72EDFF5").Result;
                Console.WriteLine(map.name);
                Console.WriteLine(API.getMapperDetailsAsync(4284317).Result.name);
                Console.WriteLine(API.getMapsByMappersAsync(4284317, 0).Result[0].name);
                Console.WriteLine(API.getPlaylistByMapper(4284317).Result.playlistTitle);
                Console.WriteLine(API.getSongLeaderboard("61856079A53AB096C2943A4CA32638AAB72EDFF5", 1, Difficulty.expertPlus, 0));
                Console.WriteLine(API.searchAsync("reality check", 0).Result[0].name);
                Console.WriteLine(API.searchAsync("reality check", 0, sortOrder.Rating).Result[0].name);
                Console.WriteLine(API.searchAsync("Ov Sacrament and Sincest", 0, sortOrder.Relevance, new filters[] { filters.ranked }).Result[0].name);
                Console.WriteLine(API.searchAsync("reality check", 0, sortOrder.Relevance, new beatsaverDateFormat(1, 1, 2018), new beatsaverDateFormat(1, 8, 2021)).Result[0].name);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
        public static void printMap(Beatmap map)
        {
            Console.WriteLine(" ");
            Console.WriteLine("map name: " + map.name);
            Console.WriteLine("map description: " + map.description);
            Console.WriteLine("key : " + map.id);
            Console.WriteLine("ranked: " + map.ranked);
        }
    }
}
