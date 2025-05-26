public class TemplateResolver
{
    public static string ResolveTemplate(string template)
    {
        var replacements = new Dictionary<string, string>
        {
            {  "{date}",    DateTime.Now.ToString("yyyy-MM-dd")},
            {  "{time}",    DateTime.Now.ToString("HH-mm-ss")},
            {  "{user}",    Environment.UserName },
        };

        foreach (var pair in replacements)
        {
            template = template.Replace(pair.Key, pair.Value);
        }

        return template;
    }
}