using System.Dynamic;

namespace Autobarn.Website.Controllers.api {
    public class Hal {
        public static dynamic Paginate(string url, int index, int count, int total) {
            dynamic links = new ExpandoObject();
            links.self = new { href = url };
            links.final = new { href = $"{url}?index={total - (total % count)}&count={count}" };
            links.first = new { href = $"{url}?index=0&count={count}" };
            if (index > 0) links.previous = new { href = $"{url}?index={index - count}&count={count}" };
            if (index + count < total) links.next = new { href = $"{url}?index={index + count}&count={count}" };
            return links;
        }

        public static dynamic PaginateByLetter(string url, char regStart) {
            dynamic links = new ExpandoObject();
            links.self = new { href = url };
            links.first = new { href = $"{url}?regStart=z" };
            links.first = new { href = $"{url}?regStart=a" };
            if (regStart != 'a') links.previous = new { href = $"{url}?regStart={(char)(((int)regStart) - 1)}" };
            if (regStart != 'z') links.next = new { href = $"{url}?regStart={(char)(((int)regStart) + 1)}" };
            return links;
        }
    }
}

