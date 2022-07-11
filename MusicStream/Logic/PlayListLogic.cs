using Microsoft.EntityFrameworkCore;
using MusicStream.Extensions;
using MusicStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStream.Logic
{
    public class PlayListLogic
    {
        public static List<Playlist> GetAllPlayListByAccountId(string accountId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Playlists.Where(pl => pl.AccountId == accountId).ToList();
            }
        }

        public static bool CheckTrackIsExistInPlaylist(string playListId, string trackId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.PlayListTracks.Any(pl => pl.PlaylistId == playListId && pl.TrackId == trackId);
            }
        }

        public static bool AddToPlayList(string playListId, string trackId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    PlayListTrack like = new PlayListTrack();
                    like.PlaylistId = playListId;
                    like.TrackId = trackId;
                    context.PlayListTracks.Add(like);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool RemoveFromPlayList(string playListId, string trackId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    PlayListTrack plt = new PlayListTrack();
                    plt.PlaylistId = playListId;
                    plt.TrackId = trackId;
                    context.PlayListTracks.Remove(plt);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static List<Playlist> GetAllPlayListAndFollowPlayListByAccountId(string accountId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Playlists.Include(p => p.PlayListFollows).ThenInclude(p => p.Account)
                    .Include(p => p.Account)
                    .Where(p => p.AccountId == accountId || p.PlayListFollows.Any(pl => pl.AccountId == accountId)).ToList();
            }
        }


        public static Playlist GetPlaylistById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Playlists.Include(p => p.PlayListTracks)
                    .ThenInclude(t => t.Track).ThenInclude(t => t.ArtistTracks)
                    .ThenInclude(t => t.Artist).Include(p => p.PlayListFollows).
                    FirstOrDefault(p => p.PlaylistId == Id && p.IsPrivate == false);
            }
        }

        public static bool AddNewPlayList(Playlist playlist)
        {
            using (var context = new MusicStreamingContext())
            {
                context.Playlists.Add(playlist);
                context.SaveChanges();
                return true;
            }
        }

        public static bool CheckPlayListIdIsExist(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Playlists.Any(p => p.PlaylistId == Id);
            }
        }

        public static async Task<bool> DeletePlayListById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.PlayListFollows.RemoveRange(context.PlayListFollows.Where(p => p.PlaylistId == Id));
                    context.PlayListTracks.RemoveRange(context.PlayListTracks.Where(p => p.PlaylistId == Id));
                    await context.SaveChangesAsync();
                    context.Playlists.Remove(context.Playlists.FirstOrDefault(p => p.PlaylistId == Id));
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static List<Playlist> GetRandomPlayList()
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Playlists.Include(p => p.Account)
                    .Where(p => p.IsPrivate == false)
                    .ToList().PickRandom(10).ToList();
            }
        }

        public static List<Playlist> GetPlaylistsBySearch(string name, string sort)
        {
            using (var context = new MusicStreamingContext())
            {
                List<Playlist> playlists = context.Playlists.Include(p => p.Account).Where(p => p.Name.ToLower().Contains(name) && p.IsPrivate == false).ToList();
                if (sort.Equals("popular"))
                {
                    playlists = playlists.OrderByDescending(p => p.PlayListFollows.Where(pf => pf.PlaylistId == p.PlaylistId).Count())
                        .ToList();
                }
                else if (sort.Equals("newest"))
                {
                    playlists = playlists.OrderByDescending(p => p.CreatedTime).ToList();
                }
                return playlists;
            }
        }

        public static bool IsAccountFollowPlaylist(string accountId, string playlistId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.PlayListFollows.Any(p => p.AccountId == accountId && p.PlaylistId == playlistId);
            }
        }

        public static bool FollowPlaylist(string accountId, string playlistId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    PlayListFollow playListFollow = new PlayListFollow() { AccountId = accountId, PlaylistId = playlistId };
                    context.PlayListFollows.Add(playListFollow);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool UnFollowPlaylist(string accountId, string playlistId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    PlayListFollow playListFollow = new PlayListFollow() { AccountId = accountId, PlaylistId = playlistId };
                    context.PlayListFollows.Remove(playListFollow);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool EditPlaylist(Playlist playlist)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Playlists.Update(playlist);
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
