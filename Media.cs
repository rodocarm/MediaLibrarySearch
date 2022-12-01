using System;
using System.Collections.Generic;

namespace MediaLibrarySearch
{
    public abstract class Media
    {
        public UInt64 mediaId { get; set; }
        public string title { get; set; }
        public List<string> genres { get; set; }

        public Media()
        {
            genres = new List<string>();
        }

        public virtual string Display()
        {
            return $"Id: {mediaId}\nTitle: {title}\nGenres: {string.Join(", ", genres)}\n";
        }
    }

    public class Movie : Media
    {
        public string director { get; set; }
        public TimeSpan runningTime { get; set; }

        public override string Display()
        {
            return $"Id: {mediaId}\nTitle: {title}\nDirector: {director}\nRun time: {runningTime}\nGenres: {string.Join(", ", genres)}\n";
        }
    }

    public class Album : Media
    {
        public string artist { get; set; }
        public string recordLabel { get; set; }

        public override string Display()
        {
            return $"Id: {mediaId}\nTitle: {title}\nArtist: {artist}\nLabel: {recordLabel}\nGenres: {string.Join(", ", genres)}\n";
        }
    }

    public class Book : Media
    {
        public string author { get; set; }
        public UInt16 pageCount { get; set; }
        public string publisher { get; set; }

        public override string Display()
        {
            return $"Id: {mediaId}\nTitle: {title}\nAuthor: {author}\nPages: {pageCount}\nPublisher: {publisher}\nGenres: {string.Join(", ", genres)}\n";
        }
    }
}