#nullable disable

#pragma warning disable CA1002, CA1724, CA1826, CS1591

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Persistence;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace MediaBrowser.Controller.Entities.Audio
{
    /// <summary>
    /// Class Audio.
    /// </summary>
    public class Audio : BaseItem,
        IHasAlbumArtist,
        IHasArtist,
        IHasMusicGenres,
        IHasLookupInfo<SongInfo>,
        IHasMediaSources
    {
        public Audio()
        {
            Artists = Array.Empty<string>();
            AlbumArtists = Array.Empty<string>();
        }

        /// <inheritdoc />
        [JsonIgnore]
        public IReadOnlyList<string> Artists { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public IReadOnlyList<string> AlbumArtists { get; set; }

        [JsonIgnore]
        public override bool SupportsPlayedStatus => true;

        [JsonIgnore]
        public override bool SupportsPeople => false;

        [JsonIgnore]
        public override bool SupportsAddingToPlaylist => true;

        [JsonIgnore]
        public override bool SupportsInheritedParentImages => true;

        [JsonIgnore]
        protected override bool SupportsOwnedItems => false;

        [JsonIgnore]
        public override Folder LatestItemsIndexContainer => AlbumEntity;

        [JsonIgnore]
        public MusicAlbum AlbumEntity => FindParent<MusicAlbum>();

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <value>The type of the media.</value>
        [JsonIgnore]
        public override string MediaType => Model.Entities.MediaType.Audio;

        public override double GetDefaultPrimaryImageAspectRatio()
        {
            return 1;
        }

        public override bool CanDownload()
        {
            return IsFileProtocol;
        }

        /// <summary>
        /// Creates the name of the sort.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string CreateSortName()
        {
            return (ParentIndexNumber != null ? ParentIndexNumber.Value.ToString("0000 - ", CultureInfo.InvariantCulture) : string.Empty)
                    + (IndexNumber != null ? IndexNumber.Value.ToString("0000 - ", CultureInfo.InvariantCulture) : string.Empty) + Name;
        }

        public override List<string> GetUserDataKeys()
        {
            var list = base.GetUserDataKeys();

            var songKey = IndexNumber.HasValue ? IndexNumber.Value.ToString("0000", CultureInfo.InvariantCulture) : string.Empty;

            if (ParentIndexNumber.HasValue)
            {
                songKey = ParentIndexNumber.Value.ToString("0000", CultureInfo.InvariantCulture) + "-" + songKey;
            }

            songKey += Name;

            if (!string.IsNullOrEmpty(Album))
            {
                songKey = Album + "-" + songKey;
            }

            var albumArtist = AlbumArtists.FirstOrDefault();
            if (!string.IsNullOrEmpty(albumArtist))
            {
                songKey = albumArtist + "-" + songKey;
            }

            list.Insert(0, songKey);

            return list;
        }

        public override UnratedItem GetBlockUnratedType()
        {
            if (SourceType == SourceType.Library)
            {
                return UnratedItem.Music;
            }

            return base.GetBlockUnratedType();
        }

        public List<MediaStream> GetMediaStreams(MediaStreamType type)
        {
            return MediaSourceManager.GetMediaStreams(new MediaStreamQuery
            {
                ItemId = Id,
                Type = type
            });
        }

        public SongInfo GetLookupInfo()
        {
            var info = GetItemLookupInfo<SongInfo>();

            info.AlbumArtists = AlbumArtists;
            info.Album = Album;
            info.Artists = Artists;

            return info;
        }

        protected override List<Tuple<BaseItem, MediaSourceType>> GetAllItemsForMediaSources()
        {
            var list = new List<Tuple<BaseItem, MediaSourceType>>();
            list.Add(new Tuple<BaseItem, MediaSourceType>(this, MediaSourceType.Default));
            return list;
        }
    }
}
