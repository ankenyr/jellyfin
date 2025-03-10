﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Emby.Server.Implementations.Localization;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Localization
{
    public class LocalizationManagerTests
    {
        [Fact]
        public void GetCountries_All_Success()
        {
            var localizationManager = Setup(new ServerConfiguration
            {
                UICulture = "de-DE"
            });
            var countries = localizationManager.GetCountries().ToList();

            Assert.Equal(139, countries.Count);

            var germany = countries.FirstOrDefault(x => x.Name.Equals("DE", StringComparison.Ordinal));
            Assert.NotNull(germany);
            Assert.Equal("Germany", germany!.DisplayName);
            Assert.Equal("DEU", germany.ThreeLetterISORegionName);
            Assert.Equal("DE", germany.TwoLetterISORegionName);
        }

        [Fact]
        public async Task GetCultures_All_Success()
        {
            var localizationManager = Setup(new ServerConfiguration
            {
                UICulture = "de-DE"
            });
            await localizationManager.LoadAll();
            var cultures = localizationManager.GetCultures().ToList();

            Assert.Equal(189, cultures.Count);

            var germany = cultures.FirstOrDefault(x => x.TwoLetterISOLanguageName.Equals("de", StringComparison.Ordinal));
            Assert.NotNull(germany);
            Assert.Equal("ger", germany!.ThreeLetterISOLanguageName);
            Assert.Equal("German", germany.DisplayName);
            Assert.Equal("German", germany.Name);
            Assert.Contains("deu", germany.ThreeLetterISOLanguageNames);
            Assert.Contains("ger", germany.ThreeLetterISOLanguageNames);
        }

        [Theory]
        [InlineData("de")]
        [InlineData("ger")]
        [InlineData("german")]
        public async Task FindLanguageInfo_Valid_Success(string identifier)
        {
            var localizationManager = Setup(new ServerConfiguration
            {
                UICulture = "de-DE"
            });
            await localizationManager.LoadAll();

            var germany = localizationManager.FindLanguageInfo(identifier);
            Assert.NotNull(germany);

            Assert.Equal("ger", germany!.ThreeLetterISOLanguageName);
            Assert.Equal("German", germany.DisplayName);
            Assert.Equal("German", germany.Name);
            Assert.Contains("deu", germany.ThreeLetterISOLanguageNames);
            Assert.Contains("ger", germany.ThreeLetterISOLanguageNames);
        }

        [Fact]
        public async Task GetParentalRatings_Default_Success()
        {
            var localizationManager = Setup(new ServerConfiguration
            {
                UICulture = "de-DE"
            });
            await localizationManager.LoadAll();
            var ratings = localizationManager.GetParentalRatings().ToList();

            Assert.Equal(23, ratings.Count);

            var tvma = ratings.FirstOrDefault(x => x.Name.Equals("TV-MA", StringComparison.Ordinal));
            Assert.NotNull(tvma);
            Assert.Equal(9, tvma!.Value);
        }

        [Fact]
        public async Task GetParentalRatings_ConfiguredCountryCode_Success()
        {
            var localizationManager = Setup(new ServerConfiguration()
            {
                MetadataCountryCode = "DE"
            });
            await localizationManager.LoadAll();
            var ratings = localizationManager.GetParentalRatings().ToList();

            Assert.Equal(10, ratings.Count);

            var fsk = ratings.FirstOrDefault(x => x.Name.Equals("FSK-12", StringComparison.Ordinal));
            Assert.NotNull(fsk);
            Assert.Equal(7, fsk!.Value);
        }

        [Theory]
        [InlineData("CA-R", "CA", 10)]
        [InlineData("FSK-16", "DE", 8)]
        [InlineData("FSK-18", "DE", 9)]
        [InlineData("FSK-18", "US", 9)]
        [InlineData("TV-MA", "US", 9)]
        [InlineData("XXX", "asdf", 100)]
        [InlineData("Germany: FSK-18", "DE", 9)]
        public async Task GetRatingLevel_GivenValidString_Success(string value, string countryCode, int expectedLevel)
        {
            var localizationManager = Setup(new ServerConfiguration()
            {
                MetadataCountryCode = countryCode
            });
            await localizationManager.LoadAll();
            var level = localizationManager.GetRatingLevel(value);
            Assert.NotNull(level);
            Assert.Equal(expectedLevel, level!);
        }

        [Fact]
        public async Task GetRatingLevel_GivenUnratedString_Success()
        {
            var localizationManager = Setup(new ServerConfiguration()
            {
                UICulture = "de-DE"
            });
            await localizationManager.LoadAll();
            Assert.Null(localizationManager.GetRatingLevel("n/a"));
        }

        [Theory]
        [InlineData("Default", "Default")]
        [InlineData("HeaderLiveTV", "Live TV")]
        public void GetLocalizedString_Valid_Success(string key, string expected)
        {
            var localizationManager = Setup(new ServerConfiguration()
            {
                UICulture = "en-US"
            });

            var translated = localizationManager.GetLocalizedString(key);
            Assert.NotNull(translated);
            Assert.Equal(expected, translated);
        }

        [Fact]
        public void GetLocalizedString_Invalid_Success()
        {
            var localizationManager = Setup(new ServerConfiguration()
            {
                UICulture = "en-US"
            });

            var key = "SuperInvalidTranslationKeyThatWillNeverBeAdded";

            var translated = localizationManager.GetLocalizedString(key);
            Assert.NotNull(translated);
            Assert.Equal(key, translated);
        }

        private LocalizationManager Setup(ServerConfiguration config)
        {
            var mockConfiguration = new Mock<IServerConfigurationManager>();
            mockConfiguration.SetupGet(x => x.Configuration).Returns(config);

            return new LocalizationManager(mockConfiguration.Object, new NullLogger<LocalizationManager>());
        }
    }
}
