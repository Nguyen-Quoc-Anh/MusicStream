using MusicStream.Models;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Controllers.Logic
{
    public class TrackLogic
    {
        public static Dictionary<Track, List<Account>> GetFeatureTrack()
        {
            Dictionary<Track, List<Account>> tracks = new Dictionary<Track, List<Account>>();
            using (var context = new MusicStreamingContext())
            {
                List<Track> trackList = context.Tracks.Join(context.Albums.OrderByDescending(al => al.ReleaseDate).Take(5),
                                                            t => t.AlbumId,
                                                            a => a.AlbumId,
                                                            (t, a) => t
                                                            )
                                                    .Take(5).ToList();


                trackList.ForEach(track =>
                {
                    List<Account> accounts = new List<Account>();
                    accounts = context.Accounts.Join(context.ArtistTracks.Where(at => at.TrackId == track.TrackId),
                        t => t.AccountId,
                        a => a.AccountId,
                        (t, a) => t
                        ).ToList();
                    tracks.Add(track, accounts);
                });
            }
            return tracks;
        }

        public static Dictionary<Track, List<Account>> GetTrackByAlbum(string AlbumID)
        {
            Dictionary<Track, List<Account>> tracks = new Dictionary<Track, List<Account>>();
            using (var context = new MusicStreamingContext())
            {
                List<Track> trackList = context.Tracks.Join(context.Albums.Where(a => a.AlbumId == AlbumID),
                                                            t => t.AlbumId,
                                                            a => a.AlbumId,
                                                            (t, a) => t
                                                            ).ToList();

                trackList.ForEach(track =>
                {
                    List<Account> accounts = new List<Account>();
                    accounts = context.Accounts.Join(context.ArtistTracks.Where(at => at.TrackId == track.TrackId),
                        t => t.AccountId,
                        a => a.AccountId,
                        (t, a) => t
                        ).ToList();
                    tracks.Add(track, accounts);
                });
            }
            return tracks;
        }

        public static Dictionary<Track, List<Account>> GetMostPopularTrack()
        {
            Dictionary<Track, List<Account>> tracks = new Dictionary<Track, List<Account>>();
            using (var context = new MusicStreamingContext())
            {
                List<Track> trackList = context.Tracks.OrderByDescending(t => t.View).Take(5).ToList();
                trackList.ForEach(track =>
                {
                    List<Account> accounts = new List<Account>();
                    accounts = context.Accounts.Join(context.ArtistTracks.Where(at => at.TrackId == track.TrackId),
                        t => t.AccountId,
                        a => a.AccountId,
                        (t, a) => t
                        ).ToList();
                    tracks.Add(track, accounts);
                });
            }
            return tracks;
        }
    }
}
