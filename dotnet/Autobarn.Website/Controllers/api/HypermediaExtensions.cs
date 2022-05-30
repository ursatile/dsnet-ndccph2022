using System.Dynamic;

namespace Autobarn.Website.Controllers.api {
    public class Hal {
        public static dynamic Paginate(string url, int index, int count, int total) {
            dynamic links = new ExpandoObject();
            links.self = new { href = $"{url}?index={index}&count={count}" };
            if (index > 0) {
                links.previous = new { href = $"{url}?index={index - count}&count={count}" };
                links.first = new { href = $"{url}?index=0&count={count}" };
            }

            if (index + count < total) {
                links.next = new { href = $"{url}?index={index + count}&count={count}" };
                links.final = new { href = $"{url}?index={index + count % index}&count={count}" };
            }

            return links;
        }
    }
}
