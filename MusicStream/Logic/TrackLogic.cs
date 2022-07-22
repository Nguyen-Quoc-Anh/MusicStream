using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MusicStream.Logic;
using MusicStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Controllers.Logic
{
    public class TrackLogic
    {
        public static Dictionary<Track, List<Artist>> GetFeatureTrack()
        {
            Dictionary<Track, List<Artist>> tracks = new Dictionary<Track, List<Artist>>();
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
                    List<Artist> Artists = new List<Artist>();
                    Artists = context.Artists.Join(context.ArtistTracks.Where(at => at.TrackId == track.TrackId),
                        t => t.ArtistId,
                        a => a.ArtistId,
                        (t, a) => t
                        ).ToList();
                    tracks.Add(track, Artists);
                });
            }
            return tracks;
        }

        public static Dictionary<Track, List<Artist>> GetTrackByAlbum(string AlbumID)
        {
            Dictionary<Track, List<Artist>> tracks = new Dictionary<Track, List<Artist>>();
            using (var context = new MusicStreamingContext())
            {
                List<Track> trackList = context.Tracks.Join(context.Albums.Where(a => a.AlbumId == AlbumID),
                                                            t => t.AlbumId,
                                                            a => a.AlbumId,
                                                            (t, a) => t
                                                            ).ToList();

                trackList.ForEach(track =>
                {
                    List<Artist> Artists = new List<Artist>();
                    Artists = context.Artists.Join(context.ArtistTracks.Where(at => at.TrackId == track.TrackId),
                        t => t.ArtistId,
                        a => a.ArtistId,
                        (t, a) => t
                        ).ToList();
                    tracks.Add(track, Artists);
                });
            }
            return tracks;
        }

        public static List<Track> GetTrackListByAlbum(string AlbumID)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Tracks.Include(t => t.Album)
                    .Include(t => t.ArtistTracks).ThenInclude(t => t.Artist)
                    .Where(t => t.AlbumId == AlbumID).ToList();
            }
        }

        public static List<Track> GetTrackListByPlayList(string playListId)
        {
            using (var context = new MusicStreamingContext())
            {
                List<string> tracks = context.Tracks.Include(t => t.PlayListTracks)
                    .Where(p => p.PlayListTracks.Any(p => p.PlaylistId == playListId)).Select(t => t.TrackId).ToList();
                return context.Tracks.Include(t => t.Album)
                    .Include(t => t.ArtistTracks).ThenInclude(t => t.Artist)
                    .Where(t => tracks.Contains(t.TrackId)).ToList();
            }
        }

        public static Dictionary<Track, List<Artist>> GetMostPopularTrack()
        {
            Dictionary<Track, List<Artist>> tracks = new Dictionary<Track, List<Artist>>();
            using (var context = new MusicStreamingContext())
            {
                List<Track> trackList = context.Tracks.OrderByDescending(t => t.View).Take(5).ToList();
                trackList.ForEach(track =>
                {
                    List<Artist> Artists = new List<Artist>();
                    Artists = context.Artists.Join(context.ArtistTracks.Where(at => at.TrackId == track.TrackId),
                        t => t.ArtistId,
                        a => a.ArtistId,
                        (t, a) => t
                        ).ToList();
                    tracks.Add(track, Artists);
                });
            }
            return tracks;
        }

        public static List<Track> GetListTrackByFilter(string[] artists, string[] genres, string key, string sortby)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Tracks.Include(t => t.ArtistTracks).ThenInclude(t => t.Artist)
                    .Include(t => t.Album)
                    .Where(t => t.Name.Contains(key.Trim()) && (artists.Length != 0 ? t.ArtistTracks.Any(s => artists.Contains(s.ArtistId)) : true) && (genres.Length != 0 ? t.GenreOfTracks.Any(g => genres.Contains(g.GenreId)) : true))
                    .OrderByDescending(t => sortby.Equals("popular") ? t.View : t.Album.ReleaseDate.Value.Year)
                    .ThenByDescending(t => sortby.Equals("popular") ? t.View : t.Album.ReleaseDate.Value.Month)
                    .ThenByDescending(t => sortby.Equals("popular") ? t.View : t.Album.ReleaseDate.Value.Day)
                    .ToList();
            }
        }

        public static List<Track> GetFavouriteListTrack(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Tracks.Include(t => t.ArtistTracks).ThenInclude(t => t.Artist)
                    .Include(t => t.Album).Include(t => t.LikeTracks)
                    .Where(t => t.LikeTracks.Any(tr => tr.AccountId == Id)).ToList();
            }
        }

        public static bool LikeTrack(string accountId, string trackId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    LikeTrack like = new LikeTrack();
                    like.AccountId = accountId;
                    like.TrackId = trackId;
                    context.LikeTracks.Add(like);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool UnLikeTrack(string accountId, string trackId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    LikeTrack like = new LikeTrack();
                    like.AccountId = accountId;
                    like.TrackId = trackId;
                    context.LikeTracks.Remove(like);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool CheckAccountLikedTrack(string accountId, string trackId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.LikeTracks.Any(t => t.AccountId == accountId && t.TrackId == trackId);
            }
        }

        public static bool CheckTrackExistById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Tracks.Any(t => t.TrackId.Equals(Id));
            }
        }

        public static void IncreaseListens(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                Track track = context.Tracks.FirstOrDefault(t => t.TrackId == Id);
                track.View++;
                context.Tracks.Update(track);
                context.SaveChanges();
            }
        }

        public static bool InsertTrack(Track track, string[] artists, string[] genres)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Tracks.Add(track);
                    context.SaveChanges();
                    artists.ToList().ForEach(artist =>
                    {
                        ArtistTrack artistTrack = new ArtistTrack();
                        artistTrack.ArtistId = artist;
                        artistTrack.TrackId = track.TrackId;
                        context.ArtistTracks.Add(artistTrack);
                    });
                    genres.ToList().ForEach(genre =>
                    {
                        GenreOfTrack genreOfTrack = new GenreOfTrack();
                        genreOfTrack.GenreId = genre;
                        genreOfTrack.TrackId = track.TrackId;
                        context.GenreOfTracks.Add(genreOfTrack);
                    });
                    context.SaveChanges();
                    AlbumLogic.ModifyAlbumArtist(track.AlbumId, artists);
                    AlbumLogic.ModifyAlbumGenre(track.AlbumId, genres);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool DeleteTrack(string Id, IWebHostEnvironment webHostEnvironment)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    Track track = context.Tracks.FirstOrDefault(t => t.TrackId == Id);
                    context.LikeTracks.RemoveRange(context.LikeTracks.Where(t => t.TrackId == Id));
                    context.ArtistTracks.RemoveRange(context.ArtistTracks.Where(t => t.TrackId == Id));
                    context.GenreOfTracks.RemoveRange(context.GenreOfTracks.Where(t => t.TrackId == Id));
                    context.PlayListTracks.RemoveRange(context.PlayListTracks.Where(t => t.TrackId == Id));
                    context.Tracks.Remove(track);
                    context.SaveChanges();
                    if (!track.Image.Contains("img/album") && !track.Image.Contains("http"))
                    {
                        Util.DeleteFile(webHostEnvironment, track.Image.Split('/').Last(), "img/track");
                    }
                    Util.DeleteFile(webHostEnvironment, track.Mp3.Split('/').Last(), "mp3/");
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static Track GetTrackByID(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Tracks.Include(t => t.ArtistTracks)
                    .Include(t => t.Album).Include(t => t.GenreOfTracks)
                    .FirstOrDefault(t => t.TrackId == Id);
            }
        }

        public static bool UpdateTrack(Track track, string[] genres, string[] artists)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Tracks.Update(track);
                    context.SaveChanges();
                    context.GenreOfTracks.RemoveRange(context.GenreOfTracks.Where(t => t.TrackId == track.TrackId));
                    genres.ToList().ForEach(genre =>
                    {
                        GenreOfTrack genreOfTrack = new GenreOfTrack();
                        genreOfTrack.GenreId = genre;
                        genreOfTrack.TrackId = track.TrackId;
                        context.GenreOfTracks.Add(genreOfTrack);
                    });
                    context.SaveChanges();
                    context.ArtistTracks.RemoveRange(context.ArtistTracks.Where(t => t.TrackId == track.TrackId));
                    artists.ToList().ForEach(artist =>
                    {
                        ArtistTrack artistTrack = new ArtistTrack();
                        artistTrack.ArtistId = artist;
                        artistTrack.TrackId = track.TrackId;
                        context.ArtistTracks.Add(artistTrack);
                    });
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
