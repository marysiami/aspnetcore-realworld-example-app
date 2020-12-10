using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Conduit.Domain
{
    public class Article
    {
        public int ArticleId { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public Person Author { get; set; }

        public List<Comment> Comments { get; set; }

        public bool IsBanned { get; set; } = false;

        public bool AfterAdminReview { get; set; } = false;

        [NotMapped]
        public bool Favorited => ArticleFavorites?.Any() ?? false;

        [NotMapped]
        public int FavoritesCount => ArticleFavorites?.Count ?? 0;

        [NotMapped]
        public List<string> TagList => (ArticleTags?.Select(x => x.TagId) ?? Enumerable.Empty<string>()).ToList();

        [JsonIgnore]
        public List<ArticleTag> ArticleTags { get; set; }

        [JsonIgnore]
        public List<ArticleFavorite> ArticleFavorites { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsReported { get; set; } = false;
    }
}