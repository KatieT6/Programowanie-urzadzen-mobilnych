using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCommon;

public static class BookTypeMapping
{
    private static Dictionary<string, BookType> stringToEnum = new Dictionary<string, BookType>
    {
        { "Sci-Fi", BookType.SciFi },
        { "Fantasy", BookType.Fantasy },
        { "Romance", BookType.Romance },
        { "Horror", BookType.Horror },
        { "Thriller", BookType.Thriller },
        { "Mystery", BookType.Mystery },
        { "Non-Fiction", BookType.NonFiction },
        { "Historical", BookType.Historical },
        { "Biography", BookType.Biography },
        { "Poetry", BookType.Poetry },
        { "None", BookType.None }
    };

    private static Dictionary<BookType, string> enumToString = new Dictionary<BookType, string>
    {
        { BookType.SciFi, "Sci-Fi" },
        { BookType.Fantasy, "Fantasy" },
        { BookType.Romance, "Romance" },
        { BookType.Horror, "Horror" },
        { BookType.Thriller, "Thriller" },
        { BookType.Mystery, "Mystery" },
        { BookType.NonFiction, "Non-Fiction" },
        { BookType.Historical, "Historical" },
        { BookType.Biography, "Biography" },
        { BookType.Poetry, "Poetry" },
        { BookType.None, "None" }
    };

    public static BookType StringToBookType(string type)
    {
        if (stringToEnum.TryGetValue(type, out var bookType)) return bookType;
        return BookType.None;
    }

    public static string BookTypeToString(BookType type)
    {
        return enumToString[type];
    }
}
