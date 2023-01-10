using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.TraxMachine
{
    public class TraxSoundManager
    {
        public static List<TraxMusicData> Songs = new List<TraxMusicData>();

        public static void Init()
        {
            TraxSoundManager.Songs.Clear();
            DataTable table;
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunQuery("SELECT * FROM jukebox_songs_data");
                table = queryReactor.GetTable();
            }
            foreach (DataRow row in (InternalDataCollectionBase)table.Rows)
                TraxSoundManager.Songs.Add(TraxMusicData.Parse(row));
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss | ") + "» " + TraxSoundManager.Songs.Count.ToString() + " Jukebox Cargada");
        }

        public static TraxMusicData GetMusic(int id)
        {
            foreach (TraxMusicData song in TraxSoundManager.Songs)
            {
                if (song.Id == id)
                    return song;
            }
            return (TraxMusicData)null;
        }
    }
}
