#nullable disable

#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.System;

namespace MediaBrowser.Controller.MediaEncoding
{
    /// <summary>
    /// Interface IMediaEncoder.
    /// </summary>
    public interface IMediaEncoder : ITranscoderSupport
    {
        /// <summary>
        /// Gets location of the discovered FFmpeg tool.
        /// </summary>
        FFmpegLocation EncoderLocation { get; }

        /// <summary>
        /// Gets the encoder path.
        /// </summary>
        /// <value>The encoder path.</value>
        string EncoderPath { get; }

        /// <summary>
        /// Whether given encoder codec is supported.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SupportsEncoder(string encoder);

        /// <summary>
        /// Whether given decoder codec is supported.
        /// </summary>
        /// <param name="decoder">The decoder.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SupportsDecoder(string decoder);

        /// <summary>
        /// Whether given hardware acceleration type is supported.
        /// </summary>
        /// <param name="hwaccel">The hwaccel.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SupportsHwaccel(string hwaccel);

        /// <summary>
        /// Whether given filter is supported.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if the filter is supported, <c>false</c> otherwise.</returns>
        bool SupportsFilter(string filter, string option);

        /// <summary>
        /// Extracts the audio image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="imageStreamIndex">Index of the image stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{Stream}.</returns>
        Task<string> ExtractAudioImage(string path, int? imageStreamIndex, CancellationToken cancellationToken);

        /// <summary>
        /// Extracts the video image.
        /// </summary>
        /// <param name="inputFile">Input file.</param>
        /// <param name="container">Video container type.</param>
        /// <param name="mediaSource">Media source information.</param>
        /// <param name="videoStream">Media stream information.</param>
        /// <param name="threedFormat">Video 3D format.</param>
        /// <param name="offset">Time offset.</param>
        /// <param name="cancellationToken">CancellationToken to use for operation.</param>
        /// <returns>Location of video image.</returns>
        Task<string> ExtractVideoImage(string inputFile, string container, MediaSourceInfo mediaSource, MediaStream videoStream, Video3DFormat? threedFormat, TimeSpan? offset, CancellationToken cancellationToken);

        /// <summary>
        /// Extracts the video image.
        /// </summary>
        /// <param name="inputFile">Input file.</param>
        /// <param name="container">Video container type.</param>
        /// <param name="mediaSource">Media source information.</param>
        /// <param name="imageStream">Media stream information.</param>
        /// <param name="imageStreamIndex">Index of the stream to extract from.</param>
        /// <param name="cancellationToken">CancellationToken to use for operation.</param>
        /// <returns>Location of video image.</returns>
        Task<string> ExtractVideoImage(string inputFile, string container, MediaSourceInfo mediaSource, MediaStream imageStream, int? imageStreamIndex, CancellationToken cancellationToken);

        /// <summary>
        /// Extracts the video images on interval.
        /// </summary>
        /// <param name="inputFile">Input file.</param>
        /// <param name="container">Video container type.</param>
        /// <param name="videoStream">Media stream information.</param>
        /// <param name="mediaSource">Media source information.</param>
        /// <param name="threedFormat">Video 3D format.</param>
        /// <param name="interval">Time interval.</param>
        /// <param name="targetDirectory">Directory to write images.</param>
        /// <param name="filenamePrefix">Filename prefix to use.</param>
        /// <param name="maxWidth">Maximum width of image.</param>
        /// <param name="cancellationToken">CancellationToken to use for operation.</param>
        /// <returns>A task.</returns>
        Task ExtractVideoImagesOnInterval(
            string inputFile,
            string container,
            MediaStream videoStream,
            MediaSourceInfo mediaSource,
            Video3DFormat? threedFormat,
            TimeSpan interval,
            string targetDirectory,
            string filenamePrefix,
            int? maxWidth,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the media info.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task<MediaInfo> GetMediaInfo(MediaInfoRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the input argument.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="mediaSource">The mediaSource.</param>
        /// <returns>System.String.</returns>
        string GetInputArgument(string inputFile, MediaSourceInfo mediaSource);

        /// <summary>
        /// Gets the time parameter.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>System.String.</returns>
        string GetTimeParameter(long ticks);

        Task ConvertImage(string inputPath, string outputPath);

        /// <summary>
        /// Escapes the subtitle filter path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        string EscapeSubtitleFilterPath(string path);

        /// <summary>
        /// Sets the path to find FFmpeg.
        /// </summary>
        void SetFFmpegPath();

        /// <summary>
        /// Updates the encoder path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathType">The type of path.</param>
        void UpdateEncoderPath(string path, string pathType);

        /// <summary>
        /// Gets the primary playlist of .vob files.
        /// </summary>
        /// <param name="path">The to the .vob files.</param>
        /// <param name="titleNumber">The title number to start with.</param>
        /// <returns>A playlist.</returns>
        IEnumerable<string> GetPrimaryPlaylistVobFiles(string path, uint? titleNumber);
    }
}
