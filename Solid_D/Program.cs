using System.Text.Json;

string origin = @"C:\Users....";
string dbPath = @"C:\Users....";


IInfo info = new InfoByRequest(origin);  /*  <-      */

//Monitor monitor = new Monitor(origin);    /*  <-      */
Monitor monitor = new Monitor(origin, info);

await monitor.Show();

//FileDB fileDB = new FileDB(dbPath, origin);    /*  <-      */
FileDB fileDB = new FileDB(dbPath, origin, info);
await fileDB.Save();    


public class Monitor
{
    private string _origin;
    private IInfo _info;

    public Monitor(string origin, IInfo info)   /*  <-      */
    {
        _origin = origin;
        _info = info;
    }

    public async Task Show()
    {
        /* InfoByFile info = new InfoByFile(_origin);*/  /*<- Existe una dependencia ya que se crea dentro del metodo*/
        //Borramos el constructor y lo transformamos en llamada de Interface para eliminar la dependencia

        //var posts = await info.Get();
        var posts = await _info.Get();      /*  <-      */
        foreach (var post in posts)
            Console.WriteLine(post.title);
    }
}

public class FileDB
{
    private string _path;
    private string _origin;
    private IInfo _info;

    public FileDB(string path, string origin, IInfo info)
    {
        _path = path;
        _origin = origin;
        _info = info;
    }

    public async Task Save()
    {
        //InfoByFile info = new InfoByFile(_origin);

        var posts = await _info.Get();
        string json = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(_path, json);
    }
}



public class InfoByFile : IInfo
{
    private string _path;
    public InfoByFile(string path)
    {
        _path = path;
    }

    public async Task<IEnumerable<Post>> Get()
    {
        var contentStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
        IEnumerable<Post> posts =
            await JsonSerializer.DeserializeAsync<IEnumerable<Post>>(contentStream);
        return posts;
    }
}

public class InfoByRequest : IInfo
{
    private string _url;
    public InfoByRequest(string url)    {
        _url = url;
    }

    public async Task<IEnumerable<Post>> Get()
    {
        HttpClient httpClient = new HttpClient();
        var response = await httpClient.GetAsync(_url);
        var stream = await response.Content.ReadAsStreamAsync();
        List<Post> posts = await JsonSerializer.DeserializeAsync<List<Post>>(stream);

        return posts;
    }

}


public interface IInfo
{
    public Task<IEnumerable<Post>> Get();
}

public class Post
{
    public int userId { get; set; }
    public int Id { get; set; }
    public string title { get; set; }
    public bool completed { get; set; }
}
